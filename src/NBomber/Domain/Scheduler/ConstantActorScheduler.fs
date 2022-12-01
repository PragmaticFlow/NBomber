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

// scnCtx * actorPool * scheduledActorCount
type SchedulerExec = ScenarioContextArgs -> ScenarioActor list -> int -> ScenarioActor list

let removeFromScheduler scheduledActorsCount removeCount =
    let actorsCount = scheduledActorsCount - removeCount
    if actorsCount < 0 then 0
    else actorsCount

// todo: add tests
let schedule workingActorCount scheduledActorCount =
    if scheduledActorCount = 0 then StopScheduler
    elif workingActorCount = scheduledActorCount then KeepWorking
    elif workingActorCount < scheduledActorCount then AddActors(scheduledActorCount - workingActorCount)
    else RemoveActor(workingActorCount - scheduledActorCount)

let exec (scnCtx: ScenarioContextArgs) (actorPool: ScenarioActor list) (scheduledActorCount: int) =
    let workingActors = ScenarioActorPool.getWorkingActors actorPool
    match schedule workingActors.Length scheduledActorCount with
    | KeepWorking ->
        actorPool

    | AddActors count ->
        let result = ScenarioActorPool.rentActors (ScenarioActorPool.createActors scnCtx) actorPool count
        let newActorPool = ScenarioActorPool.updatePool actorPool result.NewActors
        result.ActorsFromPool |> List.iter(fun x -> x.RunInfinite() |> ignore)
        result.NewActors|> List.iter(fun x -> x.RunInfinite() |> ignore)
        newActorPool

    | RemoveActor count ->
        workingActors
        |> List.take count
        |> List.iter(fun x -> x.Stop())
        actorPool

    | StopScheduler ->
        ScenarioActorPool.stopActors actorPool
        actorPool

type ConstantActorScheduler(scnCtx: ScenarioContextArgs, exec: SchedulerExec) =

    let mutable _actorPool = List.empty<ScenarioActor>
    let mutable _scheduledActorCount = 0

    let stop () = ScenarioActorPool.stopActors _actorPool

    member _.ScheduledActorCount = _scheduledActorCount
    member _.AvailableActors = _actorPool

    member _.AddActors(count) =
        _scheduledActorCount <- _scheduledActorCount + count
        _actorPool <- exec scnCtx _actorPool _scheduledActorCount

    member _.RemoveActors(count) =
        _scheduledActorCount <- removeFromScheduler _scheduledActorCount count
        _actorPool <- exec scnCtx _actorPool _scheduledActorCount

    member _.Stop() = stop()

    interface IDisposable with
        member _.Dispose() = stop()
