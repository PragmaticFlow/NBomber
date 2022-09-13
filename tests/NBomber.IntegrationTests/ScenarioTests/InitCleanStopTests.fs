module Tests.Scenario.InitCleanStopTests

open System.Threading.Tasks

open Serilog
open Serilog.Sinks.InMemory
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

    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })

    Scenario.create "withTestClean test" [okStep]
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

    let mutable scnContext: IScenarioContext option = None

    let testInit = fun context -> task {
        scnContext <- Some context
    }

    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })

    let pause = Step.createPause(fun () ->
        scnContext.Value.CustomSettings.Get<TestCustomSettings>().PauseMs
    )

    Scenario.create "test_youtube" [okStep; pause]
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
    let duration = seconds 15

    let okStep = Step.create("ok step", fun context -> task {
        do! Task.Delay(milliseconds 100)
        counter <- counter + 1

        if counter = 30 then
            context.StopScenario("test_youtube_1", "custom reason")

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

    let step1 = Step.create("ok step", fun context -> task {
        do! Task.Delay(milliseconds 100)
        counter1 <- counter1 + 1

        if counter1 = 30 then
            context.StopScenario("test_youtube_1", "custom reason")

        return Response.ok()
    })

    let step2 = Step.create("ok step", fun context -> task {
        do! Task.Delay(milliseconds 100)
        counter2 <- counter2 + 1

        if counter2 = 60 then
            context.StopScenario("test_youtube_2", "custom reason")

        return Response.ok()
    })

    let scenario1 =
        Scenario.create "test_youtube_1" [step1]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(1, duration)]

    let scenario2 =
        Scenario.create "test_youtube_2" [step2]
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
let ``Test execution should be stopped if too many errors`` () =
    let duration = seconds 60

    let step1 = Step.create("ok step", fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.fail()
    })

    let scenario1 =
        Scenario.create "test_youtube_1" [step1]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(100, duration)]

    NBomberRunner.registerScenarios [scenario1]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        test <@ stats.FailCount >= Constants.DefaultMaxFailCount @>

[<Fact>]
let ``withMaxFailCount should configure MaxFailCount`` () =
    let duration = seconds 60
    let maxFailCount = 1

    let step1 = Step.create("ok step", fun context -> task {
        do! Task.Delay(milliseconds 500)
        return Response.fail()
    })

    let scenario1 =
        Scenario.create "test_youtube_1" [step1]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [InjectPerSec(1, duration)]

    NBomberRunner.registerScenarios [scenario1]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.withMaxFailCount maxFailCount
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        test <@ stats.FailCount <> Constants.DefaultMaxFailCount @>
        test <@ stats.FailCount = 2 @>
