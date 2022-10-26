module Tests.ScenarioStatsActor

open System
open System.Threading.Tasks

open FsCheck.Xunit
open Xunit
open Swensen.Unquote

open NBomber
open NBomber.Contracts
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
        let result1 = { Name = "step_name"; ClientResponse = Response.ok(); EndTimeMs = float(100 + i); LatencyMs = i }
        let result2 = { Name = Constants.ScenarioGlobalInfo; ClientResponse = Response.ok(); EndTimeMs = float(100 + i); LatencyMs = i }
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

    statsActor.Publish StartUseTempBuffer

    for i in [1..10] do
        let result1 = { Name = "step_name"; ClientResponse = Response.ok(); EndTimeMs = float(100 + i); LatencyMs = i }
        let result2 = { Name = Constants.ScenarioGlobalInfo; ClientResponse = Response.ok(); EndTimeMs = float(100 + i); LatencyMs = i }
        statsActor.Publish(AddMeasurement result1)
        statsActor.Publish(AddMeasurement result2)

    let tcs = TaskCompletionSource<ScenarioStats>()
    let loadStats = { SimulationName = ""; Value = 10 }
    let duration = seconds 10
    statsActor.Publish(BuildReportingStats(tcs, loadStats, duration))
    let statsBufferEnabled = tcs.Task.Result

    statsActor.Publish FlushTempBuffer

    let tcs = TaskCompletionSource<ScenarioStats>()
    let loadStats = { SimulationName = ""; Value = 10 }
    let duration = seconds 10
    statsActor.Publish(BuildReportingStats(tcs, loadStats, duration))
    let statsBufferFlushed = tcs.Task.Result

    test <@ statsBufferEnabled.Ok.Request.Count = 0 @>
    test <@ statsBufferFlushed.Ok.Request.Count = 10 @>
    test <@ statsActor.AllRealtimeStats[duration].Ok.Request.Count = 10 @>

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
        let measurement = { Name = stName; ClientResponse = Response.ok(); EndTimeMs = float 100; LatencyMs = 100 }
        statsActor.Publish(AddMeasurement measurement)

    let globalStep = { Name = Constants.ScenarioGlobalInfo; ClientResponse = Response.ok(); EndTimeMs = float 100; LatencyMs = 100 }
    statsActor.Publish(AddMeasurement globalStep)

    let reply = TaskCompletionSource<ScenarioStats>()
    let simulation = { SimulationName = ""; Value = 9 }
    statsActor.Publish(BuildReportingStats(reply, simulation, seconds 10))
    let scnStats = reply.Task.Result

    stepNames
    |> List.iteri(fun i stName ->
        test <@ stName = scnStats.StepStats[i].StepName @>
    )
