module internal NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler

open System
open System.Threading.Tasks

open FSharp.Control.Reactive

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Stats.ScenarioStatsActor
open NBomber.Domain.Concurrency.ScenarioActor
open NBomber.Domain.Concurrency.Scheduler.ConstantActorScheduler
open NBomber.Domain.Concurrency.Scheduler.OneTimeActorScheduler

[<Struct>]
type SchedulerCommand =
    | AddConstantActors    of addCount:int
    | RemoveConstantActors of removeCount:int
    | InjectOneTimeActors  of scheduledCount:int
    | DoNothing

type ScenarioProgressInfo = {
    ConstantActorCount: int
    OneTimeActorCount: int
    CurrentSimulation: LoadSimulation
}

type SchedulerEvent =
    | ScenarioStarted
    | ProgressUpdated of ScenarioProgressInfo

let calcScheduleByTime (copiesCount: int, prevSegmentCopiesCount: int, timeSegmentProgress: int) =
    let value = copiesCount - prevSegmentCopiesCount
    let result = (float value / 100.0 * float timeSegmentProgress) + float prevSegmentCopiesCount
    int(Math.Round(result, 0, MidpointRounding.AwayFromZero))

let schedule (getRandomValue: int -> int -> int) // min -> max -> result
             (timeSegment: LoadTimeSegment)
             (timeSegmentProgress: int)
             (constWorkingActorCount: int) =

    match timeSegment.LoadSimulation with
    | RampConstant (copiesCount, _) ->
        let scheduled = calcScheduleByTime(copiesCount, timeSegment.PrevSegmentCopiesCount, timeSegmentProgress)
        let scheduleNow = scheduled - constWorkingActorCount
        if scheduleNow > 0 then [AddConstantActors scheduleNow]
        elif scheduleNow < 0 then [RemoveConstantActors(Math.Abs scheduleNow)]
        else [DoNothing]

    | KeepConstant (copiesCount, _) ->
        if constWorkingActorCount < copiesCount then [AddConstantActors(copiesCount - constWorkingActorCount)]
        elif constWorkingActorCount > copiesCount then [RemoveConstantActors(constWorkingActorCount - copiesCount)]
        else [DoNothing]

    | RampPerSec (copiesCount, _) ->
        let scheduled = calcScheduleByTime(copiesCount, timeSegment.PrevSegmentCopiesCount, timeSegmentProgress)
        let command = InjectOneTimeActors(Math.Abs scheduled)
        if constWorkingActorCount > 0 then [RemoveConstantActors(constWorkingActorCount); command]
        else [command]

    | InjectPerSec (copiesCount, _) ->
        let command = InjectOneTimeActors(copiesCount)
        if constWorkingActorCount > 0 then [RemoveConstantActors(constWorkingActorCount); command]
        else [command]

    | InjectPerSecRandom (minRate, maxRate, _) ->
        let copiesCount = getRandomValue minRate maxRate
        let command = InjectOneTimeActors(copiesCount)
        if constWorkingActorCount > 0 then [RemoveConstantActors(constWorkingActorCount); command]
        else [command]

let emptyExec (dep: ActorDep) (actorPool: ScenarioActor list) (scheduledActorCount: int) = actorPool

