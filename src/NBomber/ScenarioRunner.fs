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

let Run (scenario: Scenario) = 
    Infra.initLogger()

    Log.Information("{Scenario} has started", scenario.Name)
        
    Infra.initScenario(scenario)

    Log.Information("warmUp")
    Infra.warmUp(scenario)
        
    Log.Information("startContainers")
    let containers = Infra.startContainers(scenario)

    Log.Information("wait {time} until the execution ends", scenario.Interval.ToString())

    // wait until the execution ends
    Task.Delay(scenario.Interval).Wait()
    
    Log.Information("stopContainers")
    Infra.stopContainers(containers)
    
    let results = Infra.getResults(containers)

    Log.Information("buildReport")
    Reporting.buildReport(scenario, results)
    |> Reporting.saveReport

    Log.Information("{Scenario} has finished", scenario.Name) 
 

module private Infra =       

    type FlowRunner(steps: Step[]) =

        let mutable stop = false        
        let mutable currentTask = None        
        let results = Dictionary<StepName, List<Latency>>()

        do steps |> Array.iter(fun c -> if c.Name <> "pause" then results.[c.Name] <- List<Latency>())

        let runSteps (steps) = task {
            do! Task.Delay(100)
            let timer = Stopwatch()            
            while not stop do 
                for st in steps do
                    let! result = execStep(st, timer)                    
                    if st.Name <> "pause" then results.[st.Name].Add(result)
        }

        member x.Run() = currentTask <- steps |> runSteps |> Some                         
        
        member x.Stop() = stop <- true

        member x.GetResults() = results
            
                
    type FlowContainer(flow: Flow) =

        let flowRunners = [1 .. flow.ConcurrentCopies] |> List.map(fun _ -> FlowRunner(flow.Steps))

        member x.Run() = flowRunners |> List.iter(fun j -> j.Run())
        member x.Stop() = flowRunners |> List.iter(fun j -> j.Stop())
        
        member x.GetResults() =             
            let allResults = Dictionary<StepName, List<Latency>>()            
            flow.Steps |> Array.iter(fun c -> if c.Name <> "pause" then allResults.[c.Name] <- List<Latency>())
            // merge all results into one            
            for jr in flowRunners do
                jr.GetResults()
                |> Seq.iter(fun kpair -> allResults.[kpair.Key].AddRange(kpair.Value))

            { FlowName = flow.Name; Results = allResults; ConcurrentCopies = flow.ConcurrentCopies }


    let initScenario (scenario: Scenario) =         
        if scenario.InitFlow.IsSome then            
            Log.Debug("init has started", scenario.Name)
            scenario.InitFlow.Value.Execute().Wait()
            Log.Debug("init has finished", scenario.Name)        

    let warmUp (scenario: Scenario) =
        scenario.Flows         
        |> Array.iter(warmUpFlow)

    let warmUpFlow (flow: Flow) = 
        let timer = Stopwatch()
        let steps = flow.Steps |> Array.filter(fun x -> x.Name <> "pause")
        for c in steps do
            let t:Task<Latency> = execStep(c, timer)
            t.Wait()       
    
    let execStep (step: Step, timer: Stopwatch) = task {
        timer.Restart()
        do! step.Execute()
        timer.Stop()
        return timer.Elapsed.TotalMilliseconds |> Convert.ToInt64
    }

    let startContainers (scenario: Scenario) =
        let containers = scenario.Flows |> Array.map(FlowContainer)    
        containers |> Array.iter(fun c -> c.Run()) 
        containers

    let stopContainers (containers: FlowContainer[]) =
        containers |> Array.iter(fun c -> c.Stop())

    let getResults (containers: FlowContainer[]) =
        containers |> Array.map(fun c -> c.GetResults())

    let initLogger () =
        Log.Logger <- LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger()