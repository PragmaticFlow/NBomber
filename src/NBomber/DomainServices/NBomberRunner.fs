module internal NBomber.DomainServices.NBomberRunner

open System
open System.Threading.Tasks

open Serilog

open NBomber.Contracts
open NBomber.Configuration
open NBomber.Domain
open NBomber.Domain.Statistics
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices.Reporting
open NBomber.DomainServices.ScenarioRunner

let tryGetScenariosSettings (context: NBomberRunnerContext) = maybe {
    let! config = context.NBomberConfig
    let! globalSettings = config.GlobalSettings
    return! Option.ofObj(globalSettings.ScenariosSettings)
}

let applyScenariosSettings (scenarios: Scenario[], settings: ScenarioSetting[]) = 
    scenarios

let warmUpScenarios (scnRunners: ScenarioRunner[]) =
    let results = 
        scnRunners 
        |> Array.map(fun x -> Log.Information("warming up scenario: '{0}'", x.Scenario.ScenarioName)
                              x.RunInit())
                                        
    let allOk = results |> Array.forall(Result.isOk)
    if not allOk then
        results |> Errors.getErrorsString |> Log.Error
        false
    else
        true

let buildScenarios (context: NBomberRunnerContext) =    
    match tryGetScenariosSettings(context) with
    | Some settings -> applyScenariosSettings(context.Scenarios, settings)
                       |> Array.map(Scenario.create)
    
    | None          -> context.Scenarios
                       |> Array.map(Scenario.create)

let displayProgress (scnRunners: ScenarioRunner[]) =
    let longestDuration = scnRunners
                          |> Array.map(fun x -> x.Scenario.Duration)
                          |> Array.sort
                          |> Array.tryHead
                              
    match longestDuration with
    | Some v -> Dependency.ProgressBar.show(v)
    | None   -> Task.FromResult()

let waitUnitilAllFinish (scnRunners: ScenarioRunner[]) = 
    let mutable allFinish = scnRunners |> Array.forall(fun x -> x.Finished)
    while not allFinish do        
        Task.Delay(TimeSpan.FromSeconds(1.0)).Wait()
        allFinish <- scnRunners |> Array.forall(fun x -> x.Finished)

let calcStatistics (scnRunners: ScenarioRunner[]) =
    scnRunners
    |> Array.map(fun x -> x.GetResult())
    |> GlobalStats.create    

let run (dep: Dependency, context: NBomberRunnerContext) =
    Dependency.Logger.initLogger(dep.ApplicationType)
    Log.Information("NBomber started a new session: '{0}'", dep.SessionId)

    match Validation.validateRunnerContext(context) with
    | Ok context ->             
        let scnRunners = buildScenarios(context) |> Array.map(ScenarioRunner)                
        
        let allOk = warmUpScenarios(scnRunners)
        
        if allOk then
            Log.Information("starting bombing...")
            scnRunners |> Array.iter(fun x -> x.Run())
            
            if dep.ApplicationType = ApplicationType.Console then
                displayProgress(scnRunners).Wait()

            waitUnitilAllFinish(scnRunners)            

            let globalStats = calcStatistics(scnRunners)
            let allAsserts = scnRunners |> Array.collect(fun x -> x.Scenario.Assertions)
            let assertResults = Assertion.apply(globalStats, allAsserts)
            Report.build(dep, globalStats, assertResults)
            |> Report.save(dep, "./")

            if dep.ApplicationType = ApplicationType.Test then
                TestFrameworkRunner.showResults(assertResults)        
    
    | Error e -> Log.Error(e)