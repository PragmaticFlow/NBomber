module internal NBomber.DomainServices.Reporting.MdReport

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

module MdTestInfo =

    let printTestInfo (testInfo: TestInfo) (document: Document) =
        document
        |> Md.printHeader $"test suite: {testInfo.TestSuite |> Md.printInlineCode}"
        |> Md.printHeader $"test name: {testInfo.TestName |> Md.printInlineCode}"

module MdErrorStats =

    let printErrorStatsHeader (scnStats: ScenarioStats) (document: Document) =
        document
        |> Md.printHeader $"errors for scenario: {scnStats.ScenarioName |> Md.printInlineCode}"

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

    let private createStepStatsRow (s: StepStats) =
        let name = s.StepName |> Md.printInlineCode

        let count =
            $"all = {s.RequestCount |> Md.printInlineCode}" +
            $", ok = {s.OkCount |> Md.printInlineCode}" +
            $", failed = {s.FailCount |> Md.printInlineCode}" +
            $", RPS = {s.RPS |> Md.printInlineCode}"

        let times =
            $"min = {s.Min |> Md.printInlineCode}" +
            $", mean = {s.Mean |> Md.printInlineCode}" +
            $", max = {s.Max |> Md.printInlineCode}"

        let percentile =
            $"50%% = {s.Percent50 |> Md.printInlineCode}" +
            $", 75%% = {s.Percent75 |> Md.printInlineCode}" +
            $", 95%% = {s.Percent95 |> Md.printInlineCode}" +
            $", 99%% = {s.Percent99 |> Md.printInlineCode}" +
            $", StdDev = {s.StdDev |> Md.printInlineCode}"

        let min = $"%.3f{s.MinDataKb} KB" |> Md.printInlineCode
        let mean = $"%.3f{s.MeanDataKb} KB" |> Md.printInlineCode
        let max = $"%.3f{s.MaxDataKb} KB" |> Md.printInlineCode
        let all = $"%.3f{s.AllDataMB} MB" |> Md.printInlineCode

        let dataTransfer =
            $"min = {min |> Md.printInlineCode}" +
            $", mean = {mean |> Md.printInlineCode}" +
            $", max = {max |> Md.printInlineCode}" +
            $", all = {all |> Md.printInlineCode}"

        [ ["name"; name]
          ["request count"; count]
          ["latency"; times]
          ["latency percentile"; percentile]
          if s.AllDataMB > 0.0 then ["data transfer"; dataTransfer] ]

    let private createStepStatsTableRows (stepStats: StepStats[]) =
        stepStats
        |> Seq.collect createStepStatsRow
        |> List.ofSeq

    let private printScenarioErrorStats (scnStats: ScenarioStats) (document: Document) =
        if scnStats.ErrorStats.Length > 0 then
            document
            |> MdErrorStats.printErrorStatsHeader scnStats
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
