module internal rec NBomber.FlowRunner

open System
open System.Collections.Generic
open System.Diagnostics
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open NBomber.Reporting
open NBomber.FSharp

type FlowRunner(flow: TestFlow) =    

    let actors = createActors(flow)

    member x.Run() = actors |> Array.iter(fun x -> x.Run())
    member x.Stop() = actors |> Array.iter(fun x -> x.Stop())            
    member x.GetResult() = getFlowResults(flow.FlowName, actors)

type FlowActor(correlationId: string, allSteps: Step[]) =

    let mutable stop = false        
    let mutable currentTask = None        
    let stepsWithoutPause = allSteps |> Array.filter(fun st -> not(Step.isPause st))    
    let latencies = List<List<Response*Latency>>()
    let exceptions = List<Option<exn>*ExceptionCount>()

    let init () =
        stepsWithoutPause |> Array.iter(fun _ -> latencies.Add(List<Response*Latency>()))
        stepsWithoutPause |> Array.iter(fun _ -> exceptions.Add(None, 0))

    do init()
        
    let runSteps (steps: Step[]) = task {
        do! Task.Delay(10)
        let timer = Stopwatch()
            
        while not stop do
            
            let mutable request = { CorrelationId = correlationId; Payload = null }
            let mutable skipStep = false
            let mutable stepIndex = 0

            for st in steps do
                if not skipStep then
                    try
                        let! (response,latency) = execStep(st, request, timer)
                            
                        if not(Step.isPause st) then
                            latencies.[stepIndex].Add(response,latency)

                        if response.IsOk then 
                            request <- { request with Payload = response.Payload }
                            stepIndex <- stepIndex + 1
                        else
                            skipStep <- true
                        
                    with ex -> let (_, exCount) = exceptions.[stepIndex]
                               let newCount = exCount + 1
                               exceptions.[stepIndex] <- (Some(ex), newCount)
                               skipStep <- true
    }

    member x.Run() = currentTask <- allSteps |> runSteps |> Some                                 
    member x.Stop() = stop <- true    
    member x.GetResults() =
        stepsWithoutPause
        |> Array.mapi(fun i st -> StepInfo.create(Step.getName(st), latencies.[i], exceptions.[i]))
                

let createActors (flow: TestFlow): FlowActor[] =
    flow.CorrelationIds
    |> Set.toArray
    |> Array.map(fun id -> FlowActor(id, flow.Steps))      

let execStep (step: Step, req: Request, timer: Stopwatch) = task {        
    timer.Restart()        
    match step with
    | Request r  -> let! resp = r.Execute(req)
                    timer.Stop()
                    let latency = Convert.ToInt64(timer.Elapsed.TotalMilliseconds)
                    return (resp, latency)        
        
    | Listener l -> let listener = l.Listeners.Get(req.CorrelationId)
                    let! resp = listener.GetResponse()
                    timer.Stop()
                    let latency = Convert.ToInt64(timer.Elapsed.TotalMilliseconds)
                    return (resp, latency)
        
    | Pause time -> do! Task.Delay(time)
                    return (Response.Ok(req), int64(0))
}

let getFlowResults (flowName: string, actors: FlowActor[]) =
    actors
    |> Array.collect(fun actor -> actor.GetResults())
    |> FlowInfo.create(flowName, actors.Length)