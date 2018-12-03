module Tests.Configuration.ValidateSettings

open System

open Xunit
open FsCheck.Xunit

open NBomber.FSharp
open NBomber.Configuration
open NBomber.DomainServices
open NBomber.Contracts

let buildConfig (scenarioName: string, settings: ScenarioSetting[], targetScenarios: string[]) =
    let scenario = Scenario.create(scenarioName, [])
    let globalSettings = { ScenariosSettings = settings; TargetScenarios = targetScenarios }
    let config = { NBomberConfig.GlobalSettings = Some globalSettings }
    
    { Scenarios = [|scenario|]; NBomberConfig = Some config }

let buildSettings (scenarioName: string, duration: TimeSpan, concurrentCopies: int) =
    { ScenarioName = scenarioName; Duration = duration; ConcurrentCopies = concurrentCopies }

[<Property>]
let ``Basic validation of scenario Config`` (scenarioName: string, duration: TimeSpan, concurrentCopies: int) =
    let settings = buildSettings(scenarioName, duration, concurrentCopies)

    buildConfig(scenarioName, [|settings|], [|scenarioName|])
    |> Validation.validateRunnerContext
    |> function 
    | Ok context -> true
    | _ -> false

[<Fact>]
let ``Target scenrio name is not declared`` () =    
    buildConfig("scenario name", [||], [|"different scenario name"|])
    |> Validation.validateRunnerContext
    |> function 
    | Error "Target scenario is not declared" -> true
    | _ -> false
    |> Assert.True
    