module internal NBomber.Domain.Assertion

open NBomber.Contracts
open NBomber.Domain.Errors
open NBomber.Domain.DomainTypes
open NBomber.Domain.StatisticsTypes

type AssertStats with

    static member create(stats: StepStats) =
        let p = stats.Percentiles.Value
        { OkCount = stats.OkCount
          FailCount = stats.FailCount
          Min = p.Min
          Mean = p.Mean
          Max = p.Max
          RPS = p.RPS
          Percent50 = p.Percent50
          Percent75 = p.Percent75
          Percent95 = p.Percent95
          StdDev = p.StdDev }

let create (assertions: IAssertion[]) = 
    assertions |> Array.map(fun x -> x :?> Assertion)

let getStepsStats (globalStats: GlobalStats, scenario: NBomber.Contracts.Scenario) =
    globalStats.AllScenariosStats
    |> Array.tryFind(fun x -> x.ScenarioName = scenario.ScenarioName)
    |> Option.map(fun e -> e.StepsStats |> Array.map(fun e -> (e.StepName, AssertStats.create(e))))
    |> Option.defaultValue([||])

let applyForStep (asrtStats: AssertStats option, assertNum: int, asrt: StepAssertion) =
    match asrtStats with
    | Some stats ->   
        let result = asrt.AssertFunc(stats)
        if result then Ok <| Step(asrt)
        else Error <| AssertionError(assertNum, Step(asrt), stats)
    | None ->  Error <| AssertNotFound(assertNum, Step(asrt))
    
let applyScenario(globalStats: GlobalStats, sc: NBomber.Contracts.Scenario) = 
    let stepStats = getStepsStats(globalStats, sc) |> Map.ofArray
    
    let stepResults = sc.Assertions 
                      |> create
                      |> Array.mapi(fun i asrt -> match asrt with 
                                                    | NBomber.Domain.DomainTypes.Step s ->
                                                        let stepStat = Map.tryFind s.StepName stepStats
                                                        let asrtResult = applyForStep(stepStat, i, s)
                                                        (s.StepName, { ValidationResult.Number = i + 1; Result = asrtResult }))
                      |> Array.groupBy(fun (stepName, _) -> stepName)
                      |> Array.map(fun (stepName, results) -> { StepName = stepName; ValidationResults = results |> Array.map(snd); Stats = stepStats.[stepName]})
    
    { ScenarioName = sc.ScenarioName; StepResults = stepResults }