﻿module internal NBomber.DomainServices.NBomberContext

open System
open System.IO
open FsToolkit.ErrorHandling
open NBomber
open NBomber.Extensions.Internal
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Contracts.Internal
open NBomber.Errors
open NBomber.Domain

module Validation =

    let checkAvailableTargets (regScenarios: ScenarioProps list) (targetScenarios: string list) =
        let allScenarios = regScenarios |> List.map(fun x -> x.ScenarioName)
        let notFoundScenarios = targetScenarios |> List.except allScenarios

        if List.isEmpty allScenarios then Error EmptyRegisterScenarios
        elif List.isEmpty notFoundScenarios then Ok targetScenarios
        else Error (TargetScenariosNotFound(notFoundScenarios, allScenarios))

    let checkReportName (name) =
        if String.IsNullOrWhiteSpace(name) then Error EmptyReportName
        elif Path.GetInvalidFileNameChars() |> name.IndexOfAny <> -1 then Error InvalidReportName
        else Ok name

    let checkReportFolder (folderPath) =
       if String.IsNullOrWhiteSpace(folderPath) then Error EmptyReportFolderPath
       elif Path.GetInvalidPathChars() |> folderPath.IndexOfAny <> -1 then Error InvalidReportFolderPath
       else Ok folderPath

    let checkReportingInterval (interval: TimeSpan) =
        if interval >= Constants.MinReportingInterval then Ok interval
        else Error <| ReportingIntervalSmallerThanMin

    let checkDuplicateScenarioSettings (settings: ScenarioSetting list) =
        let duplicates = settings |> Seq.map(fun x -> x.ScenarioName) |> String.filterDuplicates |> Seq.toList
        if duplicates.Length > 0 then Error (DuplicateScenarioNamesInConfig duplicates)
        else Ok settings

let getTestSuite (context: NBomberContext) =
    context.NBomberConfig
    |> Option.bind(fun x -> x.TestSuite)
    |> Option.defaultValue context.TestSuite

let getTestName (context: NBomberContext) =
    context.NBomberConfig
    |> Option.bind(fun x -> x.TestName)
    |> Option.defaultValue context.TestName

let getScenariosSettings (scenarios: DomainTypes.Scenario list) (context: NBomberContext) =
    let tryGetFromConfig (ctx: NBomberContext) = option {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        return! settings.ScenariosSettings
    }
    context
    |> tryGetFromConfig
    |> Option.map Validation.checkDuplicateScenarioSettings
    |> Option.defaultValue(Ok List.empty)

let getTargetScenarios (context: NBomberContext) =
    let targetScn =
        context.TargetScenarios
        |> Option.orElseWith(fun () ->
            context.NBomberConfig |> Option.bind(fun x -> x.TargetScenarios)
        )

    let allScns = context.RegisteredScenarios |> List.map(fun x -> x.ScenarioName)
    defaultArg targetScn allScns

let setTargetScenarios (scenarios: string list) (context: NBomberContext) =
    { context with TargetScenarios = Some scenarios }

let getReportFileName (context: NBomberContext) =
    let tryGetFromConfig (ctx: NBomberContext) = option {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        return! settings.ReportFileName
    }
    context
    |> tryGetFromConfig
    |> Option.orElse(context.Reporting.FileName)

let getReportFileNameOrDefault (currentTime: DateTime) (context: NBomberContext) =
    context
    |> getReportFileName
    |> Option.defaultValue(
        let currentTime = currentTime.ToString("yyyy-MM-dd--HH-mm-ss")
        $"{Constants.DefaultReportName}_{currentTime}"
    )

let getEnableHintAnalyzer (context: NBomberContext) =
    let tryGetFromConfig (ctx: NBomberContext) = option {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        return! settings.EnableHintsAnalyzer
    }
    context
    |> tryGetFromConfig
    |> Option.defaultValue context.EnableHintsAnalyzer

let private getReportFolder (context: NBomberContext) =
    let tryGetFromConfig (ctx: NBomberContext) = option {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        return! settings.ReportFolder
    }
    context
    |> tryGetFromConfig
    |> Option.orElse context.Reporting.FolderName

let getReportFolderOrDefault (sessionId: string) (context: NBomberContext) =
    context
    |> getReportFolder
    |> Option.defaultValue(Path.Combine(Constants.DefaultReportFolder, sessionId))

let getReportFormats (context: NBomberContext) =
    let tryGetFromConfig (ctx: NBomberContext) = option {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        let! formats = settings.ReportFormats
        return formats
    }
    context
    |> tryGetFromConfig
    |> Option.defaultValue context.Reporting.Formats

let getReportingInterval (context: NBomberContext) =
    let tryGetFromConfig (ctx: NBomberContext) = option {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        return! settings.ReportingInterval
    }
    context
    |> tryGetFromConfig
    |> Option.map Validation.checkReportingInterval
    |> Option.defaultValue(context.Reporting.ReportingInterval |> Validation.checkReportingInterval)

let getUseHintsAnalyzer (context: NBomberContext) =
    let tryGetFromConfig (ctx: NBomberContext) = option {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        return! settings.EnableHintsAnalyzer
    }
    context
    |> tryGetFromConfig
    |> Option.defaultValue context.EnableHintsAnalyzer

let createSessionArgs (testInfo: TestInfo) (scenarios: DomainTypes.Scenario list) (context: NBomberContext) =
    result {
        let! targetScenarios   = context |> getTargetScenarios |> Validation.checkAvailableTargets(context.RegisteredScenarios) |> Result.mapError(AppError.create)
        let! reportName        = context |> getReportFileNameOrDefault(DateTime.UtcNow) |> Validation.checkReportName |> Result.mapError(AppError.create)
        let! reportFolder      = context |> getReportFolderOrDefault(testInfo.SessionId) |> Validation.checkReportFolder |> Result.mapError(AppError.create)
        let reportFormats      = context |> getReportFormats
        let! reportingInterval = context |> getReportingInterval |> Result.mapError(AppError.create)
        let! scenariosSettings  = context |> getScenariosSettings(scenarios) |> Result.mapError(AppError.create)
        let enableHintsAnalyzer = context |> getEnableHintAnalyzer

        let nbConfig = {
            TestSuite = Some testInfo.TestSuite
            TestName = Some testInfo.TestName
            TargetScenarios = Some targetScenarios
            GlobalSettings = Some {
                ScenariosSettings = Some scenariosSettings
                ReportFileName = Some reportName
                ReportFolder = Some reportFolder
                ReportFormats = Some reportFormats
                ReportingInterval = Some reportingInterval
                EnableHintsAnalyzer = Some enableHintsAnalyzer
            }
        }

        return { TestInfo = testInfo; NBomberConfig = nbConfig }
    }

let createScenarios (context: NBomberContext) =
    context.RegisteredScenarios |> Scenario.createScenarios

let createBaseContext (testInfo) (getNodeInfo: unit -> NodeInfo) (logger) =
    { new IBaseContext with
        member _.TestInfo = testInfo
        member _.GetNodeInfo() = getNodeInfo()
        member _.Logger = logger }
