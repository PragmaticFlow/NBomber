module internal NBomber.Domain.Concurrency.ScenarioActorPool

open FSharpx.Collections

open NBomber.Domain
open NBomber.Domain.Concurrency.ScenarioActor

let createActors (dep: ActorDep, count, fromIndex) =

    let actors = ResizeArray(capacity = count)

    let createCorrelationId (scnName: string, actorIndex: int) =
        sprintf "%s_%i" scnName actorIndex

    for i = 0 to count do
        let actorIndex = fromIndex + i
        let correlationId = createCorrelationId(dep.Scenario.ScenarioName, actorIndex)
        let actor = ScenarioActor(dep, actorIndex, correlationId, dep.Scenario)
        actors.Add(actor)
    actors

let rentActors (dep: ActorDep, actorsPool: ResizeArray<ScenarioActor>, schedulerName, count) =

    let freeActors =
        actorsPool
        |> Seq.filter(fun x -> not x.Reserved && not x.Working)
        |> Seq.truncate count
        |> ResizeArray.ofSeq

    if freeActors.Count < count then
        let createCount = count - freeActors.Count
        let fromIndex = actorsPool.Count
        let newActors = createActors(dep, createCount, fromIndex)
        freeActors.AddRange(newActors)
        freeActors |> ResizeArray.iter(fun x -> x.ReserveForScheduler(schedulerName))
        {| ActorsFromPool = freeActors; NewActors = Some newActors |}
    else
        freeActors |> ResizeArray.iter(fun x -> x.ReserveForScheduler(schedulerName))
        {| ActorsFromPool = freeActors; NewActors = None |}

type ScenarioActorPool(dep: ActorDep) =

    let _actorsPool = ResizeArray<ScenarioActor>()

    member x.Dep = dep
    member x.ActorsCount = _actorsPool.Count

    member x.RentActors(schedulerName, count) =
        let result = rentActors(dep, _actorsPool, schedulerName, count)
        match result.NewActors with
        | Some newActors ->
            _actorsPool.AddRange(newActors)
            result.ActorsFromPool.AddRange(newActors)
            result.ActorsFromPool

        | None -> result.ActorsFromPool

    member x.ReleaseActor(renterPoolName, actor: ScenarioActor) =
        actor.LeaveScheduler(renterPoolName)

    member x.ReleaseActors(renterPoolName, actors: ScenarioActor seq) =
        actors |> Seq.iter(fun x -> x.LeaveScheduler(renterPoolName))
