﻿module Tests.Scenario

open System
open System.Threading.Tasks

open Xunit
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.Domain
open NBomber.FSharp

[<Fact>]
let ``withTestClean should be invoked only once and not fail runner`` () =
    
    let mutable invokeCounter = 0

    let testClean = fun _ -> task {
        invokeCounter <- invokeCounter + 1
        failwith "exception was not handled"        
    }

    let pool = ConnectionPool.none

    let okStep = Step.create("ok step", pool, fun context -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    Scenario.create "withTestClean test" [okStep]
    |> Scenario.withTestClean testClean
    |> Scenario.withDuration(TimeSpan.FromSeconds 1.0)
    |> NBomberRunner.registerScenario
    |> NBomberRunner.runTest

    Assert.Equal(1, invokeCounter)

[<Fact>]
let ``runTest before starting test should validate assertion ScenarioName, StepName`` () =
    let pool = ConnectionPool.none

    let okStep = Step.create("Step1", pool, fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    let assertions = [
        Assertion.forStep("Step1", fun stats -> stats.OkCount = 1)
        Assertion.forStep("Step2", fun stats -> stats.OkCount = 1)
        Assertion.forStep("Step3", fun stats -> stats.OkCount = 1)
        Assertion.forStep("Step4", fun stats -> stats.OkCount = 1)
    ]

    let ex =
        Assert.ThrowsAny(fun () -> 
        Scenario.create "Scenario1" [okStep]
        |> Scenario.withAssertions assertions
        |> Scenario.withDuration(TimeSpan.FromSeconds 1.0)
        |> NBomberRunner.registerScenario
        |> NBomberRunner.runTest)

    Assert.True(
        ex.Message.Contains("Assertion is not found for step"),
        "Validation error message was not found in exception message")
    

