module Tests.ReportingSink

open System.Data
open System.Threading.Tasks

open Serilog
open Serilog.Sinks.InMemory
open Serilog.Sinks.InMemory.Assertions
open Swensen.Unquote
open Xunit
open FSharp.Control.Tasks.NonAffine

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Domain
open NBomber.FSharp
open NBomber.Extensions.InternalExtensions

//todo: test that multiply sink will be invoked correctly
//todo: test that stop timer stops sending metrics in case when stopping is still executing
//todo: test cluster stats

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

    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })

    let scenario1 =
        Scenario.create "scenario_1" [okStep]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 30)]

    NBomberRunner.registerScenarios [scenario1]
    |> NBomberRunner.withReportFolder "./reporting-sinks/3/"
    |> NBomberRunner.withReportingSinks [reportingSink]
    |> NBomberRunner.withReportingInterval(seconds 5)
    |> NBomberRunner.run
    |> Result.getOk
    |> fun finalStats ->
        let realtime = _realtimeStats.ToArray()

        test <@ realtime.Length > 0 @>
        test <@ realtime |> Array.forall(fun x -> x.CurrentOperation = OperationType.Bombing) @>
        test <@ realtime |> Array.forall(fun x -> x.OkCount > 0) @>
        test <@ realtime |> Array.forall(fun x -> x.StepStats[0].Ok.Request.Count > 0) @>
        test <@ realtime |> Array.forall(fun x -> x.StepStats[0].Ok.Request.RPS > 0.0) @>
        test <@ realtime |> Array.forall(fun x -> x.FailCount = 0) @>
        test <@ realtime |> Array.forall(fun x -> x.StepStats[0].Fail.Request.Count = 0) @>
        test <@ realtime |> Array.forall(fun x -> x.StepStats[0].Fail.Request.RPS = 0.0) @>

        test <@ finalStats.NodeInfo.CurrentOperation = OperationType.Complete @>
        test <@ finalStats.ScenarioStats[0].StepStats[0].Ok.Request.Count > 0 @>
        test <@ finalStats.ScenarioStats[0].StepStats[0].Ok.Request.RPS > 0.0 @>
        test <@ finalStats.ScenarioStats[0].StepStats[0].Fail.Request.Count = 0 @>
        test <@ finalStats.ScenarioStats[0].StepStats[0].Fail.Request.RPS = 0.0 @>

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

    let okStep = Step.create("ok step", timeout = seconds 5, execute = fun context -> task {
        do! Task.Delay delay

        if context.InvocationCount = 5 then
            delay <- milliseconds 500
            size <- 500

        if context.InvocationCount = 15 then
            delay <- milliseconds 100
            size <- 100

        return Response.ok(sizeBytes = size)
    })

    let scenario1 =
        Scenario.create "scenario_1" [okStep]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 30)]

    NBomberRunner.registerScenarios [scenario1]
    |> NBomberRunner.withReportFolder "./reporting-sinks/3/"
    |> NBomberRunner.withReportingSinks [reportingSink]
    |> NBomberRunner.withReportingInterval(seconds 5)
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->

        let first = _realtimeStats[0]
        let last = _realtimeStats[_realtimeStats.Count - 1]

        test <@ first[0].StepStats[0].Ok.Latency.MaxMs > last[0].StepStats[0].Ok.Latency.MaxMs @>
        test <@ first[0].StepStats[0].Ok.Latency.MaxMs >= 1000.0  @>
        test <@ last[0].StepStats[0].Ok.Latency.MaxMs <= 1000.0  @>

        test <@ first[0].StepStats[0].Ok.Request.RPS <= 1.0 @>
        test <@ last[0].StepStats[0].Ok.Request.RPS <= 20.0 && last[0].StepStats[0].Ok.Request.RPS >= 5.0 @>

        test <@ first[0].StepStats[0].Ok.DataTransfer.MaxBytes > last[0].StepStats[0].Ok.DataTransfer.MaxBytes @>
        test <@ first[0].StepStats[0].Ok.DataTransfer.MaxBytes >= 1000  @>
        test <@ last[0].StepStats[0].Ok.DataTransfer.MaxBytes <= 1000  @>

