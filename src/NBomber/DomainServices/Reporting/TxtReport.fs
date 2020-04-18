module internal NBomber.DomainServices.Reporting.TxtReport

open System
open System.Data

open ConsoleTables

open NBomber.Contracts
open NBomber.Extensions

let inline private concatLines s =
    String.concat Environment.NewLine s

let private printScenarioHeader (scnStats: ScenarioStats) =
    sprintf "scenario: '%s', duration: '%A'"
        scnStats.ScenarioName scnStats.Duration

let inline private printPluginStatsHeader (table: DataTable) =
    sprintf "statistics: '%s'" table.TableName

let private printStepsTable (steps: StepStats[]) =
    let stepTable = ConsoleTable("step", "details")
    steps
    |> Seq.iteri(fun i s ->
        let dataInfoAvailable = s.AllDataMB > 0.0

        [ "- name", s.StepName
          "- request count", sprintf "all = %i | OK = %i | failed = %i" s.RequestCount s.OkCount s.FailCount
          "- response time", sprintf "RPS = %i | min = %i | mean = %i | max = %i" s.RPS s.Min s.Mean s.Max
          "- response time percentile", sprintf "50%% = %i | 75%% = %i | 95%% = %i | StdDev = %i" s.Percent50 s.Percent75 s.Percent95 s.StdDev

          if dataInfoAvailable then
            "- data transfer", sprintf "min = %gKb | mean = %gKb | max = %gKb | all = %gMB"
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

let printNodeStats (stats: NodeStats) =
    stats.ScenarioStats
    |> Array.map(fun scnStats ->
        [printScenarioHeader scnStats; printStepsTable scnStats.StepStats]
        |> concatLines
    )
    |> concatLines

let printPluginStats (table: DataTable) =
    [table |> printPluginStatsHeader
     table |> printPluginStatsTable]
    |> concatLines

let print (stats: NodeStats) =
    let pluginsStats =
        stats.PluginStats
        |> Seq.collect(fun x -> x.GetTables())
        |> Seq.map(fun x -> x |> printPluginStats)
        |> Seq.toList

    printNodeStats(stats) :: pluginsStats
