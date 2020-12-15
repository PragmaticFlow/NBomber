namespace NBomber.DomainServices.Reporting.ViewModels

open System
open System.Collections.Generic
open System.Data

open NBomber.Contracts
open NBomber.DomainServices
open NBomber.Extensions
open NBomber.Extensions.InternalExtensions
open Newtonsoft.Json

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

type CustomPluginDataViewModel = {
    PluginName: string
    Title: string
    ViewModel: obj
} with
    static member Empty = {
        PluginName = String.Empty
        Title = String.Empty
        ViewModel = Unchecked.defaultof<obj>
    }

type NodeStatsViewModel = {
    RequestCount: int
    OkCount: int
    FailCount: int
    AllDataMB: float
    ScenarioStats: ScenarioStats[]
    PluginStats: PluginStatsViewModel[]
    CustomPluginData: CustomPluginDataViewModel[]
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
        |> PluginStats.getStatsTables
        |> Seq.map mapDataTableToPluginStatsViewModel
        |> Array.ofSeq

    let private mapToCustomPluginDataViewModel (pluginStats: DataSet[]) =
        pluginStats
        |> Seq.map(fun ps ->
            let customPluginData = PluginStats.tryGetCustomPluginData(ps)
            customPluginData, ps.DataSetName
        )
        |> Seq.map(fun (customPluginDataOpt, pluginName) ->
            customPluginDataOpt
            |> Option.map(fun customPluginData ->
                {
                    PluginName = pluginName
                    Title = customPluginData.Title
                    ViewModel = JsonConvert.DeserializeObject(customPluginData.ViewModel)
                }
            )
            |> Option.defaultValue CustomPluginDataViewModel.Empty
        )
        |> Seq.filter(fun customPluginData -> not(String.IsNullOrEmpty(customPluginData.Title)))
        |> Seq.toArray

    let create (stats: NodeStats): NodeStatsViewModel = {
        RequestCount = stats.RequestCount
        OkCount = stats.OkCount
        FailCount = stats.FailCount
        AllDataMB = stats.AllDataMB
        ScenarioStats = stats.ScenarioStats
        PluginStats = mapToPluginStatsViewModel(stats.PluginStats)
        CustomPluginData = mapToCustomPluginDataViewModel(stats.PluginStats)
        NodeInfo = stats.NodeInfo
    }

module TimeLineStatsViewModel =

    let private getLatencyCountDiff (latency: LatencyCount) (prevLatency: LatencyCount) = {
        Less800 = latency.Less800 - prevLatency.Less800
        More800Less1200 = latency.More800Less1200 - prevLatency.More800Less1200
        More1200 = latency.More1200 - prevLatency.More1200
    }

    let private getScenarioStatsDiff (scenarioStats: ScenarioStats) (prevScenarioStats: ScenarioStats) = {
        scenarioStats with
            LatencyCount = getLatencyCountDiff scenarioStats.LatencyCount prevScenarioStats.LatencyCount
            RequestCount = scenarioStats.RequestCount - prevScenarioStats.RequestCount
            OkCount = scenarioStats.OkCount - prevScenarioStats.OkCount
            FailCount = scenarioStats.FailCount - prevScenarioStats.FailCount
            AllDataMB = scenarioStats.AllDataMB - prevScenarioStats.AllDataMB
    }

    let private getTimeLineScenarioStatsDiff (scenarioStats: ScenarioStats[]) (prevScenarioStats: ScenarioStats[]) =
        scenarioStats
        |> Seq.mapi(fun i stats -> getScenarioStatsDiff stats prevScenarioStats.[i])
        |> Seq.toArray

    let private createTimeStamps (timeLineStats: (TimeSpan * NodeStats) list) =
        timeLineStats
        |> Seq.map(fun (timeSpan, _) -> timeSpan)
        |> Seq.map(fun timeSpan -> TimeSpan(0, 0, (int)timeSpan.TotalSeconds).ToString())
        |> Seq.toArray

    let private createScenarioStats (timeLineStats: (TimeSpan * NodeStats) list) =
        let timeLineScenarioStats =
            timeLineStats
            |> Seq.map(fun (_, nodeStats) -> nodeStats.ScenarioStats)
            |> Seq.toArray

        timeLineScenarioStats
        |> Seq.mapi(fun i scenarioStats ->
            if i = 0 then
                scenarioStats
            else
                getTimeLineScenarioStatsDiff scenarioStats timeLineScenarioStats.[i - 1]
        )
        |> Seq.toArray

    let create (timeLineStats: (TimeSpan * NodeStats) list) = {
        TimeStamps = createTimeStamps(timeLineStats)
        ScenarioStats = createScenarioStats(timeLineStats)
    }
