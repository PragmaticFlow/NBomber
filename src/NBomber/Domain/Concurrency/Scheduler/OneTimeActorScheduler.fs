module internal NBomber.Domain.Concurrency.Scheduler.OneTimeActorScheduler

open System

open NBomber.Domain.Concurrency
open NBomber.Domain.Concurrency.ScenarioActor

[<Struct>]
type SchedulerCommand =
    | StartActors of actors:ScenarioActor list
    | RentActors of actorCount:int

let schedule (actorPool: ScenarioActor list) (actorCount: int) =
    let freeActors =
        actorPool
        |> List.filter(fun x -> not x.Working)
        |> List.truncate actorCount

    if freeActors.Length < actorCount then
        RentActors actorCount
    else
        StartActors freeActors

type OneTimeActorScheduler(dep: ActorDep) =

    let _lockObj = obj()
    let mutable _actorPool = List.empty<ScenarioActor>
    let mutable _scheduledActorCount = 0
    let createActors = ScenarioActorPool.createActors dep

    let stop () =
        ScenarioActorPool.stopActors _actorPool
        _scheduledActorCount <- 0

    let execScheduler (scheduledActorCount: int) =

        let exec (actors: ScenarioActor list) =
            actors |> List.iter(fun x -> x.ExecSteps() |> ignore)

        match schedule _actorPool scheduledActorCount with
        | StartActors actors -> exec actors
        | RentActors actorCount ->
            let result = ScenarioActorPool.rentActors createActors _actorPool actorCount
            exec result.ActorsFromPool
            exec result.NewActors
            _actorPool <- ScenarioActorPool.updatePool _actorPool result

    member _.ScheduledActorCount = _scheduledActorCount
    member _.AvailableActors = _actorPool

    member _.InjectActors(count) =
        lock _lockObj (fun _ ->
            _scheduledActorCount <- count
            execScheduler _scheduledActorCount
        )

    member _.Stop() = stop()

    interface IDisposable with
        member _.Dispose() = stop()
