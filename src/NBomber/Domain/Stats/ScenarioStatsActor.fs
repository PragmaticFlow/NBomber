module internal NBomber.Domain.Stats.ScenarioStatsActor

open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

open FSharp.UMX
open Serilog

open NBomber
open NBomber.Contracts.Stats
open NBomber.Contracts.Internal
open NBomber.Extensions.Internal
open NBomber.Domain.DomainTypes
open NBomber.Domain.Stats.Statistics
open NBomber.Domain.Stats.RawMeasurementStats

type ActorMessage =
    | AddMeasurement of Measurement
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
    mutable ScenarioFailCount: int
    StepsOrder: Dictionary<string, int>

    mutable ReportingStatsCache: Map<TimeSpan,ScenarioStats>
    CoordinatorStepsResults: Dictionary<string,RawMeasurementStats>
    IntervalStepsResults: Dictionary<string,RawMeasurementStats>
    mutable ConsoleScenarioStats: ScenarioStats // need to display on console (we merge absolute request counts)

    /// agents stats
    mutable UseTempBuffer: bool
    TempBuffer: ResizeArray<Measurement>
    IntervalAgentsStats: ResizeArray<ScenarioStats>
    FinalAgentsStats: ResizeArray<ScenarioStats>
}

let createState logger scenario reportingInterval mergeStatsFn =
    let emptyScnStats = ScenarioStats.empty scenario
    let globalInfoStep = StepStats.extractGlobalInfoStep emptyScnStats
    let consoleStats = { emptyScnStats with StepStats = emptyScnStats.StepStats |> Array.append [|globalInfoStep|] }

    let stepsOrder = Dict.empty()
    stepsOrder[Constants.ScenarioGlobalInfo] <- 0

    { Logger = logger
      Scenario = scenario
      ReportingInterval = reportingInterval
      MergeStatsFn = mergeStatsFn
      ScenarioFailCount = 0
      StepsOrder = stepsOrder

      ReportingStatsCache = Map.empty

      CoordinatorStepsResults = Dict.empty()
      IntervalStepsResults = Dict.empty()
      ConsoleScenarioStats = consoleStats

      UseTempBuffer = false
      TempBuffer = ResizeArray<_>()
      IntervalAgentsStats = ResizeArray<_>()
      FinalAgentsStats = ResizeArray<_>() }

let updateCoordinatorStats (state: State) (measurement: Measurement) =
    let rawStats = state.CoordinatorStepsResults[measurement.Name]
    RawMeasurementStats.addMeasurement rawStats measurement

let updateIntervalStats (state: State) (measurement: Measurement) =
    let rawStats = state.IntervalStepsResults[measurement.Name]
    RawMeasurementStats.addMeasurement rawStats measurement

let addMeasurement (state: State) (measurement: Measurement) =
    if state.UseTempBuffer then
        state.TempBuffer.Add measurement
    else
        if state.CoordinatorStepsResults.ContainsKey measurement.Name then
            updateCoordinatorStats state measurement
        else
            state.CoordinatorStepsResults[measurement.Name] <- RawMeasurementStats.empty measurement.Name

            if not (state.StepsOrder.ContainsKey measurement.Name) then
                state.StepsOrder[measurement.Name] <- state.StepsOrder.Count

            updateCoordinatorStats state measurement

        if state.IntervalStepsResults.ContainsKey measurement.Name then
            updateIntervalStats state measurement
        else
            state.IntervalStepsResults[measurement.Name] <- RawMeasurementStats.empty measurement.Name
            updateIntervalStats state measurement

        if measurement.ClientResponse.IsError && measurement.Name = Constants.ScenarioGlobalInfo then
            state.ScenarioFailCount <- state.ScenarioFailCount + 1

let flushTempBuffer (state: State) =
    state.UseTempBuffer <- false
    state.TempBuffer |> Seq.iter(addMeasurement state)
    state.TempBuffer.Clear()

let createReportingStats (state: State) (simulationStats) (duration) (rawStats) =
    ScenarioStats.create state.Scenario.ScenarioName rawStats simulationStats OperationType.Bombing %duration state.ReportingInterval

let createFinalStats (state: State) (simulationStats) (duration) (rawStats) =
    ScenarioStats.create state.Scenario.ScenarioName rawStats simulationStats OperationType.Complete %duration duration

