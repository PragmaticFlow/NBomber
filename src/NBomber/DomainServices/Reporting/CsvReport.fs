module internal NBomber.DomainServices.Reporting.CsvReport

open System
open NBomber.Contracts

let private getHeader () =
    "scenario,duration,step_name,request_count,ok,failed," +
    "rps,min,mean,max,50_percent,75_percent,95_percent,std_dev," +
    "data_transfer_min_kb,data_transfer_mean_kb,data_transfer_max_kb,data_transfer_all_mb"

let private getLine (scenarioName: string, duration: TimeSpan, stats: StepStats) =
    String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}",
                  scenarioName, duration, stats.StepName,
                  stats.RequestCount, stats.OkCount, stats.FailCount,
                  stats.RPS, stats.Min, stats.Mean, stats.Max,
                  stats.Percent50, stats.Percent75, stats.Percent95, stats.StdDev,
                  stats.MinDataKb, stats.MeanDataKb, stats.MaxDataKb, stats.AllDataMB)

let private printSteps (scnStats: ScenarioStats) =
    scnStats.StepStats
    |> Array.map(fun stepStats -> getLine(scnStats.ScenarioName, scnStats.Duration, stepStats))
    |> String.concat(Environment.NewLine)

let print (stats: NodeStats) =
    let header = getHeader()
    let body = stats.ScenarioStats |> Array.map(printSteps) |> String.concat(String.Empty)
    header + Environment.NewLine + body
