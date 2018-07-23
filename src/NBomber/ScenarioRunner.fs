module rec NBomber.ScenarioRunner

open System
open System.IO
open System.Diagnostics   
open System.Threading.Tasks

open Serilog
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Errors
open NBomber.Reporting
open NBomber.FlowRunner
    
let Run (scenario: Scenario) =
    let mutable runningScenario = true
    while runningScenario do
        runScenario(scenario)

        Log.Information("Repeat the same Scenario one more time? (y/n)")
        let userInput = Console.ReadLine()
        runningScenario <- Seq.contains userInput ["y"; "Y"; "yes"; "Yes"]

let runScenario (scenario: Scenario) = 
    Infra.initLogger() 

    scenario.Flows |> Array.iter(TestFlow.initFlow)

    Log.Information("{Scenario} has started", scenario.ScenarioName)
        
    let result = Infra.initScenario(scenario)
                 |> Result.bind(Infra.warmUpScenario)
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

    let initScenario (scenario: Scenario) =         
        match scenario.InitStep with
        | Some step -> 
            try
                Log.Debug("init has started", scenario.ScenarioName)
                let req = { CorrelationId = Constants.InitId; Payload = null }
                step.Execute(req).Wait()
                Log.Debug("init has finished", scenario.ScenarioName)
                Ok <| scenario
            with ex -> Error <| InitStepError(ex.Message)
        | None      -> Ok <| scenario

    let warmUpScenario (scenario: Scenario) =
        Log.Information("warming up")
        let errors = scenario.Flows 
                     |> Array.map(fun flow -> warmUpFlow(flow).Result)
                     |> Array.filter(Result.isError)
                     |> Array.map(Result.getError)
        
        if errors.Length > 0 then Error <| FlowErrors(errors)
        else Ok <| scenario

    let warmUpFlow (flow: TestFlow): Task<Result<unit,FlowError>> = task {
        let timer = Stopwatch()        
        let steps = flow.Steps |> Array.filter(fun st -> not(Step.isPause st))        
        let mutable request = { CorrelationId = Constants.WarmUpId; Payload = null }
        let mutable result = Ok()
        let mutable skipStep = false

        for st in steps do
            if not skipStep then        
                try            
                    let! (response,_) = FlowRunner.execStep(st, request, timer)
                    if response.IsOk then
                        request <- { request with Payload = response.Payload }
                    else 
                        skipStep <- true                        
                        result <- Error({ FlowName = flow.FlowName; StepName = Step.getName(st); Error = response.Payload.ToString() })

                with ex -> skipStep <- true
                           result <- Error({ FlowName = flow.FlowName; StepName = Step.getName(st); Error = ex.Message })
        return result
    }    

    let startFlows (scenario: Scenario) =
        Log.Information("starting test flows")
        let runners = scenario.Flows |> Array.map(FlowRunner)    
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