module Tests

open System
open System.Threading.Tasks

open Xunit
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

let buildScenario () =

    let step1 = Step.createRequest("Step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.5))
        return Response.Ok()
    })

    Scenario.create("xunit hello world", [step1])
    |> Scenario.withConcurrentCopies(1)
    |> Scenario.withDuration(TimeSpan.FromSeconds(2.0))

[<Fact>]
let ``XUnit test`` () =    
    
    let assertions = [
       Assertion.forScenario(fun stats -> stats.OkCount > 2)
       Assertion.forStep("Step", fun stats -> stats.OkCount > 2)
    ]

    let scenario = buildScenario() |> Scenario.withAssertions(assertions)
    
    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.runTest