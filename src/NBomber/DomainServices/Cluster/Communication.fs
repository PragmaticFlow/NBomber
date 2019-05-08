module internal NBomber.DomainServices.Cluster.Communication

open FsToolkit.ErrorHandling

open NBomber.Extensions
open NBomber.Configuration
open NBomber.Domain
open NBomber.Errors
open NBomber.Infra
open NBomber.DomainServices.Cluster.Contracts
open NBomber.DomainServices.Cluster.ClusterAgent

let private sendCommand (createCommand: AgentInfo -> AgentCommand) (agents: AgentInfo[]) =
    agents
    |> Array.map(fun x ->
        createCommand(x)
        |> Http.sendRequest<AgentCommand,AgentResponse>(x.Url))
    |> Async.Parallel

let private checkResponses (operationName: string) (responses) =
    match Result.sequence(responses) with
    | Ok _     -> Ok()
    | Error es -> AppError.createResult(OperationFailed(operationName, es))

let startNewSession (sessionId: string,
                     settings: ScenarioSetting[], agents: AgentInfo[]) =
    agents
    |> sendCommand(fun x -> NewSession(sessionId, settings, x.TargetScenarios))
    |> Async.map(checkResponses("StartNewSession"))

let waitOnAllAgentsReady (sessionId: string, agents: AgentInfo[]) =
    let rec start () = asyncResult {
        let! responses =
            agents
            |> sendCommand(fun _ -> IsWorking(sessionId))

        match Result.sequence(responses) with
        | Ok data  ->
            let notReady = data |> Array.exists(fun x -> x.Error.IsSome)
            if notReady then
                do! Async.Sleep(2000)
                return! start()

        | Error es -> return! AppError.createResult(OperationFailed("WaitOnAllAgentsReady", es))
    }
    start()

let startWarmUp (sessionId: string, agents: AgentInfo[]) =
    agents
    |> sendCommand(fun _ -> StartWarmUp(sessionId))
    |> Async.map(checkResponses("StartWarmUp"))

let startBombing (sessionId: string, agents: AgentInfo[]) =
    agents
    |> sendCommand(fun _ -> StartBombing(sessionId))
    |> Async.map(checkResponses("StartBombing"))

let getStatistics (sessionId: string, agents: AgentInfo[]) = async {
    let! responses =
        agents
        |> sendCommand(fun _ -> GetStatistics(sessionId))

    match Result.sequence(responses) with
    | Ok stats -> return stats |> Array.map(fun x -> x.Data.Value :?> NodeStats) |> Ok
    | Error es -> return AppError.createResult(OperationFailed("GetStatistics", es))
}
