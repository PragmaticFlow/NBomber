module Tests.Step.BasicStepTests

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks

open FsCheck.Xunit
open Xunit
open Swensen.Unquote

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.FSharp
open NBomber.Extensions.Internal

[<Fact>]
let ``Response Ok and Fail should be properly count`` () =

    let mutable okCnt = 0
    let mutable failCnt = 0

    Scenario.create("count test", fun ctx -> task {

        let! ok = Step.run("ok step", ctx, fun () -> task {
            do! Task.Delay(milliseconds 100)
            okCnt <- okCnt + 1
            return Response.ok()
        })

        let! fail = Step.run("fail step", ctx, fun () -> task {
            do! Task.Delay(milliseconds 100)
            failCnt <- failCnt + 1
            return Response.fail()
        })

        return Response.ok()
    })
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let okSt = nodeStats.ScenarioStats[0].GetStepStats("ok step")
        let failSt = nodeStats.ScenarioStats[0].GetStepStats("fail step")

        test <@ okSt.Ok.Request.Count >= 5 && okSt.Ok.Request.Count <= 10 @>
        test <@ okSt.Fail.Request.Count = 0 @>
        test <@ failSt.Ok.Request.Count = 0 @>
        test <@ failSt.Fail.Request.Count > 5 && failSt.Fail.Request.Count <= 10 @>

[<Fact>]
[<Trait("CI", "disable")>]
let ``Min/Mean/Max/RPS/DataTransfer should be properly count`` () =

    Scenario.create("latency count test", fun ctx -> backgroundTask {
        do! Task.Delay(milliseconds 100)
        return Response.ok(sizeBytes = 1000)
    })
    |> Scenario.withWarmUpDuration(seconds 2)
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 10)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let stats = nodeStats.ScenarioStats[0]
        let ok = stats.Ok

        test <@ ok.Request.RPS >= 8.0 @>
        test <@ ok.Request.RPS <= 10.0 @>
        test <@ ok.Latency.MinMs <= 103.0 @>
        test <@ ok.Latency.MeanMs <= 111.0 @>
        test <@ ok.Latency.MaxMs <= 125.0 @>
        test <@ ok.DataTransfer.MinBytes = 1000 @>
        test <@ ok.DataTransfer.AllBytes >= 90_000L && ok.DataTransfer.AllBytes <= 100_000L @>

// [<Fact>]
// let ``can be duplicated to introduce repeatable behaviour`` () =
//
//     let mutable repeatCounter = 0
//
//     let repeatStep = Step.create("repeat_step", fun context -> task {
//         do! Task.Delay(milliseconds 100)
//         let number = context.GetPreviousStepResponse<int>()
//
//         if number = 1 then repeatCounter <- repeatCounter + 1
//
//         return Response.ok(number + 1)
//     })
//
//     Scenario.create "latency count test" [repeatStep; repeatStep]
//     |> Scenario.withoutWarmUp
//     |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 3)]
//     |> NBomberRunner.registerScenario
//     |> NBomberRunner.withoutReports
//     |> NBomberRunner.run
//     |> ignore
//
//     test <@ repeatCounter > 5 @>

// [<Fact>]
// let ``StepContext Data should store any payload data from latest step.Response`` () =
//
//     let step1 = Step.create("step 1", fun context -> task {
//         do! Task.Delay(milliseconds 100)
//         return Response.ok(context.InvocationNumber)
//     })
//
//     let step2 = Step.create("step 2", fun context -> task {
//         let prevStepRes = context.GetPreviousStepResponse<int>()
//         if prevStepRes <> context.InvocationNumber then return Response.fail()
//         else return Response.ok()
//     })
//
//     Scenario.create "test context.Data" [step1; step2]
//     |> Scenario.withoutWarmUp
//     |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 5)]
//     |> NBomberRunner.registerScenario
//     |> NBomberRunner.withoutReports
//     |> NBomberRunner.run
//     |> Result.getOk
//     |> fun stats -> test <@ stats.OkCount > 0 @>

