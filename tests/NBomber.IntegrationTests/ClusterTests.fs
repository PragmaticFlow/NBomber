module Tests.ClusterTests

open System
open System.Collections.Concurrent
open System.Threading.Tasks

open Xunit
open Swensen.Unquote
open FSharp.Control.Tasks.V2.ContextInsensitive
open FsToolkit.ErrorHandling

open NBomber.Configuration
open NBomber.Contracts
open NBomber.Errors
open NBomber.Extensions
open NBomber.Infra
open NBomber.DomainServices.TestHost
open NBomber.DomainServices.Cluster.Coordinator
open NBomber.DomainServices.Cluster.Agent
open NBomber.FSharp
open Tests.TestHelper

// todo: test on very big message limit
// todo: test on two concurrent coordinators
// todo: test that agent can stop attack if coordinator restarts
// todo: test coordinator waitOnAllAgentsReady to wait several times and stop if failure
// todo: test coordinator getAgentsStats

// todo: test NbomberRunner that if warmup duration = 0 than it will not run

let agents = [
    { TargetGroup = "1"; TargetScenarios = ["test_scenario"] }
    { TargetGroup = "2"; TargetScenarios = ["test_scenario"] }
]

let coordinatorSettings = {
    ClusterId = "test_cluster"
    TargetScenarios = ["test_scenario"]
    MqttServer = "localhost"
    MqttPort = None
    Agents = agents
}

let agentSettings = {
    ClusterId = "test_cluster"
    TargetGroup = "1"
    MqttServer = "localhost"
    MqttPort = None
}

let okStep = Step.create("ok step", fun _ -> task {
    do! Task.Delay(TimeSpan.FromSeconds(0.1))
    return Response.Ok()
})

let failStep = Step.create("fail step", fun _ -> task {
    do! Task.Delay(TimeSpan.FromSeconds(0.1))
    return Response.Fail()
})

let private scenario =
    Scenario.create "test_scenario" [okStep]
    |> Scenario.withWarmUpDuration(TimeSpan.FromSeconds 1.0)
    |> Scenario.withDuration(TimeSpan.FromSeconds 1.0)
    |> NBomber.Domain.Scenario.create

let scenarioSettings = {
    ScenarioName = "test_scenario"
    ConcurrentCopies = 1
    WarmUpDuration = DateTime(TimeSpan.FromSeconds(1.0).Ticks)
    Duration = DateTime(TimeSpan.FromSeconds(1.0).Ticks)
}

let private testSessionArgs = {
    TestInfo = { SessionId = ""; TestSuite = ""; TestName = "" }
    ScenariosSettings = [| scenarioSettings |]
    TargetScenarios = Array.empty
    CustomSettings = ""
    SendStatsInterval = TimeSpan.FromSeconds(NBomber.Domain.Constants.MinSendStatsIntervalSec)
}

let internal isWarmUpStarted (currentOperation) =
    match currentOperation with
    | NodeOperationType.WarmUp -> true
    | _                    -> false

[<Fact>]
let ``Coordinator can run as single bomber`` () = async {
    let randomPort = Random().Next(65000)
    let dep = Dependency.createFor(NodeType.Coordinator)
    let server = MqttTests.startMqttServer(randomPort)

    // specified no agents
    let coordinatorSettings = { coordinatorSettings with Agents = List.empty; MqttPort = Some randomPort }

    let sessionArgs = { testSessionArgs with TestInfo = dep.TestInfo }
    let registeredScenarios = [| scenario |]

    // we start coordinator without agents
    use coordinator = new ClusterCoordinator()
    let! statsResult = coordinator.RunSession(dep.Dep, registeredScenarios, coordinatorSettings, sessionArgs)
    MqttTests.stopMqttServer server

    let allStats = statsResult |> Result.defaultValue Array.empty
    let coordinatorStats = allStats |> Array.find(fun x -> x.NodeStatsInfo.Sender = NodeType.Coordinator)
    let clusterStats = allStats |> Array.find(fun x -> x.NodeStatsInfo.Sender = NodeType.Cluster)

    test <@ allStats.Length = 2 @>
    test <@ coordinatorStats.OkCount > 1 @>
    test <@ clusterStats.OkCount > 1 @>
}

