module internal NBomber.DomainServices.TestHost.TestHostConsole

open System.Threading.Tasks

open FSharp.Control.Reactive
open FsToolkit.ErrorHandling
open Spectre.Console

open NBomber
open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler
open NBomber.Domain.ConnectionPool
open NBomber.Extensions.InternalExtensions
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.Infra.ProgressBar

let printTargetScenarios (dep: IGlobalDependency) (targetScns: Scenario list) =
    targetScns
    |> List.map(fun x -> x.ScenarioName)
    |> fun targets -> dep.Logger.Information("Target scenarios: {0}.", String.concatWithCommaAndQuotes targets)

let displayBombingProgress (dep: IGlobalDependency, scnSchedulers: ScenarioScheduler list, isWarmUp: bool) =

    let calcTickCount (scn: Scenario) =
        if isWarmUp then int(scn.WarmUpDuration.TotalMilliseconds / Constants.SchedulerNotificationTickInterval.TotalMilliseconds)
        else int(scn.PlanedDuration.TotalMilliseconds / Constants.SchedulerNotificationTickInterval.TotalMilliseconds)

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
        let simulationName = LoadTimeLine.getSimulationName(simulation)

        match simulation with
        | RampConstant _
        | KeepConstant _        ->
            $"{simulationName}, copies: {simulationValue |> Console.highlightSecondary}"

        | RampPerSec _
        | InjectPerSec _
        | InjectPerSecRandom _  ->
            $"{simulationName}, rate: {simulationValue |> Console.highlightSecondary}"

    let createScenarioDescription (scenarioName: string) (simulation: LoadSimulation) (simulationValue: int) =
        let simulationDescription = createSimulationDescription simulation simulationValue
        $"{scenarioName |> Console.highlightPrimary}{MultilineColumn.NewLine}{simulationDescription}"

    let createProgressTaskConfig (scheduler: ScenarioScheduler) =
        let scenarioName = scheduler.Scenario.ScenarioName
        let simulation = scheduler.Scenario.LoadTimeLine.Head.LoadSimulation
        let description = createScenarioDescription scenarioName simulation 0
        let ticks = scheduler.Scenario |> calcTickCount |> float
        { Description = description; Ticks = ticks }

    let tickProgressTask (task: ProgressTask) (scenarioName: string) (progressInfo: ScenarioProgressInfo) =
        progressInfo
        |> getSimulationValue
        |> createScenarioDescription scenarioName progressInfo.CurrentSimulation
        |> ProgressBar.setDescription task
        |> ignore

        Constants.SchedulerNotificationTickInterval.TotalMilliseconds |> ProgressBar.tick task |> ignore

    let displayProgressForConcurrentScenarios (schedulers: ScenarioScheduler list) =
        schedulers
        |> List.map createProgressTaskConfig
        |> List.append [
            { Description = $"All Scenarios{MultilineColumn.NewLine}"; Ticks = schedulers |> calcTotalTickCount |> float }
        ]
        |> ProgressBar.create ProgressBar.defaultColumns
           (fun tasks ->
                let totalTask = tasks.Head

                tasks
                |> List.iteri(fun i task ->
                    if i > 0 then
                        schedulers.[i - 1].EventStream
                        |> Observable.choose(function ProgressUpdated info -> Some info | _ -> None)
                        |> Observable.subscribe(fun progressInfo ->
                            let scenarioName = schedulers.[i - 1].Scenario.ScenarioName
                            tickProgressTask task scenarioName progressInfo
                            Constants.SchedulerNotificationTickInterval.TotalMilliseconds |> ProgressBar.tick totalTask |> ignore
                        )
                        |> ignore
                )
           )

    let displayProgressForOneScenario (scheduler: ScenarioScheduler) =
        scheduler
        |> createProgressTaskConfig
        |> List.singleton
        |> ProgressBar.create ProgressBar.defaultColumns
           (fun tasks ->
                scheduler.EventStream
                |> Observable.choose(function ProgressUpdated info -> Some info | _ -> None)
                |> Observable.subscribe(fun progressInfo ->
                    let scenarioName = scheduler.Scenario.ScenarioName
                    tickProgressTask tasks.Head scenarioName progressInfo
                )
                |> ignore
           )

    match dep.ApplicationType with
    | ApplicationType.Console ->
        if scnSchedulers.Length > 1 then
            displayProgressForConcurrentScenarios(scnSchedulers) |> ignore
        else
            displayProgressForOneScenario(scnSchedulers.Head) |> ignore
    | _ -> ()

let displayConnectionPoolsProgress (dep: IGlobalDependency, pools: ConnectionPool list) =
    match dep.ApplicationType with
    | ApplicationType.Console ->
        pools
        |> List.map(fun pool -> { Description = pool.PoolName; Ticks = pool.ConnectionCount |> float })
        |> ProgressBar.create ProgressBar.defaultColumns
           (fun tasks ->
                tasks
                |> List.iteri(fun i task ->
                    pools.[i].EventStream
                    |> Observable.subscribe(fun event ->
                        let setPbDescription = ProgressBar.setDescription task >> ignore

                        match event with
                        | StartedInit poolName ->
                            setPbDescription $"{poolName |> Console.highlightPrimary}{MultilineColumn.NewLine}opening connection"

                        | StartedStop poolName ->
                            setPbDescription $"{poolName |> Console.highlightPrimary}{MultilineColumn.NewLine}closing connection"

                        | ConnectionOpened (poolName, number) ->
                            setPbDescription $"{poolName |> Console.highlightPrimary}{MultilineColumn.NewLine}opened connection: {number |> Console.highlightSecondary}"
                            Constants.SchedulerNotificationTickInterval.TotalMilliseconds |> ProgressBar.tick task |> ignore

                        | ConnectionClosed error ->
                            Constants.SchedulerNotificationTickInterval.TotalMilliseconds |> ProgressBar.tick task |> ignore
                            error |> Option.map(fun ex -> dep.Logger.Error(ex, "Close connection exception occurred.")) |> ignore

                        | InitFinished
                        | InitFailed -> ()
                    )
                    |> ignore
                )
           )

    | _ -> Task.FromResult()

let printContextInfo (dep: IGlobalDependency) =
    dep.Logger.Verbose("NBomberConfig: {NBomberConfig}", sprintf "%A" dep.NBomberConfig)

    if dep.WorkerPlugins.IsEmpty then
        dep.Logger.Information("Plugins: no plugins were loaded.")
    else
        dep.WorkerPlugins
        |> List.iter(fun plugin -> dep.Logger.Information("Plugins: '{PluginName}' loaded.", plugin.PluginName))

    if dep.ReportingSinks.IsEmpty then
        dep.Logger.Information("Reporting sinks: no reporting sinks were loaded.")
    else
        dep.ReportingSinks
        |> List.iter(fun sink -> dep.Logger.Information("Reporting sinks: '{SinkName}' loaded.", sink.SinkName))
