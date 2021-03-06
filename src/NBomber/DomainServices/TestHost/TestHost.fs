namespace NBomber.DomainServices.TestHost

open System
open System.Threading
open System.Threading.Tasks
open System.Diagnostics
open System.Runtime.InteropServices

open Nessos.Streams
open FSharp.Control.Tasks.NonAffine
open FsToolkit.ErrorHandling

open NBomber.Contracts
open NBomber.Errors
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Concurrency.ScenarioActor
open NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler
open NBomber.Infra.Dependency
open NBomber.DomainServices
open NBomber.DomainServices.NBomberContext
open NBomber.DomainServices.TestHost.TestHostReporting

type internal TestHost(dep: IGlobalDependency, registeredScenarios: Scenario list, sessionArgs: SessionArgs) as this =

    let mutable _stopped = false
    let mutable _targetScenarios = List.empty<Scenario>
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
                dep.Logger.Warning("Stopping scenario early: '{0}', reason: '{1}'", sch.Scenario.ScenarioName, reason)
            )

        | StopTest reason -> this.StopScenarios(reason) |> ignore

    let createScenarioSchedulers (targetScenarios: Scenario list) =

        let createScheduler (cancelToken: CancellationToken) (scn: Scenario) =
            let actorDep = {
                Logger = dep.Logger
                CancellationToken = cancelToken
                GlobalTimer = Stopwatch()
                Scenario = scn
                ExecStopCommand = execStopCommand
            }
            ScenarioScheduler(actorDep)

        _currentSchedulers |> List.iter(fun x -> x.Stop())
        _cancelToken.Dispose()
        _cancelToken <- new CancellationTokenSource()

        targetScenarios
        |> List.map(createScheduler _cancelToken.Token)

    let initScenarios () = taskResult {
        let baseContext = NBomberContext.createBaseContext(sessionArgs.TestInfo, getCurrentNodeInfo(), _cancelToken.Token, dep.Logger)
        let defaultScnContext = Scenario.ScenarioContext.create baseContext
        let targetScenarios = TestHostScenario.getTargetScenarios sessionArgs registeredScenarios

        do! TestHostReporting.initReportingSinks dep baseContext
        do! TestHostPlugins.initPlugins dep baseContext
        let! initializedScenarios = TestHostScenario.initScenarios dep baseContext defaultScnContext sessionArgs targetScenarios

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
        do! TestHostReporting.startReportingSinks dep
        do! TestHostPlugins.startPlugins dep

        reportingTimer.Start()
        currentOperationTimer.Start()

        do! schedulers |> List.map(fun x -> x.Start isWarmUp) |> Task.WhenAll

        reportingTimer.Stop()
        currentOperationTimer.Stop()

        do! TestHostReporting.stopReportingSinks dep
        do! TestHostPlugins.stopPlugins dep
    }

    let stopSchedulers () =
        if not _cancelToken.IsCancellationRequested then
            _cancelToken.Cancel()

        _currentSchedulers |> List.iter(fun x -> x.Stop())

    let cleanScenarios () =
        let baseContext = NBomberContext.createBaseContext(sessionArgs.TestInfo, getCurrentNodeInfo(), _cancelToken.Token, dep.Logger)
        let defaultScnContext = Scenario.ScenarioContext.create baseContext
        TestHostScenario.cleanScenarios dep baseContext defaultScnContext _targetScenarios

    member _.TestInfo = sessionArgs.TestInfo
    member _.CurrentOperation = _currentOperation
    member _.CurrentNodeInfo = getCurrentNodeInfo()
    member _.RegisteredScenarios = registeredScenarios
    member _.TargetScenarios = _targetScenarios

    member _.StartInit() = task {
        _stopped <- false
        _currentOperation <- OperationType.Init
        do! Task.Yield()

        TestHostConsole.printContextInfo(dep)

        dep.Logger.Information("Starting init...")
        match! initScenarios() with
        | Ok _ ->
            dep.Logger.Information("Init finished.")
            _currentOperation <- OperationType.None
            return Ok()

        | Error appError ->
            dep.Logger.Fatal("Init failed.")
            _currentOperation <- OperationType.Stop
            return AppError.createResult(appError)
    }

    member _.StartWarmUp(schedulers: ScenarioScheduler list) = task {
        _stopped <- false
        _currentOperation <- OperationType.WarmUp
        do! Task.Yield()

        dep.Logger.Information("Starting warm up...")
        do! startWarmUp(schedulers)
        stopSchedulers()

        _currentOperation <- OperationType.None
    }

    member _.StartBombing(schedulers: ScenarioScheduler list,
                          reportingTimer: Timers.Timer,
                          currentOperationTimer: Stopwatch) = task {
        _stopped <- false
        _currentOperation <- OperationType.Bombing
        do! Task.Yield()

        dep.Logger.Information("Starting bombing...")
        do! startBombing(schedulers, reportingTimer, currentOperationTimer)
        do! this.StopScenarios()

        _currentOperation <- OperationType.Complete
    }

    member _.StopScenarios([<Optional;DefaultParameterValue("":string)>]reason: string) = task {
        if _currentOperation <> OperationType.Stop && not _stopped then
            _currentOperation <- OperationType.Stop
            do! Task.Yield()

            if not(String.IsNullOrEmpty reason) then
                dep.Logger.Warning("Stopping test early: '{0}'", reason)
            else
                dep.Logger.Information("Stopping scenarios...")

            stopSchedulers()
            do! cleanScenarios()

            _stopped <- true
            _currentOperation <- OperationType.None
    }

    member _.GetScenarioBombingStats(duration) =
        TestHostReporting.getScenarioStats _currentSchedulers (Some true) duration
        |> Stream.toArray

    member _.GetFinalStats(duration) =
        TestHostReporting.getNodeStats dep _currentSchedulers sessionArgs.TestInfo (getCurrentNodeInfo()) OperationType.Complete None duration

    member _.CreateScenarioSchedulers() = createScenarioSchedulers(_targetScenarios)

    member _.RunSession() = taskResult {
        do! this.StartInit()
        do! this.StartWarmUp(this.CreateScenarioSchedulers())

        let schedulers = this.CreateScenarioSchedulers()
        use actor = TestHostReporting.createReportingActor(dep, schedulers, sessionArgs.TestInfo)
        actor.Error.Subscribe(fun ex -> dep.Logger.Fatal("Reporting actor error", ex)) |> ignore

        let currentOperationTimer = Stopwatch()
        use reportingTimer = new Timers.Timer(sessionArgs.SendStatsInterval.TotalMilliseconds)
        reportingTimer.Elapsed.Add(fun _ ->
            actor.Post(FetchAndSaveBombingStats currentOperationTimer.Elapsed)
        )

        // subscribes on stop scenario event
        schedulers
        |> List.iter(fun x ->
            x.EventStream
            |> Observable.choose(function
                | ScenarioStopped stats -> Some stats
                | _ -> None
            )
            |> Observable.subscribe(fun stats ->
                actor.Post(AddAndSaveScenarioStats stats)
            )
            |> ignore
        )

        do! this.StartBombing(schedulers, reportingTimer, currentOperationTimer)

        // gets final stats
        dep.Logger.Information("Calculating final statistics...")
        let finalStats = actor.PostAndReply(fun reply -> GetFinalStats(getCurrentNodeInfo(), currentOperationTimer.Elapsed, reply))
        let timeLines = actor.PostAndReply(fun reply -> GetTimeLines reply)
        return {| NodeStats = finalStats; TimeLines = timeLines |}
    }

    interface IDisposable with
        member _.Dispose() =
            this.StopScenarios().Wait()

            for sink in dep.ReportingSinks do
                use x = sink
                ()

            for plugin in dep.WorkerPlugins do
                use x = plugin
                ()
