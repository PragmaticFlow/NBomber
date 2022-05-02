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

type ActorMessage =
    | AddResponse       of StepResponse
    | AddResponses      of StepResponse list
    | GetRawStats       of reply:TaskCompletionSource<ScenarioRawStats> * timestamp:TimeSpan
    | GetRealtimeStats  of reply:TaskCompletionSource<ScenarioStats> * LoadSimulationStats * duration:TimeSpan
    | GetFinalStats     of reply:TaskCompletionSource<ScenarioStats> * LoadSimulationStats * duration:TimeSpan

type IScenarioStatsActor =
    abstract Publish: ActorMessage -> unit
    abstract GetRawStats: timestamp:TimeSpan -> Task<ScenarioRawStats>
    abstract GetRealtimeStats: LoadSimulationStats * duration:TimeSpan -> Task<ScenarioStats>
    abstract GetFinalStats: LoadSimulationStats * duration:TimeSpan -> Task<ScenarioStats>

type ScenarioStatsActor(logger: ILogger, scenario: Scenario, reportingInterval: TimeSpan, keepRawStats: bool) =

    let _allStepsData = Array.init scenario.Steps.Length (fun _ -> StepStatsRawData.createEmpty())
    let mutable _intervalStepsData = Array.init scenario.Steps.Length (fun _ -> StepStatsRawData.createEmpty())
    let mutable _intervalRawStats = List.empty

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

    let _actor = ActionBlock(fun msg ->
        try
            match msg with
            | AddResponse response   -> addResponse response
            | AddResponses responses -> responses |> List.iter addResponse

            | GetRawStats (reply, timestamp) ->
                let stats = { ScenarioName = scenario.ScenarioName; Data = _intervalRawStats; Timestamp = timestamp }
                reply.TrySetResult(stats) |> ignore
                _intervalRawStats <- List.empty

            | GetRealtimeStats (reply, simulationStats, duration) ->
                let scnStats = createScenarioStats(_intervalStepsData, simulationStats, OperationType.Bombing, duration, reportingInterval)
                // reset interval steps data
                _intervalStepsData <- Array.init scenario.Steps.Length (fun _ -> StepStatsRawData.createEmpty())
                reply.TrySetResult(scnStats) |> ignore

            | GetFinalStats (reply, simulationStats, duration) ->
                let scnStats = createScenarioStats(_allStepsData, simulationStats, OperationType.Complete, duration, duration)
                reply.TrySetResult(scnStats) |> ignore
        with
        | ex -> logger.Error $"{nameof ScenarioStatsActor} failed: {ex.ToString()}"
    )

    interface IScenarioStatsActor with

        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member _.Publish(msg) = _actor.Post(msg) |> ignore

        member _.GetRawStats(timestamp) =
            let reply = TaskCompletionSource<ScenarioRawStats>()
            GetRawStats(reply, timestamp) |> _actor.Post |> ignore
            reply.Task

        member _.GetRealtimeStats(simulationStats, duration) =
            let reply = TaskCompletionSource<ScenarioStats>()
            GetRealtimeStats(reply, simulationStats, duration) |> _actor.Post |> ignore
            reply.Task

        member _.GetFinalStats(simulationStats, duration) =
            let reply = TaskCompletionSource<ScenarioStats>()
            GetFinalStats(reply, simulationStats, duration) |> _actor.Post |> ignore
            reply.Task

let create (logger: ILogger) (scenario: Scenario) (reportingInterval: TimeSpan) =
    ScenarioStatsActor(logger, scenario, reportingInterval, keepRawStats = false) :> IScenarioStatsActor

let createWithRawStats (logger: ILogger) (scenario: Scenario) (reportingInterval: TimeSpan) =
    ScenarioStatsActor(logger, scenario, reportingInterval, keepRawStats = true) :> IScenarioStatsActor
