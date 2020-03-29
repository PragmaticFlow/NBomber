namespace NBomber.DomainServices.TestHost

open System
open System.Threading
open System.Threading.Tasks
open System.Diagnostics
open System.Runtime.InteropServices

open FSharp.Control.Tasks.V2.ContextInsensitive
open FsToolkit.ErrorHandling

open NBomber
open NBomber.Contracts
open NBomber.Errors
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Statistics
open NBomber.Domain.Concurrency.ScenarioActor
open NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler
open NBomber.Infra.Dependency
open NBomber.DomainServices.TestHost.Infra

type internal TestHost(dep: GlobalDependency, registeredScenarios: Scenario list) as x =

    let mutable _stopped = false
    let mutable _sessionArgs = TestSessionArgs.empty
    let mutable _targetScenarios = List.empty<Scenario>
    let mutable _currentOperation = NodeOperationType.None
    let mutable _scnSchedulers = List.empty<ScenarioScheduler>
    let mutable _cancelToken = new CancellationTokenSource()

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

    let execStopCommand (command: StopCommand) =
        match command with
        | StopScenario (scenarioName, reason) ->
            _scnSchedulers
            |> List.tryFind(fun x -> x.Scenario.ScenarioName = scenarioName)
            |> Option.iter(fun x ->
                if x.Stop() then dep.Logger.Warning("stopping scenario early: '{0}', reason: '{1}'", x.Scenario.ScenarioName, reason)
            )

        | StopTest (reason) -> x.StopScenarios(reason) |> ignore

    let createScenarioSchedulers (targetScns: Scenario list) =
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

        targetScns
        |> List.map(createScheduler _cancelToken.Token)

    let initScenarios (sessionArgs: TestSessionArgs) = asyncResult {
        _sessionArgs <- sessionArgs

        let defaultScnContext = {
            NodeInfo = getCurrentNodeInfo()
            CustomSettings = ""
            CancellationToken = _cancelToken.Token
            Logger = dep.Logger
        }

        let! updatedScenarios = TestHostScenario.initScenarios(dep, defaultScnContext, registeredScenarios, _sessionArgs)
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
        _stopped <- false
        _currentOperation <- NodeOperationType.Init
        do! Task.Yield()

        match! initScenarios(args) with
        | Ok _ ->
            _currentOperation <- NodeOperationType.None
            return Ok()

        | Error e ->
            _currentOperation <- NodeOperationType.Stop
            return AppError.createResult(InitScenarioError e)
    }

    member x.WarmUpScenarios() = task {
        _stopped <- false
        _currentOperation <- NodeOperationType.WarmUp
        do! Task.Yield()

        dep.Logger.Information("starting warm up...")
        let isWarmUp = true
        do! startBombing(isWarmUp)
        stopScenarios()

        _currentOperation <- NodeOperationType.None
        do! Task.Delay(Constants.OperationTimeOut)
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
        do! Task.Delay(Constants.OperationTimeOut)
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
            _stopped <- true

            _currentOperation <- NodeOperationType.None
    }

    member x.GetNodeStats(duration) = getNodeStats(duration)

    member x.RunSession(args: TestSessionArgs) = asyncResult {
        do! x.InitScenarios(args)

        let currentOperationTimer = Stopwatch()

        // warm-up
        currentOperationTimer.Restart()
        do! x.WarmUpScenarios()
        let warmUpStats = getNodeStats(currentOperationTimer.Elapsed)
        do! Scenario.Validation.validateWarmUpStats(warmUpStats)

        // bombing
        use reportingTimer =
            TestHostReporting.startReportingTimer(
                dep, _sessionArgs,
                (fun () -> _currentOperation, currentOperationTimer.Elapsed),
                getNodeStats)

        currentOperationTimer.Restart()
        do! x.StartBombing()
        reportingTimer.Stop()

        return getNodeStats(currentOperationTimer.Elapsed)
    }

    interface IDisposable with
        member x.Dispose() =
            x.StopScenarios().Wait()
