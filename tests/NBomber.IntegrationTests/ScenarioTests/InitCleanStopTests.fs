﻿module Tests.Scenario.InitCleanStopTests

open System
open System.Threading.Tasks
open Swensen.Unquote
open Xunit
open Microsoft.Extensions.Configuration
open NBomber
open NBomber.FSharp
open NBomber.Extensions.Internal
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Domain

//todo: add test on TestInit and TestClean should be executed only for scenario.Enabled
//todo: test on context.StopScenario("name") and check what realtime stats it provides

[<CLIMutable>]
type TestCustomSettings = {
    TargetHost: string
    MsgSizeInBytes: int
    PauseMs: int
}

[<Fact>]
let ``TestClean should be invoked only once and not fail runner`` () =

    let mutable cleanInvokeCounter = 0

    let testClean = fun _ -> task {
        cleanInvokeCounter <- cleanInvokeCounter + 1
        failwith "exception was not handled"
    }

    Scenario.create("withTestClean test", fun ctx -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })
    |> Scenario.withClean testClean
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(2,  seconds 1)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> ignore

    test <@ 1 = cleanInvokeCounter @>

[<Fact>]
let ``TestInit should propagate CustomSettings from config.json`` () =

    let mutable scnContext: IScenarioInitContext option = None

    let testInit = fun context -> task {
        scnContext <- Some context
    }

    Scenario.create("test_youtube", fun ctx -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })
    |> Scenario.withInit testInit
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(2,  seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.loadConfig "Configuration/test_config.json"
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> ignore

    let customSettings = scnContext.Value.CustomSettings.Get<TestCustomSettings>()

    test <@ customSettings.TargetHost = "localhost" @>
    test <@ customSettings.MsgSizeInBytes = 1000 @>

[<Fact>]
let ``should be stopped via StepContext.StopScenario`` () =

    let mutable counter = 0
    let duration = seconds 5

    let scenario1 =
        Scenario.create("test_youtube_1", fun ctx -> task {
            do! Task.Delay(milliseconds 100)
            counter <- counter + 1

            if counter = 5 then
                ctx.StopScenario("test_youtube_1", "custom reason")

            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(10, duration)]

    let scenario2 =
        Scenario.create("test_youtube_2", fun ctx -> task {
            do! Task.Delay(milliseconds 100)
            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(10, duration)]

    NBomberRunner.registerScenarios [scenario1; scenario2]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let youtube1Steps =
            nodeStats.ScenarioStats
            |> Seq.find(fun x -> x.ScenarioName = "test_youtube_1")

        let youtube2Steps =
            nodeStats.ScenarioStats
            |> Seq.find(fun x -> x.ScenarioName = "test_youtube_2")

        test <@ youtube1Steps.Duration < duration @>
        test <@ youtube2Steps.Duration = duration @>

[<Fact>]
let ``Test execution should be stopped if all scenarios are stopped`` () =
    let mutable counter1 = 0
    let mutable counter2 = 0
    let duration = seconds 60

    let scenario1 =
        Scenario.create("test_youtube_1", fun ctx -> task {
            do! Task.Delay(milliseconds 100)
            counter1 <- counter1 + 1

            if counter1 = 30 then
                ctx.StopScenario("test_youtube_1", "custom reason")

            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(1, duration)]

    let scenario2 =
        Scenario.create("test_youtube_2", fun ctx -> task {
            do! Task.Delay(milliseconds 100)
            counter2 <- counter2 + 1

            if counter2 = 60 then
                ctx.StopScenario("test_youtube_2", "custom reason")

            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(1, duration)]

    NBomberRunner.registerScenarios [scenario1; scenario2]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let youtube1Steps = nodeStats.ScenarioStats |> Seq.find(fun x -> x.ScenarioName = "test_youtube_1")
        let youtube2Steps = nodeStats.ScenarioStats |> Seq.find(fun x -> x.ScenarioName = "test_youtube_2")
        test <@ youtube1Steps.Duration < duration @>
        test <@ youtube2Steps.Duration < duration @>

[<Fact>]
let ``withClean should provide scenario execution duration via context.ScenarioInfo`` () =
    let duration = seconds 60
    let mutable plannedDuration = TimeSpan.Zero
    let mutable executionDuration = TimeSpan.Zero

    let scenario1 =
        Scenario.create("test_youtube_1", fun ctx -> task {
            do! Task.Delay(seconds 1)

            if ctx.InvocationNumber > 2 then
                ctx.StopCurrentTest("no reason")

            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withInit(fun ctx -> task {
            plannedDuration <- ctx.ScenarioInfo.ScenarioDuration
        })
        |> Scenario.withClean(fun ctx -> task {
            executionDuration <- ctx.ScenarioInfo.ScenarioDuration
        })
        |> Scenario.withLoadSimulations [KeepConstant(1, seconds 60)]

    NBomberRunner.registerScenarios [scenario1]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        let executionSeconds = executionDuration.Seconds
        test <@ plannedDuration = duration @>
        test <@ stats.ScenarioStats[0].Duration.Seconds = executionSeconds @>
        test <@ stats.ScenarioStats[0].Duration < duration @>

[<Fact>]
let ``Test execution should be stopped if too many errors`` () =
    let duration = seconds 60

    let scenario1 =
        Scenario.create("test_youtube_1", fun ctx -> task {
            do! Task.Delay(milliseconds 100)
            return Response.fail()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(100, duration)]

    NBomberRunner.registerScenarios [scenario1]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        test <@ stats.ScenarioStats[0].Fail.Request.Count >= Constants.ScenarioMaxFailCount @>

[<Fact>]
let ``withMaxFailCount should configure MaxFailCount`` () =
    let duration = seconds 60
    let maxFailCount = 1

    let scenario1 =
        Scenario.create("test_youtube_1", fun ctx -> task {
            do! Task.Delay(milliseconds 500)
            return Response.fail()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [Inject(1, seconds 1, duration)]
        |> Scenario.withMaxFailCount maxFailCount

    NBomberRunner.registerScenarios [scenario1]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        test <@ stats.ScenarioStats[0].Fail.Request.Count <> Constants.ScenarioMaxFailCount @>
        test <@ stats.ScenarioStats[0].Fail.Request.Count <= 2 @>

[<Fact>]
let ``MaxFailCount should be tracked only for scenarios failures, not steps failures`` () =
    let duration = seconds 2
    let maxFailCount = 1

    let scenario1 =
        Scenario.create("test_youtube_1", fun ctx -> task {

            let! fail = Step.run("fail_step", ctx, fun () -> task {
                do! Task.Delay(milliseconds 10)
                return Response.fail() // we return fail but this fail should not be count as scenario FailCount
            })

            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withRestartIterationOnFail false
        |> Scenario.withLoadSimulations [KeepConstant(10, duration)]
        |> Scenario.withMaxFailCount maxFailCount

    NBomberRunner.registerScenarios [scenario1]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        test <@ stats.ScenarioStats[0].Fail.Request.Count = 0 @>

[<Fact>]
let ``withInit should stop the test in case of error`` () =

    Scenario.create("test_youtube", fun ctx -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })
    |> Scenario.withInit(fun ctx -> task {
        failwith "my error"
    })
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(2,  seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> function
        | Error error when error.Contains "Init scenario error" -> ()
        | _ -> failwith "error"

[<Fact>]
let ``withClean should not stop test in case of error`` () =

    Scenario.create("test_youtube", fun ctx -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })
    |> Scenario.withClean(fun ctx -> task {
        failwith "my error"
    })
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(2,  seconds 1)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.isOk
