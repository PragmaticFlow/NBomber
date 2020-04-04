module Tests.ScenarioTests

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
let ``Scenario should be stopped via stepContext.StopScenario`` () =

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
    |> fun allstats ->
        let youtube1Steps = allstats |> Seq.find(fun x -> x.ScenarioName = "test_youtube_1" && x.StepName = "ok step")
        let youtube2Steps = allstats |> Seq.find(fun x -> x.ScenarioName = "test_youtube_2" && x.StepName = "ok step")
        test <@ youtube1Steps.Duration < duration @>
        test <@ youtube2Steps.Duration = duration @>

[<Fact>]
let ``Test should be stoped if all scenarios are stoped`` () =
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
    |> fun allstats ->
        let youtube1Steps = allstats |> Seq.find(fun x -> x.ScenarioName = "test_youtube_1" && x.StepName = "ok step")
        let youtube2Steps = allstats |> Seq.find(fun x -> x.ScenarioName = "test_youtube_2" && x.StepName = "ok step")
        test <@ youtube1Steps.Duration < duration @>
        test <@ youtube2Steps.Duration < duration @>

[<Fact>]
let ``StepContext.StopTest should stop all scenarios`` () =

    let mutable counter = 0
    let duration = TimeSpan.FromSeconds(42.0)

    let okStep = Step.create("ok step", fun context -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        counter <- counter + 1

        if counter >= 30 then
            context.StopTest("custom reason")

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
    |> fun allstats ->
        allstats
        |> Seq.filter(fun x -> x.StepName = "ok step")
        |> Seq.iter(fun x -> test <@ x.Duration < duration @>)
