namespace NBomber.DomainServices.TestHost

open System
open System.Threading
open System.Threading.Tasks
open System.Diagnostics
open System.Runtime.InteropServices

open Serilog
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
open NBomber.Domain.Stats
open NBomber.Domain.Stats.ScenarioStatsActor
open NBomber.Domain.Concurrency.ScenarioActor
open NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices
open NBomber.DomainServices.TestHost.ReportingManager

//todo: add _globalCancelToken.Cancel() + timeout for init and clean + configurable operation timeout

type internal TestHost(dep: IGlobalDependency,
                       regScenarios: Scenario list,
                       getStepOrder: Scenario -> int[],
                       execSteps: StepDep -> RunningStep[] -> int[] -> Task<unit>) as this =

    let _log = dep.Logger.ForContext<TestHost>()
    let mutable _stopped = false
    let mutable _disposed = false
    let mutable _targetScenarios = List.empty<Scenario>
    let mutable _sessionArgs = SessionArgs.empty
    let mutable _currentOperation = OperationType.None
    let mutable _currentSchedulers = List.empty<ScenarioScheduler>
    let mutable _globalCancelToken = new CancellationTokenSource()
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
                                 (createStatsActor: ILogger -> Scenario -> TimeSpan -> ScenarioStatsActor)
                                 (getStepOrder: Scenario -> int[])
                                 (execSteps: StepDep -> RunningStep[] -> int[] -> Task<unit>) =

        let createScheduler (scn: Scenario) =
            let actorDep = {
                ScenarioDep = {
                    Logger = _log
                    Scenario = scn
                    CancellationTokenSource = new CancellationTokenSource()
                    ScenarioTimer = Stopwatch()
                    ScenarioOperation = operation
                    ScenarioStatsActor = createStatsActor _log scn (_sessionArgs.GetReportingInterval())
                    ExecStopCommand = execStopCommand
                }
                GetStepOrder = getStepOrder
                ExecSteps = execSteps
            }
            let count = getScenarioClusterCount scn.ScenarioName
            new ScenarioScheduler(actorDep, count)

        _currentSchedulers |> List.iter(fun x -> x.Stop())
        targetScenarios |> List.map createScheduler

    let stopSchedulers (schedulers: ScenarioScheduler list) =
        schedulers |> List.iter(fun x -> x.Stop())

    let initScenarios (sessionArgs: SessionArgs,
                       cancelToken: CancellationToken,
                       scenarios: Scenario list) = taskResult {

        let baseContext = NBomberContext.createBaseContext(sessionArgs.TestInfo, getCurrentNodeInfo(), cancelToken, _log)
        let defaultScnContext = Scenario.ScenarioContext.create baseContext

        let enabledScenarios = scenarios |> List.filter(fun x -> x.IsEnabled)
        let disabledScenarios = scenarios |> List.filter(fun x -> not x.IsEnabled)

        do! dep.ReportingSinks |> ReportingSinks.init dep baseContext
        do! dep.WorkerPlugins  |> WorkerPlugins.init dep baseContext

        let! initializedScenarios = TestHostScenario.initScenarios(dep, baseContext, defaultScnContext, sessionArgs, enabledScenarios)
        return initializedScenarios @ disabledScenarios
    }

    let startWarmUp (schedulers: ScenarioScheduler list) = backgroundTask {
        let isWarmUp = true
        _currentSchedulers <- schedulers

        TestHostConsole.displayBombingProgress(dep.ApplicationType, schedulers, isWarmUp)
        do! schedulers |> List.map(fun x -> x.Start()) |> Task.WhenAll
    }

    let startBombing (schedulers: ScenarioScheduler list)
                     (reportingManager: IReportingManager) = backgroundTask {

        let isWarmUp = false
        _currentSchedulers <- schedulers

        TestHostConsole.displayBombingProgress(dep.ApplicationType, schedulers, isWarmUp)
        dep.ReportingSinks |> ReportingSinks.start _log
        dep.WorkerPlugins  |> WorkerPlugins.start _log

        reportingManager.Start()

        // waiting on all scenarios to finish
        do! schedulers |> List.map(fun x -> x.Start()) |> Task.WhenAll

        // wait on final metrics and reporting tick
        do! Task.Delay Constants.ReportingTimerCompleteMs

        // waiting (in case of cluster) on all raw stats
        do! reportingManager.Stop()

        do! dep.ReportingSinks |> ReportingSinks.stop _log
        do! dep.WorkerPlugins  |> WorkerPlugins.stop _log
    }

    let cleanScenarios (sessionArgs: SessionArgs,
                        cancelToken: CancellationToken,
                        scenarios: Scenario list) =

        let baseContext = NBomberContext.createBaseContext(sessionArgs.TestInfo, getCurrentNodeInfo(), cancelToken, _log)
        let defaultScnContext = Scenario.ScenarioContext.create baseContext
        let enabledScenarios = scenarios |> List.filter(fun x -> x.IsEnabled)
        TestHostScenario.cleanScenarios dep baseContext defaultScnContext enabledScenarios

    member _.SessionArgs = _sessionArgs
    member _.CurrentOperation = _currentOperation
    member _.CurrentNodeInfo = getCurrentNodeInfo()
    member _.RegisteredScenarios = regScenarios
    member _.TargetScenarios = _targetScenarios

    member _.StartInit(sessionArgs: SessionArgs, targetScenarios: Scenario list) = backgroundTask {
        _stopped <- false
        _currentOperation <- OperationType.Init

        TestHostConsole.printContextInfo(dep)
        _log.Information "Starting init..."
        _globalCancelToken.Dispose()
        _globalCancelToken <- new CancellationTokenSource()

        match! initScenarios(sessionArgs, _globalCancelToken.Token, targetScenarios) with
        | Ok initializedScenarios ->
            _log.Information "Init finished"
            _targetScenarios <- initializedScenarios
            _sessionArgs <- sessionArgs
            _currentOperation <- OperationType.None
            return Ok _targetScenarios

        | Error appError ->
            _log.Error "Init failed"
            _currentOperation <- OperationType.Stop
            return AppError.createResult appError
    }

    member _.StartWarmUp(schedulers: ScenarioScheduler list) = backgroundTask {
        _stopped <- false
        _currentOperation <- OperationType.WarmUp

        _log.Information "Starting warm up..."

        do! startWarmUp schedulers
        stopSchedulers schedulers

        _currentOperation <- OperationType.None
    }

    member _.StartBombing(schedulers: ScenarioScheduler list, reportingManager: IReportingManager) = backgroundTask {
        _stopped <- false
        _currentOperation <- OperationType.Bombing

        _log.Information "Starting bombing..."
        do! startBombing schedulers reportingManager

        do! this.StopScenarios()
        _currentOperation <- OperationType.Complete
    }

    member _.StopScenarios([<Optional;DefaultParameterValue("":string)>]reason: string) = backgroundTask {
        if _currentOperation <> OperationType.Stop && not _stopped then
            _currentOperation <- OperationType.Stop

            if not(String.IsNullOrEmpty reason) then
                _log.Warning("Stopping test early: {StopReason}", reason)
            else
                _log.Information "Stopping scenarios..."

            stopSchedulers _currentSchedulers
            do! cleanScenarios(_sessionArgs, _globalCancelToken.Token, _targetScenarios)

            _stopped <- true
            _currentOperation <- OperationType.None
    }

    member _.CreateScenarioSchedulers(scenarios: Scenario list, operation: ScenarioOperation) =
        createScenarioSchedulers scenarios operation Scenario.defaultClusterCount ScenarioStatsActor.create getStepOrder execSteps

    member _.CreateScenarioSchedulers(scenarios: Scenario list,
                                      operation: ScenarioOperation,
                                      getScenarioClusterCount: ScenarioName -> int,
                                      createStatsActor: ILogger -> Scenario -> TimeSpan -> ScenarioStatsActor) =

        createScenarioSchedulers scenarios operation getScenarioClusterCount createStatsActor getStepOrder execSteps

    member _.GetRawStats(duration) =
        _currentSchedulers |> List.map(fun x -> x.GetRawStats duration) |> Option.sequence

    member _.DelRawStats(duration) =
        _currentSchedulers |> List.iter(fun x -> x.DelRawStats duration)

    member _.RunSession(sessionArgs: SessionArgs) = taskResult {
        let targetScenarios = regScenarios |> TestHostScenario.getTargetScenarios sessionArgs
        let! initializedScenarios = this.StartInit(sessionArgs, targetScenarios)

        // start warm up
        let warmupScenarios = Scenario.getScenariosForWarmUp initializedScenarios
        if warmupScenarios.Length > 0 then
            let warmUpSchedulers = this.CreateScenarioSchedulers(warmupScenarios, ScenarioOperation.WarmUp)
            do! this.StartWarmUp warmUpSchedulers

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
