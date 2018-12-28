module Tests.Configuration.ValidateSettings

open System
open Xunit
open FsCheck.Xunit

open NBomber.FSharp
open NBomber.Configuration
open NBomber.DomainServices
open NBomber.Contracts

let buildConfig (scenarioName: string, settings: ScenarioSetting[], targetScenarios: string[], reportFileName: string option, reportFormats: string[]) =
    let scenario = Scenario.create(scenarioName, [])
    let globalSettings = { ScenariosSettings = settings; TargetScenarios = targetScenarios; ReportFileName = reportFileName; ReportFormats = reportFormats}
    let config = { NBomberConfig.GlobalSettings = Some globalSettings }
    
    { Scenarios = [|scenario|]; NBomberConfig = Some config; ReportFileName = None; ReportFormats = [||] }

let buildSettings (scenarioName: string, warmUpDuration: TimeSpan, duration: TimeSpan, concurrentCopies: int) =
    { ScenarioName = scenarioName; WarmUpDuration = warmUpDuration; Duration = duration; ConcurrentCopies = concurrentCopies }

[<Property>]
let ``validateRunnerContext() should return Ok for any args values`` (scenarioName: string, warmUpDuration: TimeSpan, duration: TimeSpan, concurrentCopies: int, reportFileName: string) =
    let settings = buildSettings(scenarioName, warmUpDuration, duration, concurrentCopies)

    let validatedContext = buildConfig(scenarioName, [|settings|], [|scenarioName|], Some(reportFileName), [||])
                            |> Validation.validateRunnerContext
    
    if duration < TimeSpan.FromSeconds(1.0) then
        let errorMessage = validatedContext |> Result.getError
        Assert.Equal(sprintf "Duration for scenarios %A can not be less than 1 sec" [|scenarioName|], errorMessage)

    elif concurrentCopies < 1 then
        let errorMessage = validatedContext |> Result.getError
        Assert.Equal(sprintf "Concurrent copies for scenarios %A can not be less than 1" [|scenarioName|], errorMessage)
    
    elif String.IsNullOrEmpty(reportFileName) then
        let errorMessage = validatedContext |> Result.getError
        Assert.Equal("Report File Name can not be empty string.", errorMessage)
        
    else
        validatedContext |> Result.isOk |> Assert.True

[<Property>]
let ``validateRunnerContext() should fail when report formats are unknown`` (reportFormats: string[]) =    
    let settings = buildSettings("scenario_name", TimeSpan.FromSeconds(10.0), TimeSpan.FromSeconds(10.0), 10)

    let validatedContext = buildConfig("scenario_name", [|settings|], [|"scenario_name"|], None, reportFormats)
                            |> Validation.validateRunnerContext

    let atLeastOneReportFormatUnknown = reportFormats |> Array.map(Validation.parseReportFormat) |> Array.exists(fun x -> x.IsNone)
    let reportFormatsNotEmpty = reportFormats |> Array.isEmpty |> not

    if reportFormatsNotEmpty && atLeastOneReportFormatUnknown then
        let errorMessage = validatedContext |> Result.getError
        Assert.EndsWith("Allowed formats: Txt, Html or Csv.", errorMessage)
    else
        validatedContext |> Result.isOk |> Assert.True                   

[<Property>]
let ``validateRunnerContext() should fail when target scenrio name is not declared`` (scenarioName: string) =    
    let targetScenarios = [|scenarioName + "new_name"|]
    let errorMessage = buildConfig(scenarioName, Array.empty, targetScenarios, None, [||])
                        |> Validation.validateRunnerContext
                        |> Result.getError
    
    errorMessage.StartsWith (sprintf "Target scenarios %A is not found." targetScenarios)
    