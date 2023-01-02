module internal NBomber.DomainServices.TestHost.ReportingSinks

open FsToolkit.ErrorHandling
open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Errors
open NBomber.Extensions.Internal
open NBomber.Infra

//todo: add Polly for timout and retry logic, using cancel token
let init (dep: IGlobalDependency) (context: IBaseContext) = taskResult {
    try
        for sink in dep.ReportingSinks do
            dep.LogInfo("Start init reporting sink: {0}", sink.SinkName)
            do! sink.Init(context, dep.InfraConfig |> Option.defaultValue Constants.EmptyInfraConfig)
    with
    | ex -> return! AppError.createResult(InitScenarioError ex)
}

let start (dep: IGlobalDependency) =
    for sink in dep.ReportingSinks do
        try
            sink.Start() |> ignore
        with
        | ex -> dep.LogWarn(ex, "Failed to start reporting sink: {0}", sink.SinkName)

//todo: add Polly for timout and retry logic, using cancel token
let stop (dep: IGlobalDependency) = backgroundTask {
    for sink in dep.ReportingSinks do
        try
            dep.LogInfo("Stop reporting sink: {0}", sink.SinkName)
            do! sink.Stop()
        with
        | ex -> dep.LogWarn(ex, "Failed to stop reporting sink: {0}", sink.SinkName)
}

let saveRealtimeStats (dep: IGlobalDependency) (stats: ScenarioStats[]) = backgroundTask {
    if dep.NodeType <> NodeType.Agent then
        for sink in dep.ReportingSinks do
            try
                do! sink.SaveRealtimeStats stats
            with
            | ex -> dep.LogWarn(ex, "Reporting sink: {0} failed to save realtime scenario stats", sink.SinkName)
}

let saveFinalStats (dep: IGlobalDependency) (finalStats: NodeStats) = backgroundTask {
    if dep.NodeType <> NodeType.Agent then
        for sink in dep.ReportingSinks do
            try
                do! sink.SaveFinalStats finalStats
            with
            | ex -> dep.LogWarn(ex, "Reporting sink: {0} failed to save final scenario stats", sink.SinkName)
}
