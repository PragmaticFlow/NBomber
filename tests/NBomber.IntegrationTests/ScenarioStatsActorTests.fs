module Tests.ScenarioStatsActor

open System
open System.Threading.Tasks

open FsCheck.Xunit
open Xunit
open Swensen.Unquote

open NBomber
open NBomber.Contracts.Stats
open NBomber.Contracts.Internal
open NBomber.Extensions.Internal
open NBomber.Domain.Stats.ScenarioStatsActor
open NBomber.FSharp
open Tests.TestHelper

let internal baseScenario =
    Scenario.create("scenario", fun ctx -> task {
        do! Task.Delay(seconds 0.5)
        return Response.ok()
    })
    |> NBomber.Domain.Scenario.createScenario
    |> Result.getOk

[<Fact>]
let ``AllRealtimeStats should contain cached realtime stats`` () =

    let env = Dependency.createWithInMemoryLogger NodeType.SingleNode
    let statsActor = ScenarioStatsActor(env.Dep.Logger, baseScenario, reportingInterval = seconds 5)

    for i in [1..10] do
        let result1 = { Name = "step_name"; ClientResponse = Response.ok()
                        StartTime = TimeSpan.Zero; Latency = seconds i }
        let result2 = { Name = Constants.ScenarioGlobalInfo; ClientResponse = Response.ok()
                        StartTime = TimeSpan.Zero; Latency = seconds i }
        statsActor.Publish(AddMeasurement result1)
        statsActor.Publish(AddMeasurement result2)

    let tcs = TaskCompletionSource<ScenarioStats>()
    let loadStats = { SimulationName = ""; Value = 10 }
    let duration = seconds 10
    statsActor.Publish(BuildReportingStats(tcs, loadStats, duration))
    let realtimeStats = tcs.Task.Result

    test <@ statsActor.AllRealtimeStats[duration].Ok.Request.Count = 10 @>
    test <@ statsActor.AllRealtimeStats[duration].StepStats[0].Ok.Request.Count = 10 @>
    test <@ statsActor.AllRealtimeStats[duration].StepStats[0].Fail.Request.Count = 0 @>
    test <@ realtimeStats.Ok.Request.Count = 10 @>

[<Fact>]
let ``TempBuffer should work correctly`` () =

    let env = Dependency.createWithInMemoryLogger NodeType.SingleNode
    let statsActor = ScenarioStatsActor(env.Dep.Logger, baseScenario, reportingInterval = seconds 5)    

    // add metrics that match the current reporting bucket
    let startTime = seconds 1
    for i in [1..5] do
        let result1 = { Name = "step_name"; ClientResponse = Response.ok(); StartTime = startTime; Latency = seconds i }
        let result2 = { Name = Constants.ScenarioGlobalInfo; ClientResponse = Response.ok(); StartTime = startTime; Latency = seconds i }
        statsActor.Publish(AddMeasurement result1)
        statsActor.Publish(AddMeasurement result2)
        
    // add metrics that StartTime is bigger than the current reporting bucket
    let startTime = seconds 6
    for i in [1..10] do
        let result1 = { Name = "step_name"; ClientResponse = Response.ok(); StartTime = startTime; Latency = seconds i }
        let result2 = { Name = Constants.ScenarioGlobalInfo; ClientResponse = Response.ok(); StartTime = startTime; Latency = seconds i }
        statsActor.Publish(AddMeasurement result1)
        statsActor.Publish(AddMeasurement result2)        

    let tcs = TaskCompletionSource<ScenarioStats>()
    let loadStats = { SimulationName = ""; Value = 10 }
    let fiveSec = seconds 5
    statsActor.Publish(BuildReportingStats(tcs, loadStats, fiveSec))    

    let tcs = TaskCompletionSource<ScenarioStats>()
    let loadStats = { SimulationName = ""; Value = 10 }
    let tenSec = seconds 10
    statsActor.Publish(BuildReportingStats(tcs, loadStats, tenSec))
    
    test <@ statsActor.AllRealtimeStats[fiveSec].Ok.Request.Count = 5 @>
    test <@ statsActor.AllRealtimeStats[tenSec].Ok.Request.Count = 10 @>

