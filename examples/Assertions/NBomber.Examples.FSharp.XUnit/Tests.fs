module Tests

open System
open Xunit

open NBomber.FSharp
open NBomber.Examples.FSharp.Scenarios
open NBomber

[<Fact>]
let ``My test`` () =
    let assertions = [|
       Assert.forScenario(fun stats -> stats.OkCount > 95)
       Assert.forScenario(fun stats -> false)
       Assert.forScenario(fun stats -> stats.FailCount > 95)
       Assert.forScenario(fun stats -> stats.OkCount > 0)
       Assert.forScenario(fun stats -> stats.OkCount > 0)
       Assert.forScenario(fun stats -> stats.OkCount > 95)
       Assert.forTestFlow("GET flow1", fun stats -> stats.FailCount < 10)
       Assert.forTestFlow("GET flow", fun stats -> false)
       Assert.forStep("GET flow", "GET github.com/VIP-Logic/NBomber html", fun stats -> false)
       Assert.forStep("GET flow", "GET github.com/VIP-Logic/NBomber html", fun stats -> Seq.exists (fun i -> i = stats.FailCount) [80;95])
       Assert.forStep("GET flow", "GET github.com/VIP-Logic/NBomber html", fun stats -> stats.OkCount > 80 && stats.OkCount > 95)
    |]

    HttpScenario.buildScenario() |> Scenario.applyAssertions(assertions)
