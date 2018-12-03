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

let tryGetTargetScenarios (context: NBomberRunnerContext) = maybe {
    let! config = context.NBomberConfig
    let! globalSettings = config.GlobalSettings
    return! Option.ofObj(globalSettings.TargetScenarios)
}

let updateScenarioWithSettings (scenario: Scenario) (settings: ScenarioSetting) =
    { scenario with ConcurrentCopies = settings.ConcurrentCopies
                    Duration = settings.Duration }

let filterTargetScenarios (targetScenarios: string[]) (scenarios: Scenario[]) =
    scenarios 
    |> Array.filter(fun x -> targetScenarios |> Array.exists(fun target -> x.ScenarioName = target))
        
let applyScenariosSettings (settings: ScenarioSetting[]) (scenarios: Scenario[]) =    
    if Array.isEmpty(settings) then
        scenarios
    else
        scenarios
        |> Array.map(fun scenario -> settings |> Array.tryFind(fun s -> s.ScenarioName = scenario.ScenarioName)
                                              |> function | Some setting -> updateScenarioWithSettings scenario setting 
                                                          | None -> scenario)
                                       
let initScenarios (scnRunners: ScenarioRunner[]) =    
    let results = 
        scnRunners
        |> Array.filter(fun x -> x.Scenario.TestInit.IsSome)
        |> Array.map(fun x -> 
            Log.Information("initializing scenario: '{0}'", x.Scenario.ScenarioName)
            let initResult = x.RunInit()
            
            if Result.isError(initResult) then
                let errorMsg = initResult |> Result.getError |> Errors.toString
                Log.Error("init failed", errorMsg)            
            initResult)
    results 
    |> Array.forall(Result.isOk)

let initUpdatesChannel (channel: DomainTypes.GlobalUpdatesChannel, scnRunners: ScenarioRunner[]) =
    let allIds = scnRunners |> Array.collect(fun x -> x.Scenario.CorrelationIds)
    
    let allPushStepNames =
        scnRunners 
        |> Array.collect(fun x -> x.Scenario.Steps) 
        |> Array.filter(Step.isPush)
        |> Array.map(Step.getName)
    
    channel.Init(allIds, allPushStepNames)

let warmUpScenarios (scnRunners: ScenarioRunner[]) =
    scnRunners 
    |> Array.iter(fun x -> 
        Log.Information("warming up scenario: '{0}'", x.Scenario.ScenarioName)
        let warmUpResult = x.WarmUp().Result
        
        if Result.isError(warmUpResult) then
            let errorMsg = warmUpResult |> Result.getError |> Errors.toString
            Log.Warning("warm up failed", errorMsg))

let buildScenarios (context: NBomberRunnerContext) =     
    let registeredScenarios = context.Scenarios
    let scenarioSettings = tryGetScenariosSettings(context)
    let targetScenarios = tryGetTargetScenarios(context)

    match (scenarioSettings, targetScenarios) with
    | Some settings, Some targetScns -> 
        registeredScenarios 
        |> filterTargetScenarios(targetScns)
        |> applyScenariosSettings(settings) 
        |> Array.map(Scenario.create)
    
    | Some settings, None ->         
        registeredScenarios
        |> applyScenariosSettings(settings)         
        |> Array.map(Scenario.create)

    | None, Some targetScns ->
        registeredScenarios
        |> filterTargetScenarios(targetScns)        
        |> Array.map(Scenario.create)

    | None, None -> context.Scenarios |> Array.map(Scenario.create)

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
        let allOk = initScenarios(scnRunners)
        
        if allOk then
            initUpdatesChannel(Dependency.GlobalUpdatesChannel, scnRunners)
            warmUpScenarios(scnRunners)
            
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