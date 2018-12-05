module Tests

open System
open System.Threading.Tasks

open Xunit
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

let buildScenario () =

    let step1 = Step.createPull("Step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.5))
        return Response.Ok()
    })

    Scenario.create("xunit hello world", [step1])
    |> Scenario.withConcurrentCopies(1)
    |> Scenario.withDuration(TimeSpan.FromSeconds(2.0))

[<Fact>]
let ``XUnit test`` () =    
    
    let assertions = [       
       Assertion.forStep("Step", fun stats -> stats.OkCount > 2)
    ]

    buildScenario()
    |> Scenario.withAssertions(assertions)    
    |> NBomberRunner.registerScenario
    |> NBomberRunner.runTest