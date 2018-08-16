module rec Tests

open System
open System.Threading.Tasks

open Xunit
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

let buildScenario () =

    let step1 = Step.createRequest("Step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(2.0))
        return Response.Ok()
    })

    Scenario.create("Scenario")
    |> Scenario.addTestFlow({ FlowName = "Flow"; Steps = [step1]; ConcurrentCopies = 100 })   
    |> Scenario.withDuration(TimeSpan.FromSeconds(10.0))


[<Fact>]
let ``XUnit test`` () =    
    
    let assertions = [
       Assertion.forScenario(fun stats -> stats.OkCount > 95)                
       Assertion.forTestFlow("Flow", fun stats -> stats.FailCount < 10)              
       Assertion.forStep("Step", "Flow", fun stats -> stats.OkCount > 80)
    ]

    buildScenario()
    |> Scenario.withAssertions(assertions)
    |> Scenario.runTest