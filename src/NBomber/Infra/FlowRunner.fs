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

    let actors = createActors(flow)

    member x.Run() = actors |> Array.iter(fun x -> x.Run())
    member x.Stop() = actors |> Array.iter(fun x -> x.Stop())            
    member x.GetResult() = getFlowResults(flow.FlowName, actors)

type FlowActor(correlationId: string, allSteps: Step[]) =

    let mutable stop = false        
    let mutable currentTask = None        
    let mutable currentCts = None
    let stepsWithoutPause = allSteps |> Array.filter(fun st -> not(Step.isPause st))    
    let latencies = List<List<Response*Latency>>()
    let exceptions = List<Option<exn>*ExceptionCount>()

    let init () =
        stepsWithoutPause |> Array.iter(fun _ -> latencies.Add(List<Response*Latency>())
                                                 exceptions.Add(None, 0))
    do init()

    member x.Run() = 
        currentCts <- Some(new CancellationTokenSource())
        currentTask <- Some(Step.runSteps(allSteps, correlationId, latencies, exceptions, currentCts.Value.Token))

    member x.Stop() = if currentCts.IsSome then currentCts.Value.Cancel()
        
    member x.GetResults() =
        stepsWithoutPause
        |> Array.mapi(fun i st -> StepInfo.create(Step.getName(st), latencies.[i], exceptions.[i]))
                

let createActors (flow: TestFlow): FlowActor[] =
    flow.CorrelationIds
    |> Set.toArray
    |> Array.map(fun id -> FlowActor(id, flow.Steps))      

let getFlowResults (flowName: string, actors: FlowActor[]) =
    actors
    |> Array.collect(fun actor -> actor.GetResults())
    |> FlowInfo.create(flowName, actors.Length)