let buildStats (state: State)
               (cordRawStats: RawMeasurementStats[])
               (agentStats: ResizeArray<ScenarioStats>)
               (simulationStats: LoadSimulationStats)
               (duration: TimeSpan)
               (isFinalStats: bool) =

    cordRawStats |> Array.sortInPlaceBy(fun x -> state.StepsOrder[x.Name])

    let cordStats =
        if isFinalStats then
            cordRawStats |> createFinalStats state simulationStats duration
        else
            cordRawStats |> createReportingStats state simulationStats duration

    if state.Scenario.IsEnabled then
        agentStats.Add cordStats

    if agentStats.Count > 0 then
        state.MergeStatsFn
        |> Option.map(fun merge -> merge simulationStats agentStats)
        |> Option.defaultValue cordStats
    else
        cordStats

let mergeConsoleStats (consoleStats: ScenarioStats) (reportingStats: ScenarioStats) =
    let globalInfoStep = StepStats.extractGlobalInfoStep reportingStats

    let updatedSteps =
        reportingStats.StepStats
        |> Seq.append [globalInfoStep]
        |> Seq.map(fun newStepStats ->

            let consoleStep = consoleStats.StepStats |> Array.tryFind(fun pSt -> pSt.StepName = newStepStats.StepName)
            match consoleStep with
            | Some console ->
                let okReq = { newStepStats.Ok.Request with Count = newStepStats.Ok.Request.Count + console.Ok.Request.Count }
                let failReq = { newStepStats.Fail.Request with Count = newStepStats.Fail.Request.Count + console.Fail.Request.Count }

                let okData = { newStepStats.Ok.DataTransfer with AllBytes = newStepStats.Ok.DataTransfer.AllBytes + console.Ok.DataTransfer.AllBytes }
                let failData = { newStepStats.Fail.DataTransfer with AllBytes = newStepStats.Fail.DataTransfer.AllBytes + console.Fail.DataTransfer.AllBytes }

                let ok = { newStepStats.Ok with Request = okReq; DataTransfer = okData }
                let fail = { newStepStats.Fail with Request = failReq; DataTransfer = failData }

                { newStepStats with Ok = ok; Fail = fail }

            | None -> newStepStats
        )
        |> Seq.toArray

    { reportingStats with StepStats = updatedSteps }

let addReportingStats (state: State) (reportingStats: ScenarioStats) =
    state.ReportingStatsCache <- Map.add reportingStats.Duration reportingStats state.ReportingStatsCache
    state.ConsoleScenarioStats <- mergeConsoleStats state.ConsoleScenarioStats reportingStats

    // reset reporting interval steps data
    state.IntervalStepsResults.Clear()
    state.IntervalAgentsStats.Clear()

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
            | AddMeasurement result ->
                addMeasurement _state result

            | AddFromAgent agentStats ->
                addStatsFromAgent _state agentStats

            | StartUseTempBuffer ->
                _state.UseTempBuffer <- true

            | FlushTempBuffer ->
                flushTempBuffer _state

            | BuildReportingStats (reply, simulationStats, duration) ->
                let isFinalStats = false
                let reportingStats = _state.IntervalStepsResults.Values |> Seq.toArray

                let stats = buildStats _state reportingStats _state.IntervalAgentsStats simulationStats duration isFinalStats

                addReportingStats _state stats
                reply.TrySetResult(stats) |> ignore

            | GetFinalStats (reply, simulationStats, duration) ->
                let isFinalStats = true

                let cordStats = _state.CoordinatorStepsResults.Values |> Seq.toArray

                let stats = buildStats _state cordStats _state.FinalAgentsStats simulationStats duration isFinalStats
                reply.TrySetResult(stats) |> ignore
        with
        | ex -> _state.Logger.Fatal $"Unhandled exception: {nameof ScenarioStatsActor} failed: {ex.ToString()}"
    )

    member _.ScenarioFailCount = _state.ScenarioFailCount
    member _.AllRealtimeStats = _state.ReportingStatsCache
    member _.ConsoleScenarioStats = _state.ConsoleScenarioStats

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.Publish(msg) = _actor.Post(msg) |> ignore

let createDefault (logger: ILogger) (scenario: Scenario) (reportingInterval: TimeSpan) =
    ScenarioStatsActor(logger, scenario, reportingInterval)

let createForCoordinator (mergeStats: LoadSimulationStats -> ScenarioStats seq -> ScenarioStats)
                         (logger: ILogger) (scenario: Scenario) (reportingInterval: TimeSpan) =
    ScenarioStatsActor(logger, scenario, reportingInterval, mergeStats)
