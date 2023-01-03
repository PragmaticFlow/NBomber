module internal NBomber.DomainServices.TestHost.ReportingManager

open System
open System.Threading.Tasks
open FsToolkit.ErrorHandling
open NBomber.Contracts.Internal
open NBomber.Contracts.Stats
open NBomber.Extensions.Internal
open NBomber.Domain
open NBomber.Domain.LoadSimulation
open NBomber.Domain.Stats.Statistics
open NBomber.Domain.Scheduler.ScenarioScheduler
open NBomber.Infra

type IReportingManager =
    inherit IDisposable
    abstract Start: unit -> unit
    abstract Stop: unit -> Task<unit>
    abstract GetSessionResult: NodeInfo -> Task<NodeSessionResult>

let getHints (dep: IGlobalDependency) (useHintsAnalyzer: bool) (finalStats: NodeStats) =
    if useHintsAnalyzer then
        dep.WorkerPlugins
        |> WorkerPlugins.getHints
        |> List.append(HintsAnalyzer.analyzeNodeStats finalStats)
        |> List.toArray
    else
        Array.empty

let getFinalStats (dep: IGlobalDependency)
                  (testInfo: TestInfo)
                  (schedulers: ScenarioScheduler list)
                  (nodeInfo: NodeInfo) = backgroundTask {

    let! scenarioStats =
        schedulers
        |> List.map(fun x -> x.GetFinalStats())
        |> Task.WhenAll

    let nodeStats = NodeStats.create testInfo nodeInfo scenarioStats

    let! pluginStats = WorkerPlugins.getStats dep nodeStats
    return { nodeStats with PluginStats = pluginStats }
}

let getSessionResult (dep: IGlobalDependency)
                     (testInfo: TestInfo)
                     (useHintsAnalyzer: bool)
                     (schedulers: ScenarioScheduler list)
                     (nodeInfo: NodeInfo) = backgroundTask {
    let history =
        schedulers
        |> Seq.map(fun x -> x.AllRealtimeStats)
        |> TimeLineHistory.create

    let! finalStats = getFinalStats dep testInfo schedulers nodeInfo
    let hints = getHints dep useHintsAnalyzer finalStats

    let result = { FinalStats = finalStats; TimeLineHistory = history; Hints = hints }
    return result
}

type ReportingManager(dep: IGlobalDependency,
                      schedulers: ScenarioScheduler list,
                      sessionArgs: SessionArgs) =

    let _reportingInterval = sessionArgs.GetReportingInterval()
    let _buildRealtimeStatsTimer = new Timers.Timer(_reportingInterval.TotalMilliseconds)
    let mutable _curDuration = TimeSpan.Zero
    let _timerMaxDuration = schedulers |> Seq.map(fun x -> x.Scenario.PlanedDuration) |> Seq.max |> fun duration -> duration.Add(TimeSpan.FromSeconds 2)

    let getSessionResult = getSessionResult dep sessionArgs.TestInfo (sessionArgs.GetUseHintsAnalyzer()) schedulers

    let start () =
        _buildRealtimeStatsTimer.Start()

    let stop () = backgroundTask {
        _buildRealtimeStatsTimer.Stop()
    }

    do
        _buildRealtimeStatsTimer.Elapsed.Add(fun _ ->
            let duration = _curDuration + _reportingInterval
            if duration <= _timerMaxDuration then
                _curDuration <- duration
                schedulers
                |> List.map(fun x -> x.BuildRealtimeStats duration)
                |> Task.WhenAll
                |> Task.map(ReportingSinks.saveRealtimeStats dep)
                |> ignore
            else
                _buildRealtimeStatsTimer.Stop()
        )

    interface IReportingManager with
        member _.Start() = start()
        member _.Stop() = stop()
        member _.GetSessionResult(nodeInfo) = getSessionResult nodeInfo

    interface IDisposable with
        member _.Dispose() =
            _buildRealtimeStatsTimer.Dispose()

let create (dep: IGlobalDependency) (schedulers: ScenarioScheduler list) (sessionArgs: SessionArgs) =
    new ReportingManager(dep, schedulers, sessionArgs) :> IReportingManager
