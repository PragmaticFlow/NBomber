module internal NBomber.Domain.Stats.ScenarioStatsActor

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

open FSharp.UMX
open Serilog

open NBomber.Contracts.Stats
open NBomber.Contracts.Internal
open NBomber.Domain.DomainTypes
open NBomber.Domain.Stats.Statistics

type ActorMessage =
    | AddResponse of StepResponse
    | AddFromAgent of ScenarioStats
    | StartUseTempBuffer
    | FlushTempBuffer
    | BuildRealtimeStats of reply:TaskCompletionSource<ScenarioStats> * LoadSimulationStats * duration:TimeSpan
    | GetFinalStats      of reply:TaskCompletionSource<ScenarioStats> * LoadSimulationStats * duration:TimeSpan

type ScenarioStatsActor(logger: ILogger,
                        scenario: Scenario,
                        reportingInterval: TimeSpan,
                        ?mergeStatsFn: LoadSimulationStats -> ScenarioStats list -> ScenarioStats) =

    let _log = logger.ForContext<ScenarioStatsActor>()
    let _allStepsData = Array.init scenario.Steps.Length (fun _ -> StepStatsRawData.createEmpty())
    let mutable _intervalStepsData = Array.init scenario.Steps.Length (fun _ -> StepStatsRawData.createEmpty())
    let mutable _allRealtimeStats = Map.empty<TimeSpan,ScenarioStats>
    let mutable _agentsStats = List.empty<ScenarioStats>
    let mutable _tempBuffer = List.empty<StepResponse>
    let mutable _useTempBuffer = false

    let addResponse (resp: StepResponse) =
        let allStData = _allStepsData.[resp.StepIndex]
        let intervalStData = _intervalStepsData.[resp.StepIndex]
        _allStepsData.[resp.StepIndex] <- StepStatsRawData.addResponse allStData resp
        _intervalStepsData.[resp.StepIndex] <- StepStatsRawData.addResponse intervalStData resp

    let createRealtimeStats (simulationStats) (duration) (stepsData) =
        ScenarioStats.create scenario stepsData simulationStats OperationType.Bombing %duration reportingInterval

    let createFinalStats (simulationStats) (duration) (stepsData) =
        ScenarioStats.create scenario stepsData simulationStats OperationType.Complete %duration duration

    let addToCacheAndReset (realtimeStats: ScenarioStats) =
        _allRealtimeStats <- _allRealtimeStats.Add(realtimeStats.Duration, realtimeStats)
        // reset interval steps data
        _intervalStepsData <- Array.init scenario.Steps.Length (fun _ -> StepStatsRawData.createEmpty())
        _agentsStats <- List.empty

    let _actor = ActionBlock(fun msg ->
        try
            match msg with
            | AddResponse response ->
                if _useTempBuffer then _tempBuffer <- response :: _tempBuffer
                else addResponse response

            | AddFromAgent agentStats ->
                _agentsStats <- agentStats :: _agentsStats

            | StartUseTempBuffer ->
                _useTempBuffer <- true

            | FlushTempBuffer ->
                _useTempBuffer <- false
                _tempBuffer |> List.iter addResponse
                _tempBuffer <- List.empty

            | BuildRealtimeStats (reply, simulationStats, duration) ->
                let cordStats = _intervalStepsData |> createRealtimeStats simulationStats duration

                let allStats =
                    if scenario.IsEnabled then cordStats :: _agentsStats
                    else _agentsStats

                if allStats.Length > 0 then
                    let merged =
                        mergeStatsFn
                        |> Option.map(fun merge -> merge simulationStats allStats)
                        |> Option.defaultValue cordStats

                    addToCacheAndReset merged
                    reply.TrySetResult(merged) |> ignore
                else
                    addToCacheAndReset cordStats
                    reply.TrySetResult(cordStats) |> ignore

            | GetFinalStats (reply, simulationStats, duration) ->
                let cordStats = _allStepsData |> createFinalStats simulationStats duration

                let allStats =
                    if scenario.IsEnabled then cordStats :: _agentsStats
                    else _agentsStats

                if allStats.Length > 0 then
                    let merged =
                        mergeStatsFn
                        |> Option.map(fun merge -> merge simulationStats allStats)
                        |> Option.defaultValue cordStats

                    reply.TrySetResult(merged) |> ignore
                else
                    reply.TrySetResult(cordStats) |> ignore
        with
        | ex -> logger.Error $"{nameof ScenarioStatsActor} failed: {ex.ToString()}"
    )

    member _.AllRealtimeStats = _allRealtimeStats

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.Publish(msg) = _actor.Post(msg) |> ignore

let createDefault (logger: ILogger) (scenario: Scenario) (reportingInterval: TimeSpan) =
    ScenarioStatsActor(logger, scenario, reportingInterval)

let createForCoordinator (mergeStats: LoadSimulationStats -> ScenarioStats list -> ScenarioStats)
                         (logger: ILogger) (scenario: Scenario) (reportingInterval: TimeSpan) =
    ScenarioStatsActor(logger, scenario, reportingInterval, mergeStats)
