module internal NBomber.DomainServices.NBomberContext

open System

open FsToolkit.ErrorHandling

open NBomber
open NBomber.Extensions
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Errors
open NBomber.Domain

type SessionArgs = {
    TestInfo: TestInfo
    ScenariosSettings: ScenarioSetting list
    TargetScenarios: string list
    ConnectionPoolSettings: ConnectionPoolSetting list
    SendStatsInterval: TimeSpan
} with
    static member Empty = {
        TestInfo = { SessionId = ""; TestSuite = ""; TestName = "" }
        ScenariosSettings = List.empty
        TargetScenarios = List.empty
        ConnectionPoolSettings = List.empty
        SendStatsInterval = Constants.MinSendStatsInterval
    }

module Validation =

    let checkAvailableTargets (scenarios: Contracts.Scenario list) (targetScenarios: string list) =
        let allScenarios = scenarios |> List.map(fun x -> x.ScenarioName)
        let notFoundScenarios = targetScenarios |> List.except(allScenarios)

        if List.isEmpty(notFoundScenarios) then Ok targetScenarios
        else Error <| TargetScenariosNotFound(notFoundScenarios, allScenarios)

    let checkReportName (name: string) =
        if String.IsNullOrWhiteSpace(name) then Error <| EmptyReportName
        else Ok name

    let checkSendStatsInterval (interval: TimeSpan) =
        if interval >= Constants.MinSendStatsInterval then Ok interval
        else Error <| SendStatsIntervalIsWrong(Constants.MinSendStatsInterval.TotalSeconds)

let empty = {
    TestSuite = Constants.DefaultTestSuite
    TestName = Constants.DefaultTestName
    RegisteredScenarios = List.empty
    NBomberConfig = None
    InfraConfig = None
    ReportFileName = None
    ReportFormats = Constants.AllReportFormats
    ReportingSinks = List.empty
    SendStatsInterval = Constants.MinSendStatsInterval
    Plugins = List.empty
    ApplicationType = None
}

let getTestSuite (context: NBomberContext) =
    context.NBomberConfig
    |> Option.bind(fun x -> x.TestSuite)
    |> Option.defaultValue context.TestSuite

let getTestName (context: NBomberContext) =
    context.NBomberConfig
    |> Option.bind(fun x -> x.TestName)
    |> Option.defaultValue context.TestName

let getScenariosSettings (context: NBomberContext) =
    context.NBomberConfig
    |> Option.bind(fun x -> x.GlobalSettings)
    |> Option.bind(fun x -> x.ScenariosSettings)
    |> Option.defaultValue List.empty

let getTargetScenarios (context: NBomberContext) =
    let targetScn =
        context.NBomberConfig
        |> Option.bind(fun x -> x.TargetScenarios)

    let allScns = context.RegisteredScenarios |> List.map(fun x -> x.ScenarioName)
    defaultArg targetScn allScns

let getReportFileName (context: NBomberContext) =
    let tryGetFromConfig (ctx) = maybe {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        return! settings.ReportFileName
    }
    context
    |> tryGetFromConfig
    |> Option.orElse(context.ReportFileName)
    |> Option.defaultValue(Constants.DefaultReportName)

let getReportFormats (context: NBomberContext) =
    let tryGetFromConfig (ctx) = maybe {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        let! formats = settings.ReportFormats
        return formats
    }
    context
    |> tryGetFromConfig
    |> Option.orElse(if List.isEmpty context.ReportFormats then None
                     else Some context.ReportFormats)
    |> Option.defaultValue List.empty

let getSendStatsInterval (context: NBomberContext) =
    let tryGetFromConfig (ctx) = maybe {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        let! intervalInDataTime = settings.SendStatsInterval
        return intervalInDataTime.TimeOfDay
    }
    context
    |> tryGetFromConfig
    |> Option.defaultValue context.SendStatsInterval

let getConnectionPoolSettings (context: NBomberContext) =
    let tryGetFromConfig (ctx) = maybe {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        return! settings.ConnectionPoolSettings
    }
    context
    |> tryGetFromConfig
    |> Option.defaultValue List.empty

let createSessionArgs (testInfo: TestInfo) (context: NBomberContext) =
    result {
        let! targetScenarios   = context |> getTargetScenarios |> Validation.checkAvailableTargets(context.RegisteredScenarios)
        let! reportName        = context |> getReportFileName  |> Validation.checkReportName
        let! sendStatsInterval = context |> getSendStatsInterval |> Validation.checkSendStatsInterval
        let scenariosSettings  = context |> getScenariosSettings
        let connectionPoolSettings = context |> getConnectionPoolSettings

        return {
          TestInfo = testInfo
          ScenariosSettings = scenariosSettings
          TargetScenarios = targetScenarios
          ConnectionPoolSettings = connectionPoolSettings
          SendStatsInterval = sendStatsInterval
        }
    }
    |> Result.mapError(AppError.create)

let createScenarios (context: NBomberContext) =
    context.RegisteredScenarios |> Scenario.createScenarios
