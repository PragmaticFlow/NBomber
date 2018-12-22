module internal NBomber.DomainServices.Reporting.CsvReport

open System
open NBomber.Domain.StatisticsTypes

let private getHeader () =
    "scenario,execution_time,step_name,request_count,ok,failed," +
    "rps,min,mean,max,50_percent,75_percent,95_percent,std_dev"

let private getLine (scenarioName: string, duration: TimeSpan, stats: StepStats) =
    sprintf "%s,%O,%s,%i,%i,%i,%i,%i,%i,%i,%i,%i,%i,%i"
            scenarioName duration stats.StepName stats.ReqeustCount
            stats.OkCount stats.FailCount stats.Percentiles.Value.RPS stats.Percentiles.Value.Min
            stats.Percentiles.Value.Mean stats.Percentiles.Value.Max stats.Percentiles.Value.Percent50
            stats.Percentiles.Value.Percent75 stats.Percentiles.Value.Percent95
            stats.Percentiles.Value.StdDev

let private printSteps (stats: ScenarioStats) =
    stats.StepsStats
    |> Array.map(fun x -> getLine(stats.ScenarioName, stats.Duration, x))
    |> String.concat Environment.NewLine

let print (stats: GlobalStats) =
    let header = getHeader()
    let body = stats.AllScenariosStats |> Array.map(printSteps) |> String.concat("")
    header + Environment.NewLine + body