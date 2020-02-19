module internal NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler

open FSharp.Control.Reactive

open System
open System.Threading.Tasks
open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.Concurrency
open NBomber.Domain.Concurrency.ScenarioActor
open NBomber.Domain.Concurrency.Scheduler.ConstantActorScheduler
open NBomber.Domain.Concurrency.Scheduler.OneTimeActorScheduler
open NBomber.Domain.Statistics

[<Struct>]
type SchedulerNotification = {
    ConstantActorsCount: int
    OneTimeActorsCount: int
}

[<Struct>]
type SchedulerCommand =
    | AddConstantActors    of addCount:int
    | RemoveConstantActors of removeCount:int
    | StartOneTimeActors   of addOneTimeCount:int
    | DoNothing

let schedule (scheduleTickInterval, constantActorsCount, simulation: LoadSimulation) =

    let schedulePerSecCount = 1000 / scheduleTickInterval

    match simulation with
    | KeepConstant (copiesCount, _) ->
        if constantActorsCount < copiesCount then [AddConstantActors(copiesCount - constantActorsCount)]
        elif constantActorsCount > copiesCount then [RemoveConstantActors(constantActorsCount - copiesCount)]
        else [DoNothing]

    | InjectPerSec (copiesCount, _) ->
        if constantActorsCount > 0 then
            [RemoveConstantActors(constantActorsCount)
             StartOneTimeActors(copiesCount / schedulePerSecCount)]
        else
            [StartOneTimeActors(copiesCount / schedulePerSecCount)]

    | RampTo (copiesCount, duration) ->
        if constantActorsCount < copiesCount then
            let copiesToStart = copiesCount - constantActorsCount
            let copiesPerSec = copiesToStart / int duration.TotalSeconds
            [AddConstantActors(copiesPerSec / schedulePerSecCount)]

        elif constantActorsCount > copiesCount then
            let copiesToStop = constantActorsCount - copiesCount
            let copiesPerSec = copiesToStop / int duration.TotalSeconds
            [RemoveConstantActors(copiesPerSec / schedulePerSecCount)]

        else [DoNothing]

type ScenarioScheduler(dep: ActorDep) =

    let mutable _warmUp = false
    let _constantScheduler = ConstantActorScheduler(dep)
    let _oneTimeScheduler = OneTimeActorScheduler(dep)
    let _timer = new System.Timers.Timer(Constants.ScheduleTickInterval)
    let _notificationTimer = new System.Timers.Timer(Constants.NotificationTickInterval)
    let _notificationStream = Subject.broadcast
    let _tcs = TaskCompletionSource()

    let getAllActors () = _constantScheduler.AvailableActors @ _oneTimeScheduler.AvailableActors

    let start () =
        _timer.Start()
        _notificationTimer.Start()
        _tcs.Task :> Task

    let stop () =
        try
            _timer.Stop()
            _notificationTimer.Stop()
            dep.GlobalTimer.Stop()
            _notificationStream.OnCompleted()
            _constantScheduler.Stop()
            _oneTimeScheduler.Stop()
            _tcs.TrySetResult() |> ignore
        with
        | ex -> ()

    let getScenarioStats () =
        getAllActors()
        |> Seq.collect(fun x -> x.GetStepResults dep.Scenario.Duration)
        |> Seq.toArray
        |> ScenarioStats.create dep.Scenario dep.Scenario.Duration

    do
        _timer.Elapsed.Add(fun _ ->

            //_timer.Stop()

            if not dep.GlobalTimer.IsRunning then dep.GlobalTimer.Restart()

            if _warmUp && dep.Scenario.WarmUpDuration <= dep.GlobalTimer.Elapsed then
                stop()

            elif dep.CancellationToken.IsCancellationRequested then
                stop()

            else
                match LoadSimulation.getRunningSimulation(dep.Scenario.LoadTimeLine, dep.GlobalTimer.Elapsed) with
                | Some simulation ->
                    schedule(int _timer.Interval, _constantScheduler.ScheduledActorCount, simulation)
                    |> List.iter(fun command ->
                        match command with
                        | AddConstantActors count    -> _constantScheduler.AddActors(count)
                        | RemoveConstantActors count -> _constantScheduler.RemoveActors(count)
                        | StartOneTimeActors count   -> _oneTimeScheduler.StartActors(count)
                        | DoNothing                  -> ()
                    )

                | None -> stop()
        )

        _notificationTimer.Elapsed.Add(fun _ ->
            let notification = {
                ConstantActorsCount = _constantScheduler.ScheduledActorCount
                OneTimeActorsCount = _oneTimeScheduler.ScheduledActorCount
            }
            _notificationStream.OnNext(notification)
        )

    member x.Working = _timer.Enabled

    member x.WarmUp() =
        _warmUp <- true
        start()

    member x.Start() =
        _warmUp <- false
        start()

    member x.Stop() = stop()
    member x.NotificationStream = _notificationStream :> IObservable<_>
    member x.Scenario = dep.Scenario
    member x.AllActors = getAllActors()
    member x.GetScenarioStats() = getScenarioStats()

    interface IDisposable with
        member x.Dispose() = stop()
