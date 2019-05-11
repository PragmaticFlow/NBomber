[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Step

open System
open System.Diagnostics
open System.Threading

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Extensions
open NBomber.Contracts
open NBomber.Domain

let create (steps: IStep[]) = 
    steps |> Seq.cast<Step> |> Seq.toArray

let setStepContext (correlationId: string, actorIndex: int, cancelToken: CancellationToken)
                   (step: Step) =
    
    let getConnection (pool: ConnectionPool<obj>) =
        let connectionIndex = actorIndex % pool.ConnectionsCount
        pool.AliveConnections.[connectionIndex]
    
    let connection = getConnection(step.ConnectionPool)
    let context = { CorrelationId = correlationId
                    CancellationToken = cancelToken
                    Connection = connection
                    Payload = Unchecked.defaultof<obj> }    
    
    { step with CurrentContext = Some context }
    
let execStep (step: Step, prevPayload: obj, timer: Stopwatch) = task {    
    timer.Restart()
    try 
        let context = step.CurrentContext.Value
        context.Payload <- prevPayload
        let! resp = step.Execute(step.CurrentContext.Value)
        timer.Stop()
        let latency = int timer.Elapsed.TotalMilliseconds
        return (resp, latency)
    with
    | ex -> timer.Stop()
            let latency = int timer.Elapsed.TotalMilliseconds
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