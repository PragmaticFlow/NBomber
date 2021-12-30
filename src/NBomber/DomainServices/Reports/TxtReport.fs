module internal NBomber.DomainServices.Reports.TxtReport

open System
open System.Collections.Generic
open System.Data

open ConsoleTables
open Serilog

open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Extensions
open NBomber.Extensions.InternalExtensions
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

    let private createTableRows = ReportHelper.StatusCodesStats.createTableRows None None

    let printScenarioHeader (scenarioName: string) =
        $"status codes for scenario: {scenarioName}"

    let printStatusCodeTable (scnStats: ScenarioStats) =
        let rows = createTableRows scnStats
        let table = ConsoleTable("status code", "count", "message")
        rows |> Seq.iter(fun x -> table.AddRow(x[0], x[1], x[2]) |> ignore)
        table.ToStringAlternative()

module TxtLoadSimulations =

    let private printLoadSimulation =
        let okColor = None
        ReportHelper.LoadSimulation.print okColor

    let print (simulations: LoadSimulation list) =
        let simulationsList = simulations |> List.map(printLoadSimulation) |> String.concatLines
        $"load simulations: {Environment.NewLine}{simulationsList}"

module TxtNodeStats =

    let private printDataKb (bytes: int) =
        $"{bytes |> Statistics.Converter.fromBytesToKb} KB"

    let private printAllData (bytes: int64) =
        $"{bytes |> Statistics.Converter.fromBytesToMb} MB"

    let private printScenarioHeader (scnStats: ScenarioStats) =
        $"scenario: {scnStats.ScenarioName}{Environment.NewLine}"
        + $"  - ok count: {scnStats.OkCount}{Environment.NewLine}"
        + $"  - fail count: {scnStats.FailCount}{Environment.NewLine}"
        + $"  - all data: {printAllData scnStats.AllBytes}{Environment.NewLine}"
        + $"  - duration: {scnStats.Duration}"

    let private printStepStatsHeader (stepStats: StepStats[]) =
        let print (stats) =
            $"step: {stats.StepName}{Environment.NewLine}"
            + $"  - timeout: {stats.StepInfo.Timeout.Milliseconds} ms{Environment.NewLine}"
            + $"  - client factory: {stats.StepInfo.ClientFactoryName}, clients: {stats.StepInfo.ClientFactoryClientCount}{Environment.NewLine}"

        stepStats |> Seq.map print |> String.concatLines

    let private printStepStatsRow (isOkStats: bool) (stepIndex: int) (stats: StepStats) =
        let allReqCount = Statistics.StepStats.getAllRequestCount stats
        let data = if isOkStats then stats.Ok else stats.Fail

        let reqCount =
            if isOkStats then $"all = {allReqCount}, ok = {data.Request.Count}, RPS = {data.Request.RPS}"
            else $"all = {allReqCount}, fail = {data.Request.Count}, RPS = {data.Request.RPS}"

        let latencies =
            $"min = {data.Latency.MinMs}" +
            $", mean = {data.Latency.MeanMs}" +
            $", max = {data.Latency.MaxMs}" +
            $", StdDev = {data.Latency.StdDev}"

        let percentiles =
            $"50%% = {data.Latency.Percent50}" +
            $", 75%% = {data.Latency.Percent75}" +
            $", 95%% = {data.Latency.Percent95}" +
            $", 99%% = {data.Latency.Percent99}"

        let dataTransfer =
            $"min = {printDataKb data.DataTransfer.MinBytes}" +
            $", mean = {printDataKb data.DataTransfer.MeanBytes}" +
            $", max = {printDataKb data.DataTransfer.MaxBytes}" +
            $", all = {printAllData data.DataTransfer.AllBytes}"

        [ if stepIndex > 0 then [String.Empty; String.Empty]
          ["name"; stats.StepName]
          ["request count"; reqCount]
          ["latency"; latencies]
          ["latency percentile"; percentiles]
          if data.DataTransfer.AllBytes > 0 then ["data transfer"; dataTransfer] ]

    let private printStepStatsTable (isOkStats: bool) (stepStats: StepStats[]) =
        let table =
            if isOkStats then ConsoleTable("step", "ok stats")
            else ConsoleTable("step", "fail stats")

        stepStats
        |> Seq.mapi(printStepStatsRow isOkStats)
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
