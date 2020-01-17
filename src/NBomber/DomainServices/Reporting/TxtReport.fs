module internal rec NBomber.DomainServices.Reporting.TxtReport

open System
open ConsoleTables
open NBomber.Domain

let print (stats: RawNodeStats) = 
    stats.AllScenariosStats
    |> Array.map(fun x -> let header = printScenarioHeader(x)
                          let stepsTable = printStepsTable(x.StepsStats)
                          header + Environment.NewLine + stepsTable)
    |> String.concat(Environment.NewLine)

let private printScenarioHeader (scnStats: ScenarioStats) =
    sprintf "scenario: '%s', duration: '%A', RPS: '%i', concurrent Copies: '%i'" scnStats.ScenarioName scnStats.Duration scnStats.RPS scnStats.ConcurrentCopies

let private printStepsTable (steps: StepStats[]) =
    
    let dataInfoAvailable = steps |> Array.exists(fun x -> x.DataTransfer.AllMB > 0.0)

    let stepTable = ConsoleTable("step", "details")
    steps    
    |> Array.iteri(fun i s ->         
        stepTable.AddRow("- name", s.StepName) |> ignore
        stepTable.AddRow("- request count", sprintf "all = %i | OK = %i | failed = %i" s.ReqeustCount s.OkCount s.FailCount) |> ignore
        stepTable.AddRow("- response time", sprintf "RPS = %i | min = %i | mean = %i | max = %i" s.RPS s.Min s.Mean s.Max) |> ignore
        stepTable.AddRow("- response time percentile", sprintf "50%% = %i | 75%% = %i | 95%% = %i | StdDev = %i" s.Percent50 s.Percent75 s.Percent95 s.StdDev) |> ignore
        
        if dataInfoAvailable then
            stepTable.AddRow("- data transfer", sprintf "min = %gKb | mean = %gKb | max = %gKb | all = %gMB" s.DataTransfer.MinKb s.DataTransfer.MeanKb s.DataTransfer.MaxKb s.DataTransfer.AllMB) |> ignore
        else
            stepTable.AddRow("- data transfer", "min = - | mean = - | max = - | all = -") |> ignore

        if steps.Length > 1 && i < (steps.Length - 1) then
            stepTable.AddRow("", "") |> ignore
    )

    stepTable.ToStringAlternative()