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
    | AddResponse of StepResponse
    | AddFromAgent of ScenarioStats
    | StartUseTempBuffer
    | FlushTempBuffer
    | BuildReportingStats of reply:TaskCompletionSource<ScenarioStats> * LoadSimulationStats * duration:TimeSpan
    | GetFinalStats       of reply:TaskCompletionSource<ScenarioStats> * LoadSimulationStats * duration:TimeSpan

type State = {
    Logger: ILogger
    Scenario: Scenario
    ReportingInterval: TimeSpan
    MergeStatsFn: (LoadSimulationStats -> ScenarioStats list -> ScenarioStats) option
    AllStepsData: StepStatsRawData[]
    mutable AllReportingStats: Map<TimeSpan,ScenarioStats>
    mutable ReportingStepsData: StepStatsRawData[]
    mutable ReportingAgentsStats: ScenarioStats list
    mutable ReportingTempBuffer: StepResponse list
    mutable UseReportingTempBuffer: bool
    mutable FinalAgentsStats: ScenarioStats list
    mutable ConsoleScenarioStats: ScenarioStats // need to display on console (we merge absolute request counts)

    mutable FailCount: int

    AllStepsResult: Dictionary<string,StepStatsRawData>
    ReportingStepsResult: Dictionary<string,StepStatsRawData>
}

let createState logger scenario reportingInterval mergeStatsFn = {
    Logger = logger
    Scenario = scenario
    ReportingInterval = reportingInterval
    MergeStatsFn = mergeStatsFn
    AllStepsData = Array.empty //Array.init scenario.Steps.Length (fun _ -> StepStatsRawData.createEmpty())
    ReportingStepsData = Array.empty //Array.init scenario.Steps.Length (fun _ -> StepStatsRawData.createEmpty())
    AllReportingStats = Map.empty
    ReportingAgentsStats = List.empty
    ReportingTempBuffer = List.empty
    UseReportingTempBuffer = false
    FinalAgentsStats = List.empty
    ConsoleScenarioStats = ScenarioStats.empty scenario
    FailCount = 0

    AllStepsResult = Dict.empty()
    ReportingStepsResult = Dict.empty()
    // ReportingTempBuffer
}

let addStepResult (state: State) (result: StepResult) =
    if state.UseReportingTempBuffer then
        ()
        //state.ReportingTempBuffer <- resp :: state.ReportingTempBuffer
    else
        let data = state.AllStepsResult[result.StepName]
        StepStatsRawData.addStepResult data result

        let reportingData = state.ReportingStepsResult[result.StepName]
        StepStatsRawData.addStepResult reportingData result

        if result.ClientResponse.IsError then
            state.FailCount <- state.FailCount + 1

let addResponse (state: State) (resp: StepResponse) =
    if state.UseReportingTempBuffer then
        state.ReportingTempBuffer <- resp :: state.ReportingTempBuffer
    else
        let allStData = state.AllStepsData[resp.StepIndex]
        let intervalStData = state.ReportingStepsData[resp.StepIndex]
        state.AllStepsData[resp.StepIndex] <- StepStatsRawData.addResponse allStData resp
        state.ReportingStepsData[resp.StepIndex] <- StepStatsRawData.addResponse intervalStData resp

        if resp.ClientResponse.IsError then
            state.FailCount <- state.FailCount + 1
    state

let flushTempBuffer (state: State) =
    state.UseReportingTempBuffer <- false
    state.ReportingTempBuffer |> List.iter(addResponse state >> ignore)
    state.ReportingTempBuffer <- List.empty
    state

let createReportingStats (state: State) (simulationStats) (duration) (stepsData) =
    ScenarioStats.create state.Scenario stepsData simulationStats OperationType.Bombing %duration state.ReportingInterval

let createFinalStats (state: State) (simulationStats) (duration) (stepsData) =
    ScenarioStats.create state.Scenario stepsData simulationStats OperationType.Complete %duration duration

let buildStats (state: State)
               (stepsData: StepStatsRawData[])
               (agentStats: ScenarioStats list)
               (simulationStats: LoadSimulationStats)
               (duration: TimeSpan)
               (isFinalStats: bool) =

    let cordStats =
        if isFinalStats then
            stepsData |> createFinalStats state simulationStats duration
        else
            stepsData |> createReportingStats state simulationStats duration

    let allStats =
        if state.Scenario.IsEnabled then cordStats :: agentStats
        else agentStats

    if allStats.Length > 0 then
        state.MergeStatsFn
        |> Option.map(fun merge -> merge simulationStats allStats)
        |> Option.defaultValue cordStats
    else
        cordStats