// [<Fact>]
// let ``NBomber should support to run and share the same step within one scenario and within several scenarios`` () =
//
//     let step1 = Step.create("step 1", fun context -> task {
//         do! Task.Delay(milliseconds 100)
//         return Response.ok()
//     })
//
//     let step2 = Step.create("step 2", fun context -> task {
//         do! Task.Delay(milliseconds 500)
//         return Response.ok()
//     })
//
//     let scenario1 =
//         Scenario.create "scenario 1" [step1; step2]
//         |> Scenario.withoutWarmUp
//         |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 3)]
//
//     let scenario2 =
//         Scenario.create "scenario 2" [step2; step1]
//         |> Scenario.withoutWarmUp
//         |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 3)]
//
//     let result =
//         NBomberRunner.registerScenarios [scenario1; scenario2]
//         |> NBomberRunner.withoutReports
//         |> NBomberRunner.runWithResult Seq.empty
//         |> Result.getOk
//
//     test <@ result.FinalStats.ScenarioStats[0].StepStats.Length = 2 @>
//     test <@ result.FinalStats.ScenarioStats[1].StepStats.Length = 2 @>

[<Fact>]
let ``NBomber shouldn't stop execution scenario if too many failed results on a warm-up`` () =

    Scenario.create("scenario", fun ctx -> task {
        do! Task.Delay(milliseconds 100)
        return Response.fail()
    })
    |> Scenario.withWarmUpDuration(seconds 5)
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 10)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.runWithResult Array.empty
    |> Result.getOk
    |> ignore

[<Fact>]
let ``NBomber should allow to set custom response latency and handle it properly`` () =

    Scenario.create("scenario", fun ctx -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(latencyMs = 2_000.0) // set custom latency
    })
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 3)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let scnStats = nodeStats.ScenarioStats[0]

        test <@ scnStats.Ok.Request.Count > 5 @>
        test <@ scnStats.Ok.Request.RPS >= 7.0 @>
        test <@ scnStats.Ok.Latency.MinMs <= 2_001.0 @>

[<Fact>]
let ``context StopTest should stop all scenarios`` () =

    let mutable counter = 0
    let duration = seconds 42

    let scenario1 =
        Scenario.create("test_youtube_1", fun ctx -> task {
            do! Task.Delay(milliseconds 100)
            counter <- counter + 1

            if counter >= 30 then
                ctx.StopCurrentTest(reason = "custom reason")

            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(10, duration)]

    let scenario2 =
        Scenario.create("test_youtube_2", fun ctx -> task {
            do! Task.Delay(milliseconds 100)
            counter <- counter + 1

            if counter >= 30 then
                ctx.StopCurrentTest(reason = "custom reason")

            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(10, duration)]

    NBomberRunner.registerScenarios [scenario1; scenario2]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        nodeStats.ScenarioStats
        |> Seq.find(fun x -> x.ScenarioName = "test_youtube_1")
        |> fun x -> test <@ x.Duration < duration @>

[<Fact>]
let ``NBomber should reset step invocation number after warm-up`` () =

    let mutable counter = 0

    Scenario.create("scenario", fun ctx -> task {
        do! Task.Delay(seconds 1)
        counter <- ctx.InvocationNumber
        return Response.ok()
    })
    |> Scenario.withWarmUpDuration(seconds 10)
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 10)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> ignore

    test <@ counter >= 5 && counter <= 11 @>

[<Fact>]
[<Trait("CI", "disable")>]
let ``NBomber should handle invocation number per step following shared-nothing approach`` () =

    let data = Dictionary<int,int>()

    Scenario.create("scenario", fun ctx -> task {
        do! Task.Delay(seconds 1)

        data[ctx.ScenarioInfo.ThreadNumber] <- ctx.InvocationNumber

        return Response.ok()
    })
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 10, during = seconds 10)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> ignore

    // hack to fix the issue with iterating over mutable dictionary
    // it show up on slow machine
    Task.Delay(seconds 10).Wait()

    let maxNumber = data.Values |> Seq.maxBy(id)

    test <@ maxNumber >= 5 && maxNumber <= 11 @>

