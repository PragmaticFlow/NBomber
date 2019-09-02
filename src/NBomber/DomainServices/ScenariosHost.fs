module internal NBomber.DomainServices.ScenariosHost

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

type WorkingState =
    | InitScenarios
    | WarmUpScenarios
    | StartBombing
    | StopScenarios

type ScenarioHostStatus =    
    | Ready
    | Working of WorkingState
    | Stopped of reason:string

type IScenariosHost =
    abstract SessionId: string
    abstract Status: ScenarioHostStatus
    abstract GetRegisteredScenarios: unit -> Scenario[]
    abstract GetRunningScenarios: unit -> Scenario[]
    abstract InitScenarios: sessionId:string * ScenarioSetting[] * targetScenarios:string[] * customSettings:string -> Task<Result<unit,AppError>>
    abstract WarmUpScenarios: unit -> Task<unit>
    abstract StartBombing: unit -> Task<unit>
    abstract StopScenarios: unit -> unit
    abstract GetStatistics: unit -> NodeStats

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

let stopAndCleanScenarios (dep: Dependency) (scnRunners: ScenarioRunner[]) =
    Log.Information("stopping bombing and cleaning resources...")
    scnRunners |> Array.iter(fun x -> x.Stop().Wait())    
    scnRunners |> Array.iter(fun x -> Scenario.clean(x.Scenario, dep.NodeType))
    Log.Information("bombing stoped and resources cleaned")

let getResults (meta: StatisticsMeta, scnRunners: ScenarioRunner[]) =
    scnRunners
    |> Array.map(fun x -> x.GetResult())
    |> NodeStats.create(meta)

let printTargetScenarios (scenarios: Scenario[]) = 
    scenarios
    |> Array.map(fun x -> x.ScenarioName)
    |> fun targets -> Log.Information("target scenarios: {0}", String.concatWithCommaAndQuotes(targets))
    |> fun _ -> scenarios

type ScenariosHost(dep: Dependency, registeredScenarios: Scenario[]) =
    
    let mutable _sessionId = ""
    let mutable _status = ScenarioHostStatus.Ready
    let mutable scnRunners: ScenarioRunner[] option = None
    
    let statsMeta = { SessionId = dep.SessionId; MachineName = dep.MachineInfo.MachineName; Sender = dep.NodeType }    

    interface IScenariosHost with
        member x.SessionId = _sessionId
        member x.Status = _status
        member x.GetRegisteredScenarios() = registeredScenarios
        member x.GetRunningScenarios() =
            match scnRunners with
            | Some runners -> runners |> Array.map(fun x -> x.Scenario)
            | None         -> Array.empty
        
        member x.InitScenarios(sessionId, settings, targetScenarios, customSettings) = task {            
            _sessionId <- sessionId
            _status <- ScenarioHostStatus.Working(InitScenarios)
            do! Task.Yield()            
            let! results = registeredScenarios
                           |> Scenario.applySettings(settings)
                           |> Scenario.filterTargetScenarios(targetScenarios)
                           |> printTargetScenarios
                           |> initScenarios dep customSettings
            
            match results with
            | Ok scns -> scnRunners <- scns |> Array.map(ScenarioRunner) |> Some
                         _status <- ScenarioHostStatus.Ready
                         return Ok() 
            
            | Error e -> _status <- ScenarioHostStatus.Stopped(AppError.toString e)
                         return AppError.createResult(e)
        }

        member x.WarmUpScenarios() = task {             
            if scnRunners.IsSome then
                _status <- ScenarioHostStatus.Working(WarmUpScenarios)
                do! Task.Yield()
                warmUpScenarios(dep, scnRunners.Value)                
                _status <- ScenarioHostStatus.Ready
                do! Task.Delay(1000)
        }

        member x.StartBombing() = task {              
            if scnRunners.IsSome then
                _status <- ScenarioHostStatus.Working(StartBombing)
                do! Task.Yield()
                let! tasks = runBombing(dep, scnRunners.Value)
                scnRunners |> Option.map(stopAndCleanScenarios dep) |> ignore                
                _status <- ScenarioHostStatus.Ready
                do! Task.Delay(1000)
        }

        member x.StopScenarios() = 
            _status <- ScenarioHostStatus.Working(StopScenarios)
            scnRunners |> Option.map(stopAndCleanScenarios dep) |> ignore
            _status <- ScenarioHostStatus.Ready

        member x.GetStatistics() =            
            match scnRunners with
            | Some v -> getResults(statsMeta, v)
            | None   -> NodeStats.create statsMeta Array.empty

let create (dep: Dependency, registeredScns: Scenario[]) =
    ScenariosHost(dep, registeredScns) :> IScenariosHost

let run (sessionId) (scnSettings: ScenarioSetting[]) (targetScns: ScenarioName[])
        (customSettings: string)
        (scnHost: IScenariosHost) = asyncResult {    
    
    do! scnHost.InitScenarios(sessionId, scnSettings, targetScns, customSettings)
    do! scnHost.WarmUpScenarios()
    do! scnHost.StartBombing()    
    return scnHost.GetStatistics()
}