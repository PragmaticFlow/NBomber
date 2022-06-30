module Tests.ScenarioStatsActor

open System.Threading.Tasks

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

let step = Step.create("step", fun ctx -> task {
    do! Task.Delay(seconds 0.5)
    return Response.ok()
})

let internal baseScenario =
    Scenario.create "scenario" [step]
    |> NBomber.Domain.Scenario.createScenario
    |> Result.getOk

[<Fact>]
let ``AllRealtimeStats should contain cached realtime stats`` () =

    let env = Dependency.createWithInMemoryLogger NodeType.SingleNode
    let statsActor = ScenarioStatsActor(env.Dep.Logger, baseScenario, reportingInterval = seconds 5)

    for i in [1..10] do
        let res = { StepIndex = 0; ClientResponse = Response.ok(); EndTimeMs = float(100 + i); LatencyMs = i }
        statsActor.Publish(AddResponse res)

    let tcs = TaskCompletionSource<ScenarioStats>()
    let loadStats = { SimulationName = ""; Value = 10 }
    let duration = seconds 10
    statsActor.Publish(BuildRealtimeStats(tcs, loadStats, duration))
    let realtimeStats = tcs.Task.Result

    test <@ statsActor.AllRealtimeStats[duration].RequestCount = 10 @>
    test <@ realtimeStats.RequestCount = 10 @>

[<Fact>]
let ``TempBuffer should work correctly`` () =

    let env = Dependency.createWithInMemoryLogger NodeType.SingleNode
    let statsActor = ScenarioStatsActor(env.Dep.Logger, baseScenario, reportingInterval = seconds 5)

    statsActor.Publish StartUseTempBuffer

    for i in [1..10] do
        let res = { StepIndex = 0; ClientResponse = Response.ok(); EndTimeMs = float(100 + i); LatencyMs = i }
        statsActor.Publish(AddResponse res)

    let tcs = TaskCompletionSource<ScenarioStats>()
    let loadStats = { SimulationName = ""; Value = 10 }
    let duration = seconds 10
    statsActor.Publish(BuildRealtimeStats(tcs, loadStats, duration))
    let statsBufferEnabled = tcs.Task.Result

    statsActor.Publish FlushTempBuffer

    let tcs = TaskCompletionSource<ScenarioStats>()
    let loadStats = { SimulationName = ""; Value = 10 }
    let duration = seconds 10
    statsActor.Publish(BuildRealtimeStats(tcs, loadStats, duration))
    let statsBufferFlushed = tcs.Task.Result

    test <@ statsBufferEnabled.RequestCount = 0 @>
    test <@ statsBufferFlushed.RequestCount = 10 @>
    test <@ statsActor.AllRealtimeStats[duration].RequestCount = 10 @>

