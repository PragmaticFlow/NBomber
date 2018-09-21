module internal rec NBomber.Reporting.TxtReport

open System
open System.IO
open System.Reflection
open System.Runtime.Versioning

open ConsoleTables

open NBomber.Contracts
open NBomber.Domain
open NBomber.Statistics
open NBomber.Infra.Dependency
open NBomber.Infra.EnvironmentInfo

let print (dep: Dependency, scResult: ScenarioStats) = 

    let header = printScenarioHeader(dep.Scenario)

    let flowTable = scResult.TestFlowsStats 
                    |> Array.mapi(fun i x -> printFlowTable(x, i + 1))
                    |> String.concat(Environment.NewLine)

    printEnvInfo(dep.EnvironmentInfo) + Environment.NewLine 
    + header + Environment.NewLine + Environment.NewLine
    + flowTable

let private printEnvInfo (envInfo: EnvironmentInfo) =
    ""

let private printScenarioHeader (scenario: Scenario) =
    String.Format("Scenario: {0}, execution time: {1}",
                  scenario.ScenarioName,
                  scenario.Duration.ToString())

let private printFlowTable (flResult: TestFlowStats, flowNo: int) =
    
    let consoleTableOptions = 
        ConsoleTableOptions(
            Columns = [String.Format("flow {0}: {1}", flowNo, flResult.FlowName)
                       "steps"; String.Format("concurrent copies: {0}", flResult.ConcurrentCopies)],
            EnableCount = false)

    let flowTable = ConsoleTable(consoleTableOptions)       
    flResult.StepsStats
    |> Array.iteri(fun i stats -> flowTable.AddRow("", String.Format("{0} - {1}", i + 1, stats.StepName), "") |> ignore)

    let stepsTable = printStepsTable(flResult.StepsStats)    
    flowTable.ToString() + stepsTable + Environment.NewLine + Environment.NewLine

let private printStepsTable (steps: StepStats[]) =    
    let stepTable = ConsoleTable("step no", "request_count", "OK", "failed", "RPS", "min", "mean", "max", "50%", "75%", "95%", "StdDev")
    steps    
    |> Array.iteri(fun i s -> 
        stepTable.AddRow(i + 1, s.OkLatencies.Length,
                         s.OkCount, s.FailCount,
                         s.LatencyDetails.Value.RPS, s.LatencyDetails.Value.Min, 
                         s.LatencyDetails.Value.Mean, s.LatencyDetails.Value.Max,
                         s.LatencyDetails.Value.Percent50, s.LatencyDetails.Value.Percent75,
                         s.LatencyDetails.Value.Percent95, s.LatencyDetails.Value.StdDev) |> ignore)
    
    stepTable.ToStringAlternative()