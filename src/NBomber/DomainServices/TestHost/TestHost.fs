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
    let mutable _sessionArgs = SessionArgs.Empty
    let mutable _currentOperation = OperationType.None
    let mutable _currentSchedulers = List.empty<ScenarioScheduler>
    let mutable _cancelToken = new CancellationTokenSource()
    let _defaultNodeInfo = NodeInfo.init()

    let getCurrentNodeInfo () =
        { _defaultNodeInfo with NodeType = dep.NodeType; CurrentOperation = _currentOperation }

    let execStopCommand (command: StopCommand) =
        match command with
        | StopScenario (scenarioName, reason) ->
            _currentSchedulers
            |> List.tryFind(fun sch -> sch.Scenario.ScenarioName = scenarioName)
            |> Option.iter(fun sch ->
                sch.Stop()
                _logger.Warning $"Stopping scenario early: '{sch.Scenario.ScenarioName}', reason: '{reason}'"
            )

        | StopTest reason -> this.StopScenarios(reason) |> ignore

    let createScenarioSchedulers (targetScenarios: Scenario list)
                                 (createStatsActor: ILogger -> Scenario -> TimeSpan -> MailboxProcessor<_>) =

        let createScheduler (cancelToken: CancellationToken) (scn: Scenario) =
            let actorDep = {
                Logger = _logger
                CancellationToken = cancelToken
                ScenarioGlobalTimer = Stopwatch()
                Scenario = scn
                ScenarioStatsActor = createStatsActor _logger scn _sessionArgs.SendStatsInterval
                ExecStopCommand = execStopCommand
            }
            new ScenarioScheduler(actorDep)

        _currentSchedulers |> List.iter(fun x -> x.Stop())
        _cancelToken.Dispose()
        _cancelToken <- new CancellationTokenSource()

        targetScenarios |> List.map(createScheduler _cancelToken.Token)

    let stopSchedulers (schedulers: ScenarioScheduler list) =
        if not _cancelToken.IsCancellationRequested then
            _cancelToken.Cancel()

        schedulers |> List.iter(fun x -> x.Stop())

    let initScenarios (sessionArgs: SessionArgs) = taskResult {
        _sessionArgs <- sessionArgs

        let baseContext = NBomberContext.createBaseContext(_sessionArgs.TestInfo, getCurrentNodeInfo(), _cancelToken.Token, _logger)
        let defaultScnContext = Scenario.ScenarioContext.create baseContext
        let targetScenarios = registeredScenarios |> TestHostScenario.getTargetScenarios _sessionArgs

        do! TestHostReportingSinks.init dep baseContext
        do! TestHostPlugins.init dep baseContext
        let! initializedScenarios = TestHostScenario.initScenarios(dep, baseContext, defaultScnContext, _sessionArgs, targetScenarios)

        _targetScenarios <- initializedScenarios
    }

    let startWarmUp (schedulers: ScenarioScheduler list) = task {
        let isWarmUp = true
        _currentSchedulers <- schedulers
        TestHostConsole.displayBombingProgress(dep, schedulers, isWarmUp)
        do! schedulers |> List.map(fun x -> x.Start(isWarmUp)) |> Task.WhenAll
    }

    let startBombing (schedulers: ScenarioScheduler list,
                      reportingTimer: Timers.Timer,
                      currentOperationTimer: Stopwatch) = task {

        let isWarmUp = false
        _currentSchedulers <- schedulers

        TestHostConsole.displayBombingProgress(dep, schedulers, isWarmUp)
        do! TestHostReportingSinks.start _logger dep.ReportingSinks
        do! TestHostPlugins.start _logger dep.WorkerPlugins

        reportingTimer.Start()
        currentOperationTimer.Start()

        do! schedulers |> List.map(fun x -> x.Start isWarmUp) |> Task.WhenAll

        reportingTimer.Stop()
        currentOperationTimer.Stop()

        do! TestHostReportingSinks.stop _logger dep.ReportingSinks
        do! TestHostPlugins.stop _logger dep.WorkerPlugins
    }

    let cleanScenarios () =
        let baseContext = NBomberContext.createBaseContext(_sessionArgs.TestInfo, getCurrentNodeInfo(), _cancelToken.Token, _logger)
        let defaultScnContext = Scenario.ScenarioContext.create(baseContext)
        TestHostScenario.cleanScenarios dep baseContext defaultScnContext _targetScenarios

    let getHints (finalStats: NodeStats) =
        if _sessionArgs.UseHintsAnalyzer then
            dep.WorkerPlugins
            |> TestHostPlugins.getHints
            |> List.append(HintsAnalyzer.analyzeNodeStats finalStats)
            |> List.append(HintsAnalyzer.analyzeScenarios _targetScenarios)
            |> List.toArray
        else
            Array.empty

    member _.SessionArgs = _sessionArgs
    member _.CurrentOperation = _currentOperation
    member _.CurrentNodeInfo = getCurrentNodeInfo()
    member _.RegisteredScenarios = registeredScenarios
    member _.TargetScenarios = _targetScenarios

    member _.StartInit(sessionArgs: SessionArgs) = task {
        _stopped <- false
        _currentOperation <- OperationType.Init
        do! Task.Yield()

        TestHostConsole.printContextInfo(dep)

        _logger.Information "Starting init..."
        match! initScenarios(sessionArgs) with
        | Ok _ ->
            _logger.Information "Init finished."
            _currentOperation <- OperationType.None
            return Ok()

        | Error appError ->
            _logger.Error "Init failed."
            _currentOperation <- OperationType.Stop
            return AppError.createResult(appError)
    }

    member _.StartWarmUp(targetScenarios: Scenario list) = task {
        _stopped <- false
        _currentOperation <- OperationType.WarmUp
        do! Task.Yield()

        _logger.Information "Starting warm up..."
        let schedulers = this.CreateScenarioSchedulers(targetScenarios, ScenarioStatsActor.create)
        do! startWarmUp schedulers
        stopSchedulers schedulers

        _currentOperation <- OperationType.None
    }

    member _.StartWarmUp() = this.StartWarmUp(_targetScenarios)

    member _.StartBombing(schedulers: ScenarioScheduler list,
                          reportingTimer: Timers.Timer,
                          currentOperationTimer: Stopwatch) = task {
        _stopped <- false
        _currentOperation <- OperationType.Bombing
        do! Task.Yield()

        _logger.Information "Starting bombing..."
        do! startBombing(schedulers, reportingTimer, currentOperationTimer)
        do! this.StopScenarios()

        _currentOperation <- OperationType.Complete
    }

    member _.StopScenarios([<Optional;DefaultParameterValue("":string)>]reason: string) = task {
        if _currentOperation <> OperationType.Stop && not _stopped then
            _currentOperation <- OperationType.Stop
            do! Task.Yield()

            if not(String.IsNullOrEmpty reason) then
                _logger.Warning("Stopping test early: '{0}'", reason)
            else
                _logger.Information "Stopping scenarios..."

            stopSchedulers _currentSchedulers
            do! cleanScenarios()

            _stopped <- true
            _currentOperation <- OperationType.None
    }

    member _.GetHints(finalStats) = getHints(finalStats)

    member _.CreateScenarioSchedulers(createStatsActor: ILogger -> Scenario -> TimeSpan -> MailboxProcessor<_>) =
        createScenarioSchedulers _targetScenarios createStatsActor

    member _.CreateScenarioSchedulers(targetScenarios: Scenario list, createStatsActor: ILogger -> Scenario -> TimeSpan -> MailboxProcessor<_>) =
        createScenarioSchedulers targetScenarios createStatsActor

    member _.RunSession(sessionArgs: SessionArgs) = taskResult {
        do! this.StartInit(sessionArgs)
        do! this.StartWarmUp()

        let schedulers = this.CreateScenarioSchedulers(ScenarioStatsActor.create)
        use reportingActor = TestHostReportingActor.create dep schedulers sessionArgs.TestInfo
        reportingActor.Error.Subscribe(fun ex -> _logger.Error("Reporting actor error", ex)) |> ignore

        let currentOperationTimer = Stopwatch()
        use reportingTimer = new Timers.Timer(sessionArgs.SendStatsInterval.TotalMilliseconds)
        reportingTimer.Elapsed.Add(fun _ ->
            reportingActor.Post(FetchAndSaveRealtimeStats currentOperationTimer.Elapsed)
        )

        do! this.StartBombing(schedulers, reportingTimer, currentOperationTimer)

        // gets final stats
        _logger.Information "Calculating final statistics..."
        let finalStats = reportingActor.PostAndReply(fun reply -> GetFinalStats(getCurrentNodeInfo(), reply))

        let timeLineHistory =
            reportingActor.PostAndReply(fun reply -> GetTimeLineHistory reply)
            |> List.toArray

        let hints = getHints finalStats

        return { FinalStats = finalStats; TimeLineHistory = timeLineHistory; Hints = hints }
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

                _logger.Verbose $"{nameof TestHost} disposed."
