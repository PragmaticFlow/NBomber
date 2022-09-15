module internal NBomber.DomainServices.TestHost.TestHostConsole

open System
open System.Threading.Tasks

open FSharp.Control.Reactive
open FsToolkit.ErrorHandling
open NBomber.Extensions.Data
open Spectre.Console

open NBomber
open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler
open NBomber.Domain.ClientPool
open NBomber.Extensions.Internal
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.Infra.ProgressBar

let printTargetScenarios (dep: IGlobalDependency) (targetScns: Scenario list) =
    targetScns
    |> List.map(fun x -> x.ScenarioName)
    |> fun targets -> dep.Logger.Information("Target scenarios: {TargetScenarios}", String.concatWithComma targets)

let displayRealtimeStatusTable (scnSchedulers: ScenarioScheduler list) (isWarmUp: bool) =

    let duration = TimeSpan.Zero

    let stats =
        scnSchedulers
        |> List.map(fun x -> x.BuildRealtimeStats(duration, shouldAddToCache = false))
        |> Task.WhenAll
        |> fun t -> t.Result

    let table = Table()
    table.Border <- TableBorder.Square

    TableColumn("scenario") |> table.AddColumn |> ignore
    TableColumn("step") |> table.AddColumn |> ignore
    TableColumn("load simulation") |> table.AddColumn |> ignore
    TableColumn("latency stats (ms)") |> table.AddColumn |> ignore
    TableColumn("data transfer stats (bytes)") |> table.AddColumn |> ignore

    let liveTable = AnsiConsole.Live(table)
    liveTable.AutoClear <- false
    liveTable.Overflow <- VerticalOverflow.Ellipsis
    liveTable.Cropping <- VerticalOverflowCropping.Bottom

    liveTable.StartAsync(fun ctx ->  backgroundTask {
        while true do
            for scnStats in stats do
                for stepStats in scnStats.StepStats do
                    let ok = stepStats.Ok
                    let req = ok.Request
                    let lt = ok.Latency
                    let data = ok.DataTransfer

                    table.AddRow(
                        scnStats.ScenarioName,
                        stepStats.StepName,
                        "inject_per_sec, rate: 30",
                        $"ok: {req.Count}, fail: {stepStats.Fail.Request.Count}, RPS: {req.RPS}, min = {lt.MinMs}, max = {lt.MaxMs}, p50 = {lt.Percent50}, p75 = {lt.Percent75}, p99 = {lt.Percent99}",
                        $"min: {data.MinBytes}, max: {data.MaxBytes}, p99: {data.Percent99}, all: {Converter.fromBytesToMb data.AllBytes} MB")
                    |> ignore

                    //table.Title <- TableTitle("real time stats table")
                    table.Title <- TableTitle("duration: (00:01:00 - 00:05:00)")

            //table.UpdateCell(0, 0, "My Row2") |> ignore
            ctx.Refresh()
            do! Task.Delay 1_000
    })

