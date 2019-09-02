module internal NBomber.DomainServices.Cluster.Coordinator

open System
open System.Collections.Generic

open FsToolkit.ErrorHandling
open MQTTnet.Client
open Serilog

open NBomber.Configuration
open NBomber.Contracts
open NBomber.Errors
open NBomber.Domain
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices.Validation
open NBomber.DomainServices.ScenariosHost
open NBomber.DomainServices.Cluster.Contracts

type State = {
    SessionId: string
    ClientId: string
    AgentsTopic: string
    CoordinatorTopic: string
    Agents: Dictionary<ClientId, ClusterNodeInfo>
    AgentsStats: Dictionary<ClientId, NodeStats>
    Settings: CoordinatorSettings
    ScenariosSettings: ScenarioSetting[]
    CustomSettings: string
    MqttClient: IMqttClient
    ScenariosHost: IScenariosHost
}

module Communication =    

    let subscribeOnTopics (st: State, handle: ResponseMessage -> unit) =        
        st.MqttClient.SubscribeAsync(st.CoordinatorTopic).Wait()
        st.MqttClient.UseApplicationMessageReceivedHandler(fun msg -> 
            msg.ApplicationMessage.Payload
            |> Mqtt.deserialize<ResponseMessage>
            |> handle
        )
        |> ignore

    let sendToAgents (st: State) (req: Request) =        
        Contracts.createRequestMsg(st.ClientId, st.SessionId, req)
        |> Mqtt.toMqttMsg(st.AgentsTopic)
        |> Mqtt.publishToBroker(st.MqttClient)

    let sendGetAgentInfo (st: State) = asyncResult {
        do! Request.GetAgentInfo |> sendToAgents(st)
        do! Async.Sleep(1_000)
    }

    let sendStartNewSession (st: State) = 
        Request.NewSession(st.ScenariosSettings, st.Settings.Agents |> Seq.toArray, st.CustomSettings)
        |> sendToAgents st

    let sendStartWarmUp (st: State) =
        Request.StartWarmUp |> sendToAgents st    

    let sendStartBombing (st: State) =
        Request.StartBombing |> sendToAgents st

    let sendGetStatistics (st: State) = asyncResult {
        do! Request.GetStatistics |> sendToAgents st
        do! Async.Sleep(2_000)
    }

    let waitOnAllAgentsReady (st: State) = asyncResult {
        let mutable stop = false
        while not stop do
            do! sendGetAgentInfo(st)
            stop <- st.Agents
                    |> Seq.map(fun x -> x.Value) 
                    |> Seq.forall(fun x -> x.HostStatus = ScenarioHostStatus.Ready)        
    }

    let validateAgents (st: State) =
        let allGroups = st.Settings.Agents |> Seq.map(fun x -> x.TargetGroup) |> Seq.toArray
        let receivedGroups = st.Agents |> Seq.map(fun x -> x.Value.TargetGroup) |> Seq.toArray
        ClusterValidation.validateTargetGroups(allGroups, receivedGroups)    

let mergeStats (sessionId, machineName, registeredScenarios, 
                scenariosSettings, allNodeStats: NodeStats[]) = 
    
    let meta = { SessionId = sessionId
                 MachineName = machineName
                 Sender = NodeType.Cluster }
                    
    registeredScenarios
    |> Scenario.applySettings(scenariosSettings)
    |> Statistics.NodeStats.merge(meta, allNodeStats)

let buildStats (st: State) = 
    let localStats = st.ScenariosHost.GetStatistics()
    let agentsStats = st.AgentsStats |> Seq.map(fun x -> x.Value) |> Seq.toArray    
    let allStats = Array.append [|localStats|] agentsStats
    Ok allStats

let printAvailableAgents (st: State) =
    Log.Information(sprintf "available agents: %i" st.Agents.Count)
    if st.Agents.Count > 0 then
       st.Agents
       |> Seq.map(fun x -> x.Value)
       |> Seq.map(sprintf "%A \n")
       |> String.concat ""
       |> Log.Information

let validate (msg: ResponseMessage) =
    match msg.Payload with
    | Some v -> Ok v
    | None   -> Error msg.Error.Value

let receive (st: State, msg: ResponseMessage) = result {    
    match! validate(msg) with
    | AgentInfo info -> 
        return st.Agents.[msg.Headers.ClientId] <- info
    | AgentStats stats ->
        return st.AgentsStats.[msg.Headers.ClientId] <- stats
}

let init (dep: Dependency, registeredScenarios: Scenario[],
          settings: CoordinatorSettings, scenariosSettings: ScenarioSetting[],
          customSettings: string) = async {

    let clientId = sprintf "coordinator_%s" (Guid.NewGuid().ToString("N"))
    let! mqttClient = Mqtt.connect(clientId, settings.MqttServer)

    let state = {
        SessionId = dep.SessionId
        ClientId = clientId
        AgentsTopic = Contracts.createAgentsTopic(settings.ClusterId)
        CoordinatorTopic = Contracts.createCoordinatorTopic(settings.ClusterId)
        Agents = Dictionary<_,_>()
        AgentsStats = Dictionary<_,_>()
        Settings = settings
        ScenariosSettings = scenariosSettings
        CustomSettings = customSettings
        MqttClient = mqttClient
        ScenariosHost = ScenariosHost(dep, registeredScenarios)
    }

    return state    
}

let run (st: State) = asyncResult {    
        
    Communication.subscribeOnTopics(st, fun msg ->
        receive(st, msg)
        |> Result.mapError(AppError.toString)
        |> ignore
    )
    
    do! Communication.sendGetAgentInfo(st)
    printAvailableAgents(st)    
    
    do! Communication.sendStartNewSession(st)
    do! st.ScenariosHost.InitScenarios(st.SessionId, st.ScenariosSettings,
                                       st.Settings.TargetScenarios |> Seq.toArray,
                                       st.CustomSettings)
    do! Communication.waitOnAllAgentsReady(st)    

    do! Communication.sendStartWarmUp(st)
    do! st.ScenariosHost.WarmUpScenarios()
    do! Communication.waitOnAllAgentsReady(st)

    do! Communication.sendStartBombing(st)
    do! st.ScenariosHost.StartBombing()
    do! Communication.waitOnAllAgentsReady(st)

    do! Communication.sendGetStatistics(st)    
    return! buildStats(st)
}

let stop (st: State) =
    st.ScenariosHost.StopScenarios()
    st.MqttClient.DisconnectAsync().Wait()
    st.MqttClient.Dispose()