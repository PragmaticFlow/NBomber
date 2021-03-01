namespace NBomber.DomainServices.Reporting.ViewModels

open System
open System.Data

open NBomber.Contracts
open NBomber.Domain.DomainTypes
open NBomber.Domain.HintsAnalyzer

type TimeLineHistoryRecordViewModel = {
    Duration: TimeSpan
    ScenarioStats: ScenarioStats[]
    PluginStats: DataSet[]
}

type HintResultViewModel = {
    SourceName: string
    SourceType: string
    Hint: string
}

type HtmlReportViewModel = {
    NodeStats: NodeStats
    TimeLineStats: TimeLineHistoryRecordViewModel[]
    Hints: HintResultViewModel[]
}

module internal TimeLineHistoryRecordViewModel =

    let create (record: TimeLineHistoryRecord) = {
        Duration = record.Duration
        ScenarioStats = record.ScenarioStats
        PluginStats = record.PluginStats
    }

module internal HintResultViewModel =

    let create (hint: HintResult) = {
        SourceName = hint.SourceName
        SourceType = hint.SourceType.ToString()
        Hint = hint.Hint
    }

module internal HtmlReportViewModel =

    let create (stats: NodeStats, timeLineStats: TimeLineHistoryRecord list, hints: HintResult list) = {
        NodeStats = stats
        TimeLineStats = timeLineStats |> Seq.map TimeLineHistoryRecordViewModel.create |> Array.ofSeq
        Hints = hints |> Seq.map HintResultViewModel.create |> Array.ofSeq
    }
