module internal NBomber.DomainServices.Cluster.Coordinator

open System
open System.Collections.Generic

open FsToolkit.ErrorHandling
open MQTTnet.Client

open NBomber.Configuration
open NBomber.Contracts
open NBomber.Extensions
open NBomber.Errors
open NBomber.Domain
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices.Validation
open NBomber.DomainServices.ScenariosHost
open NBomber.DomainServices.Cluster.Contracts

type State = {    
    ClientId: string
    AgentsTopic: string
    CoordinatorTopic: string    
    Agents: Dictionary<ClientId, AgentNodeInfo>
    AgentsStats: Dictionary<ClientId, RawNodeStats>
    ScenariosArgs: ScenariosArgs
    Settings: CoordinatorSettings    
    MqttClient: IMqttClient
    ScenariosHost: ScenariosHost
    StatisticsSink: IStatisticsSink option
    Logger: Serilog.ILogger
}

module Communication =    

    let subscribeOnAgentResponses (st: State, handle: ResponseMessage -> unit) =        
        st.MqttClient.SubscribeAsync(st.CoordinatorTopic).Wait()
        st.MqttClient.UseApplicationMessageReceivedHandler(fun msg -> 
            msg.ApplicationMessage.Payload
            |> Mqtt.deserialize<ResponseMessage>
            |> handle
        )
        |> ignore

    let sendToAgents (st: State) (req: Request) =        
        Contracts.createRequestMsg(st.ClientId, st.ScenariosArgs.SessionId, req)
        |> Mqtt.toMqttMsg(st.AgentsTopic)
        |> Mqtt.publishToBroker(st.MqttClient)

    let sendGetAllAgentInfo (st: State) = asyncResult {        
        do! Request.GetAgentInfo(onlyForSessionId = None) |> sendToAgents(st)
        do! Async.Sleep(1_000)
    }
    
    let sendGetAgentInfoForCurrentSession (st: State) = asyncResult {
        do! Request.GetAgentInfo(Some st.ScenariosArgs.SessionId) |> sendToAgents(st)
        do! Async.Sleep(1_000)
    }

    let sendStartNewSession (st: State) =        
        Request.NewSession(st.ScenariosArgs.ScenariosSettings,
                           st.Settings.Agents |> Seq.toArray,
                           st.ScenariosArgs.CustomSettings)
        |> sendToAgents st

    let sendStartWarmUp (st: State) =
        Request.StartWarmUp |> sendToAgents st    

    let sendStartBombing (st: State) =
        Request.StartBombing |> sendToAgents st

    let getAgentsStats (st: State, forDuration: TimeSpan option) = asyncResult {
        st.AgentsStats.Clear()
        do! Request.GetStatistics(forDuration) |> sendToAgents st
        
        let mutable allAgentsResponded = false
        let mutable failCounter = 0
        while not allAgentsResponded do
            do! Async.Sleep(2_000)
            if st.Agents.Count = st.AgentsStats.Count then
                allAgentsResponded <- true
            else
                failCounter <- failCounter + 1
                if failCounter > 5 then
                    allAgentsResponded <- true
        
        if failCounter > 5 then
            return! AppError.createResult(CommunicationError.NotAllStatsReceived)
        else
            return st.AgentsStats |> Map.fromDictionary              
    }

    let waitOnAllAgentsReady (st: State) = asyncResult {
        let mutable stop = false
        while not stop do
            do! sendGetAgentInfoForCurrentSession(st)
            stop <- st.Agents
                    |> Seq.map(fun x -> x.Value) 
                    |> Seq.forall(fun x -> x.HostStatus = ScenarioHostStatus.Ready)        
    }

    let validateAgents (st: State) =
        let allGroups = st.Settings.Agents |> Seq.map(fun x -> x.TargetGroup) |> Seq.toArray
        let receivedGroups = st.Agents |> Seq.map(fun x -> x.Value.TargetGroup) |> Seq.toArray
        ClusterValidation.validateTargetGroups(allGroups, receivedGroups)    

