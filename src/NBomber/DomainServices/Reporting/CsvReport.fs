module internal rec NBomber.DomainServices.Reporting.CsvReport

open System
open ConsoleTables
open NBomber.Domain.StatisticsTypes

let print (stats: GlobalStats) =
    let header = printHeader()

    stats.AllScenariosStats
    |> Array.map(fun x -> printSteps(x))
    |> String.concat(Environment.NewLine)

let private printHeader () =
    "Scenario, Execution time, request_count, OK, failed, RPS, min, mean, max, 50%, 75%, 95%, StdDev"

let private printSteps (stats: ScenarioStats) : string [] =
    stats.StepsStats
    |> Array.map(fun s ->
        String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}",
            stats.ScenarioName, stats.Duration.ToString(),
            s.StepName, s.ReqeustCount, s.OkCount, s.FailCount,
            s.Percentiles.Value.RPS, s.Percentiles.Value.Min,
            s.Percentiles.Value.Mean, s.Percentiles.Value.Max,
            s.Percentiles.Value.Percent50, s.Percentiles.Value.Percent75,
            s.Percentiles.Value.Percent95, s.Percentiles.Value.StdDev))