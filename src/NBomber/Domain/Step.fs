[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Step

open System
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Serilog
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber
open NBomber.Extensions
open NBomber.Contracts
open NBomber.Domain

let toUntypedExec (execute: StepContext<'TConnection,'TFeedItem> -> Task<Response>) =
    fun (context: UntypedStepContext) ->

        let typedContext = {
            StepContext.CorrelationId = context.CorrelationId
            CancellationToken = context.CancellationToken
            Connection = context.Connection :?> 'TConnection
            Data = context.Data
            FeedItem = context.FeedItem :?> 'TFeedItem
            Logger = context.Logger
        }

        execute(typedContext)


let setStepContext (logger: ILogger, correlationId: CorrelationId,
                    cancelToken: CancellationToken, step: Step, data: Dict<string,obj>) =

    let getConnection (pool: UntypedConnectionPool) =
        if pool.AliveConnections.Length > 0 then
            let index = correlationId.CopyNumber % pool.AliveConnections.Length
            pool.AliveConnections.[index]
        else
            () :> obj

    let connection = getConnection(step.ConnectionPool)
    let context = { CorrelationId = correlationId
                    CancellationToken = cancelToken
                    Connection = connection
                    Data = data
                    FeedItem = step.Feed.GetNextItem(correlationId, data)
                    Logger = logger }

    { step with Context = Some context }

let execStep (step: Step, globalTimer: Stopwatch) = task {
    let startTime = globalTimer.Elapsed.TotalMilliseconds
    try
        let! resp = step.Execute(step.Context.Value)
        let endTime = globalTimer.Elapsed.TotalMilliseconds
        let latency = int(endTime - startTime)
        return { Response = resp; StartTimeMs = startTime; LatencyMs = latency }
    with
    | :? TaskCanceledException
    | :? OperationCanceledException ->
        return { Response = Response.Ok(); StartTimeMs = -1.0; LatencyMs = -1 }

    | ex -> let endTime = globalTimer.Elapsed.TotalMilliseconds
            let latency = int(endTime - startTime)
            return { Response = Response.Fail(ex); StartTimeMs = startTime; LatencyMs = int latency }
}

let execSteps (logger: ILogger, correlationId: CorrelationId,
               steps: Step[],
               allScnResponses: (StepResponse list)[],
               cancelToken: CancellationToken,
               globalTimer: Stopwatch) = task {

    let data = Dict.empty
    let mutable skipStep = false
    let mutable stepIndex = 0
    let resourcesToDispose = ResizeArray<IDisposable>(steps.Length)

    let cleanResources () =
        for rs in resourcesToDispose do
            try rs.Dispose()
            with _ -> ()

    for s in steps do
        if not skipStep && not cancelToken.IsCancellationRequested then

            for i = 1 to s.RepeatCount do
                let step = setStepContext(logger, correlationId, cancelToken, s, data)
                let! response = execStep(step, globalTimer)

                if response.Response.Payload :? IDisposable then
                    resourcesToDispose.Add(response.Response.Payload :?> IDisposable)

                if not cancelToken.IsCancellationRequested && not step.DoNotTrack then
                    let stepResponses = response :: allScnResponses.[stepIndex]
                    allScnResponses.[stepIndex] <- stepResponses

                if step.RepeatCount = i then
                    if response.Response.Exception.IsNone then
                        stepIndex <- stepIndex + 1
                        data.[Constants.StepResponseKey] <- response.Response.Payload
                    else
                        logger.Error(response.Response.Exception.Value, "step '{Step}' is failed. ", step.StepName)
                        skipStep <- true
    cleanResources()
}

let filterByDuration (stepResponses: StepResponse list, duration: TimeSpan) =
    let validEndTime (endTime) = endTime <= duration.TotalMilliseconds
    let createEndTime (response) = response.StartTimeMs + float response.LatencyMs

    stepResponses
    |> List.filter(fun x -> x.StartTimeMs <> -1.0) // to filter out TaskCanceledException
    |> List.choose(fun x ->
        match x |> createEndTime |> validEndTime with
        | true  -> Some x
        | false -> None)
    |> List.toArray
