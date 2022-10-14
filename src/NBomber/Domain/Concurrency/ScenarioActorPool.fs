module internal NBomber.Domain.Concurrency.ScenarioActorPool

open NBomber.Domain
open NBomber.Domain.Concurrency.ScenarioActor
open NBomber.Domain.ScenarioContext

[<Struct>]
type ActorPoolResult = {
    ActorsFromPool: ScenarioActor list
    NewActors: ScenarioActor list
}

let createActors (scnCtx: ScenarioContextArgs) count fromIndex =
    let scenario = scnCtx.Scenario
    List.init count (fun i ->
        let actorIndex = fromIndex + i
        let scenarioInfo = Scenario.createScenarioInfo(scenario.ScenarioName, scenario.PlanedDuration, actorIndex, scnCtx.ScenarioOperation)
        ScenarioActor(scnCtx, scenarioInfo)
    )

// todo: add tests
let rentActors (createActors: int -> int -> ScenarioActor list) // count -> fromIndex
               (actorPool: ScenarioActor list)
               (actorCount: int) =

    let freeActors =
        actorPool
        |> List.filter(fun x -> not x.Working)
        |> List.truncate actorCount

    if freeActors.Length < actorCount then
        let createCount = actorCount - freeActors.Length
        let fromIndex = actorPool.Length
        let newActors = createActors createCount fromIndex
        { ActorsFromPool = freeActors; NewActors = newActors }
    else
        { ActorsFromPool = freeActors; NewActors = List.empty }

let stopActors (actorPool: ScenarioActor list) =
    actorPool |> List.iter(fun x -> x.Stop())

let getWorkingActors (actorPool: ScenarioActor list) =
    actorPool |> List.filter(fun x -> x.Working)

let updatePool (currentPool: ScenarioActor list) (newActors: ScenarioActor list) =
    currentPool |> List.append newActors
