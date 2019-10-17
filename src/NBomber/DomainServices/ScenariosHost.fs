module internal rec NBomber.DomainServices.ScenariosHost

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open FsToolkit.ErrorHandling

open NBomber.Extensions
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.Statistics
open NBomber.Errors
open NBomber.Infra.Dependency
open NBomber.DomainServices.Validation
open NBomber.DomainServices.ScenarioRunner

type ScenariosArgs = {
    SessionId: string
    ScenariosSettings: ScenarioSetting[]
    TargetScenarios: string[]
    CustomSettings: string    
} with
  
  static member empty = {
      SessionId = ""
      ScenariosSettings = Array.empty
      TargetScenarios = Array.empty
      CustomSettings = ""
  }
  
  static member getFromContext (sessionId, context: NBomberContext) =
      let scnSettings = NBomberContext.getScenariosSettings(context)
      let targetScns = NBomberContext.getTargetScenarios(context)
      let customSettings = NBomberContext.tryGetCustomSettings context    
      { SessionId = sessionId
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

let initScenarios (dep: Dependency) (customSettings: string) (scenarios: Scenario[]) = task {    
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

let warmUpScenarios (dep: Dependency, scnRunners: ScenarioRunner[]) =
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

let saveStats (nodeStats: RawNodeStats[], statsSink: IStatisticsSink) =
    nodeStats
    |> Array.map(Statistics.create >> statsSink.SaveStatistics >> Async.AwaitTask)
    |> Async.Parallel
    |> Async.StartAsTask    

let startSaveStatsTimer (dep: Dependency, duration: TimeSpan, getStatsAtTime: TimeSpan -> RawNodeStats) =    
    match dep.StatisticsSink with
    | Some statsSink ->
        let mutable executionTime = TimeSpan.Zero
        let timer = new System.Timers.Timer(Constants.GetStatsInterval - 1_000.0)
        timer.Elapsed.Add(fun _ ->
            // moving time forward
            executionTime <- executionTime.Add(TimeSpan.FromMilliseconds Constants.GetStatsInterval)
            
            if executionTime >= duration then timer.Stop() 
            else
                let rawNodeStats = getStatsAtTime(executionTime)
                saveStats([|rawNodeStats|], statsSink) |> ignore
        )
        timer.Start()
        timer
        
    | None -> new System.Timers.Timer()
    
let validateWarmUpStats (scnHost: ScenariosHost) =
    let rawNodeStats = scnHost.GetNodeStats()
    ScenarioValidation.validateWarmUpStats(rawNodeStats)

let mapToOperationType (scnHostStatus: ScenarioHostStatus) =
    match scnHostStatus with
    | ScenarioHostStatus.Working(WorkingOperation.WarmUpScenarios) ->
        OperationType.WarmUp
    
    | ScenarioHostStatus.Working(WorkingOperation.StartBombing) ->            
        OperationType.Bombing
    
    | _ -> OperationType.Complete
    
let getMaxDuration (allDurations: TimeSpan[]) =
    Array.max allDurations    

type ScenariosHost(dep: Dependency, registeredScenarios: Scenario[]) =
    
    let mutable _scnArgs = ScenariosArgs.empty  
    let mutable _status = ScenarioHostStatus.Ready
    let mutable _scnRunners = Array.empty<ScenarioRunner>
    
    let getStatsMeta () =
        { SessionId = dep.SessionId
          MachineName = dep.MachineInfo.MachineName
          Sender = dep.NodeType
          Operation = mapToOperationType(_status) }

    member x.SessionId = _scnArgs.SessionId
    member x.Status = _status
    member x.NodeInfo = getStatsMeta()
    member x.GetRegisteredScenarios() = registeredScenarios    
    member x.GetRunningScenarios() = _scnRunners |> Array.map(fun x -> x.Scenario)
    member x.GetScenarioMaxWarmUpDuration() = x.GetRunningScenarios() |> Array.map(fun x -> x.WarmUpDuration) |> getMaxDuration
    member x.GetScenarioMaxDuration() = x.GetRunningScenarios() |> Array.map(fun x -> x.Duration) |> getMaxDuration
    
    member x.InitScenarios(args: ScenariosArgs) = task {
        _scnArgs <- args
        _status <- ScenarioHostStatus.Working(InitScenarios)
        do! Task.Yield()            
        let! results = registeredScenarios
                       |> Scenario.applySettings args.ScenariosSettings
                       |> Scenario.filterTargetScenarios args.TargetScenarios
                       |> printTargetScenarios dep
                       |> initScenarios dep args.CustomSettings
        
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
        warmUpScenarios(dep, _scnRunners)                
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

    member x.GetNodeStats() =
        _scnRunners
        |> Array.map(fun x -> x.GetScenarioStats())
        |> NodeStats.create(getStatsMeta())
    
    member x.GetNodeStats(duration) =
        _scnRunners
        |> Array.map(fun x -> x.GetScenarioStats(duration))
        |> NodeStats.create(getStatsMeta())
        
    member x.RunSession(args: ScenariosArgs) = asyncResult {
        
        // init
        do! x.InitScenarios(args)        
        
        // warm-up
        let timerDuration = x.GetScenarioMaxWarmUpDuration()
        use warmUpStatsTimer = startSaveStatsTimer(dep, timerDuration, fun executionTime -> x.GetNodeStats(executionTime))
        do! x.WarmUpScenarios()        
        do! validateWarmUpStats(x)
        warmUpStatsTimer.Stop()
    
        // bombing
        let timerDuration = x.GetScenarioMaxWarmUpDuration()
        use bombingStatsTimer = startSaveStatsTimer(dep, timerDuration, fun executionTime -> x.GetNodeStats(executionTime))
        do! x.StartBombing()
        bombingStatsTimer.Stop()

        // saving final stats results
        let rawNodeStats = x.GetNodeStats()
        if dep.StatisticsSink.IsSome then
            do! saveStats([|rawNodeStats|], dep.StatisticsSink.Value)
        
        return rawNodeStats
    }
    
    interface IDisposable with
        member x.Dispose() =
            if _status <> ScenarioHostStatus.Ready then x.StopScenarios()