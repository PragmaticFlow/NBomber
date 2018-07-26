module internal rec NBomber.Statistics

open System
open System.Collections.Generic

open HdrHistogram

open NBomber.Contracts
open NBomber.Domain

type StepInfo = {
    StepName: string
    Latencies: Latency[]
    ThrownException: exn option
    OkCount: int
    FailCount: int
    ExceptionCount: ExceptionCount
}

type StepStats = {
    StepNo: int
    Info: StepInfo
    RPS: int64
    Min: Latency
    Mean: Latency
    Max: Latency
    Percent50: Latency
    Percent75: Latency
}

type FlowInfo = {
    FlowName: string
    Steps: StepInfo[]    
    ConcurrentCopies: int
}

module StepInfo =

    let create (stepName, responseResults: List<Response*Latency>, 
                exceptions: (Option<exn>*ExceptionCount)) =
    
        let results = responseResults.ToArray()
    
        { StepName = stepName 
          Latencies = results |> Array.map(snd)
          ThrownException = fst(exceptions)                                  
          OkCount = results 
                    |> Array.map(fst)
                    |> Array.filter(fun stRes -> stRes.IsOk)
                    |> Array.length                                  
          FailCount = results 
                      |> Array.map(fst)
                      |> Array.filter(fun stRes -> not(stRes.IsOk))
                      |> Array.length
          ExceptionCount = snd(exceptions) }

module StepStats =

    let create (stepNo: int, stepInfo: StepInfo, scenarioDuration: TimeSpan) = 
        let histogram = LongHistogram(TimeStamp.Hours(1), 3);
        stepInfo.Latencies |> Array.iter(fun x -> histogram.RecordValue(x))
        
        let rps = histogram.TotalCount / int64(scenarioDuration.TotalSeconds)

        let min = if stepInfo.Latencies.Length > 0 then Array.min(stepInfo.Latencies) else int64(0)
        let mean = if stepInfo.Latencies.Length > 0 then Convert.ToInt64(histogram.GetMean()) else int64(0) 
        let max  = if stepInfo.Latencies.Length > 0 then histogram.GetMaxValue() else int64(0)

        let percent50 = if stepInfo.Latencies.Length > 0 then histogram.GetValueAtPercentile(50.) else int64(0)
        let percent75 = if stepInfo.Latencies.Length > 0 then histogram.GetValueAtPercentile(75.) else int64(0)
    
        { StepNo = stepNo; Info = stepInfo; RPS = rps
          Min = min; Mean = mean; Max = max
          Percent50 = percent50; Percent75 = percent75 }

module FlowInfo =

    let create (flowName, concurrentCopies: int) (stepsStats: StepInfo[]) =
        // merge all steps results into one 
        let results = 
            stepsStats
            |> Array.groupBy(fun x -> x.StepName)
            |> Array.map(fun (stName,stRes) -> 
                { StepName = stName
                  Latencies = stRes |> Array.collect(fun x -> x.Latencies)
                  ThrownException = stRes |> Array.tryLast |> Option.bind(fun x -> x.ThrownException)
                  OkCount = stRes |> Array.map(fun x -> x.OkCount) |> Array.sum
                  FailCount = stRes |> Array.map(fun x -> x.FailCount) |> Array.sum
                  ExceptionCount = stRes |> Array.map(fun x -> x.ExceptionCount) |> Array.sum })
    
        { FlowName = flowName; Steps = results; ConcurrentCopies = concurrentCopies }