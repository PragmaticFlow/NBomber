module internal NBomber.DomainServices.Reports.ConsoleReport

open System
open System.Collections.Generic
open System.Data

open Serilog

open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Domain.Stats
open NBomber.Extensions
open NBomber.Extensions.InternalExtensions
open NBomber.Infra

let okColor: (obj -> string)    = string >> Console.escapeMarkup >> Console.okColor
let errorColor: (obj -> string) = string >> Console.escapeMarkup >> Console.errorColor
let blueColor: (obj -> string)  = string >> Console.escapeMarkup >> Console.blueColor

module ConsoleTestInfo =

    let printTestInfo (testInfo: TestInfo) =
        [ Console.addHeader "test info"
          Console.addLine String.Empty
          Console.addLine $"test suite: {okColor testInfo.TestSuite}"
          Console.addLine $"test name: {okColor testInfo.TestName}"
          Console.addLine $"session id: {okColor testInfo.SessionId}"
          Console.addLine String.Empty ]

module ConsoleStatusCodesStats =

    let printScenarioHeader (scenarioName: string)  =
        Console.addLine $"status codes for scenario: {Console.okColor scenarioName}"

    let printStatusCodeTable (scnStats: ScenarioStats)  =
        let createTableRows = ReportHelper.StatusCodesStats.createTableRows okColor errorColor
        let headers = ["status code"; "count"; "message"]
        let rows = createTableRows scnStats
        Console.addTable headers rows

module ConsoleLoadSimulations =

    let print (simulations: LoadSimulation list) =
        let printLoadSimulation = ReportHelper.LoadSimulation.print okColor
        let simulationsList = simulations |> List.map(printLoadSimulation >> Console.addLine)
        Console.addLine "load simulations: " :: simulationsList

module ConsoleNodeStats =

    let private printDataKb (consoleColorize: float -> string) (bytes: int) =
        $"{bytes |> Statistics.Converter.fromBytesToKb |> consoleColorize} KB"

    let private printAllData (bytes: int64) =
        $"{bytes |> Statistics.Converter.fromBytesToMb |> Console.okColor} MB"

    let private printScenarioHeader (scnStats: ScenarioStats) =
        [ Console.addLine $"scenario: {scnStats.ScenarioName |> Console.escapeMarkup |> Console.okColor}"
          Console.addLine $"  - ok count: {Console.okColor scnStats.OkCount}"
          Console.addLine $"  - fail count: {Console.errorColor scnStats.FailCount}"
          Console.addLine $"  - all data: {printAllData scnStats.AllBytes}"
          Console.addLine $"  - duration: {Console.okColor scnStats.Duration}" ]

    let private printStepStatsHeader (stepStats: StepStats[]) =
        let print (stats) = seq {
            $"step: {stats.StepName |> Console.escapeMarkup |> Console.blueColor}"
            $"  - timeout: {stats.StepInfo.Timeout.Milliseconds |> Console.okColor} ms"
            $"  - client factory: {stats.StepInfo.ClientFactoryName |> Console.escapeMarkup |> Console.okColor}, clients: {Console.okColor stats.StepInfo.ClientFactoryClientCount}"
        }

        stepStats |> Seq.map print |> Console.addList

    let private printStepStatsTable (isOkStats: bool) (stepStats: StepStats[]) =
        let printStepStatsRow = ReportHelper.StepStats.printStepStatsRow isOkStats okColor errorColor blueColor
        let headers = ["step"; if isOkStats then "ok stats" else "fail stats"]
        let rows = stepStats |> Seq.mapi(printStepStatsRow) |> List.concat
        Console.addTable headers rows

    let private printScenarioStatusCodes (scnStats: ScenarioStats) =
        [ ConsoleStatusCodesStats.printScenarioHeader scnStats.ScenarioName
          ConsoleStatusCodesStats.printStatusCodeTable scnStats ]

    let private printScenarioStats (scnStats: ScenarioStats) (simulations: LoadSimulation list) =
        [ yield! printScenarioHeader scnStats
          Console.addLine String.Empty

          yield! ConsoleLoadSimulations.print simulations
          Console.addLine String.Empty

          yield! printStepStatsHeader scnStats.StepStats
          Console.addLine String.Empty

          printStepStatsTable true scnStats.StepStats

          if Statistics.ScenarioStats.failStepStatsExist scnStats then
              printStepStatsTable false scnStats.StepStats

          if scnStats.StatusCodes.Length > 0 then
              Console.addLine String.Empty
              yield! printScenarioStatusCodes scnStats ]

    let printNodeStats (stats: NodeStats) (loadSimulations: IDictionary<string, LoadSimulation list>) =
        let scenarioStats =
            stats.ScenarioStats
            |> Seq.map(fun scnStats ->
                [ yield! printScenarioStats scnStats loadSimulations[scnStats.ScenarioName]
                  Console.addLine String.Empty ]
            )
            |> List.concat

        [ Console.addHeader "scenario stats"
          Console.addLine String.Empty
          yield! scenarioStats ]

