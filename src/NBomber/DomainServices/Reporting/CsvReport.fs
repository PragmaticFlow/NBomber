module internal NBomber.DomainServices.Reporting.CsvReport

open System
open NBomber.Domain

let private getHeader () =
    "scenario,execution_time,step_name,request_count,ok,failed," +
    "rps,min,mean,max,50_percent,75_percent,95_percent,std_dev," +
    "data_transfer_min_kb,data_transfer_mean_kb,data_transfer_max_kb,data_transfer_all_mb"

let private getLine (scenarioName: string, duration: TimeSpan, stats: StepStats) =
    let dt = stats.DataTransfer
    String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}",
                  scenarioName, duration, stats.StepName,
                  stats.ReqeustCount, stats.OkCount, stats.FailCount,
                  stats.RPS, stats.Min, stats.Mean, stats.Max,
                  stats.Percent50, stats.Percent75, stats.Percent95, stats.StdDev,
                  dt.MinKb, dt.MeanKb, dt.MaxKb, dt.AllMB)

let private printSteps (stats: ScenarioStats) =
    stats.StepsStats
    |> Array.map(fun x -> getLine(stats.ScenarioName, stats.Duration, x))
    |> String.concat Environment.NewLine

let print (stats: NodeStats) =
    let header = getHeader()
    let body = stats.AllScenariosStats |> Array.map(printSteps) |> String.concat("")
    header + Environment.NewLine + body
