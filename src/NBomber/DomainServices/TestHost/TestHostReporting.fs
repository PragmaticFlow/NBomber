module internal NBomber.DomainServices.TestHost.TestHostReporting

open System
open System.Threading.Tasks

open FsToolkit.ErrorHandling
open FSharp.Control.Tasks.NonAffine
open Nessos.Streams

open NBomber
open NBomber.Domain.DomainTypes
open NBomber.Domain.Statistics
open NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler
open NBomber.Errors
open NBomber.Contracts
open NBomber.Extensions.InternalExtensions
open NBomber.Infra.Dependency

type ActorMessage =
    | FetchAndSaveBombingStats of duration:TimeSpan
    | AddAndSaveScenarioStats   of ScenarioStats
    | GetTimeLines      of AsyncReplyChannel<TimeLineHistoryRecord list>
    | GetFinalStats     of NodeInfo * duration:TimeSpan * AsyncReplyChannel<NodeStats>

let saveScenarioStats (dep: IGlobalDependency) (stats: ScenarioStats[]) = task {
    for sink in dep.ReportingSinks do
        try
            do! sink.SaveScenarioStats(stats)
        with
        | ex -> dep.Logger.Warning(ex, "Reporting sink '{SinkName}' failed to save scenario stats.", sink.SinkName)
}

let saveFinalStats (dep: IGlobalDependency) (stats: NodeStats[]) = task {
    for sink in dep.ReportingSinks do
        try
            do! sink.SaveFinalStats(stats)
        with
        | ex -> dep.Logger.Warning(ex, "Reporting sink '{SinkName}' failed to save final stats.", sink.SinkName)
}

let getPluginStats (dep: IGlobalDependency) (operation: OperationType) =
    dep.WorkerPlugins
    |> List.map(fun plugin -> plugin.GetStats operation)
    |> Task.WhenAll

let getScenarioStats (schedulers: ScenarioScheduler list) (working: Option<bool>) (duration: TimeSpan) =
    schedulers
    |> Stream.ofList
    |> Stream.filter(fun x ->
        match working with
        | Some value -> x.Working = value
        | None       -> true
    )
    |> Stream.map(fun x -> x.GetScenarioStats duration)

let getNodeStats (dep: IGlobalDependency) (schedulers: ScenarioScheduler list) (testInfo: TestInfo)
                 (nodeInfo: NodeInfo) (operation: OperationType)
                 (working: Option<bool>) (duration: TimeSpan) = task {

    let! pluginStats = getPluginStats dep operation
    let scenarioStats = getScenarioStats schedulers working duration

    return if Stream.isEmpty scenarioStats then None
           else Some(NodeStats.create testInfo nodeInfo scenarioStats pluginStats)
}

let createReportingActor (dep: IGlobalDependency, schedulers: ScenarioScheduler list, testInfo: TestInfo) =
    MailboxProcessor.Start(fun inbox ->

        let getBombingPluginStats = getPluginStats dep
        let getBombingScenarioStats = getScenarioStats schedulers (Some true)
        let getNodeStats = getNodeStats dep schedulers testInfo
        let saveScenarioStats = saveScenarioStats dep

        let fetchAndSaveBombingStats (duration, history) = async {
            let pluginStatsTask = getBombingPluginStats(OperationType.Bombing)
            let scnStats = getBombingScenarioStats(duration) |> Stream.map(ScenarioStats.round) |> Stream.toArray
            do! scnStats |> saveScenarioStats |> Async.AwaitTask
            let! pluginStats = pluginStatsTask |> Async.AwaitTask
            return { Duration = duration; ScenarioStats = scnStats; PluginStats = pluginStats } :: history
        }

        let addAndSaveScenarioStats (scnStats, history) = async {
            let stats = scnStats |> ScenarioStats.round |> Array.singleton
            do! stats |> saveScenarioStats |> Async.AwaitTask
            return { Duration = scnStats.Duration; ScenarioStats = stats; PluginStats = Array.empty } :: history
        }

        let rec loop (currentHistory: TimeLineHistoryRecord list) = async {
            match! inbox.Receive() with
            | FetchAndSaveBombingStats duration ->
                let! newHistory = fetchAndSaveBombingStats(duration, currentHistory)
                return! loop newHistory

            | AddAndSaveScenarioStats scnStats ->
                let! newHistory = addAndSaveScenarioStats(scnStats, currentHistory)
                return! loop newHistory

            | GetTimeLines reply ->
                reply.Reply(currentHistory |> List.rev)
                return! loop currentHistory

            | GetFinalStats (nodeInfo, duration, reply) ->
                getNodeStats nodeInfo OperationType.Complete None duration
                |> TaskOption.map(NodeStats.round >> reply.Reply)
                |> Task.WaitAll
                return! loop currentHistory
        }
        loop List.empty
    )

let initReportingSinks (dep: IGlobalDependency) (context: IBaseContext) = taskResult {
    try
        for sink in dep.ReportingSinks do
            dep.Logger.Information("Start init reporting sink: '{SinkName}'.", sink.SinkName)
            do! sink.Init(context, dep.InfraConfig |> Option.defaultValue Constants.EmptyInfraConfig)
    with
    | ex -> return! AppError.createResult(InitScenarioError ex)
}

let startReportingSinks (dep: IGlobalDependency) = task {
    for sink in dep.ReportingSinks do
        try
            sink.Start() |> ignore
        with
        | ex -> dep.Logger.Warning(ex, "Failed to start reporting sink '{SinkName}'.", sink.SinkName)
}

let stopReportingSinks (dep: IGlobalDependency) = task {
    for sink in dep.ReportingSinks do
        try
            dep.Logger.Information("Stop reporting sink: '{SinkName}'.", sink.SinkName)
            do! sink.Stop()
        with
        | ex -> dep.Logger.Warning(ex, "Stop reporting sink '{SinkName}' failed.", sink.SinkName)
}
