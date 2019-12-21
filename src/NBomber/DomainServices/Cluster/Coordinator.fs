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
open NBomber.DomainServices.TestHost
open NBomber.DomainServices.Cluster.Contracts

type State = {    
    ClientId: string
    AgentsTopic: string
    CoordinatorTopic: string    
    Agents: Dictionary<ClientId, AgentNodeInfo>
    AgentsStats: Dictionary<ClientId, RawNodeStats>
    TestSessionArgs: TestSessionArgs
    Settings: CoordinatorSettings    
    MqttClient: IMqttClient
    TestHost: TestHost
    ReportingSinks: IReportingSink[]
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
        Contracts.createRequestMsg(st.ClientId, st.TestSessionArgs.TestInfo.SessionId, req)
        |> Mqtt.toMqttMsg(st.AgentsTopic)
        |> Mqtt.publishToBroker(st.MqttClient)

    let sendGetAllAgentInfo (st: State) = asyncResult {        
        do! Request.GetAgentInfo(onlyForSessionId = None) |> sendToAgents(st)
        do! Async.Sleep(1_000)
    }
    
    let sendGetAgentInfoForCurrentSession (st: State) = asyncResult {
        do! Request.GetAgentInfo(Some st.TestSessionArgs.TestInfo.SessionId) |> sendToAgents(st)
        do! Async.Sleep(1_000)
    }

    let sendStartNewSession (st: State) =
        Request.NewSession(st.TestSessionArgs, st.Settings.Agents |> Seq.toArray)
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

    let waitOnAllAgents (st: State, operation: NodeOperationType) = asyncResult {
        let mutable stop = false
        while not stop do
            do! sendGetAgentInfoForCurrentSession(st)
            stop <- st.Agents
                    |> Seq.map(fun x -> x.Value) 
                    |> Seq.forall(fun x -> x.NodeInfo.CurrentOperation = operation)        
    }
    
    let waitOnAllAgentsReady (st: State) = waitOnAllAgents(st, NodeOperationType.None)
    let waitOnAllAgentsComplete (st: State) = waitOnAllAgents(st, NodeOperationType.Complete)

    let validateAgents (st: State) =
        let allGroups = st.Settings.Agents |> Seq.map(fun x -> x.TargetGroup) |> Seq.toArray
        let receivedGroups = st.Agents |> Seq.map(fun x -> x.Value.TargetGroup) |> Seq.toArray
        ClusterValidation.validateTargetGroups(allGroups, receivedGroups)    

module ClusterReporting =    

    let combineAllNodeStats (coordinatorStats: RawNodeStats, agentsStats: Map<ClientId, RawNodeStats>) =
        let agentsStats = agentsStats |> Seq.map(fun x -> x.Value) |> Seq.toArray    
        let allNodesStats = Array.append [|coordinatorStats|] agentsStats
        allNodesStats
        
    let buildClusterStats (st: State, allNodeStats: RawNodeStats[], executionTime: TimeSpan option) =       
        
        let meta = { MachineName = st.TestHost.CurrentNodeInfo.MachineName
                     Sender = NodeType.Cluster
                     CurrentOperation = st.TestHost.CurrentOperation }
        
        st.TestHost.GetRegisteredScenarios()
        |> Scenario.applySettings(st.TestSessionArgs.ScenariosSettings)
        |> Statistics.NodeStats.merge meta allNodeStats executionTime       
        
    let fetchClusterStats (st: State, executionTime: TimeSpan option) = asyncResult {
        let! agentsStats = Communication.getAgentsStats(st, executionTime)
        let coordinatorStats = st.TestHost.GetNodeStats(executionTime)
        let allNodeStats = combineAllNodeStats(coordinatorStats, agentsStats)
        let clusterStats = buildClusterStats(st, allNodeStats, executionTime)
        return Array.append [|clusterStats|] allNodeStats
    }
    
    let saveClusterStats (testInfo: TestInfo, allClusterStats: RawNodeStats[], sinks: IReportingSink[]) =    
        TestHostReporting.saveStats(testInfo, allClusterStats, sinks)
    
    let startRealtimeTimer (st: State) =
        if not (Array.isEmpty st.ReportingSinks) then            
            let mutable executionTime = TimeSpan.Zero
            let timer = new System.Timers.Timer(st.TestSessionArgs.SendStatsInterval.TotalMilliseconds)
            timer.Elapsed.Add(fun _ ->
                asyncResult {
                    // moving time forward
                    executionTime <- executionTime.Add(st.TestSessionArgs.SendStatsInterval)
                    match st.TestHost.CurrentNodeInfo.CurrentOperation with
                    | NodeOperationType.WarmUp 
                    | NodeOperationType.Bombing ->                            
                        let! clusterStats = fetchClusterStats(st, Some executionTime)                                        
                        saveClusterStats(st.TestSessionArgs.TestInfo, clusterStats, st.ReportingSinks) |> ignore
                    
                    | _ -> ()
                }
                |> Async.RunSynchronously
                |> ignore
            )
            timer.Start()
            timer        
        else
            new System.Timers.Timer()
        
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
                     settings: CoordinatorSettings, testArgs: TestSessionArgs) = async {

    let clientId = sprintf "coordinator_%s" (Guid.NewGuid().ToString("N"))
    let! mqttClient = Mqtt.initClient(clientId, settings.MqttServer, settings.MqttPort, dep.Logger)
    let args = { testArgs with TargetScenarios = settings.TargetScenarios |> List.toArray }    
    
    let state = {
        ClientId = clientId
        AgentsTopic = Contracts.createAgentsTopic(settings.ClusterId)
        CoordinatorTopic = Contracts.createCoordinatorTopic(settings.ClusterId)
        Agents = Dictionary<_,_>()
        AgentsStats = Dictionary<_,_>()
        TestSessionArgs = args
        Settings = settings
        MqttClient = mqttClient
        TestHost = new TestHost(dep, registeredScenarios)
        ReportingSinks = dep.ReportingSinks
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
    do! st.TestHost.InitScenarios(st.TestSessionArgs)
    do! Communication.waitOnAllAgentsReady(st)    

    // warm-up
    do! Communication.sendStartWarmUp(st)    
    do! st.TestHost.WarmUpScenarios()    
    do! Communication.waitOnAllAgentsReady(st)
    do! ClusterReporting.validateWarmUpStats(st)

    // bombing
    do! Communication.sendStartBombing(st)
    use bombingReportingTimer = ClusterReporting.startRealtimeTimer(st)
    do! st.TestHost.StartBombing()
    bombingReportingTimer.Stop()
    do! Communication.waitOnAllAgentsComplete(st)
        
    let! allStats = ClusterReporting.fetchClusterStats(st, None)
    return allStats
}

type ClusterCoordinator() =
    
    let mutable _state: Option<State> = None    
    member x.State = _state
    
    member x.RunSession(dep: Dependency, registeredScenarios: Scenario[],
                        settings: CoordinatorSettings, sessionArgs: TestSessionArgs) =
        asyncResult {
            let! state = initCoordinator(dep, registeredScenarios, settings, sessionArgs)        
            _state <- Some state
            let! clusterStats = runSession(state)
            return clusterStats
        }
    
    interface IDisposable with
        member x.Dispose() =
            if _state.IsSome then
                use client = _state.Value.MqttClient
                use testHost = _state.Value.TestHost
                ()