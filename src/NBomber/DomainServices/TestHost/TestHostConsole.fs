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
open NBomber.Domain.ClientPool
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
            $"load: {simulationName |> Console.highlightSecondary}, copies: {simulationValue |> Console.highlightSecondary}"

        | RampPerSec _
        | InjectPerSec _
        | InjectPerSecRandom _  ->
            $"load: {simulationName |> Console.highlightSecondary}, rate: {simulationValue |> Console.highlightSecondary}"

    let createScenarioDescription (scenarioName: string) (simulation: LoadSimulation) (simulationValue: int) =
        let simulationDescription = createSimulationDescription simulation simulationValue
        $"{scenarioName |> Console.highlightPrimary}{MultilineColumn.NewLine}{simulationDescription}"

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

    let createDescriptionForStoppedTask (scenarioName) =
        $"{scenarioName |> Console.highlightDanger}"

    let displayProgressForConcurrentScenarios (schedulers: ScenarioScheduler list) =
        schedulers
        |> List.map createProgressTaskConfig
        |> List.append [
            { Description = $"All Scenarios{MultilineColumn.NewLine}"; Ticks = schedulers |> calcTotalTickCount |> float }
        ]
        |> ProgressBar.create
           (fun tasks ->
                let pbTotalTask = tasks.Head

                tasks
                |> List.iteri(fun i pbTask ->
                    if i > 0 then
                        schedulers.[i - 1].EventStream
                        |> Observable.choose(function ProgressUpdated info -> Some info | _ -> None)
                        |> Observable.subscribeWithCompletion
                            (fun progressInfo ->
                                let scenarioName = schedulers.[i - 1].Scenario.ScenarioName
                                tickProgressTask pbTask scenarioName progressInfo
                                pbTotalTask |> ProgressBar.defaultTick
                            )
                            (fun () ->
                                let remainCount = ProgressBar.getRemainTicks(pbTask)

                                if remainCount > 0.0 then
                                    let desc =
                                        schedulers.[i - 1].Scenario.ScenarioName
                                        |> createDescriptionForStoppedTask

                                    pbTask
                                    |> ProgressBar.setDescription desc
                                    |> ProgressBar.stop

                                    pbTotalTask |> ProgressBar.tick remainCount
                            )
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
                        tickProgressTask tasks.Head scenarioName progressInfo
                    )
                    (fun () ->
                        let pbTask = tasks.Head
                        let remainTicks = ProgressBar.getRemainTicks(pbTask)

                        if remainTicks > 0.0 then
                            let desc =
                                scheduler.Scenario.ScenarioName
                                |> createDescriptionForStoppedTask

                            pbTask
                            |> ProgressBar.setDescription desc
                            |> ProgressBar.stop
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
                    dep.Logger.Information($"Start init client pool: '{poolName}'.")

                | StartedDispose poolName ->
                    dep.Logger.Information($"Start disposing client pool: '{poolName}'.")

                | ClientInitialized (poolName,number) ->
                    pbTask
                    |> ProgressBar.setDescription $"{poolName |> Console.highlightPrimary}{MultilineColumn.NewLine}initialized client: {number |> Console.highlightSecondary}"
                    |> ProgressBar.defaultTick

                | ClientDisposed (poolName,number,error) ->
                    pbTask
                    |> ProgressBar.setDescription $"{poolName |> Console.highlightPrimary}{MultilineColumn.NewLine}disposed client: {number |> Console.highlightSecondary}"
                    |> ProgressBar.defaultTick

                    error |> Option.iter(fun ex -> dep.Logger.Error(ex, "Client exception occurred."))

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
