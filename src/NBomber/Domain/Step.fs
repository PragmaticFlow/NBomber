[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Step

open System
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.Domain

let isAction (step) = match step with | Action _  -> true | _ -> false
let isPause (step)  = match step with | Pause _ -> true | _ -> false    
    
let getName (step) = 
    match step with
    | Action s  -> s.StepName    
    | Pause t -> Constants.PauseStepName
        
let getAction (step) =
    match step with
    | Action r -> r
    | _        -> failwith "step is not a Action"

let getConnectionPool (step) =
    match step with
    | Action s -> Some s.ConnectionPool    
    | Pause s -> None

let setStepContext (correlationId: string, actorIndex: int, cancelToken: CancellationToken)
                   (step: Step) =
    let getConnection (pool: ConnectionPool<obj>) =
        let connectionIndex = actorIndex % pool.ConnectionsCount
        pool.AliveConnections.[connectionIndex]

    match step with
    | Action s -> 
        let connection = getConnection(s.ConnectionPool)
        let context = { CorrelationId = correlationId
                        CancellationToken = cancelToken
                        Connection = connection
                        Payload = Unchecked.defaultof<obj> }
        Step.Action { s with CurrentContext = Some context }
    
    | Pause s -> step

let execStep (step: Step, prevPayload: obj, timer: Stopwatch) = task {    
    timer.Restart()
    try
        match step with
        | Action st ->
            let context = st.CurrentContext.Value
            context.Payload <- prevPayload
            let! resp = st.Execute(st.CurrentContext.Value)
            timer.Stop()
            let latency = Convert.ToInt32(timer.Elapsed.TotalMilliseconds)
            return (resp, latency)
        
        | Pause st ->
            do! Task.Delay(st)
            timer.Stop()
            let latency = Convert.ToInt32(timer.Elapsed.TotalMilliseconds)
            return (Response.Ok(prevPayload), latency)
    with
    | ex -> timer.Stop()
            let latency = Convert.ToInt32(timer.Elapsed.TotalMilliseconds)
            return (Response.Fail(), latency)
}

let runSteps (steps: Step[], cancelToken: FastCancellationToken) = task {
    
    let timer = Stopwatch()
    let latencies = ResizeArray<ResizeArray<Response*Latency>>()
    steps |> Array.iter(fun _ -> latencies.Add(ResizeArray<Response*Latency>()))    

    while not cancelToken.ShouldCancel do
        
        let mutable payload = Unchecked.defaultof<obj>
        let mutable skipStep = false
        let mutable stepIndex = 0

        for st in steps do
            if not skipStep && not cancelToken.ShouldCancel then

                let! response, latency = execStep(st, payload, timer)
                        
                if not cancelToken.ShouldCancel then
                    latencies.[stepIndex].Add(response,latency)
                    stepIndex <- stepIndex + 1
                    
                    if response.IsOk then 
                        payload <- response.Payload                    
                    else
                        skipStep <- true                        
    return latencies              
}