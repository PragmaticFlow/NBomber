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
open NBomber.Domain.ClientPool
open NBomber.Domain.Stats.ScenarioStatsActor

type ScenarioDep = {
    Logger: ILogger
    Scenario: Scenario
    ScenarioCancellationToken: CancellationTokenSource
    ScenarioTimer: Stopwatch
    ScenarioOperation: ScenarioOperation
    ScenarioStatsActor: ScenarioStatsActor
    ExecStopCommand: StopCommand -> unit
    MaxFailCount: int
}

type StepDep = {
    ScenarioDep: ScenarioDep
    ScenarioInfo: ScenarioInfo
    Data: Dictionary<string,obj>
}

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

    let createUntyped (dep: StepDep) (step: Step) =

        let getClient (pool: ClientPool option) =
            match pool with
            | Some v ->
                let index = dep.ScenarioInfo.ThreadNumber % v.InitializedClients.Length
                v.InitializedClients[index]

            | None -> Unchecked.defaultof<_>

        { StepName = step.StepName
          ScenarioInfo = dep.ScenarioInfo
          CancellationTokenSource = new CancellationTokenSource()
          Client = getClient step.ClientPool
          Logger = dep.ScenarioDep.Logger
          FeedItem = ()
          Data = Dictionary<string,obj>()
          InvocationNumber = 0
          StopScenario = fun (scnName,reason) -> StopScenario(scnName, reason) |> dep.ScenarioDep.ExecStopCommand
          StopCurrentTest = fun reason -> StopTest(reason) |> dep.ScenarioDep.ExecStopCommand }

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

module StepClientContext =

    let create (untyped: UntypedStepContext) (clientCount: int) = {
        new IClientInterceptionContext<'TFeedItem> with
            member _.StepName = untyped.StepName
            member _.StepInvocationNumber = untyped.InvocationNumber
            member _.ScenarioInfo = untyped.ScenarioInfo
            member _.Logger = untyped.Logger
            member _.Data = untyped.Data
            member _.FeedItem = untyped.FeedItem :?> 'TFeedItem
            member _.ClientCount = clientCount
    }

    let toUntyped (getClientNumber: IClientInterceptionContext<'TFeedItem> -> int) =
        fun (untyped: IClientInterceptionContext<obj>) ->
            let typed = {
                new IClientInterceptionContext<'TFeedItem> with
                    member _.StepName = untyped.StepName
                    member _.StepInvocationNumber = untyped.StepInvocationNumber
                    member _.ScenarioInfo = untyped.ScenarioInfo
                    member _.Logger = untyped.Logger
                    member _.Data = untyped.Data
                    member _.FeedItem = untyped.FeedItem :?> 'TFeedItem
                    member _.ClientCount = untyped.ClientCount
            }
            getClientNumber typed

module RunningStep =

    let create (dep: StepDep) (stepIndex: int) (step: Step) =
        { StepIndex = stepIndex; Value = step; Context = StepContext.createUntyped dep step }

    let getClient (context: UntypedStepContext) (clientPool: ClientPool option) =

        match clientPool with
        | Some pool ->
            let index = context.ScenarioInfo.ThreadNumber % pool.InitializedClients.Length
            pool.InitializedClients[index]

        | _ -> Unchecked.defaultof<_>

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
        context.Client <- getClient context st.ClientPool

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

    let execStep (dep: StepDep) (step: RunningStep) = backgroundTask {

        let! response = measureExec step dep.ScenarioDep.ScenarioTimer
        let payload = response.ClientResponse.Payload

        if not step.Value.DoNotTrack then
            dep.ScenarioDep.ScenarioStatsActor.Publish(AddResponse response)

            if response.ClientResponse.IsError then
                dep.ScenarioDep.Logger.Fatal($"Step '{step.Value.StepName}' from Scenario: '{dep.ScenarioInfo.ScenarioName}' has failed. Error: {response.ClientResponse.Message}")
            else
                dep.Data[Constants.StepResponseKey] <- payload

            return response.ClientResponse

        elif step.Value.IsPause then
            return response.ClientResponse

        else
            if response.ClientResponse.IsError then
                dep.ScenarioDep.Logger.Fatal($"Step '{step.Value.StepName}' from Scenario: '{dep.ScenarioInfo.ScenarioName}' has failed. Error: {response.ClientResponse.Message}")
            else
                dep.Data[Constants.StepResponseKey] <- payload

            return response.ClientResponse
    }

    let execRegularExec (dep: StepDep) (steps: RunningStep[]) (stepsOrder: int[]) = backgroundTask {
        let mutable shouldWork = true
        for stepIndex in stepsOrder do

            if shouldWork
               && not dep.ScenarioDep.ScenarioCancellationToken.IsCancellationRequested
               && dep.ScenarioInfo.ScenarioDuration.TotalMilliseconds > (dep.ScenarioDep.ScenarioTimer.Elapsed.TotalMilliseconds + Constants.SchedulerTimerDriftMs) then

                use cancelToken = new CancellationTokenSource()
                let step = updateContext steps[stepIndex] dep.Data cancelToken
                let! response = execStep dep step

                if response.IsError then
                    shouldWork <- false
    }

    let execSteps (dep: StepDep) (steps: RunningStep[]) (stepsOrder: int[]) =
        execRegularExec dep steps stepsOrder
