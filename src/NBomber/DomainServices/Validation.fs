module internal NBomber.DomainServices.Validation

open System

open NBomber.Contracts
open NBomber.Configuration
open NBomber.Domain
open NBomber.Domain.Errors

let isReportFormatSupported (reportFormat: string) =
    if String.IsNullOrEmpty(reportFormat) then None
    else
        match reportFormat.ToLower().Trim() with
        | "txt"  -> Some ReportFormat.Txt
        | "html" -> Some ReportFormat.Html
        | "csv"  -> Some ReportFormat.Csv
        | "md"   -> Some ReportFormat.Md
        | _      -> None

let private getUnsupportedReportFormats (reportFormats: string[]) =
    reportFormats |> Array.filter(fun x -> x |> isReportFormatSupported |> Option.isNone)

let private uniqueCount (strings: string[]) =
    strings |> Array.distinct |> Array.length

let private isTargetScenarioPresent (globalSettings: GlobalSettings) =
    let availableScenarios = globalSettings.ScenariosSettings |> Array.map(fun x -> x.ScenarioName)
    let notFoundScenarios = globalSettings.TargetScenarios |> Array.except availableScenarios

    if Array.isEmpty(notFoundScenarios) then Ok globalSettings
    else (notFoundScenarios, availableScenarios) |> ScenariosNotFound |> Error

let private isDurationGreaterThenSecond (globalSettings: GlobalSettings) =
    let scenariosWithIncorrectDuration =
        globalSettings.ScenariosSettings
        |> Array.filter(fun x -> x.Duration < TimeSpan.FromSeconds 1.0)
        |> Array.map(fun x -> x.ScenarioName)

    if Array.isEmpty(scenariosWithIncorrectDuration) then Ok globalSettings
    else scenariosWithIncorrectDuration |> DurationLessThanOneSecond |> Error

let private isConcurrentCopiesGreaterThenOne (globalSettings: GlobalSettings) =
    let scenariosWithIncorrectConcurrentCopies =
        globalSettings.ScenariosSettings
        |> Array.filter(fun x -> x.ConcurrentCopies < 1)
        |> Array.map(fun x -> x.ScenarioName)
    
    if Array.isEmpty(scenariosWithIncorrectConcurrentCopies) then Ok globalSettings
    else scenariosWithIncorrectConcurrentCopies |> ConcurrentCopiesLessThanOne |> Error

let private isEmptyScenarioNameExist (scenarios: Scenario[]) =
    let isAnyScenarioNullOrEmpty = scenarios |> Array.exists(fun x -> String.IsNullOrEmpty(x.ScenarioName))
    if isAnyScenarioNullOrEmpty then Error EmptyScenarioName
    else Ok scenarios

let private isEmptyReportFileNameExist (globalSettings: GlobalSettings) =
    if String.IsNullOrEmpty(globalSettings.ReportFileName) then Error EmptyReportFileName
    else Ok globalSettings

let private validateReportFormat (globalSettings: GlobalSettings) =
    let unsupportedFormats = getUnsupportedReportFormats(globalSettings.ReportFormats)

    if Array.isEmpty(unsupportedFormats) then Ok globalSettings
    else unsupportedFormats |> UnsupportedReportFormat |> Error

let private isScenarioNameDuplicate (scenarios: Scenario[]) =
    let scenarioNames = scenarios |> Array.map(fun x -> x.ScenarioName)
    if uniqueCount(scenarioNames) = scenarios.Length then Ok scenarios
    else Error DuplicateScenarios

let private isStepNameDuplicate (scenarios: Scenario[]) =
    let duplicates =
        scenarios
        |> Array.filter(fun x ->
            let stepNames =
                x.Steps
                |> Array.map(fun x -> x :?> DomainTypes.Step |> Step.getName)
                |> Array.filter(fun x -> x <> "pause")

            not(Array.isEmpty(stepNames)) && uniqueCount(stepNames) <> stepNames.Length)
        |> Array.map(fun x -> x.ScenarioName)
    
    if Array.isEmpty(duplicates) then Ok scenarios
    else duplicates |> DuplicateSteps |> Error

let private isEmptyStepNameExist (scenarios: Scenario[]) =
    let scenariosWithEmptySteps =
        scenarios
        |> Array.filter(fun x ->
            x.Steps
            |> Array.map(fun x -> x :?> DomainTypes.Step |> Step.getName)
            |> Array.exists(String.IsNullOrEmpty))
        |> Array.map(fun x -> x.ScenarioName)
    
    if Array.isEmpty(scenariosWithEmptySteps) then Ok scenarios
    else scenariosWithEmptySteps |> EmptyStepName |> Error

let internal validateNaming (context: NBomberContext) =
    context.Scenarios
    |> isScenarioNameDuplicate
    |> Result.bind isStepNameDuplicate
    |> Result.bind isEmptyScenarioNameExist
    |> Result.bind isEmptyStepNameExist

let internal validateGlobalSettings (globalSettings: GlobalSettings) =
    globalSettings
    |> isTargetScenarioPresent 
    |> Result.bind isDurationGreaterThenSecond
    |> Result.bind isConcurrentCopiesGreaterThenOne
    |> Result.bind isEmptyReportFileNameExist
    |> Result.bind validateReportFormat

let validateRunnerContext (context: NBomberContext) =
    let validatedContext = validateNaming(context)

    match validatedContext with
    | Ok _ ->
        context.NBomberConfig
        |> Option.bind(fun config -> config.GlobalSettings)
        |> function
        | Some globalSettings ->
            match validateGlobalSettings(globalSettings) with
            | Ok _ -> Ok context
            | Error msg -> Error msg
        | None -> Ok context
    | Error msg -> Error msg