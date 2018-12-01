module Tests.Configuration.ApplySettings

open System

open FsCheck.Xunit

open NBomber.FSharp
open NBomber.DomainServices.NBomberRunner
open NBomber.Configuration
open NBomber.Domain.DomainTypes.Constants

[<Property>]
let ``Basic applying of scenario settings`` (name: string, duration: TimeSpan, concurrentCopies: int) =
    let scenario = Scenario.create(name, [])    
    let settings = { ScenarioName = name; Duration = duration; ConcurrentCopies = concurrentCopies }

    let appliedScenarios =  [|scenario|] |> applyScenariosSettings [|settings|]

    match appliedScenarios with
    | [|appliedScenario|] -> appliedScenario.Duration = duration && appliedScenario.ConcurrentCopies = concurrentCopies
    | _ -> false

[<Property>]
let ``Skip applying settings when scenario name is not found`` () =
    let scenario = Scenario.create("scenario name 1", [])
    let settings = { ScenarioName = "scenario name 2"; Duration = TimeSpan.MinValue; ConcurrentCopies = 0 }

    let appliedScenarios =  [|scenario|] |> applyScenariosSettings [|settings|]

    match appliedScenarios with
    | [|appliedScenario|] ->
                            appliedScenario.Duration = TimeSpan.FromSeconds(DefaultDurationInSeconds) &&
                            appliedScenario.ConcurrentCopies = DefaultConcurrentCopies
    | _ -> false

    