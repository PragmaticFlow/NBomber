module internal NBomber.Domain.Stats.ScenarioStatsActor

open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

open FSharp.UMX
open NBomber.Extensions.Internal
open Serilog

open NBomber.Contracts.Stats
open NBomber.Contracts.Internal
open NBomber.Domain.DomainTypes
open NBomber.Domain.Stats.Statistics
open NBomber.Domain.Stats.StepStatsRawData

type ActorMessage =
    | AddStepResult of StepResult
    | AddFromAgent of ScenarioStats
    | StartUseTempBuffer
    | FlushTempBuffer
    | BuildReportingStats of reply:TaskCompletionSource<ScenarioStats> * LoadSimulationStats * duration:TimeSpan
    | GetFinalStats       of reply:TaskCompletionSource<ScenarioStats> * LoadSimulationStats * duration:TimeSpan

type State = {
    Logger: ILogger
    Scenario: Scenario
    ReportingInterval: TimeSpan
    MergeStatsFn: (LoadSimulationStats -> ScenarioStats seq -> ScenarioStats) option
    mutable FailCount: int

    mutable ReportingStatsCache: Map<TimeSpan,ScenarioStats>

    CoordinatorStepsResults: Dictionary<string,StepStatsRawData>
    IntervalStepsResults: Dictionary<string,StepStatsRawData>
    mutable ConsoleScenarioStats: ScenarioStats // need to display on console (we merge absolute request counts)

    /// agents stats
    mutable UseTempBuffer: bool
    TempBuffer: ResizeArray<StepResult>
    IntervalAgentsStats: ResizeArray<ScenarioStats>
    FinalAgentsStats: ResizeArray<ScenarioStats>
}

let createState logger scenario reportingInterval mergeStatsFn = {
    Logger = logger
    Scenario = scenario
    ReportingInterval = reportingInterval
    MergeStatsFn = mergeStatsFn
    FailCount = 0

    ReportingStatsCache = Map.empty

    CoordinatorStepsResults = Dict.empty()
    IntervalStepsResults = Dict.empty()
    ConsoleScenarioStats = ScenarioStats.empty scenario

    UseTempBuffer = false
    TempBuffer = ResizeArray<_>()
    IntervalAgentsStats = ResizeArray<_>()
    FinalAgentsStats = ResizeArray<_>()
}

let updateCoordinatorStats (state: State) (result: StepResult) =
    let rawStats = state.CoordinatorStepsResults[result.StepName]
    StepStatsRawData.addStepResult rawStats result

let updateIntervalStats (state: State) (result: StepResult) =
    let rawStats = state.IntervalStepsResults[result.StepName]
    StepStatsRawData.addStepResult rawStats result

let addStepResult (state: State) (result: StepResult) =
    if state.UseTempBuffer then
        state.TempBuffer.Add result
    else
        if state.CoordinatorStepsResults.ContainsKey result.StepName then
            updateCoordinatorStats state result
        else
            state.CoordinatorStepsResults[result.StepName] <- StepStatsRawData.empty result.StepName
            updateCoordinatorStats state result

        if state.IntervalStepsResults.ContainsKey result.StepName then
            updateIntervalStats state result
        else
            state.IntervalStepsResults[result.StepName] <- StepStatsRawData.empty result.StepName
            updateIntervalStats state result

        if result.ClientResponse.IsError then
            state.FailCount <- state.FailCount + 1

let flushTempBuffer (state: State) =
    state.UseTempBuffer <- false
    state.TempBuffer |> Seq.iter(addStepResult state)
    state.TempBuffer.Clear()
    state

let createReportingStats (state: State) (simulationStats) (duration) (stepsData) =
    ScenarioStats.create state.Scenario stepsData simulationStats OperationType.Bombing %duration state.ReportingInterval

let createFinalStats (state: State) (simulationStats) (duration) (stepsData) =
    ScenarioStats.create state.Scenario stepsData simulationStats OperationType.Complete %duration duration