[<Fact>]
let ``Coordinator should be able to start bombing even when agents are offline`` () = async {
    let randomPort = Random().Next(65000)
    let dep = Dependency.createFor(NodeType.Coordinator)
    let server = MqttTests.startMqttServer(randomPort)

    // specified agents in config which are not connected
    let coordinatorSettings = { coordinatorSettings with Agents = agents; MqttPort = Some randomPort }

    let scnArgs = { testSessionArgs with TestInfo = dep.TestInfo }
    let registeredScenarios = [| scenario |]

    // we start coordinator without agents even though we specified agents in config
    use coordinator = new ClusterCoordinator()
    let! statsResult = coordinator.RunSession(dep.Dep, registeredScenarios, coordinatorSettings, scnArgs)
    MqttTests.stopMqttServer server

    let allStats = statsResult |> Result.defaultValue Array.empty
    let coordinatorStats = allStats |> Array.find(fun x -> x.NodeStatsInfo.Sender = NodeType.Coordinator)
    let clusterStats = allStats |> Array.find(fun x -> x.NodeStatsInfo.Sender = NodeType.Cluster)

    test <@ allStats.Length = 2 @>
    test <@ coordinatorStats.OkCount > 1 @>
    test <@ clusterStats.OkCount > 1 @>
}

[<Fact>]
let ``Coordinator and agents should attack together`` () = async {
    let randomPort = Random().Next(65000)
    let coordinatorSettings = { coordinatorSettings with MqttPort = Some randomPort }
    let agentSettings = { agentSettings with MqttPort = Some randomPort }
    let coordinatorDep = Dependency.createFor(NodeType.Coordinator)
    let agentDep = Dependency.createFor(NodeType.Agent)
    let server = MqttTests.startMqttServer(randomPort)

    let scnArgs = { testSessionArgs with TestInfo = coordinatorDep.TestInfo }
    let registeredScenarios = [| scenario |]

    // we start coordinator with one agent
    use coordinator = new ClusterCoordinator()
    use agent = new ClusterAgent()
    agent.StartListening(agentDep.Dep, registeredScenarios, agentSettings) |> Async.Start
    let! statsResult = coordinator.RunSession(coordinatorDep.Dep, registeredScenarios, coordinatorSettings, scnArgs)
    MqttTests.stopMqttServer server

    let allStats = statsResult |> Result.defaultValue Array.empty
    let coordinatorStats = allStats |> Array.find(fun x -> x.NodeStatsInfo.Sender = NodeType.Coordinator)
    let agentStats = allStats |> Array.find(fun x -> x.NodeStatsInfo.Sender = NodeType.Agent)

    test <@ allStats.Length = 3 @>
    test <@ coordinatorStats.OkCount > 1 @>
    test <@ agentStats.OkCount > 1 @>
}

