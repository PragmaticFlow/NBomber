﻿module Tests.Step.Pull

open System
open System.Threading.Tasks

open Xunit
open Swensen.Unquote
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.Errors
open NBomber.FSharp
open NBomber.Extensions

[<Fact>]
let ``Ok and Fail should be properly count`` () =

    let mutable okCnt = 0
    let mutable failCnt = 0

    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        okCnt <- okCnt + 1
        return Response.Ok()
    })

    let failStep = Step.create("fail step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        failCnt <- failCnt + 1
        return Response.Fail()
    })

    let scenario =
        Scenario.create "count test" [okStep; failStep]
        |> Scenario.withConcurrentCopies 1
        |> Scenario.withOutWarmUp
        |> Scenario.withDuration(TimeSpan.FromSeconds 1.0)

    let result = NBomberRunner.registerScenarios [scenario]
                 |> NBomberRunner.runTest

    match result with
    | Ok allStats ->
        let okStats = allStats |> Array.find(fun x -> x.StepName = "ok step")
        let failStats = allStats |> Array.find(fun x -> x.StepName = "fail step")

        test <@ [okCnt-1..okCnt] |> Seq.contains okStats.OkCount @>
        test <@ okStats.FailCount = 0 @>
        test <@ failStats.OkCount = 0 @>
        test <@ [failCnt-1..failCnt] |> Seq.contains failStats.FailCount @>

    | Error msg -> failwith msg

[<Fact>]
let ``Warmup should not have effect on stats`` () =

    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    let failStep = Step.create("fail step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Fail()
    })

    let scenario =
        Scenario.create "warmup test" [okStep; failStep]
        |> Scenario.withConcurrentCopies 1
        |> Scenario.withWarmUpDuration(TimeSpan.FromSeconds 3.0)
        |> Scenario.withDuration(TimeSpan.FromSeconds 1.0)

    let result = NBomberRunner.registerScenarios [scenario]
                 |> NBomberRunner.runTest

    match result with
    | Ok allStats ->
        let okStats = allStats |> Array.find(fun x -> x.StepName = "ok step")
        let failStats = allStats |> Array.find(fun x -> x.StepName = "fail step")

        test <@ okStats.OkCount <= 5 @>
        test <@ okStats.FailCount = 0 @>
        test <@ failStats.OkCount = 0 @>
        test <@ failStats.FailCount <= 5 @>

    | Error msg -> failwith msg

[<Fact>]
let ``Min/Mean/Max/RPS/DataTransfer should be properly count`` () =

    let pullStep = Step.create("pull step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok(sizeBytes = 100)
    })

    let scenario =
        Scenario.create "latency count test" [pullStep]
        |> Scenario.withConcurrentCopies 1
        |> Scenario.withWarmUpDuration(TimeSpan.FromSeconds 1.0)
        |> Scenario.withDuration(TimeSpan.FromSeconds 2.0)

    let result = NBomberRunner.registerScenarios [scenario]
                 |> NBomberRunner.runTest

    match result with
    | Ok allStats ->
        let stats = allStats |> Array.find(fun x -> x.StepName = "pull step")

        test <@ stats.RPS >= 8 @>
        test <@ stats.RPS <= 10 @>
        test <@ stats.Min <= 110 @>
        test <@ stats.Mean <= 120 @>
        test <@ stats.Max <= 150 @>
        test <@ stats.DataMinKb = 0.1 @>
        test <@ stats.AllDataMB >= 0.0015 @>

    | Error msg -> failwith msg

[<Fact>]
let ``repeatCount should set how many times one step will be repeated`` () =

    let mutable invokeCounter = 0

    let repeatStep = Step.create("repeat step", fun _ -> task {
        invokeCounter <- invokeCounter + 1

        if invokeCounter = 5 then
            invokeCounter <- 0

        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    }, repeatCount = 5)

    let resetStep = Step.create("reset step", fun _ -> task {
        if invokeCounter > 0 then
            invokeCounter <- 20
        return Response.Ok()
    })

    let scenario =
        Scenario.create "latency count test" [repeatStep; resetStep]
        |> Scenario.withOutWarmUp
        |> Scenario.withDuration(TimeSpan.FromSeconds 3.0)
        |> Scenario.withConcurrentCopies 1

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.runTest
    |> ignore

    Assert.True(invokeCounter <= 5)

