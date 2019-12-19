﻿module internal NBomber.DomainServices.Cluster.Agent

open System
open System.Threading.Tasks

open MQTTnet.Client
open FsToolkit.ErrorHandling

open NBomber.Configuration
open NBomber.Contracts
open NBomber.Domain
open NBomber.Errors
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices.TestHost
open NBomber.DomainServices.Cluster.Contracts
open NBomber.DomainServices.Validation

type State = {
    ClientId: string
    AgentsTopic: string
    CoordinatorTopic: string        
    Settings: AgentSettings
    MqttClient: IMqttClient
    TestHost: TestHost
    mutable ReconnectTimer: System.Timers.Timer
    Logger: Serilog.ILogger
}

let private subscribeOnCoordinatorRequests (st: State, handle: RequestMessage -> unit) =
    try
        st.MqttClient.SubscribeAsync(st.AgentsTopic).Wait()
        st.MqttClient.UseApplicationMessageReceivedHandler(fun msg -> 
            msg.ApplicationMessage.Payload
            |> Mqtt.deserialize<RequestMessage>
            |> handle        
        )
        |> ignore
    with
    | ex -> st.Logger.Error(ex, "Agent.subscribeOnTopics failed")

let validate (state: State, msg: RequestMessage) =
    match msg.Payload with
    | GetAgentInfo onlyForSessionId ->
        match onlyForSessionId with
        | Some sessionId ->
            if sessionId = state.TestHost.TestInfo.SessionId then Ok msg.Payload
            else AppError.createResult(SessionIsWrong)
                
        | None -> Ok msg.Payload
        
        
    | NewSession (_, agentSettings) ->
        let registeredScenarios =
            state.TestHost.GetRegisteredScenarios() |> Array.map(fun x -> x.ScenarioName)
        
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
        if msg.Headers.SessionId = state.TestHost.TestInfo.SessionId then
            Ok msg.Payload
        else
            AppError.createResult SessionIsWrong

let receiveCoordinatorRequest (st: State, msg: RequestMessage) = asyncResult {

    let getAgentInfo () =
        { NodeInfo = st.TestHost.CurrentNodeInfo; TargetGroup = st.Settings.TargetGroup }

    let sendToCoordinator (st: State) (res: Response) =
        Contracts.createResponseMsg(msg.Headers.CorrelationId, st.ClientId, 
                                    st.TestHost.TestInfo.SessionId, Some res, None)
        |> Mqtt.toMqttMsg(st.CoordinatorTopic)
        |> Mqtt.publishToBroker(st.MqttClient)

    match! validate(st, msg) with
    | GetAgentInfo _ ->       
        do! Response.AgentInfo(getAgentInfo()) |> sendToCoordinator(st)        
    
    | NewSession (sessionArgs, agentSettings) -> 
        let targetScenarios =
            agentSettings
            |> Array.tryFind(fun x -> x.TargetGroup = st.Settings.TargetGroup)
            |> Option.map(fun x -> x.TargetScenarios)
            |> Option.defaultValue []
            |> Seq.toArray
        
        let args = { sessionArgs with TargetScenarios = targetScenarios } 
        st.TestHost.InitScenarios(args) |> ignore
        do! Response.AgentInfo(getAgentInfo()) |> sendToCoordinator(st)        

    | StartWarmUp ->
        st.TestHost.WarmUpScenarios() |> ignore                
        do! Response.AgentInfo(getAgentInfo()) |> sendToCoordinator(st)        
            
    | StartBombing ->
        st.TestHost.StartBombing() |> ignore
        do! Response.AgentInfo(getAgentInfo()) |> sendToCoordinator(st)        

    | GetStatistics duration ->
        let stats = st.TestHost.GetNodeStats(duration)
        do! Response.AgentStats(stats) |> sendToCoordinator(st)        
}

let initAgent (dep: Dependency, registeredScenarios: Scenario[], settings: AgentSettings) = async {
    
    let clientId = sprintf "agent_%s" (Guid.NewGuid().ToString("N"))
    let! mqttClient = Mqtt.initClient(clientId, settings.MqttServer, settings.MqttPort, dep.Logger)

    let testHost = new TestHost(dep, registeredScenarios)

    let state = {            
        ClientId = clientId
        AgentsTopic = Contracts.createAgentsTopic(settings.ClusterId)
        CoordinatorTopic = Contracts.createCoordinatorTopic(settings.ClusterId)          
        Settings = settings
        MqttClient = mqttClient
        TestHost = testHost
        ReconnectTimer = new System.Timers.Timer()
        Logger = dep.Logger
    }
        
    dep.Logger.Information("{agent} established connection with {cluster}", state.ClientId, state.Settings.ClusterId)
    return state
}

let startListening (st: State) = async {
    
    let listenTask = TaskCompletionSource<unit>()
    
    let subscribeOnCoordinatorRequests() =        
        subscribeOnCoordinatorRequests(st, fun request ->
            receiveCoordinatorRequest(st, request)
            |> AsyncResult.mapError(fun e -> ())
            |> Async.RunSynchronously
            |> ignore
        )
    
    let createReconnectTimer () =
        let mutable reconnecting = false    
        let reconnectionTimer = new System.Timers.Timer(2_000.0)
        
        reconnectionTimer.Elapsed.Add(fun x ->
            if not reconnecting && not st.MqttClient.IsConnected then
                reconnecting <- true
                st.Logger.Error("{agent} disconnected from {cluster}", st.ClientId, st.Settings.ClusterId)
                Mqtt.reconnect(st.MqttClient, None, st.Logger).Wait()
                st.Logger.Information("{agent} established connection with {cluster}", st.ClientId, st.Settings.ClusterId)
                subscribeOnCoordinatorRequests()
                reconnecting <- false
        )
        reconnectionTimer

    st.ReconnectTimer <- createReconnectTimer()
    st.ReconnectTimer.Start()
    
    subscribeOnCoordinatorRequests()
    
    return! listenTask.Task |> Async.AwaitTask
}
    
type ClusterAgent() =
    
    let mutable _state: Option<State> = None
    member x.State = _state
    
    member x.StartListening(dep: Dependency, registeredScenarios: Scenario[],
                            settings: AgentSettings) =
        async {
            let! state = initAgent(dep, registeredScenarios, settings)
            _state <- Some state
            return! startListening(state)
        }
        
    interface IDisposable with
        member x.Dispose() =
            if _state.IsSome then
                use reconnectTimer = _state.Value.ReconnectTimer
                use client = _state.Value.MqttClient
                use testHost = _state.Value.TestHost
                ()