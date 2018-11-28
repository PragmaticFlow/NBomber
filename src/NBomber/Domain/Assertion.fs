module internal NBomber.Domain.Assertion

open NBomber.Contracts
open NBomber.Domain.Errors
open NBomber.Domain.DomainTypes
open NBomber.Domain.StatisticsTypes

type AssertStats with

    static member create(stats: StepStats) =
        { OkCount = stats.OkCount
          FailCount = stats.FailCount }
   
    static member create(stats: ScenarioStats) =
        { OkCount = stats.OkCount
          FailCount = stats.FailCount }

let create (assertions: IAssertion[]) = 
    assertions |> Array.map(fun x -> x :?> Assertion)

let applyForStep (globalStats: GlobalStats, assertNum: int, asrt: StepAssertion) =
    let stepStats = 
        globalStats.AllScenariosStats
        |> Array.filter(fun x -> x.ScenarioName = asrt.ScenarioName)
        |> Array.collect(fun x -> x.StepsStats)
        |> Array.tryFind(fun x -> x.StepName = asrt.StepName)
    
    match stepStats with
    | Some v -> let result = AssertStats.create(v) |> asrt.AssertFunc
                if result then Ok <| Step(asrt)
                else Error <| AssertionError(assertNum, Step(asrt))
    | None   -> Error <| AssertNotFound(assertNum, Step(asrt))


let applyForScenario (globalStats: GlobalStats, assertNum: int, asrt: ScenarioAssertion) =
    let scnStats = 
        globalStats.AllScenariosStats
        |> Array.tryFind(fun x -> x.ScenarioName = asrt.ScenarioName)
        
    match scnStats with
    | Some v -> let result = AssertStats.create(v) |> asrt.AssertFunc
                if result then Ok <| Scenario(asrt)
                else Error <| AssertionError(assertNum, Scenario(asrt))
    | None   -> Error <| AssertNotFound(assertNum, Scenario(asrt))

let apply (globalStats: GlobalStats, assertions: Assertion[]) =
    assertions
    |> Array.mapi(fun i assertion -> 
        let asrtNum = i + 1
        match assertion with
        | Step asrt     -> applyForStep(globalStats, asrtNum, asrt)
        | Scenario asrt -> applyForScenario(globalStats, asrtNum, asrt)            
    )

