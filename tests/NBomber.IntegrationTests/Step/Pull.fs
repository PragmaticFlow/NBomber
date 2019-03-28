module Tests.Step.Pull

open System
open System.Threading.Tasks

open Xunit
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

[<Fact>]
let ``Ok and Fail should be properly count`` () =       
    
    let mutable okCnt = 0
    let mutable failCnt = 0
    
    let okStep = Step.create("ok step", ConnectionPool.none, fun context -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        okCnt <- okCnt + 1
        return Response.Ok()
    })

    let failStep = Step.create("fail step", ConnectionPool.none, fun context -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        failCnt <- failCnt + 1
        return Response.Fail()
    })

    let assertions = [
       Assertion.forStep("ok step", fun stats -> [okCnt-1..okCnt] |> Seq.contains stats.OkCount)
       Assertion.forStep("ok step", fun stats -> stats.FailCount = 0)

       Assertion.forStep("fail step", fun stats -> stats.OkCount = 0)
       Assertion.forStep("fail step", fun stats -> [failCnt-1..failCnt] |> Seq.contains stats.FailCount)
    ]
    
    Scenario.create "count test" [okStep; failStep]
    |> Scenario.withConcurrentCopies 1
    |> Scenario.withAssertions assertions
    |> Scenario.withWarmUpDuration(TimeSpan.FromSeconds 0.0)
    |> Scenario.withDuration(TimeSpan.FromSeconds 1.0)
    |> NBomberRunner.registerScenario
    |> NBomberRunner.runTest

[<Fact>]
let ``Warmup should not have effect on stats`` () =       
    
    let okStep = Step.create("ok step", ConnectionPool.none, fun context -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    let failStep = Step.create("fail step", ConnectionPool.none, fun context -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Fail()
    })

    let assertions = [
       Assertion.forStep("ok step", fun stats -> stats.OkCount = 0 )
       Assertion.forStep("ok step", fun stats -> stats.FailCount = 0)

       Assertion.forStep("fail step", fun stats -> stats.OkCount = 0)
       Assertion.forStep("fail step", fun stats -> stats.FailCount = 0)
    ]
    
    Scenario.create "warmup test" [okStep; failStep]
    |> Scenario.withConcurrentCopies 1
    |> Scenario.withAssertions assertions
    |> Scenario.withWarmUpDuration(TimeSpan.FromSeconds 0.5)
    |> Scenario.withDuration(TimeSpan.FromSeconds 0.0)
    |> NBomberRunner.registerScenario
    |> NBomberRunner.runTest

[<Fact>]
let ``Min/Mean/Max/RPS/DataTransfer should be properly count`` () =
    
    let pullStep = Step.create("pull step", ConnectionPool.none, fun context -> task {                
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
    
    Scenario.create "latency count test" [pullStep]
    |> Scenario.withConcurrentCopies 1
    |> Scenario.withAssertions assertions
    |> Scenario.withWarmUpDuration(TimeSpan.FromSeconds 1.0)
    |> Scenario.withDuration(TimeSpan.FromSeconds 2.0)
    |> NBomberRunner.registerScenario
    |> NBomberRunner.runTest