module internal NBomber.DomainServices.Reporting.MdReport

open System
open System.Data

open FuncyDown.Document

open NBomber.Contracts
open NBomber.Domain.HintsAnalyzer
open NBomber.Extensions

module Md =

    let printInlineCode (code: obj) =
       emptyDocument
       |> addInlineCode { Code = code.ToString(); Language = None }
       |> asString

    let printHeader (header) (document: Document) =
        document
        |> addBlockQuote header
        |> addNewline
        |> addNewline

    let printBold (text) =
        emptyDocument
        |> addStrongEmphasis text
        |> asString

module MdTestInfo =

    let printTestInfo (testInfo: TestInfo) (document: Document) =
        document
        |> Md.printHeader $"test suite: {testInfo.TestSuite |> Md.printInlineCode}"
        |> Md.printHeader $"test name: {testInfo.TestName |> Md.printInlineCode}"

module MdErrorStats =

    let printErrorStatsHeader (scenarioName: string) (document: Document) =
        document
        |> Md.printHeader $"errors for scenario: {scenarioName |> Md.printInlineCode}"

    let private createErrorStatsTableRows (errorStats: ErrorStats[])=
        errorStats
        |> Seq.map(fun error -> [error.ErrorCode.ToString(); error.Count.ToString(); error.Message])
        |> List.ofSeq

    let printErrorStats (errorStats: ErrorStats[]) (document: Document) =
        let headers = ["error code"; "count"; "message"]
        let rows = createErrorStatsTableRows(errorStats)

        document |> addTable headers rows

module MdNodeStats =

    let private printScenarioHeader (scnStats: ScenarioStats) (document: Document) =
        let header =
            $"scenario: {scnStats.ScenarioName |> Md.printInlineCode}" +
            $", duration: {scnStats.Duration |> Md.printInlineCode}" +
            $", ok count: {scnStats.OkCount |> Md.printInlineCode}" +
            $", fail count: {scnStats.FailCount |> Md.printInlineCode}" +
            $", all data: {scnStats.AllDataMB |> Md.printInlineCode} MB"

        document |> Md.printHeader header

    let private createStepStatsRow (i) (s: StepStats) =
        let name = s.StepName |> Md.printInlineCode
        let okReqCount = s.Ok.Request.Count
        let failReqCount = s.Fail.Request.Count
        let allReqCount = okReqCount + failReqCount
        let okRPS = s.Ok.Request.RPS
        let failRPS = s.Fail.Request.RPS
        let okLatency = s.Ok.Latency
        let failLatency = s.Fail.Latency
        let okDataTransfer = s.Ok.DataTransfer
        let failDataTransfer = s.Fail.DataTransfer

        let allCount = $"all = {allReqCount |> Md.printInlineCode}"

        let okCount =
            $"ok = {okReqCount |> Md.printInlineCode}" +
            $", RPS = {okRPS |> Md.printInlineCode}"

        let failCount =
            $"fail = {failReqCount |> Md.printInlineCode}" +
            $", RPS = {failRPS |> Md.printInlineCode}"

        let okTimes =
            $"min = {okLatency.MinMs |> Md.printInlineCode}" +
            $", mean = {okLatency.MeanMs |> Md.printInlineCode}" +
            $", max = {okLatency.MaxMs |> Md.printInlineCode}"

        let failTimes =
            $"min = {failLatency.MinMs |> Md.printInlineCode}" +
            $", mean = {failLatency.MeanMs |> Md.printInlineCode}" +
            $", max = {failLatency.MaxMs |> Md.printInlineCode}"

        let okPercentile =
            $"50%% = {okLatency.Percent50 |> Md.printInlineCode}" +
            $", 75%% = {okLatency.Percent75 |> Md.printInlineCode}" +
            $", 95%% = {okLatency.Percent95 |> Md.printInlineCode}" +
            $", 99%% = {okLatency.Percent99 |> Md.printInlineCode}" +
            $", StdDev = {okLatency.StdDev |> Md.printInlineCode}"

        let failPercentile =
            $"50%% = {failLatency.Percent50 |> Md.printInlineCode}" +
            $", 75%% = {failLatency.Percent75 |> Md.printInlineCode}" +
            $", 95%% = {failLatency.Percent95 |> Md.printInlineCode}" +
            $", 99%% = {failLatency.Percent99 |> Md.printInlineCode}" +
            $", StdDev = {failLatency.StdDev |> Md.printInlineCode}"

        let okDtMin = $"%.3f{okDataTransfer.MinKb} KB" |> Md.printInlineCode
        let okDtMean = $"%.3f{okDataTransfer.MeanKb} KB" |> Md.printInlineCode
        let okDtMax = $"%.3f{okDataTransfer.MaxKb} KB" |> Md.printInlineCode
        let okDtAll = $"%.3f{okDataTransfer.AllMB} MB" |> Md.printInlineCode

        let okDt =
            $"min = {okDtMin |> Md.printInlineCode}" +
            $", mean = {okDtMean |> Md.printInlineCode}" +
            $", max = {okDtMax |> Md.printInlineCode}" +
            $", all = {okDtAll |> Md.printInlineCode}"


        let failDtMin = $"%.3f{failDataTransfer.MinKb} KB" |> Md.printInlineCode
        let failDtMean = $"%.3f{failDataTransfer.MeanKb} KB" |> Md.printInlineCode
        let failDtMax = $"%.3f{failDataTransfer.MaxKb} KB" |> Md.printInlineCode
        let failDtAll = $"%.3f{failDataTransfer.AllMB} MB" |> Md.printInlineCode

        let failDt =
            $"min = {failDtMin |> Md.printInlineCode}" +
            $", mean = {failDtMean |> Md.printInlineCode}" +
            $", max = {failDtMax |> Md.printInlineCode}" +
            $", all = {failDtAll |> Md.printInlineCode}"

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
        |> List.ofSeq

    let private printScenarioErrorStats (scnStats: ScenarioStats) (document: Document) =
        if scnStats.ErrorStats.Length > 0 then
            document
            |> MdErrorStats.printErrorStatsHeader scnStats.ScenarioName
            |> MdErrorStats.printErrorStats scnStats.ErrorStats
        else document

    let private printScenarioStats (scnStats: ScenarioStats) (document: Document) =
        let headers = ["step"; "details"]
        let rows = createStepStatsTableRows(scnStats.StepStats)

        document
        |> printScenarioHeader scnStats
        |> addTable headers rows
        |> printScenarioErrorStats scnStats

    let printNodeStats (stats: NodeStats) (document: Document) =
        stats.ScenarioStats
        |> Seq.fold(fun document scnStats ->
            document |> printScenarioStats scnStats |> addNewline
        ) document

