module internal NBomber.DomainServices.NBomberContext

open System
open System.IO

open FsToolkit.ErrorHandling

open NBomber
open NBomber.Extensions.Internal
open NBomber.Extensions.Operator.Result
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Contracts.Internal
open NBomber.Errors
open NBomber.Domain
open NBomber.Infra.Dependency

module EnterpriseValidation =

    let validateReportingSinks (dep: IGlobalDependency) =
        match dep.NodeType with
        | SingleNode when dep.ReportingSinks.Length > 0 ->
            Error(EnterpriseOnlyFeature "ReportingSinks feature supported only for the Enterprise version")
        | _ ->
            Ok()

    let validateStepInterception (context: NBomberContext) =
        let scenarios =
            context.RegisteredScenarios
            |> List.filter(fun x -> x.StepInterception.IsSome)
            |> List.map(fun x -> x.ScenarioName)

        if scenarios.Length > 0 then
            let names = scenarios |> String.concatWithComma
            Error(EnterpriseOnlyFeature $"Scenario: '{names}' is using StepInterception feature that supported only for the Enterprise version")
        else
            Ok()

    let validate (dep: IGlobalDependency) (context: NBomberContext) =
        result {
            do! validateReportingSinks dep
            do! validateStepInterception context
            return context
        }
        |> Result.mapError AppError.create

module Validation =

    let checkAvailableTargets (regScenarios: Scenario list) (targetScenarios: string list) =
        let allScenarios = regScenarios |> List.map(fun x -> x.ScenarioName)
        let notFoundScenarios = targetScenarios |> List.except allScenarios

        if List.isEmpty allScenarios then Error EmptyRegisterScenarios
        elif List.isEmpty notFoundScenarios then Ok targetScenarios
        else Error <| TargetScenariosNotFound(notFoundScenarios, allScenarios)

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
        if duplicates.Length > 0 then Error(DuplicateScenarioNamesInConfig duplicates)
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

let getDefaultStepTimeoutMs (context: NBomberContext) =
    let tryGetFromConfig (ctx: NBomberContext) = option {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        return! settings.DefaultStepTimeoutMs
    }
    context
    |> tryGetFromConfig
    |> Option.defaultValue context.DefaultStepTimeoutMs

let getMaxFailCount (context: NBomberContext) =
    let tryGetFromConfig (ctx: NBomberContext) = option {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        return! settings.MaxFailCount
    }
    context
    |> tryGetFromConfig
    |> Option.defaultValue context.MaxFailCount

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

let getClientFactorySettings (context: NBomberContext) =
    let tryGetFromConfig (ctx: NBomberContext) = option {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        let! scnSettings = settings.ScenariosSettings

        return scnSettings |> List.collect(fun scn ->
            option {
                let! factorySettings = scn.ClientFactorySettings
                return
                    factorySettings
                    |> List.map(fun factorySetting ->
                        let newName = ClientFactory.createFullName factorySetting.FactoryName scn.ScenarioName
                        { factorySetting with FactoryName = newName }
                    )
            }
            |> Option.defaultValue List.empty
        )
    }
    context
    |> tryGetFromConfig
    |> Option.defaultValue List.empty

let createSessionArgs (testInfo: TestInfo) (scenarios: DomainTypes.Scenario list) (context: NBomberContext) =
    result {
        let! targetScenarios   = context |> getTargetScenarios |> Validation.checkAvailableTargets context.RegisteredScenarios
        let! reportName        = context |> getReportFileNameOrDefault DateTime.UtcNow |> Validation.checkReportName
        let! reportFolder      = context |> getReportFolderOrDefault testInfo.SessionId |> Validation.checkReportFolder
        let reportFormats      = context |> getReportFormats
        let! reportingInterval = context |> getReportingInterval
        let! scenariosSettings  = context |> getScenariosSettings scenarios
        let clientFactorySettings = context |> getClientFactorySettings
        let enableHintsAnalyzer = context |> getEnableHintAnalyzer
        let stepTimeout = context |> getDefaultStepTimeoutMs
        let maxFailCount = context |> getMaxFailCount

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
                DefaultStepTimeoutMs = Some stepTimeout
                MaxFailCount = Some maxFailCount
            }
        }

        return {
            TestInfo = testInfo
            NBomberConfig = nbConfig
            UpdatedClientFactorySettings = clientFactorySettings
        }
    }
    |> Result.mapError AppError.create

let createScenarios (context: NBomberContext) =
    context.RegisteredScenarios |> Scenario.createScenarios

let createBaseContext (testInfo) (getNodeInfo: unit -> NodeInfo) (token) (logger) =
    { new IBaseContext with
        member _.TestInfo = testInfo
        member _.GetNodeInfo() = getNodeInfo()
        member _.CancellationToken = token
        member _.Logger = logger }
