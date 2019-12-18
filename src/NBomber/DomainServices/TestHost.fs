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
open NBomber.Infra.Dependency
open NBomber.DomainServices.Validation
open NBomber.DomainServices.ScenarioRunner

type TestSessionArgs = {
    TestInfo: TestInfo
    ScenariosSettings: ScenarioSetting[]
    TargetScenarios: string[]
    CustomSettings: string    
} with  
  static member empty = {
      TestInfo = { SessionId = ""; TestSuite = ""; TestName = "" }
      ScenariosSettings = Array.empty
      TargetScenarios = Array.empty
      CustomSettings = ""
  }
  
  static member getFromContext (testInfo: TestInfo, context: NBomberTestContext) =
      let scnSettings = NBomberTestContext.getScenariosSettings(context)
      let targetScns = NBomberTestContext.getTargetScenarios(context)
      let customSettings = NBomberTestContext.tryGetCustomSettings(context)
      { TestInfo = testInfo
        ScenariosSettings = scnSettings
        TargetScenarios = targetScns
        CustomSettings = customSettings }

type WorkingOperation =
    | InitScenarios
    | WarmUpScenarios
    | StartBombing
    | StopScenarios

type ScenarioHostStatus =    
    | Ready
    | Working of WorkingOperation    
    | Stopped of reason:string

let displayProgress (dep: Dependency, scnRunners: ScenarioRunner[]) =
    let runnerWithLongestScenario = scnRunners
                                    |> Array.sortByDescending(fun x -> x.Scenario.Duration)
                                    |> Array.tryHead

    match runnerWithLongestScenario with
    | Some runner ->
        if scnRunners.Length > 1 then
            dep.Logger.Information("waiting time: duration '{0}' of the longest scenario '{1}'", runner.Scenario.Duration, runner.Scenario.ScenarioName)
        else
            dep.Logger.Information("waiting time: duration '{0}'", runner.Scenario.Duration)

        dep.ShowProgressBar(runner.Scenario.Duration)

    | None -> ()

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
                let initResult = Scenario.init(scn, initAllConnectionPools, customSettings, dep.NodeType)
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
    |> Array.iter(fun x -> dep.Logger.Information("warming up scenario: '{0}', duration: '{1}'", x.Scenario.ScenarioName, x.Scenario.WarmUpDuration)
                           let duration = x.Scenario.WarmUpDuration
                           if dep.ApplicationType = ApplicationType.Console then dep.ShowProgressBar(duration)
                           x.WarmUp().Wait())    

let runBombing (dep: Dependency, scnRunners: ScenarioRunner[]) =
    dep.Logger.Information("starting bombing...")    
    if dep.ApplicationType = ApplicationType.Console then displayProgress(dep, scnRunners)
    scnRunners |> Array.map(fun x -> x.Run()) |> Task.WhenAll

let stopAndCleanScenarios (dep: Dependency, scnRunners: ScenarioRunner[]) =
    dep.Logger.Information("stopping bombing and cleaning resources...")
    scnRunners |> Array.iter(fun x -> x.Stop().Wait())    
    scnRunners |> Array.iter(fun x -> Scenario.clean(x.Scenario, dep.NodeType, dep.Logger))
    dep.Logger.Information("bombing stoped and resources cleaned")

let printTargetScenarios (dep: Dependency) (scenarios: Scenario[]) = 
    scenarios
    |> Array.map(fun x -> x.ScenarioName)
    |> fun targets -> dep.Logger.Information("target scenarios: {0}", String.concatWithCommaAndQuotes(targets))
    |> fun _ -> scenarios
    
let mapToOperationType (scnHostStatus: ScenarioHostStatus) =
    match scnHostStatus with
    | ScenarioHostStatus.Working(WorkingOperation.WarmUpScenarios) ->
        OperationType.WarmUp
    
    | ScenarioHostStatus.Working(WorkingOperation.StartBombing) ->            
        OperationType.Bombing
        
    | _ -> OperationType.Complete    

