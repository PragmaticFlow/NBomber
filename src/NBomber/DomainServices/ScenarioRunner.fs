module internal NBomber.DomainServices.ScenarioRunner

open System
open System.Collections.Generic
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
    let latencies = List<List<Response*Latency>>()

    let init () =
        latencies.Clear()
        stepsWithoutPause |> Array.iter(fun _ -> latencies.Add(List<Response*Latency>()))    

    member x.Run() = 
        init()
        currentCts <- Some(new CancellationTokenSource())
        currentTask <- Some(Step.runSteps(steps, latencies, currentCts.Value.Token))

    member x.Stop() = if currentCts.IsSome then currentCts.Value.Cancel()    
        
    member x.GetResults() =
        stepsWithoutPause
        |> Array.mapi(fun i st -> StepStats.create(Step.getName(st), latencies.[i]))

type ScenarioRunner(scenario: Scenario) =                       

    let createActors () =
        scenario.CorrelationIds |> Array.mapi(fun i id -> ScenarioActor(i, id, scenario))

    let mutable finished = false
    let actors = lazy createActors()
    let scnTimer = new System.Timers.Timer()

    let stop () =         
        actors.Value |> Array.iter(fun x -> x.Stop())
        scnTimer.Stop()
        finished <- true

    let run () =        
        stop()
        finished <- false
        scnTimer.Start()
        actors.Value |> Array.iter(fun x -> x.Run())
        
    let warmUp (interval: TimeSpan) = task {
        actors.Value |> Array.iter(fun x -> x.Run())
        do! Task.Delay(interval)
        actors.Value |> Array.iter(fun x -> x.Stop())
        do! Task.Delay(TimeSpan.FromSeconds(1.0))        
    }

    do scnTimer.Interval <- scenario.Duration.TotalMilliseconds
       scnTimer.Elapsed.Add(fun _ -> stop())

    member x.Finished = finished
    member x.Scenario = scenario
    member x.WarmUp(interval) = warmUp(interval)    
    member x.Run() = run()
    member x.Stop() = stop()
    member x.Dispose() = Scenario.dispose(scenario)
    
    member x.GetResult() =
        actors.Value
        |> Array.collect(fun actor -> actor.GetResults())
        |> ScenarioStats.create(scenario)