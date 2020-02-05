module internal NBomber.Domain.Concurrency.ScenarioScheduler

open System
open System.Threading.Tasks

open FsToolkit.ErrorHandling
open FSharp.Control.Tasks.V2.ContextInsensitive

open System.Collections.Generic
open NBomber.Extensions
open NBomber.Contracts
open NBomber.Domain.Concurrency.ScenarioActor
open NBomber.Domain.Concurrency.ScenarioActorsPool

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

type ActorTaskId = int

type ActorTask = {
    Actor: ScenarioActor
    mutable Task: Task<unit>
}

type ConstantActorsScheduler(actorsPool: ScenarioActorsPool) =

    let mutable _runningActors = Dictionary<ActorTaskId, ActorTask>()
    let _schedulerName = "ConstantActorsScheduler"

    let startActors (actors: ResizeArray<ScenarioActor>) =
        actors |> ResizeArray.map(fun x -> { Actor = x; Task = x.ExecSteps() })

    let addToRunning (runningActors: Dictionary<ActorTaskId, ActorTask>) (actors: ResizeArray<ActorTask>) =
        actors |> ResizeArray.iter(fun x -> runningActors.[x.Task.Id] <- x)
        runningActors

    let startMonitorActors () =
        //todo: make it long run task
        task {
            do! Task.Yield()
            let cancelToken = actorsPool.Dep.CancellationToken

            while not cancelToken.IsCancellationRequested do
                let! finishedTask = Task.WhenAny(_runningActors.Values |> Seq.map(fun x -> x.Task))

                let item = _runningActors.[finishedTask.Id]
                item.Task <- item.Actor.ExecSteps()

                _runningActors.Remove(finishedTask.Id) |> ignore
                _runningActors.[item.Task.Id] <- item

            let allTasks = _runningActors.Values |> Seq.map(fun x -> x.Task :> Task)
            do! Task.WhenAll(allTasks)
        }

    member x.Start() =
        startMonitorActors() |> ignore

    member x.AddActors(count) =
        _runningActors <-
            actorsPool.RentActors(_schedulerName, count)
            |> startActors
            |> addToRunning _runningActors

    member x.RemoveActors(count) =
        ()
//
//type OneTimeActorsPool(genPool: GeneralActorsPool) =
//
//    member x.AddActors(count) =
//        ()
//
//    member x.RemoveActors(count) =
//        ()
