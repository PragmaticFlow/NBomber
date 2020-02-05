module internal NBomber.Domain.Concurrency.ScenarioActorsPool

open System
open System.Threading.Tasks

open FsToolkit.ErrorHandling

open System.Collections.Generic
open NBomber.Extensions
open NBomber.Domain
open NBomber.Domain.Concurrency.ScenarioActor

let createActors (dep: ActorDep, count, fromIndex) =

    let actors = ResizeArray()

    let createCorrelationId (scnName: string, actorIndex: int) =
        sprintf "%s_%i" scnName actorIndex

    for i = 0 to count do
        let actorIndex = fromIndex + i
        let correlationId = createCorrelationId(dep.Scenario.ScenarioName, actorIndex)
        let actor = ScenarioActor(dep, actorIndex, correlationId, dep.Scenario)
        actors.Add(actor)
    actors

let rentActors (dep: ActorDep, actorsPool: ResizeArray<ScenarioActor>, schedulerName, count) =

    let freeActors = actorsPool |> ResizeArray.filter(fun x -> not x.Reserved)

    if freeActors.Count < count then
        let createCount = count - freeActors.Count
        let fromIndex = actorsPool.Count
        let actors = createActors(dep, createCount, fromIndex)
        freeActors.AddRange(actors)
        freeActors |> ResizeArray.iter(fun x -> x.ReserveForScheduler(schedulerName))
        {| FreeActors = freeActors; ShouldAddToPool = true |}
    else
        let removeCount = freeActors.Count - count
        freeActors.RemoveRange(0, removeCount)
        freeActors |> ResizeArray.iter(fun x -> x.ReserveForScheduler(schedulerName))
        {| FreeActors = freeActors; ShouldAddToPool = false |}

type ScenarioActorsPool(dep: ActorDep) =

    let _actorsPool = ResizeArray<ScenarioActor>()

    member x.Dep = dep
    member x.ActorsCount = _actorsPool.Count

    member x.RentActors(schedulerName, count) =
        let actors = rentActors(dep, _actorsPool, schedulerName, count)
        if actors.ShouldAddToPool then _actorsPool.AddRange(actors.FreeActors)
        actors.FreeActors

    member x.ReleaseActors(renterPoolName, actors: ResizeArray<ScenarioActor>) =
        actors |> ResizeArray.iter(fun x -> x.LeaveScheduler(renterPoolName))
