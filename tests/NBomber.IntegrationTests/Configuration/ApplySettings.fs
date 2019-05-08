module Tests.Configuration.ApplySettings

open System

open Xunit
open FsCheck.Xunit

open NBomber.Configuration
open NBomber.Domain
open NBomber.FSharp

[<Property>]
let ``applyScenariosSettings() should override initial settings`` (name: string, warmUpDuration: DateTime, duration: DateTime, concurrentCopies: int) =
    let settings = { ScenarioName = name; WarmUpDuration = warmUpDuration; Duration = duration; ConcurrentCopies = concurrentCopies }
    let scenario = Scenario.create name [] |> NBomber.Domain.Scenario.create

    let updatedScenarios = Scenario.applySettings [|settings|] [|scenario|]

    let result = updatedScenarios.[0].Duration = duration.TimeOfDay
                 && updatedScenarios.[0].ConcurrentCopies = concurrentCopies

    Assert.True(result)

[<Property>]
let ``applyScenariosSettings() should skip applying settings when scenario name is not found`` (name: string, warmUpDuration: DateTime, duration: DateTime, concurrentCopies: int) =
    let settings = { ScenarioName = name; WarmUpDuration = warmUpDuration; Duration = duration; ConcurrentCopies = concurrentCopies }
    let newName = name + "_new_name"
    let scenario = Scenario.create newName [] |> NBomber.Domain.Scenario.create

    let updatedScenarios = Scenario.applySettings [|settings|] [|scenario|]

    let result = updatedScenarios.[0].Duration = TimeSpan.FromSeconds(Constants.DefaultScenarioDurationInSec)
                 && updatedScenarios.[0].ConcurrentCopies = Constants.DefaultConcurrentCopies

    Assert.True(result)

[<Fact>]
let ``applyScenariosSettings() should make no changes if settings absent`` () =
    let scenario = Scenario.create "scenario name" [] |> NBomber.Domain.Scenario.create
    let settings = Array.empty
    let updatedScenarios = Scenario.applySettings settings [|scenario|]

    let result = updatedScenarios.[0].Duration = TimeSpan.FromSeconds(Constants.DefaultScenarioDurationInSec)
                 && updatedScenarios.[0].ConcurrentCopies = Constants.DefaultConcurrentCopies

    Assert.True(result)

[<Fact>]
let ``applyScenariosSettings() with no Scenarios should return empty array`` () =
    let scenarios = Array.empty
    let settings = Array.empty
    Scenario.applySettings settings scenarios
    |> Array.isEmpty
    |> Assert.True
