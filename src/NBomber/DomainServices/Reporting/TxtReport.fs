module internal NBomber.DomainServices.Reporting.TxtReport

open System
open System.Data

open ConsoleTables

open NBomber.Contracts
open NBomber.Domain.HintsAnalyzer
open NBomber.Extensions
open NBomber.Extensions.InternalExtensions

module TxtTestInfo =

    let printTestInfo (testInfo: TestInfo) =
        [$"test suite: '{testInfo.TestSuite}'"; $"test name: '{testInfo.TestName}'"]
        |> String.concatLines
        |> String.appendNewLine

module TxtErrorStats =

    let printScenarioErrorStatsHeader (scnStats: ScenarioStats) =
        $"errors for scenario: '{scnStats.ScenarioName}'"

    let printErrorStatsTable (errorStats: ErrorStats[]) =
        let errorTable = ConsoleTable("error code", "count", "message")
        errorStats |> Seq.iter(fun error -> errorTable.AddRow(error.ErrorCode, error.Count, error.Message) |> ignore)
        errorTable.ToStringAlternative()

module TxtNodeStats =

    let private printScenarioHeader (scnStats: ScenarioStats) =
        $"scenario: '{scnStats.ScenarioName}', duration: '{scnStats.Duration}'" +
        $", ok count: '{scnStats.OkCount}', fail count: '{scnStats.FailCount}', all data: '{scnStats.AllDataMB}' MB"

    let private createStepStatsRow (i) (s: StepStats) =
        let name = s.StepName
        let okReqCount = s.Ok.Request.Count
        let failReqCount = s.Fail.Request.Count
        let allReqCount = okReqCount + failReqCount
        let okRPS = s.Ok.Request.RPS
        let failRPS = s.Fail.Request.RPS
        let okLatency = s.Ok.Latency
        let failLatency = s.Fail.Latency
        let okDataTransfer = s.Ok.DataTransfer
        let failDataTransfer = s.Fail.DataTransfer

        let allCount = $"all = {allReqCount}"

        let okCount =
            $"ok = {okReqCount}" +
            $", RPS = {okRPS}"

        let failCount =
            $"fail = {failReqCount}" +
            $", RPS = {failRPS}"

        let okTimes =
            $"min = {okLatency.MinMs}" +
            $", mean = {okLatency.MeanMs}" +
            $", max = {okLatency.MaxMs}"

        let failTimes =
            $"min = {failLatency.MinMs}" +
            $", mean = {failLatency.MeanMs}" +
            $", max = {failLatency.MaxMs}"

        let okPercentile =
            $"50%% = {okLatency.Percent50}" +
            $", 75%% = {okLatency.Percent75}" +
            $", 95%% = {okLatency.Percent95}" +
            $", 99%% = {okLatency.Percent99}" +
            $", StdDev = {okLatency.StdDev}"

        let failPercentile =
            $"50%% = {failLatency.Percent50}" +
            $", 75%% = {failLatency.Percent75}" +
            $", 95%% = {failLatency.Percent95}" +
            $", 99%% = {failLatency.Percent99}" +
            $", StdDev = {failLatency.StdDev}"

        let okDtMin = $"%.3f{okDataTransfer.MinKb} KB"
        let okDtMean = $"%.3f{okDataTransfer.MeanKb} KB"
        let okDtMax = $"%.3f{okDataTransfer.MaxKb} KB"
        let okDtAll = $"%.3f{okDataTransfer.AllMB} MB"

        let okDt =
            $"min = {okDtMin}" +
            $", mean = {okDtMean}" +
            $", max = {okDtMax}" +
            $", all = {okDtAll}"

        let failDtMin = $"%.3f{failDataTransfer.MinKb} KB"
        let failDtMean = $"%.3f{failDataTransfer.MeanKb} KB"
        let failDtMax = $"%.3f{failDataTransfer.MaxKb} KB"
        let failDtAll = $"%.3f{failDataTransfer.AllMB} MB"

        let failDt =
            $"min = {failDtMin}" +
            $", mean = {failDtMean}" +
            $", max = {failDtMax}" +
            $", all = {failDtAll}"

        [ if i > 0 then [" "; " "]
          ["name"; name]
          ["all request count"; allCount]
          ["ok stats"; String.Empty]
          ["ok request count"; okCount]
          ["ok latency"; okTimes]
          ["ok latency percentile"; okPercentile]
          if okDataTransfer.AllMB > 0.0 then ["ok data transfer"; okDt]
          ["fail stats"; String.Empty]
          ["fail request count"; failCount]
          ["fail latency"; failTimes]
          ["fail latency percentile"; failPercentile]
          if failDataTransfer.AllMB > 0.0 then ["fail data transfer"; failDt] ]

    let private createStepStatsTableRows (stepStats: StepStats[]) =
        stepStats
        |> Seq.mapi createStepStatsRow
        |> Seq.concat

    let private printStepsTable (stepStats: StepStats[]) =
        let stepTable = ConsoleTable("step", "details")
        stepStats
        |> createStepStatsTableRows
        |> Seq.iter(fun row -> stepTable.AddRow(row.[0], row.[1]) |> ignore)

        stepTable.ToStringAlternative()

    let printNodeStats (stats: NodeStats) =
        stats.ScenarioStats
        |> Array.map(fun scnStats ->
            [printScenarioHeader(scnStats)
             printStepsTable(scnStats.StepStats)
             if scnStats.ErrorStats.Length > 0 then
                 TxtErrorStats.printScenarioErrorStatsHeader(scnStats)
                 TxtErrorStats.printErrorStatsTable(scnStats.ErrorStats)
            ]
            |> String.concatLines
        )
        |> String.concatLines

module TxtPluginStats =

    let private printPluginStatsHeader (table: DataTable) =
        $"plugin stats: '{table.TableName}'"

    let private printPluginStatsTable (table: DataTable) =
        let columnNames = table.GetColumns() |> Array.map(fun x -> x.ColumnName)
        let columnCaptions = table.GetColumns() |> Array.map(fun x -> x.GetColumnCaptionOrName())
        let consoleTable = ConsoleTable(columnCaptions)

        table.GetRows()
        |> Array.map(fun x -> columnNames |> Array.map(fun columnName -> x.[columnName]))
        |> Array.iter(fun x -> consoleTable.AddRow(x) |> ignore)

        consoleTable.ToStringAlternative()

    let printPluginStats (stats: NodeStats) =
        stats.PluginStats
        |> Seq.collect(fun dataSet -> dataSet.GetTables())
        |> Seq.collect(fun table ->
            seq {
                printPluginStatsHeader(table)
                printPluginStatsTable(table)
            })
        |> String.concatLines

module TxtHints =
    let private printHintsHeader () =
        "hints:"

    let private printHintsTable (hints: HintResult list) =
        let hintTable = ConsoleTable("source", "name", "hint")

        hints
        |> Seq.iter(fun hint -> hintTable.AddRow(hint.SourceType, hint.SourceName, hint.Hint) |> ignore)

        hintTable.ToStringAlternative()

    let printHints (hints: HintResult list) =
        if hints.Length > 0 then
            seq {
                yield printHintsHeader()
                yield printHintsTable(hints)
            }
            |> String.concatLines
        else
            String.Empty

let print (stats) (hints) =
    [TxtTestInfo.printTestInfo stats.TestInfo
     TxtNodeStats.printNodeStats stats
     TxtPluginStats.printPluginStats stats
     TxtHints.printHints hints]
    |> String.concatLines
