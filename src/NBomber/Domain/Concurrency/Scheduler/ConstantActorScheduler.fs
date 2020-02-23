module internal NBomber.Domain.Concurrency.Scheduler.ConstantActorScheduler

open System.Collections.Generic
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Domain.Concurrency
open NBomber.Domain.Concurrency
open NBomber.Domain.Concurrency.ScenarioActor

type ActorTask = Task

[<Struct>]
type SchedulerCommand =
    | KeepWorking
    | ReleaseFinishedActor
    | AddActors of count:int
    | StopScheduler

let removeFromScheduler (scheduledActorsCount, removeCount) =
    let actorsCount = scheduledActorsCount - removeCount
    if actorsCount < 0 then 0
    else actorsCount

let schedule (workingActorCount, scheduledActorCount) =
    if scheduledActorCount = 0 then StopScheduler
    elif workingActorCount = scheduledActorCount then KeepWorking
    elif workingActorCount < scheduledActorCount then AddActors(scheduledActorCount - workingActorCount)
    else ReleaseFinishedActor

//let schedule (workingActorCount, scheduledActorsCount) = task {
//
//    if scheduledActorsCount = 0 || workingActorCount = 0 then
//        return StopScheduler
//    else
//        let! finishedTask = Task.WhenAny(workingActors.Keys)
//        let actor = workingActors.[finishedTask]
//        workingActors.Remove(finishedTask) |> ignore
//
//        return if shouldStartActor(workingActors.Count, scheduledActorsCount)
//               then StartActor(actor)
//               else ReleaseActor(actor)
//}

type ConstantActorScheduler(dep: ActorDep) =

    let mutable _actorPool = List.empty<ScenarioActor>
    let _workingActors = Dictionary<ActorTask,ScenarioActor>()
    let mutable _scheduledActorCount = 0
    let mutable _stop = true

    let addToScheduler (currentPool, actorCount) =

        let exec (actors: ScenarioActor list) =
            actors
            |> List.iter(fun actor ->
                let startedActorTask = actor.ExecSteps()
                _workingActors.[startedActorTask] <- actor
            )

        let result = ScenarioActorPool.rentActors(dep, currentPool, actorCount)
        exec(result.ActorsFromPool)
        exec(result.NewActors)

        {| ActorPool = ScenarioActorPool.updatePool(currentPool, result)
           ScheduledActorCount = _scheduledActorCount + actorCount |}

    let stop () =
        _stop <- true
        _scheduledActorCount <- 0
        ScenarioActorPool.releaseActors(_actorPool)

    let startScheduler () =

        _stop <- false

        let waitActors (workingActors: Dictionary<ActorTask,ScenarioActor>) = task {
            let! finishedTask = Task.WhenAny(workingActors.Keys)
            let finishedActor = workingActors.[finishedTask]
            workingActors.Remove(finishedTask) |> ignore
            return finishedActor
        }

        let addAndExecActor (actor: ScenarioActor) =
            let startedActorTask = actor.ExecSteps()
            _workingActors.[startedActorTask] <- actor

        task {
            do! Task.Yield()

            while not _stop || not dep.CancellationToken.IsCancellationRequested do

                try
                    match schedule(_workingActors.Count, _scheduledActorCount) with
                    | KeepWorking          ->
                        let! finishedActor = waitActors(_workingActors)
                        addAndExecActor(finishedActor)

                    | ReleaseFinishedActor ->
                        let! finishedActor = waitActors(_workingActors)
                        finishedActor.ReserveForScheduler()

                    | AddActors count ->
                        let result = ScenarioActorPool.rentActors(dep, _actorPool, count)
                        _actorPool <- ScenarioActorPool.updatePool(_actorPool, result)
                        result.ActorsFromPool @ result.NewActors |> List.iter(addAndExecActor)

                    | StopScheduler -> stop()
                with
                | ex -> dep.Logger.Error(ex.ToString())

            stop()
        }

    member x.AvailableActors = _actorPool
    member x.WorkingActorCount = _scheduledActorCount
    member x.Stop() = stop()

    member x.AddActors(count) =
        _scheduledActorCount <- _scheduledActorCount + count
        if _stop = true then startScheduler() |> ignore

    member x.RemoveActors(count) =
        _scheduledActorCount <- removeFromScheduler(_scheduledActorCount, count)
