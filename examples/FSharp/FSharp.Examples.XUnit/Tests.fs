module Tests

open System
open System.Threading.Tasks

open Xunit
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

let buildScenario () =

    let pool = ConnectionPool.none

    let step1 = Step.createPull("Step", pool, fun context -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.5))
        return Response.Ok()
    })

    Scenario.create("xunit hello world", [step1])    

[<Fact>]
let ``XUnit test`` () =    
    
    let assertions = [       
       Assertion.forStep("Step", (fun stats -> stats.OkCount > 2), "OkCount > 2")
    ]

    buildScenario()
    |> Scenario.withConcurrentCopies(1)
    |> Scenario.withWarmUpDuration(TimeSpan.FromSeconds(1.0))
    |> Scenario.withDuration(TimeSpan.FromSeconds(2.0))
    |> Scenario.withAssertions(assertions)
    |> NBomberRunner.registerScenario
    |> NBomberRunner.runTest