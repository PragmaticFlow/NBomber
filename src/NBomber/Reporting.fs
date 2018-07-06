module internal rec NBomber.Reporting

open System
open System.IO
open System.Collections.Generic
open HdrHistogram

type Latency = int64

type FlowResult = {
    FlowName: string
    Results: Dictionary<StepName, List<Latency>>    
    ConcurrentCopies: int    
}

let buildReport (scenario: Scenario, results: FlowResult[]) =        
    let header = String.Format("Scenario: {0}, execution time: {1}", scenario.Name, scenario.Interval.ToString())
    let details = results |> Array.map(fun x -> printFlowResult(x, scenario.Interval)) |> String.concat ""    
    header + Environment.NewLine + Environment.NewLine + details

let printStepResult (name: string, executionTime: TimeSpan, latency: Latency[]) =
    let histogram = LongHistogram(TimeStamp.Hours(1), 3);
    latency |> Array.iter(fun x -> histogram.RecordValue(x))
        
    let rps = histogram.TotalCount / int64(executionTime.TotalSeconds)

    String.Format("step: {0} {1} requests:{2} RPS:{3} min:{4} mean:{5} max:{6} percentile 50%:{7} 70%:{8}",
                  name, Environment.NewLine, histogram.TotalCount, rps, 
                  Array.min(latency), Convert.ToInt64(histogram.GetMean()), histogram.GetMaxValue(),
                  histogram.GetValueAtPercentile(50.), histogram.GetValueAtPercentile(70.)) + Environment.NewLine

let printFlowResult (flowRes: FlowResult, executionTime: TimeSpan) =          
    let commands = flowRes.Results
                   |> Seq.map(fun kpair -> printStepResult(kpair.Key, executionTime, kpair.Value.ToArray()))                       
                   |> String.concat ""

    String.Format("flow name: {0}; concurrent copies: {1} {2} {3}",
                  flowRes.FlowName, flowRes.ConcurrentCopies, Environment.NewLine, commands) + Environment.NewLine

let saveReport (report: string) = 
    Directory.CreateDirectory("reports") |> ignore
    let filePath = Path.Combine("reports", "report-" + DateTime.UtcNow.ToString("yyyy-dd-M--HH-mm-ss")) + ".txt"
    File.WriteAllText(filePath, report)