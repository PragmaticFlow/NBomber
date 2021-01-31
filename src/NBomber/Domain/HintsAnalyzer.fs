module internal NBomber.Domain.HintsAnalyzer

open NBomber.Contracts

type SourceType =
    | Scenario
    | WorkerPlugin

type HintResult = {
    SourceName: string
    SourceType: SourceType
    Hint: string
}

let private analyzeScenarioFails (scnStats: ScenarioStats[]) =

    let printHint (scn: ScenarioStats) =
        $"Scenario '{scn.ScenarioName}' has '{scn.FailCount}' errors that affect overall statistics." +
        " So make sure that your load tests doesn't have errors."

    scnStats
    |> Seq.filter(fun x -> x.FailCount > 0)
    |> Seq.map(fun x -> { SourceName = x.ScenarioName; SourceType = Scenario; Hint = printHint x })

let private analyzeRPS (scnStats: ScenarioStats[]) =

    let printHint (scnName, stepName) =
        $"Step '{stepName}' in scenario '{scnName}' has RPS '0' because all response latencies (min/mean/max)" +
        " are higher than 1 sec which means there is no request that finished in less than 1 sec."

    scnStats
    |> Seq.collect(fun scn ->
        scn.StepStats
        |> Seq.filter(fun step -> step.Ok.Request.RPS = 0.0)
        |> Seq.map(fun step -> scn.ScenarioName, step.StepName)
    )
    |> Seq.map(fun (scnName,stepName) -> { SourceName = scnName; SourceType = Scenario; Hint = printHint(scnName, stepName) })

let private analyzeAllDataMb (scnStats: ScenarioStats[]) =

    let printHint (scnName, stepName) =
        $"Step '{stepName}' in scenario '{scnName}' didn't track data transfer." +
        " In order to track data transfer, you should use Response.Ok(sizeInBytes: value)"

    scnStats
    |> Seq.collect(fun scn ->
        scn.StepStats
        |> Seq.filter(fun step -> step.Ok.DataTransfer.AllMB + step.Fail.DataTransfer.AllMB = 0.0)
        |> Seq.map(fun step -> scn.ScenarioName, step.StepName)
    )
    |> Seq.map(fun (scnName,stepName) -> { SourceName = scnName; SourceType = Scenario; Hint = printHint(scnName, stepName) })

let analyze (stats: NodeStats) =
    seq {
        yield! analyzeScenarioFails stats.ScenarioStats
        yield! analyzeRPS stats.ScenarioStats
        yield! analyzeAllDataMb stats.ScenarioStats
    }
    |> Seq.toList
