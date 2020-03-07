module internal rec NBomber.DomainServices.TestHost

open System
open System.Threading
open System.Threading.Tasks
open System.Diagnostics

open FSharp.Control.Reactive
open FSharp.Control.Tasks.V2.ContextInsensitive
open FsToolkit.ErrorHandling

open NBomber
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Extensions
open NBomber.Errors
open NBomber.Domain
open NBomber.Domain.Statistics
open NBomber.Domain.Concurrency.ScenarioActor
open NBomber.Domain.Concurrency.Scheduler.ScenarioScheduler
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices.Validation

type TestSessionArgs = {
    TestInfo: TestInfo
    ScenariosSettings: ScenarioSetting[]
    TargetScenarios: string[]
    CustomSettings: string
    SendStatsInterval: TimeSpan
} with
  static member empty = {
      TestInfo = { SessionId = ""; TestSuite = ""; TestName = "" }
      ScenariosSettings = Array.empty
      TargetScenarios = Array.empty
      CustomSettings = ""
      SendStatsInterval = TimeSpan.FromSeconds(Constants.MinSendStatsIntervalSec)
  }

  static member getFromContext (testInfo: TestInfo, context: TestContext) =
      let scnSettings = TestContext.getScenariosSettings(context)
      let targetScns = TestContext.getTargetScenarios(context)
      let customSettings = TestContext.getCustomSettings(context)
      let statsInterval = TestContext.getSendStatsInterval(context)
      { TestInfo = testInfo
        ScenariosSettings = scnSettings
        TargetScenarios = targetScns
        CustomSettings = customSettings
        SendStatsInterval = statsInterval }

let buildInitConnectionPools (dep: GlobalDependency) =
    if dep.ApplicationType = ApplicationType.Console then
        let mutable pb = Unchecked.defaultof<ShellProgressBar.ProgressBar>

        let onStartedInitPool = fun (_, connectionsCount) ->
            pb <- dep.CreateProgressBar(connectionsCount)

        let onConnectionOpened = fun _ ->
            pb.Tick()

        let onFinishInitPool = fun _ ->
            pb.Dispose()

        fun scenario ->
            ConnectionPool.init(scenario, onStartedInitPool, onConnectionOpened, onFinishInitPool, dep.Logger)
    else
        fun scenario ->
            ConnectionPool.init(scenario, ignore, ignore, ignore, dep.Logger)

// todo: run in parallel
let runInitScenarios (dep: GlobalDependency) (customSettings: string) (scenarios: Scenario[]) = task {
    let mutable failed = false
    let mutable error = Unchecked.defaultof<_>

    let flow = seq {
        for scn in scenarios do
            if not failed then
                dep.Logger.Information("initializing scenario: '{0}', concurrent copies: '{1}'", scn.ScenarioName, 0)

                let initAllConnectionPools = buildInitConnectionPools(dep)
                let initResult = Scenario.init(scn, initAllConnectionPools, customSettings, dep.NodeType, dep.Logger)
                if Result.isError(initResult) then
                    failed <- true
                    error <- initResult
                yield initResult
    }

    let results = flow |> Seq.toArray
    let allOk   = results |> Array.forall(Result.isOk)

    return if allOk then results |> Array.map(Result.getOk) |> Ok
           else error |> Result.getError |> Error
}

let createNodeInfo (dep: GlobalDependency, currentOperation: NodeOperationType) =
    { MachineName = dep.MachineInfo.MachineName
      Sender = dep.NodeType
      CurrentOperation = currentOperation }

module TestHostReporting =

    let saveStats (testInfo: TestInfo, nodeStats: RawNodeStats[], sinks: IReportingSink[]) =
        nodeStats
        |> Array.collect(fun x ->
            let stats = Statistics.create(x)
            sinks |> Array.map(fun snk -> snk.SaveRealtimeStats(testInfo, stats))
        )
        |> Task.WhenAll

