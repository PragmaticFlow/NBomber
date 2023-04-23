module internal NBomber.Domain.Stats.ScenarioStatsActor

open System
open System.Collections.Generic
open System.Collections.Concurrent
open System.Runtime.CompilerServices
open System.Threading.Tasks
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
    | AddFromAgent   of ScenarioStats
    | BuildReportingStats of reply:TaskCompletionSource<ScenarioStats> * LoadSimulationStats * executedDuration:TimeSpan
    | GetFinalStats       of reply:TaskCompletionSource<ScenarioStats> * LoadSimulationStats * executedDuration:TimeSpan * pause:TimeSpan

type State = {
    Logger: ILogger
    Scenario: Scenario
    ReportingInterval: TimeSpan
    mutable AcceptMaxTimeBucket: TimeSpan
    MergeStatsFn: (LoadSimulationStats -> ScenarioStats seq -> ScenarioStats) option
    mutable ScenarioFailCount: int
    StepsOrder: Dictionary<string, int>
    GlobalInfoDataSize: ResizeArray<int64>

    AllRealtimeStats: ConcurrentDictionary<TimeSpan,ScenarioStats>
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
      AcceptMaxTimeBucket = reportingInterval
      MergeStatsFn = mergeStatsFn
      ScenarioFailCount = 0
      StepsOrder = stepsOrder
      GlobalInfoDataSize = ResizeArray<_>()

      AllRealtimeStats = ConcurrentDictionary<_,_>()

      CoordinatorStepsResults = Dict.empty()
      IntervalStepsResults = Dict.empty()
      ConsoleScenarioStats = consoleStats

      UseTempBuffer = true
      TempBuffer = ResizeArray<_>()
      IntervalAgentsStats = ResizeArray<_>()
      FinalAgentsStats = ResizeArray<_>() }

let updateGlobalInfoDataSize (state: State) (measurement: Measurement) =
    if measurement.Name <> Constants.ScenarioGlobalInfo && measurement.ClientResponse.SizeBytes > 0 then
        state.GlobalInfoDataSize.Add measurement.ClientResponse.SizeBytes

let calcFinalDataSize (state: State) (measurement: Measurement) =
    if measurement.Name = Constants.ScenarioGlobalInfo && state.GlobalInfoDataSize.Count > 0 then
        let sizeBytes = (state.GlobalInfoDataSize |> Seq.sum) + measurement.ClientResponse.SizeBytes
        state.GlobalInfoDataSize.Clear()
        sizeBytes
    else
        measurement.ClientResponse.SizeBytes

let updateCoordinatorStats (state: State) (measurement: Measurement) (finalDataSize) =
    let rawStats = state.CoordinatorStepsResults[measurement.Name]
    RawMeasurementStats.addMeasurement rawStats measurement finalDataSize

let updateIntervalStats (state: State) (measurement: Measurement) (finalDataSize) =
    let rawStats = state.IntervalStepsResults[measurement.Name]
    RawMeasurementStats.addMeasurement rawStats measurement finalDataSize

let addMeasurement (measurement: Measurement) (state: State) =
    if state.UseTempBuffer && measurement.CurrentTimeBucket >= state.AcceptMaxTimeBucket then
        state.TempBuffer.Add measurement
    else
        updateGlobalInfoDataSize state measurement
        let finalDataSize = calcFinalDataSize state measurement

        if state.CoordinatorStepsResults.ContainsKey measurement.Name then
            updateCoordinatorStats state measurement finalDataSize
        else
            state.CoordinatorStepsResults[measurement.Name] <- RawMeasurementStats.empty measurement.Name

            if not (state.StepsOrder.ContainsKey measurement.Name) then
                state.StepsOrder[measurement.Name] <- state.StepsOrder.Count

            updateCoordinatorStats state measurement finalDataSize

        if state.IntervalStepsResults.ContainsKey measurement.Name then
            updateIntervalStats state measurement finalDataSize
        else
            state.IntervalStepsResults[measurement.Name] <- RawMeasurementStats.empty measurement.Name
            updateIntervalStats state measurement finalDataSize

        if measurement.ClientResponse.IsError && measurement.Name = Constants.ScenarioGlobalInfo then
            state.ScenarioFailCount <- state.ScenarioFailCount + 1

let flushTempBuffer (state: State) =
    state.UseTempBuffer <- false
    state.TempBuffer |> Seq.iter(fun measurement -> state |> addMeasurement measurement)
    state.TempBuffer.Clear()
    state.UseTempBuffer <- true

