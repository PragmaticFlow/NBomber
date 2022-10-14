[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Step

open System
open System.Collections.Generic
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Serilog

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Internal
open NBomber.Domain.DomainTypes
open NBomber.Domain.ScenarioContext
open NBomber.Domain.Stats.ScenarioStatsActor

let emptyFail<'T> : FlowResponse<'T> =
    { StatusCode = Nullable()
      IsError = true
      SizeBytes = 0
      Message = String.Empty
      LatencyMs = 0
      Payload = None }

let measure (name: string) (ctx: ScenarioContext) (run: unit -> Task<FlowResponse<'T>>) = backgroundTask {
    let startTime = ctx.Timer.Elapsed.TotalMilliseconds
    try
        let! response = run()
        let endTime = ctx.Timer.Elapsed.TotalMilliseconds
        let latency = endTime - startTime

        let result = { StepName = name; ClientResponse = response; EndTimeMs = endTime; LatencyMs = latency }
        ctx.StatsActor.Publish(AddStepResult result)
        return response
    with
    | ex ->
        let endTime = ctx.Timer.Elapsed.TotalMilliseconds
        let latency = endTime - startTime

        ctx.StopIteration <- true
        let context = ctx :> IFlowContext
        context.Logger.Fatal(ex, $"Unhandled exception for Scenario: {0}, Step: {1}", context.ScenarioInfo.ScenarioName, name)

        let error = FlowResponse.fail<'T>(ex, latencyMs = latency)
        let result = { StepName = name; ClientResponse = error; EndTimeMs = endTime; LatencyMs = latency }
        ctx.StatsActor.Publish(AddStepResult result)
        return error
}

// let startTime = globalTimer.Elapsed.TotalMilliseconds
//         try
//             let responseTask = step.Value.Execute(step.Context)
//
//             // for pause we skip timeout logic
//             if step.Value.IsPause then
//                 let! pause = responseTask
//                 return { StepIndex = step.StepIndex; ClientResponse = pause; EndTimeMs = 0.0; LatencyMs = 0.0 }
//             else
//                 let! finishedTask = Task.WhenAny(responseTask, Task.Delay step.Value.Timeout)
//                 let endTime = globalTimer.Elapsed.TotalMilliseconds
//                 let latency = endTime - startTime
//
//                 if finishedTask.Id = responseTask.Id then
//                     return { StepIndex = step.StepIndex; ClientResponse = responseTask.Result; EndTimeMs = endTime; LatencyMs = latency }
//                 else
//                     step.Context.CancellationTokenSource.Cancel()
//                     let resp = Response.fail(statusCode = Constants.TimeoutStatusCode, error = $"step timeout: {step.Value.Timeout.TotalMilliseconds} ms")
//                     return { StepIndex = step.StepIndex; ClientResponse = resp; EndTimeMs = endTime; LatencyMs = latency }
//         with
//         | :? TaskCanceledException
//         | :? OperationCanceledException ->
//             let endTime = globalTimer.Elapsed.TotalMilliseconds
//             let latency = endTime - startTime
//             let resp = Response.fail(statusCode = Constants.TimeoutStatusCode, error = "step timeout")
//             return { StepIndex = step.StepIndex; ClientResponse = resp; EndTimeMs = endTime; LatencyMs = latency }
//         | ex ->
//             let endTime = globalTimer.Elapsed.TotalMilliseconds
//             let latency = endTime - startTime
//             let resp = Response.fail(statusCode = Constants.StepUnhandledErrorCode, error = $"step unhandled exception: {ex.Message}")
//             return { StepIndex = step.StepIndex; ClientResponse = resp; EndTimeMs = endTime; LatencyMs = latency }

module StepContext =

    let getPreviousStepResponse (data: Dictionary<string,obj>) =
        try
            let prevStepResponse = data[Constants.StepResponseKey]
            if isNull prevStepResponse then
                Unchecked.defaultof<'T>
            else
                prevStepResponse :?> 'T
        with
        | ex -> Unchecked.defaultof<'T>

    let getClient (scnInfo: ScenarioInfo) (factory: IUntypedClientFactory option) =
        match factory with
        | Some v ->
            let index = scnInfo.ThreadNumber % v.ClientCount
            v.GetClient index

        | None -> Unchecked.defaultof<_>

    let createUntyped (stCtx: StepExecContext) (step: Step) =
        { StepName = step.StepName
          ScenarioInfo = stCtx.ScenarioInfo
          CancellationTokenSource = new CancellationTokenSource()
          Client = getClient stCtx.ScenarioInfo step.ClientFactory
          Logger = stCtx.ScenarioExecContext.Logger
          FeedItem = ()
          Data = Dictionary<string,obj>()
          InvocationNumber = 0
          StopScenario = fun (scnName,reason) -> StopScenario(scnName, reason) |> stCtx.ScenarioExecContext.ExecStopCommand
          StopCurrentTest = fun reason -> StopTest(reason) |> stCtx.ScenarioExecContext.ExecStopCommand }

    let create (untyped: UntypedStepContext) = {
        new IStepContext<'TClient,'TFeedItem> with
            member _.StepName = untyped.StepName
            member _.ScenarioInfo = untyped.ScenarioInfo
            member _.CancellationToken = untyped.CancellationTokenSource.Token
            member _.Client = untyped.Client :?> 'TClient
            member _.Data = untyped.Data
            member _.FeedItem = untyped.FeedItem :?> 'TFeedItem
            member _.Logger = untyped.Logger
            member _.InvocationNumber = untyped.InvocationNumber
            member _.GetPreviousStepResponse() = getPreviousStepResponse(untyped.Data)
            member _.StopScenario(scenarioName, reason) = untyped.StopScenario(scenarioName, reason)
            member _.StopCurrentTest(reason) = untyped.StopCurrentTest(reason)
    }

    let toUntypedExecute (execute: IStepContext<'TClient,'TFeedItem> -> Task<Response>) =
        fun (untyped: UntypedStepContext) ->
            let typed = create untyped
            execute typed

module RunningStep =

    let create (stCtx: StepExecContext) (stepIndex: int) (step: Step) =
        { StepIndex = stepIndex; Value = step; Context = StepContext.createUntyped stCtx step }

    let updateContext (step: RunningStep) (data: Dictionary<string,obj>) (cancelToken: CancellationTokenSource) =
        let st = step.Value
        let context = step.Context

        let feedItem =
            match step.Value.Feed with
            | Some feed -> feed.GetNextItem(context.ScenarioInfo, data)
            | None      -> Unchecked.defaultof<_>

        context.CancellationTokenSource <- cancelToken
        context.InvocationNumber <- context.InvocationNumber + 1
        context.Data <- data
        context.FeedItem <- feedItem
        // context.Client should be set as the last field because init order matter here
        context.Client <- StepContext.getClient context.ScenarioInfo st.ClientFactory

        step

    let measureExec (step: RunningStep) (globalTimer: Stopwatch) = backgroundTask {
        let startTime = globalTimer.Elapsed.TotalMilliseconds
        try
            let responseTask = step.Value.Execute(step.Context)

            // for pause we skip timeout logic
            if step.Value.IsPause then
                let! pause = responseTask
                return { StepIndex = step.StepIndex; ClientResponse = pause; EndTimeMs = 0.0; LatencyMs = 0.0 }
            else
                let! finishedTask = Task.WhenAny(responseTask, Task.Delay step.Value.Timeout)
                let endTime = globalTimer.Elapsed.TotalMilliseconds
                let latency = endTime - startTime

                if finishedTask.Id = responseTask.Id then
                    return { StepIndex = step.StepIndex; ClientResponse = responseTask.Result; EndTimeMs = endTime; LatencyMs = latency }
                else
                    step.Context.CancellationTokenSource.Cancel()
                    let resp = Response.fail(statusCode = Constants.TimeoutStatusCode, error = $"step timeout: {step.Value.Timeout.TotalMilliseconds} ms")
                    return { StepIndex = step.StepIndex; ClientResponse = resp; EndTimeMs = endTime; LatencyMs = latency }
        with
        | :? TaskCanceledException
        | :? OperationCanceledException ->
            let endTime = globalTimer.Elapsed.TotalMilliseconds
            let latency = endTime - startTime
            let resp = Response.fail(statusCode = Constants.TimeoutStatusCode, error = "step timeout")
            return { StepIndex = step.StepIndex; ClientResponse = resp; EndTimeMs = endTime; LatencyMs = latency }
        | ex ->
            let endTime = globalTimer.Elapsed.TotalMilliseconds
            let latency = endTime - startTime
            let resp = Response.fail(statusCode = Constants.StepUnhandledErrorCode, error = $"step unhandled exception: {ex.Message}")
            return { StepIndex = step.StepIndex; ClientResponse = resp; EndTimeMs = endTime; LatencyMs = latency }
    }

    let execStep (stCtx: StepExecContext) (step: RunningStep) = backgroundTask {

        let! response = measureExec step stCtx.ScenarioExecContext.ScenarioTimer
        let payload = response.ClientResponse.Payload

        if not step.Value.DoNotTrack then
            stCtx.ScenarioExecContext.ScenarioStatsActor.Publish(AddResponse response)

            if response.ClientResponse.IsError then
                stCtx.ScenarioExecContext.Logger.Fatal($"Step '{step.Value.StepName}' from Scenario: '{stCtx.ScenarioInfo.ScenarioName}' has failed. Error: {response.ClientResponse.Message}")
            else
                stCtx.Data[Constants.StepResponseKey] <- payload

            return response.ClientResponse

        elif step.Value.IsPause then
            return response.ClientResponse

        else
            if response.ClientResponse.IsError then
                stCtx.ScenarioExecContext.Logger.Fatal($"Step '{step.Value.StepName}' from Scenario: '{stCtx.ScenarioInfo.ScenarioName}' has failed. Error: {response.ClientResponse.Message}")
            else
                stCtx.Data[Constants.StepResponseKey] <- payload

            return response.ClientResponse
    }

    let execCustomExec (stCtx: StepExecContext) (steps: RunningStep[]) (stepInterception: IStepInterceptionContext voption -> string voption) = backgroundTask {
        let mutable shouldWork = true
        let mutable execContext = ValueNone

        while shouldWork
              && not stCtx.ScenarioExecContext.ScenarioCancellationToken.IsCancellationRequested
              && stCtx.ScenarioInfo.ScenarioDuration.TotalMilliseconds > (stCtx.ScenarioExecContext.ScenarioTimer.Elapsed.TotalMilliseconds + Constants.SchedulerTimerDriftMs) do

            let nextStep = stepInterception execContext
            match nextStep with
            | ValueSome stepName ->
                let stepIndex = stCtx.ScenarioExecContext.Scenario.StepOrderIndex[stepName]
                use cancelToken = new CancellationTokenSource()
                let step = updateContext steps[stepIndex] stCtx.Data cancelToken
                let! response = execStep stCtx step

                execContext <- ValueSome {
                    new IStepInterceptionContext with
                        member _.PrevStepContext = StepContext.create step.Context
                        member _.PrevStepResponse = response
                }

            | ValueNone -> shouldWork <- false
    }

    let execRegularExec (stCtx: StepExecContext) (steps: RunningStep[]) (stepsOrder: int[]) = backgroundTask {
        let mutable shouldWork = true
        for stepIndex in stepsOrder do

            if shouldWork
               && not stCtx.ScenarioExecContext.ScenarioCancellationToken.IsCancellationRequested
               && stCtx.ScenarioInfo.ScenarioDuration.TotalMilliseconds > (stCtx.ScenarioExecContext.ScenarioTimer.Elapsed.TotalMilliseconds + Constants.SchedulerTimerDriftMs) then

                use cancelToken = new CancellationTokenSource()
                let step = updateContext steps[stepIndex] stCtx.Data cancelToken
                let! response = execStep stCtx step

                if response.IsError then
                    shouldWork <- false
    }

    let execSteps (stCtx: StepExecContext) (steps: RunningStep[]) (stepsOrder: int[]) =
        match stCtx.ScenarioExecContext.Scenario.StepInterception with
        | Some stepInterception -> execCustomExec stCtx steps stepInterception
        | None                  -> execRegularExec stCtx steps stepsOrder
