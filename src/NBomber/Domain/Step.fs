[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Step

open System
open System.Collections.Generic
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Serilog
open FSharp.Control.Tasks.NonAffine

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Internal
open NBomber.Domain.DomainTypes
open NBomber.Domain.ClientPool
open NBomber.Domain.Stats.ScenarioStatsActor

type StepDep = {
    Scenario: Scenario
    ScenarioInfo: ScenarioInfo
    Logger: ILogger
    CancellationToken: CancellationToken
    ScenarioGlobalTimer: Stopwatch
    ExecStopCommand: StopCommand -> unit
    ScenarioStatsActor: IScenarioStatsActor
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
          Logger = dep.Logger
          FeedItem = ()
          Data = Dictionary<string,obj>()
          InvocationCount = 0
          StopScenario = fun (scnName,reason) -> StopScenario(scnName, reason) |> dep.ExecStopCommand
          StopCurrentTest = fun reason -> StopTest(reason) |> dep.ExecStopCommand }

    let inline create (untyped: UntypedStepContext) = {
        new IStepContext<'TClient,'TFeedItem> with
            member _.StepName = untyped.StepName
            member _.ScenarioInfo = untyped.ScenarioInfo
            member _.CancellationToken = untyped.CancellationTokenSource.Token
            member _.Client = untyped.Client :?> 'TClient
            member _.Data = untyped.Data
            member _.FeedItem = untyped.FeedItem :?> 'TFeedItem
            member _.Logger = untyped.Logger
            member _.InvocationCount = untyped.InvocationCount
            member _.GetPreviousStepResponse() = getPreviousStepResponse(untyped.Data)
            member _.StopScenario(scenarioName, reason) = untyped.StopScenario(scenarioName, reason)
            member _.StopCurrentTest(reason) = untyped.StopCurrentTest(reason)
    }

    let toUntypedExecute (execute: IStepContext<'TClient,'TFeedItem> -> Task<Response>) =
        fun (untyped: UntypedStepContext) ->
            let typed = create untyped
            execute typed

module StepClientContext =

    let inline create (untyped: UntypedStepContext) (clientCount: int) = {
        new IStepClientContext<'TFeedItem> with
            member _.StepName = untyped.StepName
            member _.ScenarioInfo = untyped.ScenarioInfo
            member _.Logger = untyped.Logger
            member _.Data = untyped.Data
            member _.FeedItem = untyped.FeedItem :?> 'TFeedItem
            member _.InvocationCount = untyped.InvocationCount
            member _.ClientCount = clientCount
    }

    let toUntyped (getClientNumber: IStepClientContext<'TFeedItem> -> int) =
        fun (untyped: IStepClientContext<obj>) ->
            let typed = {
                new IStepClientContext<'TFeedItem> with
                    member _.StepName = untyped.StepName
                    member _.ScenarioInfo = untyped.ScenarioInfo
                    member _.Logger = untyped.Logger
                    member _.Data = untyped.Data
                    member _.FeedItem = untyped.FeedItem :?> 'TFeedItem
                    member _.InvocationCount = untyped.InvocationCount
                    member _.ClientCount = untyped.ClientCount
            }
            getClientNumber typed

module RunningStep =

    let create (dep: StepDep) (stepIndex: int) (step: Step) =
        { StepIndex = stepIndex; Value = step; Context = StepContext.createUntyped dep step }

    let getClient (context: UntypedStepContext)
                  (clientPool: ClientPool option)
                  (clientDistribution: (IStepClientContext<obj> -> int) option) =

        match clientPool, clientDistribution with
        | Some pool, Some getClientIndex ->
            let ctx = StepClientContext.create context pool.ClientCount
            let index = getClientIndex ctx
            pool.InitializedClients[index]

        | Some pool, None ->
            let index = context.ScenarioInfo.ThreadNumber % pool.InitializedClients.Length
            pool.InitializedClients[index]

        | _, _ -> Unchecked.defaultof<_>

    let updateContext (step: RunningStep) (data: Dictionary<string,obj>) =
        let st = step.Value
        let context = step.Context

        let feedItem =
            match step.Value.Feed with
            | Some feed -> feed.GetNextItem(context.ScenarioInfo, data)
            | None      -> Unchecked.defaultof<_>

        context.CancellationTokenSource <- new CancellationTokenSource()
        context.InvocationCount <- context.InvocationCount + 1
        context.Data <- data
        context.FeedItem <- feedItem
        // context.Client should be set as the last field because init order matter here
        context.Client <- getClient context st.ClientPool st.ClientDistribution

        step

    let measureExec (step: RunningStep) (globalTimer: Stopwatch) = task {
        let startTime = globalTimer.Elapsed.TotalMilliseconds
        try
            let responseTask = step.Value.Execute(step.Context)

            // for pause we skip timeout logic
            if step.Value.StepName = Constants.StepPauseName then
                let! pause = responseTask
                return { StepIndex = step.StepIndex; ClientResponse = pause; EndTimeMs = 0.0; LatencyMs = 0.0 }
            else
                let! finishedTask = Task.WhenAny(responseTask, Task.Delay(step.Value.Timeout, step.Context.CancellationTokenSource.Token))
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

    let execStep (dep: StepDep) (step: RunningStep) = task {

        let! response = measureExec step dep.ScenarioGlobalTimer

        if not dep.CancellationToken.IsCancellationRequested && not step.Value.DoNotTrack
            && dep.ScenarioInfo.ScenarioDuration.TotalMilliseconds >= response.EndTimeMs then

                dep.ScenarioStatsActor.Publish(AddResponse response)

                if response.ClientResponse.IsError then
                    dep.Logger.Fatal($"Step '{step.Value.StepName}' from Scenario: '{dep.ScenarioInfo.ScenarioName}' has failed. Error: {response.ClientResponse.Message}")
                else
                    dep.Data[Constants.StepResponseKey] <- response.ClientResponse.Payload

                return ValueSome response.ClientResponse
        else
            return ValueNone
    }

    let execCustomExec (dep: StepDep) (steps: RunningStep[]) (execControl: IStepExecControlContext voption -> string voption) = task {
        let mutable stop = false
        let mutable execContext = ValueNone
        while stop || not dep.CancellationToken.IsCancellationRequested do
            let nextStep = execControl execContext
            match nextStep with
            | ValueSome stepName ->
                let stepIndex = dep.Scenario.StepOrderIndex[stepName]
                let step = updateContext steps[stepIndex] dep.Data
                let! response = execStep dep step

                match response with
                | ValueSome resp ->
                    execContext <- ValueSome {
                        new IStepExecControlContext with
                            member _.PrevStepContext = StepContext.create step.Context
                            member _.PrevStepResponse = resp
                    }

                | ValueNone -> stop <- true

            | ValueNone -> stop <- true
    }

    let execRegularExec (dep: StepDep) (steps: RunningStep[]) (stepsOrder: int[]) = task {
        let mutable stop = false
        for stepIndex in stepsOrder do
            if not stop && not dep.CancellationToken.IsCancellationRequested then
                let step = updateContext steps[stepIndex] dep.Data
                let! response = execStep dep step
                match response with
                | ValueSome r when r.IsError -> stop <- true
                | ValueSome _ -> ()
                | ValueNone   -> stop <- true
    }

    let execSteps (dep: StepDep) (steps: RunningStep[]) (stepsOrder: int[]) =
        match dep.Scenario.CustomStepExecControl with
        | Some execControl -> execCustomExec dep steps execControl
        | None             -> execRegularExec dep steps stepsOrder