[<Fact>]
let ``Changing cluster topology should not affect a current test execution`` () = async {

    let randomPort = Random().Next(65000)
    let coordinatorSettings = { coordinatorSettings with MqttPort = Some randomPort }
    let agentSettings = { agentSettings with MqttPort = Some randomPort }

    let coordinatorDep = Dependency.createFor(NodeType.Coordinator)
    let agentDep = Dependency.createFor(NodeType.Agent)
    let server = MqttTests.startMqttServer(randomPort)

    // we specify long warm-up to be able to inject a second agent in the middle of the execution
    let scnSettings = { scenarioSettings with WarmUpDuration = DateTime(TimeSpan.FromSeconds(5.0).Ticks)
                                              Duration = DateTime(TimeSpan.FromSeconds(5.0).Ticks) }
    let sessionArgs = { testSessionArgs with TestInfo = coordinatorDep.TestInfo
                                             ScenariosSettings = [| scnSettings |] }
    let registeredScenarios = [| scenario |]

    // we start coordinator with one agent
    use coordinator = new ClusterCoordinator()
    use agent1 = new ClusterAgent()

    // for now we start only agent1
    agent1.StartListening(agentDep.Dep, registeredScenarios, agentSettings) |> Async.Start
    do! Async.Sleep(1_000) // wait on init agent1.State to be abe to access it

    let coordinatorTask = coordinator.RunSession(coordinatorDep.Dep, registeredScenarios,
                                                 coordinatorSettings, sessionArgs)
                          |> Async.StartAsTask

    // waiting until warm-up starts
    while not <| isWarmUpStarted(agent1.State.Value.TestHost.CurrentOperation) do
        do! Async.Sleep(1_000)

    // spin up a new agent2
    use agent2 = new ClusterAgent()
    agent2.StartListening(agentDep.Dep, [| scenario |], agentSettings) |> Async.Start
    let! statsResult = coordinatorTask |> Async.AwaitTask
    MqttTests.stopMqttServer server

    let allStats = statsResult |> Result.defaultValue Array.empty
    let coordinatorStats = allStats |> Array.find(fun x -> x.NodeStatsInfo.Sender = NodeType.Coordinator)
    let agentStats = allStats |> Array.filter(fun x -> x.NodeStatsInfo.Sender = NodeType.Agent)

    test <@ agentStats.Length = 1 @> // it should be '2' if agent2 joined the test
    test <@ agentStats.[0].OkCount > 1 @>
    test <@ coordinator.State.Value.Agents.Count = 1 @> // coordinator has found 1 agent
    test <@ coordinatorStats.OkCount > 1 @>
}

[<Fact>]
let ``Coordinator should be able to propagate all types of settings among the agents`` () = async {

    let randomPort = Random().Next(65000)
    let coordinatorSettings = { coordinatorSettings with Agents = agents; MqttPort = Some randomPort }
    let agentSettings = { agentSettings with MqttPort = Some randomPort }
    let coordinatorDep = Dependency.createFor(NodeType.Coordinator)
    let agentDep = Dependency.createFor(NodeType.Agent)
    let server = MqttTests.startMqttServer(randomPort)

    // set up custom settings
    let scenarioSettings = { scenarioSettings with ConcurrentCopies = 5 }
    let customSettings = "{ Age: 28 }"

    let sessionArgs = {
        TestInfo = coordinatorDep.TestInfo
        ScenariosSettings = [| scenarioSettings |]
        TargetScenarios = Array.empty
        CustomSettings = customSettings
        SendStatsInterval = TimeSpan.FromSeconds(NBomber.Domain.Constants.MinSendStatsIntervalSec)
    }

    // set up scenarios
    let customSettingsList = ConcurrentDictionary<NodeType, string>()
    let scenario =
        Scenario.create "test_scenario" [okStep]
        |> Scenario.withDuration(TimeSpan.FromSeconds 1.0)
        |> Scenario.withTestInit(fun context -> task {
            // TestInit will be invoked on agent and on coordinator
            customSettingsList.[context.NodeType] <- context.CustomSettings
        })
        |> NBomber.Domain.Scenario.create
    let registeredScenarios = [| scenario |]

    // we start coordinator which will propagate settings to agent
    use coordinator = new ClusterCoordinator()
    use agent = new ClusterAgent()
    agent.StartListening(agentDep.Dep, registeredScenarios, agentSettings) |> Async.Start
    let! statsResult = coordinator.RunSession(coordinatorDep.Dep, registeredScenarios, coordinatorSettings, sessionArgs)
    MqttTests.stopMqttServer server

    let customSettings = customSettingsList |> Seq.filter(fun x -> x.Value = customSettings) |> Seq.toArray
    let agentScenario = agent.State.Value.TestHost.GetRunningScenarios() |> Array.item 0

    test <@ customSettings.Length = 2 @> // because one from coordinator and one from agent
    test <@ scenario.ConcurrentCopies = NBomber.Domain.Constants.DefaultConcurrentCopies @>
    test <@ agentScenario.ConcurrentCopies = scenarioSettings.ConcurrentCopies @>
}

