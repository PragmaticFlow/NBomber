module internal NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler

open System
open System.Threading.Tasks

open Nessos.Streams
open FSharp.Control.Reactive

open NBomber
open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Statistics
open NBomber.Domain.Concurrency.ScenarioActor
open NBomber.Domain.Concurrency.Scheduler.ConstantActorScheduler
open NBomber.Domain.Concurrency.Scheduler.OneTimeActorScheduler

[<Struct>]
type SchedulerCommand =
    | AddConstantActors    of addCount:int
    | RemoveConstantActors of removeCount:int
    | InjectOneTimeActors  of scheduledCount:int
    | DoNothing

[<Struct>]
type ScenarioProgressInfo = {
    ConstantActorCount: int
    OneTimeActorCount: int
    CurrentSimulation: LoadSimulation
}

let calcScheduleByTime (copiesCount: int, prevSegmentCopiesCount: int, timeSegmentProgress: int) =
    let value = copiesCount - prevSegmentCopiesCount
    let result = (float value / 100.0 * float timeSegmentProgress) + float prevSegmentCopiesCount
    int(Math.Round(result, 0, MidpointRounding.AwayFromZero))

let schedule (timeSegment: LoadTimeSegment, timeSegmentProgress: int, constWorkingActorCount: int) =
    match timeSegment.LoadSimulation with
    | RampConcurrentScenarios (copiesCount, _) ->
        let scheduled = calcScheduleByTime(copiesCount, timeSegment.PrevSegmentCopiesCount, timeSegmentProgress)
        let scheduleNow = scheduled - constWorkingActorCount
        if scheduleNow > 0 then [AddConstantActors scheduleNow]
        elif scheduleNow < 0 then [RemoveConstantActors(Math.Abs scheduleNow)]
        else [DoNothing]

    | KeepConcurrentScenarios (copiesCount, _) ->
        if constWorkingActorCount < copiesCount then [AddConstantActors(copiesCount - constWorkingActorCount)]
        elif constWorkingActorCount > copiesCount then [RemoveConstantActors(constWorkingActorCount - copiesCount)]
        else [DoNothing]

    | RampScenariosPerSec (copiesCount, _) ->
        let scheduled = calcScheduleByTime(copiesCount, timeSegment.PrevSegmentCopiesCount, timeSegmentProgress)
        let command = InjectOneTimeActors(Math.Abs scheduled)
        if constWorkingActorCount > 0 then [RemoveConstantActors(constWorkingActorCount); command]
        else [command]

    | InjectScenariosPerSec (copiesCount, _) ->
        let command = InjectOneTimeActors(copiesCount)
        if constWorkingActorCount > 0 then [RemoveConstantActors(constWorkingActorCount); command]
        else [command]

let correctExecutionTime (executionTime: TimeSpan, scnDuration: TimeSpan) =
    if executionTime = TimeSpan.Zero || executionTime > scnDuration then scnDuration
    else executionTime

type ScenarioScheduler(dep: ActorDep) =

    [<Literal>]
    let SchedulerTickIntervalMs = 1_000.0

    let mutable _disposed = false
    let mutable _warmUp = false
    let mutable _scenario = dep.Scenario
    let mutable _currentSimulation = LoadSimulation.KeepConcurrentScenarios(0, TimeSpan.MinValue)

    let _constantScheduler = ConstantActorScheduler(dep)
    let _oneTimeScheduler = OneTimeActorScheduler(dep)
    let _timer = new System.Timers.Timer(SchedulerTickIntervalMs)
    let _progressInfoTimer = new System.Timers.Timer(Constants.SchedulerNotificationTickInterval.TotalMilliseconds)
    let _eventStream = Subject.broadcast
    let _tcs = TaskCompletionSource()

    let getAllActors () = _constantScheduler.AvailableActors @ _oneTimeScheduler.AvailableActors

    let start () =
        _timer.Start()
        _progressInfoTimer.Start()
        _tcs.Task :> Task

    let stop () =
        if not _disposed then
            _disposed <- true
            _timer.Stop()
            _progressInfoTimer.Stop()
            dep.GlobalTimer.Stop()
            _scenario <- Scenario.setExecutedDuration(_scenario, dep.GlobalTimer.Elapsed)
            _eventStream.OnCompleted()
            _eventStream.Dispose()
            _constantScheduler.Stop()
            _oneTimeScheduler.Stop()
            _tcs.TrySetResult() |> ignore
            true

        else false

    let getScenarioStats (duration) =
        let scnDuration = _scenario.ExecutedDuration |> Option.defaultValue(_scenario.PlanedDuration)
        let executionTime = correctExecutionTime(duration, scnDuration)

        getAllActors()
        |> Stream.ofList
        |> Stream.collect(fun x -> x.GetStepResults executionTime)
        |> RawScenarioStats.create dep.Scenario executionTime

    do
        _timer.Elapsed.Add(fun _ ->

            if not dep.GlobalTimer.IsRunning then dep.GlobalTimer.Restart()

            let currentTime = dep.GlobalTimer.Elapsed

            if _warmUp && dep.Scenario.WarmUpDuration <= currentTime then
                stop() |> ignore

            elif dep.CancellationToken.IsCancellationRequested then
                stop() |> ignore

            else
                match LoadTimeLine.getRunningTimeSegment(dep.Scenario.LoadTimeLine, currentTime) with
                | Some timeSegment ->

                    _currentSimulation <- timeSegment.LoadSimulation

                    let timeProgress =
                        timeSegment
                        |> LoadTimeLine.calcTimeSegmentProgress(currentTime)
                        |> LoadTimeLine.correctTimeProgress

                    schedule(timeSegment, timeProgress, _constantScheduler.WorkingActorCount)
                    |> List.iter(function
                        | AddConstantActors count    -> _constantScheduler.AddActors(count)
                        | RemoveConstantActors count -> _constantScheduler.RemoveActors(count)
                        | InjectOneTimeActors count  -> _oneTimeScheduler.InjectActors(count)
                        | DoNothing -> ()
                    )

                | None -> stop() |> ignore
        )

        _progressInfoTimer.Elapsed.Add(fun _ ->
            let progressInfo = {
                ConstantActorCount = _constantScheduler.WorkingActorCount
                OneTimeActorCount = _oneTimeScheduler.ScheduledActorCount
                CurrentSimulation = _currentSimulation
            }
            _eventStream.OnNext(progressInfo)
        )

    member x.Working = _timer.Enabled

    member x.Start(isWarmUp) =
        _warmUp <- isWarmUp
        start()

    member x.Stop() =
        stop()

    member x.EventStream = _eventStream :> IObservable<_>
    member x.Scenario = dep.Scenario
    member x.AllActors = getAllActors()
    member x.GetScenarioStats(duration) = getScenarioStats(duration)

    interface IDisposable with
        member x.Dispose() = stop() |> ignore
