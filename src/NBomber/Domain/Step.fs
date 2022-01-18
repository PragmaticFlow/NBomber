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

type StepDep = {
    ScenarioInfo: ScenarioInfo
    Logger: ILogger
    CancellationToken: CancellationToken
    ScenarioGlobalTimer: Stopwatch
    ExecStopCommand: StopCommand -> unit
}

module StepContext =

    let create (dep: StepDep) (step: Step) =

        let getClient (pool: ClientPool option) =
            match pool with
            | Some v ->
                let index = dep.ScenarioInfo.ThreadNumber % v.InitializedClients.Length
                v.InitializedClients[index]

            | None -> Unchecked.defaultof<_>

        { ScenarioInfo = dep.ScenarioInfo
          CancellationToken = dep.CancellationToken
          Client = getClient step.ClientPool
          Logger = dep.Logger
          FeedItem = Unchecked.defaultof<_>
          Data = Dictionary<string,obj>()
          InvocationCount = 0
          StopScenario = fun (scnName,reason) -> StopScenario(scnName, reason) |> dep.ExecStopCommand
          StopCurrentTest = fun reason -> StopTest(reason) |> dep.ExecStopCommand }

module StepClientContext =

    let toUntyped (getClientNumber: IStepClientContext<'TFeedItem> -> int) =

        fun (untyped: IStepClientContext<obj>) ->

            let typed = {
                new IStepClientContext<'TFeedItem> with
                    member _.ScenarioInfo = untyped.ScenarioInfo
                    member _.Logger = untyped.Logger
                    member _.Data = untyped.Data
                    member _.FeedItem = untyped.FeedItem :?> 'TFeedItem
                    member _.InvocationCount = untyped.InvocationCount
                    member _.ClientCount = untyped.ClientCount
            }

            getClientNumber typed

    let create (context: UntypedStepContext) (clientCount: int) = {
        new IStepClientContext<obj> with
            member _.ScenarioInfo = context.ScenarioInfo
            member _.Logger = context.Logger
            member _.Data = context.Data
            member _.FeedItem = context.FeedItem
            member _.InvocationCount = context.InvocationCount
            member _.ClientCount = clientCount
    }


module RunningStep =

    let create (dep: StepDep) (step: Step) =
        { Value = step; Context = StepContext.create dep step }

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

        context.InvocationCount <- context.InvocationCount + 1
        context.Data <- data
        context.FeedItem <- feedItem
        // context.Client should be set as the last field because init order matter here
        context.Client <- getClient context st.ClientPool st.ClientDistribution

        step

let toUntypedExecute (execute: IStepContext<'TClient,'TFeedItem> -> Response) =

    fun (untyped: UntypedStepContext) ->

        let typed = {
            new IStepContext<'TClient,'TFeedItem> with
                member _.ScenarioInfo = untyped.ScenarioInfo
                member _.CancellationToken = untyped.CancellationToken
                member _.Client = untyped.Client :?> 'TClient
                member _.Data = untyped.Data
                member _.FeedItem = untyped.FeedItem :?> 'TFeedItem
                member _.Logger = untyped.Logger
                member _.InvocationCount = untyped.InvocationCount
                member _.StopScenario(scenarioName, reason) = untyped.StopScenario(scenarioName, reason)
                member _.StopCurrentTest(reason) = untyped.StopCurrentTest(reason)

                member _.GetPreviousStepResponse() =
                    try
                        let prevStepResponse = untyped.Data[Constants.StepResponseKey]
                        if isNull prevStepResponse then
                            Unchecked.defaultof<'T>
                        else
                            prevStepResponse :?> 'T
                    with
                    | ex -> Unchecked.defaultof<'T>
        }

        execute typed

let toUntypedExecuteAsync (execute: IStepContext<'TClient,'TFeedItem> -> Task<Response>) =

    fun (untyped: UntypedStepContext) ->

        let typed = {
            new IStepContext<'TClient,'TFeedItem> with
                member _.ScenarioInfo = untyped.ScenarioInfo
                member _.CancellationToken = untyped.CancellationToken
                member _.Client = untyped.Client :?> 'TClient
                member _.Data = untyped.Data
                member _.FeedItem = untyped.FeedItem :?> 'TFeedItem
                member _.Logger = untyped.Logger
                member _.InvocationCount = untyped.InvocationCount
                member _.StopScenario(scenarioName, reason) = untyped.StopScenario(scenarioName, reason)
                member _.StopCurrentTest(reason) = untyped.StopCurrentTest(reason)

                member _.GetPreviousStepResponse() =
                    try
                        let prevStepResponse = untyped.Data[Constants.StepResponseKey]
                        if isNull prevStepResponse then
                            Unchecked.defaultof<'T>
                        else
                            prevStepResponse :?> 'T
                    with
                    | ex -> Unchecked.defaultof<'T>
        }

        execute typed

let execStep (step: RunningStep) (globalTimer: Stopwatch) =
    let startTime = globalTimer.Elapsed.TotalMilliseconds
    try
        let resp =
            match step.Value.Execute with
            | SyncExec exec  -> exec step.Context
            | AsyncExec exec -> (exec step.Context).Result

        let endTime = globalTimer.Elapsed.TotalMilliseconds
        let latency = endTime - startTime

        { ClientResponse = resp; EndTimeMs = endTime; LatencyMs = latency }
    with
    | :? TaskCanceledException
    | :? OperationCanceledException ->
        let endTime = globalTimer.Elapsed.TotalMilliseconds
        let latency = endTime - startTime
        { ClientResponse = Response.fail(statusCode = Constants.TimeoutStatusCode, error = "step timeout")
          EndTimeMs = endTime
          LatencyMs = latency }
    | ex ->
        let endTime = globalTimer.Elapsed.TotalMilliseconds
        let latency = endTime - startTime
        { ClientResponse = Response.fail(statusCode = Constants.StepUnhandledErrorCode,
                                         error = $"step unhandled exception: {ex.Message}")
          EndTimeMs = endTime
          LatencyMs = latency }

let execStepAsync (step: RunningStep) (globalTimer: Stopwatch) = task {
    let startTime = globalTimer.Elapsed.TotalMilliseconds
    try
        let responseTask =
            match step.Value.Execute with
            | SyncExec exec  -> Task.FromResult(exec step.Context)
            | AsyncExec exec -> exec step.Context

        // for pause we skip timeout logic
        if step.Value.StepName = Constants.StepPauseName then
            let! pause = responseTask
            return { ClientResponse = pause; EndTimeMs = 0; LatencyMs = 0 }
        else
            let! finishedTask = Task.WhenAny(responseTask, Task.Delay(step.Value.Timeout, step.Context.CancellationToken))
            let endTime = globalTimer.Elapsed.TotalMilliseconds
            let latency = endTime - startTime

            if finishedTask.Id = responseTask.Id then
                return { ClientResponse = responseTask.Result
                         EndTimeMs = endTime
                         LatencyMs = latency }
            else
                return { ClientResponse = Response.fail(statusCode = Constants.TimeoutStatusCode, error = $"step timeout: {step.Value.Timeout.TotalMilliseconds} ms")
                         EndTimeMs = endTime
                         LatencyMs = latency }
    with
    | :? TaskCanceledException
    | :? OperationCanceledException ->
        let endTime = globalTimer.Elapsed.TotalMilliseconds
        let latency = endTime - startTime
        return { ClientResponse = Response.fail(statusCode = Constants.TimeoutStatusCode, error = "step timeout")
                 EndTimeMs = endTime
                 LatencyMs = latency }
    | ex ->
        let endTime = globalTimer.Elapsed.TotalMilliseconds
        let latency = endTime - startTime
        return { ClientResponse = Response.fail(statusCode = Constants.StepUnhandledErrorCode,
                                                error = $"step unhandled exception: {ex.Message}")
                 EndTimeMs = endTime
                 LatencyMs = latency }
}

let execSteps (dep: StepDep,
               steps: RunningStep[],
               stepsOrder: int[],
               responseBuffer: ResizeArray<int * StepResponse>,
               stepDataDict: Dictionary<string,obj>) =

    let mutable skipStep = false

    for stepIndex in stepsOrder do
        if not skipStep && not dep.CancellationToken.IsCancellationRequested then
            try
                let step = RunningStep.updateContext steps[stepIndex] stepDataDict
                let response = execStep step dep.ScenarioGlobalTimer

                if not dep.CancellationToken.IsCancellationRequested && not step.Value.DoNotTrack
                    && dep.ScenarioInfo.ScenarioDuration.TotalMilliseconds >= response.EndTimeMs then

                        responseBuffer.Add(stepIndex, response)

                        match response.ClientResponse.IsError with
                        | true ->
                            dep.Logger.Fatal($"Step '{step.Value.StepName}' from Scenario: '{dep.ScenarioInfo.ScenarioName}' has failed. Error: {response.ClientResponse.Message}.")
                            skipStep <- true

                        | false ->
                            stepDataDict[Constants.StepResponseKey] <- response.ClientResponse.Payload
            with
            | ex ->
                dep.Logger.Fatal(ex, $"Step with index '{stepIndex}' from Scenario: '{dep.ScenarioInfo.ScenarioName}' has failed.")
                let response = Response.fail(error = $"step internal client exception, stepIndex: {stepIndex}, error: {ex.Message}", statusCode = Constants.StepInternalClientErrorCode)
                responseBuffer.Add(stepIndex, { ClientResponse = response; EndTimeMs = 0; LatencyMs = 0 })

let execStepsAsync (dep: StepDep,
                    steps: RunningStep[],
                    stepsOrder: int[],
                    responseBuffer: ResizeArray<int * StepResponse>,
                    stepDataDict: Dictionary<string,obj>) = task {

    let mutable skipStep = false

    for stepIndex in stepsOrder do
        if not skipStep && not dep.CancellationToken.IsCancellationRequested then
            try
                let step = RunningStep.updateContext steps[stepIndex] stepDataDict
                let! response = execStepAsync step dep.ScenarioGlobalTimer

                if not dep.CancellationToken.IsCancellationRequested && not step.Value.DoNotTrack
                    && dep.ScenarioInfo.ScenarioDuration.TotalMilliseconds >= response.EndTimeMs then

                        responseBuffer.Add(stepIndex, response)

                        match response.ClientResponse.IsError with
                        | true ->
                            dep.Logger.Fatal($"Step: '{step.Value.StepName}' from Scenario: '{dep.ScenarioInfo.ScenarioName}' has failed. Error: {response.ClientResponse.Message}.")
                            skipStep <- true

                        | false ->
                            stepDataDict[Constants.StepResponseKey] <- response.ClientResponse.Payload
            with
            | ex ->
                dep.Logger.Fatal(ex, $"Step with index '{stepIndex}' from Scenario: '{dep.ScenarioInfo.ScenarioName}' has failed.")
                let response = Response.fail(error = $"step internal client exception, stepIndex: {stepIndex}, error: {ex.Message}", statusCode = Constants.StepInternalClientErrorCode)
                responseBuffer.Add(stepIndex, { ClientResponse = response; EndTimeMs = 0; LatencyMs = 0 })
}

let isAllExecSync (steps: Step list) =
    steps
    |> List.map(fun x -> x.Execute)
    |> List.forall(function SyncExec _ -> true | AsyncExec _ -> false)

