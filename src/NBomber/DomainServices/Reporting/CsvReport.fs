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
    String.Format(format,
                  testInfo.TestSuite, testInfo.TestName,
                  scenarioName, duration, stats.StepName,
                  stats.RequestCount, stats.OkCount, stats.FailCount,
                  stats.RPS, stats.Min, stats.Mean, stats.Max,
                  stats.Percent50, stats.Percent75, stats.Percent95, stats.Percent99, stats.StdDev,
                  stats.MinDataKb, stats.MeanDataKb, stats.MaxDataKb, stats.AllDataMB)

let private printSteps (testInfo: TestInfo, scnStats: ScenarioStats) =
    scnStats.StepStats
    |> Array.map(fun stepStats -> getLine(scnStats.ScenarioName, scnStats.Duration, stepStats, testInfo))
    |> String.concat(Environment.NewLine)

let print (testInfo: TestInfo, stats: NodeStats) =
    let header = getHeader()
    let body = stats.ScenarioStats |> Array.map(fun stats -> printSteps(testInfo, stats)) |> String.concat(String.Empty)
    header + Environment.NewLine + body
