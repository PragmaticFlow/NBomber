module internal NBomber.DomainServices.Cluster.Agent

open System
open System.Threading.Tasks

open MQTTnet.Client
open FsToolkit.ErrorHandling

open NBomber.Configuration
open NBomber.Domain
open NBomber.Errors
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices.ScenariosHost
open NBomber.DomainServices.Cluster.Contracts
open NBomber.DomainServices.Validation
open Serilog

type State = {    
    ClientId: string
    AgentsTopic: string
    CoordinatorTopic: string
    NodeInfo: ClusterNodeInfo    
    Settings: AgentSettings
    MqttClient: IMqttClient
    ScenariosHost: ScenariosHost
    mutable Working: bool
}

let private subscribeOnTopics (st: State, handle: RequestMessage -> unit) =
    try
        st.MqttClient.SubscribeAsync(st.AgentsTopic).Wait()
        st.MqttClient.UseApplicationMessageReceivedHandler(fun msg -> 
            msg.ApplicationMessage.Payload
            |> Mqtt.deserialize<RequestMessage>
            |> handle        
        )
        |> ignore
    with
    | ex -> Log.Error(ex, "Agent.subscribeOnTopics failed")

let validate (state: State, msg: RequestMessage) =
    match msg.Payload with
    | GetAgentInfo -> Ok msg.Payload
    | NewSession (_, agentSettings, _) ->
        let registeredScenarios =
            state.ScenariosHost.GetRegisteredScenarios() |> Array.map(fun x -> x.ScenarioName)
        
        agentSettings
        |> Array.tryFind(fun x -> state.Settings.TargetGroup = x.TargetGroup)
        |> function
            | Some matchedTargetGroup ->
                ClusterValidation.validateTargetGroupScenarios(
                    matchedTargetGroup.TargetScenarios |> Seq.toArray,
                    registeredScenarios
                )
                |> Result.bind(fun _ -> Ok msg.Payload)
            | None ->
                AppError.createResult(CurrentTargetGroupNotMatched(state.Settings.TargetGroup))            
    | _ -> 
        if msg.Headers.SessionId = state.ScenariosHost.SessionId then
            Ok msg.Payload
        else
            AppError.createResult SessionIsWrong

let receive (st: State, msg: RequestMessage) = asyncResult {

    let getCurrentInfo () = 
        { st.NodeInfo with HostStatus = st.ScenariosHost.Status }

    let sendToCoordinator (st: State) (res: Response) =
        Contracts.createResponseMsg(msg.Headers.CorrelationId, st.ClientId, 
                                    st.ScenariosHost.SessionId, Some res, None)
        |> Mqtt.toMqttMsg(st.CoordinatorTopic)
        |> Mqtt.publishToBroker(st.MqttClient)        

    match! validate(st, msg) with
    | GetAgentInfo ->       
        do! Response.AgentInfo(getCurrentInfo()) |> sendToCoordinator(st)        
    
    | NewSession (scnSettings, agentSettings, customSettings) -> 
        let targetScenarios =
            agentSettings
            |> Array.tryFind(fun x -> x.TargetGroup = st.Settings.TargetGroup)
            |> Option.map(fun x -> x.TargetScenarios)
            |> Option.defaultValue []
            |> Seq.toArray
        
        let scnArgs = {
            SessionId = msg.Headers.SessionId
            ScenariosSettings = scnSettings
            TargetScenarios = targetScenarios
            CustomSettings = customSettings
        } 
        st.ScenariosHost.InitScenarios(scnArgs) |> ignore        
        do! Response.AgentInfo(getCurrentInfo()) |> sendToCoordinator(st)        

    | StartWarmUp ->
        st.ScenariosHost.WarmUpScenarios() |> ignore                
        do! Response.AgentInfo(getCurrentInfo()) |> sendToCoordinator(st)        
            
    | StartBombing ->
        st.ScenariosHost.StartBombing() |> ignore
        do! Response.AgentInfo(getCurrentInfo()) |> sendToCoordinator(st)        

    | GetStatistics ->
        do! Response.AgentStats(st.ScenariosHost.GetStatistics()) |> sendToCoordinator(st)        
}

let init (dep: Dependency, registeredScenarios: Scenario[], settings: AgentSettings) = async {
    
    let clientId = sprintf "agent_%s" (Guid.NewGuid().ToString("N"))
    let! mqttClient = Mqtt.initClient(clientId, settings.MqttServer)

    let nodeInfo = { 
        MachineName = dep.MachineInfo.MachineName
        TargetGroup = settings.TargetGroup
        HostStatus = ScenarioHostStatus.Ready 
    }

    let state = {            
        ClientId = clientId
        AgentsTopic = Contracts.createAgentsTopic(settings.ClusterId)
        CoordinatorTopic = Contracts.createCoordinatorTopic(settings.ClusterId)
        NodeInfo = nodeInfo   
        Settings = settings
        MqttClient = mqttClient
        ScenariosHost = ScenariosHost(dep, registeredScenarios)
        Working = false
    }
        
    Log.Logger.Information("{agent} established connection with {cluster}", state.ClientId, state.Settings.ClusterId)        
        
    return state
}

let startListening (st: State) = async {
    
    st.Working <- true
    let listenTask = TaskCompletionSource<unit>()
    
    let subscribe () =        
        subscribeOnTopics(st, fun msg ->
            receive(st, msg)
            |> AsyncResult.mapError(fun e -> ())
            |> Async.RunSynchronously
            |> ignore
        )
    
    let createReconnectTimer () =
        let mutable reconnecting = false    
        let reconnectionTimer = new System.Timers.Timer(2_000.0)
        
        reconnectionTimer.Elapsed.Add(fun x ->
            
            if not st.Working then
                reconnectionTimer.Stop()
                listenTask.SetResult()
            elif
                not reconnecting && not st.MqttClient.IsConnected then
                reconnecting <- true
                Log.Logger.Error("{agent} disconnected from {cluster}", st.ClientId, st.Settings.ClusterId)
                Mqtt.reconnect(st.MqttClient, None).Wait()
                Log.Logger.Information("{agent} established connection with {cluster}", st.ClientId, st.Settings.ClusterId)
                subscribe()
                reconnecting <- false
        )
        reconnectionTimer

    let timer = createReconnectTimer()
    timer.Start()
    
    subscribe()   
    
    return! listenTask.Task |> Async.AwaitTask
}

let stop (st: State) =
    st.Working <- false
    st.ScenariosHost.StopScenarios()
    st.MqttClient.DisconnectAsync().Wait()
    st.MqttClient.Dispose()