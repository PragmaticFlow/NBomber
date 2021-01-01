module internal NBomber.DomainServices.Reporting.MdReport

open System
open System.Data

open NBomber.Contracts
open NBomber.Domain.HintsAnalyzer
open NBomber.Extensions
open NBomber.Extensions.InternalExtensions

module Md =

    let inline private createTableHeader (header) =
        String.Join("|", header |> Seq.map(fun x -> sprintf "__%s__" x))

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
        sprintf "`%A`" text

    let createBlockquote (text) =
        sprintf "> %s" text

module MdTestInfo =
    let printTestInfo (testInfo: TestInfo) =
        [testInfo.TestSuite |> Md.createInlineCode |> sprintf "test suite: %s" |> Md.createBlockquote
         Md.createBlockquote("")
         testInfo.TestName |> Md.createInlineCode |> sprintf "test name: %s" |> Md.createBlockquote
        ]
        |> String.concatLines
        |> String.appendNewLine

module MdErrorStats =

    let printScenarioErrorStatsHeader (scnStats: ScenarioStats) =
        scnStats.ScenarioName |> Md.createInlineCode |> sprintf "errors for scenario: %s" |> Md.createBlockquote

    let printScenarioErrorStats (errorStats: ErrorStats[]) =
        errorStats
        |> Seq.map(fun error -> [error.ErrorCode.ToString(); error.Count.ToString(); error.Message])
        |> Seq.append [["error code"; "count"; "message"]]
        |> Seq.toList

module MdNodeStats =

    let private printScenarioHeader (scnStats: ScenarioStats) =
        sprintf "scenario: %s, duration: %s, ok count: %s, fail count: %s, all data: %s MB"
            (scnStats.ScenarioName |> Md.createInlineCode)
            (scnStats.Duration |> Md.createInlineCode)
            (scnStats.OkCount |> Md.createInlineCode)
            (scnStats.FailCount |> Md.createInlineCode)
            (scnStats.AllDataMB |> Md.createInlineCode)
        |> Md.createBlockquote
        |> String.appendNewLine

    let private stepStatsColHeaders = ["step"; "details"]

    let private printStepStatsRows (s: StepStats) =
        let name = Md.createInlineCode(s.StepName)

        let count =
            sprintf "all = %s, ok = %s, failed = %s, RPS = %s"
                (s.RPS |> Md.createInlineCode)
                (s.RequestCount |> Md.createInlineCode)
                (s.OkCount |> Md.createInlineCode)
                (s.FailCount |> Md.createInlineCode)

        let times =
            sprintf "min = %s, mean = %s, max = %s"
                (s.Min |> Md.createInlineCode)
                (s.Mean |> Md.createInlineCode)
                (s.Max |> Md.createInlineCode)

        let percentile =
            sprintf "50%% = %s, 75%% = %s, 95%% = %s, 99%% = %s, StdDev = %s"
                (s.Percent50 |> Md.createInlineCode)
                (s.Percent75 |> Md.createInlineCode)
                (s.Percent95 |> Md.createInlineCode)
                (s.Percent99 |> Md.createInlineCode)
                (s.StdDev |> Md.createInlineCode)

        let dataTransfer =
            sprintf "min = %s, mean = %s, max = %s, all = %s"
                (s.MinDataKb |> sprintf "%.3f KB" |> Md.createInlineCode)
                (s.MeanDataKb |> sprintf "%.3f KB" |> Md.createInlineCode)
                (s.MaxDataKb |> sprintf "%.3f KB" |> Md.createInlineCode)
                (s.AllDataMB |> sprintf "%.3f MB" |> Md.createInlineCode)

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
        table.TableName |> Md.createInlineCode |> sprintf "plugin stats: %s" |> Md.createBlockquote

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
        |> Seq.collect(fun dataSet -> dataSet.GetTables())
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
