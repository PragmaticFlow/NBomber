namespace NBomber.DomainServices.TestHost

open System
open System.Threading
open System.Threading.Tasks
open System.Diagnostics
open System.Runtime.InteropServices

open Serilog
open FSharp.Control.Tasks.NonAffine
open FsToolkit.ErrorHandling

open NBomber.Contracts.Stats
open NBomber.Errors
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Stats
open NBomber.Domain.Stats.ScenarioStatsActor
open NBomber.Domain.Concurrency.ScenarioActor
open NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices
open NBomber.DomainServices.NBomberContext
open NBomber.DomainServices.TestHost.TestHostReportingActor

type internal TestHost(dep: IGlobalDependency, registeredScenarios: Scenario list) as this =

    let _logger = dep.Logger.ForContext<TestHost>()
    let mutable _stopped = false
    let mutable _disposed = false
    let mutable _targetScenarios = List.empty<Scenario>
    let mutable _sessionArgs = SessionArgs.empty
    let mutable _currentOperation = OperationType.None
    let mutable _currentSchedulers = List.empty<ScenarioScheduler>
    let mutable _cancelToken = new CancellationTokenSource()
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
                _logger.Warning("Stopping scenario early: {ScenarioName}, reason: {StopReason}", sch.Scenario.ScenarioName, reason)
            )

        | StopTest reason -> this.StopScenarios(reason) |> ignore

    let createScenarioSchedulers (targetScenarios: Scenario list)
                                 (createStatsActor: ILogger -> Scenario -> TimeSpan -> IScenarioStatsActor) =

        let createScheduler (cancelToken: CancellationToken) (scn: Scenario) =
            let actorDep = {
                Logger = _logger
                CancellationToken = cancelToken
                ScenarioGlobalTimer = Stopwatch()
                Scenario = scn
                ScenarioStatsActor = createStatsActor _logger scn _sessionArgs.ReportingInterval
                ExecStopCommand = execStopCommand
            }
            new ScenarioScheduler(actorDep)

        _currentSchedulers |> List.iter(fun x -> x.Stop())
        _cancelToken.Dispose()
        _cancelToken <- new CancellationTokenSource()

        targetScenarios |> List.map(createScheduler _cancelToken.Token)

    let stopSchedulers (cancelToken: CancellationTokenSource, schedulers: ScenarioScheduler list) =
        if not cancelToken.IsCancellationRequested then
            cancelToken.Cancel()

        schedulers |> List.iter(fun x -> x.Stop())

    let initScenarios (sessionArgs: SessionArgs,
                       cancelToken: CancellationToken,
                       scenarios: Scenario list) = taskResult {

        let baseContext = NBomberContext.createBaseContext(sessionArgs.TestInfo, getCurrentNodeInfo(), cancelToken, _logger)
        let defaultScnContext = Scenario.ScenarioContext.create baseContext

        let enabledScenarios = scenarios |> List.filter(fun x -> x.IsEnabled)
        let disabledScenarios = scenarios |> List.filter(fun x -> not x.IsEnabled)

        do! dep.ReportingSinks |> TestHostReportingSinks.init dep baseContext
        do! dep.WorkerPlugins  |> TestHostPlugins.init dep baseContext

        let! initializedScenarios = TestHostScenario.initScenarios(dep, baseContext, defaultScnContext, sessionArgs, enabledScenarios)
        return initializedScenarios @ disabledScenarios
    }

    let startWarmUp (schedulers: ScenarioScheduler list) = task {
        let isWarmUp = true
        TestHostConsole.displayBombingProgress(dep.ApplicationType, schedulers, isWarmUp)
        do! schedulers |> List.map(fun x -> x.Start isWarmUp) |> Task.WhenAll
    }

    let startBombing (schedulers: ScenarioScheduler list,
                      flushStatsTimer: Timers.Timer option,
                      reportingTimer: Timers.Timer,
                      currentOperationTimer: Stopwatch) = task {

        let isWarmUp = false

        TestHostConsole.displayBombingProgress(dep.ApplicationType, schedulers, isWarmUp)
        do! dep.ReportingSinks |> TestHostReportingSinks.start _logger
        do! dep.WorkerPlugins  |> TestHostPlugins.start _logger

        flushStatsTimer |> Option.iter(fun x -> x.Start())
        reportingTimer.Start()
        currentOperationTimer.Start()

        // waiting on all scenarios to finish
        do! schedulers |> List.map(fun x -> x.Start isWarmUp) |> Task.WhenAll

        flushStatsTimer |> Option.iter(fun x -> x.Stop())
        reportingTimer.Stop()
        currentOperationTimer.Stop()

        do! dep.ReportingSinks |> TestHostReportingSinks.stop _logger
        do! dep.WorkerPlugins  |> TestHostPlugins.stop _logger
    }

    let cleanScenarios (sessionArgs: SessionArgs,
                        cancelToken: CancellationToken,
                        scenarios: Scenario list) =

        let baseContext = NBomberContext.createBaseContext(sessionArgs.TestInfo, getCurrentNodeInfo(), cancelToken, _logger)
        let defaultScnContext = Scenario.ScenarioContext.create baseContext
        let enabledScenarios = scenarios |> List.filter(fun x -> x.IsEnabled)
        TestHostScenario.cleanScenarios dep baseContext defaultScnContext enabledScenarios

    let getHints (finalStats: NodeStats) (targetScenarios: Scenario list) =
        if _sessionArgs.UseHintsAnalyzer then
            dep.WorkerPlugins
            |> TestHostPlugins.getHints
            |> List.append(HintsAnalyzer.analyzeNodeStats finalStats)
            |> List.toArray
        else
            Array.empty

    member _.SessionArgs = _sessionArgs
    member _.CurrentOperation = _currentOperation
    member _.CurrentNodeInfo = getCurrentNodeInfo()
    member _.RegisteredScenarios = registeredScenarios
    member _.TargetScenarios = _targetScenarios

    member _.StartInit(sessionArgs: SessionArgs, targetScenarios: Scenario list) = task {
        _stopped <- false
        _currentOperation <- OperationType.Init
        do! Task.Yield()

        TestHostConsole.printContextInfo(dep)
        _logger.Information "Starting init..."
        _cancelToken.Dispose()
        _cancelToken <- new CancellationTokenSource()

        match! initScenarios(sessionArgs, _cancelToken.Token, targetScenarios) with
        | Ok initializedScenarios ->
            _logger.Information "Init finished"
            _targetScenarios <- initializedScenarios
            _sessionArgs <- sessionArgs
            _currentOperation <- OperationType.None
            return Ok()

        | Error appError ->
            _logger.Error "Init failed"
            _currentOperation <- OperationType.Stop
            return AppError.createResult appError
    }

    member _.StartWarmUp() = task {
        _stopped <- false
        _currentOperation <- OperationType.WarmUp
        do! Task.Yield()

        _logger.Information "Starting warm up..."

        let schedulers = this.CreateScenarioSchedulers(ScenarioStatsActor.create)
        _currentSchedulers <- schedulers

        do! startWarmUp schedulers
        stopSchedulers(_cancelToken, schedulers)

        _currentOperation <- OperationType.None
    }

    member _.StartBombing(schedulers: ScenarioScheduler list,
                          flushStatsTimer: Timers.Timer option,
                          reportingTimer: Timers.Timer,
                          currentOperationTimer: Stopwatch) = task {
        _stopped <- false
        _currentOperation <- OperationType.Bombing
        _currentSchedulers <- schedulers
        do! Task.Yield()

        _logger.Information "Starting bombing..."
        do! startBombing(schedulers, flushStatsTimer, reportingTimer, currentOperationTimer)
        do! this.StopScenarios()

        _currentOperation <- OperationType.Complete
    }

    member _.StopScenarios([<Optional;DefaultParameterValue("":string)>]reason: string) = task {
        if _currentOperation <> OperationType.Stop && not _stopped then
            _currentOperation <- OperationType.Stop
            do! Task.Yield()

            if not(String.IsNullOrEmpty reason) then
                _logger.Warning("Stopping test early: {StopReason}", reason)
            else
                _logger.Information "Stopping scenarios..."

            stopSchedulers(_cancelToken, _currentSchedulers)
            do! cleanScenarios(_sessionArgs, _cancelToken.Token, _targetScenarios)

            _stopped <- true
            _currentOperation <- OperationType.None
    }

    member _.GetHints(finalStats) = _targetScenarios |> getHints finalStats

    member _.CreateScenarioSchedulers(createStatsActor: ILogger -> Scenario -> TimeSpan -> IScenarioStatsActor) =
        createScenarioSchedulers _targetScenarios createStatsActor

    member _.RunSession(sessionArgs: SessionArgs) = taskResult {
        let targetScenarios = registeredScenarios |> TestHostScenario.getTargetScenarios sessionArgs
        do! this.StartInit(sessionArgs, targetScenarios)
        do! this.StartWarmUp()

        let schedulers = this.CreateScenarioSchedulers(ScenarioStatsActor.create)

        // create timers
        let currentOperationTimer = Stopwatch()
        let reportingActor = TestHostReportingActor(dep, schedulers, sessionArgs.TestInfo)
        use reportingTimer = new Timers.Timer(sessionArgs.ReportingInterval.TotalMilliseconds)
        reportingTimer.Elapsed.Add(fun _ ->
            reportingActor.Publish(SaveRealtimeStats currentOperationTimer.Elapsed)
        )

        // start bombing
        do! this.StartBombing(schedulers, None, reportingTimer, currentOperationTimer)

        // gets final stats
        _logger.Information "Calculating final statistics..."
        let! finalStats = reportingActor.GetFinalStats(getCurrentNodeInfo())
        let! timeLineHistory = reportingActor.GetTimeLineHistory()
        let hints = _targetScenarios |> getHints finalStats

        return { FinalStats = finalStats; TimeLineHistory = Array.ofList timeLineHistory; Hints = hints }
    }

    interface IDisposable with
        member _.Dispose() =
            if not _disposed then
                _disposed <- true
                this.StopScenarios().Wait()

                for sink in dep.ReportingSinks do
                    use x = sink
                    ()

                for plugin in dep.WorkerPlugins do
                    use x = plugin
                    ()

                _logger.Verbose $"{nameof TestHost} disposed"
