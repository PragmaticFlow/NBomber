module Tests.Configuration.ApplySettings

open System

open Xunit
open FsCheck.Xunit

open NBomber.FSharp
open NBomber.DomainServices.NBomberRunner
open NBomber.Configuration
open NBomber.Domain.DomainTypes.Constants

[<Property>]
let ``Basic applying of scenario settings`` (name: string, duration: TimeSpan, concurrentCopies: int) =
    let settings = { ScenarioName = name; Duration = duration; ConcurrentCopies = concurrentCopies }
    let scenario = Scenario.create(name, [])

    let updatedScenarios = applyScenariosSettings [|settings|] [|scenario|]

    match updatedScenarios with
    | [|updatedScenario|] -> updatedScenario.Duration = duration &&
                             updatedScenario.ConcurrentCopies = concurrentCopies;    
    | _ -> false

[<Fact>]
let ``Skip applying settings when scenario name is not found`` () =
    let settings = { ScenarioName = "different scenario name"; Duration = TimeSpan.MinValue; ConcurrentCopies = 0 }
    let scenario = Scenario.create("scenario name", [])

    let updatedScenarios = applyScenariosSettings [|settings|] [|scenario|]

    match updatedScenarios with
    | [|updatedScenario|] -> updatedScenario.Duration = TimeSpan.FromSeconds(DefaultDurationInSeconds) &&
                             updatedScenario.ConcurrentCopies = DefaultConcurrentCopies
    | _ -> false
    |> Assert.True

[<Fact>]
let ``Running applyScenariosSettings() with no Settings should make no changes`` () =
    let scenario = Scenario.create("scenario name", [])
    let updatedScenarios = applyScenariosSettings Array.empty [|scenario|]

    match updatedScenarios with
    | [|updatedScenario|] -> updatedScenario.Duration = TimeSpan.FromSeconds(DefaultDurationInSeconds) &&
                             updatedScenario.ConcurrentCopies = DefaultConcurrentCopies
    | _ -> false
    |> Assert.True

[<Fact>]
let ``applyScenariosSettings() with no Scenarios should return empty array`` () =
    applyScenariosSettings Array.empty Array.empty |> Array.isEmpty |> Assert.True
    