module internal NBomber.Domain.Stats.GlobalScenarioStatsActor

open System
open System.Buffers
open System.Runtime.CompilerServices
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

open Serilog

open NBomber.Contracts.Stats
open NBomber.Contracts.Internal
open NBomber.Domain.DomainTypes
open NBomber.Domain.Stats.Statistics

type ActorMessage =
    | AddResponses     of StepResponse[]
    | GetRealtimeStats of TaskCompletionSource<ScenarioStats> * LoadSimulationStats * duration:TimeSpan
    | GetFinalStats    of TaskCompletionSource<ScenarioStats> * LoadSimulationStats * duration:TimeSpan

type GlobalScenarioStatsActor(logger: ILogger, scenario: Scenario, reportingInterval: TimeSpan) =

    let _allStepsData = Array.init scenario.Steps.Length (fun _ -> StepStatsRawData.createEmpty())
    let _arrayPool = ArrayPool<StepResponse>.Shared
    let mutable _intervalStepsData = Array.init scenario.Steps.Length (fun _ -> StepStatsRawData.createEmpty())

    let addResponses (allData: StepStatsRawData[], intervalData: StepStatsRawData[], responses: StepResponse[]) =
        for resp in responses do
            let allStData = allData.[resp.StepIndex]
            let intervalStData = intervalData.[resp.StepIndex]
            allData.[resp.StepIndex] <- StepStatsRawData.addResponse allStData resp
            intervalData.[resp.StepIndex] <- StepStatsRawData.addResponse intervalStData resp

    let createScenarioStats (stepsData, simulationStats, operation, duration, interval) =
        ScenarioStats.create scenario stepsData simulationStats operation duration interval

    let _actor = ActionBlock(fun msg ->
        try
            match msg with
            | AddResponses responses ->
                addResponses(_allStepsData, _intervalStepsData, responses)
                _arrayPool.Return(responses, clearArray = true)

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

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.GetEmptyResponsesFromPool(count) = _arrayPool.Rent(count)

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