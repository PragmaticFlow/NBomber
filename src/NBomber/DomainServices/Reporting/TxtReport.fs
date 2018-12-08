module internal rec NBomber.DomainServices.Reporting.TxtReport

open System
open ConsoleTables
open NBomber.Domain.StatisticsTypes

let print (stats: GlobalStats) = 
    stats.AllScenariosStats
    |> Array.map(fun x -> let header = printScenarioHeader(x)
                          let stepsTable = printStepsTable(x.StepsStats)
                          header + Environment.NewLine + stepsTable)
    |> String.concat(Environment.NewLine)

let private printScenarioHeader (scnStats: ScenarioStats) =
    String.Format("Scenario: '{0}', execution time: {1}, concurrent copies: {2}",
                  scnStats.ScenarioName,
                  scnStats.Duration.ToString(),
                  scnStats.ConcurrentCopies)

let private printStepsTable (steps: StepStats[]) =    
    let stepTable = ConsoleTable("step name", "request_count", "OK", "failed", "RPS", "min", "mean", "max", "50%", "75%", "95%", "StdDev")
    steps    
    |> Array.iteri(fun i s -> 
        stepTable.AddRow(s.StepName, s.ReqeustCount,
                         s.OkCount, s.FailCount,
                         s.Percentiles.Value.RPS, s.Percentiles.Value.Min, 
                         s.Percentiles.Value.Mean, s.Percentiles.Value.Max,
                         s.Percentiles.Value.Percent50, s.Percentiles.Value.Percent75,
                         s.Percentiles.Value.Percent95, s.Percentiles.Value.StdDev) |> ignore)
    
    stepTable.ToStringAlternative()