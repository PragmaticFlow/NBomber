module internal NBomber.DomainServices.TestHost.ReportingSinks

open Serilog
open FsToolkit.ErrorHandling

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Errors
open NBomber.Extensions.Internal
open NBomber.Infra.Dependency

//todo: add Polly for timout and retry logic, using cancel token
let init (dep: IGlobalDependency) (context: IBaseContext) (sinks: IReportingSink list) = taskResult {
    try
        for sink in sinks do
            dep.Logger.Information("Start init reporting sink: {SinkName}", sink.SinkName)
            do! sink.Init(context, dep.InfraConfig |> Option.defaultValue Constants.EmptyInfraConfig)
    with
    | ex -> return! AppError.createResult(InitScenarioError ex)
}

let start (logger: ILogger) (sinks: IReportingSink list) =
    for sink in sinks do
        try
            sink.Start() |> ignore
        with
        | ex -> logger.Warning(ex, "Failed to start reporting sink: {SinkName}", sink.SinkName)

//todo: add Polly for timout and retry logic, using cancel token
let stop (logger: ILogger) (sinks: IReportingSink list) = backgroundTask {
    for sink in sinks do
        try
            logger.Information("Stop reporting sink: {SinkName}", sink.SinkName)
            do! sink.Stop()
        with
        | ex -> logger.Warning(ex, "Failed to stop reporting sink: {SinkName}", sink.SinkName)
}

let saveRealtimeStats (dep: IGlobalDependency) (stats: ScenarioStats[]) = backgroundTask {
    if dep.NodeType <> NodeType.Agent then
        for sink in dep.ReportingSinks do
            try
                do! sink.SaveRealtimeStats stats
            with
            | ex -> dep.Logger.Fatal(ex, "Reporting sink: {SinkName} failed to save scenario stats", sink.SinkName)
}

let saveFinalStats (dep: IGlobalDependency) (finalStats: NodeStats) = backgroundTask {
    if dep.NodeType <> NodeType.Agent then
        for sink in dep.ReportingSinks do
            try
                do! sink.SaveFinalStats finalStats
            with
            | ex -> dep.Logger.Fatal(ex, "Reporting sink: {SinkName} failed to save scenario stats", sink.SinkName)
}
