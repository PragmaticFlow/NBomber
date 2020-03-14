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
open NBomber.Domain.ConnectionPool

type StepDep = {
    Logger: ILogger
    CancellationToken: CancellationToken
    GlobalTimer: Stopwatch
    CorrelationId: CorrelationId
}

let toUntypedExec (execute: StepContext<'TConnection,'TFeedItem> -> Task<Response>) =
    fun (context: StepContext<obj,obj>) ->
        let typedContext = {
            StepContext.CorrelationId = context.CorrelationId
            CancellationToken = context.CancellationToken
            Connection = context.Connection :?> 'TConnection
            Data = context.Data
            FeedItem = context.FeedItem :?> 'TFeedItem
            Logger = context.Logger
        }
        execute(typedContext)

let setStepContext (dep: StepDep, step: Step, data: Dict<string,obj>) =

    let getConnection (pool: ConnectionPool option) =
        match pool with
        | Some v ->
            let index = dep.CorrelationId.CopyNumber % v.AliveConnections.Length
            v.AliveConnections.[index]

        | None -> () :> obj

    let connection = getConnection(step.ConnectionPool)
    let context = { CorrelationId = dep.CorrelationId
                    CancellationToken = dep.CancellationToken
                    Connection = connection
                    Data = data
                    FeedItem = step.Feed.GetNextItem(dep.CorrelationId, data)
                    Logger = dep.Logger }

    { step with Context = Some context }

let execStep (step: Step, globalTimer: Stopwatch) = task {
    let startTime = globalTimer.Elapsed.TotalMilliseconds
    try
        let! resp = step.Execute(step.Context.Value)

        let latency =
            if resp.LatencyMs > 0 then resp.LatencyMs
            else
                let endTime = globalTimer.Elapsed.TotalMilliseconds
                int(endTime - startTime)

        return { Response = resp; StartTimeMs = startTime; LatencyMs = latency }
    with
    | :? TaskCanceledException
    | :? OperationCanceledException ->
        return { Response = Response.Ok(); StartTimeMs = -1.0; LatencyMs = -1 }

    | ex -> let endTime = globalTimer.Elapsed.TotalMilliseconds
            let latency = int(endTime - startTime)
            return { Response = Response.Fail(ex); StartTimeMs = startTime; LatencyMs = int latency }
}

let execSteps (dep: StepDep, steps: Step[], allScnResponses: (StepResponse list)[]) = task {

    let data = Dict.empty
    let mutable skipStep = false
    let mutable stepIndex = 0
    let resourcesToDispose = ResizeArray<IDisposable>(steps.Length)

    let cleanResources () =
        for rs in resourcesToDispose do
            try rs.Dispose()
            with _ -> ()

    for s in steps do
        if not skipStep && not dep.CancellationToken.IsCancellationRequested then

            for i = 1 to s.RepeatCount do
                let step = setStepContext(dep, s, data)
                let! response = execStep(step, dep.GlobalTimer)

                if response.Response.Payload :? IDisposable then
                    resourcesToDispose.Add(response.Response.Payload :?> IDisposable)

                if not dep.CancellationToken.IsCancellationRequested && not step.DoNotTrack then
                    let stepResponses = response :: allScnResponses.[stepIndex]
                    allScnResponses.[stepIndex] <- stepResponses

                if step.RepeatCount = i then
                    if response.Response.Exception.IsNone then
                        stepIndex <- stepIndex + 1
                        data.[Constants.StepResponseKey] <- response.Response.Payload
                    else
                        dep.Logger.Error(response.Response.Exception.Value, "step '{Step}' is failed. ", step.StepName)
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
