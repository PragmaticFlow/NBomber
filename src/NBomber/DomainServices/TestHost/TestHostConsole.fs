module internal NBomber.DomainServices.TestHost.TestHostConsole

open System
open System.Diagnostics
open System.Threading
open System.Threading.Tasks
open FsToolkit.ErrorHandling
open Spectre.Console

open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Contracts.Internal
open NBomber.Extensions.Internal
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Scheduler.ScenarioScheduler
open NBomber.Infra
open NBomber.DomainServices.Reports
open NBomber.DomainServices.TestHost.ReportingManager

let printTargetScenarios (dep: IGlobalDependency) (targetScns: Scenario list) =
    targetScns
    |> Seq.map(fun x -> x.ScenarioName)
    |> fun targets -> dep.LogInfo("Target scenarios: {0}", String.concatWithComma targets)

let printWarmUpScenarios (dep: IGlobalDependency) (warmUpScns: Scenario list) =
    warmUpScns
    |> Seq.map(fun x -> x.ScenarioName)
    |> fun targets -> dep.LogInfo("Warm up for scenarios: {0}", String.concatWithComma targets)

let displayStatus (dep: IGlobalDependency) (msg: string) (runAction: StatusContext option -> Task<'T>) =
    if dep.ApplicationType = ApplicationType.Console then
        let status = AnsiConsole.Status()
        status.StartAsync(msg, fun ctx -> runAction (Some ctx))
    else
        dep.LogInfo msg
        runAction None

let printContextInfo (dep: IGlobalDependency) (sessionArgs: SessionArgs) =
    let reportsFolder = ReportHelper.getFullReportsFolderPath sessionArgs
    dep.LogInfo("Reports folder: {0}", reportsFolder)

    dep.Logger.Verbose("NBomberConfig: {0}", $"%A{dep.NBomberConfig}")

    if dep.WorkerPlugins.IsEmpty then
        dep.LogInfo "Plugins: no plugins were loaded"
    else
        dep.WorkerPlugins
        |> List.iter(fun plugin -> dep.LogInfo("Plugin loaded: {0}", plugin.PluginName) )

    if dep.ReportingSinks.IsEmpty then
        dep.LogInfo "Reporting sinks: no reporting sinks were loaded"
    else
        dep.ReportingSinks
        |> List.iter(fun sink -> dep.LogInfo("Reporting sink loaded: {0}", sink.SinkName))

module LiveStatusTable =

    let private buildScenariosTable () =
        let table = Table()
        table.Border <- TableBorder.Square
        table.Title <- TableTitle "scenarios stats"

        table.AddColumn("scenario")
             .AddColumn("step")
             .AddColumn("simulation")
             .AddColumn("statistics (ms)")

    let private buildMetricsTable () =
        let table = Table()
        table.Border <- TableBorder.Square
        table.Title <- TableTitle "metrics stats"

        table.AddColumn("metric name")
             .AddColumn("stats")

    let private renderScenarioStats (table: Table) (scenariosStats: ScenarioStats list) =
        table.Rows.Clear()

        for scnStats in scenariosStats do

            let simulation =
                if scnStats.LoadSimulationStats.SimulationName = "pause" then "pause"
                else $"{scnStats.LoadSimulationStats.SimulationName}: {Console.blueColor scnStats.LoadSimulationStats.Value}"

            for stepStats in scnStats.StepStats do
                let ok = stepStats.Ok
                let okR = ok.Request
                let okL = ok.Latency

                table.AddRow(
                    scnStats.ScenarioName,
                    stepStats.StepName,
                    simulation,
                    $"ok: {Console.okColor okR.Count}, fail: {Console.errorColor stepStats.Fail.Request.Count}, RPS: {Console.okColor okR.RPS},\nmin = {Console.okColor okL.MinMs}, mean = {Console.okColor okL.MeanMs}, max = {Console.okColor okL.MaxMs},\np50 = {Console.okColor okL.Percent50}, p75 = {Console.okColor okL.Percent75}, p99 = {Console.okColor okL.Percent99}")
                |> ignore

    let private renderMetricsStats (table: Table) (stats: MetricStats list) =
        table.Rows.Clear()

        for s in stats do
            match s.MetricType with
            | MetricType.Histogram ->
                table.AddRow($"{s.Name}", $"current: {Console.okColor s.Current} {s.MeasureUnit}, max: {Console.okColor s.Max} {s.MeasureUnit}") |> ignore

            | MetricType.Gauge ->
                table.AddRow($"{s.Name}", $"current: {Console.okColor s.Current} {s.MeasureUnit}") |> ignore

            | _ ->
                table.AddRow($"{s.Name}", $"current: {Console.okColor s.Current} {s.MeasureUnit}") |> ignore

    let private getMaxScnDuration (isWarmUp: bool) (scnSchedulers: ScenarioScheduler list) =
        if isWarmUp then
            scnSchedulers |> Seq.map(fun x -> x.Scenario) |> Scenario.getMaxWarmUpDuration
        else
            scnSchedulers |> Seq.map(fun x -> x.Scenario) |> Scenario.getMaxDuration

    let display (dep: IGlobalDependency)
                (cancelToken: CancellationToken)
                (isWarmUp: bool)
                (scnSchedulers: ScenarioScheduler list)
                (reportingManager: IReportingManager) =

        if dep.ApplicationType = ApplicationType.Console then

            let stopWatch = Stopwatch()
            let mutable refreshTableCounter = 0

            let maxDuration = getMaxScnDuration isWarmUp scnSchedulers

            let scnTable = buildScenariosTable()
            let metricsTable = buildMetricsTable()

            let mainTable = Table()
            mainTable.Border <- TableBorder.Double

            mainTable
                .AddColumn("real-time stats table")
                .AddRow(scnTable)
                .AddEmptyRow()
                .AddRow(metricsTable) |> ignore

            let liveTable = AnsiConsole.Live(mainTable)
            liveTable.AutoClear <- false
            liveTable.Overflow <- VerticalOverflow.Ellipsis
            liveTable.Cropping <- VerticalOverflowCropping.Bottom

            stopWatch.Start()

            liveTable.StartAsync(fun ctx -> backgroundTask {
                while not cancelToken.IsCancellationRequested do
                    try
                        let currentTime = stopWatch.Elapsed
                        if currentTime < maxDuration && refreshTableCounter = 0 then

                            let scenariosStats = scnSchedulers |> List.map(fun x -> x.ConsoleScenarioStats)
                            renderScenarioStats scnTable scenariosStats
                            renderMetricsStats metricsTable (reportingManager.GetCurrentMetrics())

                        if currentTime <= maxDuration then
                            mainTable.Title <- TableTitle($"duration: ({currentTime:``hh\:mm\:ss``} - {maxDuration:``hh\:mm\:ss``})")
                            ctx.Refresh()

                        do! Task.Delay(1_000, cancelToken)

                        refreshTableCounter <- refreshTableCounter + 1

                        if refreshTableCounter >= NBomber.Constants.ConsoleRefreshTableCounter then
                            refreshTableCounter <- 0
                    with
                    | :? OperationCanceledException as ex -> ()
                    | ex ->
                        refreshTableCounter <- 1
                        dep.Logger.Fatal(ex.ToString())

                mainTable.Title <- TableTitle($"duration: ({maxDuration:``hh\:mm\:ss``} - {maxDuration:``hh\:mm\:ss``})")
                ctx.Refresh()
            })
            |> ignore
