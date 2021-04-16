module internal NBomber.DomainServices.Reporting.CsvReport

open System
open NBomber.Contracts

let private separator = ","

let private getHeader () =
    ["test_suite"; "test_name"
     "scenario"; "duration"; "step_name";
     "request_count"; "ok"; "failed"
     "rps"; "min"; "mean"; "max";
     "50_percent"; "75_percent"; "95_percent"; "99_percent"; "std_dev"
     "data_transfer_min_kb"; "data_transfer_mean_kb"; "data_transfer_max_kb"; "data_transfer_all_mb"]
    |> String.concat(separator)

let private getLine (scenarioName: string, duration: TimeSpan, stats: StepStats, testInfo: TestInfo) =
    let format = seq {0 .. 19} |> Seq.map(sprintf "{%i}") |> String.concat(separator) // {0},{1},{2},...
    let okCount = stats.Ok.Request.Count
    let failCount = stats.Fail.Request.Count
    let reqCount = okCount + failCount
    let okRPS = stats.Ok.Request.RPS
    let lt = stats.Ok.Latency
    let dt = stats.Ok.DataTransfer

    String.Format(format,
                  testInfo.TestSuite, testInfo.TestName,
                  scenarioName, duration, stats.StepName,
                  reqCount, okCount, failCount,
                  okRPS, lt.MinMs, lt.MeanMs, lt.MaxMs,
                  lt.Percent50, lt.Percent75, lt.Percent95, lt.Percent99, lt.StdDev,
                  dt.MinBytes, dt.MeanBytes, dt.MaxBytes, dt.AllBytes)

let private printSteps (testInfo: TestInfo) (scnStats: ScenarioStats) =
    scnStats.StepStats
    |> Array.map(fun stepStats -> getLine(scnStats.ScenarioName, scnStats.Duration, stepStats, testInfo))
    |> String.concat Environment.NewLine

let print (nodeStats: NodeStats) =
    let header = getHeader()

    let body = nodeStats.ScenarioStats
               |> Array.map(printSteps nodeStats.TestInfo)
               |> String.concat String.Empty

    header + Environment.NewLine + body