let buildStats (state: State)
               (cordStepsData: StepStatsRawData[])
               (agentStats: ResizeArray<ScenarioStats>)
               (simulationStats: LoadSimulationStats)
               (duration: TimeSpan)
               (isFinalStats: bool) =

    let cordStats =
        if isFinalStats then
            cordStepsData |> createFinalStats state simulationStats duration
        else
            cordStepsData |> createReportingStats state simulationStats duration

    if state.Scenario.IsEnabled then
        agentStats.Add cordStats

    if agentStats.Count > 0 then
        state.MergeStatsFn
        |> Option.map(fun merge -> merge simulationStats agentStats)
        |> Option.defaultValue cordStats
    else
        cordStats

let mergeConsoleStats (latestReportingStats: ScenarioStats) (reportingStats: ScenarioStats) =
    { reportingStats with
        OkCount = reportingStats.OkCount + latestReportingStats.OkCount
        FailCount = reportingStats.FailCount + latestReportingStats.FailCount
        RequestCount = reportingStats.RequestCount + latestReportingStats.RequestCount
        AllBytes = reportingStats.AllBytes + latestReportingStats.AllBytes }

let addReportingStats (state: State) (reportingStats: ScenarioStats) =
    state.ReportingStatsCache <- Map.add reportingStats.Duration reportingStats state.ReportingStatsCache
    state.ConsoleScenarioStats <- mergeConsoleStats state.ConsoleScenarioStats reportingStats

    // reset reporting interval steps data
    state.IntervalStepsResults.Clear()
    state.IntervalAgentsStats.Clear()

    state

let addStatsFromAgent (state: State) (agentStats: ScenarioStats) =
    if agentStats.CurrentOperation = OperationType.Bombing then
        state.IntervalAgentsStats.Add agentStats
    else
        state.FinalAgentsStats.Add agentStats

type ScenarioStatsActor(logger: ILogger,
                        scenario: Scenario,
                        reportingInterval: TimeSpan,
                        ?mergeStatsFn: LoadSimulationStats -> ScenarioStats seq -> ScenarioStats) =

    let mutable _state = createState logger scenario reportingInterval mergeStatsFn

    let _actor = ActionBlock(fun msg ->
        try
            match msg with
            | AddStepResult result ->
                addStepResult _state result

            | AddFromAgent agentStats ->
                addStatsFromAgent _state agentStats

            | StartUseTempBuffer ->
                _state.UseTempBuffer <- true

            | FlushTempBuffer ->
                _state <- flushTempBuffer _state

            | BuildReportingStats (reply, simulationStats, duration) ->
                let isFinalStats = false
                let reportingStats = _state.IntervalStepsResults.Values |> Seq.toArray

                let stats = buildStats _state reportingStats _state.IntervalAgentsStats simulationStats duration isFinalStats

                _state <- addReportingStats _state stats
                reply.TrySetResult(stats) |> ignore

            | GetFinalStats (reply, simulationStats, duration) ->
                let isFinalStats = true

                let cordStats = _state.CoordinatorStepsResults.Values |> Seq.toArray

                let stats = buildStats _state cordStats _state.FinalAgentsStats simulationStats duration isFinalStats
                reply.TrySetResult(stats) |> ignore
        with
        | ex -> _state.Logger.Error $"{nameof ScenarioStatsActor} failed: {ex.ToString()}"
    )

    member _.FailCount = _state.FailCount
    member _.AllRealtimeStats = _state.ReportingStatsCache
    member _.MergedReportingStats = _state.ConsoleScenarioStats

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.Publish(msg) = _actor.Post(msg) |> ignore

let createDefault (logger: ILogger) (scenario: Scenario) (reportingInterval: TimeSpan) =
    ScenarioStatsActor(logger, scenario, reportingInterval)

let createForCoordinator (mergeStats: LoadSimulationStats -> ScenarioStats seq -> ScenarioStats)
                         (logger: ILogger) (scenario: Scenario) (reportingInterval: TimeSpan) =
    ScenarioStatsActor(logger, scenario, reportingInterval, mergeStats)