[<Fact>]
let ``Agent should run test only under their agent group`` () = async {

    let randomPort = Random().Next(65000)
    let coordinatorDep = Dependency.createFor(NodeType.Coordinator)
    let agentDep = Dependency.createFor(NodeType.Agent)
    let server = MqttTests.startMqttServer(randomPort)

    // set up agent group
    let agentSettings = { agentSettings with TargetGroup = "not_matched_group"; MqttPort = Some randomPort }
    let agents = [
        { TargetGroup = "222"; TargetScenarios = ["test_scenario"] }
    ]
    let coordinatorSettings = { coordinatorSettings with Agents = agents; MqttPort = Some randomPort }

    // set up scenarios
    let scenario =
        Scenario.create "test_scenario" [okStep]
        |> Scenario.withDuration(TimeSpan.FromSeconds 1.0)
        |> NBomber.Domain.Scenario.create

    let scnArgs = { testSessionArgs with TestInfo = coordinatorDep.TestInfo }
    let registeredScenarios = [| scenario |]

    // we start coordinator which will propagate settings to agent
    use coordinator = new ClusterCoordinator()
    use agent = new ClusterAgent()
    agent.StartListening(agentDep.Dep, registeredScenarios, agentSettings) |> Async.Start

    let! statsResult = coordinator.RunSession(coordinatorDep.Dep, registeredScenarios, coordinatorSettings, scnArgs)
    MqttTests.stopMqttServer server

    let allStats = statsResult |> Result.defaultValue Array.empty
    let agentStats = allStats |> Array.tryFind(fun x -> x.NodeStatsInfo.Sender = NodeType.Agent)

    test <@ agentStats.IsNone @>
}

[<Fact>]
let ``Coordinator and Agent should run tests only from TargetScenarios`` () = async {

    let randomPort = Random().Next(65000)
    let coordinatorDep = Dependency.createFor(NodeType.Coordinator)
    let agentDep = Dependency.createFor(NodeType.Agent)
    let server = MqttTests.startMqttServer(randomPort)

    // set up agent group
    let agentSettings = { agentSettings with TargetGroup = "222"; MqttPort = Some randomPort }
    // agent will run only test_scenario_222
    let agents = [
        { TargetGroup = "222"; TargetScenarios = ["test_scenario_222"] }
    ]
    // coordinator will run only test_scenario_111
    let coordinatorSettings = { coordinatorSettings with Agents = agents
                                                         TargetScenarios = ["test_scenario_111"]
                                                         MqttPort = Some randomPort }

    let scnArgs = { testSessionArgs with TestInfo = coordinatorDep.TestInfo }

    // set up scenarios
    let scenario_111 =
        Scenario.create "test_scenario_111" [okStep]
        |> Scenario.withDuration(TimeSpan.FromSeconds 1.0)
        |> NBomber.Domain.Scenario.create

    let scenario_222 = { scenario_111 with ScenarioName = "test_scenario_222"  }
    let registeredScenarios = [| scenario_111; scenario_222 |]

    // we start coordinator which will propagate settings to agent
    use coordinator = new ClusterCoordinator()
    use agent = new ClusterAgent()
    agent.StartListening(agentDep.Dep, registeredScenarios, agentSettings) |> Async.Start

    let! statsResult = coordinator.RunSession(coordinatorDep.Dep, registeredScenarios, coordinatorSettings, scnArgs)
    MqttTests.stopMqttServer server

    let allStats = statsResult |> Result.defaultValue Array.empty
    let coordinatorStats = allStats |> Array.find(fun x -> x.NodeStatsInfo.Sender = NodeType.Coordinator)
    let agentStats = allStats |> Array.find(fun x -> x.NodeStatsInfo.Sender = NodeType.Agent)

    test <@ coordinatorStats.AllScenariosStats.[0].ScenarioName = "test_scenario_111" @>
    test <@ agentStats.AllScenariosStats.[0].ScenarioName = "test_scenario_222" @>
}

