module internal rec NBomber.DomainServices.Reporting.CsvReport

open System
open NBomber.Domain.StatisticsTypes

let print (stats: GlobalStats) =
    let header = printHeader()
    let body = stats.AllScenariosStats |> Array.map printSteps |> String.concat ""
    header + Environment.NewLine + body

let private printHeader () =
    sprintf "%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s"
            "Scenario" "Execution time" "step name" "request_count"
            "OK" "failed" "RPS" "min" "mean" "max" "50%" "75%"
            "95%" "StdDev"

let private printStep (scenarioName: string, duration: TimeSpan, stats: StepStats) =
    sprintf "%s, %O, %s, %i, %i, %i, %i, %i, %i, %i, %i, %i, %i, %i"
            scenarioName duration stats.StepName stats.ReqeustCount
            stats.OkCount stats.FailCount stats.Percentiles.Value.RPS stats.Percentiles.Value.Min
            stats.Percentiles.Value.Mean stats.Percentiles.Value.Max stats.Percentiles.Value.Percent50
            stats.Percentiles.Value.Percent75 stats.Percentiles.Value.Percent95
            stats.Percentiles.Value.StdDev

let private printSteps (stats: ScenarioStats) =
    stats.StepsStats
    |> Array.map(fun s -> printStep(stats.ScenarioName, stats.Duration, s))
    |> String.concat(Environment.NewLine)    