[<Fact>]
let ``SaveRealtimeStats should receive correct calculated stats for long running steps`` () =

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

    let fastStep = Step.create("fast_step", timeout = seconds 30, execute = fun context -> task {
        do! Task.Delay(seconds 1)
        return Response.ok()
    })

    let longStep = Step.create("long_step", timeout = seconds 60, execute = fun context -> task {
        do! Task.Delay(seconds 30)
        return Response.ok()
    })

    let scenario1 =
        Scenario.create "scenario_1" [fastStep; longStep]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 10, during = seconds 20)]

    NBomberRunner.registerScenarios [scenario1]
    |> NBomberRunner.withReportFolder "./reporting-sinks/3/"
    |> NBomberRunner.withReportingSinks [reportingSink]
    |> NBomberRunner.withReportingInterval(seconds 5)
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let first = _realtimeStats.[0]

        test <@ first.[0].StepStats.[0].Ok.Request.Count = 10 @>
        test <@ first.[0].StepStats.[0].Ok.Request.RPS = 2 @>

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
                _finalStats.AddRange(stats)
                Task.CompletedTask

            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })

    let scenario1 =
        Scenario.create "scenario_1" [okStep]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 30)]

    NBomberRunner.registerScenarios [scenario1]
    |> NBomberRunner.withReportFolder "./reporting-sinks/3/"
    |> NBomberRunner.withReportingSinks [reportingSink]
    |> NBomberRunner.withReportingInterval(seconds 5)
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let final = _finalStats.ToArray()
        test <@ final[0] = nodeStats @>
        test <@ nodeStats.Duration = seconds 30 @>
        test <@ nodeStats.ScenarioStats |> Array.filter(fun x -> x.CurrentOperation = OperationType.Complete) |> Array.length = 1 @>

        test <@ nodeStats.ScenarioStats |> Array.forall(fun x -> x.OkCount > 0) @>
        test <@ nodeStats.ScenarioStats |> Array.forall(fun x -> x.StepStats[0].Ok.Request.Count > 0) @>
        test <@ nodeStats.ScenarioStats |> Array.forall(fun x -> x.StepStats[0].Ok.Request.RPS > 0.0) @>

        test <@ nodeStats.ScenarioStats |> Array.forall(fun x -> x.FailCount = 0) @>
        test <@ nodeStats.ScenarioStats |> Array.forall(fun x -> x.StepStats[0].Fail.Request.Count = 0) @>
        test <@ nodeStats.ScenarioStats |> Array.forall(fun x -> x.StepStats[0].Fail.Request.RPS = 0.0) @>

[<Fact>]
let ``PluginStats should return empty data set in case of execution timeout`` () =
    let inMemorySink = InMemorySink()
    let loggerConfig = fun () -> LoggerConfiguration().WriteTo.Sink(inMemorySink)

    let timeoutPlugin = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin"

            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask
            member _.GetStats(_) = task {
                do! Task.Delay(seconds 10) // we waiting more than default timeout = 5 sec
                return new DataSet()
            }
            member _.GetHints() = Array.empty
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    let step1 = Step.create("step 1", fun _ -> Task.FromResult(Response.ok()))
    Scenario.create "1" [step1]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(1, seconds 10)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withLoggerConfig loggerConfig
    |> NBomberRunner.withWorkerPlugins [timeoutPlugin]
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        test <@ Array.isEmpty nodeStats.PluginStats @>
        inMemorySink.Should().HaveMessage("Getting plugin stats failed with the timeout error", "because timeout has been reached") |> ignore

[<Fact>]
let ``PluginStats should return empty data set in case of internal exception`` () =
    let inMemorySink = InMemorySink()
    let loggerConfig = fun () -> LoggerConfiguration().WriteTo.Sink(inMemorySink)

    let exceptionPlugin = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask
            member _.GetStats(_) = failwith "test exception" // we throw exception
            member _.GetHints() = Array.empty
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    let step1 = Step.create("step 1", fun _ -> Task.FromResult(Response.ok()))
    Scenario.create "1" [step1]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(1, seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withLoggerConfig loggerConfig
    |> NBomberRunner.withWorkerPlugins [exceptionPlugin]
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        test <@ Array.isEmpty nodeStats.PluginStats @>
        inMemorySink.Should().HaveMessage("Getting plugin stats failed with the following error", "because exception was thrown") |> ignore