module ConsolePluginStats =

    let private printPluginStatsHeader (table: DataTable) =
        Console.addLine $"plugin stats: {Console.okColor table.TableName}"

    let private createPluginStatsRow (columns: DataColumn[]) (row: DataRow) =
        columns
        |> Seq.map(fun col ->
            [ col.GetColumnCaptionOrName() |> Console.escapeMarkup
              row[col].ToString() |> Console.escapeMarkup ]
        )

    let private createPluginStatsTable (table: DataTable)=
        let headers = ["key"; "value"]
        let createPluginStatsRow = table.GetColumns() |> createPluginStatsRow
        let rows =
            table.GetRows()
            |> Seq.mapi(fun i row ->
                [ if i > 0 then [String.Empty; String.Empty]
                  yield! createPluginStatsRow row ]
            )
            |> List.concat

        Console.addTable headers rows

    let printPluginStats (stats: NodeStats) =
        if stats.PluginStats.Length > 0 then
            let pluginStats =
                stats.PluginStats
                |> Seq.collect(fun dataSet -> dataSet.GetTables())
                |> Seq.map(fun table ->
                    [ printPluginStatsHeader table
                      Console.addLine String.Empty
                      createPluginStatsTable table
                      Console.addLine String.Empty ]
                )
                |> Seq.concat

            [ Console.addHeader "plugin stats"
              Console.addLine String.Empty
              yield! pluginStats ]
        else
            List.empty

module ConsoleHints =

    let private printHintsList (hints: HintResult[]) =
        hints
        |> Seq.map(fun hint -> seq {
            $"hint for {hint.SourceType} {hint.SourceName |> Console.escapeMarkup |> Console.okColor}:"
            $"{hint.Hint |> Console.escapeMarkup |> Console.warningColor}"
        })
        |> Console.addList

    let printHints (hints: HintResult[]) =
        if hints.Length > 0 then
            [ Console.addHeader "hints"
              Console.addLine String.Empty
              yield! printHintsList hints ]
        else
            List.empty

let print (logger: ILogger) (sessionResult: NodeSessionResult) (simulations: IDictionary<string, LoadSimulation list>) =
    try
        logger.Verbose("ConsoleReport.print")

        [ Console.addLine String.Empty
          yield! ConsoleTestInfo.printTestInfo sessionResult.FinalStats.TestInfo
          yield! ConsoleNodeStats.printNodeStats sessionResult.FinalStats simulations
          yield! ConsolePluginStats.printPluginStats sessionResult.FinalStats
          yield! ConsoleHints.printHints sessionResult.Hints
          Console.addLine String.Empty ]
    with
    | ex ->
        logger.Error(ex, "ConsoleReport.print failed")
        ["Could not generate report" |> Console.addLine]
