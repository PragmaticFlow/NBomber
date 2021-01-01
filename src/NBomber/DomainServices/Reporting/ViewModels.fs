namespace NBomber.DomainServices.Reporting.ViewModels

open System
open System.Data

open NBomber.Contracts
open NBomber.Domain.HintsAnalyzer
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

type HintViewModel = {
    SourceName: string
    SourceType: string
    Hint: string
}

type HintsViewModel = HintViewModel[]

module internal NBomberInfoViewModel =

    let create (nodeInfo: NodeInfo) = {
        NBomberVersion = nodeInfo.NBomberVersion
    }

module internal TestInfoViewModel =

    let create (testInfo: TestInfo): TestInfoViewModel = {
        TestSuite = testInfo.TestSuite
        TestName = testInfo.TestName
    }

module internal NodeStatsViewModel =

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

module internal TimeLineStatsViewModel =

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
        |> Seq.map fst
        |> Seq.map(fun timeSpan -> TimeSpan(0, 0, (int)timeSpan.TotalSeconds).ToString())
        |> Seq.toArray

    let private createScenarioStats (timeLineStats: (TimeSpan * NodeStats) list) =
        let timeLineScenarioStats =
            timeLineStats
            |> Seq.map snd
            |> Seq.map(fun nodeStats -> nodeStats.ScenarioStats)
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

module internal HintsViewModel =

    let create(hints: HintResult list) =
        hints
        |> Seq.map(fun hint -> {
            SourceName = hint.SourceName
            SourceType = hint.SourceType.ToString()
            Hint = hint.Hint
        })
        |> Seq.toArray
