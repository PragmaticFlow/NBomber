module internal NBomber.DomainServices.ScenarioRunner

open System
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Statistics

type ScenarioActor(actorIndex: int, correlationId: string, scenario: Scenario) =
    
    let mutable currentTask = None        
    let mutable currentCts = None
    let steps = scenario.Steps |> Array.map(Step.setStepContext(correlationId, actorIndex))
    let stepsWithoutPause = steps |> Array.filter(fun st -> not(Step.isPause st))
    let latencies = ResizeArray<ResizeArray<Response*Latency>>()

    let init () =
        latencies.Clear()
        stepsWithoutPause |> Array.iter(fun _ -> latencies.Add(ResizeArray<Response*Latency>()))    

    member x.Run() = 
        init()
        currentCts <- Some(new CancellationTokenSource())
        currentTask <- Some(Step.runSteps(steps, latencies, currentCts.Value.Token))

    member x.Stop() = if currentCts.IsSome then currentCts.Value.Cancel()    
        
    member x.GetResults() =
        stepsWithoutPause
        |> Array.mapi(fun i st -> StepResults.create(Step.getName(st), latencies.[i].ToArray()))

type ScenarioRunner(scenario: Scenario) =                       

    let createActors () =
        scenario.CorrelationIds |> Array.mapi(fun i id -> ScenarioActor(i, id, scenario))
    
    let mutable finished = true
    let actors = lazy createActors()    

    let stop () =  
        if not finished then
            actors.Value |> Array.iter(fun x -> x.Stop())        
            finished <- true

    let run () = task {       
        stop()
        finished <- false        
        actors.Value |> Array.iter(fun x -> x.Run())
        do! Task.Delay(scenario.Duration)
        actors.Value |> Array.iter(fun x -> x.Stop())
        do! Task.Delay(TimeSpan.FromSeconds(1.0))    
    }
        
    let warmUp (interval: TimeSpan) = task {
        actors.Value |> Array.iter(fun x -> x.Run())
        do! Task.Delay(interval)
        actors.Value |> Array.iter(fun x -> x.Stop())
        do! Task.Delay(TimeSpan.FromSeconds(1.0))        
    }    
        
    member x.Scenario = scenario
    member x.WarmUp(interval) = warmUp(interval)    
    member x.Run() = run()
    member x.Stop() = stop()    
    
    member x.GetResult() =
        actors.Value
        |> Array.collect(fun actor -> actor.GetResults())
        |> ScenarioStats.create(scenario)