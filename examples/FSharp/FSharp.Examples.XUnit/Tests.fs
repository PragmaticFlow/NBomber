module Tests

open System
open System.Threading.Tasks

open Xunit
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

let buildScenario () =

    let step1 = Step.create("simple step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok(sizeBytes = 1024)
    })

    Scenario.create "xunit hello world" [step1]

[<Fact>]
let ``XUnit test`` () =    
    
    let assertions = [       
       Assertion.forStep("simple step", (fun stats -> stats.OkCount > 2), "OkCount > 2")
       Assertion.forStep("simple step", (fun stats -> stats.RPS > 8), "RPS > 8")
       Assertion.forStep("simple step", (fun stats -> stats.Percent75 >= 102), "Percent75 >= 102")
       Assertion.forStep("simple step", (fun stats -> stats.DataMinKb = 1.0), "DataMinKb = 1.0")
       Assertion.forStep("simple step", (fun stats -> stats.AllDataMB >= 0.01), "AllDataMB >= 0.01")
    ]

    buildScenario()
    |> Scenario.withConcurrentCopies 1
    |> Scenario.withWarmUpDuration(TimeSpan.FromSeconds 0.0)
    |> Scenario.withDuration(TimeSpan.FromSeconds 2.0)
    |> Scenario.withAssertions assertions
    |> NBomberRunner.registerScenario
    |> NBomberRunner.runTest