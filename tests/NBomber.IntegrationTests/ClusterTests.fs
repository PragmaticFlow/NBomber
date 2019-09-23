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
open NBomber.Infra
open NBomber.DomainServices.ScenariosHost
open NBomber.DomainServices.Cluster
open NBomber.FSharp
open Tests.TestHelper
 
// todo: test on very big message limit
// todo: test on two concurrent coordinators
// todo: test that agent can stop attack if coordinator restarts
// todo: test coordinator waitOnAllAgentsReady

// todo: test NbomberRunner that if warmup duration = 0 than it will not run 

let agents = [
    { TargetGroup = "1"; TargetScenarios = ["test_scenario"] }    
    { TargetGroup = "2"; TargetScenarios = ["test_scenario"] }
]

let coordinatorSettings = {
    ClusterId = "test_cluster"
    TargetScenarios = ["test_scenario"]
    MqttServer = "localhost"
    Agents = agents
}

let agentSettings = {
    ClusterId = "test_cluster"
    TargetGroup = "1"
    MqttServer = "localhost"
}

let customSettings = ""

let okStep = Step.create("ok step", fun _ -> task {
    do! Task.Delay(TimeSpan.FromSeconds(0.1))
    return Response.Ok()
})

let private scenario =
    Scenario.create "test_scenario" [okStep]
    |> Scenario.withDuration(TimeSpan.FromSeconds 1.0)
    |> NBomber.Domain.Scenario.create

let scenarioSettings = {
    ScenarioName = "test_scenario"
    ConcurrentCopies = 1
    WarmUpDuration = DateTime(TimeSpan.FromSeconds(1.0).Ticks)
    Duration = DateTime(TimeSpan.FromSeconds(1.0).Ticks)
}

let internal isWarmUpScenarios (status) =
    match status with
    | ScenarioHostStatus.Working st when st = WorkingState.WarmUpScenarios -> true
    | _ -> false

[<Fact>]
let ``Coordinator can run as single bomber`` () = async {
    let dep = Dependency.createFor(NodeType.Coordinator)
    let server = MqttTests.startMqttServer()
    // specified no agents
    
    let coordinatorSettings = { coordinatorSettings with Agents = List.empty }
    let scnArgs = {
        SessionId = dep.SessionId
        ScenariosSettings = [| scenarioSettings |]
        TargetScenarios = Array.empty
        CustomSettings = customSettings
    }
    let registeredScenarios = [| scenario |]
    
    // we start coordinator without agents
    let! coordinator = Coordinator.init(dep, registeredScenarios, coordinatorSettings, scnArgs)
    let! statsResult = Coordinator.run(coordinator)
    let allStats = statsResult |> Result.defaultValue Array.empty
    
    Coordinator.stop coordinator
    MqttTests.stopMqttServer server
    
    test <@ allStats.Length = 1 @>
    test <@ allStats.[0].Meta.Sender = NodeType.Coordinator @>
    test <@ allStats.[0].OkCount > 1 @>
}

[<Fact>]
let ``Coordinator should be able to start bombing even when agents are offline`` () = async {
    let dep = Dependency.createFor(NodeType.Coordinator)
    let server = MqttTests.startMqttServer()
    // specified agents in config
    
    let coordinatorSettings = { coordinatorSettings with Agents = agents }
    let scnArgs = {
        SessionId = dep.SessionId
        ScenariosSettings = [| scenarioSettings |]
        TargetScenarios = Array.empty
        CustomSettings = customSettings
    }
    let registeredScenarios = [| scenario |]
    
    // we start coordinator without agents even though we specified agents in config
    let! coordinator = Coordinator.init(dep, registeredScenarios, coordinatorSettings, scnArgs)
    let! statsResult = Coordinator.run(coordinator)
    let allStats = statsResult |> Result.defaultValue Array.empty
        
    Coordinator.stop coordinator
    MqttTests.stopMqttServer server
    
    test <@ allStats.Length = 1 @>
    test <@ allStats.[0].Meta.Sender = NodeType.Coordinator @>
    test <@ allStats.[0].OkCount > 1 @>
}

