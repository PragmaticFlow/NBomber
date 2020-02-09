module internal NBomber.Domain.Concurrency.ScenarioScheduler

open System.Collections.Generic
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open FSharpx.Collections

open NBomber.Extensions
open NBomber.Contracts
open NBomber.Domain.Concurrency.ScenarioActor
open NBomber.Domain.Concurrency.ScenarioActorPool

type SchedulerResult =
    | AddConstantActors    of count:uint32
    | RemoveConstantActors of count:uint32
    | AddOneTimeActors     of count:uint32
    | DoNothing

let schedule (invokeSchedulePerSec: uint32,
              constantRunCopiesCount: uint32,
              runningStrategy: ConcurrencyStrategy) =

    match runningStrategy with
    | Constant (copiesCount, duration) ->
        if constantRunCopiesCount < copiesCount then [AddConstantActors(copiesCount - constantRunCopiesCount)]
        elif constantRunCopiesCount > copiesCount then [RemoveConstantActors(constantRunCopiesCount - copiesCount)]
        else [DoNothing]

    | AddPerSec (copiesCount, duration) ->
        [RemoveConstantActors(constantRunCopiesCount)
         AddOneTimeActors(copiesCount / invokeSchedulePerSec)]

    | RampTo (copiesCount, duration) ->
        if constantRunCopiesCount < copiesCount then
            let copiesToStart = copiesCount - constantRunCopiesCount
            let copiesPerSec = copiesToStart / uint32 duration.TotalSeconds
            [AddConstantActors(copiesPerSec / invokeSchedulePerSec)]

        elif constantRunCopiesCount > copiesCount then
            let copiesToStop = constantRunCopiesCount - copiesCount
            let copiesPerSec = copiesToStop / uint32 duration.TotalSeconds
            [RemoveConstantActors(copiesPerSec / invokeSchedulePerSec)]

        else [DoNothing]

type ActorTask = Task

type ConstantActorsScheduler(actorsPool: ScenarioActorPool) =

    let _runningActors = Dictionary<ActorTask,ScenarioActor>()
    let _schedulerName = "ConstantActorsScheduler"
    let mutable _scheduledActorsCount = 0
    let mutable _stop = false

    let addToScheduler (actorsCount) =
        _scheduledActorsCount <- _scheduledActorsCount + actorsCount

        actorsPool.RentActors(_schedulerName, actorsCount)
        |> ResizeArray.iter(fun actor ->
            let startedActorTask = actor.ExecSteps()
            _runningActors.[startedActorTask] <- actor
        )

    let removeFromScheduler (actorsCount) =
        let actorsCount = _scheduledActorsCount - actorsCount
        if actorsCount < 0 then _scheduledActorsCount <- 0
        else _scheduledActorsCount <- _scheduledActorsCount - actorsCount

    let startScheduler () =

        let shouldStartActor (runningActors: Dictionary<ActorTask,ScenarioActor>, scheduledActorsCount)  =
            scheduledActorsCount > runningActors.Count

        _stop <- false

        task {
            do! Task.Yield()
            let cancelToken = actorsPool.Dep.CancellationToken

            while not _stop || not cancelToken.IsCancellationRequested do
                if _runningActors.Count > 0 then
                    let! finishedTask = Task.WhenAny(_runningActors.Keys)

                    let actor = _runningActors.[finishedTask]
                    _runningActors.Remove(finishedTask) |> ignore

                    if shouldStartActor(_runningActors, _scheduledActorsCount) then
                        let startedActorTask = actor.ExecSteps()
                        _runningActors.[startedActorTask] <- actor
                    else
                        actorsPool.ReleaseActor(_schedulerName, actor)
                else
                    _stop <- true
        }

    let stopScheduler () =
        _stop <- true
        actorsPool.ReleaseActors(_schedulerName, _runningActors.Values)
        _runningActors.Clear()

    member x.RunningActors = _runningActors :> IReadOnlyDictionary<_,_>

    member x.Start() =
        startScheduler() |> ignore

    member x.Stop() =
        stopScheduler()

    member x.AddActors(count) =
        addToScheduler(count)
        if _stop = true then x.Start()

    member x.RemoveActors(count) =
        removeFromScheduler count

type OneTimeActorsPool(actorsPool: ScenarioActorPool) =

    let _runningActors = ResizeArray<ScenarioActor>()
    let _schedulerName = "OneTimeActorsPool"

    let startActors (actorsCount) =
        let freeActors =
            _runningActors
            |> Seq.filter(fun x -> not(x.Working))
            |> Seq.truncate(actorsCount)
            |> Seq.toArray

        freeActors |> Array.iter(fun x -> x.ExecSteps() |> ignore)

        if freeActors.Length < actorsCount then
            let count = actorsCount - freeActors.Length
            let newActors = actorsPool.RentActors(_schedulerName, count)
            newActors |> ResizeArray.iter(fun x -> x.ExecSteps() |> ignore)
            _runningActors.AddRange(newActors)

    let releaseActors () =
        actorsPool.ReleaseActors(_schedulerName, _runningActors)
        _runningActors.Clear()

    member x.StartActors(count) =
        startActors(count)

    member x.Stop() =
        releaseActors()
