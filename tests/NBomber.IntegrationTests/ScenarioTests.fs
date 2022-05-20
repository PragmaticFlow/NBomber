module Tests.Scenario

open System
open System.Threading.Tasks

open Xunit
open FsCheck.Xunit
open Swensen.Unquote
open Microsoft.Extensions.Configuration

open NBomber
open NBomber.Extensions.Internal
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.FSharp
open NBomber.Domain
open NBomber.Domain.DomainTypes

[<CLIMutable>]
type TestCustomSettings = {
    TargetHost: string
    MsgSizeInBytes: int
    PauseMs: int
}

//todo: add test on TestInit and TestClean should be executed only for scenario.Enabled
//todo: test on context.StopScenario("name") and check what realtime stats it provides

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
    |> NBomberRunner.withReportFolder "./scenarios-tests/1/"
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
    |> NBomberRunner.withReportFolder "./scenarios-tests/2/"
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
    |> NBomberRunner.withReportFolder "./scenarios-tests/3/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let youtube1Steps = nodeStats.ScenarioStats |> Seq.find(fun x -> x.ScenarioName = "test_youtube_1")
        let youtube2Steps = nodeStats.ScenarioStats |> Seq.find(fun x -> x.ScenarioName = "test_youtube_2")
        test <@ youtube1Steps.Duration < duration @>
        test <@ youtube2Steps.Duration < duration @>

[<Fact>]
let ``Warmup should have no effect on stats`` () =

    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })

    let failStep = Step.create("fail step", fun _ -> task {
        do! Task.Delay(milliseconds 100)
        return Response.fail()
    })

    Scenario.create "warmup test" [okStep; failStep]
    |> Scenario.withWarmUpDuration(seconds 3)
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 1)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./scenarios-tests/4/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let allStepStats = nodeStats.ScenarioStats |> Seq.collect(fun x -> x.StepStats)
        let okSt = allStepStats |> Seq.find(fun x -> x.StepName = "ok step")
        let failSt = allStepStats |> Seq.find(fun x -> x.StepName = "fail step")

        test <@ okSt.Ok.Request.Count <= 10 @>
        test <@ okSt.Fail.Request.Count = 0 @>
        test <@ failSt.Ok.Request.Count = 0 @>
        test <@ failSt.Fail.Request.Count <= 10 @>

[<Fact>]
let ``applyScenariosSettings() should override initial settings if the scenario name is matched`` () =

    let scnName1 = "scenario_1"
    let warmUp1 = seconds 30
    let duration1 = seconds 50

    let scnName2 = "scenario_1"
    let duration2 = seconds 5

    let settings = {
        ScenarioName = scnName1
        WarmUpDuration = Some(warmUp1.ToString("hh\:mm\:ss"))
        LoadSimulationsSettings = Some [LoadSimulationSettings.KeepConstant(10, duration1.ToString("hh\:mm\:ss"))]
        ClientFactorySettings = None
        CustomStepOrder = None
        CustomSettings = Some "some data"
    }

    let originalScenarios =
        [Scenario.create scnName2 [Step.createPause 1]
        |> Scenario.withLoadSimulations [RampConstant(500, duration2)] ]
        |> Scenario.createScenarios
        |> Result.getOk

    let updatedScenarios = Scenario.applySettings [settings] originalScenarios

    test <@ updatedScenarios[0].PlanedDuration = duration1 @>
    test <@ updatedScenarios[0].WarmUpDuration = warmUp1 @>
    test <@ updatedScenarios[0].CustomSettings = settings.CustomSettings.Value @>

[<Property>]
let ``applyScenariosSettings() should skip applying settings when scenario name is not match`` () =

    let scnName1 = "scenario_1"
    let warmUp1 = seconds 30
    let duration1 = seconds 50

    let scnName2 = "scenario_2"
    let duration2 = seconds 5

    let settings = {
        ScenarioName = scnName1
        WarmUpDuration = Some(warmUp1.ToString("hh\:mm\:ss"))
        LoadSimulationsSettings = Some [LoadSimulationSettings.RampConstant(5, duration1.ToString("hh\:mm\:ss"))]
        ClientFactorySettings = None
        CustomSettings = None
        CustomStepOrder = None
    }

    let scenario =
        Scenario.create scnName2 [Step.createPause 120]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [LoadSimulation.KeepConstant(500, duration2)]
        |> List.singleton
        |> Scenario.createScenarios
        |> Result.getOk

    let updatedScenario = Scenario.applySettings [settings] scenario

    test <@ updatedScenario.Head.WarmUpDuration = TimeSpan.Parse "00:00:00" @>
    test <@ updatedScenario.Head.PlanedDuration = duration2 @>

