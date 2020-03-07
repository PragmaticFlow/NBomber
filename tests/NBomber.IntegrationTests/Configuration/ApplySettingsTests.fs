module Tests.Configuration.ApplySettingsTests

open System

open Xunit
open FsCheck
open FsCheck.Xunit
open Swensen.Unquote

open NBomber.Configuration
open NBomber.Domain
open NBomber.FSharp

[<Property>]
let ``applyScenariosSettings() should override initial settings if the name is matched``
    (name: string, warmUpDuration: DateTime, duration: DateTime, copiesCount: uint32) =

    (copiesCount > 0u && duration.TimeOfDay.TotalSeconds > 1.0) ==> lazy

    let settings = {
        ScenarioName = name
        WarmUpDuration = warmUpDuration
        LoadSimulationsSettings = [LoadSimulationSettings.KeepConcurrentScenarios(int copiesCount, during = duration)]
    }

    let originalScenario = Scenario.create name [] |> NBomber.Domain.Scenario.create
    let updatedScenarios = Scenario.applySettings [|settings|] [|originalScenario|]
    let newDuration = updatedScenarios.[0].LoadTimeLine |> List.head

    test <@ updatedScenarios.[0].Duration = newDuration.EndTime @>
    test <@ updatedScenarios.[0].WarmUpDuration = settings.WarmUpDuration.TimeOfDay @>

[<Property>]
let ``applyScenariosSettings() should skip applying settings when scenario name is not found``
    (name: string, warmUpDuration: DateTime, duration: DateTime, copiesCount: uint32) =

    let settings = {
        ScenarioName = name
        WarmUpDuration = warmUpDuration
        LoadSimulationsSettings = [LoadSimulationSettings.RampConcurrentScenarios(int copiesCount, during = duration)]
    }

    let newName = name + "_new_name"
    let originalScenario = Scenario.create newName [] |> NBomber.Domain.Scenario.create

    let updatedScenario = Scenario.applySettings [|settings|] [|originalScenario|]

    test <@ updatedScenario.[0].Duration = originalScenario.Duration @>
    test <@ updatedScenario.[0].WarmUpDuration = originalScenario.WarmUpDuration @>

[<Fact>]
let ``applyScenariosSettings() with no Scenarios should return empty array`` () =
    let scenarios = Array.empty
    let settings = Array.empty
    Scenario.applySettings settings scenarios
    |> Array.isEmpty
    |> Assert.True
