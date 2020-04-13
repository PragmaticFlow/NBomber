module internal NBomber.DomainServices.Reporting.TxtReport

open System
open ConsoleTables

open NBomber.Contracts

let inline private concatLines s =
    String.concat Environment.NewLine s

let private printScenarioHeader (scnStats: ScenarioStats) =
    sprintf "scenario: '%s', duration: '%A'"
        scnStats.ScenarioName scnStats.Duration

let private printStepsTable (steps: StepStats[]) =
    let stepTable = ConsoleTable("step", "details")
    steps
    |> Seq.iteri(fun i s ->
        let dataInfoAvailable = s.AllDataMB > 0.0

        [ "- name", s.StepName
          "- request count", sprintf "all = %i | OK = %i | failed = %i" s.RequestCount s.OkCount s.FailCount
          "- response time", sprintf "RPS = %i | min = %i | mean = %i | max = %i" s.RPS s.Min s.Mean s.Max
          "- response time percentile", sprintf "50%% = %i | 75%% = %i | 95%% = %i | StdDev = %i" s.Percent50 s.Percent75 s.Percent95 s.StdDev

          if dataInfoAvailable then
            "- data transfer", sprintf "min = %gKb | mean = %gKb | max = %gKb | all = %gMB"
                                   s.MinDataKb s.MeanDataKb s.MaxDataKb s.AllDataMB
          if steps.Length > 1 && i < (steps.Length - 1) then
            "", ""
        ]
        |> Seq.iter(fun (name, value) -> stepTable.AddRow(name, value) |> ignore)
    )
    stepTable.ToStringAlternative()

let print (stats: NodeStats) =
    stats.ScenarioStats
    |> Array.map(fun scnStats ->
        [printScenarioHeader scnStats; printStepsTable scnStats.StepStats]
        |> concatLines
    )
    |> concatLines
