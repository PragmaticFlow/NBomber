module internal rec NBomber.Reporting

open System
open System.IO
open System.Reflection
open System.Runtime.Versioning

open ConsoleTables

open NBomber.Contracts
open NBomber.Domain
open NBomber.Statistics

let buildReport (scenario: Scenario, flowInfo: FlowInfo[]) = 
    let getPausedTime (scenario: Scenario) =
        scenario.TestFlows
        |> Array.collect(fun x -> x.Steps)
        |> Array.sumBy(fun x -> match x with | Pause time -> time.Ticks | _ -> int64 0)
        |> TimeSpan

    let envInfo = HostEnvironmentInfo.getEnvironmentInfo()   
    let header  = printScenarioHeader(scenario)
    
    let actualTime = scenario.Duration - getPausedTime(scenario)

    let flowTable = flowInfo 
                    |> Array.mapi(fun i x -> printFlowTable(x, actualTime, i+1))
                    |> String.concat(Environment.NewLine)

    envInfo + Environment.NewLine + header + Environment.NewLine + Environment.NewLine + flowTable                 

let printScenarioHeader (scenario: Scenario) =
    String.Format("Scenario: {0}, execution time: {1}", scenario.ScenarioName, scenario.Duration.ToString())

let printFlowTable (flowStats: FlowInfo, activeStepsDuration: TimeSpan, flowCount: int) =
    
    let consoleTableOptions = 
        ConsoleTableOptions(
            Columns = [String.Format("flow {0}: {1}", flowCount, flowStats.FlowName)
                       "steps"; String.Format("concurrent copies: {0}", flowStats.ConcurrentCopies)],
            EnableCount = false)

    let flowTable = ConsoleTable(consoleTableOptions)       
    flowStats.Steps
    |> Array.iteri(fun i stats -> flowTable.AddRow("", String.Format("{0} - {1}", i + 1, stats.StepName), "") |> ignore)

    let stepsTable = printStepsTable(flowStats.Steps, activeStepsDuration)    
    flowTable.ToString() + stepsTable + Environment.NewLine + Environment.NewLine

let printStepsTable (steps: StepInfo[], activeStepsDuration: TimeSpan) =
    let stepTable = ConsoleTable("step no", "request_count", "OK", "failed", "exceptions", "RPS", "min", "mean", "max", "50%", "70%")
    steps
    |> Array.mapi(fun i stInfo -> StepStats.create(i + 1, stInfo, activeStepsDuration))
    |> Array.iter(fun s -> stepTable.AddRow(s.StepNo, s.Info.Latencies.Length,
                                            s.Info.OkCount, s.Info.FailCount, s.Info.ExceptionCount,
                                            s.RPS, s.Min, s.Mean, s.Max, s.Percent50, s.Percent75) |> ignore)
    stepTable.ToStringAlternative()

let saveReport (report: string) = 
    Directory.CreateDirectory("reports") |> ignore
    let filePath = Path.Combine("reports", "report-" + DateTime.UtcNow.ToString("yyyy-dd-M--HH-mm-ss")) + ".txt"
    File.WriteAllText(filePath, report)
    report

        
module HostEnvironmentInfo =    

    let getEnvironmentInfo () =
        let assembly = Assembly.GetAssembly(typedefof<Request>)
        let assemblyVersion = assembly.GetName().Version.ToString()
        let assemblyName = assembly.GetName().Name
        let dotNetVersion = assembly.GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName;
        let os = Environment.OSVersion.ToString()

        let versionsAndProcessorCountInfo =
            "{0}:v{1}, OS:{2}, Processor Count:{3}" + Environment.NewLine +
            "Target Runtime Version: {4}" + Environment.NewLine

        let processor =
            match Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") with
            | null -> String.Empty
            | processor -> processor + Environment.NewLine

        let processorArchitecture =
            match Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") with
            | null -> String.Empty
            | architecture -> "Processor Architecture:" + architecture + Environment.NewLine

        let environmentInfo = versionsAndProcessorCountInfo + processor + processorArchitecture
        String.Format(environmentInfo, assemblyName, assemblyVersion, os, Environment.ProcessorCount, dotNetVersion)