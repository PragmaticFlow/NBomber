module internal rec NBomber.FlowRunner

open System
open System.Collections.Generic
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.Domain
open NBomber.Statistics

type FlowRunner(flow: TestFlow) =    

    let createActors (flow: TestFlow) =
        flow.CorrelationIds
        |> Set.toArray
        |> Array.map(fun id -> FlowActor(id, flow))

    let actors = createActors(flow)

    member x.Run() = actors |> Array.iter(fun x -> x.Run())
    member x.Stop() = actors |> Array.iter(fun x -> x.Stop())            
    
    member x.GetResult() = 
        actors
        |> Array.collect(fun actor -> actor.GetResults())
        |> FlowStats.create(flow)

type FlowActor(correlationId: string, flow: TestFlow) =
    
    let mutable currentTask = None        
    let mutable currentCts = None
    let stepsWithoutPause = flow.Steps |> Array.filter(fun st -> not(Step.isPause st))    
    let latencies = List<List<Response*Latency>>()    

    let init () =
        stepsWithoutPause |> Array.iter(fun _ -> latencies.Add(List<Response*Latency>()))
    do init()

    member x.Run() = 
        currentCts <- Some(new CancellationTokenSource())
        currentTask <- Some(Step.runSteps(flow.Steps, correlationId, latencies, currentCts.Value.Token))

    member x.Stop() = if currentCts.IsSome then currentCts.Value.Cancel()
        
    member x.GetResults() =
        stepsWithoutPause
        |> Array.mapi(fun i st -> StepStats.create(Step.getName(st), latencies.[i]))