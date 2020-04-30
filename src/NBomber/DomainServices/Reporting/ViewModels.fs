module NBomber.DomainServices.Reporting.ViewModels

open System
open System.Data

open Newtonsoft.Json

open NBomber.Contracts
open NBomber.Extensions

type PluginStatsViewModel = {
    TableName: string
    Columns: string[]
    Rows: string[][]
}

type NodeStatsViewModel = {
    RequestCount: int
    OkCount: int
    FailCount: int
    AllDataMB: float
    ScenarioStats: ScenarioStats[]
    PluginStats: PluginStatsViewModel[]
    NodeInfo: NodeInfo
}

type TimeLineStatsViewModel = {
    TimeStamps: string[]
    ScenarioStats: ScenarioStats[][]
}

module NodeStatsViewModel =

    let private mapDataTableToPluginStatsViewModel (table: DataTable) =
        let tableName = table.TableName
        let columns = table.GetColumns() |> Array.map(fun col -> col.GetColumnCaptionOrName())
        let rows = table.GetRows() |> Array.map(fun row -> row.ItemArray |> Array.map(fun x -> x.ToString()))

        { TableName = tableName; Columns = columns; Rows = rows }

    let private mapToPluginStatsViewModel (pluginStats: DataSet[]) =
        pluginStats
        |> Array.collect(fun dataSet -> dataSet.GetTables())
        |> Array.map mapDataTableToPluginStatsViewModel

    let create (stats: NodeStats): NodeStatsViewModel = {
        RequestCount = stats.RequestCount
        OkCount = stats.OkCount
        FailCount = stats.FailCount
        AllDataMB = stats.AllDataMB
        ScenarioStats = stats.ScenarioStats
        PluginStats = stats.PluginStats |> mapToPluginStatsViewModel
        NodeInfo = stats.NodeInfo
    }

    let serializeJson (viewModel: NodeStatsViewModel) =
        JsonConvert.SerializeObject(viewModel, Formatting.Indented)

module TimeLineStatsViewModel =

    let create (timeLineStats: (TimeSpan * NodeStats) list) = {
        TimeStamps = timeLineStats |> List.toArray |> Array.map(fun (timeSpan, _) -> TimeSpan(0, 0, (int)timeSpan.TotalSeconds).ToString())
        ScenarioStats = timeLineStats |> List.toArray |> Array.map(fun (_, nodeStats) -> nodeStats.ScenarioStats)
    }

    let serializeJson (viewModel: TimeLineStatsViewModel) =
        JsonConvert.SerializeObject(viewModel, Formatting.Indented)
