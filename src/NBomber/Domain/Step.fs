[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Step

open System
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Serilog
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Extensions
open NBomber.Contracts
open NBomber.Domain

let setStepContext (correlationId: string, actorIndex: int, cancelToken: CancellationToken)
                   (step: Step) =
    
    let getConnection (pool: ConnectionPool<obj>) =
        match pool.ConnectionsCount with
        | Some v -> let connectionIndex = actorIndex % pool.ConnectionsCount.Value
                    pool.AliveConnections.[connectionIndex]
        | None   -> pool.AliveConnections.[actorIndex]
    
    let connection = getConnection(step.ConnectionPool)
    let context = { CorrelationId = correlationId
                    CancellationToken = cancelToken
                    Connection = connection
                    Data = Unchecked.defaultof<obj> }    
    
    { step with CurrentContext = Some context }
    
let execStep (step: Step, data: obj, globalTimer: Stopwatch) = task {    
    let startTime = globalTimer.Elapsed.TotalMilliseconds
    try 
        step.CurrentContext.Value.Data <- data
        
        let! resp = step.Execute(step.CurrentContext.Value)
        
        let endTime = globalTimer.Elapsed.TotalMilliseconds
        let latency = int(endTime - startTime)        
        return { Response = resp
                 StartTimeMs = startTime                 
                 LatencyMs = latency }
    with
    | :? TaskCanceledException as ex ->
        let endTime = globalTimer.Elapsed.TotalMilliseconds
        let latency = int(endTime - startTime)
        return { Response = Response.Ok()
                 StartTimeMs = startTime
                 LatencyMs = latency }
        
    | ex -> let endTime = globalTimer.Elapsed.TotalMilliseconds
            let latency = int(endTime - startTime)
            return { Response = Response.Fail(ex)
                     StartTimeMs = startTime                     
                     LatencyMs = int latency }
}

let execSteps (logger: ILogger,
               steps: Step[],
               responses: ResizeArray<ResizeArray<StepResponse>>,
               cancelToken: FastCancellationToken,
               globalTimer: Stopwatch) = task {

    let mutable data = Unchecked.defaultof<obj>
    let mutable skipStep = false
    let mutable stepIndex = 0
    let resourcesToDispose = ResizeArray<IDisposable>(steps.Length)

    let cleanResources () =
        for rs in resourcesToDispose do
            try rs.Dispose()
            with _ -> ()
    
    for st in steps do
        if not skipStep && not cancelToken.ShouldCancel then
            
            for i = 1 to st.RepeatCount do
                let! response = execStep(st, data, globalTimer)
                
                if response.Response.Payload :? IDisposable then
                    resourcesToDispose.Add(response.Response.Payload :?> IDisposable)                
                    
                if not cancelToken.ShouldCancel && st.DoNotTrack = false then
                    responses.[stepIndex].Add(response)

                if st.RepeatCount = i then
                    if response.Response.Exception.IsNone then
                        stepIndex <- stepIndex + 1
                        data <- response.Response.Payload
                    else
                        logger.Error(response.Response.Exception.Value, "step '{Step}' is failed", st.StepName)
                        skipStep <- true
    
    cleanResources()
}

let filterLateResponses (responses: StepResponse seq, duration: TimeSpan) =        
    let validEndTime (endTime) = endTime <= duration.TotalMilliseconds
    let createEndTime (response) = response.StartTimeMs + float response.LatencyMs
    
    responses
    |> Seq.choose(fun x ->
        match x |> createEndTime |> validEndTime with
        | true  -> Some x
        | false -> None)
    |> Seq.toArray