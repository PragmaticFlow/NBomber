module internal NBomber.DomainServices.TestHost.ReportingManager

open System
open System.Threading.Tasks
open FsToolkit.ErrorHandling

open NBomber.Constants.Reporting
open NBomber.Contracts.Internal
open NBomber.Contracts.Stats
open NBomber.Extensions.Internal
open NBomber.Domain
open NBomber.Domain.LoadSimulation
open NBomber.Domain.Stats.Statistics
open NBomber.Domain.Stats.MetricsStatsActor
open NBomber.Domain.Scheduler.ScenarioScheduler
open NBomber.Infra
open NBomber.DomainServices.TestHost.MetricsGrabber

type IReportingManager =
    inherit IDisposable
    abstract Start: unit -> Task<unit>
    abstract Stop: unit -> Task<unit>
    abstract GetCurrentMetrics: unit -> MetricStats[]
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
                  (metricsStatsActor: MetricsStatsActor)
                  (nodeInfo: NodeInfo) = backgroundTask {

    let! scenarioStats =
        schedulers
        |> List.map(fun x -> x.GetFinalStats())
        |> Task.WhenAll

    let maxDuration =
        if Array.isEmpty scenarioStats then
            TimeSpan.Zero
        else
            scenarioStats |> Seq.map(fun x -> x.Duration) |> Seq.max |> Converter.roundDuration

    let! metrics = metricsStatsActor.GetFinalStats maxDuration

    let nodeStats = NodeStats.create testInfo nodeInfo scenarioStats metrics

    let! pluginStats = WorkerPlugins.getStats dep nodeStats
    return { nodeStats with PluginStats = pluginStats }
}

let getSessionResult (dep: IGlobalDependency)
                     (testInfo: TestInfo)
                     (useHintsAnalyzer: bool)
                     (schedulers: ScenarioScheduler list)
                     (metricsStatsActor: MetricsStatsActor)
                     (nodeInfo: NodeInfo) = backgroundTask {
    let history =
        schedulers
        |> Seq.map(fun x -> x.AllRealtimeStats)
        |> TimeLineHistory.create

    let! finalStats = getFinalStats dep testInfo schedulers metricsStatsActor nodeInfo
    let hints = getHints dep useHintsAnalyzer finalStats

    let result = { FinalStats = finalStats; TimeLineHistory = history; Hints = hints }
    return result
}

type ReportingManager(dep: IGlobalDependency,
                      schedulers: ScenarioScheduler list,
                      sessionArgs: SessionArgs) =

    let _reportingInterval = sessionArgs.GetReportingInterval()
    let _buildRealtimeStatsTimer = new Timers.Timer(_reportingInterval.TotalMilliseconds)
    let _metricsActor = new MetricsStatsActor(dep.Logger)
    let _timerMaxDuration = schedulers |> Seq.map(fun x -> x.Scenario.PlanedDuration) |> Seq.max

    let mutable _metricsGrabber = Option.None
    let mutable _currentTime = TimeSpan.Zero

    let getSessionResult = getSessionResult dep sessionArgs.TestInfo (sessionArgs.GetUseHintsAnalyzer()) schedulers _metricsActor

    let start () = backgroundTask {
        _metricsGrabber <- Some (new RuntimeMetricsGrabber(_metricsActor))
        do! Task.Delay TIMER_START_DELAY
        _buildRealtimeStatsTimer.Start()
    }

    let stop () = backgroundTask {
        do! Task.Delay TIMER_STOP_DELAY
        _metricsGrabber |> Option.iter(fun x -> x.Dispose())
        _buildRealtimeStatsTimer.Stop()
    }

    do
        _buildRealtimeStatsTimer.Elapsed.Add(fun _ ->
            let duration = _currentTime + _reportingInterval
            if duration <= _timerMaxDuration then
                _currentTime <- duration

                backgroundTask {
                    let statsTasks =
                        schedulers
                        |> Seq.map(fun x -> x.BuildRealtimeStats _currentTime)

                    let! metrics = _metricsActor.BuildReportingStats _currentTime

                    let! realtimeStats = statsTasks |> Task.WhenAll

                    ReportingSinks.saveRealtimeStats dep realtimeStats
                    |> ignore
                }
                |> ignore
            else
                _buildRealtimeStatsTimer.Stop()
        )

    interface IReportingManager with
        member this.Start() = start()
        member this.Stop() = stop()
        member this.GetCurrentMetrics() = _metricsActor.CurrentMetrics
        member this.GetSessionResult(nodeInfo) = getSessionResult nodeInfo

        member this.Dispose() =
            _buildRealtimeStatsTimer.Dispose()
            (_metricsActor :> IDisposable).Dispose()

let create (dep: IGlobalDependency) (schedulers: ScenarioScheduler list) (sessionArgs: SessionArgs) =
    new ReportingManager(dep, schedulers, sessionArgs) :> IReportingManager
