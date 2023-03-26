namespace NBomber.DomainServices.TestHost

open System
open System.Threading
open System.Threading.Tasks
open System.Diagnostics
open System.Runtime.InteropServices

open Serilog
open Spectre.Console
open FsToolkit.ErrorHandling

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Contracts.Internal
open NBomber.Extensions.Internal
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.ScenarioContext
open NBomber.Domain.Stats
open NBomber.Domain.Stats.ScenarioStatsActor
open NBomber.Domain.Scheduler.ScenarioScheduler
open NBomber.Infra
open NBomber.DomainServices
open NBomber.DomainServices.TestHost.ReportingManager

type internal TestHost(dep: IGlobalDependency, regScenarios: Scenario list) as this =

    let mutable _currentSchedulers = List.empty<ScenarioScheduler>
    let mutable _stopped = false
    let mutable _disposed = false
    let mutable _targetScenarios = List.empty<Scenario>
    let mutable _sessionArgs = SessionArgs.empty
    let mutable _currentOperation = OperationType.None
    let mutable _nodeInfo = { NodeInfo.init None with NodeType = dep.NodeType; CurrentOperation = _currentOperation }
    let mutable _currentBombingTask = Task.FromResult()

    let getCurrentNodeInfo () =
        if _nodeInfo.CurrentOperation = _currentOperation then
            _nodeInfo
        else
            _nodeInfo <- { _nodeInfo with CurrentOperation = _currentOperation }
            _nodeInfo

    let createScenarioSchedulers (targetScenarios: Scenario list)
                                 (operation: ScenarioOperation)
                                 (getScenarioClusterCount: ScenarioName -> int)
                                 (createStatsActor: ILogger -> Scenario -> TimeSpan -> ScenarioStatsActor) =

        let createScheduler (scn: Scenario) =
            let scnDep = {
                Logger = dep.Logger
                Scenario = scn
                ScenarioCancellationToken = new CancellationTokenSource()
                ScenarioTimer = Stopwatch()
                ScenarioOperation = operation
                ScenarioStatsActor = createStatsActor dep.Logger scn (_sessionArgs.GetReportingInterval())
                ExecStopCommand = this.ExecStopCommand
                TestInfo = _sessionArgs.TestInfo
                GetNodeInfo = getCurrentNodeInfo
            }
            let count = getScenarioClusterCount scn.ScenarioName
            new ScenarioScheduler(scnDep, count)

        _currentSchedulers |> List.iter(fun x -> x.Stop() |> ignore)
        targetScenarios |> Scenario.getScenariosForBombing |> List.map createScheduler

    let stopSchedulers (schedulers: ScenarioScheduler list) =
        schedulers |> List.map(fun x -> x.Stop())
    
    let startScenarios (isWarmUp: bool)
                       (schedulers: ScenarioScheduler list)
                       (reportingManager: IReportingManager) = backgroundTask {
        
        WorkerPlugins.start dep
        ReportingSinks.start dep

        use consoleCancelToken = new CancellationTokenSource()
        let maxDuration = schedulers |> Seq.map(fun x -> x.Scenario) |> Scenario.getMaxDuration
        
        TestHostConsole.LiveStatusTable.display dep consoleCancelToken.Token isWarmUp schedulers
        reportingManager.Start()

        // waiting on all scenarios to finish        
        let bombingTask = schedulers |> Seq.map(fun x -> x.Start(consoleCancelToken.Token)) |> Task.WhenAll
        consoleCancelToken.CancelAfter maxDuration
        do! bombingTask
        consoleCancelToken.Cancel()

        // wait on final metrics and reporting tick
        do! Task.Delay Constants.ReportingTimerCompleteMs

        // waiting (in case of cluster) on all raw stats
        do! reportingManager.Stop()
        do! WorkerPlugins.stop dep
        do! ReportingSinks.stop dep

        if isWarmUp then
            GC.Collect()
            do! Task.Delay 1_000
    }

    let startInit (consoleStatus: StatusContext option) (sessionArgs: SessionArgs) = taskResult {

        let baseContext = NBomberContext.createBaseContext sessionArgs.TestInfo getCurrentNodeInfo dep.Logger

        do! WorkerPlugins.init dep baseContext
        do! ReportingSinks.init dep baseContext

        return! TestHostScenario.initScenarios dep consoleStatus baseContext sessionArgs regScenarios
    }

    let startClean (consoleStatus: StatusContext option)
                   (sessionArgs: SessionArgs)
                   (scenarios: Scenario list) =

        let baseContext = NBomberContext.createBaseContext sessionArgs.TestInfo getCurrentNodeInfo dep.Logger
        let enabledScenarios = scenarios |> List.filter(fun x -> x.IsEnabled)
        TestHostScenario.cleanScenarios dep consoleStatus baseContext sessionArgs enabledScenarios

    member _.SessionArgs = _sessionArgs
    member _.CurrentOperation = _currentOperation
    member _.CurrentNodeInfo = getCurrentNodeInfo()
    member _.RegisteredScenarios = regScenarios
    member _.TargetScenarios = _targetScenarios
    member _.CurrentSchedulers = _currentSchedulers

    member _.StartInit(sessionArgs: SessionArgs) =
        _stopped <- false
        _currentOperation <- OperationType.Init

        TestHostConsole.printContextInfo dep sessionArgs
        dep.LogInfo "Starting init..."

        TestHostConsole.displayStatus dep "Initializing scenarios..." (fun consoleStatus -> backgroundTask {
            let! initResult = startInit consoleStatus sessionArgs
            match initResult with
            | Ok initializedScenarios ->
                dep.LogInfo "Init finished"

                _targetScenarios  <- initializedScenarios
                _sessionArgs      <- sessionArgs
                _currentOperation <- OperationType.None

                return Ok _targetScenarios

            | Error _ ->
                _currentOperation <- OperationType.Error
                return initResult
        })

    member _.StartWarmUp(scenarios: Scenario list, schedulers: ScenarioScheduler list, reportingManager: IReportingManager) = backgroundTask {
        _stopped <- false
        _currentOperation <- OperationType.WarmUp
        _currentSchedulers <- schedulers

        dep.LogInfo "Starting warm up..."
        TestHostConsole.printWarmUpScenarios dep scenarios

        let isWarmUp = true
        _currentBombingTask <- startScenarios isWarmUp schedulers reportingManager
        do! _currentBombingTask
        do! Task.WhenAll(stopSchedulers schedulers)

        _currentOperation <- OperationType.None
    }

    member _.StartBombing(schedulers: ScenarioScheduler list, reportingManager: IReportingManager) = backgroundTask {
        _stopped <- false
        _currentOperation <- OperationType.Bombing
        _currentSchedulers <- schedulers

        dep.LogInfo "Starting bombing..."
        let isWarmUp = false
        _currentBombingTask <- startScenarios isWarmUp schedulers reportingManager
        do! _currentBombingTask
        do! this.StopTest()
        
        _currentOperation <- OperationType.Complete
    }

    abstract member ExecStopCommand: command:StopCommand -> unit
    default _.ExecStopCommand(command) =
        match command with
        | StopScenario (scenarioName, reason) ->
            this.StopScenario(scenarioName, reason)

        | StopTest reason ->            
            this.StopTest reason
            |> ignore

    member _.StopScenario(scenarioName, reason) =
        _currentSchedulers
        |> List.tryFind(fun sch -> sch.Working && sch.Scenario.ScenarioName = scenarioName)
        |> Option.iter(fun sch ->
            sch.Stop()
            dep.LogWarn("Stopping scenario early: {0}, reason: {1}", sch.Scenario.ScenarioName, reason)
        )        
    
    member _.StopTest([<Optional;DefaultParameterValue("":string)>]reason: string) = backgroundTask {
        if _currentOperation <> OperationType.Stop && not _stopped then
            _currentOperation <- OperationType.Stop            
            
            do! Task.WhenAll(stopSchedulers _currentSchedulers)
            
            if not(String.IsNullOrEmpty reason) then
                dep.LogWarn("Stopping test early: {0}", reason)
            else
                dep.LogInfo "Stopping scenarios..."
                        
            do! _currentBombingTask
            
            // we use Scenarios from Schedulers because scenario.ExecutedDuration will be available
            let finishedScenarios = _currentSchedulers |> List.map(fun x -> x.Scenario)
            let scenarios = Scenario.updatedExecutedDuration _targetScenarios finishedScenarios
            
            do! TestHostConsole.displayStatus dep "Cleaning scenarios..." (fun consoleStatus -> backgroundTask {
                do! startClean consoleStatus _sessionArgs scenarios

                _stopped <- true
                _currentOperation <- OperationType.None
            })
    }

    member _.CreateScenarioSchedulers(scenarios: Scenario list,
                                      operation: ScenarioOperation,
                                      ?getScenarioClusterCount: ScenarioName -> int,
                                      ?createStatsActor: ILogger -> Scenario -> TimeSpan -> ScenarioStatsActor) =

        let getScenarioClusterCount =
            getScenarioClusterCount
            |> Option.defaultValue Scenario.defaultClusterCount

        let createStatsActor =
            createStatsActor
            |> Option.defaultValue ScenarioStatsActor.createDefault

        createScenarioSchedulers scenarios operation getScenarioClusterCount createStatsActor

    member _.GetRealtimeStats(duration) =
        _currentSchedulers
        |> List.map(fun x ->
            if x.AllRealtimeStats.ContainsKey duration then
                Some x.AllRealtimeStats[duration]
            else
                None
        )
        |> Option.sequence

    member _.RunSession(sessionArgs: SessionArgs) = taskResult {
        let! initializedScenarios = this.StartInit sessionArgs

        // start warm up
        let warmupScenarios = Scenario.getScenariosForWarmUp initializedScenarios
        if warmupScenarios.Length > 0 then
            let warmupSchedulers = this.CreateScenarioSchedulers(warmupScenarios, ScenarioOperation.WarmUp)
            use reportingManager = ReportingManager.create dep warmupSchedulers sessionArgs
            do! this.StartWarmUp(warmupScenarios, warmupSchedulers, reportingManager)

        // start bombing
        let bombingSchedulers = this.CreateScenarioSchedulers(initializedScenarios, ScenarioOperation.Bombing)
        if bombingSchedulers.Length > 0 then
            use reportingManager = ReportingManager.create dep bombingSchedulers sessionArgs
            do! this.StartBombing(bombingSchedulers, reportingManager)

            // gets final stats
            dep.LogInfo "Calculating final statistics..."
            return! reportingManager.GetSessionResult(getCurrentNodeInfo())
        else
            let emptyResult = NodeSessionResult.empty            
            let finalStats = { emptyResult.FinalStats with TestInfo = sessionArgs.TestInfo }
            return { emptyResult with FinalStats = finalStats }
    }

    member _.Destroy() =
        if not _disposed then
            _disposed <- true
            this.StopTest().Wait()

            for sink in dep.ReportingSinks do
                use _ = sink
                ()

            for plugin in dep.WorkerPlugins do
                use _ = plugin
                ()

    interface IDisposable with
        member _.Dispose() = this.Destroy()
