module internal NBomber.DomainServices.TestHost.TestHostReportingActor

open System
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Task

open NBomber
open NBomber.Contracts.Stats
open NBomber.Extensions.Internal
open NBomber.Domain.LoadTimeLine
open NBomber.Domain.Stats.Statistics
open NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler
open NBomber.Infra.Dependency

let saveRealtimeScenarioStats (dep: IGlobalDependency) (stats: ScenarioStats[]) = backgroundTask {
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
    |> Task.WhenAll

let getFinalScenarioStats (schedulers: ScenarioScheduler list) =
    schedulers
    |> List.map(fun x -> x.GetFinalStats())
    |> Task.WhenAll

let getPluginStats (dep: IGlobalDependency) (operation: OperationType) = backgroundTask {
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
                  (nodeInfo: NodeInfo) = backgroundTask {

    let pluginStats = getPluginStats dep nodeInfo.CurrentOperation
    let scenarioStats = getFinalScenarioStats schedulers
    do! Task.WhenAll(pluginStats, scenarioStats)

    return if Array.isEmpty scenarioStats.Result then None
           else Some(NodeStats.create testInfo nodeInfo scenarioStats.Result pluginStats.Result)
}

type ActorMessage =
    | SaveRealtimeStats  of duration:TimeSpan
    | GetTimeLineHistory of TaskCompletionSource<TimeLineHistoryRecord list>
    | GetFinalStats      of TaskCompletionSource<NodeStats> * NodeInfo

type TestHostReportingActor(dep: IGlobalDependency, schedulers: ScenarioScheduler list, testInfo: TestInfo) =

    let saveRealtimeStats = saveRealtimeScenarioStats dep
    let getRealtimeStats = getRealtimeScenarioStats schedulers
    let getFinalStats = getFinalStats dep schedulers testInfo

    let mutable _currentHistory = List.empty<TimeLineHistoryRecord>

    let getAndSaveRealtimeStats (duration, history) = backgroundTask {
        let! scnStats = getRealtimeStats duration
        if Array.isEmpty scnStats then return history
        else
            do! scnStats |> Array.map(ScenarioStats.round) |> saveRealtimeStats
            let historyRecord = TimeLineHistoryRecord.create scnStats
            return historyRecord :: history
    }

    let _actor = ActionBlock(fun msg ->
        backgroundTask {
            try
                match msg with
                | SaveRealtimeStats duration ->
                    let! newHistory = getAndSaveRealtimeStats(duration, _currentHistory)
                    _currentHistory <- newHistory

                | GetTimeLineHistory reply ->
                    _currentHistory |> TimeLineHistory.filterRealtime |> reply.TrySetResult |> ignore

                | GetFinalStats (reply, nodeInfo) ->
                    nodeInfo
                    |> getFinalStats
                    |> TaskOption.map(NodeStats.round >> reply.TrySetResult)
                    |> Task.WaitAll
            with
            | ex -> dep.Logger.Error(ex, "TestHostReporting actor failed")
        }
        :> Task
    )

    member _.Publish(msg) = _actor.Post(msg) |> ignore

    member _.GetTimeLineHistory() =
        let tcs = TaskCompletionSource<TimeLineHistoryRecord list>()
        GetTimeLineHistory(tcs) |> _actor.Post |> ignore
        tcs.Task

    member _.GetFinalStats(nodeInfo) =
        let tcs = TaskCompletionSource<NodeStats>()
        GetFinalStats(tcs, nodeInfo) |> _actor.Post |> ignore
        tcs.Task
