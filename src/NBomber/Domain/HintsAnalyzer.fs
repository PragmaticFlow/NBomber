module internal NBomber.Domain.HintsAnalyzer

open NBomber.Contracts.Stats

let private analyzeDataTransfer (scnStats: ScenarioStats) =

    let printHint (scnName, stepName) =
        $"Step: '{stepName}' in Scenario: '{scnName}' didn't track data transfer." +
        " In order to track data transfer, you should use Response.Ok(sizeInBytes: value)"

    scnStats.StepStats
    |> Seq.filter(fun step -> step.Ok.DataTransfer.MinBytes + step.Fail.DataTransfer.MinBytes = 0)
    |> Seq.map(fun step -> {
        SourceName = scnStats.ScenarioName
        SourceType = HintSourceType.Scenario
        Hint = printHint(scnStats.ScenarioName, step.StepName)
    })

let private analyzeStatusCodes (scnStats: ScenarioStats) =

    let printHint (scnName, stepName) =
        $"Step: '{stepName}' in Scenario: '{scnName}' didn't track status code." +
        " In order to track status code, you should use Response.Ok(statusCode: value)"

    scnStats.StepStats
    |> Seq.filter(fun step ->
        (step.Ok.Request.Count > 0 && Array.isEmpty step.Ok.StatusCodes) ||
        (step.Fail.Request.Count > 0 && Array.isEmpty step.Fail.StatusCodes)
    )
    |> Seq.map(fun step -> {
        SourceName = scnStats.ScenarioName
        SourceType = HintSourceType.Scenario
        Hint = printHint(scnStats.ScenarioName, step.StepName)
    })

let analyzeNodeStats (stats: NodeStats) =
    stats.ScenarioStats
    |> Seq.collect(fun scnStats ->
        scnStats
        |> analyzeDataTransfer
        |> Seq.append(analyzeStatusCodes scnStats)
    )
    |> Seq.toList
