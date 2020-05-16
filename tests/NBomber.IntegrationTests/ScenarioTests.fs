module Tests.Scenario

open System
open System.Threading.Tasks

open Xunit
open FsCheck
open FsCheck.Xunit
open Swensen.Unquote
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Extensions
open NBomber.Configuration
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Domain
open NBomber.Domain.DomainTypes

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
        |> NBomberRunner.run
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
    |> NBomberRunner.loadConfig "Configuration/test_config.json"
    |> NBomberRunner.run
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
                 |> NBomberRunner.run

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

[<Property>]
let ``applyScenariosSettings() should override initial settings if the name is matched``
    (warmUpDuration: DateTime, duration: DateTime, copiesCount: uint32) =

    (copiesCount > 0u && duration.TimeOfDay.TotalSeconds > 1.0) ==> lazy

    let name = "same_name"

    let settings = {
        ScenarioName = name
        WarmUpDuration = warmUpDuration
        LoadSimulationsSettings = [LoadSimulationSettings.KeepConcurrentScenarios(int copiesCount, during = duration)]
        CustomSettings = Some "some data"
    }

    let originalScenarios =
        [Scenario.create name [Step.createPause(duration.TimeOfDay)]]
        |> NBomber.Domain.Scenario.createScenarios
        |> Result.getOk

    let updatedScenarios = Scenario.applySettings [settings] originalScenarios
    let newDuration = updatedScenarios.[0].LoadTimeLine |> List.head

    test <@ updatedScenarios.[0].PlanedDuration = newDuration.EndTime @>
    test <@ updatedScenarios.[0].WarmUpDuration = settings.WarmUpDuration.TimeOfDay @>
    test <@ updatedScenarios.[0].CustomSettings = settings.CustomSettings.Value @>

[<Property>]
let ``applyScenariosSettings() should skip applying settings when scenario name is not match``
    (warmUpDuration: DateTime, duration: DateTime, copiesCount: uint32) =

    (copiesCount > 0u && duration.TimeOfDay.TotalSeconds > 1.0) ==> lazy

    let name = "same_name"

    let settings = {
        ScenarioName = name
        WarmUpDuration = warmUpDuration
        LoadSimulationsSettings = [LoadSimulationSettings.RampConcurrentScenarios(int copiesCount, during = duration)]
        CustomSettings = None
    }

    let newName = name + "_new_name"
    let originalScenarios =
        [Scenario.create newName [Step.createPause(duration.TimeOfDay)]
         |> Scenario.withWarmUpDuration(warmUpDuration.AddMinutes(2.0).TimeOfDay)
         |> Scenario.withLoadSimulations[
             NBomber.Contracts.LoadSimulation.KeepConcurrentScenarios(int copiesCount, duration.AddMinutes(2.0).TimeOfDay)
         ]]
        |> NBomber.Domain.Scenario.createScenarios
        |> Result.getOk

    let updatedScenario = Scenario.applySettings [settings] originalScenarios

    test <@ settings.WarmUpDuration.TimeOfDay <> originalScenarios.Head.WarmUpDuration @>
    test <@ updatedScenario.[0].WarmUpDuration = originalScenarios.Head.WarmUpDuration @>
    test <@ updatedScenario.[0].PlanedDuration = originalScenarios.Head.PlanedDuration @>

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
    match Scenario.Validation.checkDuplicateName([scn1; scn2]) with
    | Error _ -> ()
    | _       -> failwith ""

[<Fact>]
let ``checkEmptyStepName should return fail if scenario has empty step name`` () =
    let step = NBomber.FSharp.Step.create(" ", fun _ -> Task.FromResult(Response.Ok()))
    let scn = Scenario.create "1" [step]
    match Scenario.Validation.checkEmptyStepName(scn) with
    | Error _ -> ()
    | _       -> failwith ""

[<Fact>]
let ``checkStepsNotEmpty should return fail if scenario has no steps`` () =
    let scn = Scenario.create "1" []
    match Scenario.Validation.checkStepsNotEmpty(scn) with
    | Error _ -> ()
    | _       -> failwith ""
