module Tests.Reporting.ReportingSinkTests

open System.Threading.Tasks

open Swensen.Unquote
open Xunit

open NBomber
open NBomber.Extensions.Internal
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Domain
open NBomber.FSharp
open Tests

//todo: test that multiply sink will be invoked correctly
//todo: test that stop timer stops sending metrics in case when stopping is still executing
//todo: test cluster stats

let createScenarios () =

    let scenario1 =
        Scenario.create("plugin scenario 1", fun ctx -> task {

            let! step1 = Step.run("step 1", ctx, fun () -> task {
                do! Task.Delay(seconds 0.1)
                return Response.ok()
            })

            let! step2 = Step.run("step 2", ctx, fun () -> task {
                do! Task.Delay(seconds 0.2)
                return Response.ok()
            })

            let! step3 = Step.run("step 3", ctx, fun () -> task {
                do! Task.Delay(seconds 0.3)
                return Response.ok()
            })

            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConstant(copies = 2, during = seconds 10)
        ]

    let scenario2 =
        Scenario.create("plugin scenario 2", fun ctx -> task {
            do! Task.Delay(seconds 0.3)
            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConstant(copies = 2, during = seconds 10)
        ]

    [scenario1; scenario2]

[<Fact>]
let ``SaveFinalStats should receive correct stats`` () =

    let _finalStats = ResizeArray<NodeStats>()

    let reportingSink = {
        new IReportingSink with
            member _.SinkName = "TestSink"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask
            member _.SaveRealtimeStats(stats) = Task.CompletedTask

            member _.SaveFinalStats(stats) =
                _finalStats.Add stats
                Task.CompletedTask

            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    let scenario1 =
        Scenario.create("scenario_1", fun ctx -> task {
            do! Task.Delay(milliseconds 100)
            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 30)]

    NBomberRunner.registerScenarios [scenario1]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.withReportingSinks [reportingSink]
    |> NBomberRunner.withReportingInterval(seconds 5)
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let final = _finalStats.ToArray()
        test <@ final[0] = nodeStats @>
        test <@ nodeStats.Duration = seconds 30 @>
        test <@ nodeStats.ScenarioStats |> Array.filter(fun x -> x.CurrentOperation = OperationType.Complete) |> Array.length = 1 @>

        test <@ nodeStats.ScenarioStats |> Array.forall(fun x -> x.Ok.Request.Count > 0) @>
        test <@ nodeStats.ScenarioStats |> Array.forall(fun x -> x.Ok.Request.Count > 0) @>
        test <@ nodeStats.ScenarioStats |> Array.forall(fun x -> x.Ok.Request.RPS > 0.0) @>

        test <@ nodeStats.ScenarioStats |> Array.forall(fun x -> x.Fail.Request.Count = 0) @>
        test <@ nodeStats.ScenarioStats |> Array.forall(fun x -> x.Fail.Request.Count = 0) @>
        test <@ nodeStats.ScenarioStats |> Array.forall(fun x -> x.Fail.Request.RPS = 0.0) @>

[<Fact>]
let ``SaveRealtimeStats should receive calculated stats by intervals`` () =

    let _realtimeStats = ResizeArray<ScenarioStats[]>()

    let reportingSink = {
        new IReportingSink with
            member _.SinkName = "TestSink"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask

            member _.SaveRealtimeStats(stats) =
                _realtimeStats.Add(stats)
                Task.CompletedTask

            member _.SaveFinalStats(stats) = Task.CompletedTask
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    let mutable delay = seconds 1
    let mutable size = 1000

    let scenario1 =
        Scenario.create("scenario_1", fun ctx -> task {
            do! Task.Delay delay

            if ctx.InvocationNumber = 5 then
                delay <- milliseconds 500
                size <- 500

            if ctx.InvocationNumber = 15 then
                delay <- milliseconds 100
                size <- 100

            return Response.ok(sizeBytes = size)
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 30)]

    NBomberRunner.registerScenarios [scenario1]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.withReportingSinks [reportingSink]
    |> NBomberRunner.withReportingInterval(seconds 5)
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->

        let first = _realtimeStats[0]
        let last = _realtimeStats[_realtimeStats.Count - 1]

        test <@ first[0].Ok.Latency.MaxMs > last[0].Ok.Latency.MaxMs @>
        test <@ first[0].Ok.Latency.MaxMs >= 1000.0  @>
        test <@ last[0].Ok.Latency.MaxMs <= 1000.0  @>

        test <@ first[0].Ok.Request.RPS <= 1.0 @>
        test <@ last[0].Ok.Request.RPS <= 20.0 && last[0].Ok.Request.RPS >= 5.0 @>

        test <@ first[0].Ok.DataTransfer.MaxBytes > last[0].Ok.DataTransfer.MaxBytes @>
        test <@ first[0].Ok.DataTransfer.MaxBytes >= 1000  @>
        test <@ last[0].Ok.DataTransfer.MaxBytes <= 1000  @>

[<Fact>]
let ``SaveRealtimeStats should receive correct calculated stats for long running steps`` () =

    let _realtimeStats = ResizeArray<ScenarioStats[]>()

    let reportingSink = {
        new IReportingSink with
            member _.SinkName = "TestSink"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask

            member _.SaveRealtimeStats(stats) =
                _realtimeStats.Add stats
                Task.CompletedTask

            member _.SaveFinalStats(stats) = Task.CompletedTask
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    let scenario1 =
        Scenario.create("scenario_1", fun ctx -> task {

            let! fastStep = Step.run("fast_step", ctx, fun () -> task {
                do! Task.Delay(seconds 1)
                return Response.ok()
            })

            let! fastStep = Step.run("long_step", ctx, fun () -> task {
                do! Task.Delay(seconds 30)
                return Response.ok()
            })

            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 10, during = seconds 20)]

    NBomberRunner.registerScenarios [scenario1]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.withReportingSinks [reportingSink]
    |> NBomberRunner.withReportingInterval(seconds 5)
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let first = _realtimeStats[0]

        test <@ first[0].StepStats[0].Ok.Request.Count = 10 @>
        test <@ first[0].StepStats[0].Ok.Request.RPS = 2 @>

[<Fact>]
let ``SaveRealtimeStats should receive correct stats`` () =

    let _realtimeStats = ResizeArray<ScenarioStats>()
    let mutable _finalStats = Unchecked.defaultof<NodeStats>

    let reportingSink = {
        new IReportingSink with
            member _.SinkName = "TestSink"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask

            member _.SaveRealtimeStats(stats) =
                _realtimeStats.AddRange(stats)
                Task.CompletedTask

            member _.SaveFinalStats(stats) = Task.CompletedTask
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    let scenario1 =
        Scenario.create("scenario_1", fun ctx -> task {
            do! Task.Delay(milliseconds 100)
            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 30)]

    NBomberRunner.registerScenarios [scenario1]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.withReportingSinks [reportingSink]
    |> NBomberRunner.withReportingInterval(seconds 5)
    |> NBomberRunner.run
    |> Result.getOk
    |> fun finalStats ->
        let realtime = _realtimeStats.ToArray()

        test <@ realtime.Length > 0 @>
        test <@ realtime |> Array.forall(fun x -> x.CurrentOperation = OperationType.Bombing) @>
        test <@ realtime |> Array.forall(fun x -> x.Ok.Request.Count > 0) @>
        test <@ realtime |> Array.forall(fun x -> x.Ok.Request.Count > 0) @>
        test <@ realtime |> Array.forall(fun x -> x.Ok.Request.RPS > 0.0) @>
        test <@ realtime |> Array.forall(fun x -> x.Fail.Request.Count = 0) @>
        test <@ realtime |> Array.forall(fun x -> x.Fail.Request.Count = 0) @>
        test <@ realtime |> Array.forall(fun x -> x.Fail.Request.RPS = 0.0) @>

        test <@ finalStats.NodeInfo.CurrentOperation = OperationType.Complete @>
        test <@ finalStats.ScenarioStats[0].Ok.Request.Count > 0 @>
        test <@ finalStats.ScenarioStats[0].Ok.Request.RPS > 0.0 @>
        test <@ finalStats.ScenarioStats[0].Fail.Request.Count = 0 @>
        test <@ finalStats.ScenarioStats[0].Fail.Request.RPS = 0.0 @>

[<Fact>]
let ``WorkerPlugin stats should be passed to IReportingSink`` () =

    let scenarios = createScenarios()
    let mutable _nodeStats = NodeStats.empty

    let plugin = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask
            member _.GetStats(_) = TestHelper.PluginStatisticsHelper.createPluginStats() |> Task.FromResult
            member _.GetHints() = Array.empty
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    let reportingSink = {
        new IReportingSink with
            member _.SinkName = "TestSink"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask

            member _.SaveRealtimeStats(_) = Task.CompletedTask

            member _.SaveFinalStats(stats) =
                _nodeStats <- stats
                Task.CompletedTask

            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.withoutReports
    |> NBomberRunner.withReportingSinks [reportingSink]
    |> NBomberRunner.withReportingInterval(seconds 10)
    |> NBomberRunner.withWorkerPlugins [plugin]
    |> NBomberRunner.run
    |> Result.mapError failwith
    |> ignore

    let pluginStats = _nodeStats.PluginStats[0]
    let table1 = pluginStats.Tables["PluginStatistics1Table"]
    let table2 = pluginStats.Tables["PluginStatistics2Table"]

    // assert on IReportingSink
    test <@ table1.Columns.Count > 0 @>
    test <@ table1.Rows.Count > 0 @>
    test <@ table2.Columns.Count > 0 @>
    test <@ table2.Rows.Count > 0 @>