[<Fact>]
let ``applyScenariosSettings() with no Scenarios should return empty array`` () =
    let scenarios = List.empty
    let settings = List.empty
    Scenario.applySettings settings scenarios
    |> List.isEmpty
    |> Assert.True

[<Fact>]
let ``checkEmptyName should return fail if scenario has empty name`` () =
    let scn = Scenario.create " " []
    match Scenario.Validation.checkEmptyScenarioName scn with
    | Error _ -> ()
    | _       -> failwith ""

[<Fact>]
let ``checkDuplicateName should return fail if scenario has duplicate name`` () =
    let scn1 = Scenario.create "1" []
    let scn2 = Scenario.create "1" []
    match Scenario.Validation.checkDuplicateScenarioName([scn1; scn2]) with
    | Error _ -> ()
    | _       -> failwith ""

[<Fact>]
let ``scenario should fail when it has duplicate step names that has different implementations`` () =
    let step1 = Step.create("step 1", fun _ -> task { return Response.ok() })
    let step2 = Step.create("step 1", fun _ -> task { return Response.ok() })
    Scenario.create "1" [step1; step2]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(1, seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.run
    |> Result.getError
    |> ignore

[<Fact>]
let ``checkEmptyStepName should return fail if scenario has empty step name`` () =
    let step = NBomber.FSharp.Step.create(" ", fun _ -> task { return Response.ok() })
    let scn = Scenario.create "1" [step]
    match Scenario.Validation.checkEmptyStepName(scn) with
    | Error _ -> ()
    | _       -> failwith ""

[<Fact>]
let ``check that scenario should fail if it has no steps and no init and no clean`` () =
    Scenario.create "1" []
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(1, seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.run
    |> Result.getError
    |> ignore

[<Fact>]
let ``check that scenario should be ok if it has no steps but clean function exist`` () =
    Scenario.create "1" []
    |> Scenario.withClean(fun _ -> task { return () })
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(1, seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.run
    |> Result.getOk
    |> ignore

[<Fact>]
let ``check that scenario should be ok if it has no steps but init function exist`` () =
    Scenario.create "1" []
    |> Scenario.withInit(fun _ -> task { return () })
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(1, seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.run
    |> Result.getOk
    |> ignore

[<Fact>]
let ``ScenarioSettings should be validated on duplicates `` () =

    let step1 = Step.create("step_1", fun context -> task {
        do! Task.Delay(milliseconds 10)
        return Response.ok()
    })

    Scenario.create "1" [step1]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(1, seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.loadConfig "Configuration/duplicate_scenarios_config.json" // duplicated scenario 1
    |> NBomberRunner.run
    |> Result.getError
    |> fun error -> test <@ error.Contains("Scenario names are not unique in JSON config") @>

[<Fact>]
let ``withStepTimeout should set step timeout`` () =

    let step1 = Step.create("step_1", timeout = seconds 2, execute = fun context -> task {
        do! Task.Delay(seconds 4)
        return Response.ok()
    })

    Scenario.create "1" [step1]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(1, seconds 10)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.run
    |> Result.getOk
    |> NodeStats.getScenarioStats "1"
    |> ScenarioStats.getStepStats "step_1"
    |> fun stepsStats ->
        test <@ stepsStats.Ok.Request.Count = 0 @>
        test <@ stepsStats.Fail.Request.Count > 0 @>
        test <@ stepsStats.Fail.StatusCodes[0].StatusCode = NBomber.Constants.TimeoutStatusCode @>
        test <@ stepsStats.Fail.StatusCodes[0].Count = stepsStats.Fail.Request.Count @>
