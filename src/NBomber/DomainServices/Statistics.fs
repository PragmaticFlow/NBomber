module internal rec NBomber.Statistics

open System
open System.Collections.Generic

open HdrHistogram

open NBomber.Contracts
open NBomber.Domain

let apply (scenario: Scenario, flowsStats: TestFlowStats[]) = ScenarioStats.create(scenario, flowsStats)

type LatencyCount = {
    Less800: int
    More800Less1200: int
    More1200: int
}

type LatencyDetails = {
    RPS: int64
    Min: Latency
    Mean: Latency
    Max: Latency
    Percent50: Latency
    Percent75: Latency
    Percent95: Latency
    StdDev: int64
}

type StepStats = {     
    StepName: string
    OkLatencies: Latency[]
    FailLatencies: Latency[]    
    OkCount: int
    FailCount: int    
    LatencyDetails: LatencyDetails option
}

type TestFlowStats = {
    FlowName: string
    StepsStats: StepStats[]    
    ConcurrentCopies: int
    OkCount: int
    FailCount: int
    LatencyCount: LatencyCount
}

type ScenarioStats = {
    ScenarioName: string
    TestFlowsStats: TestFlowStats[]
    ActiveTime: TimeSpan
    OkCount: int
    FailCount: int
    LatencyCount: LatencyCount
}

module StepStats =

    let create (stepName, responseResults: List<Response*Latency>) =
    
        let allResults = responseResults.ToArray()
        let okResults = allResults |> Array.filter(fun (res,_) -> res.IsOk)
        let failResults = allResults |> Array.filter(fun (res,_) -> not(res.IsOk))

        { StepName = stepName 
          OkLatencies = okResults |> Array.map(snd)
          FailLatencies = failResults |> Array.map(snd)          
          OkCount = okResults.Length
          FailCount = allResults.Length - okResults.Length
          LatencyDetails = None }    

    let calcDetails (stats: StepStats, scenarioDuration: TimeSpan) =
        
        let buildHistogram (latencies) =            
            let histogram = LongHistogram(TimeStamp.Hours(1), 3);
            latencies |> Array.iter(fun x -> histogram.RecordValue(x))
            histogram           
        
        let histogram = buildHistogram(stats.OkLatencies)
            
        { RPS = calcRPS(stats.OkLatencies, scenarioDuration)
          Min = calcMin(stats.OkLatencies)
          Mean = calcMean(histogram)
          Max = calcMax(histogram)
          Percent50 = calcPercentile(histogram, 50.0)
          Percent75 = calcPercentile(histogram, 75.0)
          Percent95 = calcPercentile(histogram, 95.0)
          StdDev = calcStdDev(histogram) }

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

    let calcStdDev (histogram: LongHistogram) =
        if histogram.TotalCount > 0L then
            histogram.GetStdDeviation() |> Math.Round |> Convert.ToInt64
        else
            0L

module TestFlowStats =

    let create (flow: TestFlow) (stepsStats: StepStats[]) =
                
        let mergeByStepName (allCopies: StepStats[]) =
            allCopies
            |> Array.groupBy(fun x -> x.StepName)
            |> Array.map(fun (stName, results) ->                 
                { StepName = stName
                  OkLatencies = results |> Array.collect(fun x -> x.OkLatencies)
                  FailLatencies = results |> Array.collect(fun x -> x.FailLatencies)                  
                  OkCount = results |> Array.map(fun x -> x.OkCount) |> Array.sum
                  FailCount = results |> Array.map(fun x -> x.FailCount) |> Array.sum                  
                  LatencyDetails = None })
        
        let mergedStats = mergeByStepName(stepsStats)
        let latency = calcLatencyCount(mergedStats)
        let allOkCount = mergedStats |> Array.sumBy(fun x -> x.OkCount)
        let allFailCount = mergedStats |> Array.sumBy(fun x -> x.FailCount)

        { FlowName = flow.FlowName
          StepsStats = mergedStats
          ConcurrentCopies = flow.CorrelationIds.Count
          OkCount = allOkCount
          FailCount = allFailCount
          LatencyCount = latency }

    let calcLatencyCount (stepsStats: StepStats[]) = 
        let a = stepsStats |> Array.collect(fun x -> x.OkLatencies |> Array.filter(fun x -> x < 800L))
        let b = stepsStats |> Array.collect(fun x -> x.OkLatencies |> Array.filter(fun x -> x > 800L && x < 1200L))
        let c = stepsStats |> Array.collect(fun x -> x.OkLatencies |> Array.filter(fun x -> x > 1200L))        

        { Less800 = a.Length
          More800Less1200 = b.Length
          More1200 = c.Length }

module ScenarioStats =    

    let create (scenario: Scenario, flowsStats: TestFlowStats[]) =        

        let applyCalculations (scenarioDuration) (flowStats: TestFlowStats) =
            { flowStats 
              with StepsStats = flowStats.StepsStats
                                |> Array.map(fun x -> { x with LatencyDetails = Some(StepStats.calcDetails(x, scenarioDuration)) }) }

        let activeTime = scenario.Duration - getPausedTime(scenario)

        let stats = flowsStats |> Array.map(applyCalculations(activeTime))
        let latencyCount = calcLatencyCount(stats)
        let allOkCount = flowsStats |> Array.sumBy(fun x -> x.StepsStats |> Array.sumBy(fun x -> x.OkCount))
        let allFailCount = flowsStats |> Array.sumBy(fun x -> x.StepsStats |> Array.sumBy(fun x -> x.FailCount))

        { ScenarioName = scenario.ScenarioName
          TestFlowsStats = stats 
          ActiveTime = activeTime
          OkCount = allOkCount
          FailCount = allFailCount
          LatencyCount = latencyCount }

    let calcLatencyCount (flowsStats: TestFlowStats[]) =
        let latCounts = flowsStats |> Array.map(fun x -> x.LatencyCount)
        { Less800 = latCounts |> Array.sumBy(fun x -> x.Less800)
          More800Less1200 = latCounts |> Array.sumBy(fun x -> x.More800Less1200)
          More1200 = latCounts |> Array.sumBy(fun x -> x.More1200) }        

    let private getPausedTime (scenario: Scenario) =
        scenario.TestFlows
        |> Array.collect(fun x -> x.Steps)
        |> Array.sumBy(fun x -> match x with | Pause time -> time.Ticks | _ -> int64 0)
        |> TimeSpan