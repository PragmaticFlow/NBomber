module Tests.ReportingSink

open System
open System.Threading.Tasks

open Xunit
open Swensen.Unquote
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.Domain
open NBomber.FSharp

//todo: test that multiply sink will be invoked correctly
//todo: test that stop timer stops sending metrics in case when stopping is still executing
//todo: test cluster stats

[<Fact>]
let ``SaveRealtimeStats should be invoked many times during test execution to send realtime stats`` () =

    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(milliseconds 100)
        return Response.Ok()
    })

    let scenario =
        Scenario.create "realtime stats scenario" [okStep]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConstant(copies = 5, during = seconds 40)
        ]

    let mutable statsInvokedCounter = 0

    let reportingSink = {
        new IReportingSink with
            member _.SinkName = "TestSink"
            member _.Init(_, _) = ()
            member _.Start(_) = Task.CompletedTask
            member _.SaveStats(_) =
                // 1 invoke per 5 sec
                statsInvokedCounter <- statsInvokedCounter + 1
                Task.CompletedTask

            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.withReportingSinks [reportingSink] (seconds 10)
    |> NBomberRunner.run
    |> ignore

    test <@ statsInvokedCounter >= 3 @> // 1 invoke as realtime and 1 invoke at the end

[<Fact>]
let ``SaveRealtimeStats should be invoked with correct operation Bombing`` () =

    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    let scenario =
        Scenario.create "realtime stats scenario" [okStep]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConstant(copies = 5, during = TimeSpan.FromSeconds 30.0)
        ]

    let mutable bombingCounter = 0
    let mutable completeCounter = 0

    let reportingSink = {
        new IReportingSink with
            member _.SinkName = "TestSink"
            member _.Init(_, _) = ()
            member _.Start(_) = Task.CompletedTask

            member _.SaveStats(stats) =
                match stats.[0].NodeInfo.CurrentOperation with
                | NodeOperationType.Bombing  -> bombingCounter <- bombingCounter + 1
                | NodeOperationType.Complete -> completeCounter <- completeCounter + 1
                | _                          -> failwith "operation type is invalid for SaveStats"
                Task.CompletedTask

            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.withReportingSinks [reportingSink] (seconds 10)
    |> NBomberRunner.run
    |> ignore

    test <@ bombingCounter > 0 && completeCounter = 1 @>
