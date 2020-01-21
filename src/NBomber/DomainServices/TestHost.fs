module internal rec NBomber.DomainServices.TestHost

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open FsToolkit.ErrorHandling

open NBomber.Configuration
open NBomber.Contracts
open NBomber.Extensions
open NBomber.Domain
open NBomber.Domain.Statistics
open NBomber.Errors
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices.Validation
open NBomber.DomainServices.ScenarioRunner

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

let displayProgress (dep: Dependency, scnRunners: ScenarioRunner[]) =
    let runner = scnRunners |> Array.sortByDescending(fun x -> x.Scenario.Duration) |> Array.head
    if scnRunners.Length > 1 then
        dep.Logger.Information("waiting time: duration '{0}' of the longest scenario '{1}'", runner.Scenario.Duration, runner.Scenario.ScenarioName)
    else
        dep.Logger.Information("waiting time: duration '{0}'", runner.Scenario.Duration)

    dep.ShowProgressBar(runner.Scenario.Duration)

let buildInitConnectionPools (dep: Dependency) =
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

let runInitScenarios (dep: Dependency) (customSettings: string) (scenarios: Scenario[]) = task {
    let mutable failed = false
    let mutable error = Unchecked.defaultof<_>

    let flow = seq {
        for scn in scenarios do
            if not failed then
                dep.Logger.Information("initializing scenario: '{0}', concurrent copies: '{1}'", scn.ScenarioName, scn.ConcurrentCopies)

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

let runWarmUpScenarios (dep: Dependency, scnRunners: ScenarioRunner[]) =
    scnRunners
    |> Array.filter(fun x -> x.Scenario.WarmUpDuration.Ticks > 0L)
    |> Array.iter(fun x ->
        dep.Logger.Information("warming up scenario: '{0}', duration: '{1}'", x.Scenario.ScenarioName, x.Scenario.WarmUpDuration)
        let warmupTask = x.WarmUp()
        if dep.ApplicationType = ApplicationType.Console then
            use pb = dep.ShowProgressBar(x.Scenario.WarmUpDuration)
            warmupTask.Wait()
        else
            warmupTask.Wait()
    )

let runBombing (dep: Dependency, scnRunners: ScenarioRunner[]) =
    dep.Logger.Information("starting bombing...")
    let bombingTask = scnRunners |> Array.map(fun x -> x.Run()) |> Task.WhenAll
    if dep.ApplicationType = ApplicationType.Console then
        use pb = displayProgress(dep, scnRunners)
        bombingTask.Wait()
    else
        bombingTask.Wait()

let stopAndCleanScenarios (dep: Dependency, scnRunners: ScenarioRunner[], customSettings: string) =
    dep.Logger.Information("stopping bombing and cleaning resources...")
    scnRunners |> Array.iter(fun x -> x.Stop().Wait())
    scnRunners |> Array.iter(fun x -> Scenario.clean(x.Scenario, dep.NodeType, dep.Logger, customSettings))
    dep.Logger.Information("bombing stoped and resources cleaned")

let printTargetScenarios (dep: Dependency) (scenarios: Scenario[]) =
    scenarios
    |> Array.map(fun x -> x.ScenarioName)
    |> fun targets -> dep.Logger.Information("target scenarios: {0}", String.concatWithCommaAndQuotes(targets))
    |> fun _ -> scenarios

let createNodeInfo (dep: Dependency, currentOperation: NodeOperationType) =
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

    let startRealtimeTimer (sinks: IReportingSink[],
                            timerInterval: TimeSpan,
                            testInfo: TestInfo,
                            getCurrentOperation: unit -> NodeOperationType,
                            getNodeStats: TimeSpan -> RawNodeStats) =
        if not (Array.isEmpty sinks) then
            let mutable executionTime = TimeSpan.Zero
            let timer = new System.Timers.Timer(timerInterval.TotalMilliseconds)
            timer.Elapsed.Add(fun _ ->
                // moving time forward
                executionTime <- executionTime.Add(timerInterval)
                match getCurrentOperation() with
                | NodeOperationType.WarmUp
                | NodeOperationType.Bombing ->
                    let rawNodeStats = getNodeStats(executionTime)
                    saveStats(testInfo, [|rawNodeStats|], sinks)
                    |> ignore

                | _ -> ()
            )
            timer.Start()
            timer
        else
            new System.Timers.Timer()

type TestHost(dep: Dependency, registeredScenarios: Scenario[]) =

    let mutable _scnArgs = TestSessionArgs.empty
    let mutable _currentOperation = NodeOperationType.None
    let mutable _scnRunners = Array.empty<ScenarioRunner>

    let getNodeStats (duration: TimeSpan option) =
        _scnRunners
        |> Array.map(fun x -> x.GetScenarioStats duration)
        |> NodeStats.create(TestHost.createNodeInfo(dep, _currentOperation))

    member x.TestInfo = _scnArgs.TestInfo
    member x.CurrentOperation = _currentOperation
    member x.CurrentNodeInfo = TestHost.createNodeInfo(dep, _currentOperation)
    member x.GetRegisteredScenarios() = registeredScenarios
    member x.GetRunningScenarios() = _scnRunners |> Array.map(fun x -> x.Scenario)

    member x.InitScenarios(args: TestSessionArgs) = task {
        _scnArgs <- args
        _currentOperation <- NodeOperationType.Init
        do! Task.Yield()
        let! results = registeredScenarios
                       |> Scenario.applySettings args.ScenariosSettings
                       |> Scenario.filterTargetScenarios args.TargetScenarios
                       |> printTargetScenarios dep
                       |> runInitScenarios dep args.CustomSettings

        match results with
        | Ok scns -> _scnRunners <- scns |> Array.map(fun x -> ScenarioRunner(x, dep.Logger))
                     _currentOperation <- NodeOperationType.None
                     return Ok()

        | Error e -> _currentOperation <- NodeOperationType.Stop
                     return AppError.createResult(e)
    }

    member x.WarmUpScenarios() = task {
        _currentOperation <- NodeOperationType.WarmUp
        do! Task.Yield()
        runWarmUpScenarios(dep, _scnRunners)
        _currentOperation <- NodeOperationType.None
        do! Task.Delay(1_000)
    }

    member x.StartBombing() = task {
        _currentOperation <- NodeOperationType.Bombing
        do! Task.Yield()
        runBombing(dep, _scnRunners)
        x.StopScenarios()
        _currentOperation <- NodeOperationType.Complete
        do! Task.Delay(1000)
    }

    member x.StopScenarios() =
        _currentOperation <- NodeOperationType.Stop
        stopAndCleanScenarios(dep, _scnRunners, _scnArgs.CustomSettings)
        _currentOperation <- NodeOperationType.None

    member x.GetNodeStats(duration) = getNodeStats(duration)

    member x.RunSession(args: TestSessionArgs) = asyncResult {

        let startRealtimeTimer () =
            TestHostReporting.startRealtimeTimer(
                dep.ReportingSinks,
                args.SendStatsInterval,
                x.TestInfo,
                (fun () -> _currentOperation),
                (fun duration -> x.GetNodeStats(Some duration))
            )

        // init
        do! x.InitScenarios(args)

        // warm-up
        do! x.WarmUpScenarios()
        do! ScenarioValidation.validateWarmUpStats(x.GetNodeStats(None))

        // bombing
        use bombingReportingTimer = startRealtimeTimer()
        do! x.StartBombing()
        bombingReportingTimer.Stop()

        let rawNodeStats = x.GetNodeStats(None)
        return rawNodeStats
    }

    interface IDisposable with
        member x.Dispose() =
            if _currentOperation <> NodeOperationType.Complete then x.StopScenarios()