type TestHost(dep: GlobalDependency, registeredScenarios: Scenario[]) =

    let mutable _scnArgs = TestSessionArgs.empty
    let mutable _targetScenarios = Array.empty<Scenario>
    let mutable _currentOperation = NodeOperationType.None
    let mutable _scnSchedulers = Array.empty<ScenarioScheduler>
    let mutable _cancelToken = new CancellationTokenSource()
    let _currentOperationTimer = Stopwatch()

    let getNodeStats (executionTime) =
        _scnSchedulers
        |> Array.map(fun x -> x.GetScenarioStats executionTime)
        |> NodeStats.create(TestHost.createNodeInfo(dep, _currentOperation))

    let filterTargetScenarios (scns: Scenario[], args: TestSessionArgs) =
        scns
        |> Scenario.applySettings(args.ScenariosSettings)
        |> Scenario.filterTargetScenarios(args.TargetScenarios)

    let printTargetScenarios (targetScns: Scenario[]) =
        targetScns
        |> Array.map(fun x -> x.ScenarioName)
        |> fun targets -> dep.Logger.Information("target scenarios: {0}", String.concatWithCommaAndQuotes(targets))

    let tryDisplayProgress (scnSchedulers: ScenarioScheduler[], isWarmUp: bool) =
        match dep.ApplicationType with
        | ApplicationType.Console ->
            scnSchedulers
            |> Array.map(fun x ->

                let tickCount =
                    if isWarmUp then int x.Scenario.WarmUpDuration.TotalMilliseconds / Constants.NotificationTickIntervalMs
                    else int x.Scenario.Duration.TotalMilliseconds / Constants.NotificationTickIntervalMs

                let pb = dep.CreateProgressBar(tickCount)
                x.ProgressInfoStream
                |> Observable.subscribeWithCompletion
                    (fun x -> let msg = sprintf "keep concurrent scenarios: '%i', inject scenarios per sec: '%i'" x.ConstantActorCount x.OneTimeActorCount
                              pb.Tick(msg))

                    (fun () -> pb.Dispose())
            )
        | _ -> Array.empty

    let createScenarioSchedulers (targetScns: Scenario[]) =
        let createScheduler (cancelToken: CancellationToken) (scn: Scenario) =
            let actorDep = {
                Logger = dep.Logger
                CancellationToken = cancelToken
                GlobalTimer = Stopwatch()
                Scenario = scn
            }
            new ScenarioScheduler(actorDep)

        _scnSchedulers |> Array.iter(fun x -> (x :> IDisposable).Dispose())
        _cancelToken.Dispose()
        _cancelToken <- new CancellationTokenSource()

        targetScns
        |> Array.map(createScheduler _cancelToken.Token)

    let initScenarios (args: TestSessionArgs) = asyncResult {
        let scns = filterTargetScenarios(registeredScenarios, args)

        printTargetScenarios(scns)
        let! initedScns = runInitScenarios dep args.CustomSettings scns

        _targetScenarios <- initedScns
        _scnSchedulers <- createScenarioSchedulers(_targetScenarios)
    }

    let rec waitForNotFinishedTasks (tryCount: int, scnSchedulers: ScenarioScheduler[]) = async {
        let waitingTaskCount =
            scnSchedulers
            |> Seq.collect(fun x -> x.AllActors)
            |> Seq.filter(fun x -> x.Working)
            |> Seq.length

        if tryCount >= Constants.ReTryCount then
            dep.Logger.Information("hard stop with '{WaitingTaskCount}' not finished tasks", waitingTaskCount)

        elif waitingTaskCount <> 0 then
            dep.Logger.Information("waiting for not finished tasks '{WaitingTaskCount}'...", waitingTaskCount)
            do! Async.Sleep(Constants.OperationTimeOut)
            return! waitForNotFinishedTasks(tryCount + 1, scnSchedulers)
    }

    let stopScenarios () =
        if not _cancelToken.IsCancellationRequested then
            _cancelToken.Cancel()
            waitForNotFinishedTasks(0, _scnSchedulers)
            |> Async.StartAsTask
        else
            Task.FromResult()

    let cleanScenarios () =
        dep.Logger.Information("cleaning resources...")
        _scnSchedulers |> Array.iter(fun x -> Scenario.clean(x.Scenario, dep.NodeType, dep.Logger, _scnArgs.CustomSettings))
        dep.Logger.Information("resources cleaned")

    let startBombing (isWarmUp) = task {
        if isWarmUp then dep.Logger.Information("starting warm up...")
        else dep.Logger.Information("starting bombing...")

        _scnSchedulers <- createScenarioSchedulers(_targetScenarios)

        let scnsProgressSubscriptions = tryDisplayProgress(_scnSchedulers, isWarmUp)
        do! _scnSchedulers |> Array.map(fun x -> x.Start(isWarmUp)) |> Task.WhenAll
        scnsProgressSubscriptions |> Array.iter(fun x -> x.Dispose())
    }

    let startReportingTimer (bombingOperationTimer: Stopwatch) =
        if not (Array.isEmpty dep.ReportingSinks) then

            let timer = new System.Timers.Timer(_scnArgs.SendStatsInterval.TotalMilliseconds)
            timer.Elapsed.Add(fun _ ->

                match _currentOperation with
                | NodeOperationType.Bombing ->
                    let rawNodeStats = getNodeStats(bombingOperationTimer.Elapsed)
                    TestHostReporting.saveStats(_scnArgs.TestInfo, [|rawNodeStats|], dep.ReportingSinks)
                    |> ignore

                | _ -> ()
            )
            timer.Start()
            timer
        else
            new System.Timers.Timer()

    member x.TestInfo = _scnArgs.TestInfo
    member x.CurrentOperation = _currentOperation
    member x.CurrentNodeInfo = TestHost.createNodeInfo(dep, _currentOperation)
    member x.RegisteredScenarios = registeredScenarios
    member x.TargetScenarios = _targetScenarios

    member x.InitScenarios(args: TestSessionArgs) = task {
        _currentOperation <- NodeOperationType.Init
        _currentOperationTimer.Restart()
        do! Task.Yield()

        _scnArgs <- args
        match! initScenarios(args) with
        | Ok _ ->
            _currentOperationTimer.Stop()
            _currentOperation <- NodeOperationType.None
            return Ok()

        | Error e ->
            _currentOperationTimer.Stop()
            _currentOperation <- NodeOperationType.Stop
            return AppError.createResult(e)
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

        if _scnSchedulers |> Array.exists(fun x -> x.Working) then
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
        do! ScenarioValidation.validateWarmUpStats(warmUpStats)

        // bombing
        use reportingTimer = startReportingTimer(_currentOperationTimer)
        do! x.StartBombing()
        reportingTimer.Stop()

        let rawNodeStats = getNodeStats(_currentOperationTimer.Elapsed)
        return rawNodeStats
    }

    interface IDisposable with
        member x.Dispose() =
            if _currentOperation <> NodeOperationType.Complete then x.StopScenarios().Wait()
