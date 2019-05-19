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
    | ex -> let endTime = globalTimer.Elapsed.TotalMilliseconds
            let latency = int(endTime - startTime)
            return { Response = Response.Fail()
                     StartTimeMs = startTime                     
                     LatencyMs = int latency }
}

let runSteps (steps: Step[], cancelToken: FastCancellationToken,
              globalTimer: Stopwatch) = task {
        
    let responses = ResizeArray<ResizeArray<StepResponse>>()
    steps |> Array.iter(fun _ -> responses.Add(ResizeArray<StepResponse>()))    

    while not cancelToken.ShouldCancel do
        
      let mutable data = Unchecked.defaultof<obj>
      let mutable skipStep = false
      let mutable stepIndex = 0      

      for st in steps do
        if not skipStep && not cancelToken.ShouldCancel then

          for i = 1 to st.RepeatCount do
            let! response = execStep(st, data, globalTimer)

            if not cancelToken.ShouldCancel then
               responses.[stepIndex].Add(response)

               if st.RepeatCount = i then
                 if response.Response.IsOk then
                    stepIndex <- stepIndex + 1
                    data <- response.Response.Payload
                 else
                    skipStep <- true

    return responses
}

let filterInvalidResponses (responses: StepResponse[], duration: TimeSpan) =        
    let validEndTime (endTime) = endTime <= duration.TotalMilliseconds
    let createEndTime (response) = response.StartTimeMs + float response.LatencyMs
    
    responses
    |> Array.choose(fun x ->
        match x |> createEndTime |> validEndTime with
        | true  -> Some x
        | false -> None)