type ScenarioScheduler(dep: ActorDep, scenarioClusterCount: int) =

    let _log = dep.ScenarioDep.Logger.ForContext<ScenarioScheduler>()
    let _scnDep = dep.ScenarioDep
    let mutable _scenario = _scnDep.Scenario
    let mutable _currentSimulation = _scenario.LoadTimeLine.Head.LoadSimulation
    let mutable _cachedSimulationStats = Unchecked.defaultof<LoadSimulationStats>

    let _constantScheduler =
        if _scenario.IsEnabled then new ConstantActorScheduler(dep, ConstantActorScheduler.exec)
        else new ConstantActorScheduler(dep, emptyExec)

    let _oneTimeScheduler =
        if _scenario.IsEnabled then new OneTimeActorScheduler(dep, OneTimeActorScheduler.exec)
        else new OneTimeActorScheduler(dep, emptyExec)

    let _schedulerTimer = new System.Timers.Timer(Constants.SchedulerTickIntervalMs)
    let _progressTimer = new System.Timers.Timer(Constants.SchedulerTickIntervalMs)
    let _eventStream = Subject.broadcast
    let _tcs = TaskCompletionSource()
    let _randomGen = Random()

    let getConstantActorCount () = _constantScheduler.ScheduledActorCount * scenarioClusterCount
    let getOneTimeActorCount () = _oneTimeScheduler.ScheduledActorCount * scenarioClusterCount

    let getCurrentSimulationStats () =
        LoadTimeLine.createSimulationStats(_currentSimulation, getConstantActorCount(), getOneTimeActorCount())

    let prepareForRealtimeStats () =
        _cachedSimulationStats <- getCurrentSimulationStats()
        _scnDep.ScenarioStatsActor.Publish StartUseTempBuffer

    let buildRealtimeStats (duration) =
        let simulationStats = getCurrentSimulationStats()
        let reply = TaskCompletionSource<ScenarioStats>()
        _scnDep.ScenarioStatsActor.Publish(BuildRealtimeStats(reply, simulationStats, duration))
        reply.Task

    let commitRealtimeStats (duration) =
        let reply = TaskCompletionSource<ScenarioStats>()
        _scnDep.ScenarioStatsActor.Publish(BuildRealtimeStats(reply, _cachedSimulationStats, duration))
        _scnDep.ScenarioStatsActor.Publish FlushTempBuffer
        reply.Task

    let getFinalStats () =
        let simulationStats = getCurrentSimulationStats()
        let duration = Scenario.getExecutedDuration _scenario
        let reply = TaskCompletionSource<ScenarioStats>()
        _scnDep.ScenarioStatsActor.Publish(GetFinalStats(reply, simulationStats, duration))
        reply.Task

    let getRandomValue minRate maxRate =
        _randomGen.Next(minRate, maxRate)

    let stop () =
        _tcs.TrySetResult() |> ignore

        if not _scnDep.ScenarioCancellationToken.IsCancellationRequested then
            _scnDep.ScenarioCancellationToken.Cancel()

        if _schedulerTimer.Enabled then
            _schedulerTimer.Stop()
            _scenario <- Scenario.setExecutedDuration(_scenario, _scnDep.ScenarioTimer.Elapsed)

            _scnDep.ScenarioTimer.Stop()
            _progressTimer.Stop()
            _constantScheduler.Stop()
            _oneTimeScheduler.Stop()
            _eventStream.OnCompleted()

    let execScheduler () =
        let currentTime = _scnDep.ScenarioTimer.Elapsed

        if _scnDep.ScenarioOperation = ScenarioOperation.WarmUp
           && _scenario.WarmUpDuration.IsSome && _scenario.WarmUpDuration.Value <= currentTime then
            stop()
        else
            match LoadTimeLine.getRunningTimeSegment(_scenario.LoadTimeLine, currentTime) with
            | Some timeSegment ->

                _currentSimulation <- timeSegment.LoadSimulation

                let timeProgress =
                    timeSegment
                    |> LoadTimeLine.calcTimeSegmentProgress currentTime
                    |> LoadTimeLine.correctTimeProgress

                schedule getRandomValue timeSegment timeProgress _constantScheduler.ScheduledActorCount
                |> List.iter(function
                    | AddConstantActors count    -> _constantScheduler.AddActors(count)
                    | RemoveConstantActors count -> _constantScheduler.RemoveActors(count)
                    | InjectOneTimeActors count  -> _oneTimeScheduler.InjectActors(count)
                    | DoNothing -> ()
                )

            | None -> stop()

    let start () =
        _schedulerTimer.Start()
        _progressTimer.Start()
        _eventStream.OnNext(ScenarioStarted)
        _scnDep.ScenarioTimer.Restart()
        execScheduler()
        _tcs.Task :> Task

    do
        _schedulerTimer.Elapsed.Add(fun _ -> execScheduler())
        _progressTimer.Elapsed.Add(fun _ ->
            let progressInfo = {
                ConstantActorCount = getConstantActorCount()
                OneTimeActorCount = getOneTimeActorCount()
                CurrentSimulation = _currentSimulation
            }
            _eventStream.OnNext(ProgressUpdated progressInfo)
        )

    member _.Working = _schedulerTimer.Enabled
    member _.EventStream = _eventStream :> IObservable<_>
    member _.Scenario = _scenario
    member _.AllRealtimeStats = _scnDep.ScenarioStatsActor.AllRealtimeStats

    member _.Start() = start()
    member _.Stop() = stop()

    member _.AddStatsFromAgent(stats) = _scnDep.ScenarioStatsActor.Publish(AddFromAgent stats)
    member _.PrepareForRealtimeStats() = prepareForRealtimeStats()
    member _.CommitRealtimeStats(duration) = commitRealtimeStats duration
    member _.BuildRealtimeStats(duration) = buildRealtimeStats duration
    member _.GetFinalStats() = getFinalStats()

    interface IDisposable with
        member _.Dispose() =
            stop()
            _eventStream.Dispose()
            _log.Verbose $"{nameof ScenarioScheduler} disposed"