[<Fact>]
let ``NBomber by default should reset scenario iteration on step fail`` () =

    let mutable step3Invoked = false

    Scenario.create("scenario", fun ctx -> task {

        let! step1 = Step.run("step1", ctx, fun () -> task {
            return Response.ok()
        })

        let! step2 = Step.run("step2", ctx, fun () -> task {
            return Response.fail()
        })

        let! step3 = Step.run("step3", ctx, fun () -> task {
            step3Invoked <- true // this step should not be invoked
            return Response.ok()
        })

        return Response.ok()
    })
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 1)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        test <@ step3Invoked = false @>
        test <@ stats.ScenarioStats[0].Ok.Request.Count = 0 @>
        test <@ stats.ScenarioStats[0].Fail.Request.Count > 0 @>
        test <@ stats.ScenarioStats[0].Fail.Request.Count = stats.ScenarioStats[0].GetStepStats("step2").Fail.Request.Count @>
        test <@ stats.ScenarioStats[0].GetStepStats("step1").Ok.Request.Count > 0 @>

[<Fact>]
let ``withResetIterationOnFail should allow to configure reset = false`` () =

    let mutable step3Invoked = false

    Scenario.create("scenario", fun ctx -> task {

        let! step1 = Step.run("step1", ctx, fun () -> task {
            return Response.ok()
        })

        let! step2 = Step.run("step2", ctx, fun () -> task {
            return Response.fail()
        })

        let! step3 = Step.run("step3", ctx, fun () -> task {
            step3Invoked <- true // this step should be invoked
            return Response.ok()
        })

        return Response.ok()
    })
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 1)]
    |> Scenario.withResetIterationOnFail false
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        test <@ step3Invoked = true @>
        test <@ stats.ScenarioStats[0].Ok.Request.Count > 0 @>
        test <@ stats.ScenarioStats[0].Fail.Request.Count = 0 @>
        test <@ stats.ScenarioStats[0].GetStepStats("step1").Ok.Request.Count > 0 @>
        test <@ stats.ScenarioStats[0].GetStepStats("step2").Fail.Request.Count > 0 @>
        test <@ stats.ScenarioStats[0].GetStepStats("step3").Ok.Request.Count > 0 @>

[<Fact>]
let ``operation timeout should be tracked properly`` () =

    let scn1 =
        Scenario.create("scenario_1", fun ctx -> task {

            let! step1 = Step.run("step1", ctx, fun () -> task {
                use timeout = new CancellationTokenSource()
                timeout.CancelAfter 50

                do! Task.Delay(100, timeout.Token)

                return Response.ok()
            })

            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 1)]

    let scn2 =
        Scenario.create("scenario_2", fun ctx -> task {
            use timeout = new CancellationTokenSource()
            timeout.CancelAfter 50

            do! Task.Delay(100, timeout.Token)

            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 1)]

    NBomberRunner.registerScenarios [scn1; scn2]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        let scn1Stats = stats.GetScenarioStats("scenario_1")
        let scn2Stats = stats.GetScenarioStats("scenario_2")

        test <@ scn1Stats.Fail.StatusCodes[0].IsError @>
        test <@ scn1Stats.Fail.StatusCodes[0].StatusCode = Constants.TimeoutStatusCode @>
        test <@ scn1Stats.Fail.StatusCodes[0].Count = scn1Stats.Fail.Request.Count @>

        test <@ scn2Stats.Fail.StatusCodes[0].IsError @>
        test <@ scn2Stats.Fail.StatusCodes[0].StatusCode = Constants.TimeoutStatusCode @>
        test <@ scn2Stats.Fail.StatusCodes[0].Count = scn2Stats.Fail.Request.Count @>

