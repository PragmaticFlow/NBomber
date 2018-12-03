module Tests.Step.Pull

open System
open System.Threading.Tasks

open Xunit
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

[<Fact>]
let ``Ok and Fail should be properly count`` () =
        
    let okStep = Step.createPull("ok step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    let failStep = Step.createPull("fail step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Fail("")
    })

    let assertions = [
       Assertion.forStep("ok step", fun stats -> stats.OkCount >= 4 && stats.OkCount < 6)
       Assertion.forStep("fail step", fun stats -> stats.FailCount >= 4 && stats.OkCount < 6)
    ]

    let scenario =
        Scenario.create("pull test", [okStep; failStep])
        |> Scenario.withConcurrentCopies(1)    
        |> Scenario.withAssertions(assertions)
        |> Scenario.withDuration(TimeSpan.FromSeconds(1.0))
    
    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.runTest
    