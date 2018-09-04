module internal rec NBomber.Statistics

open System
open System.Collections.Generic

open HdrHistogram

open NBomber.Contracts
open NBomber.Domain

let apply (scenario: Scenario, flowsStats: FlowStats[]) = ScenarioStats.create(scenario, flowsStats)

type StepStats = {     
    StepName: string
    Latencies: Latency[]
    ThrownException: exn option
    OkCount: int
    FailCount: int
    ExceptionCount: ExceptionCount 
    Details: StepDetails option
}

type StepDetails = {
    RPS: int64
    Min: Latency
    Mean: Latency
    Max: Latency
    Percent50: Latency
    Percent75: Latency
}

type FlowStats = {
    FlowName: string
    StepsStats: StepStats[]    
    ConcurrentCopies: int    
}

type ScenarioStats = {
    ScenarioName: string
    FlowsStats: FlowStats[]
    ActiveTime: TimeSpan
}

module StepStats =

    let create (stepName, responseResults: List<Response*Latency>, 
                exceptions: (Option<exn>*ExceptionCount)) =
    
        let results = responseResults.ToArray()
        let responses = results |> Array.map(fst)
        let latencies = results |> Array.map(snd)        

        { StepName = stepName 
          Latencies = latencies
          ThrownException = fst(exceptions)
          OkCount = calcOkCount(responses)
          FailCount = calcFailCount(responses)
          ExceptionCount = snd(exceptions)
          Details = None }

    let calcDetails (stats: StepStats, scenarioDuration: TimeSpan) =
        
        let buildHistogram (latencies) =            
            let histogram = LongHistogram(TimeStamp.Hours(1), 3);
            latencies |> Array.iter(fun x -> histogram.RecordValue(x))
            histogram           
        
        let histogram = buildHistogram(stats.Latencies)
            
        { RPS = calcRPS(stats.Latencies, scenarioDuration)
          Min = calcMin(stats.Latencies)
          Mean = calcMean(histogram)
          Max = calcMax(histogram)
          Percent50 = calcPercentile(histogram, 50.0)
          Percent75 = calcPercentile(histogram, 75.0) }

    let calcOkCount (responses: Response[]) = 
        responses
        |> Array.filter(fun stRes -> stRes.IsOk)
        |> Array.length

    let calcFailCount (responses: Response[]) = 
        responses
        |> Array.filter(fun response -> not(response.IsOk))
        |> Array.length

    let calcRPS (latencies: Latency[], scenarioDuration: TimeSpan) =
        latencies.LongLength / int64(scenarioDuration.TotalSeconds)    

    let calcMin (latencies: Latency[]) =
        if latencies.Length > 0 then Array.min(latencies) else 0L

    let calcMean (histogram: LongHistogram) =
        if histogram.TotalCount > 0L then Convert.ToInt64(histogram.GetMean()) else 0L

    let calcMax (histogram: LongHistogram) =
        if histogram.TotalCount > 0L then histogram.GetMaxValue() else 0L

    let calcPercentile (histogram: LongHistogram, percentile: float) =
        if histogram.TotalCount > 0L then histogram.GetValueAtPercentile(percentile) else 0L

module FlowStats =

    let create (flow: TestFlow) (stepsStats: StepStats[]) =
                
        let mergeByStepName (allCopies: StepStats[]) =
            allCopies
            |> Array.groupBy(fun x -> x.StepName)
            |> Array.map(fun (stName, results) ->                 
                { StepName = stName
                  Latencies = results |> Array.collect(fun x -> x.Latencies)
                  ThrownException = results |> Array.tryLast |> Option.bind(fun x -> x.ThrownException)
                  OkCount = results |> Array.map(fun x -> x.OkCount) |> Array.sum
                  FailCount = results |> Array.map(fun x -> x.FailCount) |> Array.sum
                  ExceptionCount = results |> Array.map(fun x -> x.ExceptionCount) |> Array.sum
                  Details = None })
        
        let mergedStats = mergeByStepName(stepsStats)

        { FlowName = flow.FlowName
          StepsStats = mergedStats
          ConcurrentCopies = flow.CorrelationIds.Count }

module ScenarioStats =    

    let create (scenario: Scenario, flowsStats: FlowStats[]) =        

        let applyCalculations (scenarioDuration) (flowStats: FlowStats) =
            { flowStats 
              with StepsStats = flowStats.StepsStats
                                |> Array.map(fun x -> { x with Details = Some(StepStats.calcDetails(x, scenarioDuration)) }) }

        let activeTime = scenario.Duration - getPausedTime(scenario)

        let stats = flowsStats |> Array.map(applyCalculations(activeTime))

        { ScenarioName = scenario.ScenarioName
          FlowsStats = stats 
          ActiveTime = activeTime }

    let private getPausedTime (scenario: Scenario) =
        scenario.TestFlows
        |> Array.collect(fun x -> x.Steps)
        |> Array.sumBy(fun x -> match x with | Pause time -> time.Ticks | _ -> int64 0)
        |> TimeSpan