let mergeConsoleStats (latestReportingStats: ScenarioStats) (reportingStats: ScenarioStats) =

    // let mergedStepSteps =
    //     reportingStats.StepStats
    //     |> Array.mapi(fun i x ->
    //         let latestStep = latestReportingStats.StepStats[i]
    //         let ok = { x.Ok.Request with Count = x.Ok.Request.Count + latestStep.Ok.Request.Count }
    //         let fail = { x.Fail.Request with Count = x.Fail.Request.Count + latestStep.Fail.Request.Count }
    //         let allBytes = { x.Ok.DataTransfer with AllBytes = x.Ok.DataTransfer.AllBytes + latestStep.Ok.DataTransfer.AllBytes }
    //
    //         let okData = { x.Ok with Request = ok; DataTransfer = allBytes }
    //         let failData = { x.Fail with Request = fail }
    //
    //         { x with Ok = okData; Fail = failData }
    //     )
    //
    // { reportingStats with StepStats = mergedStepSteps }

    { reportingStats with
        OkCount = reportingStats.OkCount + latestReportingStats.OkCount
        FailCount = reportingStats.FailCount + latestReportingStats.FailCount
        RequestCount = reportingStats.RequestCount + latestReportingStats.RequestCount
        AllBytes = reportingStats.AllBytes + latestReportingStats.AllBytes }

let addReportingStats (state: State) (reportingStats: ScenarioStats) =
    state.AllReportingStats <- Map.add reportingStats.Duration reportingStats state.AllReportingStats
    // reset reporting interval steps data
    state.ReportingStepsData <- Array.empty //Array.init state.Scenario.Steps.Length (fun _ -> StepStatsRawData.createEmpty())
    state.ReportingAgentsStats <- List.empty
    state.ConsoleScenarioStats <- mergeConsoleStats state.ConsoleScenarioStats reportingStats
    state

let addStatsFromAgent (state: State) (agentStats: ScenarioStats) =
    if agentStats.CurrentOperation = OperationType.Bombing then
        state.ReportingAgentsStats <- agentStats :: state.ReportingAgentsStats
    else
        state.FinalAgentsStats <- agentStats :: state.FinalAgentsStats
    state

type ScenarioStatsActor(logger: ILogger,
                        scenario: Scenario,
                        reportingInterval: TimeSpan,
                        ?mergeStatsFn: LoadSimulationStats -> ScenarioStats list -> ScenarioStats) =

    let mutable _state = createState logger scenario reportingInterval mergeStatsFn

    let _actor = ActionBlock(fun msg ->
        try
            match msg with
            | AddStepResult result ->
                if _state.AllStepsResult.ContainsKey result.StepName then
                    addStepResult _state result
                else
                    _state.AllStepsResult[result.StepName] <- StepStatsRawData.empty(result.StepName)
                    _state.ReportingStepsResult[result.StepName] <- StepStatsRawData.empty(result.StepName)
                    addStepResult _state result

            | AddResponse response ->
                _state <- addResponse _state response

            | AddFromAgent agentStats ->
                _state <- addStatsFromAgent _state agentStats

            | StartUseTempBuffer ->
                _state.UseReportingTempBuffer <- true

            | FlushTempBuffer ->
                _state <- flushTempBuffer _state

            | BuildReportingStats (reply, simulationStats, duration) ->
                let isFinalStats = false

                let reportingStats = _state.ReportingStepsResult.Values |> Seq.toArray

                let stats = buildStats _state reportingStats _state.ReportingAgentsStats simulationStats duration isFinalStats
                //let stats = buildStats _state _state.ReportingStepsData _state.ReportingAgentsStats simulationStats duration isFinalStats
                _state <- addReportingStats _state stats
                reply.TrySetResult(stats) |> ignore

            | GetFinalStats (reply, simulationStats, duration) ->
                let isFinalStats = true

                let allStats = _state.AllStepsResult.Values |> Seq.toArray

                let stats = buildStats _state allStats _state.FinalAgentsStats simulationStats duration isFinalStats
                reply.TrySetResult(stats) |> ignore
        with
        | ex -> _state.Logger.Error $"{nameof ScenarioStatsActor} failed: {ex.ToString()}"
    )

    member _.FailCount = _state.FailCount
    member _.AllRealtimeStats = _state.AllReportingStats
    member _.MergedReportingStats = _state.ConsoleScenarioStats

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.Publish(msg) = _actor.Post(msg) |> ignore

let createDefault (logger: ILogger) (scenario: Scenario) (reportingInterval: TimeSpan) =
    ScenarioStatsActor(logger, scenario, reportingInterval)

let createForCoordinator (mergeStats: LoadSimulationStats -> ScenarioStats list -> ScenarioStats)
                         (logger: ILogger) (scenario: Scenario) (reportingInterval: TimeSpan) =
    ScenarioStatsActor(logger, scenario, reportingInterval, mergeStats)