module MdPluginStats =

    let private printPluginStatsHeader (table: DataTable) (document: Document) =
        document
        |> Md.printHeader $"plugin stats: {table.TableName |> Md.printInlineCode}"

    let private createPluginStatsTableHeaders (table: DataTable) =
        table.GetColumns()
        |> Seq.map(fun col -> col.GetColumnCaptionOrName())
        |> Seq.toList

    let private createPluginStatsTableRows (table: DataTable) =
        let columns = table.GetColumns()

        table.GetRows()
        |> Seq.map(fun row -> columns |> Seq.map(fun col -> row.[col] |> string) |> List.ofSeq)
        |> List.ofSeq

    let private printPluginStatsTable (table: DataTable) (document: Document) =
        let headers = createPluginStatsTableHeaders(table)
        let rows = createPluginStatsTableRows(table)

        document
        |> printPluginStatsHeader table
        |> addTable headers rows

    let printPluginStats (stats: NodeStats) (document: Document) =
        stats.PluginStats
        |> Seq.collect(fun dataSet -> dataSet.GetTables())
        |> Seq.fold(fun document table ->
            document |> printPluginStatsTable table |> addNewline
        ) document

module MdHints =

    let private printHintsHeader (document: Document) =
        document
        |> Md.printHeader "hints:"

    let private createHintsTableRows (hints: HintResult list) =
        hints
        |> Seq.map(fun hint -> [hint.SourceType.ToString(); hint.SourceName; hint.Hint])
        |> List.ofSeq

    let printHints (hints: HintResult list) (document: Document) =
        if hints.Length > 0 then
            let headers = ["source"; "name"; "hint"]
            let rows = createHintsTableRows(hints)

            document
            |> printHintsHeader
            |> addTable headers rows
        else
            document

let print (stats: NodeStats) (hints: HintResult list) =
    emptyDocument
    |> MdTestInfo.printTestInfo stats.TestInfo
    |> MdNodeStats.printNodeStats stats
    |> MdPluginStats.printPluginStats stats
    |> MdHints.printHints hints
    |> asString
