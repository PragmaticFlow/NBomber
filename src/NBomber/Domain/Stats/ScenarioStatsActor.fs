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
    | AddResponse      of StepResponse
    | AddResponses     of StepResponse[]
    | PublishStatsToCoordinator
    | GetRealtimeStats of TaskCompletionSource<ScenarioStats> * LoadSimulationStats * duration:TimeSpan
    | GetFinalStats    of TaskCompletionSource<ScenarioStats> * LoadSimulationStats * duration:TimeSpan

type IScenarioStatsActor =
    abstract Publish: ActorMessage -> unit
    abstract GetRealtimeStats: LoadSimulationStats * duration:TimeSpan -> Task<ScenarioStats>
    abstract GetFinalStats: LoadSimulationStats * duration:TimeSpan -> Task<ScenarioStats>

type ScenarioStatsActor(logger: ILogger, scenario: Scenario, reportingInterval: TimeSpan) =

    let _allStepsData = Array.init scenario.Steps.Length (fun _ -> StepStatsRawData.createEmpty())
    let mutable _intervalStepsData = Array.init scenario.Steps.Length (fun _ -> StepStatsRawData.createEmpty())

    let addResponse (allData: StepStatsRawData[]) (intervalData: StepStatsRawData[]) (resp: StepResponse) =
        let allStData = allData.[resp.StepIndex]
        let intervalStData = intervalData.[resp.StepIndex]
        allData.[resp.StepIndex] <- StepStatsRawData.addResponse allStData resp
        intervalData.[resp.StepIndex] <- StepStatsRawData.addResponse intervalStData resp

    let createScenarioStats (stepsData, simulationStats, operation, duration, interval) =
        ScenarioStats.create scenario stepsData simulationStats operation duration interval

    let _actor = ActionBlock(fun msg ->
        try
            match msg with
            | AddResponse response ->
                addResponse _allStepsData _intervalStepsData response

            | AddResponses responses ->
                responses |> Array.iter(addResponse _allStepsData _intervalStepsData)

            | PublishStatsToCoordinator -> () // it's only needed for cluster

            | GetRealtimeStats (reply, simulationStats, duration) ->
                let scnStats = createScenarioStats(_intervalStepsData, simulationStats, OperationType.Bombing, duration, reportingInterval)
                // reset interval steps data
                _intervalStepsData <- Array.init scenario.Steps.Length (fun _ -> StepStatsRawData.createEmpty())
                reply.TrySetResult(scnStats) |> ignore

            | GetFinalStats (reply, simulationStats, duration) ->
                let scnStats = createScenarioStats(_allStepsData, simulationStats, OperationType.Complete, duration, duration)
                reply.TrySetResult(scnStats) |> ignore
        with
        | ex -> logger.Error(ex, "GlobalScenarioStatsActor failed.")
    )

    interface IScenarioStatsActor with

        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member _.Publish(msg) = _actor.Post(msg) |> ignore

        member _.GetRealtimeStats(simulationStats, duration) =
            let tcs = TaskCompletionSource<ScenarioStats>()
            GetRealtimeStats(tcs, simulationStats, duration) |> _actor.Post |> ignore
            tcs.Task

        member _.GetFinalStats(simulationStats, duration) =
            let tcs = TaskCompletionSource<ScenarioStats>()
            GetFinalStats(tcs, simulationStats, duration) |> _actor.Post |> ignore
            tcs.Task

let create (logger: ILogger) (scenario: Scenario) (reportingInterval: TimeSpan) =
    ScenarioStatsActor(logger, scenario, reportingInterval) :> IScenarioStatsActor
