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
        [sprintf "test suite: '%s'" testInfo.TestSuite; sprintf "test name: '%s'" testInfo.TestName]
        |> String.concatLines
        |> String.appendNewLine

module TxtErrorStats =

    let printScenarioErrorStatsHeader (scnStats: ScenarioStats) =
        sprintf "errors for scenario: '%s'" scnStats.ScenarioName

    let printErrorStatsTable (errorStats: ErrorStats[]) =
        let errorTable = ConsoleTable("error code", "count", "message")
        errorStats |> Seq.iter(fun error -> errorTable.AddRow(error.ErrorCode, error.Count, error.Message) |> ignore)
        errorTable.ToStringAlternative()

module TxtNodeStats =

    let private printScenarioHeader (scnStats: ScenarioStats) =
        sprintf "scenario: '%s', duration: '%A', ok count: '%A', fail count: '%A', all data: '%A' MB"
            scnStats.ScenarioName scnStats.Duration scnStats.OkCount scnStats.FailCount scnStats.AllDataMB

    let private printStepsTable (steps: StepStats[]) =
        let stepTable = ConsoleTable("step", "details")
        steps
        |> Seq.iteri(fun i s ->
            let dataInfoAvailable = s.AllDataMB > 0.0

            [ "- name", s.StepName
              "- request count", sprintf "all = %i | OK = %i | failed = %i | RPS = %i" s.RPS s.RequestCount s.OkCount s.FailCount
              "- latency", sprintf "min = %i | mean = %i | max = %i" s.Min s.Mean s.Max
              "- latency percentile", sprintf "50%% = %i | 75%% = %i | 95%% = %i | 99%% = %i | StdDev = %i" s.Percent50 s.Percent75 s.Percent95 s.Percent99 s.StdDev

              if dataInfoAvailable then
                "- data transfer", sprintf "min = %g KB | mean = %g KB | max = %g KB | all = %g MB"
                                       s.MinDataKb s.MeanDataKb s.MaxDataKb s.AllDataMB
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
                 TxtErrorStats.printScenarioErrorStatsHeader(scnStats)
                 TxtErrorStats.printErrorStatsTable(scnStats.ErrorStats)
            ]
            |> String.concatLines
        )
        |> String.concatLines

module TxtPluginStats =

    let private printPluginStatsHeader (table: DataTable) =
        sprintf "plugin stats: '%s'" table.TableName

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
