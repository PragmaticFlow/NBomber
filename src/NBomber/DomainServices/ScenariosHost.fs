module internal rec NBomber.DomainServices.ScenariosHost

open System
open System.Threading.Tasks

open Serilog
open FSharp.Control.Tasks.V2.ContextInsensitive
open FsToolkit.ErrorHandling

open NBomber.Extensions
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.Statistics
open NBomber.Errors
open NBomber.Infra.Dependency
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

type WorkingState =
    | InitScenarios
    | WarmUpScenarios
    | StartBombing
    | StopScenarios

type ScenarioHostStatus =    
    | Ready
    | Working of WorkingState    
    | Stopped of reason:string

let displayProgress (dep: Dependency, scnRunners: ScenarioRunner[]) =
    let runnerWithLongestScenario = scnRunners
                                    |> Array.sortByDescending(fun x -> x.Scenario.Duration)
                                    |> Array.tryHead

    match runnerWithLongestScenario with
    | Some runner ->
        if scnRunners.Length > 1 then
            Log.Information("waiting time: duration '{0}' of the longest scenario '{1}'", runner.Scenario.Duration, runner.Scenario.ScenarioName)
        else
            Log.Information("waiting time: duration '{0}'", runner.Scenario.Duration)

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
            ConnectionPool.init(scenario, onStartedInitPool, onConnectionOpened, onFinishInitPool)
    else
        fun scenario ->
            ConnectionPool.init(scenario, ignore, ignore, ignore)

let initScenarios (dep: Dependency) (customSettings: string) (scenarios: Scenario[]) = task {    
    let mutable failed = false    
    let mutable error = Unchecked.defaultof<_>
    
    let flow = seq {
        for scn in scenarios do 
            if not failed then
                Log.Information("initializing scenario: '{0}', concurrent copies: '{1}'", scn.ScenarioName, scn.ConcurrentCopies)                

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
    |> Array.iter(fun x -> Log.Information("warming up scenario: '{0}', duration: '{1}'", x.Scenario.ScenarioName, x.Scenario.WarmUpDuration)
                           let duration = x.Scenario.WarmUpDuration
                           if dep.ApplicationType = ApplicationType.Console then dep.ShowProgressBar(duration)
                           x.WarmUp().Wait())    

let runBombing (dep: Dependency, scnRunners: ScenarioRunner[]) =
    Log.Information("starting bombing...")    
    if dep.ApplicationType = ApplicationType.Console then displayProgress(dep, scnRunners)
    scnRunners |> Array.map(fun x -> x.Run()) |> Task.WhenAll

let stopAndCleanScenarios (dep: Dependency, scnRunners: ScenarioRunner[]) =
    Log.Information("stopping bombing and cleaning resources...")
    scnRunners |> Array.iter(fun x -> x.Stop().Wait())    
    scnRunners |> Array.iter(fun x -> Scenario.clean(x.Scenario, dep.NodeType))
    Log.Information("bombing stoped and resources cleaned")

let printTargetScenarios (scenarios: Scenario[]) = 
    scenarios
    |> Array.map(fun x -> x.ScenarioName)
    |> fun targets -> Log.Information("target scenarios: {0}", String.concatWithCommaAndQuotes(targets))
    |> fun _ -> scenarios

let saveStats (nodeStats: RawNodeStats[], statsSink: IStatisticsSink) =
    nodeStats
    |> Array.map(Statistics.create >> statsSink.SaveStatistics >> Async.AwaitTask)
    |> Async.Parallel
    |> Async.StartAsTask    

let startSaveStatsTimer (dep: Dependency, getStatsAtTime: TimeSpan -> RawNodeStats) =    
    match dep.StatisticsSink with
    | Some statsSink ->
        let mutable executionTime = TimeSpan.Zero
        let timer = new System.Timers.Timer(Constants.GetStatsInterval)
        timer.Elapsed.Add(fun _ ->
            // moving time forward
            executionTime <- executionTime.Add(TimeSpan.FromMilliseconds Constants.GetStatsInterval)
            let rawNodeStats = getStatsAtTime(executionTime)
            saveStats([|rawNodeStats|], statsSink) |> ignore
        )
        timer.Start()
        timer
        
    | None -> new System.Timers.Timer()    

type ScenariosHost(dep: Dependency, registeredScenarios: Scenario[]) =
    
    let mutable _scnArgs = ScenariosArgs.empty  
    let mutable _status = ScenarioHostStatus.Ready
    let mutable _scnRunners = Array.empty<ScenarioRunner>
    
    let _statsMeta = { SessionId = dep.SessionId
                       MachineName = dep.MachineInfo.MachineName
                       Sender = dep.NodeType }

    member x.SessionId = _scnArgs.SessionId
    member x.Status = _status
    member x.NodeInfo = _statsMeta
    member x.GetRegisteredScenarios() = registeredScenarios    
    member x.GetRunningScenarios() = _scnRunners |> Array.map(fun x -> x.Scenario)
    
    member x.InitScenarios(args: ScenariosArgs) = task {
        _scnArgs <- args
        _status <- ScenarioHostStatus.Working(InitScenarios)
        do! Task.Yield()            
        let! results = registeredScenarios
                       |> Scenario.applySettings(args.ScenariosSettings)
                       |> Scenario.filterTargetScenarios(args.TargetScenarios)
                       |> printTargetScenarios
                       |> initScenarios dep args.CustomSettings
        
        match results with
        | Ok scns -> _scnRunners <- scns |> Array.map(ScenarioRunner)
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
        |> NodeStats.create(_statsMeta)
    
    member x.GetNodeStats(duration) =
        _scnRunners
        |> Array.map(fun x -> x.GetScenarioStats(duration))
        |> NodeStats.create(_statsMeta)
        
    member x.RunSession(args: ScenariosArgs) = asyncResult {
        
        do! x.InitScenarios(args)
        do! x.WarmUpScenarios()                     
        let bombingTask = x.StartBombing()
        
        use saveStatsTimer = startSaveStatsTimer(dep, fun executionTime -> x.GetNodeStats(executionTime))
        
        do! bombingTask
        saveStatsTimer.Stop()

        // saving final stats results
        let rawNodeStats = x.GetNodeStats()
        if dep.StatisticsSink.IsSome then
            do! saveStats([|rawNodeStats|], dep.StatisticsSink.Value)
        
        return rawNodeStats
    }
    
    interface IDisposable with
        member x.Dispose() =
            if _status <> ScenarioHostStatus.Ready then x.StopScenarios()