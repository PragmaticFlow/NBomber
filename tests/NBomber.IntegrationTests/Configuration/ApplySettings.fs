﻿module Tests.Configuration.ApplySettings

open System

open Xunit
open FsCheck.Xunit

open NBomber.FSharp
open NBomber.Configuration
open NBomber.Domain.DomainTypes
open NBomber.DomainServices

[<Property>]
let ``applyScenariosSettings() should override initial settings`` (name: string, warmUpDuration: TimeSpan, duration: TimeSpan, concurrentCopies: int) =
    let settings = { ScenarioName = name; WarmUpDuration = warmUpDuration; Duration = duration; ConcurrentCopies = concurrentCopies }
    let scenario = Scenario.create(name, []) |> NBomber.Domain.Scenario.create

    let updatedScenarios = ScenarioBuilder.applyScenariosSettings [|settings|] [|scenario|]
    
    let result = updatedScenarios.[0].Duration = duration
                 && updatedScenarios.[0].ConcurrentCopies = concurrentCopies
    
    Assert.True(result)

[<Property>]
let ``applyScenariosSettings() should skip applying settings when scenario name is not found`` (name: string, warmUpDuration: TimeSpan, duration: TimeSpan, concurrentCopies: int) =
    let settings = { ScenarioName = name; WarmUpDuration = warmUpDuration; Duration = duration; ConcurrentCopies = concurrentCopies }
    let newName = name + "_new_name"
    let scenario = Scenario.create(newName, []) |> NBomber.Domain.Scenario.create

    let updatedScenarios = ScenarioBuilder.applyScenariosSettings [|settings|] [|scenario|]

    let result = updatedScenarios.[0].Duration = TimeSpan.FromSeconds(Constants.DefaultScenarioDurationInSec)
                 && updatedScenarios.[0].ConcurrentCopies = Constants.DefaultConcurrentCopies
    
    Assert.True(result)

[<Fact>]
let ``applyScenariosSettings() should make no changes if settings absent`` () =
    let scenario = Scenario.create("scenario name", []) |> NBomber.Domain.Scenario.create
    let settings = Array.empty
    let updatedScenarios = ScenarioBuilder.applyScenariosSettings settings [|scenario|]

    let result = updatedScenarios.[0].Duration = TimeSpan.FromSeconds(Constants.DefaultScenarioDurationInSec)
                 && updatedScenarios.[0].ConcurrentCopies = Constants.DefaultConcurrentCopies
    
    Assert.True(result)

[<Fact>]
let ``applyScenariosSettings() with no Scenarios should return empty array`` () =
    let scenarios = Array.empty
    let settings = Array.empty
    ScenarioBuilder.applyScenariosSettings settings scenarios
    |> Array.isEmpty
    |> Assert.True