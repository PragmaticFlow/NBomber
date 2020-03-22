module Tests.Configuration.ApplySettingsTests

open System

open Xunit
open FsCheck
open FsCheck.Xunit
open Swensen.Unquote

open NBomber.Extensions
open NBomber.Configuration
open NBomber.Domain
open NBomber.FSharp

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

    let updatedScenarios = Scenario.applySettings [|settings|] originalScenarios
    let newDuration = updatedScenarios.[0].LoadTimeLine |> List.head

    test <@ updatedScenarios.[0].Duration = newDuration.EndTime @>
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

    let updatedScenario = Scenario.applySettings [|settings|] originalScenarios

    test <@ settings.WarmUpDuration.TimeOfDay <> originalScenarios.Head.WarmUpDuration @>
    test <@ updatedScenario.[0].WarmUpDuration = originalScenarios.Head.WarmUpDuration @>
    test <@ updatedScenario.[0].Duration = originalScenarios.Head.Duration @>

[<Fact>]
let ``applyScenariosSettings() with no Scenarios should return empty array`` () =
    let scenarios = List.empty
    let settings = Array.empty
    Scenario.applySettings settings scenarios
    |> List.isEmpty
    |> Assert.True
