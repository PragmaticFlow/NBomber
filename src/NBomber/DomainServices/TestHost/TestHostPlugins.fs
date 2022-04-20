module internal NBomber.DomainServices.TestHost.TestHostPlugins

open System.Threading.Tasks

open Serilog
open FsToolkit.ErrorHandling

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Errors
open NBomber.Extensions.Internal
open NBomber.Infra.Dependency

//todo: add Polly for timout and retry logic, using cancel token
let init (dep: IGlobalDependency) (context: IBaseContext) (plugins: IWorkerPlugin list) = taskResult {
    try
        for plugin in plugins do
            dep.Logger.Information("Start init plugin: {PluginName}", plugin.PluginName)
            do! plugin.Init(context, dep.InfraConfig |> Option.defaultValue Constants.EmptyInfraConfig)
    with
    | ex -> return! AppError.createResult(InitScenarioError ex)
}

let start (logger: ILogger) (plugins: IWorkerPlugin list) = backgroundTask {
    for plugin in plugins do
        try
            plugin.Start() |> ignore
        with
        | ex -> logger.Warning(ex, "Failed to start plugin: {PluginName}", plugin.PluginName)
}

//todo: add Polly for timout and retry logic, using cancel token
let stop (logger: ILogger) (plugins: IWorkerPlugin list) = backgroundTask {
    for plugin in plugins do
        try
            logger.Information("Stop plugin: {PluginName}", plugin.PluginName)
            do! plugin.Stop()
        with
        | ex -> logger.Warning(ex, "Failed to stop plugin: {PluginName}", plugin.PluginName)
}

let getHints (plugins: IWorkerPlugin list) =
    plugins
    |> Seq.collect(fun plugin ->
        plugin.GetHints()
        |> Seq.map(fun x -> { SourceName = plugin.PluginName; SourceType = HintSourceType.WorkerPlugin; Hint = x })
    )
    |> Seq.toList

let getStats (logger: ILogger) (plugins: IWorkerPlugin list) (stats: NodeStats) = backgroundTask {
    try
        let pluginStatusesTask =
            plugins
            |> List.map(fun plugin -> plugin.GetStats stats)
            |> Task.WhenAll

        let! finishedTask = Task.WhenAny(pluginStatusesTask, Task.Delay(Constants.GetPluginStatsTimeout))
        if finishedTask.Id = pluginStatusesTask.Id then return pluginStatusesTask.Result
        else
            logger.Error("Getting plugin stats failed with the timeout error")
            return Array.empty
    with
    | ex ->
        logger.Error("Getting plugin stats failed: {0}", ex.ToString())
        return Array.empty
}



