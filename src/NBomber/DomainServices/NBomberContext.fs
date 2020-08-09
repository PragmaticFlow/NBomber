module internal NBomber.DomainServices.NBomberContext

open System
open System.Globalization

open FsToolkit.ErrorHandling

open NBomber
open NBomber.Extensions.InternalExtensions
open NBomber.Extensions.Operator.Result
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
        if String.IsNullOrWhiteSpace(name) then Error EmptyReportName
        else Ok name

    let checkSendStatsInterval (interval: TimeSpan) =
        if interval >= Constants.MinSendStatsInterval then Ok interval
        else Error <| SendStatsValueSmallerThanMin

    let checkSendStatsSettings (interval: string) =
        match TimeSpan.TryParseExact(interval, "hh\:mm\:ss", CultureInfo.InvariantCulture) with
        | true, value -> checkSendStatsInterval(value)
        | false, _    -> Error <| SendStatsConfigValueHasInvalidFormat(interval)

    let checkWarmUpSettings (settings: ScenarioSetting list) =
        settings
        |> Seq.tryFind(fun x ->
            match TimeSpan.TryParseExact(x.WarmUpDuration, "hh\:mm\:ss", CultureInfo.InvariantCulture) with
            | true, _  -> false
            | false, _ -> true
        )
        |> Option.map(fun x -> Error <| WarmUpConfigValueHasInvalidFormat(x.ScenarioName, x.WarmUpDuration))
        |> Option.defaultValue(Ok settings)

    let checkLoadSimulationsSettings (settings: ScenarioSetting list) =
        settings
        |> Seq.tryFind(fun scenarioSetting ->
            try
                scenarioSetting.LoadSimulationsSettings
                |> Seq.iter(NBomber.Domain.LoadTimeLine.createSimulationFromSettings >> ignore)
                false
            with
            | ex -> true
        )
        |> Option.map(fun invalidScenario -> Error <| LoadSimulationConfigValueHasInvalidFormat(invalidScenario.ScenarioName))
        |> Option.defaultValue(Ok settings)

let empty = {
    TestSuite = Constants.DefaultTestSuite
    TestName = Constants.DefaultTestName
    RegisteredScenarios = List.empty
    NBomberConfig = None
    InfraConfig = None
    CreateLoggerConfig = None
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
    let tryGetFromConfig (ctx) = maybe {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        return! settings.ScenariosSettings
    }
    context
    |> tryGetFromConfig
    |> Option.map(Validation.checkWarmUpSettings >=> Validation.checkLoadSimulationsSettings)
    |> Option.defaultValue(Ok List.empty)

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
        return! settings.SendStatsInterval
    }
    context
    |> tryGetFromConfig
    |> Option.map(Validation.checkSendStatsSettings)
    |> Option.defaultValue(Ok context.SendStatsInterval)

let getConnectionPoolSettings (context: NBomberContext) =
    let tryGetFromConfig (ctx) = maybe {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        let! scnSettings = settings.ScenariosSettings

        return scnSettings |> List.collect(fun scn ->
            maybe {
                let! poolSettings = scn.ConnectionPoolSettings
                return poolSettings |> List.map(fun pool ->
                    let newName = Scenario.createConnectionPoolName(scn.ScenarioName, pool.PoolName)
                    { pool with PoolName = newName }
                )
            }
            |> Option.defaultValue List.empty
        )
    }
    context
    |> tryGetFromConfig
    |> Option.defaultValue List.empty

let createSessionArgs (testInfo: TestInfo) (context: NBomberContext) =
    result {
        let! targetScenarios   = context |> getTargetScenarios |> Validation.checkAvailableTargets(context.RegisteredScenarios)
        let! reportName        = context |> getReportFileName  |> Validation.checkReportName
        let! sendStatsInterval = context |> getSendStatsInterval
        let! scenariosSettings  = context |> getScenariosSettings
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
