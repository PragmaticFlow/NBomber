module internal NBomber.DomainServices.TestHost.TestHostConsole

open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Reactive
open FsToolkit.ErrorHandling
open Spectre.Console

open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Extensions.Data
open NBomber.Extensions.Internal
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler
open NBomber.Domain.ClientPool
open NBomber.Infra
open NBomber.Infra.Dependency

let printTargetScenarios (dep: IGlobalDependency) (targetScns: Scenario list) =
    targetScns
    |> List.map(fun x -> x.ScenarioName)
    |> fun targets -> dep.Logger.Information("Target scenarios: {TargetScenarios}", String.concatWithComma targets)

let displayStatus (dep: IGlobalDependency) (msg: string) (runAction: StatusContext option -> Task<'T>) =
    if dep.ApplicationType = ApplicationType.Console then
        let status = AnsiConsole.Status()
        status.StartAsync(msg, fun ctx -> runAction (Some ctx))
    else
        dep.Logger.Information msg
        runAction None

let displayClientPoolProgress (dep: IGlobalDependency, consoleStatus: StatusContext option, pool: ClientPool) =

    if consoleStatus.IsSome then

        pool.EventStream
        |> Observable.takeWhile(function
            | InitFinished -> false
            | InitFailed   -> false
            | _            -> true
        )
        |> Observable.subscribe(function
            | StartedInit (poolName, clientCount) ->
                dep.Logger.Information("Start init client factory: {0}, client count: {1}", poolName, clientCount)

            | StartedDispose poolName ->
                dep.Logger.Information("Start disposing client factory: {0}", poolName)

            | ClientInitialized (poolName,number) ->
                consoleStatus.Value.Status <- $"Initializing client factory: {Console.okColor poolName}, initialized client: {Console.blueColor number}"
                consoleStatus.Value.Refresh()

            | ClientDisposed (poolName,number,error) ->
                consoleStatus.Value.Status <- $"Disposing client factory: {Console.okColor poolName}, disposed client: {Console.blueColor number}"
                consoleStatus.Value.Refresh()
                error |> Option.iter(fun ex -> dep.Logger.Error(ex, "Client exception occurred"))

            | InitFinished
            | InitFailed -> ()
        )
        |> ignore

let printContextInfo (dep: IGlobalDependency) =
    dep.Logger.Verbose("NBomberConfig: {NBomberConfig}", $"%A{dep.NBomberConfig}")

    if dep.WorkerPlugins.IsEmpty then
        dep.Logger.Information "Plugins: no plugins were loaded"
    else
        dep.WorkerPlugins
        |> List.iter(fun plugin -> dep.Logger.Information("Plugin loaded: {PluginName}", plugin.PluginName))

    if dep.ReportingSinks.IsEmpty then
        dep.Logger.Information "Reporting sinks: no reporting sinks were loaded"
    else
        dep.ReportingSinks
        |> List.iter(fun sink -> dep.Logger.Information("Reporting sink loaded: {SinkName}", sink.SinkName))

module LiveStatusTable =

    let private buildTable () =
        let table = Table()
        table.Border <- TableBorder.Square

        TableColumn("scenario") |> table.AddColumn |> ignore
        TableColumn("step") |> table.AddColumn |> ignore
        TableColumn("load simulation") |> table.AddColumn |> ignore
        TableColumn("latency stats (ms)") |> table.AddColumn |> ignore
        TableColumn("data transfer stats (KB)") |> table.AddColumn

    let private renderTable (table: Table) (scenariosStats: ScenarioStats list) =
        let mutable rowIndex = 0
        let updateOperation = table.Rows.Count > 0

        for scnStats in scenariosStats do
            for stepStats in scnStats.StepStats do
                let ok = stepStats.Ok
                let req = ok.Request
                let lt = ok.Latency
                let data = ok.DataTransfer

                if updateOperation then
                    table.UpdateCell(rowIndex, 2, $"{scnStats.LoadSimulationStats.SimulationName}: {Console.blueColor scnStats.LoadSimulationStats.Value}") |> ignore
                    table.UpdateCell(rowIndex, 3, $"ok: {Console.okColor req.Count}, fail: {Console.errorColor stepStats.Fail.Request.Count}, RPS: {Console.okColor req.RPS}, p50 = {Console.okColor lt.Percent50}, p99 = {Console.okColor lt.Percent99}") |> ignore
                    table.UpdateCell(rowIndex, 4, $"min: {data.MinBytes |> Converter.fromBytesToKb |> Console.blueColor}, max: {data.MaxBytes |> Converter.fromBytesToKb |> Console.blueColor}, all: {data.AllBytes |> Converter.fromBytesToMb |> Console.blueColor} MB") |> ignore
                    rowIndex <- rowIndex + 1
                else
                    table.AddRow(
                        scnStats.ScenarioName,
                        stepStats.StepName,
                        $"{scnStats.LoadSimulationStats.SimulationName}: {Console.blueColor scnStats.LoadSimulationStats.Value}",
                        $"ok: {Console.okColor req.Count}, fail: {Console.errorColor stepStats.Fail.Request.Count}, RPS: {Console.okColor req.RPS}, p50 = {Console.okColor lt.Percent50}, p99 = {Console.okColor lt.Percent99}",
                        $"min: {data.MinBytes |> Converter.fromBytesToKb |> Console.blueColor}, max: {data.MaxBytes |> Converter.fromBytesToKb |> Console.blueColor}, all: {data.AllBytes |> Converter.fromBytesToMb |> Console.blueColor} MB")
                    |> ignore

    let display (appType: ApplicationType)
                (cancelToken: CancellationToken)
                (isWarmUp: bool)
                (scnSchedulers: ScenarioScheduler list) =

        if appType = ApplicationType.Console then

            let stopWatch = Stopwatch()
            let mutable refreshTableCounter = 0

            let maxDuration =
                if isWarmUp then scnSchedulers |> List.map(fun x -> x.Scenario) |> Scenario.getMaxWarmUpDuration
                else scnSchedulers |> List.map(fun x -> x.Scenario) |> Scenario.getMaxDuration

            let table = buildTable ()

            let liveTable = AnsiConsole.Live(table)
            liveTable.AutoClear <- false
            liveTable.Overflow <- VerticalOverflow.Ellipsis
            liveTable.Cropping <- VerticalOverflowCropping.Bottom

            stopWatch.Start()

            liveTable.StartAsync(fun ctx -> backgroundTask {
                while not cancelToken.IsCancellationRequested do
                    try
                        let currentTime = stopWatch.Elapsed
                        if currentTime < maxDuration && refreshTableCounter = 0 then

                            let scenariosStats =
                                scnSchedulers
                                |> List.map(fun x -> x.MergedReportingStats)

                            renderTable table scenariosStats

                        table.Title <- TableTitle($"duration: ({currentTime:``hh\:mm\:ss``} - {maxDuration:``hh\:mm\:ss``})")
                        ctx.Refresh()
                        do! Task.Delay(1_000, cancelToken)

                        refreshTableCounter <- refreshTableCounter + 1
                        if refreshTableCounter = NBomber.Constants.ConsoleRefreshTableCounter then refreshTableCounter <- 0
                    with
                    | _ -> ()

                table.Title <- TableTitle($"duration: ({maxDuration:``hh\:mm\:ss``} - {maxDuration:``hh\:mm\:ss``})")
                ctx.Refresh()
            })
            |> ignore
