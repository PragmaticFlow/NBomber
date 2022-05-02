module internal NBomber.DomainServices.TestHost.ReportingManager

open System
open System.Diagnostics
open System.Threading.Tasks

open FSharp.Control.Reactive
open FsToolkit.ErrorHandling

open NBomber.Contracts.Stats
open NBomber.Extensions.Internal
open NBomber.Domain
open NBomber.Domain.LoadTimeLine
open NBomber.Domain.Stats.Statistics
open NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler
open NBomber.Infra.Dependency
open NBomber.DomainServices.NBomberContext

type IReportingManager =
    inherit IDisposable
    abstract IsWorking: bool
    abstract RealtimeStatsStream: IObservable<ScenarioStats[]>
    abstract Start: unit -> unit
    abstract Stop: unit -> Task<unit>
    abstract GetCurrentSessionTime: unit -> TimeSpan
    abstract GetSessionResult: NodeInfo -> Task<NodeSessionResult>

type ReportingManager(dep: IGlobalDependency, schedulers: ScenarioScheduler list, sessionArgs: SessionArgs) =

    let _saveStatsTimer = new Timers.Timer(sessionArgs.ReportingInterval.TotalMilliseconds)
    let _globalSessionTimer = Stopwatch()
    let _statsStream = Subject.broadcast
    let mutable _currentHistory = List.empty
    let mutable _isWorking = false

    let _realtimeTimerMaxDuration =
        schedulers |> Seq.map(fun x -> x.Scenario.PlanedDuration) |> Seq.max |> fun duration -> duration.Add(TimeSpan.FromSeconds 2)

    let getHints (finalStats: NodeStats) =
        if sessionArgs.UseHintsAnalyzer then
            dep.WorkerPlugins
            |> WorkerPlugins.getHints
            |> List.append(HintsAnalyzer.analyzeNodeStats finalStats)
            |> List.toArray
        else
            Array.empty

    let getFinalStats (nodeInfo: NodeInfo) = backgroundTask {
        let! scenarioStats =
            schedulers
            |> List.map(fun x -> x.GetFinalStats())
            |> Task.WhenAll

        let nodeStats =
            NodeStats.create sessionArgs.TestInfo nodeInfo scenarioStats
            |> NodeStats.round

        let! pluginStats = WorkerPlugins.getStats dep.Logger dep.WorkerPlugins nodeStats
        return { nodeStats with PluginStats = pluginStats }
    }

    let getRealtimeStats () =
        schedulers
        |> List.filter(fun x  -> x.Working = true)
        |> List.map(fun x ->
            _globalSessionTimer.Elapsed
            |> x.GetRealtimeStats
            |> Task.map ScenarioStats.round
        )
        |> Task.WhenAll

    let getRealtimeHistoryRecord () = backgroundTask {
        let! realtimeStats = getRealtimeStats()
        return TimeLineHistoryRecord.create realtimeStats
    }

    let fetchAndPublishStats () = backgroundTask {
        let! historyRecord = getRealtimeHistoryRecord()
        _currentHistory <- historyRecord :: _currentHistory

        _statsStream.OnNext historyRecord.ScenarioStats
    }

    let getSessionResult (nodeInfo: NodeInfo) = backgroundTask {
        let history =
            _currentHistory
            |> TimeLineHistory.filterRealtime
            |> List.toArray

        let! finalStats = getFinalStats nodeInfo
        let hints = getHints finalStats

        let result = { FinalStats = finalStats; TimeLineHistory = history; Hints = hints }
        return result
    }

    do
        _saveStatsTimer.Elapsed.Add(fun _ ->
            if _globalSessionTimer.Elapsed <= _realtimeTimerMaxDuration then
                fetchAndPublishStats() |> ignore
            else
                _isWorking <- false
        )

    interface IReportingManager with
        member _.IsWorking = _isWorking
        member _.RealtimeStatsStream = _statsStream :> IObservable<_>

        member _.Start() =
            _isWorking <- true
            _saveStatsTimer.Start()
            _globalSessionTimer.Reset()
            _globalSessionTimer.Start()

        member _.Stop() = backgroundTask {
            _isWorking <- false
            _saveStatsTimer.Stop()
            _globalSessionTimer.Stop()
        }

        member _.GetCurrentSessionTime() = _globalSessionTimer.Elapsed
        member _.GetSessionResult(nodeInfo) = getSessionResult nodeInfo

    interface IDisposable with
        member _.Dispose() =
            _saveStatsTimer.Dispose()
            _statsStream.Dispose()
