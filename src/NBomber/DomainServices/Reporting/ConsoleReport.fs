module internal NBomber.DomainServices.Reporting.ConsoleReport

open System
open System.Data

open ConsoleTables

open NBomber.Contracts
open NBomber.Domain.HintsAnalyzer
open NBomber.Extensions
open NBomber.Extensions.InternalExtensions

module ConsoleTestInfo =

    let printTestInfo (testInfo: TestInfo) =
        [$"test suite: '{testInfo.TestSuite}'"; $"test name: '{testInfo.TestName}'"]
        |> String.concatLines
        |> String.appendNewLine

module ConsoleErrorStats =

    let printScenarioErrorStatsHeader (scnStats: ScenarioStats) =
        $"errors for scenario: '{scnStats.ScenarioName}'"

    let printErrorStats (errorStats: ErrorStats[]) =
        errorStats
        |> Seq.map(fun error ->
            [ $"- error code: ${error.ErrorCode}"
              $"- count: ${error.Count}"
              $"- message: ${error.Message}"
              String.Empty
            ]
            |> String.concatLines
        )
        |> String.concatLines

module ConsoleNodeStats =

    let private printScenarioHeader (scnStats: ScenarioStats) =
        $"scenario: '{scnStats.ScenarioName}', duration: '{scnStats.Duration}'" +
        $", ok count: '{scnStats.OkCount}', fail count: '{scnStats.FailCount}', all data: '{scnStats.AllDataMB}' MB"

    let private printStepsTable (steps: StepStats[]) =
        let stepTable = ConsoleTable("step", "details")
        steps
        |> Seq.iteri(fun i s ->
            let dataInfoAvailable = s.AllDataMB > 0.0

            [ "- name", s.StepName
              "- request count", $"all = {s.RequestCount} | OK = {s.OkCount} | failed = {s.FailCount} | RPS = {s.RPS}"
              "- latency", $"min = {s.Min} | mean = {s.Mean} | max = {s.Max}"
              "- latency percentile", $"50%% = {s.Percent50} | 75%% = {s.Percent75} | 95%% = {s.Percent95} | 99%% = {s.Percent99} | StdDev = {s.StdDev}"

              if dataInfoAvailable then
                "- data transfer", $"min = %g{s.MinDataKb} KB | mean = %g{s.MeanDataKb} KB | max = %g{s.MaxDataKb} KB | all = %g{s.AllDataMB} MB"
              if steps.Length > 1 && i < (steps.Length - 1) then
                "", ""
            ]
            |> Seq.iter(fun (name, value) -> stepTable.AddRow(name, value) |> ignore)
        )
        stepTable.ToStringAlternative()

    let printNodeStats (stats: NodeStats) =
        stats.ScenarioStats
        |> Array.map(fun scnStats ->
            [printScenarioHeader(scnStats)
             printStepsTable(scnStats.StepStats)
             if scnStats.ErrorStats.Length > 0 then
                 ConsoleErrorStats.printScenarioErrorStatsHeader(scnStats)
                 ConsoleErrorStats.printErrorStats(scnStats.ErrorStats)
            ]
            |> String.concatLines
        )
        |> String.concatLines

module ConsolePluginStats =

    let private printPluginStatsHeader (table: DataTable) =
        $"plugin stats: '{table.TableName}'" |> String.appendNewLine

    let private printPluginStatsList (table: DataTable) =
        let columns = table.GetColumns()

        table.GetRows()
        |> Seq.map(fun row ->
            columns
            |> Seq.map(fun col -> $"- {col.GetColumnCaptionOrName()}: {row.[col.ColumnName]}")
            |> String.concatLines
            |> String.appendNewLine
        )
        |> String.concatLines


    let printPluginStats (stats: NodeStats) =
        stats.PluginStats
        |> Seq.collect(fun dataSet -> dataSet.GetTables())
        |> Seq.collect(fun table ->
            seq {
                printPluginStatsHeader(table)
                printPluginStatsList(table)
                String.Empty
            })
        |> String.concatLines

module ConsoleHints =
    let private printHintsHeader () =
        "hints:" |> String.appendNewLine

    let private printHintsList (hints: HintResult list) =
        hints
        |> Seq.map(fun hint ->
            [ $"- source: {hint.SourceType}"
              $"- name: {hint.SourceName}"
              $"- hint: {hint.Hint}"
              String.Empty
            ]
            |> String.concatLines
        )
        |> String.concatLines

    let printHints (hints: HintResult list) =
        if hints.Length > 0 then
            seq {
                yield printHintsHeader()
                yield printHintsList(hints)
                yield String.Empty
            }
            |> String.concatLines
        else
            String.Empty

let print (stats) (hints) =
    [ConsoleTestInfo.printTestInfo stats.TestInfo
     ConsoleNodeStats.printNodeStats stats
     ConsolePluginStats.printPluginStats stats
     ConsoleHints.printHints hints]
    |> String.concatLines
