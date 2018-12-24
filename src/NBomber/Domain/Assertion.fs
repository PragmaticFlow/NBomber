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
          StdDev = p.StdDev
          DataMinKB = stats.DataTransfer.MinKB
          DataMeanKB = stats.DataTransfer.MeanKB
          DataMaxKB = stats.DataTransfer.MaxKB
          AllDataMB = stats.DataTransfer.AllMB }

let create (assertions: IAssertion[]) = 
    assertions |> Array.map(fun x -> x :?> Assertion)

let applyForStep (globalStats: GlobalStats, assertNum: int, asrt: StepAssertion) =
    let stepStats = 
        globalStats.AllScenariosStats
        |> Array.filter(fun x -> x.ScenarioName = asrt.ScenarioName)
        |> Array.collect(fun x -> x.StepsStats)
        |> Array.tryFind(fun x -> x.StepName = asrt.StepName)
    
    match stepStats with
    | Some v -> let asrtStats = AssertStats.create(v)
                let result = asrt.AssertFunc(asrtStats)
                if result then Ok <| Step(asrt)
                else Error <| AssertionError(assertNum, Step(asrt), asrtStats)
    | None   -> Error <| AssertNotFound(assertNum, Step(asrt))

let apply (globalStats: GlobalStats, assertions: Assertion[]) =
    assertions
    |> Array.mapi(fun i assertion -> 
        let asrtNum = i + 1
        match assertion with
        | Step asrt -> applyForStep(globalStats, asrtNum, asrt)        
    )