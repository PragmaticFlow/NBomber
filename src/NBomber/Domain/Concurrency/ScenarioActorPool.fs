module internal NBomber.Domain.Concurrency.ScenarioActorPool

open NBomber.Domain
open NBomber.Domain.Concurrency.ScenarioActor

[<Struct>]
type ActorPoolResult = {
    ActorsFromPool: ScenarioActor list
    NewActors: ScenarioActor list
}

let createActors (dep: ActorDep, count, fromIndex) =

    List.init count (fun i ->
        let actorIndex = fromIndex + i
        let correlationId = Scenario.createCorrelationId(dep.Scenario.ScenarioName, actorIndex)
        ScenarioActor(dep, correlationId)
    )

let rentActors (dep: ActorDep, actorPool: ScenarioActor list, actorCount) =

    let freeActors =
        actorPool
        |> List.filter(fun x -> not x.Reserved)
        |> List.truncate(actorCount)

    freeActors |> List.iter(fun x -> x.ReserveForScheduler())

    if freeActors.Length < actorCount then
        let createCount = actorCount - freeActors.Length
        let fromIndex = actorPool.Length
        let newActors = createActors(dep, createCount, fromIndex)
        newActors |> List.iter(fun x -> x.ReserveForScheduler())
        { ActorsFromPool = freeActors; NewActors = newActors }
    else
        { ActorsFromPool = freeActors; NewActors = List.empty }

let releaseActors (actorPool: ScenarioActor list) =
    actorPool |> List.iter(fun x -> x.LeaveScheduler())

let updatePool (currentPool: ScenarioActor list, result: ActorPoolResult) =
    currentPool |> List.append(result.NewActors)
