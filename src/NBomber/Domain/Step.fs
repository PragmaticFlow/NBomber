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

let getConnectionPool (step) =
    match step with
    | Pull s -> Some s.ConnectionPool
    | Push s -> Some s.ConnectionPool
    | Pause s -> None

let setStepContext (correlationId: string, actorIndex: int) (step: Step) =
    let getConnection (pool: ConnectionPool<obj>) =
        let connectionIndex = actorIndex % pool.ConnectionsCount
        pool.AliveConnections.[connectionIndex]

    match step with
    | Pull s -> let connection = getConnection(s.ConnectionPool)
                let context = { CorrelationId = correlationId
                                Connection = connection
                                Payload = Unchecked.defaultof<obj> }
                Pull { s with CurrentContext = Some context }
    
    | Push s -> let connection = getConnection(s.ConnectionPool)
                let context = { CorrelationId = correlationId
                                Connection = connection
                                UpdatesChannel = UpdatesChannel() }
                Push { s with CurrentContext = Some context }
    
    | Pause s -> step 
    
let setConnectionPool (allPools: ConnectionPool<obj>[]) (step) =    
    let findPool (poolName) = allPools |> Array.find(fun x -> x.PoolName = poolName)
    match step with
    | Pull s -> Pull { s with ConnectionPool = findPool(s.ConnectionPool.PoolName) }    
    | Push s -> Push { s with ConnectionPool = findPool(s.ConnectionPool.PoolName) }
    | Pause s -> Pause s    

let execStep (step: Step, prevPayload: obj, timer: Stopwatch) = task {    
    timer.Restart()        
    try
        match step with
        | Pull s  -> let context = s.CurrentContext.Value
                     context.Payload <- prevPayload
                     let! resp = s.Execute(s.CurrentContext.Value)
                     timer.Stop()
                     let latency = Convert.ToInt32(timer.Elapsed.TotalMilliseconds)
                     return (resp, latency)
        
        | Push s  -> let context = s.CurrentContext.Value
                     let channel = context.UpdatesChannel :?> UpdatesChannel
                     let responseT = channel.GetResponse()
                     do! s.Handler(context)
                     let! response = responseT
                     timer.Stop()                     
                     let latency = Convert.ToInt32(timer.Elapsed.TotalMilliseconds)
                     return (response, latency)
        
        | Pause s -> do! Task.Delay(s)
                     return (Response.Ok(prevPayload), 0)
    with
    | ex -> timer.Stop()
            let latency = Convert.ToInt32(timer.Elapsed.TotalMilliseconds)
            return (Response.Fail(ex.ToString()), latency)
}

let runSteps (steps: Step[], latencies: List<List<Response*Latency>>, 
              ct: CancellationToken) = task {
        
    do! Task.Delay(10)        
    let timer = Stopwatch()

    while not ct.IsCancellationRequested do
        
        let mutable payload = Unchecked.defaultof<obj>
        let mutable skipStep = false
        let mutable stepIndex = 0

        for st in steps do
            if not skipStep && not ct.IsCancellationRequested then

                let! (response,latency) = execStep(st, payload, timer)
                            
                if not(isPause st) then                        
                    latencies.[stepIndex].Add(response,latency)
                    stepIndex <- stepIndex + 1

                if response.IsOk then 
                    payload <- response.Payload                    
                else
                    skipStep <- true                
}