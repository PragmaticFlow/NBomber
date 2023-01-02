module internal NBomber.DomainServices.TestHost.WorkerPlugins

open System.Threading.Tasks
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
        for plugin in dep.WorkerPlugins do
            dep.LogInfo("Start init plugin: {0}", plugin.PluginName)
            do! plugin.Init(context, dep.InfraConfig |> Option.defaultValue Constants.EmptyInfraConfig)
    with
    | ex -> return! AppError.createResult(InitScenarioError ex)
}

let start (dep: IGlobalDependency) =
    for plugin in dep.WorkerPlugins do
        try
            plugin.Start() |> ignore
        with
        | ex -> dep.LogWarn(ex, "Failed to start plugin: {0}", plugin.PluginName)

//todo: add Polly for timout and retry logic, using cancel token
let stop (dep: IGlobalDependency) = backgroundTask {
    for plugin in dep.WorkerPlugins do
        try
            dep.LogInfo("Stop plugin: {0}", plugin.PluginName)
            do! plugin.Stop()
        with
        | ex -> dep.LogWarn(ex, "Failed to stop plugin: {0}", plugin.PluginName)
}

let getHints (plugins: IWorkerPlugin list) =
    plugins
    |> Seq.collect(fun plugin ->
        plugin.GetHints()
        |> Seq.map(fun x -> { SourceName = plugin.PluginName; SourceType = HintSourceType.WorkerPlugin; Hint = x })
    )
    |> Seq.toList

let getStats (dep: IGlobalDependency) (stats: NodeStats) = backgroundTask {
    try
        let pluginStatusesTask =
            dep.WorkerPlugins
            |> List.map(fun plugin -> plugin.GetStats stats)
            |> Task.WhenAll

        let! finishedTask = Task.WhenAny(pluginStatusesTask, Task.Delay(Constants.GetPluginStatsTimeout))
        if finishedTask.Id = pluginStatusesTask.Id then return pluginStatusesTask.Result
        else
            dep.LogWarn "Getting plugin stats failed with the timeout error"
            return Array.empty
    with
    | ex ->
        dep.LogWarn(ex, "Getting plugin stats failed")
        return Array.empty
}



