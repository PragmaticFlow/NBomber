namespace NBomber.DomainServices.TestHost

open System
open System.Threading
open System.Threading.Tasks
open System.Diagnostics
open System.Runtime.InteropServices
open Microsoft.FSharp.Collections

open Serilog
open Spectre.Console
open FsToolkit.ErrorHandling

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Contracts.Internal
open NBomber.Extensions.Internal
open NBomber.Errors
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Step
open NBomber.Domain.ScenarioContext
open NBomber.Domain.Stats
open NBomber.Domain.Stats.ScenarioStatsActor
open NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices
open NBomber.DomainServices.TestHost.ReportingManager

//todo: add _globalCancelToken.Cancel() + timeout for init and clean + configurable operation timeout

type internal TestHost(dep: IGlobalDependency, regScenarios: Scenario list) as this =

    let _log = dep.Logger.ForContext<TestHost>()
    let mutable _currentSchedulers = List.empty<ScenarioScheduler>

    let mutable _stopped = false
    let mutable _disposed = false
    let mutable _targetScenarios = List.empty<Scenario>
    let mutable _sessionArgs = SessionArgs.empty
    let mutable _currentOperation = OperationType.None
    let _defaultNodeInfo = NodeInfo.init None

    let getCurrentNodeInfo () =
        { _defaultNodeInfo with NodeType = dep.NodeType; CurrentOperation = _currentOperation }

    let execStopCommand (command: StopCommand) =
        match command with
        | StopScenario (scenarioName, reason) ->
            _currentSchedulers
            |> List.tryFind(fun sch -> sch.Scenario.ScenarioName = scenarioName)
            |> Option.iter(fun sch ->
                sch.Stop()
                _log.Warning("Stopping scenario early: {ScenarioName}, reason: {StopReason}", sch.Scenario.ScenarioName, reason)
            )

        | StopTest reason -> this.StopScenarios(reason) |> ignore

    let createScenarioSchedulers (targetScenarios: Scenario list)
                                 (operation: ScenarioOperation)
                                 (getScenarioClusterCount: ScenarioName -> int)
                                 (createStatsActor: ILogger -> Scenario -> TimeSpan -> ScenarioStatsActor) =

        let createScheduler (scn: Scenario) =
            let scnDep = {
                Logger = _log
                Scenario = scn
                ScenarioCancellationToken = new CancellationTokenSource()
                ScenarioTimer = Stopwatch()
                ScenarioOperation = operation
                ScenarioStatsActor = createStatsActor _log scn (_sessionArgs.GetReportingInterval())
                ExecStopCommand = execStopCommand
                MaxFailCount = _sessionArgs.GetMaxFailCount()
            }
            let count = getScenarioClusterCount scn.ScenarioName
            new ScenarioScheduler(scnDep, count)

        _currentSchedulers |> List.iter(fun x -> x.Stop())
        targetScenarios |> List.map createScheduler

    let stopSchedulers (schedulers: ScenarioScheduler list) =
        schedulers |> List.iter(fun x -> x.Stop())

    let startScenarios (schedulers: ScenarioScheduler list) (reportingManager: IReportingManager option) = backgroundTask {

        let isWarmUp = reportingManager.IsNone

        if not isWarmUp then
            dep.WorkerPlugins |> WorkerPlugins.start _log
            dep.ReportingSinks |> ReportingSinks.start _log

        use cancelToken = new CancellationTokenSource()
        TestHostConsole.LiveStatusTable.display dep cancelToken.Token isWarmUp schedulers

        if reportingManager.IsSome then
            reportingManager.Value.Start()

        // waiting on all scenarios to finish
        let schedulersArray = schedulers |> List.toArray
        use schedulerTimer = new System.Timers.Timer(Constants.SchedulerTickIntervalMs)
        schedulerTimer.Elapsed.Add(fun _ ->
            schedulersArray |> Array.Parallel.iter(fun x -> x.ExecScheduler())
        )

        schedulerTimer.Start()
        do! schedulers |> List.map(fun x -> x.Start()) |> Task.WhenAll
        cancelToken.Cancel()
        schedulerTimer.Stop()

        if not isWarmUp then
            // wait on final metrics and reporting tick
            do! Task.Delay Constants.ReportingTimerCompleteMs

            // waiting (in case of cluster) on all raw stats
            do! reportingManager.Value.Stop()

            do! dep.WorkerPlugins |> WorkerPlugins.stop _log
            do! dep.ReportingSinks |> ReportingSinks.stop _log
    }

    let startInit (consoleStatus: StatusContext option) (cancelToken: CancellationToken) (sessionArgs: SessionArgs) = taskResult {

        let baseContext = NBomberContext.createBaseContext sessionArgs.TestInfo getCurrentNodeInfo cancelToken _log

        do! dep.WorkerPlugins |> WorkerPlugins.init dep baseContext
        do! dep.ReportingSinks |> ReportingSinks.init dep baseContext

        return! TestHostScenario.initScenarios dep consoleStatus baseContext sessionArgs regScenarios
    }

    let startClean (sessionArgs: SessionArgs)
                   (consoleStatus: StatusContext option)
                   (cancelToken: CancellationToken)
                   (scenarios: Scenario list) =

        let baseContext = NBomberContext.createBaseContext sessionArgs.TestInfo getCurrentNodeInfo cancelToken _log
        let enabledScenarios = scenarios |> List.filter(fun x -> x.IsEnabled)
        TestHostScenario.cleanScenarios dep consoleStatus baseContext enabledScenarios

    member _.SessionArgs = _sessionArgs
    member _.CurrentOperation = _currentOperation
    member _.CurrentNodeInfo = getCurrentNodeInfo()
    member _.RegisteredScenarios = regScenarios
    member _.TargetScenarios = _targetScenarios
    member _.CurrentSchedulers = _currentSchedulers

    member _.StartInit(sessionArgs: SessionArgs) =
        _stopped <- false
        _currentOperation <- OperationType.Init

        TestHostConsole.printContextInfo dep
        _log.Information "Starting init..."

        TestHostConsole.displayStatus dep "Initializing scenarios..." (fun consoleStatus -> backgroundTask {
            use cancelToken = new CancellationTokenSource()
            match! startInit consoleStatus cancelToken.Token sessionArgs with
            | Ok initializedScenarios ->
                _log.Information "Init finished"
                cancelToken.Cancel()
                _targetScenarios <- initializedScenarios
                _sessionArgs <- sessionArgs
                _currentOperation <- OperationType.None
                return Ok _targetScenarios

            | Error appError ->
                _log.Error "Init failed"
                cancelToken.Cancel()
                _currentOperation <- OperationType.Stop
                return AppError.createResult appError
        })

    member _.StartWarmUp(scenarios: Scenario list, ?getScenarioClusterCount: ScenarioName -> int) = backgroundTask {
        _stopped <- false
        _currentOperation <- OperationType.WarmUp

        let warmupScenarios = Scenario.getScenariosForWarmUp scenarios
        if warmupScenarios.Length > 0 then

            _log.Information "Starting warm up..."

            let warmUpSchedulers = this.CreateScenarioSchedulers(
                warmupScenarios, ScenarioOperation.WarmUp, ?getScenarioClusterCount = getScenarioClusterCount
            )

            let names = warmupScenarios |> Seq.map(fun x -> x.ScenarioName) |> String.concatWithComma
            _log.Verbose $"Warm up for scenarios: [{names}]"

            _currentSchedulers <- warmUpSchedulers

            do! startScenarios warmUpSchedulers None
            stopSchedulers warmUpSchedulers

        _currentOperation <- OperationType.None
    }

    member _.StartBombing(schedulers: ScenarioScheduler list, reportingManager: IReportingManager) = backgroundTask {
        _stopped <- false
        _currentOperation <- OperationType.Bombing
        _currentSchedulers <- schedulers

        _log.Information "Starting bombing..."
        do! startScenarios schedulers (Some reportingManager)

        do! this.StopScenarios()
        _currentOperation <- OperationType.Complete
    }

    member _.StopScenarios([<Optional;DefaultParameterValue("":string)>]reason: string) =
        if _currentOperation <> OperationType.Stop && not _stopped then
            _currentOperation <- OperationType.Stop

            if not(String.IsNullOrEmpty reason) then
                _log.Warning("Stopping test early: {StopReason}", reason)
            else
                _log.Information "Stopping scenarios..."

            TestHostConsole.displayStatus dep "Cleaning scenarios..." (fun consoleStatus -> backgroundTask {
                use cancelToken = new CancellationTokenSource()
                stopSchedulers _currentSchedulers

                do! startClean _sessionArgs consoleStatus cancelToken.Token _targetScenarios

                _stopped <- true
                _currentOperation <- OperationType.None
            })
        else
            Task.FromResult()

    member _.CreateScenarioSchedulers(scenarios: Scenario list,
                                      operation: ScenarioOperation,
                                      ?getScenarioClusterCount: ScenarioName -> int,
                                      ?createStatsActor: ILogger -> Scenario -> TimeSpan -> ScenarioStatsActor) =

        let getScenarioClusterCount =
            getScenarioClusterCount
            |> Option.defaultValue Scenario.defaultClusterCount

        let createStatsActor =
            createStatsActor
            |> Option.defaultValue ScenarioStatsActor.createDefault

        createScenarioSchedulers scenarios operation getScenarioClusterCount createStatsActor

    member _.GetRealtimeStats(duration) =
        _currentSchedulers |> List.map(fun x -> x.AllRealtimeStats.TryFind duration) |> Option.sequence

    member _.RunSession(sessionArgs: SessionArgs) = taskResult {
        let! initializedScenarios = this.StartInit sessionArgs

        // start warm up
        do! this.StartWarmUp initializedScenarios

        // start bombing
        let bombingSchedulers = this.CreateScenarioSchedulers(initializedScenarios, ScenarioOperation.Bombing)
        use reportingManager = ReportingManager.create dep bombingSchedulers sessionArgs
        do! this.StartBombing(bombingSchedulers, reportingManager)

        // gets final stats
        _log.Information "Calculating final statistics..."
        return! reportingManager.GetSessionResult(getCurrentNodeInfo())
    }

    member _.Dispose() =
        if not _disposed then
            _disposed <- true
            this.StopScenarios().Wait()

            for sink in dep.ReportingSinks do
                use _ = sink
                ()

            for plugin in dep.WorkerPlugins do
                use _ = plugin
                ()

            _log.Verbose $"{nameof TestHost} disposed"

    interface IDisposable with
        member _.Dispose() = this.Dispose()
