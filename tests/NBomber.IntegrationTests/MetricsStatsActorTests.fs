module Tests.MetricsStatsActor

open System
open System.Threading.Tasks

open FsCheck.Xunit
open NBomber.Domain.Stats.MetricsStatsActor
open Xunit
open Swensen.Unquote

open NBomber
open NBomber.Contracts.Stats
open NBomber.Contracts.Internal
open NBomber.Extensions.Internal
open NBomber.Domain.Stats.ScenarioStatsActor
open NBomber.FSharp
open Tests.TestHelper

[<Fact>]
let ``AllRealtimeStats should contain cached realtime stats`` () =

    let env = Dependency.createWithInMemoryLogger NodeType.SingleNode
    let metricsActor = new MetricsStatsActor(env.Dep.Logger)

    ()

    // for i in [1..10] do
    //     let result1 = { Name = "step_name"; ClientResponse = Response.ok()
    //                     CurrentTimeBucket = TimeSpan.Zero; Latency = seconds i }
    //     let result2 = { Name = Constants.ScenarioGlobalInfo; ClientResponse = Response.ok()
    //                     CurrentTimeBucket = TimeSpan.Zero; Latency = seconds i }
    //     metricsActor.Publish(AddMeasurement result1)
    //     metricsActor.Publish(AddMeasurement result2)
    //
    // let tcs = TaskCompletionSource<ScenarioStats>()
    // let loadStats = { SimulationName = ""; Value = 10 }
    // let duration = seconds 10
    // metricsActor.Publish(BuildReportingStats(tcs, loadStats, duration))
    // let realtimeStats = tcs.Task.Result
    //
    // test <@ metricsActor.AllRealtimeStats[duration].Ok.Request.Count = 10 @>
    // test <@ metricsActor.AllRealtimeStats[duration].StepStats[0].Ok.Request.Count = 10 @>
    // test <@ metricsActor.AllRealtimeStats[duration].StepStats[0].Fail.Request.Count = 0 @>
    // test <@ realtimeStats.Ok.Request.Count = 10 @>