let createReportingStats (state: State) simulationStats executedDuration pause rawStats =
    ScenarioStats.create
        state.Scenario.ScenarioName rawStats simulationStats OperationType.Bombing executedDuration
        state.ReportingInterval pause

let createFinalStats (state: State) simulationStats executedDuration pause rawStats =
    ScenarioStats.create
        state.Scenario.ScenarioName rawStats simulationStats OperationType.Complete executedDuration
        executedDuration pause

let buildStats (state: State)
               (cordRawStats: RawMeasurementStats[])
               (agentStats: ResizeArray<ScenarioStats>)
               (simulationStats: LoadSimulationStats)
               (executedDuration: TimeSpan)
               (pause: TimeSpan)
               (isFinalStats: bool) =

    cordRawStats |> Array.sortInPlaceBy(fun x -> state.StepsOrder[x.Name])

    let cordStats =
        if isFinalStats then
            cordRawStats |> createFinalStats state simulationStats executedDuration pause
        else
            cordRawStats |> createReportingStats state simulationStats executedDuration pause

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
    state.AllRealtimeStats[reportingStats.Duration] <- reportingStats
    state.ConsoleScenarioStats <- mergeConsoleStats state.ConsoleScenarioStats reportingStats

    // reset reporting interval steps data
    state.IntervalStepsResults.Clear()
    state.IntervalAgentsStats.Clear()
    state.AcceptMaxTimeBucket <- state.AcceptMaxTimeBucket + state.ReportingInterval

let addStatsFromAgent (agentStats: ScenarioStats) (state: State) =
    if agentStats.CurrentOperation = OperationType.Bombing then
        state.IntervalAgentsStats.Add agentStats
    else
        state.FinalAgentsStats.Add agentStats

type ScenarioStatsActor(logger: ILogger,
                        scenario: Scenario,
                        reportingInterval: TimeSpan,
                        ?mergeStatsFn: LoadSimulationStats -> ScenarioStats seq -> ScenarioStats) =

    let mutable _state = createState logger scenario reportingInterval mergeStatsFn
    let mutable _stop = false

    let _queue = ConcurrentQueue<ActorMessage>()

    let loop () = backgroundTask {
        try
            while not _stop do
                match _queue.TryDequeue() with
                | true, msg ->

                    match msg with
                    | AddMeasurement result ->
                        _state |> addMeasurement result

                    | AddFromAgent agentStats ->
                        _state |> addStatsFromAgent agentStats

                    | BuildReportingStats (reply, simulationStats, executedDuration) ->
                        let isFinalStats = false
                        let pause = TimeSpan.Zero
                        let reportingStats = _state.IntervalStepsResults.Values |> Seq.toArray

                        let stats = buildStats _state reportingStats _state.IntervalAgentsStats simulationStats executedDuration pause isFinalStats

                        addReportingStats _state stats
                        flushTempBuffer _state
                        reply.TrySetResult(stats) |> ignore

                    | GetFinalStats (reply, simulationStats, executedDuration, pause) ->
                        let isFinalStats = true
                        flushTempBuffer _state

                        let cordStats = _state.CoordinatorStepsResults.Values |> Seq.toArray

                        let stats = buildStats _state cordStats _state.FinalAgentsStats simulationStats executedDuration pause isFinalStats
                        reply.TrySetResult(stats) |> ignore

                | _ -> do! Task.Delay 100
        with
        | ex -> _state.Logger.Fatal $"Unhandled exception: {nameof ScenarioStatsActor} failed: {ex.ToString()}"
    }

    do
        loop() |> ignore

    member this.ScenarioFailCount = _state.ScenarioFailCount
    member this.AllRealtimeStats = _state.AllRealtimeStats :> IReadOnlyDictionary<_,_>
    member this.ConsoleScenarioStats = _state.ConsoleScenarioStats

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member this.Publish(msg) = _queue.Enqueue msg

    member this.Stop() = _stop <- true

let createDefault (logger: ILogger) (scenario: Scenario) (reportingInterval: TimeSpan) =
    ScenarioStatsActor(logger, scenario, reportingInterval)

let createForCoordinator (mergeStats: LoadSimulationStats -> ScenarioStats seq -> ScenarioStats)
                         (logger: ILogger) (scenario: Scenario) (reportingInterval: TimeSpan) =
    ScenarioStatsActor(logger, scenario, reportingInterval, mergeStats)
