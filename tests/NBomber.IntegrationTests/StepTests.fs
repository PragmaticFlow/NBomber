module Tests.Step

open System
open System.Collections.Generic
open System.Threading.Tasks

open FsCheck.Xunit
open Xunit
open Swensen.Unquote
open FSharp.Control.Tasks.NonAffine

open NBomber
open NBomber.Contracts
open NBomber.Errors
open NBomber.FSharp
open NBomber.Extensions.InternalExtensions

[<Property>]
let ``Ok(payload: byte[]) should calculate SizeBytes automatically`` (payload: byte[]) =
    let response = Response.ok(payload)

    let actual = {| Size = response.SizeBytes |}

    if isNull payload then
        test <@ 0 = actual.Size @>
    else
        test <@ payload.Length = actual.Size @>

[<Fact>]
let ``Response Ok and Fail should be properly count`` () =

    let mutable okCnt = 0
    let mutable failCnt = 0

    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(milliseconds 100)
        okCnt <- okCnt + 1
        return Response.ok()
    })

    let failStep = Step.create("fail step", fun _ -> task {
        do! Task.Delay(milliseconds 100)
        failCnt <- failCnt + 1
        return Response.fail()
    })

    Scenario.create "count test" [okStep; failStep]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./steps-tests/1/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let allStepStats = nodeStats.ScenarioStats |> Seq.collect(fun x -> x.StepStats)
        let okSt = allStepStats |> Seq.find(fun x -> x.StepName = "ok step")
        let failSt = allStepStats |> Seq.find(fun x -> x.StepName = "fail step")

        test <@ okSt.Ok.Request.Count >= 5 && okSt.Ok.Request.Count <= 10 @>
        test <@ okSt.Fail.Request.Count = 0 @>
        test <@ failSt.Ok.Request.Count = 0 @>
        test <@ failSt.Fail.Request.Count > 5 && failSt.Fail.Request.Count <= 10 @>

[<Fact>]
[<Trait("CI", "disable")>]
let ``Min/Mean/Max/RPS/DataTransfer should be properly count`` () =

    let pullStep = Step.create("pull step", fun _ -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(sizeBytes = 1000)
    })

    Scenario.create "latency count test" [pullStep]
    |> Scenario.withWarmUpDuration(TimeSpan.FromSeconds 1.0)
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 10)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./steps-tests/2/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let stats = nodeStats.ScenarioStats
                    |> Seq.collect(fun x -> x.StepStats)
                    |> Seq.find(fun x -> x.StepName = "pull step")

        test <@ stats.Ok.Request.RPS >= 6.0 @>
        test <@ stats.Ok.Request.RPS <= 10.0 @>
        test <@ stats.Ok.Latency.MinMs <= 103.0 @>
        test <@ stats.Ok.Latency.MeanMs <= 110.0 @>
        test <@ stats.Ok.Latency.MaxMs <= 120.0 @>
        test <@ stats.Ok.DataTransfer.MinBytes = 1000 @>
        test <@ stats.Ok.DataTransfer.AllBytes >= 90_000L && stats.Ok.DataTransfer.AllBytes <= 100_000L @>

[<Fact>]
let ``can be duplicated to introduce repeatable behaviour`` () =

    let mutable repeatCounter = 0

    let repeatStep = Step.create("repeat_step", fun context -> task {
        do! Task.Delay(milliseconds 100)
        let number = context.GetPreviousStepResponse<int>()

        if number = 1 then repeatCounter <- repeatCounter + 1

        return Response.ok(number + 1)
    })

    Scenario.create "latency count test" [repeatStep; repeatStep]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 3)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./steps-tests/3/"
    |> NBomberRunner.run
    |> ignore

    test <@ repeatCounter > 5 @>

