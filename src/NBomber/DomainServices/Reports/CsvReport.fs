module internal NBomber.DomainServices.Reports.CsvReport

open System
open Serilog
open NBomber.Contracts.Stats
open NBomber.Extensions.Internal

let private separator = ","

let private getHeader () =
    ["test_suite"; "test_name"
     "scenario"; "duration"; "step_name";
     "request_count"; "ok"; "failed"
     "rps"; "min"; "mean"; "max";
     "50_percent"; "75_percent"; "95_percent"; "99_percent"; "std_dev"
     "data_transfer_min_kb"; "data_transfer_mean_kb"; "data_transfer_max_kb"; "data_transfer_all_mb"]
    |> String.concat(separator)

let private toKb (bytes: int) =
    bytes |> Converter.fromBytesToKb

let private toMb (bytes: int64) =
    bytes |> Converter.fromBytesToMb

let private getLine (scenarioName: string, duration: TimeSpan, stats: StepStats, testInfo: TestInfo) =
    let format = [0 .. 20] |> List.map(fun x -> $"{{{x}}}") |> String.concat separator // {0},{1},{2},...
    let okCount = stats.Ok.Request.Count
    let failCount = stats.Fail.Request.Count
    let reqCount = okCount + failCount
    let okRPS = stats.Ok.Request.RPS
    let lt = stats.Ok.Latency
    let dt = stats.Ok.DataTransfer

    String.Format(System.Globalization.CultureInfo.InvariantCulture, format,
                  testInfo.TestSuite, testInfo.TestName,
                  scenarioName, duration, stats.StepName,
                  reqCount, okCount, failCount,
                  okRPS, lt.MinMs, lt.MeanMs, lt.MaxMs,
                  lt.Percent50, lt.Percent75, lt.Percent95, lt.Percent99, lt.StdDev,
                  dt.MinBytes |> toKb, dt.MeanBytes |> toKb, dt.MaxBytes |> toKb, dt.AllBytes |> toMb)

let private printSteps (testInfo: TestInfo) (scnStats: ScenarioStats) =
    scnStats.StepStats
    |> Array.map(fun stepStats -> getLine(scnStats.ScenarioName, scnStats.Duration, stepStats, testInfo))
    |> String.concat Environment.NewLine

let print (logger: ILogger) (sessionResult: NodeSessionResult) =
    try
        logger.Verbose("CsvReport.print")

        let header = getHeader()

        let body = sessionResult.FinalStats.ScenarioStats
                   |> Array.map(printSteps sessionResult.FinalStats.TestInfo)
                   |> String.concat Environment.NewLine

        header + Environment.NewLine + body
    with
    | ex ->
        logger.Error(ex, "CsvReport.print failed")
        "Could not generate report"

