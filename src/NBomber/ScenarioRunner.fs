module rec NBomber.ScenarioRunner

open System
open System.IO
open System.Diagnostics   
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks

open Serilog
open FSharp.Control.Tasks.V2.ContextInsensitive
open HdrHistogram

open Infra
open Reporting

let inline Run (scenario: Scenario) = run(scenario)

let run (scenario: Scenario) = 
    Infra.initLogger()

    Log.Information("{Scenario} has started", scenario.ScenarioName)
        
    Infra.initScenario(scenario)

    Log.Information("warming up")
    Infra.warmUp(scenario)
        
    Log.Information("starting test flows")
    let containers = Infra.startContainers(scenario)

    Log.Information("wait {time} until the execution ends", scenario.Interval.ToString())

    // wait until the execution ends
    Task.Delay(scenario.Interval).Wait()    
    
    Log.Information("stoping test flows")
    Infra.stopContainers(containers)

    // wait until the containers stop
    Task.Delay(TimeSpan.FromSeconds(1.0)).Wait()
    
    let results = Infra.getResults(containers)

    Log.Information("building report")
    Reporting.buildReport(scenario, results)
    |> Reporting.saveReport

    Log.Information("{Scenario} has finished", scenario.ScenarioName) 


module private Infra =

    type FlowRunner(steps: Step[]) =

        let mutable stop = false        
        let mutable currentTask = None        
        let latencies = Dictionary<StepName, List<OkOrFail * Latency>>()
        let exceptions = Dictionary<StepName, (Option<exn> * ExceptionCount)>()

        let init () =
            let withoutPause = steps |> Array.filter(fun st -> st.StepName <> "pause")
            withoutPause |> Array.iter(fun st -> latencies.[st.StepName] <- List<OkOrFail*Latency>())
            withoutPause |> Array.iter(fun st -> exceptions.[st.StepName] <- (None, 0))        

        do init()

        let runSteps (steps) = task {
            do! Task.Delay(10)
            let timer = Stopwatch()            
            while not stop do 
                for st in steps do
                    try
                        let! (okOrFail,latency) = execStep(st, timer)
                        if st.StepName <> "pause" then 
                            latencies.[st.StepName].Add(okOrFail,latency)
                        
                    with ex -> let (_, exCount) = exceptions.[st.StepName]
                               let newCount = exCount + 1
                               exceptions.[st.StepName] <- (Some(ex), newCount)                               
        }

        member x.Run() = currentTask <- steps |> runSteps |> Some                         
        
        member x.Stop() = stop <- true

        member x.GetResults() = 
            latencies 
            |> Seq.map(fun x -> StepResult.Create(x.Key, x.Value, exceptions.[x.Key]))
            |> Seq.toArray
            
                
    type FlowsContainer(flow: TestFlow) =

        let flowRunners = [|1 .. flow.ConcurrentCopies|] |> Array.map(fun _ -> FlowRunner(flow.Steps))

        member x.Run() = flowRunners |> Array.iter(fun j -> j.Run())
        member x.Stop() = flowRunners |> Array.iter(fun j -> j.Stop())
        
        member x.GetResult() =
            flowRunners
            |> Array.collect(fun runner -> runner.GetResults())
            |> FlowResult.Create(flow.FlowName, flow.ConcurrentCopies)            


    let initScenario (scenario: Scenario) =         
        if scenario.InitStep.IsSome then            
            Log.Debug("init has started", scenario.ScenarioName)
            scenario.InitStep.Value.Execute().Wait()
            Log.Debug("init has finished", scenario.ScenarioName)        

    let warmUp (scenario: Scenario) =
        scenario.Flows         
        |> Array.iter(warmUpFlow)

    let warmUpFlow (flow: TestFlow) = 
        let timer = Stopwatch()
        let steps = flow.Steps |> Array.filter(fun x -> x.StepName <> "pause")
        for st in steps do
            try
                let t:Task<OkOrFail*Latency> = execStep(st, timer)
                t.Wait()
            with ex -> Log.Error(ex.InnerException, "exception during warm up for TestFlow:{Flow} Step:{Step}", flow.FlowName, st.StepName)                
    
    let execStep (step: Step, timer: Stopwatch) = task {
        timer.Restart()
        let! okOrFail = step.Execute()
        timer.Stop()
        let latency = Convert.ToInt64(timer.Elapsed.TotalMilliseconds)
        return (okOrFail, latency)
    }

    let startContainers (scenario: Scenario) =
        let containers = scenario.Flows |> Array.map(FlowsContainer)    
        containers |> Array.iter(fun c -> c.Run()) 
        containers

    let stopContainers (containers: FlowsContainer[]) =
        containers |> Array.iter(fun c -> c.Stop())

    let getResults (containers: FlowsContainer[]) =
        containers |> Array.map(fun c -> c.GetResult())

    let initLogger () =
        Log.Logger <- LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger()