module internal NBomber.Domain.Concurrency.Scheduler.OneTimeActorScheduler

open System

open NBomber.Domain.Concurrency
open NBomber.Domain.Concurrency.ScenarioActor

[<Struct>]
type SchedulerCommand =
    | StartActors of actors:ScenarioActor list
    | RentActors of actorCount:int

// dep * actorPool * scheduledActorCount
type SchedulerExec = ActorDep -> ScenarioActor list -> int -> ScenarioActor list

// todo: add tests
let schedule (actorPool: ScenarioActor list) (actorCount: int) =
    let freeActors =
        actorPool
        |> List.filter(fun x -> not x.Working)
        |> List.truncate actorCount

    if freeActors.Length < actorCount then
        RentActors actorCount
    else
        StartActors freeActors

let exec (dep: ActorDep) (actorPool: ScenarioActor list) (scheduledActorCount: int) =

    let execSteps (actors: ScenarioActor list) =
        actors |> List.iter(fun x -> x.ExecSteps() |> ignore)

    match schedule actorPool scheduledActorCount with
    | StartActors actors ->
        execSteps actors
        actorPool

    | RentActors actorCount ->
        let result = ScenarioActorPool.rentActors (ScenarioActorPool.createActors dep) actorPool actorCount
        execSteps result.ActorsFromPool
        execSteps result.NewActors
        ScenarioActorPool.updatePool actorPool result.NewActors

type OneTimeActorScheduler(dep: ActorDep, exec: SchedulerExec) =

    let _lockObj = obj()
    let mutable _actorPool = List.empty<ScenarioActor>
    let mutable _scheduledActorCount = 0

    let stop () = ScenarioActorPool.stopActors _actorPool

    member _.ScheduledActorCount = _scheduledActorCount
    member _.AvailableActors = _actorPool

    member _.InjectActors(count) =
        lock _lockObj (fun _ ->
            _scheduledActorCount <- count
            _actorPool <- exec dep _actorPool _scheduledActorCount
        )

    member _.Stop() = stop()

    interface IDisposable with
        member _.Dispose() = stop()
