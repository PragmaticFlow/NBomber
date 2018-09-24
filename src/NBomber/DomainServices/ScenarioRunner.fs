module internal NBomber.DomainServices.ScenarioRunner

open System
open System.Diagnostics   
open System.Threading.Tasks

open Serilog

open NBomber
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.StatisticsTypes
open NBomber.Domain.Statistics
open NBomber.Infra
open NBomber.DomainServices.Reporting
open NBomber.DomainServices.FlowRunner

type Config = {
    IsVerbose: bool
    ShouldSaveReport: bool    
}

let private initScenario (scenario: Scenario) =
    Log.Information("initializing scenario...")
    Scenario.runInit(scenario)

let private warmUpScenario (scenario: Scenario) =
    Log.Information("warming up scenario...")
    let result = Scenario.warmUp(scenario)
    match result with
    | Ok v    -> Ok(scenario)
    | Error e -> e |> Errors.toString |> Log.Warning
                 Ok(scenario)

let private buildAndSaveReport (scenario: Scenario, scenarioStats: ScenarioStats, shouldSaveReport: bool) =
    Log.Information("building report...")
    let dep = Dependency.create(scenario)
    let report = Report.build(dep, scenarioStats)
    if shouldSaveReport then Report.save(dep, report, "./")
    report

let private startFlows (scenario: Scenario) =
    Log.Information("starting test flows")
    let runners = scenario.TestFlows |> Array.map(TestFlowRunner)    
    runners |> Array.iter(fun x -> x.Run()) 
    runners

let private stopFlows (runner: TestFlowRunner[]) =
    Log.Information("stoping test flows")
    runner |> Array.iter(fun x -> x.Stop())

let private getResults (runner: TestFlowRunner[]) =
    runner |> Array.map(fun x -> x.GetResult())

let runScenario (scn: Contracts.Scenario, config: Config) = 
    if config.IsVerbose then Dependency.Logger.initLogger() 

    let scenario = Scenario.create(scn)
    Log.Information("{Scenario} has started", scn.ScenarioName)        

    let result = initScenario(scenario)
                 |> Result.bind(warmUpScenario)
                 |> Result.map(startFlows)
    match result with
    | Ok actorsHosts ->
        Log.Information("wait {time} until the execution ends", scenario.Duration.ToString())
        Log.Information("processing...")
        
        let t1 = Task.Delay(scenario.Duration)
        let t2 = if config.IsVerbose then Dependency.ProgressBar.show(scenario.Duration)
                 else Task.FromResult()
                
        // waiting until the Scenario ends
        t1.Wait()
        stopFlows(actorsHosts)                        
        let results = getResults(actorsHosts) 
        let scenarioStats = ScenarioStats.create(scenario, results)

        t2.Wait()

        let report = buildAndSaveReport(scenario, scenarioStats, config.ShouldSaveReport)
        Log.Information("{Scenario} has finished", scenario.ScenarioName)
        Log.Information(report.TxtReport)

        Ok(scenarioStats)        
         
    | Error e -> let message = Errors.toString(e)
                 Log.Error(message)
                 Error(e)

let runInConsole (scenario: Contracts.Scenario) =
    let mutable runningScenario = true
    let config = { IsVerbose = true; ShouldSaveReport = true }    
    while runningScenario do
        runScenario(scenario, config) |> ignore

        Log.Information("Repeat the same Scenario one more time? (y/n)")
        
        let userInput = Console.ReadLine()
        runningScenario <- List.contains userInput ["y"; "Y"; "yes"; "Yes"]

let runTest (scenario: Contracts.Scenario) =    
    let config = { IsVerbose = false
                   ShouldSaveReport = false }
    let result = runScenario(scenario, config)
    match result with
    | Ok scnStats -> TestRunner.run(scenario.Assertions, scnStats)
    | Error error -> error |> Domain.Errors.toString |> failwith