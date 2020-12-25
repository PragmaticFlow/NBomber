[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Step

open System
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Nessos.Streams
open Serilog
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber
open NBomber.Extensions.InternalExtensions
open NBomber.Contracts
open NBomber.Domain.DomainTypes
open NBomber.Domain.ConnectionPool

type StepDep = {
    Logger: ILogger
    CancellationToken: CancellationToken
    GlobalTimer: Stopwatch
    CorrelationId: CorrelationId
    ExecStopCommand: StopCommand -> unit
}

module StepContext =

    let create (dep: StepDep) (step: Step) =

        let getConnection (pool: ConnectionPool option) =
            match pool with
            | Some v ->
                let index = dep.CorrelationId.CopyNumber % v.AliveConnections.Length
                v.AliveConnections.[index]

            | None -> () :> obj

        { CorrelationId = dep.CorrelationId
          CancellationToken = dep.CancellationToken
          Connection = getConnection(step.ConnectionPool)
          Logger = dep.Logger
          FeedItem = Unchecked.defaultof<_>
          Data = Dict.empty
          InvocationCount = 0
          StopScenario = fun (scnName,reason) -> StopScenario(scnName, reason) |> dep.ExecStopCommand
          StopCurrentTest = fun reason -> StopTest(reason) |> dep.ExecStopCommand }

module RunningStep =

    let create (dep: StepDep) (step: Step) =
        { Value = step; Context = StepContext.create dep step }

    let updateContext (step: RunningStep) (data: Dict<string,obj>) =
        let context = step.Context
        let feedItem = step.Value.Feed.GetNextItem(context.CorrelationId, data)

        context.InvocationCount <- context.InvocationCount + 1
        context.Data <- data
        context.FeedItem <- feedItem
        step

let toUntypedExec (execute: IStepContext<'TConnection,'TFeedItem> -> Task<Response>) =

    fun (untypedCtx: UntypedStepContext) ->

        let typedCtx = {
            new IStepContext<'TConnection,'TFeedItem> with
                member _.CorrelationId = untypedCtx.CorrelationId
                member _.CancellationToken = untypedCtx.CancellationToken
                member _.Connection = untypedCtx.Connection :?> 'TConnection
                member _.Data = untypedCtx.Data
                member _.FeedItem = untypedCtx.FeedItem :?> 'TFeedItem
                member _.Logger = untypedCtx.Logger
                member _.InvocationCount = untypedCtx.InvocationCount
                member _.StopScenario(scenarioName, reason) = untypedCtx.StopScenario(scenarioName, reason)
                member _.StopCurrentTest(reason) = untypedCtx.StopCurrentTest(reason)

                member _.GetPreviousStepResponse() =
                    try
                        let prevStepResponse = untypedCtx.Data.[Constants.StepResponseKey]
                        if isNull prevStepResponse then
                            Unchecked.defaultof<'T>
                        else
                            prevStepResponse :?> 'T
                    with
                    | ex -> Unchecked.defaultof<'T>
        }

        execute typedCtx

let execStep (step: RunningStep) (globalTimer: Stopwatch) = task {
    let startTime = globalTimer.Elapsed.TotalMilliseconds
    try
        let! resp = step.Value.Execute(step.Context)

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

let execSteps (dep: StepDep)
              (steps: RunningStep[])
              (allScnResponses: (ResizeArray<StepResponse>)[]) = task {

    let data = Dict.empty
    let mutable skipStep = false
    let mutable stepIndex = 0

    for st in steps do
        if not skipStep && not dep.CancellationToken.IsCancellationRequested then
            try
                let step = RunningStep.updateContext st data
                let! response = execStep step dep.GlobalTimer

                let payload = response.Response.Payload

                if not dep.CancellationToken.IsCancellationRequested && not step.Value.DoNotTrack then
                    response.Response.Payload <- null
                    allScnResponses.[stepIndex].Add(response)

                if response.Response.Exception.IsNone then
                    stepIndex <- stepIndex + 1
                    data.[Constants.StepResponseKey] <- payload
                else
                    dep.Logger.Error(response.Response.Exception.Value, "Step '{Step}' is failed. ", step.Value.StepName)
                    skipStep <- true
            with
            | ex -> dep.Logger.Error(ex, "Step '{Step}' is failed. ", st.Value.StepName)
}

let filterByDuration (duration: TimeSpan) (stepResponses: Stream<StepResponse>) =
    let validEndTime (endTime) = endTime <= duration.TotalMilliseconds
    let createEndTime (response) = response.StartTimeMs + float response.LatencyMs

    stepResponses
    |> Stream.filter(fun x -> x.StartTimeMs <> -1.0) // to filter out TaskCanceledException
    |> Stream.choose(fun x ->
        match x |> createEndTime |> validEndTime with
        | true  -> Some x
        | false -> None)