[<Property>]
let ``BuildReportingStats should preserver Steps order at which steps arrived`` (stepNames: string list) =

    let env = Dependency.createWithInMemoryLogger NodeType.SingleNode
    let statsActor = ScenarioStatsActor(env.Dep.Logger, baseScenario, reportingInterval = seconds 5)

    let stepNames =
        stepNames
        |> List.filter(String.IsNullOrEmpty >> not)
        |> List.groupBy id
        |> List.map fst

    for stName in stepNames do
        let measurement = { Name = stName; ClientResponse = Response.ok(); StartTime = TimeSpan.Zero; Latency = seconds 100 }
        statsActor.Publish(AddMeasurement measurement)

    let globalStep = { Name = Constants.ScenarioGlobalInfo; ClientResponse = Response.ok(); StartTime = TimeSpan.Zero; Latency = seconds 100 }
    statsActor.Publish(AddMeasurement globalStep)

    let reply = TaskCompletionSource<ScenarioStats>()
    let simulation = { SimulationName = ""; Value = 9 }
    statsActor.Publish(BuildReportingStats(reply, simulation, seconds 10))
    let scnStats = reply.Task.Result

    stepNames
    |> List.iteri(fun i stName ->
        test <@ stName = scnStats.StepStats[i].StepName @>
    )

[<Fact>]
let ``DataTransfer should be calculated properly for Global Step Info`` () =

    let env = Dependency.createWithInMemoryLogger NodeType.SingleNode
    let statsActor = ScenarioStatsActor(env.Dep.Logger, baseScenario, reportingInterval = seconds 5)

    for i in [1 .. 100] do
        let stepSmall = { Name = "step_small"; ClientResponse = Response.ok(sizeBytes = 1); StartTime = TimeSpan.Zero; Latency = seconds 100 }
        statsActor.Publish(AddMeasurement stepSmall)

    let stepBig = { Name = "step_big"; ClientResponse = Response.ok(sizeBytes = 1000); StartTime = TimeSpan.Zero; Latency = seconds 100 }
    statsActor.Publish(AddMeasurement stepBig)

    let stepGlobal = { Name = Constants.ScenarioGlobalInfo; ClientResponse = Response.ok(sizeBytes = 5); StartTime = TimeSpan.Zero; Latency = seconds 100 }
    statsActor.Publish(AddMeasurement stepGlobal)

    let reply = TaskCompletionSource<ScenarioStats>()
    let simulation = { SimulationName = ""; Value = 9 }
    statsActor.Publish(BuildReportingStats(reply, simulation, seconds 10))
    let scnStats = reply.Task.Result

    // GlobalInfoDataSize stats should be cleared
    let stepBig = { Name = "step_big"; ClientResponse = Response.ok(sizeBytes = 1000); StartTime = TimeSpan.Zero; Latency = seconds 100 }
    statsActor.Publish(AddMeasurement stepBig)

    let reply = TaskCompletionSource<ScenarioStats>()
    statsActor.Publish(BuildReportingStats(reply, simulation, seconds 10))
    let scnStats2 = reply.Task.Result

    // 100 (step_small) + 1000 (step_big) + 5 (global_info)
    test <@ scnStats.AllBytes = 1105 @>
    test <@ scnStats.Ok.DataTransfer.AllBytes = 1105 @>
    test <@ scnStats.Fail.DataTransfer.AllBytes = 0 @>
    test <@ scnStats.Ok.DataTransfer.Percent99 = 1105 @>
    test <@ scnStats.GetStepStats("step_small").Ok.DataTransfer.AllBytes = 100 @>
    test <@ scnStats.GetStepStats("step_big").Ok.DataTransfer.AllBytes = 1000 @>

    test <@ scnStats2.Ok.DataTransfer.AllBytes = 0 @>
    test <@ scnStats2.AllBytes = 1000 @>
