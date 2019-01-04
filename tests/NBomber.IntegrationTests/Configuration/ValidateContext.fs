module Tests.Configuration.ValidateSettings

open System
open FSharp.Control.Tasks.V2.ContextInsensitive

open Xunit
open FsCheck.Xunit

open NBomber.FSharp
open NBomber.Configuration
open NBomber.Contracts
open NBomber.DomainServices
open NBomber.Domain.Errors

let buildConfig (scenarioName: string, steps: IStep[], settings: ScenarioSetting[], targetScenarios: string[], reportFileName: string, reportFormats: string[]) =
    let scenario = Scenario.create(scenarioName, Array.toList steps)
    let globalSettings = { ScenariosSettings = settings; TargetScenarios = targetScenarios; ReportFileName = reportFileName; ReportFormats = reportFormats }
    let config = { NBomberConfig.GlobalSettings = Some globalSettings }

    { Scenarios = [|scenario|]
      NBomberConfig = Some config
      ReportFileName = None
      ReportFormats = Array.empty }

let buildSettings (scenarioName: string, warmUpDuration: TimeSpan, duration: TimeSpan, concurrentCopies: int) =
    { ScenarioName = scenarioName
      WarmUpDuration = warmUpDuration
      Duration = duration
      ConcurrentCopies = concurrentCopies }

let buildGlobalSettings (settings: ScenarioSetting[], targetScenarios: string[], reportFileName: string, reportFormats: string[]) =
    { ScenariosSettings = settings
      TargetScenarios = targetScenarios
      ReportFileName = reportFileName
      ReportFormats = reportFormats }

[<Property>]
let ``validateGlobalSettings() should return Ok for any args values`` (scenarioName: string, warmUpDuration: TimeSpan, duration: TimeSpan, concurrentCopies: int, reportFileName: string) =
    let settings = buildSettings(scenarioName, warmUpDuration, duration, concurrentCopies)

    let validatedContext =
        buildGlobalSettings([|settings|], [|scenarioName|], reportFileName, Array.empty) |> Validation.validateGlobalSettings
    
    if duration < TimeSpan.FromSeconds(1.0) then
        let errorMessage = validatedContext |> Result.getError |> toString
        Assert.Equal(sprintf "Duration for scenarios '%s' can not be less than 1 sec." scenarioName, errorMessage)

    elif concurrentCopies < 1 then
        let errorMessage = validatedContext |> Result.getError |> toString
        Assert.Equal(sprintf "Concurrent copies for scenarios '%s' can not be less than 1." scenarioName, errorMessage)

    elif String.IsNullOrEmpty(reportFileName) then
        let errorMessage = validatedContext |> Result.getError |> toString
        Assert.Equal("Report File Name can not be empty string.", errorMessage)

    else
        validatedContext |> Result.isOk |> Assert.True

[<Property>]
let ``validateNaming() should return Ok for any args values`` (scenarioName: string, warmUpDuration: TimeSpan, duration: TimeSpan, concurrentCopies: int, reportFileName: string) =
    let settings = buildSettings(scenarioName, warmUpDuration, duration, concurrentCopies)

    let validatedContext =
        buildConfig(scenarioName, Array.empty, [|settings|], [|scenarioName|], reportFileName, Array.empty)
        |> Validation.validateNaming
    
    if String.IsNullOrEmpty(scenarioName) then
        let errorMessage = validatedContext |> Result.getError |> toString
        Assert.Equal("Scenario name can not be empty.", errorMessage) 

    else
        validatedContext |> Result.isOk |> Assert.True

[<Property>]
let ``validateRunnerContext() should fail when report formats are unknown`` (reportFormats: string[]) =    
    let settings = buildSettings("scenario_name", TimeSpan.FromSeconds 10.0, TimeSpan.FromSeconds 10.0, 10)

    let validatedContext =
        buildConfig("scenario_name", Array.empty, [|settings|], [|"scenario_name"|], "report_file_name", reportFormats)
        |> Validation.validateRunnerContext

    let atLeastOneReportFormatUnknown =
        reportFormats
        |> Array.map(Validation.isReportFormatSupported)
        |> Array.exists(fun x -> x.IsNone)

    let reportFormatsNotEmpty = reportFormats |> Array.isEmpty |> not

    if reportFormatsNotEmpty && atLeastOneReportFormatUnknown then
        let errorMessage = validatedContext |> Result.getError |> toString
        Assert.EndsWith("Allowed formats: Txt, Html or Csv.", errorMessage)

    else validatedContext |> Result.isOk |> Assert.True                   

[<Property>]
let ``validateRunnerContext() should fail when target scenrio name is not declared`` (scenarioName: string) =    
    let targetScenarios = [|scenarioName + "new_name"|]
    let errorMessage =
        buildConfig(scenarioName + "not_empty", Array.empty, Array.empty, targetScenarios, "report_file_name", Array.empty)
        |> Validation.validateRunnerContext
        |> Result.getError
        |> toString
    
    errorMessage.StartsWith (sprintf "Target scenarios '%s' is not found." (scenarioName + "new_name"))

[<Fact>]
let ``validateRunnerContext() should fail when scenario names are duplicates`` () =
    let scenario = Scenario.create("scenario", []);
    let context = { Scenarios = [|scenario; scenario|]; NBomberConfig = None; ReportFileName = None; ReportFormats = Array.empty }

    let errorMessage = context |> Validation.validateNaming |> Result.getError |> toString
    Assert.Equal("Scenario names should be unique.", errorMessage)

[<Fact>]
let ``validateRunnerContext() should fail when step names are duplicates`` () =
    let step = Step.createPull("simple step", ConnectionPool.none, fun _ -> task { return Response.Ok() })
    let scenario = Scenario.create("scenario", [step; step])
    let context = { Scenarios = [|scenario|]; NBomberConfig = None; ReportFileName = None; ReportFormats = Array.empty }

    let errorMessage = context |> Validation.validateNaming |> Result.getError |> toString
    Assert.Equal("Step names are not unique in scenarios: 'scenario'. Step names should be unique within scenario.", errorMessage)

[<Fact>]
let ``validateRunnerContext() should fail when at least one step name is empty`` () =
    let step = Step.createPull("", ConnectionPool.none, fun _ -> task { return Response.Ok() })
    let scenario = Scenario.create("scenario", [step])
    let context = { Scenarios = [|scenario|]; NBomberConfig = None; ReportFileName = None; ReportFormats = Array.empty }

    let errorMessage = context |> Validation.validateNaming |> Result.getError |> toString
    Assert.Equal("Step names are empty in scenarios: 'scenario'. Step names should not be empty within scenario.", errorMessage)
