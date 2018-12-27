module Tests.Step.Pull

open System
open System.Threading.Tasks

open Xunit
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

[<Fact>]
let ``Ok and Fail should be properly count`` () =
        
    let pool = ConnectionPool.none

    let okStep = Step.createPull("ok step", pool, fun context -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    let failStep = Step.createPull("fail step", pool, fun context -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Fail()
    })

    let assertions = [
       Assertion.forStep("ok step", fun stats -> stats.OkCount >= 4)
       Assertion.forStep("ok step", fun stats -> stats.OkCount < 6)
       Assertion.forStep("fail step", fun stats -> stats.FailCount >= 4)
       Assertion.forStep("fail step", fun stats -> stats.OkCount < 6)
    ]
    
    Scenario.create("pull test", [okStep; failStep])
    |> Scenario.withConcurrentCopies(1)    
    |> Scenario.withAssertions(assertions)
    |> Scenario.withWarmUpDuration(TimeSpan.FromSeconds(1.0))
    |> Scenario.withDuration(TimeSpan.FromSeconds(1.0))
    |> NBomberRunner.registerScenario
    |> NBomberRunner.runTest

[<Fact>]
let ``Min/Mean/Max/RPS/DataTransfer should be properly count`` () =
    
    let pool = ConnectionPool.none
    
    let pullStep = Step.createPull("pull step", pool, fun context -> task {                
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok(sizeBytes = 100)
    })

    let assertions = [
       Assertion.forStep("pull step", (fun stats -> stats.RPS >= 8), "RPS >= 8")
       Assertion.forStep("pull step", (fun stats -> stats.RPS <= 10), "RPS <= 10")        
       Assertion.forStep("pull step", (fun stats -> stats.Min <= 110), "Min <= 110")
       Assertion.forStep("pull step", (fun stats -> stats.Mean <= 120), "Mean <= 120")
       Assertion.forStep("pull step", (fun stats -> stats.Max <= 150), "Max <= 150")
       Assertion.forStep("pull step", (fun stats -> stats.DataMinKb = 0.1), "DataMinKb = 0.1")
       Assertion.forStep("pull step", (fun stats -> stats.AllDataMB >= 0.0017), "AllDataMB >= 0.0017")
    ]
    
    Scenario.create("latency count test", [pullStep])
    |> Scenario.withConcurrentCopies(1)
    |> Scenario.withAssertions(assertions)
    |> Scenario.withWarmUpDuration(TimeSpan.FromSeconds(1.0))
    |> Scenario.withDuration(TimeSpan.FromSeconds(2.0))
    |> NBomberRunner.registerScenario
    |> NBomberRunner.runTest