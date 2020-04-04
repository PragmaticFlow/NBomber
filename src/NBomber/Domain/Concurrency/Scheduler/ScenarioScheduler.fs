module internal NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler

open System
open System.Threading.Tasks

open FSharp.Control.Reactive

open NBomber
open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.Concurrency.ScenarioActor
open NBomber.Domain.Concurrency.Scheduler.ConstantActorScheduler
open NBomber.Domain.Concurrency.Scheduler.OneTimeActorScheduler

[<Struct>]
type ScenarioProgressInfo = {
    ConstantActorCount: int
    OneTimeActorCount: int
}

[<Struct>]
type SchedulerCommand =
    | AddConstantActors    of addCount:int
    | RemoveConstantActors of removeCount:int
    | InjectOneTimeActors  of scheduledCount:int * perSecCount:int
    | DoNothing

let calcScheduleTickProgress (scheduleTickInterval: float) =
    if scheduleTickInterval < 1000.0 then 1000.0 / scheduleTickInterval
    else scheduleTickInterval / 1000.0

let calcScheduledCount (actorsCount: int, scheduleTickProgress: float, during: TimeSpan) =
    let actorsPerSec = float actorsCount / during.TotalSeconds
    int(actorsPerSec / scheduleTickProgress)

let schedule (scheduleTickIntervalMs,
              constWorkingActorCount, oneTimeActorPerSecCount, simulation: LoadSimulation) =

    let scheduleTickProgress = calcScheduleTickProgress(scheduleTickIntervalMs)

    match simulation with
    | KeepConcurrentScenarios (copiesCount, _) ->
        if constWorkingActorCount < copiesCount then [AddConstantActors(copiesCount - constWorkingActorCount)]
        elif constWorkingActorCount > copiesCount then [RemoveConstantActors(constWorkingActorCount - copiesCount)]
        else [DoNothing]

    | RampConcurrentScenarios (copiesCount, during) ->
        if constWorkingActorCount < copiesCount then
            let actorsToStart = copiesCount - constWorkingActorCount
            let scheduled = calcScheduledCount(actorsToStart, scheduleTickProgress, during)
            if scheduled = 0 then [AddConstantActors 1]
            else [AddConstantActors scheduled]

        elif constWorkingActorCount > copiesCount then
            let actorsToStop = constWorkingActorCount - copiesCount
            let scheduled = calcScheduledCount(actorsToStop, scheduleTickProgress, during)
            if scheduled = 0 then [RemoveConstantActors 1]
            else [RemoveConstantActors scheduled]

        else [DoNothing]

    | InjectScenariosPerSec (perSecCount, _) ->
        let scheduled = perSecCount / int scheduleTickProgress
        let command = [InjectOneTimeActors(scheduled, perSecCount)]

        if constWorkingActorCount > 0 then [RemoveConstantActors(constWorkingActorCount)] @ command
        else command

    | RampScenariosPerSec (perSecCount, during) ->
        let actorsToStart = Math.Abs(perSecCount - oneTimeActorPerSecCount)
        let scheduled = calcScheduledCount(actorsToStart, scheduleTickProgress, during)

        let command =
            if perSecCount > oneTimeActorPerSecCount then
                let scheduledCount = oneTimeActorPerSecCount + scheduled
                [InjectOneTimeActors(scheduledCount, scheduledCount)]

            elif perSecCount < oneTimeActorPerSecCount then
                let scheduledCount = oneTimeActorPerSecCount - scheduled
                [InjectOneTimeActors(scheduledCount, scheduledCount)]

            else [InjectOneTimeActors(scheduled, perSecCount)]

        if constWorkingActorCount > 0 then [RemoveConstantActors(constWorkingActorCount)] @ command
        else command

let correctExecutionTime (executionTime: TimeSpan, scnDuration: TimeSpan) =
    if executionTime = TimeSpan.Zero || executionTime > scnDuration then scnDuration
    else executionTime

type ScenarioScheduler(dep: ActorDep) =

    let mutable _disposed = false
    let mutable _warmUp = false
    let mutable _scenario = dep.Scenario
    let _constantScheduler = ConstantActorScheduler(dep)
    let _oneTimeScheduler = OneTimeActorScheduler(dep)
    let _timer = new System.Timers.Timer(float Constants.SchedulerTickIntervalMs)
    let _progressInfoTimer = new System.Timers.Timer(float Constants.NotificationTickIntervalMs)
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
        |> List.collect(fun x -> x.GetStepResults executionTime)
        |> Seq.toArray
        |> Statistics.ScenarioStats.create dep.Scenario executionTime

    do
        _timer.Elapsed.Add(fun _ ->

            if not dep.GlobalTimer.IsRunning then dep.GlobalTimer.Restart()

            if _warmUp && dep.Scenario.WarmUpDuration <= dep.GlobalTimer.Elapsed then
                stop() |> ignore

            elif dep.CancellationToken.IsCancellationRequested then
                stop() |> ignore

            else
                match LoadTimeLine.getRunningSimulation(dep.Scenario.LoadTimeLine, dep.GlobalTimer.Elapsed) with
                | Some simulation ->

                    schedule(_timer.Interval,
                             _constantScheduler.WorkingActorCount, _oneTimeScheduler.ActorPerSecCount, simulation)

                    |> List.iter(fun command ->
                        match command with
                        | AddConstantActors count    -> _constantScheduler.AddActors(count)
                        | RemoveConstantActors count -> _constantScheduler.RemoveActors(count)
                        | InjectOneTimeActors (count,perSecCount) -> _oneTimeScheduler.InjectActors(count, perSecCount)
                        | DoNothing -> ()
                    )

                | None -> stop() |> ignore
        )

        _progressInfoTimer.Elapsed.Add(fun _ ->
            let progressInfo = {
                ConstantActorCount = _constantScheduler.WorkingActorCount
                OneTimeActorCount = _oneTimeScheduler.ActorPerSecCount
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
