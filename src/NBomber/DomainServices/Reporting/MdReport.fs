module internal NBomber.DomainServices.Reporting.MdReport

open System
open System.Data

open NBomber.Contracts
open NBomber.Domain.HintsAnalyzer
open NBomber.DomainServices
open NBomber.Extensions
open NBomber.Extensions.InternalExtensions

module Md =

    let inline private createTableHeader (header) =
        String.Join("|", header |> Seq.map(fun x -> $"__{x}__"))

    let inline private createTableSeparator (header: string list) =
        String.Join("|", Array.create header.Length "---")

    let private createTableRows (rows: string list list) =
        rows |> Seq.map(fun row -> String.Join("|", row)) |> String.concatLines

    let createTable (headerWithRows: string list list) =
        match headerWithRows with
        | header :: rows ->
            [createTableHeader(header); createTableSeparator(header); createTableRows(rows)]
            |> String.concatLines

        | [] -> String.Empty

    let createInlineCode (text: obj) =
        $"`{text}`"

    let createBlockquote (text) =
        $"> {text}"

module MdTestInfo =

    let printTestInfo (testInfo: TestInfo) =
        [$"test suite: {testInfo.TestSuite |> Md.createInlineCode}" |> Md.createBlockquote
         Md.createBlockquote("")
         $"test name: {testInfo.TestName |> Md.createInlineCode}" |> Md.createBlockquote
        ]
        |> String.concatLines
        |> String.appendNewLine

module MdErrorStats =

    let printScenarioErrorStatsHeader (scnStats: ScenarioStats) =
        $"errors for scenario: {scnStats.ScenarioName |> Md.createInlineCode}" |> Md.createBlockquote

    let printScenarioErrorStats (errorStats: ErrorStats[]) =
        errorStats
        |> Seq.map(fun error -> [error.ErrorCode.ToString(); error.Count.ToString(); error.Message])
        |> Seq.append [["error code"; "count"; "message"]]
        |> Seq.toList

module MdNodeStats =

    let private printScenarioHeader (scnStats: ScenarioStats) =
        ($"scenario: {scnStats.ScenarioName |> Md.createInlineCode}" +
         $", duration: {scnStats.Duration |> Md.createInlineCode}" +
         $", ok count: {scnStats.OkCount |> Md.createInlineCode}" +
         $", fail count: {scnStats.FailCount |> Md.createInlineCode}" +
         $", all data: {scnStats.AllDataMB |> Md.createInlineCode} MB"
        )
        |> Md.createBlockquote
        |> String.appendNewLine

    let private stepStatsColHeaders = ["step"; "details"]

    let private printStepStatsRows (s: StepStats) =
        let name = Md.createInlineCode(s.StepName)

        let count =
            $"all = {s.RequestCount |> Md.createInlineCode}" +
            $", ok = {s.OkCount |> Md.createInlineCode}" +
            $", failed = {s.FailCount |> Md.createInlineCode}" +
            $", RPS = {s.RPS |> Md.createInlineCode}"

        let times = $"min = {s.Min |> Md.createInlineCode}, mean = {s.Mean |> Md.createInlineCode}, max = {s.Max |> Md.createInlineCode}"

        let percentile =
            $"50%% = {s.Percent50 |> Md.createInlineCode}" +
            $", 75%% = {s.Percent75 |> Md.createInlineCode}" +
            $", 95%% = {s.Percent95 |> Md.createInlineCode}" +
            $", 99%% = {s.Percent99 |> Md.createInlineCode}" +
            $", StdDev = {s.StdDev |> Md.createInlineCode}"

        let min = $"%.3f{s.MinDataKb} KB" |> Md.createInlineCode
        let mean = $"%.3f{s.MeanDataKb} KB" |> Md.createInlineCode
        let max = $"%.3f{s.MaxDataKb} KB" |> Md.createInlineCode
        let all = $"%.3f{s.AllDataMB} MB" |> Md.createInlineCode

        let dataTransfer = $"min = {min}, mean = {mean}, max = {max}, all = {all}"

        [ ["name"; name]
          ["request count"; count]
          ["latency"; times]
          ["latency percentile"; percentile]
          if s.AllDataMB > 0.0 then ["data transfer"; dataTransfer] ]

    let printNodeStats (stats: NodeStats) =
        stats.ScenarioStats
        |> Seq.collect(fun scnStats ->
            seq {
                    printScenarioHeader(scnStats)

                    scnStats.StepStats
                    |> Seq.collect printStepStatsRows
                    |> Seq.append [stepStatsColHeaders]
                    |> Seq.toList
                    |> Md.createTable
                    |> String.appendNewLine

                    if scnStats.ErrorStats.Length > 0 then
                        MdErrorStats.printScenarioErrorStatsHeader(scnStats)
                        |> String.appendNewLine

                        MdErrorStats.printScenarioErrorStats(scnStats.ErrorStats)
                        |> Md.createTable
                        |> String.appendNewLine
            }
        )
        |> String.concatLines

module MdPluginStats =

    let inline private printPluginStatsHeader (table: DataTable) =
        $"plugin stats: {table.TableName |> Md.createInlineCode}" |> Md.createBlockquote

    let private printPluginStatsTableHeader (table: DataTable) =
        table.GetColumns()
        |> Seq.map(fun col -> col.GetColumnCaptionOrName())
        |> Seq.toList

    let private printPluginStatsTableRows (table: DataTable) =
        let columns = table.GetColumns()

        table.GetRows()
        |> Seq.map(fun row -> columns |> Seq.map(fun col -> row.[col] |> string) |> List.ofSeq)
        |> List.ofSeq

    let printPluginStats (stats: NodeStats) =
        stats.PluginStats
        |> PluginStats.getStatsTables
        |> Seq.collect(fun table ->
            seq {
                table |> printPluginStatsHeader |> String.appendNewLine

                table
                |> printPluginStatsTableRows
                |> List.append [printPluginStatsTableHeader(table)]
                |> Md.createTable
                |> String.appendNewLine
            }
        )
        |> String.concatLines

module MdHints =
    let private printHintsHeader () =
        Md.createBlockquote("hints:")

    let private printHintsTableHeader () =
        ["source"; "name"; "hint"]

    let private printHintsTableRows (hint: HintResult) =
        [hint.SourceType.ToString(); hint.SourceName; hint.Hint]

    let printHints (hints: HintResult list) =
        if hints.Length > 0 then
            seq {
                printHintsHeader() |> String.appendNewLine

                hints
                |> List.map printHintsTableRows
                |> List.append [printHintsTableHeader()]
                |> Md.createTable
                |> String.appendNewLine
            }
            |> String.concatLines
        else
            String.Empty

let print (stats) (hints) =
    [MdTestInfo.printTestInfo stats.TestInfo
     MdNodeStats.printNodeStats stats
     MdPluginStats.printPluginStats stats
     MdHints.printHints hints]
    |> String.concatLines
