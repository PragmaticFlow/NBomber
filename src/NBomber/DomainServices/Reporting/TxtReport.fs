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
    String.Format("Scenario: {0}, execution time: {1}",
                  scnStats.ScenarioName,
                  scnStats.Duration.ToString())

let private printStepsTable (steps: StepStats[]) = 
    
    let dataInfoAvailable = steps |> Array.exists(fun x -> x.DataTransfer.AllMB > 0.0)

    let stepTable = ConsoleTable("step", "details")
    steps    
    |> Array.iteri(fun i s -> 
        let p = s.Percentiles.Value
        stepTable.AddRow("- name", s.StepName) |> ignore
        stepTable.AddRow("- request count", String.Format("all = {0} | OK = {1} | failed = {2}", s.ReqeustCount, s.OkCount, s.FailCount)) |> ignore
        stepTable.AddRow("- response time", String.Format("RPS = {0} | min = {1} | mean = {2} | max = {3} ", p.RPS, p.Min, p.Mean, p.Max)) |> ignore
        stepTable.AddRow("- response time percentile", String.Format("50% = {0} | 75% = {1} | 95% = {2} | StdDev= {3}", p.Percent50, p.Percent75, p.Percent95, p.StdDev)) |> ignore
        
        if dataInfoAvailable then
            stepTable.AddRow("- data transfer", String.Format("min = {0}KB | mean = {1}KB | max = {2}KB | all = {3}MB", s.DataTransfer.MinKB, s.DataTransfer.MeanKB, s.DataTransfer.MaxKB, s.DataTransfer.AllMB)) |> ignore
        else
            stepTable.AddRow("- data transfer", "no information") |> ignore

        if steps.Length > 1 && i < (steps.Length - 1) then
            stepTable.AddRow("", "") |> ignore
        )
    
    stepTable.ToStringAlternative()