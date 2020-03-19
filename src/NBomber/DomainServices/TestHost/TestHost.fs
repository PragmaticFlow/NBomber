namespace NBomber.DomainServices.TestHost

open System
open System.Threading
open System.Threading.Tasks
open System.Diagnostics

open FSharp.Control.Tasks.V2.ContextInsensitive
open FsToolkit.ErrorHandling

open NBomber
open NBomber.Contracts
open NBomber.Extensions
open NBomber.Errors
open NBomber.Domain
open NBomber.Domain.Statistics
open NBomber.Domain.Concurrency.ScenarioActor
open NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler
open NBomber.Infra.Dependency
open NBomber.DomainServices.Validation
open NBomber.DomainServices.TestHost.Infra

type internal TestHost(dep: GlobalDependency, registeredScenarios: Scenario list) =

    let mutable _sessionArgs = TestSessionArgs.empty
    let mutable _targetScenarios = List.empty<Scenario>
    let mutable _currentOperation = NodeOperationType.None
    let mutable _scnSchedulers = List.empty<ScenarioScheduler>
    let mutable _cancelToken = new CancellationTokenSource()
    let _currentOperationTimer = Stopwatch()

    let getCurrentNodeInfo () =
        { MachineName = dep.MachineInfo.MachineName
          Sender = dep.NodeType
          CurrentOperation = _currentOperation }

    let getNodeStats (executionTime) =
        _scnSchedulers
        |> Seq.map(fun x -> x.GetScenarioStats executionTime)
        |> Seq.toArray
        |> NodeStats.create(getCurrentNodeInfo())
        |> List.singleton

    let createScenarioSchedulers (targetScns: Scenario list) =
        let createScheduler (cancelToken: CancellationToken) (scn: Scenario) =
            let actorDep = {
                Logger = dep.Logger
                CancellationToken = cancelToken
                GlobalTimer = Stopwatch()
                Scenario = scn
            }
            new ScenarioScheduler(actorDep)

        _scnSchedulers |> List.iter(fun x -> (x :> IDisposable).Dispose())
        _cancelToken.Dispose()
        _cancelToken <- new CancellationTokenSource()

        targetScns
        |> List.map(createScheduler _cancelToken.Token)

    let initScenarios (sessionArgs: TestSessionArgs) = asyncResult {
        _sessionArgs <- sessionArgs

        let scnContext = {
            NodeInfo = getCurrentNodeInfo()
            CustomSettings = _sessionArgs.CustomSettings
            CancellationToken = _cancelToken.Token
            Logger = dep.Logger
        }

        let! updatedScenarios = TestHostScenario.initScenarios(dep, scnContext, registeredScenarios, _sessionArgs)
        _targetScenarios <- updatedScenarios
    }

    let stopScenarios () =
        if not _cancelToken.IsCancellationRequested then
            _cancelToken.Cancel()
            TestHostScenario.waitForNotFinishedScenarios(dep, 0, _scnSchedulers)
            |> Async.StartAsTask
        else
            Task.FromResult()

    let cleanScenarios () =
        let scnContext = {
            NodeInfo = getCurrentNodeInfo()
            CustomSettings = _sessionArgs.CustomSettings
            CancellationToken = _cancelToken.Token
            Logger = dep.Logger
        }
        TestHostScenario.cleanScenarios(dep, scnContext, _targetScenarios)

    let startBombing (isWarmUp) = task {
        if isWarmUp then dep.Logger.Information("starting warm up...")
        else dep.Logger.Information("starting bombing...")

        _scnSchedulers <- createScenarioSchedulers(_targetScenarios)

        let scnsProgressSubscriptions = TestHostConsole.displayBombingProgress(dep, _scnSchedulers, isWarmUp)
        do! _scnSchedulers |> List.map(fun x -> x.Start(isWarmUp)) |> Task.WhenAll
        scnsProgressSubscriptions |> List.iter(fun x -> x.Dispose())
    }

    member x.TestInfo = _sessionArgs.TestInfo
    member x.CurrentOperation = _currentOperation
    member x.CurrentNodeInfo = getCurrentNodeInfo()
    member x.RegisteredScenarios = registeredScenarios
    member x.TargetScenarios = _targetScenarios

    member x.InitScenarios(args: TestSessionArgs) = task {
        _currentOperation <- NodeOperationType.Init
        _currentOperationTimer.Restart()
        do! Task.Yield()

        match! initScenarios(args) with
        | Ok _ ->
            _currentOperationTimer.Stop()
            _currentOperation <- NodeOperationType.None
            return Ok()

        | Error e ->
            _currentOperationTimer.Stop()
            _currentOperation <- NodeOperationType.Stop
            return AppError.createResult(InitScenarioError e)
    }

    member x.WarmUpScenarios() = task {
        _currentOperation <- NodeOperationType.WarmUp
        _currentOperationTimer.Restart()
        do! Task.Yield()

        let isWarmUp = true
        do! startBombing(isWarmUp)
        do! stopScenarios()

        _currentOperationTimer.Stop()
        _currentOperation <- NodeOperationType.None
        do! Task.Delay(Constants.OperationTimeOut)
    }

    member x.StartBombing() = task {
        _currentOperation <- NodeOperationType.Bombing
        _currentOperationTimer.Restart()
        do! Task.Yield()

        let isWarmUp = false
        do! startBombing(isWarmUp)
        do! stopScenarios()
        cleanScenarios()

        _currentOperationTimer.Stop()
        _currentOperation <- NodeOperationType.Complete
        do! Task.Delay(Constants.OperationTimeOut)
    }

    member x.StopScenarios() = task {
        _currentOperation <- NodeOperationType.Stop
        _currentOperationTimer.Restart()
        do! Task.Yield()

        if _scnSchedulers |> List.exists(fun x -> x.Working) then
            do! stopScenarios()
            cleanScenarios()

        _currentOperationTimer.Stop()
        _currentOperation <- NodeOperationType.None
    }

    member x.GetNodeStats(duration) = getNodeStats(duration)

    member x.RunSession(args: TestSessionArgs) = asyncResult {
        do! x.InitScenarios(args)

        // warm-up
        do! x.WarmUpScenarios()
        let warmUpStats = getNodeStats(_currentOperationTimer.Elapsed)
        do! Scenario.Validation.validateWarmUpStats(warmUpStats)

        // bombing
        use reportingTimer =
            TestHostReporting.startReportingTimer(
                dep, _sessionArgs,
                (fun () -> _currentOperation, _currentOperationTimer.Elapsed),
                getNodeStats)

        do! x.StartBombing()
        reportingTimer.Stop()

        return getNodeStats(_currentOperationTimer.Elapsed)
    }

    interface IDisposable with
        member x.Dispose() =
            if _currentOperation <> NodeOperationType.Complete then x.StopScenarios().Wait()
