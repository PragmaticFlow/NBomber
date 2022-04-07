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

let correctExecutedDuration (executionTime: TimeSpan) (scnDuration: TimeSpan) =
    if executionTime > scnDuration then scnDuration
    else executionTime

let emptyExec (dep: ActorDep) (actorPool: ScenarioActor list) (scheduledActorCount: int) = actorPool

type ScenarioScheduler(dep: ActorDep, scenarioClusterCount: int) =

    let _logger = dep.Logger.ForContext<ScenarioScheduler>()
    let mutable _warmUp = false
    let mutable _scenario = dep.Scenario
    let mutable _currentSimulation = dep.Scenario.LoadTimeLine.Head.LoadSimulation
    let mutable _currentOperation = OperationType.None

    let _constantScheduler =
        if dep.Scenario.IsEnabled then new ConstantActorScheduler(dep, ConstantActorScheduler.exec)
        else new ConstantActorScheduler(dep, emptyExec)

    let _oneTimeScheduler =
        if dep.Scenario.IsEnabled then new OneTimeActorScheduler(dep, OneTimeActorScheduler.exec)
        else new OneTimeActorScheduler(dep, emptyExec)

    let _schedulerTimer = new System.Timers.Timer(Constants.SchedulerTickIntervalMs)
    let _progressTimer = new System.Timers.Timer(Constants.SchedulerTickIntervalMs)
    let _eventStream = Subject.broadcast
    let _tcs = TaskCompletionSource()
    let _randomGen = Random()

    let getConstantActorCount () = _constantScheduler.ScheduledActorCount * scenarioClusterCount
    let getOneTimeActorCount () = _oneTimeScheduler.ScheduledActorCount * scenarioClusterCount

    let getCurrentSimulationStats () =
        LoadTimeLine.createSimulationStats(
            _currentSimulation,
            getConstantActorCount(),
            getOneTimeActorCount()
        )

    let getRealtimeStats (duration) =
        let executedDuration = _scenario |> Scenario.getDuration |> correctExecutedDuration duration
        let simulationStats = getCurrentSimulationStats()
        dep.ScenarioStatsActor.GetRealtimeStats(simulationStats, executedDuration)

    let getFinalStats () =
        let scnDuration = Scenario.getDuration _scenario
        let simulationStats = getCurrentSimulationStats()
        dep.ScenarioStatsActor.GetFinalStats(simulationStats, scnDuration)

    let start () =
        dep.ShouldWork <- true
        dep.ScenarioGlobalTimer.Stop()
        _schedulerTimer.Start()
        _progressTimer.Start()
        _tcs.Task :> Task

    let stop () =
        if _schedulerTimer.Enabled then
            dep.ShouldWork <- false
            _scenario <- Scenario.setExecutedDuration(_scenario, dep.ScenarioGlobalTimer.Elapsed)
            _currentOperation <- OperationType.Complete

            dep.ScenarioGlobalTimer.Stop()
            _schedulerTimer.Stop()
            _progressTimer.Stop()
            _constantScheduler.Stop()
            _oneTimeScheduler.Stop()
            _eventStream.OnCompleted()

            _tcs.TrySetResult() |> ignore

    let getRandomValue minRate maxRate =
        _randomGen.Next(minRate, maxRate)

    do
        _schedulerTimer.Elapsed
        |> Observable.subscribe(fun _ ->
            if not dep.ScenarioGlobalTimer.IsRunning then
                _eventStream.OnNext(ScenarioStarted)
                dep.ScenarioGlobalTimer.Restart()

            let currentTime = dep.ScenarioGlobalTimer.Elapsed

            if _warmUp && dep.Scenario.WarmUpDuration <= currentTime then
                stop()
            else
                match LoadTimeLine.getRunningTimeSegment(dep.Scenario.LoadTimeLine, currentTime) with
                | Some timeSegment ->

                    _currentSimulation <- timeSegment.LoadSimulation

                    let timeProgress =
                        timeSegment
                        |> LoadTimeLine.calcTimeSegmentProgress(currentTime)
                        |> LoadTimeLine.correctTimeProgress

                    schedule getRandomValue timeSegment timeProgress _constantScheduler.ScheduledActorCount
                    |> List.iter(function
                        | AddConstantActors count    -> _constantScheduler.AddActors(count)
                        | RemoveConstantActors count -> _constantScheduler.RemoveActors(count)
                        | InjectOneTimeActors count  -> _oneTimeScheduler.InjectActors(count)
                        | DoNothing -> ()
                    )

                | None -> stop()
        )
        |> ignore

        _progressTimer.Elapsed
        |> Observable.subscribe(fun _ ->
            let progressInfo = {
                ConstantActorCount = getConstantActorCount()
                OneTimeActorCount = getOneTimeActorCount()
                CurrentSimulation = _currentSimulation
            }
            _eventStream.OnNext(ProgressUpdated progressInfo)
        )
        |> ignore

    member _.Working = _schedulerTimer.Enabled

    member _.Start(isWarmUp) =
        _warmUp <- isWarmUp

        if _warmUp then
            _currentOperation <- OperationType.WarmUp
        else
            _currentOperation <- OperationType.Bombing

        start()

    member _.Stop() = stop()

    member _.EventStream = _eventStream :> IObservable<_>
    member _.Scenario = dep.Scenario
    member _.PublishStatsToCoordinator() = dep.ScenarioStatsActor.Publish(ActorMessage.PublishStatsToCoordinator)
    member _.GetRealtimeStats(duration) = getRealtimeStats duration
    member _.GetFinalStats() = getFinalStats()

    interface IDisposable with
        member _.Dispose() =
            stop()
            _eventStream.Dispose()
            _logger.Verbose $"{nameof ScenarioScheduler} disposed"

