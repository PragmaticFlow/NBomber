module internal rec NBomber.Reporting

open System
open System.IO
open System.Collections.Generic
open HdrHistogram

type Latency = int64
type ExceptionCount = int

type StepStats = {
    StepName: string
    Latencies: Latency[]
    ThrownException: exn option
    OkCount: int
    ErrorCount: int
    ExceptionCount: ExceptionCount
} with
  static member Create(stepName, responseResults: List<StepResult*Latency>, 
                                 exceptions: (Option<exn>*ExceptionCount)) =     
    
    let results = responseResults.ToArray()
    
    { StepName = stepName 
      Latencies = results |> Array.map(snd)
      ThrownException = fst(exceptions)                                  
      OkCount = results 
                |> Array.map(fst)
                |> Array.filter(fun stRes -> stRes = StepResult.Ok)
                |> Array.length                                  
      ErrorCount = results 
                   |> Array.map(fst)
                   |> Array.filter(fun stRes -> stRes = StepResult.Fail)
                   |> Array.length
      ExceptionCount = snd(exceptions) }
    
type FlowStats = {
    FlowName: string
    StepStats: StepStats[]    
    ConcurrentCopies: int
} with
  static member Create(flowName, concurrentCopies: int) (stepsResults: StepStats[]) =
    // merge all steps results into one 
    let results = 
        stepsResults
        |> Array.groupBy(fun x -> x.StepName)
        |> Array.map(fun (stName,stRes) -> 
            { StepName = stName
              Latencies = stRes |> Array.collect(fun x -> x.Latencies)
              ThrownException = stRes |> Array.tryLast |> Option.bind(fun x -> x.ThrownException)
              OkCount = stRes |> Array.map(fun x -> x.OkCount) |> Array.sum
              ErrorCount = stRes |> Array.map(fun x -> x.ErrorCount) |> Array.sum
              ExceptionCount = stRes |> Array.map(fun x -> x.ExceptionCount) |> Array.sum })
    
    { FlowName = flowName; StepStats = results; ConcurrentCopies = concurrentCopies }


let buildReport (scenario: Scenario, results: FlowStats[]) =        
    let header = String.Format("Scenario: {0}, execution time: {1}", scenario.ScenarioName, scenario.Interval.ToString())
    let details = results |> Array.map(fun x -> printFlowResult(x, scenario.Interval)) |> String.concat ""    
    header + Environment.NewLine + Environment.NewLine + details

let printStepResult (result: StepStats, scenarioTime: TimeSpan) =
    let histogram = LongHistogram(TimeStamp.Hours(1), 3);
    result.Latencies |> Array.iter(fun x -> histogram.RecordValue(x))
        
    let rps = histogram.TotalCount / int64(scenarioTime.TotalSeconds)

    let minLatency  = if result.Latencies.Length > 0 then Array.min(result.Latencies) else int64(0)
    let meanLatency = if result.Latencies.Length > 0 then Convert.ToInt64(histogram.GetMean()) else int64(0) 
    let maxLatency  = if result.Latencies.Length > 0 then histogram.GetMaxValue() else int64(0)

    let percent50 = if result.Latencies.Length > 0 then histogram.GetValueAtPercentile(50.) else int64(0)
    let percent75 = if result.Latencies.Length > 0 then histogram.GetValueAtPercentile(75.) else int64(0)

    String.Format("step: {0} {1} requests_count:{2} RPS:{3} exceptions_count:{4}   min:{5} mean:{6} max:{7}   percentile_rank: 50%:{8} 75%:{9}",
                  result.StepName, Environment.NewLine, histogram.TotalCount, rps, result.ExceptionCount,
                  minLatency, meanLatency, maxLatency, percent50, percent75)
                  + Environment.NewLine

let printFlowResult (flowRes: FlowStats, executionTime: TimeSpan) =
    let stepsStr = flowRes.StepStats
                   |> Seq.map(fun stRes -> printStepResult(stRes, executionTime))
                   |> String.concat ""

    String.Format("flow name: {0}; concurrent copies: {1} {2} {3}",
                  flowRes.FlowName, flowRes.ConcurrentCopies, Environment.NewLine, stepsStr) + Environment.NewLine

let saveReport (report: string) = 
    Directory.CreateDirectory("reports") |> ignore
    let filePath = Path.Combine("reports", "report-" + DateTime.UtcNow.ToString("yyyy-dd-M--HH-mm-ss")) + ".txt"
    File.WriteAllText(filePath, report)