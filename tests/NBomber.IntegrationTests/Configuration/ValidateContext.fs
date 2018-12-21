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
    
    { Scenarios = [|scenario|]; NBomberConfig = Some config; OutputFilename = None; OutputFileTypes = [||] }

let buildSettings (scenarioName: string, duration: TimeSpan, concurrentCopies: int) =
    { ScenarioName = scenarioName; Duration = duration; ConcurrentCopies = concurrentCopies }

[<Property>]
let ``validateRunnerContext() should return Ok for any args values`` (scenarioName: string, duration: TimeSpan, concurrentCopies: int) =
    let settings = buildSettings(scenarioName, duration, concurrentCopies)

    let validatedContext = buildConfig(scenarioName, [|settings|], [|scenarioName|])
                            |> Validation.validateRunnerContext
    
    if duration < TimeSpan.FromSeconds(1.0) then
        let errorMessage = validatedContext |> Result.getError
        Assert.Equal(sprintf "Duration for scenarios %A can not be less than 1 sec" [|scenarioName|], errorMessage)

    elif concurrentCopies < 1 then
        let errorMessage = validatedContext |> Result.getError
        Assert.Equal(sprintf "Concurrent copies for scenarios %A can not be less than 1" [|scenarioName|], errorMessage)
    else
        validatedContext |> Result.isOk |> Assert.True

[<Property>]
let ``validateRunnerContext() should fail when target scenrio name is not declared`` (scenarioName: string) =    
    let targetScenarios = [|scenarioName + "new_name"|]
    let errorMessage = buildConfig(scenarioName, [||], targetScenarios)
                        |> Validation.validateRunnerContext
                        |> Result.getError
    
    errorMessage.StartsWith (sprintf "Target scenarios %A is not found." targetScenarios)
    
    