module ClusterStats =    

    let combineAllNodeStats (coordinatorStats: RawNodeStats, agentsStats: Map<ClientId, RawNodeStats>) =
        let agentsStats = agentsStats |> Seq.map(fun x -> x.Value) |> Seq.toArray    
        let allNodesStats = Array.append [|coordinatorStats|] agentsStats
        allNodesStats
        
    let buildClusterStats (st: State, allNodeStats: RawNodeStats[], executionTime: TimeSpan option) =       
        
        let meta = { SessionId = st.ScenariosArgs.SessionId
                     MachineName = st.ScenariosHost.NodeStatsMeta.MachineName
                     Sender = NodeType.Cluster
                     Operation = NBomber.DomainServices.ScenariosHost.mapToOperationType(st.ScenariosHost.Status) }
        
        st.ScenariosHost.GetRegisteredScenarios()
        |> Scenario.applySettings(st.ScenariosArgs.ScenariosSettings)
        |> Statistics.NodeStats.merge meta allNodeStats executionTime       
        
    let fetchClusterStats (st: State, executionTime: TimeSpan option) = asyncResult {
        let! agentsStats = Communication.getAgentsStats(st, executionTime)
        let coordinatorStats = st.ScenariosHost.GetNodeStats(executionTime)
        let allNodeStats = combineAllNodeStats(coordinatorStats, agentsStats)
        let clusterStats = buildClusterStats(st, allNodeStats, executionTime)
        return Array.append [|clusterStats|] allNodeStats
    }
    
    let saveClusterStats (allClusterStats: RawNodeStats[], statisticsSink: IStatisticsSink) =    
        ScenarioHostStats.saveStats(allClusterStats, statisticsSink)        
    
    let startSaveStatsTimer (st: State) =    
        match st.StatisticsSink with
        | Some statsSink->        
            let mutable executionTime = TimeSpan.Zero
            let timer = new System.Timers.Timer(Constants.GetStatsInterval)
            timer.Elapsed.Add(fun _ ->
                asyncResult {
                    // moving time forward
                    executionTime <- executionTime.Add(TimeSpan.FromMilliseconds Constants.GetStatsInterval)
                    let! clusterStats = fetchClusterStats(st, Some executionTime)                                        
                    saveClusterStats(clusterStats, statsSink) |> ignore                    
                }
                |> Async.RunSynchronously
                |> ignore
            )
            timer.Start()
            timer
        
        | None -> new System.Timers.Timer()
        
    let validateWarmUpStats (st: State) = asyncResult {
        let! allStats = fetchClusterStats(st, None)
        let clusterStats = allStats |> Array.find(fun x -> x.NodeStatsInfo.Sender = NodeType.Cluster)
        do! ScenarioValidation.validateWarmUpStats(clusterStats)
    }
        
let printAvailableAgents (st: State) =
    st.Logger.Information(sprintf "available agents: %i" st.Agents.Count)
    if st.Agents.Count > 0 then
       st.Agents
       |> Seq.map(fun x -> x.Value)
       |> Seq.map(sprintf "%A \n")
       |> String.concat ""
       |> st.Logger.Information

let validate (msg: ResponseMessage) =
    match msg.Payload with
    | Some v -> Ok v
    | None   -> Error msg.Error.Value

let receiveAgentResponse (st: State, msg: ResponseMessage) = result {    
    match! validate(msg) with
    | AgentInfo info ->
        st.Agents.[msg.Headers.ClientId] <- info
            
    | AgentStats stats ->
        st.AgentsStats.[msg.Headers.ClientId] <- stats
}

let initCoordinator (dep: Dependency, registeredScenarios: Scenario[],
                     settings: CoordinatorSettings, scnArgs: ScenariosArgs) = async {

    let clientId = sprintf "coordinator_%s" (Guid.NewGuid().ToString("N"))
    let! mqttClient = Mqtt.initClient(clientId, settings.MqttServer, settings.MqttPort, dep.Logger)
    let scnArgs = { scnArgs with TargetScenarios = settings.TargetScenarios |> List.toArray }    
    
    let state = {        
        ClientId = clientId
        AgentsTopic = Contracts.createAgentsTopic(settings.ClusterId)
        CoordinatorTopic = Contracts.createCoordinatorTopic(settings.ClusterId)
        Agents = Dictionary<_,_>()
        AgentsStats = Dictionary<_,_>()
        ScenariosArgs = scnArgs
        Settings = settings        
        MqttClient = mqttClient
        ScenariosHost = new ScenariosHost(dep, registeredScenarios)
        StatisticsSink = dep.StatisticsSink
        Logger = dep.Logger
    }
    return state    
}

let runSession (st: State) = asyncResult {    
        
    Communication.subscribeOnAgentResponses(st, fun response ->
        receiveAgentResponse(st, response)
        |> Result.mapError(AppError.toString)
        |> ignore
    )
            
    do! Communication.sendGetAllAgentInfo(st)
    printAvailableAgents(st)    
    
    do! Communication.sendStartNewSession(st)
    do! st.ScenariosHost.InitScenarios(st.ScenariosArgs)
    do! Communication.waitOnAllAgentsReady(st)    

    // warm-up
    do! Communication.sendStartWarmUp(st)
    use warmUpStatsTimer = ClusterStats.startSaveStatsTimer(st)
    do! st.ScenariosHost.WarmUpScenarios()
    do! Communication.waitOnAllAgentsReady(st)
    warmUpStatsTimer.Stop()
    do! ClusterStats.validateWarmUpStats(st)

    // bombing
    do! Communication.sendStartBombing(st)
    use bombingStatsTimer = ClusterStats.startSaveStatsTimer(st)
    do! st.ScenariosHost.StartBombing()
    do! Communication.waitOnAllAgentsReady(st)
    bombingStatsTimer.Stop()
    
    // saving final stats results
    let! allStats = ClusterStats.fetchClusterStats(st, None)
    if st.StatisticsSink.IsSome then
        do! ClusterStats.saveClusterStats(allStats, st.StatisticsSink.Value)
    
    return allStats
}

type ClusterCoordinator() =
    
    let mutable _state: Option<State> = None    
    member x.State = _state
    
    member x.RunSession(dep: Dependency, registeredScenarios: Scenario[],
                        settings: CoordinatorSettings, scnArgs: ScenariosArgs) =
        asyncResult {
            let! state = initCoordinator(dep, registeredScenarios, settings, scnArgs)        
            _state <- Some state
            let! clusterStats = runSession(state)
            return clusterStats
        }
    
    interface IDisposable with
        member x.Dispose() =
            if _state.IsSome then
                use client = _state.Value.MqttClient
                use scnHost = _state.Value.ScenariosHost
                ()