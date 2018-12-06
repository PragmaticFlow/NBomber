module Tests.Configuration.ValidateSettings

open System
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
let ``validateRunnerContext() should return Ok`` (scenarioName: string, concurrentCopies: int) =
    let settings = buildSettings(scenarioName, TimeSpan.FromSeconds(1.0), 1)

    buildConfig(scenarioName, [|settings|], [|scenarioName|])
    |> Validation.validateRunnerContext
    |> Result.isOk

[<Property>]
let ``validateRunnerContext() should fail when target scenrio name is not declared`` (scenarioName: string) =    
    let targetScenarios = [|scenarioName + "new_name"|]
    let errorMessage = buildConfig(scenarioName, [||], targetScenarios)
                        |> Validation.validateRunnerContext
                        |> Result.getError
    
    errorMessage.StartsWith (sprintf "Target scenarios %A is not found." targetScenarios)

[<Property>]
let ``validateRunnerContext() should fail when duration is less than 1 sec`` (scenarioName: string) =
    let settings = buildSettings(scenarioName, TimeSpan.FromSeconds(0.0), 1)

    let errorMessage = buildConfig(scenarioName, [|settings|], [|scenarioName|])
                        |> Validation.validateRunnerContext
                        |> Result.getError
    
    errorMessage = (sprintf "Duration for scenarios %A can not be less than 1 sec" [|scenarioName|])

[<Property>]
let ``validateRunnerContext() should fail when concurrent copies are less than 1`` (scenarioName: string) =
    let settings = buildSettings(scenarioName, TimeSpan.FromSeconds(1.0), 0)

    let errorMessage = buildConfig(scenarioName, [|settings|], [|scenarioName|])
                        |> Validation.validateRunnerContext
                        |> Result.getError
    
    errorMessage = (sprintf "Concurrent copies for scenarios %A can not be less than 1" [|scenarioName|])
    
    