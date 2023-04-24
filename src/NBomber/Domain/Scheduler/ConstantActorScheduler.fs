module internal NBomber.Domain.Scheduler.ConstantActorScheduler

open System
open NBomber.Domain.Concurrency
open NBomber.Domain.Concurrency.ScenarioActor
open NBomber.Domain.ScenarioContext

[<Struct>]
type SchedulerCommand =
    | KeepWorking
    | AddActors of addCount:int
    | RemoveActor of removeCount:int
    | StopScheduler

/// (createActors: count -> fromIndex -> ScenarioActor[]) (actorPool) (scheduledActorCount) (injectInterval)
type SchedulerExec = (int -> int -> ScenarioActor[]) -> ResizeArray<ScenarioActor> -> int -> TimeSpan -> ResizeArray<ScenarioActor>

let inline removeFromScheduler scheduledActorsCount removeCount =
    let actorsCount = scheduledActorsCount - removeCount
    if actorsCount < 0 then 0
    else actorsCount

// todo: add tests
let inline schedule workingActorCount scheduledActorCount =
    if scheduledActorCount = 0 then StopScheduler
    elif workingActorCount = scheduledActorCount then KeepWorking
    elif workingActorCount < scheduledActorCount then AddActors(scheduledActorCount - workingActorCount)
    else RemoveActor(workingActorCount - scheduledActorCount)

let inline exec
    (createActors: int -> int -> ScenarioActor[])
    (actorPool: ResizeArray<ScenarioActor>)
    (scheduledActorCount: int)
    (injectInterval: TimeSpan) =

    let workingActors = ScenarioActorPool.getWorkingActors actorPool
    match schedule (Seq.length workingActors) scheduledActorCount with
    | KeepWorking ->
        actorPool

    | AddActors count ->
        let result = ScenarioActorPool.rentActors createActors actorPool count

        result.ActorsFromPool |> Array.iter(fun x -> x.RunInfinite(injectInterval) |> ignore)
        result.NewActors |> Array.iter(fun x -> x.RunInfinite(injectInterval) |> ignore)

        ScenarioActorPool.updatePool actorPool result.NewActors

    | RemoveActor count ->
        workingActors
        |> Seq.take count
        |> Seq.iter(fun x -> (x :> IDisposable).Dispose())
        actorPool

    | StopScheduler ->
        ScenarioActorPool.askToStop actorPool
        actorPool

type ConstantActorScheduler(scnCtx: ScenarioContextArgs, exec: SchedulerExec) =

    let mutable _actorPool = ResizeArray<ScenarioActor>()
    let mutable _scheduledActorCount = 0
    let createActors = ScenarioActorPool.createActors scnCtx

    member this.ScheduledActorCount = _scheduledActorCount
    member this.AvailableActors = _actorPool

    member this.AddActors(count, injectInterval) =
        _scheduledActorCount <- _scheduledActorCount + count
        _actorPool           <- exec createActors _actorPool _scheduledActorCount injectInterval

    member this.RemoveActors(count) =
        _scheduledActorCount <- removeFromScheduler _scheduledActorCount count
        _actorPool           <- exec createActors _actorPool _scheduledActorCount TimeSpan.Zero

    interface IDisposable with
        member this.Dispose() = ScenarioActorPool.askToStop _actorPool

module Test =

    let exec createActors actorPool scheduledActorCount injectInterval = exec createActors actorPool scheduledActorCount injectInterval
