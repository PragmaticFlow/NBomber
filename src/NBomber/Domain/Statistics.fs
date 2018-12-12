module internal NBomber.Domain.Statistics

open System
open System.Collections.Generic

open HdrHistogram

open NBomber.Contracts
open NBomber.Domain.DomainTypes
open NBomber.Domain.StatisticsTypes

let buildHistogram (latencies) =    
    let histogram = LongHistogram(TimeStamp.Hours(24), 3)
    latencies |> Array.filter(fun x -> x > 0)
              |> Array.iter(fun x -> x |> int64 |> histogram.RecordValue)
    histogram    

let calcRPS (latencies: Latency[], scnDuration: TimeSpan) =
    let totalSec = if scnDuration.TotalSeconds < 1.0 then 1.0
                   else scnDuration.TotalSeconds
    latencies.Length / int(totalSec)

let calcMin (latencies: Latency[]) =
    if latencies.Length > 0 then Array.min(latencies) else 0

let calcMean (latencies: Latency[]) =        
    if latencies.Length > 0 
    then latencies |> Array.map(float) |> Array.average |> int
    else 0

let calcMax (latencies: Latency[]) =
    if latencies.Length > 0 then Array.max(latencies) else 0

let calcPercentile (histogram: LongHistogram, percentile: float) =
    if histogram.TotalCount > 0L then int(histogram.GetValueAtPercentile(percentile)) else 0

let calcStdDev (histogram: LongHistogram) =
    if histogram.TotalCount > 0L then
        histogram.GetStdDeviation() |> Math.Round |> int
    else
        0

module StepStats =

    let create (stepName, responseResults: List<Response*Latency>) =
    
        let allResults = responseResults.ToArray()
        let okResults = allResults |> Array.filter(fun (res,_) -> res.IsOk)
        let failResults = allResults |> Array.filter(fun (res,_) -> not(res.IsOk))

        { StepName = stepName 
          OkLatencies = okResults |> Array.map(snd)
          FailLatencies = failResults |> Array.map(snd)          
          ReqeustCount = allResults.Length
          OkCount = okResults.Length
          FailCount = allResults.Length - okResults.Length
          Percentiles = None } 

    let calcPercentiles (stats: StepStats, scenarioDuration: TimeSpan) =
        let histogram = buildHistogram(stats.OkLatencies)            
        { RPS = calcRPS(stats.OkLatencies, scenarioDuration)
          Min = calcMin(stats.OkLatencies)
          Mean = calcMean(stats.OkLatencies)
          Max = calcMax(stats.OkLatencies)
          Percent50 = calcPercentile(histogram, 50.0)
          Percent75 = calcPercentile(histogram, 75.0)
          Percent95 = calcPercentile(histogram, 95.0)
          StdDev = calcStdDev(histogram) }

module ScenarioStats =        

    let calcPausedTime (scenario: Scenario) =
        scenario.Steps
        |> Array.sumBy(fun x -> match x with | Pause time -> time.Ticks | _ -> int64 0)
        |> TimeSpan

    let calcLatencyCount (stepsStats: StepStats[]) = 
        let a = stepsStats |> Array.collect(fun x -> x.OkLatencies |> Array.filter(fun x -> x < 800))
        let b = stepsStats |> Array.collect(fun x -> x.OkLatencies |> Array.filter(fun x -> x > 800 && x < 1200))
        let c = stepsStats |> Array.collect(fun x -> x.OkLatencies |> Array.filter(fun x -> x > 1200))        
        { Less800 = a.Length
          More800Less1200 = b.Length
          More1200 = c.Length } 

    let mergeByStepName (stepsStats: StepStats[]) =
        stepsStats
        |> Array.groupBy(fun x -> x.StepName)
        |> Array.map(fun (stName, results) ->
            let okCount = results |> Array.map(fun x -> x.OkCount) |> Array.sum
            let failCount = results |> Array.map(fun x -> x.FailCount) |> Array.sum
            let reqeustCount = okCount + failCount
            { StepName = stName
              OkLatencies = results |> Array.collect(fun x -> x.OkLatencies)
              FailLatencies = results |> Array.collect(fun x -> x.FailLatencies)                  
              ReqeustCount = reqeustCount
              OkCount = okCount
              FailCount = failCount
              Percentiles = None })

    let calcPercentiles (activeScnTime: TimeSpan) (stepsStats: StepStats[]) =        
        stepsStats 
        |> Array.map(fun x -> { x with Percentiles = Some(StepStats.calcPercentiles(x, activeScnTime)) })

    let create (scenario: Scenario) (stepsStats: StepStats[]) =
        
        let activeTime = scenario.Duration - calcPausedTime(scenario)
        let mergedStats = mergeByStepName(stepsStats) |> calcPercentiles(activeTime)                        
        let latencyCount = calcLatencyCount(mergedStats)

        let allOkCount = mergedStats |> Array.sumBy(fun x -> x.OkCount)
        let allFailCount = mergedStats |> Array.sumBy(fun x -> x.FailCount)

        { ScenarioName = scenario.ScenarioName
          StepsStats = mergedStats
          ConcurrentCopies = scenario.ConcurrentCopies
          OkCount = allOkCount
          FailCount = allFailCount
          LatencyCount = latencyCount
          ActiveTime = activeTime
          Duration = scenario.Duration } 

module GlobalStats =

    let create (allScnStats: ScenarioStats[]) =
        
        let allOkCount = allScnStats
                         |> Array.collect(fun x -> x.StepsStats)
                         |> Array.sumBy(fun x -> x.OkCount)

        let allFailCount = allScnStats
                           |> Array.collect(fun x -> x.StepsStats)
                           |> Array.sumBy(fun x -> x.FailCount)

        let latencyCount = allScnStats
                           |> Array.collect(fun x -> x.StepsStats)
                           |> ScenarioStats.calcLatencyCount

        { AllScenariosStats = allScnStats
          OkCount = allOkCount
          FailCount = allFailCount
          LatencyCount = latencyCount }