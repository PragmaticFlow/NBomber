module internal NBomber.DomainServices.Reporting.TxtReport

open System.Data

open ConsoleTables

open NBomber.Contracts
open NBomber.Extensions
open NBomber.Extensions.InternalExtensions

let private printTestInfo (testInfo: TestInfo) =
    [sprintf "test suite: '%s'" testInfo.TestSuite; sprintf "test name: '%s'" testInfo.TestName]
    |> String.concatLines

let private printScenarioHeader (scnStats: ScenarioStats) =
    sprintf "scenario: '%s', duration: '%A', ok count: '%A', fail count: '%A', all data: '%A' MB"
        scnStats.ScenarioName scnStats.Duration scnStats.OkCount scnStats.FailCount scnStats.AllDataMB

let inline private printPluginStatsHeader (table: DataTable) =
    sprintf "plugin stats: '%s'" table.TableName

let private printStepsTable (steps: StepStats[]) =
    let stepTable = ConsoleTable("step", "details")
    steps
    |> Seq.iteri(fun i s ->
        let dataInfoAvailable = s.AllDataMB > 0.0

        [ "- name", s.StepName
          "- request count", sprintf "all = %i | OK = %i | failed = %i" s.RequestCount s.OkCount s.FailCount
          "- latency", sprintf "RPS = %i | min = %i | mean = %i | max = %i" s.RPS s.Min s.Mean s.Max
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

let private printPluginStatsTable (table: DataTable) =
    let columnNames = table.GetColumns() |> Array.map(fun x -> x.ColumnName)
    let columnCaptions = table.GetColumns() |> Array.map(fun x -> x.GetColumnCaptionOrName())
    let consoleTable = ConsoleTable(columnCaptions)

    table.GetRows()
    |> Array.map(fun x -> columnNames |> Array.map(fun columnName -> x.[columnName]))
    |> Array.iter(fun x -> consoleTable.AddRow(x) |> ignore)

    consoleTable.ToStringAlternative()

let private printNodeStats (stats: NodeStats) =
    stats.ScenarioStats
    |> Array.map(fun scnStats ->
        [printScenarioHeader(scnStats)
         printStepsTable(scnStats.StepStats)]
        |> String.concatLines
    )
    |> String.concatLines

let private printPluginStats (stats: NodeStats) =
    stats.PluginStats
    |> Seq.collect(fun dataSet -> dataSet.GetTables())
    |> Seq.collect(fun table ->
        seq {
            yield table |> printPluginStatsHeader
            yield table |> printPluginStatsTable |> String.appendNewLine
        })
    |> String.concatLines

let print (testInfo: TestInfo, stats: NodeStats) =
    [printTestInfo(testInfo)
     printNodeStats(stats)
     printPluginStats(stats)]
    |> String.concatLines
