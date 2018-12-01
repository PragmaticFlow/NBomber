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

    let updatedScenarios = applyScenariosSettings [|settings|] [|Scenario.create(name, [])|]

    match updatedScenarios with
    | [|updatedScenario|] -> updatedScenario.Duration = duration &&
                             updatedScenario.ConcurrentCopies = concurrentCopies
    | _ -> false

[<Fact>]
let ``Skip applying settings when scenario name is not found`` () =
    let settings = { ScenarioName = "different scenario name"; Duration = TimeSpan.MinValue; ConcurrentCopies = 0 }

    let updatedScenarios = applyScenariosSettings [|settings|] [|Scenario.create("scenario name", [])|]

    match updatedScenarios with
    | [|updatedScenario|] -> updatedScenario.Duration = TimeSpan.FromSeconds(DefaultDurationInSeconds) &&
                             updatedScenario.ConcurrentCopies = DefaultConcurrentCopies
    | _ -> false

[<Fact>]
let ``Running applyScenariosSettings() with no Settings should make no changes`` () =
    let updatedScenarios = applyScenariosSettings [||] [|Scenario.create("scenario name", [])|]

    match updatedScenarios with
    | [|updatedScenario|] -> updatedScenario.Duration = TimeSpan.FromSeconds(DefaultDurationInSeconds) &&
                             updatedScenario.ConcurrentCopies = DefaultConcurrentCopies
    | _ -> false

[<Fact>]
let ``applyScenariosSettings() with no Scenarios should return empty array`` () =
    applyScenariosSettings [||] [||] |> Array.isEmpty
    