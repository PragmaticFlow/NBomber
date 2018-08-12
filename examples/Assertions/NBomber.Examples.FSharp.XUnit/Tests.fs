module Tests

open System
open Xunit

open NBomber.FSharp
open NBomber.Examples.FSharp.Scenarios
open NBomber

[<Fact>]
let ``My test`` () =
    let assertions = [|
       Assertion.forScenario(fun stats -> stats.OkCount > 95)
       Assertion.forScenario(fun stats -> false)
       Assertion.forScenario(fun stats -> stats.FailCount > 95)
       Assertion.forScenario(fun stats -> stats.OkCount > 0)
       Assertion.forScenario(fun stats -> stats.OkCount > 0)
       Assertion.forScenario(fun stats -> stats.OkCount > 95)
       Assertion.forTestFlow("GET flow1", fun stats -> stats.FailCount < 10)
       Assertion.forTestFlow("GET flow", fun stats -> false)
       Assertion.forStep("GET flow", "GET github.com/VIP-Logic/NBomber html", fun stats -> false)
       Assertion.forStep("GET flow", "GET github.com/VIP-Logic/NBomber html", fun stats -> Seq.exists (fun i -> i = stats.FailCount) [80;95])
       Assertion.forStep("GET flow", "GET github.com/VIP-Logic/NBomber html", fun stats -> stats.OkCount > 80 && stats.OkCount > 95)
    |]

    HttpScenario.buildScenario() |> Scenario.applyAssertions(assertions)
