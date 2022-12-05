module internal NBomber.Domain.Scheduler.OneTimeActorScheduler

open System
open NBomber.Domain.Concurrency
open NBomber.Domain.Concurrency.ScenarioActor
open NBomber.Domain.ScenarioContext

[<Struct>]
type SchedulerCommand =
    | StartActors of actors:ScenarioActor[]
    | RentActors of actorCount:int

/// (createActors: count -> fromIndex -> ScenarioActor[]) * actorPool * scheduledActorCount
type SchedulerExec = (int -> int -> ScenarioActor[]) -> ResizeArray<ScenarioActor> -> int -> ResizeArray<ScenarioActor>

// todo: add tests
let inline schedule (actorPool: ScenarioActor seq) (actorCount: int) =
    let freeActors =
        actorPool
        |> Seq.filter(fun x -> not x.Working)
        |> Seq.truncate actorCount
        |> Seq.toArray

    if freeActors.Length < actorCount then
        RentActors actorCount
    else
        StartActors freeActors

let inline exec
    (createActors: int -> int -> ScenarioActor[])
    (actorPool: ResizeArray<ScenarioActor>)
    (scheduledActorCount: int) =

    let inline execSteps (actors: ScenarioActor[]) =
        actors |> Array.iter(fun x -> x.ExecSteps() |> ignore)

    match schedule actorPool scheduledActorCount with
    | StartActors actors ->
        execSteps actors
        actorPool

    | RentActors actorCount ->
        let result = ScenarioActorPool.rentActors createActors actorPool actorCount
        execSteps result.ActorsFromPool
        execSteps result.NewActors
        ScenarioActorPool.updatePool actorPool result.NewActors

type OneTimeActorScheduler(scnCtx: ScenarioContextArgs, exec: SchedulerExec) =

    let mutable _actorPool = ResizeArray<ScenarioActor>()
    let mutable _scheduledActorCount = 0
    let createActors = ScenarioActorPool.createActors scnCtx

    let stop () = ScenarioActorPool.stopActors _actorPool

    member _.ScheduledActorCount = _scheduledActorCount
    member _.AvailableActors = _actorPool

    member _.InjectActors(count) =
        _scheduledActorCount <- count
        _actorPool <- exec createActors _actorPool _scheduledActorCount

    member _.Stop() = stop()

    interface IDisposable with
        member _.Dispose() = stop()

module Test =

    let exec createActors actorPool scheduledActorCount = exec createActors actorPool scheduledActorCount
