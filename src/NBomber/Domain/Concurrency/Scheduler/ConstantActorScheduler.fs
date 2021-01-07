module internal NBomber.Domain.Concurrency.Scheduler.ConstantActorScheduler

open System

open NBomber.Domain.Concurrency
open NBomber.Domain.Concurrency.ScenarioActor

[<Struct>]
type SchedulerCommand =
    | KeepWorking
    | AddActors of addCount:int
    | RemoveActor of removeCount:int
    | StopScheduler

let removeFromScheduler scheduledActorsCount removeCount =
    let actorsCount = scheduledActorsCount - removeCount
    if actorsCount < 0 then 0
    else actorsCount

let schedule workingActorCount scheduledActorCount =
    if scheduledActorCount = 0 then StopScheduler
    elif workingActorCount = scheduledActorCount then KeepWorking
    elif workingActorCount < scheduledActorCount then AddActors(scheduledActorCount - workingActorCount)
    else RemoveActor(workingActorCount - scheduledActorCount)

type ConstantActorScheduler(dep: ActorDep) =

    let mutable _actorPool = List.empty<ScenarioActor>
    let mutable _scheduledActorCount = 0
    let createActors = ScenarioActorPool.createActors dep

    let stop () =
        ScenarioActorPool.stopActors _actorPool
        _scheduledActorCount <- 0

    let execScheduler (scheduledActorCount) =
        let workingActors = ScenarioActorPool.getWorkingActors _actorPool
        match schedule workingActors.Length scheduledActorCount with
        | KeepWorking -> ()
        | AddActors count ->
            let result = ScenarioActorPool.rentActors createActors _actorPool count
            _actorPool <- ScenarioActorPool.updatePool _actorPool result
            result.ActorsFromPool |> List.iter(fun x -> x.RunInfinite() |> ignore)
            result.NewActors|> List.iter(fun x -> x.RunInfinite() |> ignore)

        | RemoveActor count ->
            workingActors
            |> List.take count
            |> List.iter(fun x -> x.Stop())

        | StopScheduler -> stop()

    member _.ScheduledActorCount = _scheduledActorCount
    member _.AvailableActors = _actorPool

    member _.AddActors(count) =
        _scheduledActorCount <- _scheduledActorCount + count
        execScheduler _scheduledActorCount

    member _.RemoveActors(count) =
        _scheduledActorCount <- removeFromScheduler _scheduledActorCount count
        execScheduler _scheduledActorCount

    member _.Stop() = stop()

    interface IDisposable with
        member _.Dispose() = stop()
