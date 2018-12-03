module internal NBomber.Domain.Step

open System
open System.Collections.Generic
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.Domain.DomainTypes

let isPull (step)  = match step with | Pull _  -> true | _ -> false
let isPush (step)  = match step with | Push _  -> true | _ -> false
let isPause (step) = match step with | Pause _ -> true | _ -> false    
    
let getName (step) = 
    match step with
    | Pull s  -> s.StepName
    | Push s  -> s.StepName
    | Pause t -> "pause"
        
let getPull (step) =
    match step with
    | Pull r -> r
    | _         -> failwith "step is not a Pull"

let getPush (step) = 
    match step with 
    | Push l -> l
    | _          -> failwith "step is not a Push"

let execStep (step: Step, req: Request, timer: Stopwatch) = task {        
    timer.Restart()        
    try
        match step with
        | Pull s  -> let! resp = s.Execute(req)
                     timer.Stop()
                     let latency = Convert.ToInt32(timer.Elapsed.TotalMilliseconds)
                     return (resp, latency)
        
        | Push s  -> let listener = s.UpdatesChannel.GetPushListener(req.CorrelationId, s.StepName)
                     let! resp = listener.GetResponse()
                     timer.Stop()
                     let latency = Convert.ToInt32(timer.Elapsed.TotalMilliseconds)
                     return (resp, latency)
        
        | Pause s -> do! Task.Delay(s)
                     return (Response.Ok(req), 0)
    with
    | ex -> timer.Stop()
            let latency = Convert.ToInt32(timer.Elapsed.TotalMilliseconds)
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