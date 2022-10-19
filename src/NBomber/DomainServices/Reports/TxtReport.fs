module internal NBomber.DomainServices.Reports.TxtReport

open System
open System.Collections.Generic
open System.Data

open ConsoleTables
open Serilog

open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Extensions.Data
open NBomber.Extensions.Internal
open NBomber.Domain.Stats

module TxtTestInfo =

    let printTestInfo (testInfo: TestInfo) =
        ["test info"
         $"test suite: {testInfo.TestSuite}"
         $"test name: {testInfo.TestName}"
         $"session id: {testInfo.SessionId}"]
        |> String.concatLines
        |> String.appendNewLine

module TxtStatusCodeStats =

    let printScenarioHeader (scenarioName: string) =
        $"status codes for scenario: {scenarioName}"

    let printStatusCodeTable (scnStats: ScenarioStats) =
        let createTableRows = ReportHelper.StatusCodesStats.createTableRows string string
        let rows = createTableRows scnStats
        let table = ConsoleTable("status code", "count", "message")
        rows |> Seq.iter(fun x -> table.AddRow(x[0], x[1], x[2]) |> ignore)
        table.ToStringAlternative()

module TxtLoadSimulations =

    let print (simulations: LoadSimulation list) =
        let printLoadSimulation = ReportHelper.LoadSimulation.print string
        let simulationsList = simulations |> List.map(printLoadSimulation) |> String.concatLines
        $"load simulations: {Environment.NewLine}{simulationsList}"

module TxtNodeStats =

    let private printScenarioHeader (scnStats: ScenarioStats) =
        $"scenario: {scnStats.ScenarioName}{Environment.NewLine}"
        + $"  - ok count: {scnStats.OkCount}{Environment.NewLine}"
        + $"  - fail count: {scnStats.FailCount}{Environment.NewLine}"
        + $"  - all data: {ReportHelper.printAllData string scnStats.AllBytes}{Environment.NewLine}"
        + $"  - duration: {scnStats.Duration}"

    let private printStepStatsHeader (stepStats: StepStats[]) =
        let print (stats) =
            $"step: {stats.StepName}{Environment.NewLine}"
            + $"  - timeout: {stats.StepInfo.Timeout.TotalMilliseconds} ms{Environment.NewLine}"

        stepStats |> Seq.map print |> String.concatLines

    let private printStepStatsTable (isOkStats: bool) (stepStats: StepStats[]) =
        let printStepStatsRow = ReportHelper.StepStats.printStepStatsRow isOkStats string string string

        let table =
            if isOkStats then ConsoleTable("step", "ok stats")
            else ConsoleTable("step", "fail stats")

        stepStats
        |> Seq.mapi printStepStatsRow
        |> Seq.concat
        |> Seq.iter(fun row -> table.AddRow(row[0], row[1]) |> ignore)

        table.ToStringAlternative()

    let private printScenarioStatusCodes (scnStats: ScenarioStats) =
        [ TxtStatusCodeStats.printScenarioHeader scnStats.ScenarioName
          TxtStatusCodeStats.printStatusCodeTable scnStats ]

    let private printScenarioStats (scnStats: ScenarioStats) (simulations: LoadSimulation list) =
        [ scnStats           |> printScenarioHeader      |> String.appendNewLine
          simulations        |> TxtLoadSimulations.print |> String.appendNewLine
          scnStats.StepStats |> printStepStatsHeader

          printStepStatsTable true scnStats.StepStats

          if Statistics.ScenarioStats.failStepStatsExist scnStats then
              printStepStatsTable false scnStats.StepStats

          if scnStats.StatusCodes.Length > 0 then
              yield! printScenarioStatusCodes scnStats ]

    let printNodeStats (stats: NodeStats) (loadSimulations: IDictionary<string, LoadSimulation list>) =
        stats.ScenarioStats
        |> Seq.map(fun scnStats -> printScenarioStats scnStats loadSimulations[scnStats.ScenarioName])
        |> Seq.concat
        |> String.concatLines

module TxtPluginStats =

    let private printPluginStatsHeader (table: DataTable) =
        $"plugin stats: '{table.TableName}'"

    let private printPluginStatsTable (table: DataTable) =
        let columnNames = table.GetColumns() |> Array.map(fun x -> x.ColumnName)
        let columnCaptions = table.GetColumns() |> Array.map(fun x -> x.GetColumnCaptionOrName())
        let consoleTable = ConsoleTable(columnCaptions)

        table.GetRows()
        |> Seq.map(fun x -> columnNames |> Array.map(fun columnName -> x[columnName]))
        |> Seq.iter(fun x -> consoleTable.AddRow(x) |> ignore)

        consoleTable.ToStringAlternative()

    let printPluginStats (stats: NodeStats) =
        stats.PluginStats
        |> Seq.collect(fun dataSet -> dataSet.GetTables())
        |> Seq.collect(fun table -> seq {
            printPluginStatsHeader table
            printPluginStatsTable table
        })
        |> String.concatLines

module TxtHints =
    let private printHintsHeader () =
        "hints:"

    let private printHintsTable (hints: HintResult[]) =
        let hintTable = ConsoleTable("source", "name", "hint")

        hints
        |> Seq.iter(fun hint -> hintTable.AddRow(hint.SourceType, hint.SourceName, hint.Hint) |> ignore)

        hintTable.ToStringAlternative()

    let printHints (hints: HintResult[]) =
        if hints.Length > 0 then
            seq {
                yield printHintsHeader()
                yield printHintsTable hints
            }
            |> String.concatLines
        else
            String.Empty

let print (logger: ILogger) (sessionResult: NodeSessionResult) (simulations: IDictionary<string, LoadSimulation list>) =
    try
        logger.Verbose("TxtReport.print")

        [ TxtTestInfo.printTestInfo sessionResult.FinalStats.TestInfo
          TxtNodeStats.printNodeStats sessionResult.FinalStats simulations
          TxtPluginStats.printPluginStats sessionResult.FinalStats
          TxtHints.printHints sessionResult.Hints ]
        |> String.concatLines
    with
    | ex ->
        logger.Error(ex, "TxtReport.print failed")
        "Could not generate report"
