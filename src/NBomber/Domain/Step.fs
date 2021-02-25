[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Step

open System
open System.Collections.Generic
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Serilog
open HdrHistogram
open FSharp.UMX
open FSharp.Control.Tasks.NonAffine

open NBomber
open NBomber.Extensions.InternalExtensions
open NBomber.Contracts
open NBomber.Domain.DomainTypes
open NBomber.Domain.ConnectionPool
open NBomber.Domain.Statistics

type StepDep = {
    ScenarioName: string
    ScenarioMaxDuration: int64<ticks>
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

            | None -> Unchecked.defaultof<_>

        { CorrelationId = dep.CorrelationId
          CancellationToken = dep.CancellationToken
          Connection = getConnection step.ConnectionPool
          Logger = dep.Logger
          FeedItem = Unchecked.defaultof<_>
          Data = Dict.empty
          InvocationCount = 0
          StopScenario = fun (scnName,reason) -> StopScenario(scnName, reason) |> dep.ExecStopCommand
          StopCurrentTest = fun reason -> StopTest(reason) |> dep.ExecStopCommand }

module StepExecutionData =

    let createEmpty () =

        let createStats () = {
            RequestCount = 0
            MinTicks = % Int64.MaxValue
            MaxTicks = 0L<ticks>
            LessOrEq800 = 0
            More800Less1200 = 0
            MoreOrEq1200 = 0
            LatencyHistogramTicks = LongHistogram(TimeStamp.Hours(1), 3)
            MinBytes = % Int64.MaxValue
            MaxBytes = 0L<bytes>
            AllMB = 0.0<mb>
            DataTransferBytes = LongHistogram(TimeStamp.Hours(1), 3)
        }

        { OkStats = createStats()
          FailStats = createStats()
          ErrorStats = Dictionary<ErrorCode,ErrorStats>() }

    let addResponse (stData: StepExecutionData) (response: StepResponse) =

        let addErrorStats (errors: Dictionary<ErrorCode,ErrorStats>) (res: Response) =
            match errors.TryGetValue res.ErrorCode with
            | true, errorStats ->
                errors.[res.ErrorCode] <- { errorStats with Count = errorStats.Count + 1 }
            | false, _ ->
                errors.[res.ErrorCode] <- { ErrorCode = res.ErrorCode
                                            Message = res.Exception.Value.Message
                                            Count = 1 }

        let clientRes = response.ClientResponse

        // calc latency
        let latencyTicks =
            if clientRes.LatencyMs > 0.0 then Converter.fromMsToTicks(% clientRes.LatencyMs)
            else response.LatencyTicks

        let latencyMs = Converter.fromTicksToMs latencyTicks
        let responseBytes = clientRes.SizeBytes |> UMX.tag<bytes>

        let stats =
            match clientRes.Exception with
            | Some _ -> addErrorStats stData.ErrorStats clientRes
                        stData.FailStats

            | None   -> stData.OkStats

        stats.RequestCount <- stats.RequestCount + 1

        // checks that the response is real (it was created after the request was sent)
        if latencyTicks > 0L<ticks> then

            // add data transfer
            stats.MinTicks <- Statistics.min stats.MinTicks latencyTicks
            stats.MaxTicks <- Statistics.max stats.MaxTicks latencyTicks
            stats.LatencyHistogramTicks.RecordValue(int64 latencyTicks)

            if latencyMs <= 800.0<ms> then stats.LessOrEq800 <- stats.LessOrEq800 + 1
            elif latencyMs > 800.0<ms> && latencyMs < 1200.0<ms> then stats.More800Less1200 <- stats.More800Less1200 + 1
            elif latencyMs >= 1200.0<ms> then stats.MoreOrEq1200 <- stats.MoreOrEq1200 + 1

            // add data transfer
            stats.MinBytes <- Statistics.min stats.MinBytes responseBytes
            stats.MaxBytes <- Statistics.max stats.MaxBytes responseBytes
            stats.AllMB <- stats.AllMB + Statistics.Converter.fromBytesToMB responseBytes
            stats.DataTransferBytes.RecordValue(int64 responseBytes)

        stData

module RunningStep =

    let create (dep: StepDep) (step: Step) =
        { Value = step; Context = StepContext.create dep step; ExecutionData = StepExecutionData.createEmpty()  }

    let updateContext (step: RunningStep) (data: Dict<string,obj>) =
        let context = step.Context

        let feedItem =
            match step.Value.Feed with
            | Some feed -> feed.GetNextItem(context.CorrelationId, data)
            | None      -> Unchecked.defaultof<_>

        context.InvocationCount <- context.InvocationCount + 1
        context.Data <- data
        context.FeedItem <- feedItem
        step

let toUntypedExecute (execute: IStepContext<'TConnection,'TFeedItem> -> Response) =

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

let toUntypedExecuteAsync (execute: IStepContext<'TConnection,'TFeedItem> -> Task<Response>) =

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

let execStep (step: RunningStep) (globalTimer: Stopwatch) =
    let startTime = globalTimer.Elapsed
    try
        let resp =
            match step.Value.Execute with
            | SyncExec exec  -> exec step.Context
            | AsyncExec exec -> (exec step.Context).Result

        let endTime = globalTimer.Elapsed
        let latency = endTime - startTime

        { ClientResponse = resp; EndTimeTicks = % endTime.Ticks; LatencyTicks = % latency.Ticks }
    with
    | :? TaskCanceledException
    | :? OperationCanceledException ->
        { ClientResponse = Response.ok(); EndTimeTicks = 0L<ticks>; LatencyTicks = 0L<ticks> }

    | ex ->
        let endTime = globalTimer.Elapsed
        let latency = endTime - startTime
        { ClientResponse = Response.fail(ex); EndTimeTicks = % endTime.Ticks; LatencyTicks = % latency.Ticks }

let execStepAsync (step: RunningStep) (globalTimer: Stopwatch) = task {
    let startTime = globalTimer.Elapsed
    try
        let! resp =
            match step.Value.Execute with
            | SyncExec exec  -> Task.FromResult(exec step.Context)
            | AsyncExec exec -> exec step.Context

        let endTime = globalTimer.Elapsed
        let latency = endTime - startTime

        return { ClientResponse = resp; EndTimeTicks = % endTime.Ticks; LatencyTicks = % latency.Ticks }
    with
    | :? TaskCanceledException
    | :? OperationCanceledException ->
        return { ClientResponse = Response.ok(); EndTimeTicks = 0L<ticks>; LatencyTicks = 0L<ticks> }

    | ex ->
        let endTime = globalTimer.Elapsed
        let latency = endTime - startTime
        return { ClientResponse = Response.fail(ex); EndTimeTicks = % endTime.Ticks; LatencyTicks = % latency.Ticks }
}

let execSteps (dep: StepDep) (steps: RunningStep[]) (stepsOrder: int[]) =

    let data = Dictionary<string,obj>()
    let mutable skipStep = false

    for stepIndex in stepsOrder do
        if not skipStep && not dep.CancellationToken.IsCancellationRequested then
            try
                let step = RunningStep.updateContext steps.[stepIndex] data
                let response = execStep step dep.GlobalTimer

                if not dep.CancellationToken.IsCancellationRequested && not step.Value.DoNotTrack
                    && dep.ScenarioMaxDuration >= response.EndTimeTicks then

                        step.ExecutionData <- StepExecutionData.addResponse step.ExecutionData response

                        match response.ClientResponse.Exception with
                        | Some ex ->
                            dep.Logger.Error(ex, "Step '{StepName}' from scenario '{ScenarioName}' has failed. ", step.Value.StepName, dep.ScenarioName)
                            skipStep <- true

                        | None ->
                            data.[Constants.StepResponseKey] <- response.ClientResponse.Payload
            with
            | ex -> dep.Logger.Fatal(ex, "Step with index '{0}' from scenario '{ScenarioName}' has failed.", stepIndex, dep.ScenarioName)

let execStepsAsync (dep: StepDep) (steps: RunningStep[]) (stepsOrder: int[]) = task {

    let data = Dict.empty
    let mutable skipStep = false

    for stepIndex in stepsOrder do
        if not skipStep && not dep.CancellationToken.IsCancellationRequested then
            try
                let step = RunningStep.updateContext steps.[stepIndex] data
                let! response = execStepAsync step dep.GlobalTimer

                if not dep.CancellationToken.IsCancellationRequested && not step.Value.DoNotTrack
                    && dep.ScenarioMaxDuration >= response.EndTimeTicks then

                        step.ExecutionData <- StepExecutionData.addResponse step.ExecutionData response

                        match response.ClientResponse.Exception with
                        | Some ex ->
                            dep.Logger.Error(ex, "Step '{StepName}' from scenario '{ScenarioName}' has failed. ", step.Value.StepName, dep.ScenarioName)
                            skipStep <- true

                        | None ->
                            data.[Constants.StepResponseKey] <- response.ClientResponse.Payload
            with
            | ex -> dep.Logger.Fatal(ex, "Step with index '{0}' from scenario '{ScenarioName}' has failed.", stepIndex, dep.ScenarioName)
}

let isAllExecSync (steps: Step list) =
    steps
    |> List.map(fun x -> x.Execute)
    |> List.forall(function SyncExec _ -> true | AsyncExec _ -> false)

