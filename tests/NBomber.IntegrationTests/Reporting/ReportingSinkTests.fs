module Tests.ReportingSink

open System
open System.Threading.Tasks

open Xunit
open Swensen.Unquote
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
        |> Scenario.withLoadSimulations [KeepConstant(copies = 5, during = seconds 30)]

    NBomberRunner.registerScenarios [scenario1]
    |> NBomberRunner.withReportFolder "./reporting-sinks/3/"
    |> NBomberRunner.withReportingSinks [reportingSink]
    |> NBomberRunner.withReportingInterval(seconds 5)
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let realtime = _realtimeStats.ToArray()
        test <@ realtime.Length > 0 @>
        test <@ realtime |> Array.filter(fun x -> x.CurrentOperation = OperationType.Complete) |> Array.length = 1 @>
        test <@ realtime |> Array.filter(fun x -> x.CurrentOperation = OperationType.Bombing) |> Array.length > 0 @>

        test <@ realtime |> Array.forall(fun x -> x.OkCount > 0) @>
        test <@ realtime |> Array.forall(fun x -> x.StepStats.[0].Ok.Request.Count > 0) @>
        test <@ realtime |> Array.forall(fun x -> x.StepStats.[0].Ok.Request.RPS > 0.0) @>

        test <@ realtime |> Array.forall(fun x -> x.FailCount = 0) @>
        test <@ realtime |> Array.forall(fun x -> x.StepStats.[0].Fail.Request.Count = 0) @>
        test <@ realtime |> Array.forall(fun x -> x.StepStats.[0].Fail.Request.RPS = 0.0) @>

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
        |> Scenario.withLoadSimulations [KeepConstant(copies = 5, during = seconds 30)]

    NBomberRunner.registerScenarios [scenario1]
    |> NBomberRunner.withReportFolder "./reporting-sinks/3/"
    |> NBomberRunner.withReportingSinks [reportingSink]
    |> NBomberRunner.withReportingInterval(seconds 5)
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let final = _finalStats.ToArray()
        test <@ final.[0] = nodeStats @>
        test <@ nodeStats.Duration = seconds 30 @>
        test <@ nodeStats.ScenarioStats |> Array.filter(fun x -> x.CurrentOperation = OperationType.Complete) |> Array.length = 1 @>

        test <@ nodeStats.ScenarioStats |> Array.forall(fun x -> x.OkCount > 0) @>
        test <@ nodeStats.ScenarioStats |> Array.forall(fun x -> x.StepStats.[0].Ok.Request.Count > 0) @>
        test <@ nodeStats.ScenarioStats |> Array.forall(fun x -> x.StepStats.[0].Ok.Request.RPS > 0.0) @>

        test <@ nodeStats.ScenarioStats |> Array.forall(fun x -> x.FailCount = 0) @>
        test <@ nodeStats.ScenarioStats |> Array.forall(fun x -> x.StepStats.[0].Fail.Request.Count = 0) @>
        test <@ nodeStats.ScenarioStats |> Array.forall(fun x -> x.StepStats.[0].Fail.Request.RPS = 0.0) @>
