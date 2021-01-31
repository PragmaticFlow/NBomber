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
            ]
            |> String.concatLines
            |> String.appendNewLine
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
            let dataInfoAvailable = s.Ok.DataTransfer.AllMB + s.Fail.DataTransfer.AllMB > 0.0
            let okCount = s.Ok.Request.Count
            let failCount = s.Fail.Request.Count
            let reqCount = okCount + failCount
            let okRPS = s.Ok.Request.RPS
            let lt = s.Ok.Latency
            let dt = s.Ok.DataTransfer

            [ "- name", s.StepName
              "- request count", $"all = {reqCount} | OK = {okCount} | failed = {failCount} | RPS = {okRPS}"
              "- latency", $"min = {lt.MinMs} | mean = {lt.MeanMs} | max = {lt.MaxMs}"
              "- latency percentile", $"50%% = {lt.Percent50} | 75%% = {lt.Percent75} | 95%% = {lt.Percent95} | 99%% = {lt.Percent99} | StdDev = {lt.StdDev}"

              if dataInfoAvailable then
                "- data transfer", $"min = %g{dt.MinKb} KB | mean = %g{dt.MeanKb} KB | max = %g{dt.MaxKb} KB | all = %g{dt.AllMB} MB"
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
            ]
            |> String.concatLines
            |> String.appendNewLine
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
