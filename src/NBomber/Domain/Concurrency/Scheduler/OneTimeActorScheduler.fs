module internal NBomber.Domain.Concurrency.Scheduler.OneTimeActorScheduler

open NBomber.Domain.Concurrency
open NBomber.Domain.Concurrency.ScenarioActor

[<Struct>]
type SchedulerCommand =
    | StartActors of actors:ScenarioActor list
    | RentActors of actorCount:int

let schedule (actorPool: ScenarioActor list, actorCount) =
    let freeActors =
        actorPool
        |> List.filter(fun x -> not x.Working && x.Reserved)
        |> List.truncate(actorCount)

    if freeActors.Length < actorCount then
        RentActors(actorCount)
    else
        StartActors(freeActors)

type OneTimeActorScheduler(dep: ActorDep) =

    let mutable _actorPool = List.empty<ScenarioActor>
    let mutable _scheduledActorCount = 0

    let startActors (currentPool, scheduledActorCount) =

        let exec (actors: ScenarioActor list) =
            actors |> List.iter(fun x -> x.ExecSteps() |> ignore)

        match schedule(currentPool, scheduledActorCount) with
        | StartActors actors ->
            exec(actors)
            currentPool

        | RentActors actorCount ->
            let result = ScenarioActorPool.rentActors(dep, currentPool, actorCount)
            exec(result.ActorsFromPool)
            exec(result.NewActors)
            ScenarioActorPool.updatePool(currentPool, result)

    let stop () =
        _scheduledActorCount <- 0
        ScenarioActorPool.releaseActors(_actorPool)

    member x.ScheduledActorCount = _scheduledActorCount
    member x.AvailableActors = _actorPool

    member x.InjectActors(scheduledCount) =
        _scheduledActorCount <- scheduledCount
        _actorPool <- startActors(_actorPool, _scheduledActorCount)

    member x.Stop() = stop()
