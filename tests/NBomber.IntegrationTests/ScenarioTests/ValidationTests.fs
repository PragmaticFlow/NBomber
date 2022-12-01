module Tests.Scenario.ValidationTests

open Xunit
open Swensen.Unquote
open NBomber
open NBomber.FSharp
open NBomber.Extensions.Internal
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.DomainTypes

[<Fact>]
let ``applyScenariosSettings() should override initial settings if the scenario name is matched`` () =

    let scnName1 = "scenario_1"
    let warmUp1 = seconds 30
    let duration1 = seconds 50

    let scnName2 = "scenario_1"
    let duration2 = seconds 5

    let settings = {
        ScenarioName = scnName1
        WarmUpDuration = Some warmUp1
        LoadSimulationsSettings = Some [LoadSimulation.KeepConstant(10, duration1)]
        CustomSettings = Some "some data"
        MaxFailCount = Some Constants.ScenarioMaxFailCount
    }

    let originalScenarios =
        [Scenario.create(scnName2, fun ctx -> task { return Response.ok() })
        |> Scenario.withLoadSimulations [RampingConstant(500, duration2)] ]
        |> Scenario.createScenarios
        |> Result.getOk

    let updatedScenarios = Scenario.applySettings [settings] originalScenarios

    test <@ updatedScenarios[0].PlanedDuration = duration1 @>
    test <@ updatedScenarios[0].WarmUpDuration.Value = warmUp1 @>
    test <@ updatedScenarios[0].CustomSettings = settings.CustomSettings.Value @>

[<Fact>]
let ``applyScenariosSettings() should skip applying settings when scenario name is not match`` () =

    let scnName1 = "scenario_1"
    let warmUp1 = seconds 30
    let duration1 = seconds 50

    let scnName2 = "scenario_2"
    let duration2 = seconds 5

    let settings = {
        ScenarioName = scnName1
        WarmUpDuration = Some warmUp1
        LoadSimulationsSettings = Some [LoadSimulation.RampingConstant(5, duration1)]
        CustomSettings = None
        MaxFailCount = Some Constants.ScenarioMaxFailCount
    }

    let scenario =
        Scenario.create(scnName2, fun ctx -> task { return Response.ok() })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [LoadSimulation.KeepConstant(500, duration2)]
        |> List.singleton
        |> Scenario.createScenarios
        |> Result.getOk

    let updatedScenario = Scenario.applySettings [settings] scenario

    test <@ updatedScenario.Head.WarmUpDuration = None @>
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
    let scn = Scenario.create(" ", fun ctx -> task { return Response.ok() })
    match Scenario.Validation.checkEmptyScenarioName scn with
    | Error _ -> ()
    | _       -> failwith ""

[<Fact>]
let ``checkDuplicateName should return fail if scenario has duplicate name`` () =
    let scn1 = Scenario.create("1", fun ctx -> task { return Response.ok() })
    let scn2 = Scenario.create("1", fun ctx -> task { return Response.ok() })
    match Scenario.Validation.checkDuplicateScenarioName([scn1; scn2]) with
    | Error _ -> ()
    | _       -> failwith ""

[<Fact>]
let ``ScenarioSettings should be validated on duplicates `` () =

    Scenario.create("1", fun ctx -> task { return Response.ok() })
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(1, seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.loadConfig "Configuration/duplicate_scenarios_config.json" // duplicated scenario 1
    |> NBomberRunner.run
    |> Result.getError
    |> fun error -> test <@ error.Contains("Scenario names are not unique in JSON config") @>

// [<Fact>]
// let ``withStepTimeout should set step timeout`` () =
//
//     let step1 = Step.create("step_1", timeout = seconds 2, execute = fun context -> task {
//         do! Task.Delay(seconds 4)
//         return Response.ok()
//     })
//
//     Scenario.create "1" [step1]
//     |> Scenario.withoutWarmUp
//     |> Scenario.withLoadSimulations [KeepConstant(1, seconds 10)]
//     |> NBomberRunner.registerScenario
//     |> NBomberRunner.withoutReports
//     |> NBomberRunner.run
//     |> Result.getOk
//     |> NodeStats.getScenarioStats "1"
//     |> ScenarioStats.getStepStats "step_1"
//     |> fun stepsStats ->
//         test <@ stepsStats.Ok.Request.Count = 0 @>
//         test <@ stepsStats.Fail.Request.Count > 0 @>
//         test <@ stepsStats.Fail.StatusCodes[0].StatusCode = NBomber.Constants.TimeoutStatusCode @>
//         test <@ stepsStats.Fail.StatusCodes[0].Count = stepsStats.Fail.Request.Count @>
