module internal NBomber.DomainServices.TestHost.TestHostReporting

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.NonAffine
open FsToolkit.ErrorHandling

open NBomber
open NBomber.Contracts
open NBomber.Errors
open NBomber.Extensions.InternalExtensions
open NBomber.Infra.Dependency

let saveRealtimeStats (sinks: IReportingSink list) (nodeStats: NodeStats list) =
    sinks
    |> List.map(fun x -> nodeStats |> List.toArray |> x.SaveStats)
    |> Task.WhenAll

let saveFinalStats (dep: IGlobalDependency) (stats: NodeStats list) = task {
    for sink in dep.ReportingSinks do
        try
            do! sink.SaveStats(stats |> Seq.toArray)
        with
        | ex -> dep.Logger.Warning(ex, "Reporting sink '{SinkName}' failed to save stats.", sink.SinkName)
}

let createReportingTimer (dep: IGlobalDependency,
                          sendStatsInterval: TimeSpan,
                          getData: unit -> (NodeOperationType * NodeStats)) =

        let timer = new System.Timers.Timer(sendStatsInterval.TotalMilliseconds)
        timer.Elapsed.Add(fun _ ->
            let (operation,nodeStats) = getData()
            match operation with
            | NodeOperationType.Bombing ->
                if not (List.isEmpty dep.ReportingSinks) then
                    nodeStats
                    |> List.singleton
                    |> saveRealtimeStats dep.ReportingSinks
                    |> ignore
            | _ -> ()
        )
        timer

let initReportingSinks (dep: IGlobalDependency) (context: IBaseContext) = taskResult {
    try
        for sink in dep.ReportingSinks do
            dep.Logger.Information("Start init reporting sink: '{SinkName}'.", sink.SinkName)
            do! sink.Init(context, dep.InfraConfig |> Option.defaultValue Constants.EmptyInfraConfig)
    with
    | ex -> return! AppError.createResult(InitScenarioError ex)
}

let startReportingSinks (dep: IGlobalDependency) = task {
    for sink in dep.ReportingSinks do
        try
            sink.Start() |> ignore
        with
        | ex -> dep.Logger.Warning(ex, "Failed to start reporting sink '{SinkName}'.", sink.SinkName)
}

let stopReportingSinks (dep: IGlobalDependency) = task {
    for sink in dep.ReportingSinks do
        try
            dep.Logger.Information("Stop reporting sink: '{SinkName}'.", sink.SinkName)
            do! sink.Stop()
        with
        | ex -> dep.Logger.Warning(ex, "Stop reporting sink '{SinkName}' failed.", sink.SinkName)
}
