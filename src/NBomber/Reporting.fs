module internal rec NBomber.Reporting

open System
open System.IO
open System.Collections.Generic
open HdrHistogram
open PrettyTable
open FsPrettyTable

type Latency = int64
type ExceptionCount = int

type StepStats = {
    StepName: string
    Latencies: Latency[]
    ThrownException: exn option
    OkCount: int
    FailCount: int
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
      FailCount = results 
                   |> Array.map(fst)
                   |> Array.filter(fun stRes -> stRes = StepResult.Fail)
                   |> Array.length
      ExceptionCount = snd(exceptions) }
    
type FlowStats = {
    FlowName: string
    StepStats: StepStats[]    
    ConcurrentCopies: int
} with
  static member Create(flowName, concurrentCopies: int) (stepsStats: StepStats[]) =
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
    
    { FlowName = flowName; StepStats = results; ConcurrentCopies = concurrentCopies }


let buildReport (scenario: Scenario, stats: FlowStats[]) =        
    let header = String.Format("Scenario: {0}, execution time: {1}", scenario.ScenarioName, scenario.Duration.ToString())
    let details = stats |> Array.map(fun x -> printFlowStats(x, scenario.Duration)) |> String.concat ""    
    header + Environment.NewLine + Environment.NewLine + details

let printStepStats (stats: StepStats, scenarioDuration: TimeSpan, stepCount: int) =
    let histogram = LongHistogram(TimeStamp.Hours(1), 3);
    stats.Latencies |> Array.iter(fun x -> histogram.RecordValue(x))
        
    let rps = histogram.TotalCount / int64(scenarioDuration.TotalSeconds)

    let minLatency  = if stats.Latencies.Length > 0 then Array.min(stats.Latencies) else int64(0)
    let meanLatency = if stats.Latencies.Length > 0 then Convert.ToInt64(histogram.GetMean()) else int64(0) 
    let maxLatency  = if stats.Latencies.Length > 0 then histogram.GetMaxValue() else int64(0)

    let percent50 = if stats.Latencies.Length > 0 then histogram.GetValueAtPercentile(50.) else int64(0)
    let percent75 = if stats.Latencies.Length > 0 then histogram.GetValueAtPercentile(75.) else int64(0)

    let stepStats = [int64(stepCount); histogram.TotalCount; int64(stats.OkCount); int64(stats.FailCount)
                     int64(stats.ExceptionCount); rps; minLatency; meanLatency; maxLatency; percent50; percent75]
    
    stepStats |> Seq.map(fun stats -> stats.ToString()) |> Seq.toList

let printFlowStats (flowStats: FlowStats, scenarioDuration: TimeSpan) =
    let headers =
        ["step No";"request_count";"ok_count";"fail_count"; "exception_count"; "RPS"; "min"; "mean"; "max"; "percentile 50%"; "70%"]

    let stepsNames =        
        flowStats.StepStats
        |> Seq.mapi(fun index stats ->  String.Format("{0} - {1}{2}", index+1, stats.StepName, Environment.NewLine))
        |> String.concat ""

    let rows =
        flowStats.StepStats
        |> Seq.mapi(fun index stats -> printStepStats(stats, scenarioDuration, index + 1))
        |> Seq.toList

    let flowStatsTable =
        prettyTable rows |> withHeaders headers |> headerStyle Types.UpperCase |> sprintTable
    
    String.Format("flow name: {0}; concurrent copies: {1} {2}{3}",
                  flowStats.FlowName, flowStats.ConcurrentCopies, Environment.NewLine + Environment.NewLine,
                  stepsNames + Environment.NewLine + flowStatsTable) + Environment.NewLine

let saveReport (report: string) = 
    Directory.CreateDirectory("reports") |> ignore
    let filePath = Path.Combine("reports", "report-" + DateTime.UtcNow.ToString("yyyy-dd-M--HH-mm-ss")) + ".txt"
    File.WriteAllText(filePath, report)