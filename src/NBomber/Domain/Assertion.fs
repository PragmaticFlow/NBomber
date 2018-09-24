module internal NBomber.Domain.Assertion

open NBomber.Contracts
open NBomber.Domain.Errors
open NBomber.Domain.DomainTypes
open NBomber.Domain.StatisticsTypes

type AssertStats with

    static member Create(stats: StepStats) =
        { OkCount = stats.OkCount
          FailCount = stats.FailCount }

    static member Create(stats: TestFlowStats) =
        { OkCount = stats.OkCount
          FailCount = stats.FailCount }

    static member Create(stats: ScenarioStats) =
        { OkCount = stats.OkCount
          FailCount = stats.FailCount }

let private mapToResult (scopeType: string, scopeName: string, assertNumber: int) (result: bool) =
    if result then
        Ok()
    else 
        AssertionError(scopeType, scopeName, assertNumber)
        |> Error

let create (assertions: IAssertion[]) = 
    assertions |> Array.map(fun x -> x :?> Assertion)

let execStep (scenarioStats: ScenarioStats, stepName: StepName, flowName: FlowName,
              assertFunc: AssertFunc, assertNumber: int) =
    let result = 
        scenarioStats.TestFlowsStats
        |> Array.filter(fun x -> x.FlowName = flowName)
        |> Array.collect(fun x -> x.StepsStats)
        |> Array.tryFind(fun x -> x.StepName = stepName)

    match result with
    | Some v -> AssertStats.Create(v)
                |> assertFunc
                |> mapToResult("Step", v.StepName, assertNumber)

    | None   -> AssertNotFound("Step", stepName) 
                |> Error

let executeTestFlow (scenarioStats: ScenarioStats, flowName: FlowName, 
                     assertFunc: AssertFunc, assertNumber: int) =
    let result = 
        scenarioStats.TestFlowsStats
        |> Array.tryFind(fun x -> x.FlowName = flowName)
        
    match result with
    | Some v -> AssertStats.Create(v)
                |> assertFunc
                |> mapToResult("TestFlow", v.FlowName, assertNumber)

    | None   -> AssertNotFound("TestFlow", flowName) 
                |> Error    

let execScenario (scenarioStats: ScenarioStats, assertFunc: AssertFunc, assertNumber: int) =
    scenarioStats
    |> AssertStats.Create
    |> assertFunc
    |> mapToResult("Scenario", scenarioStats.ScenarioName, assertNumber)

let run (scnStats: ScenarioStats) (assertions: Assertion[]) =       
    assertions
    |> Array.mapi(fun i assertion -> 
        let asrtNum = i + 1
        match assertion with
        | Step (stepName,flowName,assertFunc) -> execStep(scnStats, stepName, flowName, assertFunc, asrtNum)
        | TestFlow (flowName,asrtFunc)        -> executeTestFlow(scnStats, flowName, asrtFunc, asrtNum)
        | Scenario assertFunc                 -> execScenario(scnStats, assertFunc, asrtNum)
    )