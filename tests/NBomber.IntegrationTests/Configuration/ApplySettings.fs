module Tests.Configuration.ApplySettings

open System

open Xunit
open FsCheck.Xunit

open NBomber.FSharp
open NBomber.Configuration
open NBomber.Domain.DomainTypes
open NBomber.DomainServices.NBomberRunner

[<Property>]
let ``applyScenariosSettings() should override initial settings`` (name: string, duration: TimeSpan, concurrentCopies: int) =
    let settings = { ScenarioName = name; Duration = duration; ConcurrentCopies = concurrentCopies }
    let scenario = Scenario.create(name, [])

    let updatedScenarios = applyScenariosSettings [|settings|] [|scenario|]
    
    let result = updatedScenarios.[0].Duration = duration
                 && updatedScenarios.[0].ConcurrentCopies = concurrentCopies
    
    Assert.True(result)

[<Property>]
let ``applyScenariosSettings() should skip applying settings when scenario name is not found`` (name: string, duration: TimeSpan, concurrentCopies: int) =
    let settings = { ScenarioName = name; Duration = duration; ConcurrentCopies = concurrentCopies }
    let newName = name + "_new_name"
    let scenario = Scenario.create(newName, [])

    let updatedScenarios = applyScenariosSettings [|settings|] [|scenario|]

    let result = updatedScenarios.[0].Duration = TimeSpan.FromSeconds(Constants.DefaultScenarioDurationInSec)
                 && updatedScenarios.[0].ConcurrentCopies = Constants.DefaultConcurrentCopies
    
    Assert.True(result)

[<Fact>]
let ``applyScenariosSettings() should make no changes if settings absent`` () =
    let scenario = Scenario.create("scenario name", [])
    let settings = Array.empty
    let updatedScenarios = applyScenariosSettings settings [|scenario|]

    let result = updatedScenarios.[0].Duration = TimeSpan.FromSeconds(Constants.DefaultScenarioDurationInSec)
                 && updatedScenarios.[0].ConcurrentCopies = Constants.DefaultConcurrentCopies
    
    Assert.True(result)

[<Fact>]
let ``applyScenariosSettings() with no Scenarios should return empty array`` () =
    let scenarios = Array.empty
    let settings = Array.empty
    applyScenariosSettings settings scenarios
    |> Array.isEmpty
    |> Assert.True
    