[<Fact>]
let ``unhandled exceptions should have a proper status code`` () =

    let scn1 =
        Scenario.create("scenario_1", fun ctx -> task {

            let! step1 = Step.run("step1", ctx, fun () -> task {

                do! Task.Delay 100

                failwith "my exception"

                return Response.ok()
            })

            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 1)]

    let scn2 =
        Scenario.create("scenario_2", fun ctx -> task {

            do! Task.Delay 100

            failwith "my exception"

            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 1)]

    NBomberRunner.registerScenarios [scn1; scn2]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        let scn1Stats = stats.GetScenarioStats("scenario_1")
        let scn2Stats = stats.GetScenarioStats("scenario_2")

        test <@ scn1Stats.Fail.StatusCodes[0].IsError @>
        test <@ scn1Stats.Fail.StatusCodes[0].StatusCode = Constants.UnhandledExceptionCode @>
        test <@ scn1Stats.Fail.StatusCodes[0].Count = scn1Stats.Fail.Request.Count @>
        test <@ scn1Stats.Fail.StatusCodes[0].Message = "my exception" @>

        test <@ scn2Stats.Fail.StatusCodes[0].IsError @>
        test <@ scn2Stats.Fail.StatusCodes[0].StatusCode = Constants.UnhandledExceptionCode @>
        test <@ scn2Stats.Fail.StatusCodes[0].Count = scn2Stats.Fail.Request.Count @>
        test <@ scn1Stats.Fail.StatusCodes[0].Message = "my exception" @>

[<Fact>]
let ``Response Ok message should be presented in status codes`` () =

    Scenario.create("scenario", fun ctx -> task {

        let! step = Step.run("step", ctx, fun () -> task {
            do! Task.Delay 100
            return Response.ok(statusCode = "200", message = "my message 1")
        })

        return Response.ok(statusCode = "300", message = "my message 2")
    })
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 1)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        let scnStats = stats.GetScenarioStats("scenario")

        test <@ scnStats.Ok.StatusCodes[0].IsError = false @>
        test <@ scnStats.Ok.StatusCodes[0].StatusCode = "200" @>
        test <@ scnStats.Ok.StatusCodes[0].Count = scnStats.StepStats[0].Ok.Request.Count @>
        test <@ scnStats.Ok.StatusCodes[0].Message = "my message 1" @>

        test <@ scnStats.Ok.StatusCodes[1].IsError = false @>
        test <@ scnStats.Ok.StatusCodes[1].StatusCode = "300" @>
        test <@ scnStats.Ok.StatusCodes[1].Count = scnStats.Ok.Request.Count @>
        test <@ scnStats.Ok.StatusCodes[1].Message = "my message 2" @>

// [<Fact>]
// let ``create should check feed on null and throw NRE`` () =
//     Assert.Throws(
//         typeof<NullReferenceException>,
//         fun _ -> let nullFeed = Unchecked.defaultof<_>()
//                  Step.create("null_feed", feed = nullFeed, execute = fun context -> task { return Response.ok() })
//                  |> ignore
//     )

// [<Fact>]
// let ``create should check clientFactory on null and throw NRE`` () =
//     Assert.Throws(
//         typeof<NullReferenceException>,
//         fun _ -> let nullFactory = Unchecked.defaultof<_>()
//                  Step.create("null_feed", clientFactory = nullFactory, execute = fun context -> task { return Response.ok() })
//                  |> ignore
//     )

// [<Fact>]
// let ``create should allow set step timeout`` () =
//
//     let step1 = Step.create("step 1", timeout = milliseconds 500, execute = fun context -> task {
//         do! Task.Delay(milliseconds 100)
//         return Response.ok()
//     })
//
//     let step2 = Step.create("step 2", timeout = milliseconds 500, execute = fun context -> task {
//         do! Task.Delay(600)
//         return Response.ok()
//     })
//
//     Scenario.create "timeout tests" [step1; step2]
//     |> Scenario.withoutWarmUp
//     |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 5)]
//     |> NBomberRunner.registerScenario
//     |> NBomberRunner.withoutReports
//     |> NBomberRunner.run
//     |> Result.getOk
//     |> fun stats ->
//         test <@ stats.ScenarioStats[0].GetStepStats("step 1").Fail.Request.Count = 0 @>
//         test <@ stats.ScenarioStats[0].GetStepStats("step 2").Fail.Request.Count > 0 @>
//         test <@ stats.ScenarioStats[0].GetStepStats("step 2").Fail.StatusCodes[0].StatusCode = Constants.TimeoutStatusCode @>
