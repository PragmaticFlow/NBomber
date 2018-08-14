module Tests

open Xunit
open NBomber.FSharp

[<Fact>]
let ``XUnit test`` () =    
    
    let assertions = [
       Assertion.forScenario(fun stats -> stats.OkCount > 95)                
       Assertion.forTestFlow("GET flow1", fun stats -> stats.FailCount < 10)              
       Assertion.forStep("GET flow", "GET github.com/VIP-Logic/NBomber html", fun stats -> stats.OkCount > 80)
    ]

    HttpScenario.buildScenario()
    |> Scenario.runTest(assertions)
