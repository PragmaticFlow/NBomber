module internal NBomber.DomainServices.TestHost.TestHostReportingSinks

open Serilog
open FsToolkit.ErrorHandling
open FSharp.Control.Tasks.NonAffine

open NBomber
open NBomber.Contracts
open NBomber.Errors
open NBomber.Extensions.InternalExtensions
open NBomber.Infra.Dependency

let init (dep: IGlobalDependency) (context: IBaseContext) = taskResult {
    try
        for sink in dep.ReportingSinks do
            dep.Logger.Information("Start init reporting sink: '{SinkName}'.", sink.SinkName)
            do! sink.Init(context, dep.InfraConfig |> Option.defaultValue Constants.EmptyInfraConfig)
    with
    | ex -> return! AppError.createResult(InitScenarioError ex)
}

let start (logger: ILogger) (sinks: IReportingSink list) = task {
    for sink in sinks do
        try
            sink.Start() |> ignore
        with
        | ex -> logger.Warning(ex, "Failed to start reporting sink '{SinkName}'.", sink.SinkName)
}

let stop (logger: ILogger) (sinks: IReportingSink list) = task {
    for sink in sinks do
        try
            logger.Information("Stop reporting sink: '{SinkName}'.", sink.SinkName)
            do! sink.Stop()
        with
        | ex -> logger.Warning(ex, "Stop reporting sink '{SinkName}' failed.", sink.SinkName)
}
