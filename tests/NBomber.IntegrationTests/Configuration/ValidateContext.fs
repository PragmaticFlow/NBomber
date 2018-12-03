module Tests.Configuration.ValidateSettings

open System

open Xunit
open FsCheck.Xunit

open NBomber.FSharp
open NBomber.Configuration
open NBomber.DomainServices
open NBomber.Contracts

[<Property>]
let ``Basic validation of scenario Config`` (name: string, duration: TimeSpan, concurrentCopies: int) =
    let scenario = Scenario.create(name, [])
    let settings = { ScenarioName = name; Duration = duration; ConcurrentCopies = concurrentCopies }
    let globalSettings = { ScenariosSettings = [|settings|]; TargetScenarios = [|name|] }
    let config = { NBomberConfig.GlobalSettings = Some globalSettings }
    let context = { Scenarios = [|scenario|]; NBomberConfig = Some config }

    match Validation.validateRunnerContext(context) with
    | Ok context -> true
    | _ -> false

[<Fact>]
let ``Target scenrio name is not declared`` () =
    let scenario = Scenario.create("scenario name", [])
    let globalSettings = { ScenariosSettings = [||]; TargetScenarios = [|"different scenario name"|] }
    let config = { NBomberConfig.GlobalSettings = Some globalSettings }
    let context = { Scenarios = [|scenario|]; NBomberConfig = Some config }

    match Validation.validateRunnerContext(context) with
    | Error "Target scenario is not declared" -> true
    | _ -> false
    |> Assert.True
    