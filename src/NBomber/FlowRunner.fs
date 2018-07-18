module internal rec NBomber.FlowRunner

open System
open System.Collections.Generic
open System.Diagnostics
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open NBomber.Reporting

type FlowRunner(allSteps: Step[]) =

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
            
            let mutable request = null
            let mutable skipStep = false
            let mutable stepIndex = 0

            for st in steps do
                if not skipStep then
                    try
                        let! (response,latency) = execStep(st, request, timer)
                            
                        if not(Step.isPause st) then
                            latencies.[stepIndex].Add(response,latency)
                            
                        stepIndex <- stepIndex + 1

                        if response.IsOk then request <- response.Payload
                        else skipStep <- true
                        
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
                

type FlowsContainer(flow: TestFlow) =

    let flowRunners = [|1 .. flow.ConcurrentCopies|] |> Array.map(fun _ -> FlowRunner(flow.Steps))

    member x.Run() = flowRunners |> Array.iter(fun j -> j.Run())
    member x.Stop() = flowRunners |> Array.iter(fun j -> j.Stop())            
    member x.GetResult() =
        flowRunners
        |> Array.collect(fun runner -> runner.GetResults())
        |> FlowInfo.create(flow.FlowName, flow.ConcurrentCopies)
        

let execStep (step: Step, req: Request, timer: Stopwatch) = task {        
    timer.Restart()        
    match step with
    | Request s  -> let! resp = s.Execute(req)
                    timer.Stop()
                    let latency = Convert.ToInt64(timer.Elapsed.TotalMilliseconds)
                    return (resp, latency)        
        
    | Push p     -> let! tResp = p.Trigger(req)
                    let! (resp,msgCount) = p.Listener.WaitOnResponse()
                    timer.Stop()
                    let latency = Convert.ToInt64(timer.Elapsed.TotalMilliseconds)
                    return (resp, latency)
        
    | Pause time -> do! Task.Delay(time)
                    return (Response.Ok(req), int64(0))
}
