module internal NBomber.Domain.Stats.ScenarioStatsActor

open System

open Serilog

open NBomber.Contracts.Stats
open NBomber.Domain.DomainTypes
open NBomber.Domain.Stats.Statistics

type ActorMessage =
    | AddResponses     of responses:(int * StepResponse)[] // stepIndex * StepResponse
    | GetScenarioStats of AsyncReplyChannel<ScenarioStats> * LoadSimulationStats * OperationType * duration:TimeSpan

let create (logger: ILogger, scenario: Scenario) =

    let allStepsData = Array.init scenario.Steps.Length (fun _ -> StepStatsRawData.createEmpty())

    let addResponses (responses: (int * StepResponse)[]) =
        for (stepIndex, res) in responses do
            let stData = allStepsData.[stepIndex]
            allStepsData.[stepIndex] <- StepStatsRawData.addResponse stData res

    let getScenarioStats (simulationStats, currentOperation, duration) =
        ScenarioStats.create scenario allStepsData simulationStats currentOperation duration

    MailboxProcessor.Start(fun inbox ->

        let rec loop () = async {
            try
                match! inbox.Receive() with
                | AddResponses responses ->
                    addResponses responses
                    return! loop()

                | GetScenarioStats (reply, simulationStats, currentOperation, duration) ->
                    let scnStats = getScenarioStats(simulationStats, currentOperation, duration)
                    reply.Reply(scnStats)
                    return! loop()
            with
            | ex -> logger.Fatal(ex, "ScenarioStatsActor failed.")
        }

        loop()
    )
