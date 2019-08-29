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

type State = {    
    ClientId: string
    AgentsTopic: string
    CoordinatorTopic: string
    NodeInfo: ClusterNodeInfo    
    Settings: AgentSettings
    MqttClient: IMqttClient
    ScenariosHost: IScenariosHost
}

let private subscribeOnTopics (st: State, handle: RequestMessage -> unit) =    
    st.MqttClient.SubscribeAsync(st.AgentsTopic).Wait()
    st.MqttClient.UseApplicationMessageReceivedHandler(fun msg -> 
        msg.ApplicationMessage.Payload
        |> Mqtt.deserialize<RequestMessage>
        |> handle        
    )
    |> ignore

let validate (state: State, msg: RequestMessage) =
    match msg.Payload with
    | GetAgentInfo -> Ok msg.Payload
    | NewSession _ -> Ok msg.Payload
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
        let targetScns = [||]
        st.ScenariosHost.InitScenarios(msg.Headers.SessionId, scnSettings, targetScns, customSettings) |> ignore        
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
    let! mqttClient = Mqtt.connect(clientId, settings.MqttServer)

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
    }
        
    return state
}

let run (st: State) = async {    
     
    subscribeOnTopics(st, fun msg ->
        receive(st, msg)
        |> AsyncResult.mapError(fun e -> ())
        |> Async.RunSynchronously
        |> ignore
    )

    let listenTask = TaskCompletionSource<unit>()
    return! listenTask.Task |> Async.AwaitTask
}