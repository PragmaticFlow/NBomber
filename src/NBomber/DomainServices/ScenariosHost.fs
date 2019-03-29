module internal NBomber.DomainServices.ScenariosHost

open System.Threading.Tasks

open Serilog
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Configuration
open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.Statistics
open NBomber.Infra.Dependency
open NBomber.DomainServices.ScenarioRunner

type IScenariosHost =
    abstract GetRegisteredScenarios: unit -> Scenario[]
    abstract IsWorking: unit -> Result<bool,DomainError>        
    abstract InitScenarios: ScenarioSetting[] * targetScenarios:string[] -> Task<Result<unit,DomainError>>
    abstract WarmUpScenarios: unit -> Task<unit>
    abstract RunBombing: unit -> Task<unit>
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

let initScenarios (scenarios: Scenario[]) = task {    
    let mutable failed = false    
    let mutable error = Unchecked.defaultof<_>
    
    let flow = seq {
        for scn in scenarios do 
            if not failed then
                Log.Information("initializing scenario: '{0}'", scn.ScenarioName)
                let initResult = Scenario.init(scn)
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
    |> Array.iter(fun x -> Log.Information("warming up scenario: '{0}'", x.Scenario.ScenarioName)
                           let duration = x.Scenario.WarmUpDuration
                           if dep.ApplicationType = ApplicationType.Console then dep.ShowProgressBar(duration)
                           x.WarmUp().Wait())    

let runBombing (dep: Dependency, scnRunners: ScenarioRunner[]) =
    Log.Information("starting bombing...")    
    if dep.ApplicationType = ApplicationType.Console then displayProgress(dep, scnRunners)
    scnRunners |> Array.map(fun x -> x.Run()) |> Task.WhenAll

let stopAndCleanScenarios (scnRunners: ScenarioRunner[]) =
    scnRunners |> Array.iter(fun x -> x.Stop().Wait())    
    scnRunners |> Array.iter(fun x -> Scenario.clean(x.Scenario))

let getResults (meta: StatisticsMeta, scnRunners: ScenarioRunner[]) =
    scnRunners
    |> Array.map(fun x -> x.GetResult())
    |> NodeStats.create(meta)

type ScenariosHost(dep: Dependency, registeredScenarios: Scenario[]) =
    
    let mutable scnRunners = None
    let mutable isWorking = Ok false
    let statsMeta = { SessionId = dep.SessionId; NodeName = dep.NodeInfo.NodeName; Sender = dep.NodeType }
    let startedWork () = isWorking <- Ok true
    let stoppedWork () = isWorking <- Ok false
    let failed (error) = isWorking <- Error error

    interface IScenariosHost with
        member x.GetRegisteredScenarios() = registeredScenarios
        member x.IsWorking() = isWorking        

        member x.InitScenarios(settings, targetScenarios) = task {
            startedWork()
            let! results = registeredScenarios
                           |> ScenarioBuilder.applyScenariosSettings(settings)
                           |> ScenarioBuilder.filterTargetScenarios(targetScenarios)
                           |> initScenarios
            
            match results with
            | Ok scns -> stoppedWork()
                         scnRunners <- scns |> Array.map(ScenarioRunner) |> Some
                         return Ok() 
            
            | Error e -> failed(e)
                         return Error e
        }

        member x.WarmUpScenarios() = task {
            do! Task.Delay(10)
            if scnRunners.IsSome then
                startedWork()
                warmUpScenarios(dep, scnRunners.Value)
                stoppedWork()       
        }

        member x.RunBombing() = task {
            if scnRunners.IsSome then
                startedWork()
                let! tasks = runBombing(dep, scnRunners.Value) 
                stoppedWork()                
        }

        member x.StopScenarios() = scnRunners |> Option.map(stopAndCleanScenarios) |> ignore

        member x.GetStatistics() =            
            match scnRunners with
            | Some v -> getResults(statsMeta, v)
            | None   -> NodeStats.create statsMeta Array.empty

let create (dep: Dependency, registeredScns: Scenario[]) =
    ScenariosHost(dep, registeredScns) :> IScenariosHost

let run (scnSettings: ScenarioSetting[], targetScns: ScenarioName[])
        (scnHost: IScenariosHost) = trial {    
    
    do! scnHost.InitScenarios(scnSettings, targetScns).Result
    scnHost.WarmUpScenarios().Wait()
    scnHost.RunBombing().Wait()
    scnHost.StopScenarios()
    return scnHost.GetStatistics()
}