module TestHostStats =    
        
    let getNodeStatsMeta (dep: Dependency, status: ScenarioHostStatus) =
        { MachineName = dep.MachineInfo.MachineName
          Sender = dep.NodeType
          Operation = mapToOperationType(status) }
        
    let saveStats (testInfo: TestInfo, nodeStats: RawNodeStats[], sinks: IReportingSink[]) =
        nodeStats
        |> Array.collect(fun x ->
            let stats = Statistics.create(x)
            sinks |> Array.map(fun snk -> snk.SaveStatistics(testInfo, stats))
        )
        |> Task.WhenAll
        
    let startSaveStatsTimer (sinks: IReportingSink[], testInfo: TestInfo, getNodeStats: TimeSpan -> RawNodeStats) =        
        if not (Array.isEmpty sinks) then
            let mutable executionTime = TimeSpan.Zero
            let timer = new System.Timers.Timer(Constants.GetStatsInterval)
            timer.Elapsed.Add(fun _ ->
                // moving time forward
                executionTime <- executionTime.Add(TimeSpan.FromMilliseconds Constants.GetStatsInterval)
                let rawNodeStats = getNodeStats(executionTime)
                saveStats(testInfo, [|rawNodeStats|], sinks)
                |> ignore
            )
            timer.Start()
            timer            
        else
            new System.Timers.Timer()        

type TestHost(dep: Dependency, registeredScenarios: Scenario[]) =
    
    let mutable _scnArgs = TestSessionArgs.empty  
    let mutable _status = ScenarioHostStatus.Ready
    let mutable _scnRunners = Array.empty<ScenarioRunner>    
    
    let getNodeStats (duration: TimeSpan option) =
        _scnRunners
        |> Array.map(fun x -> x.GetScenarioStats duration)
        |> NodeStats.create(TestHostStats.getNodeStatsMeta(dep, _status))

    member x.TestInfo = _scnArgs.TestInfo
    member x.Status = _status
    member x.NodeStatsMeta = TestHostStats.getNodeStatsMeta(dep, _status)
    member x.GetRegisteredScenarios() = registeredScenarios    
    member x.GetRunningScenarios() = _scnRunners |> Array.map(fun x -> x.Scenario)    
    
    member x.InitScenarios(args: TestSessionArgs) = task {
        _scnArgs <- args
        _status <- ScenarioHostStatus.Working(InitScenarios)
        do! Task.Yield()            
        let! results = registeredScenarios
                       |> Scenario.applySettings args.ScenariosSettings
                       |> Scenario.filterTargetScenarios args.TargetScenarios
                       |> printTargetScenarios dep
                       |> runInitScenarios dep args.CustomSettings
        
        match results with
        | Ok scns -> _scnRunners <- scns |> Array.map(fun x -> ScenarioRunner(x, dep.Logger))
                     _status <- ScenarioHostStatus.Ready
                     return Ok()
        
        | Error e -> _status <- ScenarioHostStatus.Stopped(AppError.toString e)
                     return AppError.createResult(e)
    }

    member x.WarmUpScenarios() = task {
        _status <- ScenarioHostStatus.Working(WarmUpScenarios)
        do! Task.Yield()
        runWarmUpScenarios(dep, _scnRunners)                
        _status <- ScenarioHostStatus.Ready
        do! Task.Delay(1_000)
    }

    member x.StartBombing() = task {
        _status <- ScenarioHostStatus.Working(StartBombing)
        do! Task.Yield()
        let! tasks = runBombing(dep, _scnRunners)
        x.StopScenarios()
        _status <- ScenarioHostStatus.Ready
        do! Task.Delay(1000)
    }

    member x.StopScenarios() = 
        _status <- ScenarioHostStatus.Working(StopScenarios)
        stopAndCleanScenarios(dep, _scnRunners)
        _status <- ScenarioHostStatus.Ready

    member x.GetNodeStats(duration) = getNodeStats(duration)
    
    member x.RunSession(args: TestSessionArgs) = asyncResult {
        
        // init
        do! x.InitScenarios(args)        
        
        // warm-up
        use warmUpStatsTimer = TestHostStats.startSaveStatsTimer(dep.ReportingSinks,
                                                                 x.TestInfo,
                                                                 fun duration -> x.GetNodeStats(Some duration))
        do! x.WarmUpScenarios()
        warmUpStatsTimer.Stop()
        do! ScenarioValidation.validateWarmUpStats(x.GetNodeStats(None))        
    
        // bombing        
        use bombingStatsTimer = TestHostStats.startSaveStatsTimer(dep.ReportingSinks,
                                                                  x.TestInfo,
                                                                  fun duration -> x.GetNodeStats(Some duration))
        do! x.StartBombing()
        bombingStatsTimer.Stop()

        // saving final stats results
        let rawNodeStats = x.GetNodeStats(None)
        do! TestHostStats.saveStats(x.TestInfo, [|rawNodeStats|], dep.ReportingSinks)
        
        return rawNodeStats
    }
    
    interface IDisposable with
        member x.Dispose() =
            if _status <> ScenarioHostStatus.Ready then x.StopScenarios()