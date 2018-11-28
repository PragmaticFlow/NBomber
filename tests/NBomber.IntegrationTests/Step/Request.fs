module Tests.Step.Request

open System
open System.Threading.Tasks

open Xunit
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

[<Fact>]
let ``Response.Ok | Fail should be properly count`` () =
        
    let okStep = Step.createRequest("OK step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.5))
        return Response.Ok()
    })

    let failStep = Step.createRequest("Fail step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.5))
        return Response.Fail("")
    })

    let assertions = [
       Assertion.forScenario(fun stats -> stats.OkCount >= 2 && stats.FailCount >= 2)
    ]

    let scenario =
        Scenario.create("simple test", [okStep; failStep])
        |> Scenario.withConcurrentCopies(1)    
        |> Scenario.withAssertions(assertions)
        |> Scenario.withDuration(TimeSpan.FromSeconds(3.0))
    
    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.runTest
    