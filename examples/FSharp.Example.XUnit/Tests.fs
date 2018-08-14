module Tests

open Xunit
open NBomber.FSharp

[<Fact>]
let ``XUnit test`` () =    
    
    let assertions = [
       Assertion.forScenario(fun stats -> stats.OkCount > 95)                
       Assertion.forTestFlow("Flow", fun stats -> stats.FailCount < 10)              
       Assertion.forStep("Step", "Flow", fun stats -> stats.OkCount > 80)
    ]

    SimpleScenario.buildScenario()
    |> Scenario.runTest(assertions)
