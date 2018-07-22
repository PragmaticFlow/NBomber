module rec NBomber.ScenarioRunner

open System
open System.IO
open System.Diagnostics   
open System.Threading.Tasks

open Serilog
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Errors
open NBomber.Reporting
open NBomber.FlowActor
open Infra

let inline Run (scenario: Scenario) = run(scenario)
    
let run (scenario: Scenario) =
    let mutable runningScenario = true
    while runningScenario do
        runScenario(scenario)

        Log.Information("Repeat the same Scenario one more time? (y/n)")
        let userInput = Console.ReadLine()
        runningScenario <- Seq.contains userInput ["y"; "Y"; "yes"; "Yes"]

let runScenario (scenario: Scenario) = 
    Infra.initLogger() 

    scenario.Flows |> Array.iter(Infra.initFlow)

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

        let assertionResults = Infra.executeAssertions(results, scenario.Asserts)
        for result in assertionResults do Log.Information(result)

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
                let req = { FlowId = 0; Payload = null }
                step.Execute(req).Wait()
                Log.Debug("init has finished", scenario.ScenarioName)
                Ok <| scenario
            with ex -> Error <| InitStepError(ex.Message)
        | None      -> Ok <| scenario
    
    let initFlow (flow: TestFlow) =
        let flowListeners = Array.create flow.ConcurrentCopies (FlowListener())
        flow.Steps
        |> Array.filter(Step.isListener)
        |> Array.map(Step.getListener)
        |> Array.iter(fun listStep -> listStep.Listeners.Init(flowListeners))            

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
        let mutable request = { FlowId = 0; Payload = null }
        let mutable result = Ok()
        let mutable skipStep = false

        for st in steps do
            if not skipStep then        
                try            
                    let! (response,_) = FlowActor.execStep(st, request, timer)
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
        let actorsHost = scenario.Flows |> Array.map(FlowActorsHost)    
        actorsHost |> Array.iter(fun x -> x.Run()) 
        actorsHost

    let stopFlows (actorsHost: FlowActorsHost[]) =
        Log.Information("stoping test flows")
        actorsHost |> Array.iter(fun x -> x.Stop())

    let getResults (actorsHost: FlowActorsHost[]) =
        actorsHost |> Array.map(fun x -> x.GetResult())

    let executeAssertions(flows:FlowInfo[], assertions:Assert[]) =
        assertions |> Seq.mapi (fun i assertion -> executeAssertion(flows, i+1, assertion)) |> checkIfAllOk                
    
    let executeAssertion(flows:FlowInfo[], i:Index, assertion:Assert) =
        match assertion with
            | All assertion -> assertForAll(flows, i, assertion)

            | Flow (flowName, assertion) -> 
                flows |> Seq.tryFind (fun f -> f.FlowName = flowName) |> assertForFlow <| (i, flowName, assertion)

            | Step (flowName, stepName, assertion) -> 
                flows |> Seq.tryFind (fun f -> f.FlowName = flowName) |> (function
                    | Some f -> f.Steps
                                |> Seq.tryFind (fun s -> s.StepName = stepName)
                                |> assertForStep <| (i, stepName, assertion)
                    | None -> NotFound(i, Scope.Flow, flowName))

    let checkIfAllOk (asserted:seq<Asserted>) =
        asserted |> Seq.forall(fun a -> Asserted.isOk(a)) |> function 
                    | true -> asserted |> Seq.length |> 
                                function 
                                | 0 -> [||] 
                                | length -> [| sprintf "Assertions: %i - OK" length |]
                    | _ -> asserted |> Seq.choose(fun result -> Asserted.getMessage(result)) |> Seq.toArray

    let assertForAll (flows:FlowInfo[], i:Index, assertion:Assertion) =
         flows |> Seq.map (fun f -> Some(f) |> assertForFlow <| (i, f.FlowName, assertion))
               |> Seq.exists (fun a -> Asserted.isFailed(a)) |> Asserted.create <| (i, Scope.All, "")  

    let assertForFlow (flow:FlowInfo option) (i:Index, flowName:string, assertion:Assertion) =
        match flow with
        | Some f -> f.Steps 
                    |> Seq.map (fun s -> Some(s) |> assertForStep <| (i, s.StepName, assertion))
                    |> Seq.exists(fun a -> Asserted.isFailed(a)) 
                    |> Asserted.create <| (i, Scope.Flow, f.FlowName)        
        | None -> NotFound(i, Scope.Flow, flowName)      

    let assertForStep (step:StepInfo option) (i:Index, stepName:string, assertion:Assertion) =
        match step with
        | Some s -> s   |> AssertionStats.Create
                        |> assertion.Invoke
                        |> Asserted.create <| (i, Scope.Step, s.StepName)                    
        | None -> NotFound(i, Scope.Step, stepName) 

    let initLogger () =
        Log.Logger <- LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger()