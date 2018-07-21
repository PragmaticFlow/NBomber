module internal rec NBomber.FlowActor

open System
open System.Collections.Generic
open System.Diagnostics
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open NBomber.Reporting

type FlowActor(flowId: int, allSteps: Step[]) =

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
            
            let mutable request = { FlowId = flowId; Payload = null }
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
                

type FlowActorsHost(flow: TestFlow) =

    let actors = [|0 .. flow.ConcurrentCopies - 1|] 
                 |> Array.map(fun flowId -> FlowActor(flowId, flow.Steps))

    member x.Run() = actors |> Array.iter(fun x -> x.Run())
    member x.Stop() = actors |> Array.iter(fun x -> x.Stop())            
    member x.GetResult() =
        actors
        |> Array.collect(fun runner -> runner.GetResults())
        |> FlowInfo.create(flow.FlowName, flow.ConcurrentCopies)
        

let execStep (step: Step, req: Request, timer: Stopwatch) = task {        
    timer.Restart()        
    match step with
    | Request r  -> let! resp = r.Execute(req)
                    timer.Stop()
                    let latency = Convert.ToInt64(timer.Elapsed.TotalMilliseconds)
                    return (resp, latency)        
        
    | Listener l -> let listener = l.Listeners.Get(req.FlowId)
                    let! resp = listener.GetResponse()
                    timer.Stop()
                    let latency = Convert.ToInt64(timer.Elapsed.TotalMilliseconds)
                    return (resp, latency)
        
    | Pause time -> do! Task.Delay(time)
                    return (Response.Ok(req), int64(0))
}