[<Fact>]
let ``Coordinator and agents should attack together`` () = async {
    let coordinatorDep = Dependency.createFor(NodeType.Coordinator)
    let agentDep = Dependency.createFor(NodeType.Agent)
    let server = MqttTests.startMqttServer()
    
    let scnArgs = {
        SessionId = coordinatorDep.SessionId
        ScenariosSettings = [| scenarioSettings |]
        TargetScenarios = Array.empty
        CustomSettings = customSettings
    }
    let registeredScenarios = [| scenario |]
    
    // we start coordinator with one agent
    let! coordinator = Coordinator.init(coordinatorDep, registeredScenarios, coordinatorSettings, scnArgs)
    let! agent = Agent.init(agentDep, [| scenario |], agentSettings)
    Agent.startListening(agent) |> Async.Start
    let! statsResult = Coordinator.run(coordinator)    
    let allStats = statsResult |> Result.defaultValue Array.empty
    
    let coordinatorStats = allStats |> Array.find(fun x -> x.Meta.Sender = NodeType.Coordinator)
    let agentStats = allStats |> Array.find(fun x -> x.Meta.Sender = NodeType.Agent)
    
    Agent.stop agent
    Coordinator.stop coordinator
    MqttTests.stopMqttServer server
    
    test <@ allStats.Length = 2 @>
    test <@ coordinatorStats.OkCount > 1 @>
    test <@ agentStats.OkCount > 1 @>
}

[<Fact>]
let ``Changing cluster topology should not affect a current test execution`` () = async {
    
    let coordinatorDep = Dependency.createFor(NodeType.Coordinator)
    let agentDep = Dependency.createFor(NodeType.Agent)
    let server = MqttTests.startMqttServer()
    let scenarioSettings = { scenarioSettings with WarmUpDuration = DateTime(TimeSpan.FromSeconds(5.0).Ticks) }
    
    let scnArgs = {
        SessionId = coordinatorDep.SessionId
        ScenariosSettings = [| scenarioSettings |]
        TargetScenarios = Array.empty
        CustomSettings = customSettings
    }
    let registeredScenarios = [| scenario |]
    
    // we start coordinator with one agent    
    let! coordinator = Coordinator.init(coordinatorDep, registeredScenarios, coordinatorSettings, scnArgs)
    let! agent1 = Agent.init(agentDep, [| scenario |], agentSettings)
    Agent.startListening(agent1) |> Async.Start
    let coordinatorTask = Coordinator.run(coordinator) |> Async.StartAsTask
    
    // waiting until agent starts bombing
    while not <| isWarmUpScenarios(agent1.ScenariosHost.Status) do
        do! Async.Sleep(1_000)
        
    // spin up a new agent
    let! agent2 = Agent.init(agentDep, [| scenario |], agentSettings)
    Agent.startListening(agent2) |> Async.Start
    
    let! statsResult = coordinatorTask |> Async.AwaitTask
    let allStats = statsResult |> Result.defaultValue Array.empty
    
    let coordinatorStats = allStats |> Array.find(fun x -> x.Meta.Sender = NodeType.Coordinator)
    let agentStats = allStats |> Array.find(fun x -> x.Meta.Sender = NodeType.Agent)
    
    Agent.stop agent1
    Agent.stop agent2
    Coordinator.stop coordinator
    MqttTests.stopMqttServer server

    test <@ allStats.Length = 2 @> // it should be '3' if agent2 joined the test
    test <@ coordinator.Agents.Count = 2 @> // coordinator has found two agents
    test <@ coordinatorStats.OkCount > 1 @>
    test <@ agentStats.OkCount > 1 @>
}

