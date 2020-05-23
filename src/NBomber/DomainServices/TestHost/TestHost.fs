namespace NBomber.DomainServices.TestHost

open System
open System.Threading
open System.Threading.Tasks
open System.Diagnostics
open System.Runtime.InteropServices

open Nessos.Streams
open FSharp.Control.Tasks.V2.ContextInsensitive
open FsToolkit.ErrorHandling

open NBomber.Contracts
open NBomber.Errors
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Statistics
open NBomber.Domain.Concurrency.ScenarioActor
open NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler
open NBomber.Infra.Dependency
open NBomber.DomainServices.NBomberContext

type internal TestHost(dep: IGlobalDependency, registeredScenarios: Scenario list, sessionArgs: SessionArgs) as x =

    let mutable _stopped = false
    let mutable _targetScenarios = List.empty<Scenario>
    let mutable _currentOperation = NodeOperationType.None
    let mutable _scnSchedulers = List.empty<ScenarioScheduler>
    let mutable _cancelToken = new CancellationTokenSource()
    let mutable _timeLineStats: (TimeSpan * NodeStats) list = List.empty
    let _defaultNodeInfo = NodeInfo.init()

    let getCurrentNodeInfo () =
        { _defaultNodeInfo with NodeType = dep.NodeType; CurrentOperation = _currentOperation }

    let getNodeStats (executionTime) =
        let nodeInfo = getCurrentNodeInfo()
        let pluginStats = dep.Plugins |> Stream.ofList |> Stream.map(fun x -> x.GetStats())
        let scnStats = _scnSchedulers |> Stream.ofList |> Stream.map(fun x -> x.GetScenarioStats executionTime)
        NodeStats.create nodeInfo scnStats pluginStats

    let execStopCommand (command: StopCommand) =
        match command with
        | StopScenario (scenarioName, reason) ->
            _scnSchedulers
            |> List.tryFind(fun x -> x.Scenario.ScenarioName = scenarioName)
            |> Option.iter(fun x ->
                if x.Stop() then dep.Logger.Warning("stopping scenario early: '{0}', reason: '{1}'", x.Scenario.ScenarioName, reason)
            )

        | StopTest (reason) -> x.StopScenarios(reason) |> ignore

    let createScenarioSchedulers (targetScenarios: Scenario list) =
        let createScheduler (cancelToken: CancellationToken) (scn: Scenario) =
            let actorDep = {
                Logger = dep.Logger
                CancellationToken = cancelToken
                GlobalTimer = Stopwatch()
                Scenario = scn
                ExecStopCommand = execStopCommand
            }
            new ScenarioScheduler(actorDep)

        _scnSchedulers |> List.iter(fun x -> (x :> IDisposable).Dispose())
        _cancelToken.Dispose()
        _cancelToken <- new CancellationTokenSource()

        targetScenarios
        |> List.map(createScheduler _cancelToken.Token)

    let initScenarios () = taskResult {

        let defaultScnContext = {
            NodeInfo = getCurrentNodeInfo()
            CustomSettings = ""
            CancellationToken = _cancelToken.Token
            Logger = dep.Logger
        }

        let! updatedScenarios = TestHostScenario.initScenarios(dep, defaultScnContext, registeredScenarios, sessionArgs)
        TestHostPlugins.startPlugins dep sessionArgs.TestInfo
        TestHostReporting.startReportingSinks dep sessionArgs.TestInfo

        _targetScenarios <- updatedScenarios
    }

    let stopScenarios () =
        if not _cancelToken.IsCancellationRequested then
            _cancelToken.Cancel()

    let cleanScenarios () =
        let defaultScnContext = {
            NodeInfo = getCurrentNodeInfo()
            CustomSettings = ""
            CancellationToken = _cancelToken.Token
            Logger = dep.Logger
        }
        TestHostScenario.cleanScenarios(dep, defaultScnContext, _targetScenarios)

    let startBombing (isWarmUp) = task {
        _scnSchedulers <- createScenarioSchedulers(_targetScenarios)
        let progressBars = TestHostConsole.displayBombingProgress(dep, _scnSchedulers, isWarmUp)
        do! _scnSchedulers |> List.map(fun x -> x.Start(isWarmUp)) |> Task.WhenAll
        progressBars |> List.iter(fun x -> x.Dispose())
    }

    member x.TestInfo = sessionArgs.TestInfo
    member x.CurrentOperation = _currentOperation
    member x.CurrentNodeInfo = getCurrentNodeInfo()
    member x.RegisteredScenarios = registeredScenarios
    member x.TargetScenarios = _targetScenarios

    member x.StartInit() = task {
        _stopped <- false
        _currentOperation <- NodeOperationType.Init
        do! Task.Yield()

        TestHostConsole.printContextInfo(dep)

        dep.Logger.Information("starting init...")
        match! initScenarios() with
        | Ok _ ->
            dep.Logger.Information("init finished")
            _currentOperation <- NodeOperationType.None
            return Ok()

        | Error e ->
            dep.Logger.Information("init failed")
            _currentOperation <- NodeOperationType.Stop
            return AppError.createResult(InitScenarioError e)
    }

    member x.StartWarmUp() = task {
        _stopped <- false
        _currentOperation <- NodeOperationType.WarmUp
        do! Task.Yield()

        dep.Logger.Information("starting warm up...")
        let isWarmUp = true
        do! startBombing(isWarmUp)
        stopScenarios()

        _currentOperation <- NodeOperationType.None
    }

    member x.StartBombing() = task {
        _stopped <- false
        _currentOperation <- NodeOperationType.Bombing
        do! Task.Yield()

        dep.Logger.Information("starting bombing...")
        let isWarmUp = false
        do! startBombing(isWarmUp)
        do! x.StopScenarios()

        _currentOperation <- NodeOperationType.Complete
    }

    member x.StopScenarios([<Optional;DefaultParameterValue("":string)>]reason: string) = task {
        if _currentOperation <> NodeOperationType.Stop && _stopped = false then
            _currentOperation <- NodeOperationType.Stop
            do! Task.Yield()

            if not(String.IsNullOrEmpty reason) then
                dep.Logger.Warning("stopping test early: '{0}'", reason)
            else
                dep.Logger.Information("stopping scenarios...")

            stopScenarios()
            cleanScenarios()
            TestHostPlugins.stopPlugins(dep)
            TestHostReporting.stopReportingSinks(dep)
            _stopped <- true

            _currentOperation <- NodeOperationType.None
    }

    member x.GetNodeStats(executionTime) = getNodeStats(executionTime)
    member x.GetTimeLineNodeStats() = List.rev _timeLineStats

    member x.RunSession() = taskResult {
        do! x.StartInit()

        let currentOperationTimer = Stopwatch()

        // warm-up
        currentOperationTimer.Restart()
        do! x.StartWarmUp()
        let warmUpStats = getNodeStats(currentOperationTimer.Elapsed)
        do! Scenario.Validation.validateWarmUpStats [warmUpStats]

        // bombing
        use reportingTimer =
            TestHostReporting.startReportingTimer(
                dep, sessionArgs.SendStatsInterval,
                (fun () ->
                    let operationTime = currentOperationTimer.Elapsed
                    let nodeStats = getNodeStats(operationTime)
                    // prepend nodeStats to timeLineStats
                    _timeLineStats <- (operationTime, nodeStats) :: _timeLineStats
                    _currentOperation, nodeStats
                )
            )

        currentOperationTimer.Restart()
        do! x.StartBombing()
        reportingTimer.Stop()

        let finalStats = getNodeStats(currentOperationTimer.Elapsed)
        TestHostReporting.saveFinalStats dep [finalStats]

        return finalStats
    }

    interface IDisposable with
        member x.Dispose() =
            x.StopScenarios().Wait()
            dep.Dispose()
