module NBomber.DomainServices.Reporting.ViewModels

open System.Data

open Newtonsoft.Json

open NBomber.Contracts
open NBomber.Extensions

type PluginStatsViewModel = {
    TableName: string
    Columns: string[]
    Rows: string[][]
}

type HtmlReportViewModel = {
    RequestCount: int
    OkCount: int
    FailCount: int
    AllDataMB: float
    ScenarioStats: ScenarioStats[]
    PluginStats: PluginStatsViewModel[]
    NodeInfo: NodeInfo
}

module HtmlReportViewModel =

    let private mapDataTableToPluginStatsViewModel (table: DataTable) =
        let tableName = table.TableName
        let columns = table.GetColumns() |> Array.map(fun col -> col.GetColumnCaptionOrName())
        let rows = table.GetRows() |> Array.map(fun row -> row.ItemArray |> Array.map(fun x -> x.ToString()))

        { TableName = tableName; Columns = columns; Rows = rows }

    let private mapToPluginStatsViewModel (pluginStats: DataSet[]) =
        pluginStats
        |> Array.collect(fun dataSet -> dataSet.GetTables())
        |> Array.map mapDataTableToPluginStatsViewModel

    let map (stats: NodeStats) =
        let viewModel: HtmlReportViewModel = {
            RequestCount = stats.RequestCount
            OkCount = stats.OkCount
            FailCount = stats.FailCount
            AllDataMB = stats.AllDataMB
            ScenarioStats = stats.ScenarioStats
            PluginStats = stats.PluginStats |> mapToPluginStatsViewModel
            NodeInfo = stats.NodeInfo
        }

        viewModel

    let serialize (viewModel: HtmlReportViewModel) =
        JsonConvert.SerializeObject(viewModel, Formatting.Indented)
