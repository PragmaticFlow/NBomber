module internal NBomber.DomainServices.Validation

open System

open NBomber.Contracts
open NBomber.Configuration
    
let targetScenarioIsNotPresent (globalSettings: GlobalSettings) =
    let availableScenarios = globalSettings.ScenariosSettings |> Array.map(fun x -> x.ScenarioName)
    let notFound = globalSettings.TargetScenarios |> Array.except availableScenarios
    
    match notFound with
    | [||] -> Ok(globalSettings)
    | _ -> sprintf "Target scenarios %A is not found. Available scenarios are %A" notFound availableScenarios
            |> Error

let durationGreaterThenSecond (globalSettings: GlobalSettings) =
    globalSettings.ScenariosSettings
    |> Array.filter(fun x -> x.Duration < TimeSpan.FromSeconds(1.0))
    |> Array.map(fun x -> x.ScenarioName)
    |> function
    | [||] -> Ok(globalSettings)
    | scenariosWithIncorrectDuration -> sprintf "Duration for scenarios %A can not be less than 1 sec" scenariosWithIncorrectDuration
                                        |> Error

let concurrentCopiesGreaterThenOne (globalSettings: GlobalSettings) =
    globalSettings.ScenariosSettings
    |> Array.filter(fun x -> x.ConcurrentCopies < 1)
    |> Array.map(fun x -> x.ScenarioName)
    |> function
    | [||] -> Ok(globalSettings)
    | scenariosWithIncorrectConcurrentCopies -> sprintf "Concurrent copies for scenarios %A can not be less than 1" scenariosWithIncorrectConcurrentCopies
                                                |> Error

let reportFileNameIsNotEmpty (globalSettings: GlobalSettings) =
    match globalSettings.ReportFileName with
    | Some reportFileName -> 
        if String.IsNullOrEmpty(reportFileName) then
            Error("Report File Name can not be empty string.")
        else
            Ok(globalSettings)
    | None -> Ok(globalSettings)

let parseReportFormat (reportFormat: string) =
    match reportFormat with 
    | "Txt" -> Some(ReportFormat.Txt)
    | "Html" -> Some(ReportFormat.Html)
    | "Csv" -> Some(ReportFormat.Csv)
    | _ -> None

let parseAndGetValidReportFormats (reportFormat: string[]) =
    reportFormat |> Array.choose(parseReportFormat)

let reportFormatsAreWithinAllowedReportFormats (globalSettings: GlobalSettings) =
    globalSettings.ReportFormats
    |> Array.choose(fun x -> x |> parseReportFormat |> function | None -> Some(x) | _ -> None)
    |> function
    | [||] -> Ok(globalSettings)
    | unknownReportFormats -> Error(sprintf "Unknown Report Formats '%A'. Allowed formats: Txt, Html or Csv." unknownReportFormats)

let validateRunnerContext(context: NBomberRunnerContext) = 
    let globalSettings = context.NBomberConfig |> Option.bind(fun config -> config.GlobalSettings)
    match globalSettings with
    | Some globalSettings -> globalSettings
                             |> targetScenarioIsNotPresent 
                             |> Result.bind durationGreaterThenSecond
                             |> Result.bind concurrentCopiesGreaterThenOne
                             |> Result.bind reportFileNameIsNotEmpty
                             |> Result.bind reportFormatsAreWithinAllowedReportFormats
                             |> function
                             | Ok _ -> Ok(context)
                             | Error msg -> Error(msg)
    | None -> Ok(context)