module internal NBomber.Infra.TestContext

open System

open NBomber
open NBomber.Extensions
open NBomber.Configuration
open NBomber.Contracts

let empty =
    { TestSuite = Constants.DefaultTestSuite
      TestName = Constants.DefaultTestName
      RegisteredScenarios = List.empty
      NBomberConfig = None
      InfraConfig = None
      ReportFileName = None
      ReportFormats = Constants.AllReportFormats
      ReportingSinks = List.empty
      SendStatsInterval = TimeSpan.FromSeconds Constants.MinSendStatsIntervalSec
      Extensions = List.empty }

let getTestSuite (context: TestContext) =
    context.NBomberConfig
    |> Option.bind(fun x -> x.TestSuite)
    |> Option.defaultValue context.TestSuite

let getTestName (context: TestContext) =
    context.NBomberConfig
    |> Option.bind(fun x -> x.TestName)
    |> Option.defaultValue context.TestName

let getScenariosSettings (context: TestContext) =
    context.NBomberConfig
    |> Option.bind(fun x -> x.GlobalSettings)
    |> Option.bind(fun x -> x.ScenariosSettings)
    |> Option.defaultValue List.empty
    |> List.toArray

let getTargetScenarios (context: TestContext) =
    let targetScn =
        context.NBomberConfig
        |> Option.bind(fun x -> x.GlobalSettings)
        |> Option.bind(fun x -> x.TargetScenarios)

    let allScns = context.RegisteredScenarios
                  |> List.map(fun x -> x.ScenarioName)

    defaultArg targetScn allScns
    |> List.toArray

let getReportFileName (sessionId: string, context: TestContext) =
    let tryGetFromConfig (ctx) = maybe {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        return! settings.ReportFileName
    }
    context
    |> tryGetFromConfig
    |> Option.orElse(context.ReportFileName)
    |> Option.defaultValue("report_" + sessionId)

let getReportFormats (context: TestContext) =
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
    |> Option.defaultValue Constants.AllReportFormats

let getSendStatsInterval (context: TestContext) =
    let tryGetFromConfig (ctx) = maybe {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        let! intervalInDataTime = settings.SendStatsInterval
        return intervalInDataTime.TimeOfDay
    }
    context
    |> tryGetFromConfig
    |> Option.defaultValue context.SendStatsInterval

let getConnectionPoolSettings (context: TestContext) =
    let tryGetFromConfig (ctx) = maybe {
        let! config = ctx.NBomberConfig
        let! settings = config.GlobalSettings
        return! settings.ConnectionPoolSettings
    }
    context
    |> tryGetFromConfig
    |> Option.defaultValue List.empty
