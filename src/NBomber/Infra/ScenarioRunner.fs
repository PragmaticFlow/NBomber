﻿module rec NBomber.ScenarioRunner

open System
open System.IO
open System.Diagnostics   
open System.Threading.Tasks

open Serilog
open ShellProgressBar
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Errors
open NBomber.Domain
open NBomber.Statistics
open NBomber.FlowRunner

let Run (scenario: Contracts.Scenario) =
    let mutable runningScenario = true
    while runningScenario do
        runScenario(scenario)

        Log.Information("Repeat the same Scenario one more time? (y/n)")
        let userInput = Console.ReadLine()
        runningScenario <- Seq.contains userInput ["y"; "Y"; "yes"; "Yes"]

let private runScenario (config: Contracts.Scenario) = 
    
    initLogger() 

    let scenario = Scenario.create(config)

    Log.Information("{Scenario} has started", config.ScenarioName)

    let result = Scenario.runInit(scenario)
                 |> Result.bind(Scenario.warmUpScenario)
                 |> Result.map(startFlows) 
    
    match result with
    | Ok actorsHosts ->

        Log.Information("wait {time} until the execution ends", scenario.Duration.ToString())
        Log.Information("processing...")
        
        let t1 = Task.Delay(scenario.Duration)
        let t2 = runProgressBar(scenario.Duration)
        Task.WhenAll(t1, t2).Wait()
        
        stopFlows(actorsHosts)

        // wait until the flow runners stop
        Task.Delay(TimeSpan.FromSeconds(1.0)).Wait()
        
        let results = getResults(actorsHosts)        
                                                                                       
        let assertionData = results |> Array.collect (fun f -> f.Steps |> Array.map(fun step -> Contracts.AssertionStats.Create(step.StepName, f.FlowName, step.OkCount,
                                                                                                    step.FailCount, step.ExceptionCount, step.ThrownException)))

        let assertionResults = Assertions.apply(scenario.ScenarioName, assertionData, scenario.Assertions)
        for result in assertionResults do Log.Error(result)

        Log.Information("building report")
        Reporting.buildReport(scenario, results)    
        |> Reporting.saveReport
        |> Log.Information

        Log.Information("{Scenario} has finished", scenario.ScenarioName)

    | Error e -> let msg = Errors.printError(e)
                 Log.Error(msg) 

let private startFlows (scenario: Scenario) =
    Log.Information("starting test flows")
    let runners = scenario.TestFlows |> Array.map(FlowRunner)    
    runners |> Array.iter(fun x -> x.Run()) 
    runners

let private stopFlows (runner: FlowRunner[]) =
    Log.Information("stoping test flows")
    runner |> Array.iter(fun x -> x.Stop())

let private getResults (runner: FlowRunner[]) =
    runner |> Array.map(fun x -> x.GetResult())    

let private initLogger () =
    Log.Logger <- LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger()

let private runProgressBar (scenarioDuration: TimeSpan) = task {    
    let options = ProgressBarOptions(ProgressBarOnBottom = true,                                     
                                     ForegroundColor = ConsoleColor.Yellow,
                                     ForegroundColorDone = Nullable<ConsoleColor>(ConsoleColor.DarkGreen),
                                     BackgroundColor = Nullable<ConsoleColor>(ConsoleColor.DarkGray),
                                     BackgroundCharacter = Nullable<char>('\u2593'))

    let totalSeconds = int(scenarioDuration.TotalSeconds)                                        
    use pbar = new ProgressBar(totalSeconds, String.Empty, options)
    
    for i = 0 to totalSeconds do        
        do! Task.Delay(TimeSpan.FromSeconds(1.0))
        pbar.Tick()
}