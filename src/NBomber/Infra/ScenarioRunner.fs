module internal rec NBomber.ScenarioRunner

open System
open System.IO
open System.Diagnostics   
open System.Threading.Tasks

open Serilog
open ShellProgressBar
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Errors
open NBomber.Domain
open NBomber.Domain.Assertions
open NBomber.Statistics
open NBomber.FlowRunner
open System.Runtime.InteropServices

let runInConsole (scenario: Contracts.Scenario) =
    let mutable runningScenario = true
    while runningScenario do
        run(scenario, true) |> ignore

        Log.Information("Repeat the same Scenario one more time? (y/n)")
        
        let userInput = Console.ReadLine()
        runningScenario <- Seq.contains userInput ["y"; "Y"; "yes"; "Yes"]

let run (config: Contracts.Scenario, isVerbose: bool) = 
    if isVerbose then initLogger() 

    let scenario = Scenario.create(config)

    Log.Information("{Scenario} has started", config.ScenarioName)        

    let result = initScenario(scenario) |> Result.bind(warmUpScenario)
    let actorsHosts = match result with
                        | Ok warmedUpScenario -> warmedUpScenario
                        | Error e -> Errors.printError(e) |> Log.Error; scenario
                        |> startFlows
    
    Log.Information("{Scenario} has started", config.ScenarioName)

    Log.Information("wait {time} until the execution ends", scenario.Duration.ToString())
    Log.Information("processing...")

    let t1 = Task.Delay(scenario.Duration)
    let t2 = if isVerbose then runProgressBar(scenario.Duration) else Task.FromResult(())
            
    // waiting until the Scenario ends
    t1.Wait()
    stopFlows(actorsHosts)                        
    let results = getResults(actorsHosts) 

    t2.Wait()

    if isVerbose then 
        Log.Information("building report...")
        Reporting.buildReport(scenario, results)    
        |> Reporting.saveReport
        |> Log.Information

        Log.Information("{Scenario} has finished", scenario.ScenarioName)

    results
    |> Array.collect (fun flow -> flow.Steps |> Array.map(fun step -> (flow, step)))
    |> Array.map(fun (flow, step) -> createAssertionStats(flow.FlowName, step))
    |> applyAssertions(scenario.ScenarioName, scenario.Assertions)
    |> outputAssertionResults 

let private initLogger () =
    Log.Logger <- LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger()

let private initScenario (scenario: Scenario) =
    Log.Information("initializing scenario...")
    Scenario.init(scenario)

let private warmUpScenario (scenario: Scenario) =
    Log.Information("warming up scenario...")
    Scenario.warmUp(scenario)

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

let private createAssertionStats (flowName: string, step: StepInfo) =
    { StepName = step.StepName; FlowName = flowName; OkCount = step.OkCount; FailCount = step.FailCount;
        ExceptionCount = step.ExceptionCount; ThrownException = step.ThrownException }

let private outputAssertionResults (assertionResult: Result<int, string[]>) =
    match assertionResult with
     | Ok 0 -> [||]
     | Ok assertionCount -> Log.Information(sprintf "Assertions: %i - OK" assertionCount); [||]
     | Error messages -> messages |> Array.iter(fun msg -> Log.Error(msg)); messages