[<Fact>]
let ``StepContext Data should store any payload data from latest step.Response`` () =

    let step1 = Step.create("step 1", fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.InvocationCount)
    })

    let step2 = Step.create("step 2", fun context -> task {
        let prevStepRes = context.GetPreviousStepResponse<int>()
        if prevStepRes <> context.InvocationCount then return Response.fail()
        else return Response.ok()
    })

    Scenario.create "test context.Data" [step1; step2]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 5)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./steps-tests/4/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats -> test <@ stats.OkCount > 0 @>

[<Fact>]
let ``Step with DoNotTrack = true should has empty stats and not be printed`` () =

    let step1 = Step.create("step 1", fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })

    let step2 = Step.create("step 2", fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    }, doNotTrack = true)

    Scenario.create "test context.Data" [step1; step2]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 3)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./steps-tests/5/"
    |> NBomberRunner.runWithResult Array.empty
    |> Result.getOk
    |> fun result ->
        test <@ result.ScenarioStats.Length = 1 @>
        test <@ result.ScenarioStats
                |> Seq.collect(fun x -> x.StepStats)
                |> Seq.tryFind(fun x -> x.StepName = "step 2")
                |> Option.isNone @>

[<Fact>]
let ``createPause should work correctly and not printed in statistics`` () =

    let step1 = Step.create("step 1", fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })

    let pause4sec = Step.createPause(seconds 4)

    Scenario.create "test context.Data" [pause4sec; step1]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 3)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./steps-tests/6/"
    |> NBomberRunner.runWithResult Array.empty
    |> Result.getOk
    |> fun result ->
        test <@ result.ScenarioStats.Length = 1 @>

[<Fact>]
let ``NBomber should support to run and share the same step within one scenario and within several scenarios`` () =

    let step1 = Step.create("step 1", fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })

    let step2 = Step.create("step 2", fun context -> task {
        do! Task.Delay(milliseconds 500)
        return Response.ok()
    })

    let scenario1 =
        Scenario.create "scenario 1" [step1; step2]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 3)]

    let scenario2 =
        Scenario.create "scenario 2" [step2; step1]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 3)]

    let result =
        NBomberRunner.registerScenarios [scenario1; scenario2]
        |> NBomberRunner.withReportFolder "./steps-tests/7/"
        |> NBomberRunner.runWithResult Array.empty
        |> Result.getOk

    test <@ result.ScenarioStats.[0].StepStats.Length = 2 @>
    test <@ result.ScenarioStats.[1].StepStats.Length = 2 @>

[<Fact>]
let ``NBomber shouldn't stop execution scenario if too many failed results on a warm-up`` () =

    let step = Step.create("step", fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.fail()
    })

    Scenario.create "scenario" [step]
    |> Scenario.withWarmUpDuration(seconds 5)
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 10)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./steps-tests/8/"
    |> NBomberRunner.runWithResult Array.empty
    |> Result.getOk
    |> ignore

[<Fact>]
let ``NBomber should allow to set custom response latency and handle it properly`` () =

    let step = Step.create("step", fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(latencyMs = 2_000.0) // set custom latency
    })

    Scenario.create "scenario" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 3)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./steps-tests/9/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let st = nodeStats.ScenarioStats
                 |> Seq.collect(fun x -> x.StepStats)
                 |> Seq.find(fun x -> x.StepName = "step")

        test <@ st.Ok.Request.Count > 5 @>
        test <@ st.Ok.Request.RPS >= 7.0 @>
        test <@ st.Ok.Latency.MinMs <= 2_001.0 @>

[<Fact>]
let ``context StopTest should stop all scenarios`` () =

    let mutable counter = 0
    let duration = seconds 42

    let okStep = Step.create("ok step", fun context -> task {
        do! Task.Delay(milliseconds 100)
        counter <- counter + 1

        if counter >= 30 then
            context.StopCurrentTest(reason = "custom reason")

        return Response.ok()
    })

    let scenario1 =
        Scenario.create "test_youtube_1" [okStep]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(10, duration)]

    let scenario2 =
        Scenario.create "test_youtube_2" [okStep]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(10, duration)]

    NBomberRunner.registerScenarios [scenario1; scenario2]
    |> NBomberRunner.withReportFolder "./steps-tests/10/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        nodeStats.ScenarioStats
        |> Seq.find(fun x -> x.ScenarioName = "test_youtube_1")
        |> fun x -> test <@ x.Duration < duration @>

[<Fact>]
let ``NBomber should reset step invocation number after warm-up`` () =

    let mutable counter = 0

    let step = Step.create("step", fun context -> task {
        do! Task.Delay(seconds 1)
        counter <- context.InvocationCount
        return Response.ok()
    })

    Scenario.create "scenario" [step]
    |> Scenario.withWarmUpDuration(seconds 10)
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 10)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./steps-tests/11/"
    |> NBomberRunner.run
    |> ignore

    test <@ counter >= 5 && counter <= 11 @>

[<Fact>]
[<Trait("CI", "disable")>]
let ``NBomber should handle invocation number per step following shared-nothing approach`` () =

    let data = Dictionary<int,int>()

    let step = Step.create("step", timeout = seconds 2, execute = fun context -> task {
        do! Task.Delay(seconds 1, context.CancellationToken)

        data.[context.ScenarioInfo.ThreadNumber] <- context.InvocationCount

        return Response.ok()
    })

    Scenario.create "scenario" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 10, during = seconds 10)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./steps-tests/12/"
    |> NBomberRunner.run
    |> ignore

    // hack to fix the issue with iterating over mutable dictionary
    // it show up on slow machine
    Task.Delay(seconds 10).Wait()

    let maxNumber = data.Values |> Seq.maxBy(id)

    test <@ maxNumber >= 5 && maxNumber <= 11 @>

[<Fact>]
let ``NBomber should support synchronous step execution`` () =

    let mutable counter = 0

    let step = SyncApi.SyncStep.create("step", fun context ->
        Task.Delay(seconds 1).Wait()
        counter <- context.InvocationCount
        Response.ok()
    )

    Scenario.create "scenario" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 5, during = seconds 5)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./steps-tests/12/"
    |> NBomberRunner.run
    |> ignore

    test <@ counter > 0 && counter <= 6 @>

[<Fact>]
let ``StepContext Data should be cleared after each iteration`` () =

    let step1 = Step.create("step 1", fun context -> task {
        if context.Data.Count = 0 then return Response.ok()
        else return Response.fail()
    })

    let step2 = Step.create("step 2", fun context -> task {
        do! Task.Delay(100)
        context.Data.Add(nameof(context.InvocationCount), context.InvocationCount)
        return Response.ok();
    })

    Scenario.create "test context.Data" [step1; step2]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 5)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./steps-tests/13/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats -> test <@ stats.FailCount = 0 @>

[<Fact>]
let ``Response Payload should be cleared from StepContext Data after each iteration`` () =

    let step1 = Step.create("step 1", fun context -> task {
        let subject = context.GetPreviousStepResponse<int>()
        if Unchecked.defaultof<int> = subject then return Response.ok()
        else return Response.fail()
    })

    let step2 = Step.create("step 2", fun context -> task {
        do! Task.Delay(100)
        return Response.ok(context.InvocationCount);
    })

    Scenario.create "test context.Data" [step1; step2]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 5)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./steps-tests/14/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats -> test <@ stats.FailCount = 0 @>

[<Fact>]
let ``create should check feed on null and throw NRE`` () =
    Assert.Throws(
        typeof<NullReferenceException>,
        fun _ -> let nullFeed = Unchecked.defaultof<_>()
                 Step.create("null_feed", feed = nullFeed, execute = fun context -> task { return Response.ok() })
                 |> ignore
    )

[<Fact>]
let ``create should check clientFactory on null and throw NRE`` () =
    Assert.Throws(
        typeof<NullReferenceException>,
        fun _ -> let nullFactory = Unchecked.defaultof<_>()
                 Step.create("null_feed", clientFactory = nullFactory, execute = fun context -> task { return Response.ok() })
                 |> ignore
    )
