module internal NBomber.Domain.Assertion

open NBomber.Contracts
open NBomber.Domain.Errors
open NBomber.Domain.DomainTypes
open NBomber.Domain.StatisticsTypes

type AssertStats with

    static member create(stats: StepStats) =        
        { OkCount = stats.OkCount
          FailCount = stats.FailCount
          Min = stats.Min
          Mean = stats.Mean
          Max = stats.Max
          RPS = stats.RPS
          Percent50 = stats.Percent50
          Percent75 = stats.Percent75
          Percent95 = stats.Percent95
          StdDev = stats.StdDev
          DataMinKb = stats.DataTransfer.MinKb
          DataMeanKb = stats.DataTransfer.MeanKb
          DataMaxKb = stats.DataTransfer.MaxKb
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

let apply (globalStats: GlobalStats) (assertions: Assertion[]) =
    assertions
    |> Array.mapi(fun i assertion -> 
        let asrtNum = i + 1
        match assertion with
        | Step asrt -> applyForStep(globalStats, asrtNum, asrt)        
    )