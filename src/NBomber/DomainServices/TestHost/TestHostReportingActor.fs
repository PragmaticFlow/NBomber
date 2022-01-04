module internal NBomber.DomainServices.TestHost.TestHostReportingActor

open System
open System.Threading.Tasks

open FsToolkit.ErrorHandling
open FSharp.Control.Tasks.NonAffine
open FsToolkit.ErrorHandling.Operator.Task

open NBomber
open NBomber.Contracts.Stats
open NBomber.Extensions.InternalExtensions
open NBomber.Domain.LoadTimeLine
open NBomber.Domain.Stats.Statistics
open NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler
open NBomber.Infra.Dependency

type ReportingActorMessage =
    | FetchAndSaveRealtimeStats of duration:TimeSpan
    | GetTimeLineHistory        of AsyncReplyChannel<TimeLineHistoryRecord list>
    | GetFinalStats             of NodeInfo * AsyncReplyChannel<NodeStats>

let saveRealtimeScenarioStats (dep: IGlobalDependency) (stats: ScenarioStats[]) = task {
    for sink in dep.ReportingSinks do
        try
            do! sink.SaveRealtimeStats(stats)
        with
        | ex -> dep.Logger.Warning(ex, "Reporting sink: {SinkName} failed to save scenario stats", sink.SinkName)
}

let getRealtimeScenarioStats (schedulers: ScenarioScheduler list) (duration: TimeSpan) =
    schedulers
    |> List.filter(fun x  -> x.Working = true)
    |> List.map(fun x -> x.GetRealtimeStats duration)
    |> List.toArray

let getFinalScenarioStats (schedulers: ScenarioScheduler list) =
    schedulers
    |> List.map(fun x -> x.GetFinalStats())
    |> List.toArray

let getPluginStats (dep: IGlobalDependency) (operation: OperationType) = task {
    try
        let pluginStatusesTask =
            dep.WorkerPlugins
            |> List.map(fun plugin -> plugin.GetStats operation)
            |> Task.WhenAll

        let! finishedTask = Task.WhenAny(pluginStatusesTask, Task.Delay(Constants.GetPluginStatsTimeout))
        if finishedTask.Id = pluginStatusesTask.Id then return pluginStatusesTask.Result
        else
            dep.Logger.Error("Getting plugin stats failed with the timeout error")
            return Array.empty
    with
    | ex ->
        dep.Logger.Error(ex, "Getting plugin stats failed with the following error")
        return Array.empty
}

let getFinalStats (dep: IGlobalDependency)
                  (schedulers: ScenarioScheduler list)
                  (testInfo: TestInfo)
                  (nodeInfo: NodeInfo) = task {

    let! pluginStats = getPluginStats dep nodeInfo.CurrentOperation
    let scenarioStats = getFinalScenarioStats schedulers

    return if Array.isEmpty scenarioStats then None
           else Some(NodeStats.create testInfo nodeInfo scenarioStats pluginStats)
}

let create (dep: IGlobalDependency) (schedulers: ScenarioScheduler list) (testInfo: TestInfo) =
    MailboxProcessor.Start(fun inbox ->

        let saveRealtimeStats = saveRealtimeScenarioStats dep
        let getRealtimeStats = getRealtimeScenarioStats schedulers
        let getFinalStats = getFinalStats dep schedulers testInfo

        let fetchAndSaveRealtimeStats (duration, history) = async {
            let scnStats = duration |> getRealtimeStats |> Array.map(ScenarioStats.round)
            if Array.isEmpty scnStats then return history
            else
                do! scnStats |> saveRealtimeStats |> Async.AwaitTask
                let historyRecord = TimeLineHistoryRecord.create scnStats
                return historyRecord :: history
        }

        let rec loop (currentHistory: TimeLineHistoryRecord list) = async {
            try
                match! inbox.Receive() with
                | FetchAndSaveRealtimeStats duration ->
                    let! newHistory = fetchAndSaveRealtimeStats(duration, currentHistory)
                    return! loop newHistory

                | GetTimeLineHistory reply ->
                    currentHistory |> TimeLineHistory.filterRealtime |> reply.Reply
                    return! loop currentHistory

                | GetFinalStats (nodeInfo, reply) ->
                    nodeInfo
                    |> getFinalStats
                    |> TaskOption.map(NodeStats.round >> reply.Reply)
                    |> Task.WaitAll
                    return! loop currentHistory
            with
            | ex -> dep.Logger.Error(ex, "Reporting actor failed")
        }
        loop List.empty
    )
