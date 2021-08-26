module internal NBomber.Domain.Stats.ScenarioStatsActor

open System

open Serilog

open NBomber.Contracts.Stats
open NBomber.Domain.DomainTypes
open NBomber.Domain.Stats.Statistics

type ActorMessage =
    | AddResponses     of responses:(int * StepResponse)[] // stepIndex * StepResponse
    | GetRealtimeStats of AsyncReplyChannel<ScenarioStats> * LoadSimulationStats * duration:TimeSpan
    | GetFinalStats    of AsyncReplyChannel<ScenarioStats> * LoadSimulationStats * duration:TimeSpan

let create (logger: ILogger, scenario: Scenario, reportingInterval: TimeSpan) =

    let _allStepsData = Array.init scenario.Steps.Length (fun _ -> StepStatsRawData.createEmpty())
    let mutable _intervalStepsData = Array.init scenario.Steps.Length (fun _ -> StepStatsRawData.createEmpty())

    let addResponses (allData: StepStatsRawData[],
                      intervalData: StepStatsRawData[],
                      responses: (int * StepResponse)[]) =

        for (stepIndex, res) in responses do
            let allStData = allData.[stepIndex]
            let intervalStData = intervalData.[stepIndex]
            allData.[stepIndex] <- StepStatsRawData.addResponse allStData res
            intervalData.[stepIndex] <- StepStatsRawData.addResponse intervalStData res

    let createScenarioStats (stepsData, simulationStats, operation, duration, interval) =
        ScenarioStats.create scenario stepsData simulationStats operation duration interval

    MailboxProcessor.Start(fun inbox ->

        let rec loop () = async {
            try
                match! inbox.Receive() with
                | AddResponses responses ->
                    addResponses(_allStepsData, _intervalStepsData, responses)
                    return! loop()

                | GetRealtimeStats (reply, simulationStats, duration) ->
                    let scnStats = createScenarioStats(_intervalStepsData, simulationStats, OperationType.Bombing, duration, reportingInterval)
                    // reset interval steps data
                    _intervalStepsData <- Array.init scenario.Steps.Length (fun _ -> StepStatsRawData.createEmpty())
                    reply.Reply(scnStats)
                    return! loop()

                | GetFinalStats (reply, simulationStats, duration) ->
                    let scnStats = createScenarioStats(_allStepsData, simulationStats, OperationType.Complete, duration, duration)
                    reply.Reply(scnStats)
                    return! loop()
            with
            | ex -> logger.Error(ex, "ScenarioStatsActor failed.")
        }

        loop()
    )