[<Fact>]
let ``context.Data should store any payload data from latest step.Response`` () =

    let mutable counter = 0
    let mutable step2Counter = 0
    let mutable counterFromStep1 = null

    let step1 = Step.create("step 1", fun context -> task {
        counter <- counter + 1
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok(counter)
    })

    let step2 = Step.create("step 2", fun context -> task {
        step2Counter <- counter
        counterFromStep1 <- context.Data
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    let scenario =
        Scenario.create "test context.Data" [step1; step2]
        |> Scenario.withOutWarmUp
        |> Scenario.withDuration(TimeSpan.FromSeconds 3.0)
        |> Scenario.withConcurrentCopies 1

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.runTest
    |> ignore

    Assert.Equal(Convert.ToInt32(counterFromStep1), step2Counter)

[<Fact>]
let ``Step with DoNotTrack = true should has empty stats and not be printed`` () =

    let step1 = Step.create("step 1", fun context -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    let step2 = Step.create("step 2", fun context -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    }, doNotTrack = true)

    let scenario =
        Scenario.create "test context.Data" [step1; step2]
        |> Scenario.withOutWarmUp
        |> Scenario.withDuration(TimeSpan.FromSeconds 3.0)
        |> Scenario.withConcurrentCopies 1

    let result =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.runWithResult
        |> Result.getOk


    test <@ result.Statistics.Length = 1 @>
    test <@ result.Statistics
            |> Array.tryFind(fun x -> x.StepName = "step 2")
            |> Option.isNone  @>

[<Fact>]
let ``Step.CreatePause should work correctly and not printed in statistics`` () =

    let mutable step1Invoked = false

    let step1 = Step.create("step 1", fun context -> task {
        step1Invoked <- true
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    let pause4sec = Step.createPause(TimeSpan.FromSeconds 4.0)

    let scenario =
        Scenario.create "test context.Data" [pause4sec; step1]
        |> Scenario.withOutWarmUp
        |> Scenario.withDuration(TimeSpan.FromSeconds 3.0)
        |> Scenario.withConcurrentCopies 1

    let result =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.runWithResult
        |> Result.getOk

    test <@ step1Invoked = false @>
    test <@ result.Statistics.Length = 1 @>

[<Fact>]
let ``NBomber should support to run and share the same step within one scenario and within several scenarios`` () =

    let step1 = Step.create("step 1", fun context -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    let step2 = Step.create("step 2", fun context -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.5))
        return Response.Ok()
    })

    let scenario1 =
        Scenario.create "scenario 1" [step1; step2]
        |> Scenario.withOutWarmUp
        |> Scenario.withDuration(TimeSpan.FromSeconds 3.0)
        |> Scenario.withConcurrentCopies 1

    let scenario2 =
        Scenario.create "scenario 2" [step2; step1]
        |> Scenario.withOutWarmUp
        |> Scenario.withDuration(TimeSpan.FromSeconds 3.0)
        |> Scenario.withConcurrentCopies 1

    let result =
        NBomberRunner.registerScenarios [scenario1; scenario2]
        |> NBomberRunner.runWithResult
        |> Result.getOk

    test <@ result.Statistics.Length = 4 @>

[<Fact>]
let ``NBomber should stop execution scenario if too many failed results on a warm-up`` () =

    let step = Step.create("step", fun context -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Fail()
    })

    let scenario =
        Scenario.create "scenario" [step]
        |> Scenario.withWarmUpDuration(TimeSpan.FromSeconds 1.0)
        |> Scenario.withDuration(TimeSpan.FromSeconds 60.0)
        |> Scenario.withConcurrentCopies 1

    let result =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.runWithResult
        |> Result.getError

    let warmUpErrorFound =
        match result with
        | Domain error -> match error with
                          | WarmUpErrorWithManyFailedSteps _ -> true
                          | _ -> false
        | _ -> false

    test <@ warmUpErrorFound = true @>
