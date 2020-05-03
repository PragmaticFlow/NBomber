namespace NBomber.DomainServices.TestHost

open System
open System.Diagnostics
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open FsToolkit.ErrorHandling

open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Concurrency.ScenarioActor
open NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler
open NBomber.Domain.Statistics
open NBomber.DomainServices.TestHost.Infra
open NBomber.Errors
open NBomber.Infra.Dependency

type internal TestHost(dep: GlobalDependency, registeredScenarios: Scenario list) as x =

    let mutable _stopped = false
    let mutable _sessionArgs = SessionArgs.empty
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
        let pluginStats = dep.Plugins |> Seq.map(fun x -> x.GetStats()) |> Seq.toArray
        let scnStats = _scnSchedulers |> Seq.map(fun x -> x.GetScenarioStats executionTime) |> Seq.toArray
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

    let initScenarios (sessionArgs: SessionArgs) = taskResult {

        let defaultScnContext = {
            NodeInfo = getCurrentNodeInfo()
            CustomSettings = ""
            CancellationToken = _cancelToken.Token
            Logger = dep.Logger
        }

        let! updatedScenarios = TestHostScenario.initScenarios(dep, defaultScnContext, registeredScenarios, sessionArgs)
        TestHostPlugins.startPlugins dep sessionArgs.TestInfo
        TestHostReporting.startReportingSinks dep sessionArgs.TestInfo

        _sessionArgs <- sessionArgs
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

    let addTimeLineStats (timeLineStats: (TimeSpan * NodeStats) list)
                         (operationTime: TimeSpan, nodeStats: NodeStats list) =
        nodeStats
        |> List.tryHead
        |> Option.map(fun stats -> timeLineStats @ [ operationTime, stats ])
        |> Option.defaultValue timeLineStats

    member x.TestInfo = _sessionArgs.TestInfo
    member x.CurrentOperation = _currentOperation
    member x.CurrentNodeInfo = getCurrentNodeInfo()
    member x.RegisteredScenarios = registeredScenarios
    member x.TargetScenarios = _targetScenarios

    member x.StartInitScenarios(args: SessionArgs) = task {
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

    member x.StartWarmUpScenarios() = task {
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
            TestHostPlugins.stopPlugins dep
            TestHostReporting.stopReportingSinks dep
            _stopped <- true

            _currentOperation <- NodeOperationType.None
    }

    member x.GetNodeStats(executionTime) = getNodeStats(executionTime)

    member x.GetTimeLineNodeStats() = _timeLineStats

    member x.RunSession(args: SessionArgs) = taskResult {
        do! x.StartInitScenarios(args)

        let currentOperationTimer = Stopwatch()

        // warm-up
        currentOperationTimer.Restart()
        do! x.StartWarmUpScenarios()
        let warmUpStats = getNodeStats(currentOperationTimer.Elapsed)
        do! Scenario.Validation.validateWarmUpStats [warmUpStats]

        // bombing
        use reportingTimer =
            TestHostReporting.startReportingTimer(
                dep, _sessionArgs.SendStatsInterval,
                (fun () -> _currentOperation, currentOperationTimer.Elapsed),
                (fun operationTime -> operationTime |> getNodeStats |> List.singleton |> Task.FromResult),
                (fun (operationTime, nodeStats) ->
                    _timeLineStats <- addTimeLineStats _timeLineStats (operationTime, nodeStats)
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
            dep.Plugins |> Seq.iter(fun x -> x.Dispose())
            dep.ReportingSinks |> Seq.iter(fun x -> x.Dispose())