[<Fact>]
let ``Coordinator should be able to propagate all types of settings among the agents`` () = async {
    
    let coordinatorDep = Dependency.createFor(NodeType.Coordinator)
    let agentDep = Dependency.createFor(NodeType.Agent)
    let server = MqttTests.startMqttServer()
    
    // set up custom settings
    let agents = [
        { TargetGroup = "1"; TargetScenarios = ["test_scenario"] }
    ]
    let coordinatorSettings = { coordinatorSettings with Agents = agents }
    let scenarioSettings = { scenarioSettings with ConcurrentCopies = 5 }
    let customSettings = "{ Age: 28 }"
        
    let scnArgs = {
        SessionId = coordinatorDep.SessionId
        ScenariosSettings = [| scenarioSettings |]
        TargetScenarios = Array.empty
        CustomSettings = customSettings
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
    let! coordinator = Coordinator.init(coordinatorDep, registeredScenarios, coordinatorSettings, scnArgs)
    let! agent = Agent.init(agentDep, registeredScenarios, agentSettings)
    Agent.startListening(agent) |> Async.Start
    let! statsResult = Coordinator.run(coordinator)
    
    Agent.stop agent
    Coordinator.stop coordinator
    MqttTests.stopMqttServer server          
        
    let customSettings = customSettingsList |> Seq.filter(fun x -> x.Value = customSettings) |> Seq.toArray
    let agentScenario = agent.ScenariosHost.GetRunningScenarios() |> Array.item 0
        
    test <@ customSettings.Length = 2 @> // because one from coordinator and one from agent
    test <@ scenario.ConcurrentCopies = NBomber.Domain.Constants.DefaultConcurrentCopies @>
    test <@ agentScenario.ConcurrentCopies = scenarioSettings.ConcurrentCopies @>
}

[<Fact>]
let ``Agent should run test only under their agent group`` () = async {
    
    let coordinatorDep = Dependency.createFor(NodeType.Coordinator)
    let agentDep = Dependency.createFor(NodeType.Agent)
    let server = MqttTests.startMqttServer()
    
    // set up agent group
    let agentSettings = { agentSettings with TargetGroup = "not_matched_group" }
    let agents = [
        { TargetGroup = "222"; TargetScenarios = ["test_scenario"] }
    ]
    let coordinatorSettings = { coordinatorSettings with Agents = agents }
    
    // set up scenarios    
    let scenario =
        Scenario.create "test_scenario" [okStep]
        |> Scenario.withDuration(TimeSpan.FromSeconds 1.0)        
        |> NBomber.Domain.Scenario.create
    
    let scnArgs = {
        SessionId = coordinatorDep.SessionId
        ScenariosSettings = [| scenarioSettings |]
        TargetScenarios = Array.empty
        CustomSettings = customSettings
    }
    let registeredScenarios = [| scenario |]
    
    // we start coordinator which will propagate settings to agent
    let! coordinator = Coordinator.init(coordinatorDep, registeredScenarios, coordinatorSettings, scnArgs)
    let! agent = Agent.init(agentDep, registeredScenarios, agentSettings)
    Agent.startListening(agent) |> Async.Start
    let! statsResult = Coordinator.run(coordinator)
    let allStats = statsResult |> Result.defaultValue Array.empty
    let agentStats = allStats |> Array.tryFind(fun x -> x.Meta.Sender = NodeType.Agent)
    
    Agent.stop agent
    Coordinator.stop coordinator
    MqttTests.stopMqttServer server
        
    test <@ agentStats.IsNone @>
}

[<Fact>]
let ``Coordinator and Agent should run tests only from TargetScenarios`` () = async {
    
    let coordinatorDep = Dependency.createFor(NodeType.Coordinator)
    let agentDep = Dependency.createFor(NodeType.Agent)
    let server = MqttTests.startMqttServer()
    
    // set up agent group
    let agentSettings = { agentSettings with TargetGroup = "222" }
    // agent will run only test_scenario_222
    let agents = [
        { TargetGroup = "222"; TargetScenarios = ["test_scenario_222"] }        
    ]
    // coordinator will run only test_scenario_111
    let coordinatorSettings = { coordinatorSettings with Agents = agents; TargetScenarios = ["test_scenario_111"] }
    let scnArgs = {
        SessionId = coordinatorDep.SessionId
        ScenariosSettings = [| scenarioSettings |]
        TargetScenarios = Array.empty
        CustomSettings = customSettings
    }
    
    // set up scenarios    
    let scenario_111 =
        Scenario.create "test_scenario_111" [okStep]
        |> Scenario.withDuration(TimeSpan.FromSeconds 1.0)        
        |> NBomber.Domain.Scenario.create
    
    let scenario_222 = { scenario_111 with ScenarioName = "test_scenario_222"  }
    let registeredScenarios = [| scenario_111; scenario_222 |]
    
    // we start coordinator which will propagate settings to agent
    let! coordinator = Coordinator.init(coordinatorDep, registeredScenarios, coordinatorSettings, scnArgs)
    let! agent = Agent.init(agentDep, registeredScenarios, agentSettings)
    Agent.startListening(agent) |> Async.Start
    let! statsResult = Coordinator.run(coordinator)
    let allStats = statsResult |> Result.defaultValue Array.empty             
    let coordinatorStats = allStats |> Array.find(fun x -> x.Meta.Sender = NodeType.Coordinator)
    let agentStats = allStats |> Array.find(fun x -> x.Meta.Sender = NodeType.Agent)
    
    Agent.stop agent
    Coordinator.stop coordinator
    MqttTests.stopMqttServer server
    
    test <@ coordinatorStats.AllScenariosStats.[0].ScenarioName = "test_scenario_111" @>
    test <@ agentStats.AllScenariosStats.[0].ScenarioName = "test_scenario_222" @>    
}

[<Fact>]
let ``Agent should be able to reconnect automatically and join the cluster`` () = async {
    
    let coordinatorDep = Dependency.createFor(NodeType.Coordinator)
    let (agentDep, loggerBuffer) = Dependency.createWithInMemoryLogger(NodeType.Agent)
    let server = MqttTests.startMqttServer()
    
    let scnArgs = {
        SessionId = coordinatorDep.SessionId
        ScenariosSettings = [| scenarioSettings |]
        TargetScenarios = Array.empty
        CustomSettings = customSettings
    }
    let registeredScenarios = [| scenario |]
    
    // we start one agent    
    let! agent = Agent.init(agentDep, [| scenario |], agentSettings)
    Agent.startListening(agent) |> Async.Start    
    
    // we stop mqtt server
    MqttTests.stopMqttServer server
    
    // and wait some time
    do! Async.Sleep(5_000)
    
    // spin up the mqtt server again to see that agent will reconnect
    let server = MqttTests.startMqttServer()
    let! coordinator = Coordinator.init(coordinatorDep, registeredScenarios, coordinatorSettings, scnArgs)
    
    // waiting on reconnect
    do! Async.Sleep(2_000)
    
    let! statsResult = Coordinator.run(coordinator)
    let allStats = statsResult |> Result.defaultValue Array.empty             
    let coordinatorStats = allStats |> Array.find(fun x -> x.Meta.Sender = NodeType.Coordinator)
    let agentStats = allStats |> Array.find(fun x -> x.Meta.Sender = NodeType.Agent)
    
    let logEvents = loggerBuffer.LogEvents |> Seq.toArray
    
    let agentDisconnectedEvents =
        logEvents
        |> Array.filter(fun x -> x.MessageTemplate.Text.Contains("{agent} disconnected from {cluster}"))
        
    let agentConnectedEvents =
        logEvents
        |> Array.filter(fun x -> x.MessageTemplate.Text.Contains("{agent} established connection with {cluster}"))
    
    Agent.stop agent    
    MqttTests.stopMqttServer server
    
    test <@ allStats.Length = 2 @>
    test <@ coordinatorStats.OkCount > 1 @>
    test <@ agentStats.OkCount > 1 @>
    test <@ agentDisconnectedEvents.Length = 1 @>
    test <@ agentConnectedEvents.Length = 2 @>
}