[<Fact>]
let ``Agent should be able to reconnect automatically and join the cluster`` () = async {

    let randomPort = Random().Next(65000)
    let coordinatorSettings = { coordinatorSettings with MqttPort = Some randomPort }
    let agentSettings = { agentSettings with MqttPort = Some randomPort }
    let coordinatorDep = Dependency.createFor(NodeType.Coordinator)
    let agentDep = Dependency.createWithInMemoryLogger(NodeType.Agent)
    let server = MqttTests.startMqttServer(randomPort)

    let sessionArgs = { testSessionArgs with TestInfo = coordinatorDep.TestInfo }
    let registeredScenarios = [| scenario |]

    // we start one agent
    use agent = new ClusterAgent()
    agent.StartListening(agentDep.Dep, registeredScenarios, agentSettings) |> Async.Start
    do! Async.Sleep(1_000) // waiting on establish connection

    // we stop mqtt server and wait some time
    MqttTests.stopMqttServer server
    do! Async.Sleep(5_000)

    // spin up the mqtt server again to see that agent will reconnect
    let server = MqttTests.startMqttServer(randomPort)
    do! Async.Sleep(2_000) // waiting on reconnect

    use coordinator = new ClusterCoordinator()
    let! statsResult = coordinator.RunSession(coordinatorDep.Dep, registeredScenarios, coordinatorSettings, sessionArgs)
    MqttTests.stopMqttServer server

    let allStats = statsResult |> Result.defaultValue Array.empty
    let coordinatorStats = allStats |> Array.find(fun x -> x.NodeStatsInfo.Sender = NodeType.Coordinator)
    let agentStats = allStats |> Array.find(fun x -> x.NodeStatsInfo.Sender = NodeType.Agent)
    let logEvents = agentDep.MemorySink.LogEvents |> Seq.toArray

    let agentDisconnectedEvents =
        logEvents
        |> Array.filter(fun x -> x.MessageTemplate.Text.Contains("{agent} disconnected from {cluster}"))

    let agentConnectedEvents =
        logEvents
        |> Array.filter(fun x -> x.MessageTemplate.Text.Contains("{agent} established connection with {cluster}"))

    test <@ allStats.Length = 3 @>
    test <@ coordinatorStats.OkCount > 1 @>
    test <@ agentStats.OkCount > 1 @>
    test <@ agentDisconnectedEvents.Length = 1 @>
    test <@ agentConnectedEvents.Length = 2 @>
}

[<Fact>]
let ``Coordinator should stop session execution if too many failed results on a warm-up`` () = async {
    let randomPort = Random().Next(65000)
    let coordinatorSettings =
        { coordinatorSettings with MqttPort = Some randomPort
                                   TargetScenarios = ["fail_scenario"]
                                   Agents = [{ TargetGroup = "1"; TargetScenarios = ["fail_scenario"] }] }

    let agentSettings = { agentSettings with MqttPort = Some randomPort }
    let coordinatorDep = Dependency.createFor(NodeType.Coordinator)
    let agentDep = Dependency.createFor(NodeType.Agent)
    let server = MqttTests.startMqttServer(randomPort)

    let scenario =
        Scenario.create "fail_scenario" [failStep]
        |> Scenario.withWarmUpDuration(TimeSpan.FromSeconds 1.0)
        |> Scenario.withDuration(TimeSpan.FromSeconds 60.0)
        |> NBomber.Domain.Scenario.create

    let registeredScenarios = [| scenario |]
    let scnArgs = { testSessionArgs with TestInfo = coordinatorDep.TestInfo }

    // we start coordinator and agent
    use coordinator = new ClusterCoordinator()
    use agent = new ClusterAgent()
    agent.StartListening(agentDep.Dep, registeredScenarios, agentSettings) |> Async.Start

    let! statsResult = coordinator.RunSession(coordinatorDep.Dep, registeredScenarios, coordinatorSettings, scnArgs)
    MqttTests.stopMqttServer server

    let warmUpErrorFound =
        match Result.getError statsResult with
        | Domain error -> match error with
                          | WarmUpErrorWithManyFailedSteps _ -> true
                          | _ -> false
        | _ -> false

    test <@ warmUpErrorFound = true @>
}
