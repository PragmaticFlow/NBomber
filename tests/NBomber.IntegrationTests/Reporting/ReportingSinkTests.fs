module Tests.ReportingSink

open System
open System.Threading.Tasks

open Xunit
open Swensen.Unquote
open FSharp.Control.Tasks.NonAffine

open NBomber.Contracts
open NBomber.Domain
open NBomber.FSharp

//todo: test that multiply sink will be invoked correctly
//todo: test that stop timer stops sending metrics in case when stopping is still executing
//todo: test cluster stats

[<Fact>]
let ``SaveStats should be invoked many times during test execution to send realtime stats`` () =

    let mutable statsInvokedCounter = 0

    let reportingSink = {
        new IReportingSink with
            member _.SinkName = "TestSink"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask
            member _.SaveScenarioStats(_) =
                // 1 invoke per 5 sec
                statsInvokedCounter <- statsInvokedCounter + 1
                Task.CompletedTask

            member _.SaveFinalStats(_) = Task.CompletedTask
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    let okStep = Step.createAsync("ok step", fun _ -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })

    Scenario.create "realtime stats scenario" [okStep]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 5, during = seconds 40)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./reporting-sinks/1/"
    |> NBomberRunner.withReportingSinks [reportingSink] (seconds 10)
    |> NBomberRunner.run
    |> ignore

    test <@ statsInvokedCounter >= 3 @> // 1 invoke as realtime and 1 invoke at the end

[<Fact>]
let ``SaveStats should be invoked with OperationType = Complete only once`` () =

    let mutable bombingCounter = 0
    let mutable completeCounter = 0
    let mutable finalStatsCounter = 0

    let reportingSink = {
        new IReportingSink with
            member _.SinkName = "TestSink"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask

            member _.SaveScenarioStats(stats) =
                match stats.[0].CurrentOperation with
                | OperationType.Bombing  -> bombingCounter <- bombingCounter + 1
                | OperationType.Complete -> completeCounter <- completeCounter + 1
                | _                      -> failwith "operation type is invalid for SaveStats"
                Task.CompletedTask

            member _.SaveFinalStats(stats) =
                finalStatsCounter <- finalStatsCounter + 1
                Task.CompletedTask

            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    let okStep = Step.createAsync("ok step", fun _ -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })

    Scenario.create "realtime stats scenario" [okStep]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 5, during = seconds 30)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./reporting-sinks/2/"
    |> NBomberRunner.withReportingSinks [reportingSink] (seconds 10)
    |> NBomberRunner.run
    |> ignore

    test <@ bombingCounter > 0 @>
    test <@ completeCounter = 1 @>
    test <@ finalStatsCounter = 1 @>

[<Fact>]
let ``SaveStats for real-time reporting should contains only bombing stats`` () =

    let mutable scn1BombingInvokedCount = 0
    let mutable scn2BombingInvokedCount = 0
    let mutable scn1CompleteInvokedCount = 0
    let mutable scn2CompleteInvokedCount = 0

    let reportingSink = {
        new IReportingSink with
            member _.SinkName = "TestSink"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask

            member _.SaveScenarioStats(stats) =
                match stats.[0].CurrentOperation with
                | OperationType.Bombing ->
                    stats
                    |> Seq.filter(fun x -> x.ScenarioName = "scenario_1")
                    |> Seq.iter(fun _ -> scn1BombingInvokedCount <- scn1BombingInvokedCount + 1)

                    stats
                    |> Seq.filter(fun x -> x.ScenarioName = "scenario_2")
                    |> Seq.iter(fun _ -> scn2BombingInvokedCount <- scn2BombingInvokedCount + 1)

                | OperationType.Complete ->
                    stats
                    |> Seq.filter(fun x -> x.ScenarioName = "scenario_1")
                    |> Seq.iter(fun _ -> scn1CompleteInvokedCount <- scn1CompleteInvokedCount + 1)

                    stats
                    |> Seq.filter(fun x -> x.ScenarioName = "scenario_2")
                    |> Seq.iter(fun _ -> scn2CompleteInvokedCount <- scn2CompleteInvokedCount + 1)

                | _ -> failwith "operation type is invalid for SaveStats"

                Task.CompletedTask

            member _.SaveFinalStats(_) = Task.CompletedTask
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    let okStep = Step.createAsync("ok step", fun _ -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })

    let scenario1 =
        Scenario.create "scenario_1" [okStep]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 5, during = seconds 20)]

    let scenario2 =
        Scenario.create "scenario_2" [okStep]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 5, during = seconds 40)]

    NBomberRunner.registerScenarios [scenario1; scenario2]
    |> NBomberRunner.withReportFolder "./reporting-sinks/3/"
    |> NBomberRunner.withReportingSinks [reportingSink] (seconds 10)
    |> NBomberRunner.run
    |> ignore

    test <@ scn1BombingInvokedCount < scn2BombingInvokedCount @> // since scenario_2 has bigger duration
    test <@ scn1CompleteInvokedCount = scn2CompleteInvokedCount && scn2CompleteInvokedCount = 1 @>

[<Fact>]
let ``SaveStats receive 1 initial stats`` () =

    let mutable initialStatsCounter = 0

    let reportingSink = {
        new IReportingSink with
            member _.SinkName = "TestSink"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask

            member _.SaveScenarioStats(stats) =
                let s = stats.[0]
                if s.OkCount = 0 && s.Duration = TimeSpan.Zero then initialStatsCounter <- initialStatsCounter + 1
                Task.CompletedTask

            member _.SaveFinalStats(_) = Task.CompletedTask
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    let okStep = Step.createAsync("ok step", fun _ -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })

    Scenario.create "realtime stats scenario" [okStep]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 5, during = seconds 40)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./reporting-sinks/1/"
    |> NBomberRunner.withReportingSinks [reportingSink] (seconds 10)
    |> NBomberRunner.run
    |> ignore

    test <@ initialStatsCounter = 1 @> // 1 invoke as empty
