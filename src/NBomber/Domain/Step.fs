module internal NBomber.Domain.Step

open System
open System.Collections.Generic
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.Domain.DomainTypes

let isRequest (step)  = match step with | Request _  -> true | _ -> false
let isListener (step) = match step with | Listener _ -> true | _ -> false
let isPause (step)    = match step with | Pause _    -> true | _ -> false    
    
let getName (step) = 
    match step with
    | Request s  -> s.StepName
    | Listener s -> s.StepName
    | Pause t    -> "pause"
        
let getRequest (step) =
    match step with
    | Request r -> r
    | _         -> failwith "step is not a Request"

let getListener (step) = 
    match step with 
    | Listener l -> l
    | _          -> failwith "step is not a Listener"

let execStep (step: Step, req: Request, timer: Stopwatch) = task {        
    timer.Restart()        
    try
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
                        return (Response.Ok(req), 0L)
    with
    | ex -> timer.Stop()
            let latency = Convert.ToInt64(timer.Elapsed.TotalMilliseconds)
            return (Response.Fail(ex.ToString()), latency)
}

let runSteps (steps: Step[], correlationId: CorrelationId,
              latencies: List<List<Response*Latency>>,
              ct: CancellationToken) = task {
        
    do! Task.Delay(10)        
    let timer = Stopwatch()

    while not ct.IsCancellationRequested do
            
        let mutable request = { CorrelationId = correlationId; Payload = null }
        let mutable skipStep = false
        let mutable stepIndex = 0

        for st in steps do
            if not skipStep && not ct.IsCancellationRequested then

                let! (response,latency) = execStep(st, request, timer)
                            
                if not(isPause st) then                        
                    latencies.[stepIndex].Add(response,latency)

                if response.IsOk then 
                    request <- { request with Payload = response.Payload }
                    stepIndex <- stepIndex + 1
                else
                    skipStep <- true
}