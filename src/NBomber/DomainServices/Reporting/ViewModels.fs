module internal NBomber.DomainServices.Reporting.ViewModels

open System
open System.Data

open NBomber.Contracts
open NBomber.Extensions

type NBomberInfoViewModel = {
    NBomberVersion: string
}

type TestInfoViewModel = {
    TestSuite: string
    TestName: string
}

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

module NBomberInfoViewModel =

    let create (nodeInfo: NodeInfo) = {
        NBomberVersion = nodeInfo.NBomberVersion
    }

module TestInfoViewModel =

    let create (testInfo: TestInfo): TestInfoViewModel = {
        TestSuite = testInfo.TestSuite
        TestName = testInfo.TestName
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

module TimeLineStatsViewModel =

    let create (timeLineStats: (TimeSpan * NodeStats) list) = {
        TimeStamps = timeLineStats
                     |> Seq.map(fun (timeSpan, _) -> TimeSpan(0, 0, (int)timeSpan.TotalSeconds).ToString())
                     |> Seq.toArray

        ScenarioStats = timeLineStats
                        |> Seq.map(fun (_, nodeStats) -> nodeStats.ScenarioStats)
                        |> Seq.toArray
    }
