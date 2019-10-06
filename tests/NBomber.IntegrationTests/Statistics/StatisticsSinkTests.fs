module Tests.StatisticsSinkTests

open System
open System.Threading.Tasks

open Xunit
open Swensen.Unquote
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.Domain
open NBomber.FSharp

[<Fact>]
let ``NBomberRunner.saveStatisticsTo should be invoked many times during test execution to send realtime stats`` () =
    
    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    let scenario =
        Scenario.create "realtime stats scenario" [okStep]
        |> Scenario.withDuration(TimeSpan.FromSeconds 10.0)

    let mutable statsInvokedCounter = 0

    let statsSync = { new IStatisticsSink with
                        member x.SaveStatistics(stats) =
                            // 1 invoke per 5 sec
                            statsInvokedCounter <- statsInvokedCounter + 1
                            Task.CompletedTask }
    
    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.saveStatisticsTo(statsSync)
    |> NBomberRunner.runTest
    
    test <@ statsInvokedCounter >= 2 @> // 1 invoke as realtime and 1 invoke at the end