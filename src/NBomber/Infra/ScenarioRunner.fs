module rec NBomber.ScenarioRunner

open System
open System.IO
open System.Diagnostics   
open System.Threading.Tasks

open Serilog
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.Errors
open NBomber.Domain
open NBomber.Statistics
open NBomber.FlowRunner
    
let Run (scenario: ScenarioConfig) =
    let mutable runningScenario = true
    while runningScenario do
        runScenario(scenario)

        Log.Information("Repeat the same Scenario one more time? (y/n)")
        let userInput = Console.ReadLine()
        runningScenario <- Seq.contains userInput ["y"; "Y"; "yes"; "Yes"]

let private runScenario (config: ScenarioConfig) = 
    
    Infra.initLogger() 

    let scenario = Scenario.create(config)

    Log.Information("{Scenario} has started", config.ScenarioName)

    let result = Scenario.runInit(scenario)
                 |> Result.bind(Scenario.warmUpScenario)
                 |> Result.map(Infra.startFlows) 
    
    match result with
    | Ok actorsHosts ->   
    
        Log.Information("wait {time} until the execution ends", scenario.Duration.ToString())
        Task.Delay(scenario.Duration).Wait()    
        
        Infra.stopFlows(actorsHosts)

        // wait until the flow runners stop
        Task.Delay(TimeSpan.FromSeconds(1.0)).Wait()
        
        let results = Infra.getResults(actorsHosts)        

        Log.Information("building report")
        Reporting.buildReport(scenario, results)    
        |> Reporting.saveReport
        |> Log.Information

        Log.Information("{Scenario} has finished", scenario.ScenarioName)

    | Error e -> let msg = Errors.printError(e)
                 Log.Error(msg)


module private Infra =               

    let startFlows (scenario: Scenario) =
        Log.Information("starting test flows")
        let runners = scenario.TestFlows |> Array.map(FlowRunner)    
        runners |> Array.iter(fun x -> x.Run()) 
        runners

    let stopFlows (runner: FlowRunner[]) =
        Log.Information("stoping test flows")
        runner |> Array.iter(fun x -> x.Stop())

    let getResults (runner: FlowRunner[]) =
        runner |> Array.map(fun x -> x.GetResult())    

    let initLogger () =
        Log.Logger <- LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger()