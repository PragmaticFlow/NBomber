module Tests.Scenario

open System
open System.Threading.Tasks

open Xunit
open Swensen.Unquote
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Extensions
open NBomber.Contracts
open NBomber.FSharp

[<CLIMutable>]
type TestCustomSettings = {
    TargetHost: string
    MsgSizeInBytes: int
}

[<Fact>]
let ``TestClean should be invoked only once and not fail runner`` () =

    let mutable cleanInvokeCounter = 0

    let testClean = fun _ -> task {
        cleanInvokeCounter <- cleanInvokeCounter + 1
        failwith "exception was not handled"
    }

    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    let scenario =
        Scenario.create "withTestClean test" [okStep]
        |> Scenario.withTestClean testClean
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(2,  TimeSpan.FromSeconds(1.0))
        ]

    let allStats =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.runTest
        |> Result.getOk

    test <@ 1 = cleanInvokeCounter @>

[<Fact>]
let ``TestInit should propagate CustomSettings from config.json`` () =

    let mutable scnContext = Option.None

    let testInit = fun context -> task {
        scnContext <- Some context
    }

    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    let scenario =
        Scenario.create "test_youtube" [okStep]
        |> Scenario.withTestInit testInit
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(2,  TimeSpan.FromSeconds(2.0))
        ]


    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.loadConfigJson "Configuration/test_config.json"
    |> NBomberRunner.runTest
    |> ignore

    let cusomSettings = scnContext.Value.CustomSettings.DeserializeJson<TestCustomSettings>()

    test <@ cusomSettings.TargetHost = "localhost" @>
    test <@ cusomSettings.MsgSizeInBytes = 1000 @>

[<Fact>]
let ``should be stopped via StepContext.StopScenario`` () =

    let mutable counter = 0
    let duration = TimeSpan.FromSeconds(15.0)

    let okStep = Step.create("ok step", fun context -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        counter <- counter + 1

        if counter = 30 then
            context.StopScenario("test_youtube_1", "custom reason")

        return Response.Ok()
    })

    let scenario1 =
        Scenario.create "test_youtube_1" [okStep]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(10, duration)
        ]

    let scenario2 =
        Scenario.create "test_youtube_2" [okStep]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(10, duration)
        ]

    NBomberRunner.registerScenarios [scenario1; scenario2]
    |> NBomberRunner.runTest
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
let ``Test execution should be stoped if all scenarios are stoped`` () =
    let mutable counter = 0
    let duration = TimeSpan.FromSeconds(15.0)

    let okStep = Step.create("ok step", fun context -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        counter <- counter + 1

        if counter = 30 then
            context.StopScenario("test_youtube_1", "custom reason")

        if counter = 60 then
            context.StopScenario("test_youtube_2", "custom reason")

        return Response.Ok()
    })

    let scenario1 =
        Scenario.create "test_youtube_1" [okStep]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(10, duration)
        ]

    let scenario2 =
        Scenario.create "test_youtube_2" [okStep]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(10, duration)
        ]

    NBomberRunner.registerScenarios [scenario1; scenario2]
    |> NBomberRunner.runTest
    |> Result.getOk
    |> fun nodeStats ->
        let youtube1Steps = nodeStats.ScenarioStats |> Seq.find(fun x -> x.ScenarioName = "test_youtube_1")
        let youtube2Steps = nodeStats.ScenarioStats |> Seq.find(fun x -> x.ScenarioName = "test_youtube_2")
        test <@ youtube1Steps.Duration < duration @>
        test <@ youtube2Steps.Duration < duration @>

[<Fact>]
let ``Warmup should have no effect on stats`` () =

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
        |> Scenario.withWarmUpDuration(TimeSpan.FromSeconds 3.0)
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(copiesCount = 1, during = TimeSpan.FromSeconds 1.0)
        ]

    let result = NBomberRunner.registerScenarios [scenario]
                 |> NBomberRunner.runTest

    match result with
    | Ok nodeStats ->
        let allStepStats = nodeStats.ScenarioStats |> Seq.collect(fun x -> x.StepStats)
        let okStats = allStepStats |> Seq.find(fun x -> x.StepName = "ok step")
        let failStats = allStepStats |> Seq.find(fun x -> x.StepName = "fail step")

        test <@ okStats.OkCount <= 10 @>
        test <@ okStats.FailCount = 0 @>
        test <@ failStats.OkCount = 0 @>
        test <@ failStats.FailCount <= 10 @>

    | Error msg -> failwith msg
