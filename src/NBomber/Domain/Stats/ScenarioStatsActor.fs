module internal NBomber.Domain.Stats.ScenarioStatsActor

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

open Serilog

open NBomber.Contracts.Stats
open NBomber.Contracts.Internal
open NBomber.Domain.DomainTypes
open NBomber.Domain.Stats.Statistics

// todo: use UMX to distinguish scnDuration vs reportingInterval
type ScenarioDuration = string

type ActorMessage =
    | AddResponse        of StepResponse
    | AddFromAgent       of StepResponse list
    | StartUseTempBuffer
    | FlushTempBuffer
    | BuildRealtimeStats  of reply:TaskCompletionSource<ScenarioStats> * LoadSimulationStats * duration:TimeSpan
    | GetRemainedRawStats of reply:TaskCompletionSource<ScenarioRawStats>
    | DelRawStats         of duration:TimeSpan
    | GetFinalStats       of reply:TaskCompletionSource<ScenarioStats> * LoadSimulationStats * duration:TimeSpan

type ScenarioStatsActor(logger: ILogger, scenario: Scenario, reportingInterval: TimeSpan, keepRawStats: bool) =

    let _log = logger.ForContext<ScenarioStatsActor>()
    let _allStepsData = Array.init scenario.Steps.Length (fun _ -> StepStatsRawData.createEmpty())
    let mutable _intervalStepsData = Array.init scenario.Steps.Length (fun _ -> StepStatsRawData.createEmpty())
    let mutable _intervalRawStats = List.empty<StepResponse>
    let mutable _allRealtimeStats = Map.empty<ScenarioDuration,ScenarioStats>
    let mutable _allRawStats = Map.empty<ScenarioDuration,ScenarioRawStats>
    let mutable _tempBuffer = List.empty<StepResponse>
    let mutable _useTempBuffer = false

    let addResponse (resp: StepResponse) =
        let allStData = _allStepsData.[resp.StepIndex]
        let intervalStData = _intervalStepsData.[resp.StepIndex]
        _allStepsData.[resp.StepIndex] <- StepStatsRawData.addResponse allStData resp
        _intervalStepsData.[resp.StepIndex] <- StepStatsRawData.addResponse intervalStData resp

        if keepRawStats then
            resp.ClientResponse.Payload <- null // to prevent sending in cluster mode
            resp.ClientResponse.Message <- null
            _intervalRawStats <- resp :: _intervalRawStats

    let createScenarioStats (stepsData, simulationStats, operation, duration, interval) =
        ScenarioStats.create scenario stepsData simulationStats operation duration interval

    let buildRealtimeStats (simulationStats: LoadSimulationStats) (duration: TimeSpan) =
        let scnStats = createScenarioStats(_intervalStepsData, simulationStats, OperationType.Bombing, duration, reportingInterval)
        _allRealtimeStats <- _allRealtimeStats.Add(duration.ToString(), scnStats)
        _intervalStepsData <- Array.init scenario.Steps.Length (fun _ -> StepStatsRawData.createEmpty()) // reset interval steps data

        if keepRawStats then
            let rawStats = { ScenarioName = scenario.ScenarioName; StepResponses = _intervalRawStats; Duration = duration }
            _allRawStats <- _allRawStats.Add(duration.ToString(), rawStats)
            _intervalRawStats <- List.empty // reset interval steps data

        scnStats

    let _actor = ActionBlock(fun msg ->
        try
            match msg with
            | AddResponse response ->
                if _useTempBuffer then _tempBuffer <- response :: _tempBuffer
                else addResponse response

            | AddFromAgent responses ->
                responses |> List.iter addResponse

            | StartUseTempBuffer ->
                _useTempBuffer <- true

            | FlushTempBuffer ->
                _useTempBuffer <- false
                _tempBuffer |> List.iter addResponse
                _tempBuffer <- List.empty

            | BuildRealtimeStats (reply, simulationStats, duration) ->
                let scnStats = buildRealtimeStats simulationStats duration
                reply.TrySetResult(scnStats) |> ignore

            | GetRemainedRawStats reply ->
                let rawStats = { ScenarioName = scenario.ScenarioName; StepResponses = _intervalRawStats; Duration = TimeSpan.MaxValue }
                reply.TrySetResult(rawStats) |> ignore

            | DelRawStats duration ->
                _allRawStats <- _allRawStats.Remove(duration.ToString())

            | GetFinalStats (reply, simulationStats, duration) ->
                let scnStats = createScenarioStats(_allStepsData, simulationStats, OperationType.Complete, duration, duration)
                reply.TrySetResult(scnStats) |> ignore
        with
        | ex -> logger.Error $"{nameof ScenarioStatsActor} failed: {ex.ToString()}"
    )

    member _.AllRealtimeStats = _allRealtimeStats
    member _.AllRawStats = _allRawStats

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.Publish(msg) = _actor.Post(msg) |> ignore

let create (logger: ILogger) (scenario: Scenario) (reportingInterval: TimeSpan) =
    ScenarioStatsActor(logger, scenario, reportingInterval, keepRawStats = false)

let createWithRawStats (logger: ILogger) (scenario: Scenario) (reportingInterval: TimeSpan) =
    ScenarioStatsActor(logger, scenario, reportingInterval, keepRawStats = true)
