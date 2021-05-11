module internal NBomber.Domain.HintsAnalyzer

open NBomber.Contracts.Stats

let private analyzeDataTransfer (scnStats: ScenarioStats[]) =

    let printHint (scnName, stepName) =
        $"Step '{stepName}' in scenario '{scnName}' didn't track data transfer." +
        " In order to track data transfer, you should use Response.Ok(sizeInBytes: value)"

    scnStats
    |> Seq.collect(fun scn ->
        scn.StepStats
        |> Seq.filter(fun step -> step.Ok.DataTransfer.MinBytes + step.Fail.DataTransfer.MinBytes = 0)
        |> Seq.map(fun step -> scn.ScenarioName, step.StepName)
    )
    |> Seq.map(fun (scnName,stepName) -> {
        SourceName = scnName
        SourceType = HintSourceType.Scenario
        Hint = printHint(scnName, stepName)
    })

let analyze (stats: NodeStats) =
    seq {
        yield! analyzeDataTransfer stats.ScenarioStats
    }
    |> Seq.toList
