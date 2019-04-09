module internal NBomber.DomainServices.Cluster.Communication

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Configuration
open NBomber.Domain
open NBomber.Infra
open NBomber.DomainServices.Cluster.Contracts
open NBomber.DomainServices.Cluster.ClusterAgent

let private sendCommand (createCommand: AgentInfo -> AgentCommand) (agents: AgentInfo[]) = 
    agents 
    |> Array.map(fun x -> 
        createCommand(x)
        |> Http.sendRequest<AgentCommand,AgentResponse>(x.Url))
    |> Task.WhenAll

let private checkResponses (operationName: string) (responses) =     
    match Result.sequence(responses) with
    | Ok _     -> Ok()
    | Error es -> Error <| OperationFailed(operationName, es)

let startNewSession (sessionId: string, 
                     settings: ScenarioSetting[], agents: AgentInfo[]) =
    agents
    |> sendCommand(fun x -> NewSession(sessionId, settings, x.TargetScenarios))
    |> Task.map(checkResponses("StartNewSession"))        

let waitOnAllAgentsReady (sessionId: string, agents: AgentInfo[]) =
    let rec start () = task {
        let! responses =
            agents
            |> sendCommand(fun _ -> IsWorking(sessionId))        
        
        match Result.sequence(responses) with
        | Ok data  -> 
            let notReady = data |> Array.exists(fun x -> x.Error.IsSome)
            if notReady then
                do! Task.Delay(TimeSpan.FromSeconds(1.0))
                return! start()
            else
                return Ok()

        | Error es -> return Error <| OperationFailed("WaitOnAllAgentsReady", es)
    }
    start()

let startWarmUp (sessionId: string, agents: AgentInfo[]) =
    agents
    |> sendCommand(fun _ -> StartWarmUp(sessionId))
    |> Task.map(checkResponses("StartWarmUp"))

let startBombing (sessionId: string, agents: AgentInfo[]) =
    agents
    |> sendCommand(fun _ -> StartBombing(sessionId))
    |> Task.map(checkResponses("StartBombing"))

let getStatistics (sessionId: string, agents: AgentInfo[]) = task {    
    let! responses =
        agents
        |> sendCommand(fun _ -> GetStatistics(sessionId)) 

    match Result.sequence(responses) with
    | Ok stats -> return stats |> Array.map(fun x -> x.Data.Value :?> NodeStats) |> Ok
    | Error es -> return Error <| OperationFailed("GetStatistics", es)
}