let displayBombingProgress (applicationType: ApplicationType, scnSchedulers: ScenarioScheduler list, isWarmUp: bool) =

    let calcTickCount (scn: Scenario) =
        if isWarmUp then int(scn.WarmUpDuration.Value.TotalMilliseconds / Constants.SchedulerTickIntervalMs)
        else int(scn.PlanedDuration.TotalMilliseconds / Constants.SchedulerTickIntervalMs)

    let calcTotalTickCount (schedulers: ScenarioScheduler list) =
        schedulers |> Seq.map(fun scheduler -> scheduler.Scenario) |> Seq.map(calcTickCount) |> Seq.sum

    let getSimulationValue (progressInfo: ScenarioProgressInfo) =
        match progressInfo.CurrentSimulation with
        | RampConstant _
        | KeepConstant _ -> progressInfo.ConstantActorCount

        | RampPerSec _
        | InjectPerSec _
        | InjectPerSecRandom _ -> progressInfo.OneTimeActorCount

    let createSimulationDescription (simulation: LoadSimulation) (simulationValue: int) =
        let simulationName = LoadTimeLine.getSimulationName simulation

        match simulation with
        | RampConstant _
        | KeepConstant _        ->
            $"load: {Console.blueColor simulationName}, copies: {Console.blueColor simulationValue}"

        | RampPerSec _
        | InjectPerSec _
        | InjectPerSecRandom _  ->
            $"load: {Console.blueColor simulationName}, rate: {Console.blueColor simulationValue}"

    let createScenarioDescription (scenarioName: string) (simulation: LoadSimulation) (simulationValue: int) =
        let simulationDescription = createSimulationDescription simulation simulationValue
        $"{Console.okEscColor scenarioName}{MultilineColumn.NewLine}{simulationDescription}"

    let createProgressTaskConfig (scheduler: ScenarioScheduler) =
        let scenarioName = scheduler.Scenario.ScenarioName
        let simulation = scheduler.Scenario.LoadTimeLine.Head.LoadSimulation
        let description = createScenarioDescription scenarioName simulation 0
        let ticks = scheduler.Scenario |> calcTickCount |> float
        { Description = description; Ticks = ticks }

    let tickProgressTask (pbTask: ProgressTask) (scenarioName: string) (progressInfo: ScenarioProgressInfo) =

        let description =
            progressInfo
            |> getSimulationValue
            |> createScenarioDescription scenarioName progressInfo.CurrentSimulation

        pbTask
        |> ProgressBar.setDescription description
        |> ProgressBar.defaultTick

    let displayProgressForConcurrentScenarios (schedulers: ScenarioScheduler list) =
        schedulers
        |> List.map createProgressTaskConfig
        |> List.append [
            { Description = $"All Scenarios{MultilineColumn.NewLine}"; Ticks = schedulers |> calcTotalTickCount |> float }
        ]
        |> ProgressBar.create
            (fun progressTasks ->
                let pbTotalTask = progressTasks.Head

                progressTasks
                |> List.iteri(fun i pbTask ->
                    if i > 0 then
                        schedulers[i - 1].EventStream
                        |> Observable.choose(function ProgressUpdated info -> Some info | _ -> None)
                        |> Observable.subscribeWithCompletion

                            (fun progressInfo ->
                                let scenarioName = schedulers[i - 1].Scenario.ScenarioName
                                tickProgressTask pbTask scenarioName progressInfo
                                pbTotalTask |> ProgressBar.defaultTick)

                            (fun () -> ProgressBar.stop pbTask)

                        |> ignore
                )
            )

    let displayProgressForOneScenario (scheduler: ScenarioScheduler) =
        scheduler
        |> createProgressTaskConfig
        |> List.singleton
        |> ProgressBar.create
            (fun tasks ->
                scheduler.EventStream
                |> Observable.choose(function ProgressUpdated info -> Some info | _ -> None)
                |> Observable.subscribeWithCompletion
                    (fun progressInfo ->
                        let scenarioName = scheduler.Scenario.ScenarioName
                        tickProgressTask tasks.Head scenarioName progressInfo)

                    (fun () -> tasks |> List.iter ProgressBar.stop)

                |> ignore
            )

    match applicationType with
    | ApplicationType.Console ->
        if scnSchedulers.Length > 1 then
            scnSchedulers |> displayProgressForConcurrentScenarios |> ignore
        else
            scnSchedulers.Head |> displayProgressForOneScenario |> ignore
    | _ -> ()

let displayClientPoolProgress (dep: IGlobalDependency, pool: ClientPool) =

    let pbHandler (pbTasks: ProgressTask list) =
        pbTasks
        |> List.iteri(fun i pbTask ->
            pool.EventStream
            |> Observable.takeWhile(function
                | InitFinished -> false
                | InitFailed   -> false
                | _            -> true
            )
            |> Observable.subscribe(function
                | StartedInit poolName ->
                    dep.Logger.Information("Start init client factory: {ClientFactory}", poolName)

                | StartedDispose poolName ->
                    dep.Logger.Information("Start disposing client factory: {ClientFactory}", poolName)

                | ClientInitialized (poolName,number) ->
                    pbTask
                    |> ProgressBar.setDescription $"{Console.okColor poolName}{MultilineColumn.NewLine}initialized client: {Console.blueColor number}"
                    |> ProgressBar.defaultTick

                | ClientDisposed (poolName,number,error) ->
                    pbTask
                    |> ProgressBar.setDescription $"{Console.okColor poolName}{MultilineColumn.NewLine}disposed client: {Console.blueColor number}"
                    |> ProgressBar.defaultTick

                    error |> Option.iter(fun ex -> dep.Logger.Error(ex, "Client exception occurred"))

                | InitFinished
                | InitFailed -> ()
            )
            |> ignore
        )

    match dep.ApplicationType with
    | ApplicationType.Console ->
        let pbConfig = { Description = pool.PoolName; Ticks = pool.ClientCount |> float }
        ProgressBar.create pbHandler [pbConfig]

    | _ -> Task.FromResult()

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
