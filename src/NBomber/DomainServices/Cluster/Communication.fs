module internal NBomber.DomainServices.Cluster.Communication

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Configuration
open NBomber.Domain
open NBomber.Infra
open NBomber.DomainServices.Cluster.Contracts
open NBomber.DomainServices.Cluster.ClusterAgent

let startNewSession (sessionId: string, settings: ScenarioSetting[], agents: AgentInfo[]) =
    try
        let errors =
            agents 
            |> Array.map(fun x -> 
                let cmd = NewSession(sessionId, settings, x.TargetScenarios)
                Http.sendMsg<AgentCommand,AgentResponse>(x.Url, cmd).Result)
            |> Array.filter(fun x -> x.Error.IsSome)
            |> Array.map(fun x -> x.Error.Value)
        
        if errors.Length = 0 then Ok()
        else errors |> StartNewSessionError |> Error
    with
    | ex -> ex |> CommunicationError |> Error

let waitAllAgentsReady (sessionId: string, agents: AgentInfo[]) = task {
    try
        let mutable returnResult = Ok()
        let mutable allFinished = false
        let exit (result) = allFinished <- true
                            returnResult <- result

        while not allFinished do
            let responses = 
                agents 
                |> Array.map(fun x -> 
                    let cmd = IsWorking(sessionId)
                    Http.sendMsg<AgentCommand,AgentResponse>(x.Url, cmd).Result)
            
            let errors = responses |> Array.filter(fun x -> x.Error.IsSome) |> Array.map(fun x -> x.Error.Value)
            if errors.Length > 0 then
                errors |> AgentErrors |> Error |> exit
            else
                let stillWorking = 
                    responses 
                    |> Array.filter(fun x -> x.Data.IsSome)
                    |> Array.exists(fun x -> x.Data.Value :?> bool)
                
                if stillWorking then do! Task.Delay(TimeSpan.FromSeconds(5.0))
                else Ok() |> exit
                
        return returnResult
    with
    | ex -> return ex |> CommunicationError |> Error
}

let startWarmUp (sessionId: string, agents: AgentInfo[]) =
    try
        let errors =
            agents 
            |> Array.map(fun x -> 
                let cmd = StartWarmUp(sessionId)
                Http.sendMsg<AgentCommand,AgentResponse>(x.Url, cmd).Result)
            |> Array.filter(fun x -> x.Error.IsSome)
            |> Array.map(fun x -> x.Error.Value)
        
        if errors.Length = 0 then Ok()
        else errors |> StartWarmUpError |> Error
    with
    | ex -> ex |> CommunicationError |> Error

let startBombing (sessionId: string, agents: AgentInfo[]) =
    try
        let errors =
            agents 
            |> Array.map(fun x -> 
                let cmd = StartBombing(sessionId)
                Http.sendMsg<AgentCommand,AgentResponse>(x.Url, cmd).Result)
            |> Array.filter(fun x -> x.Error.IsSome)
            |> Array.map(fun x -> x.Error.Value)
        
        if errors.Length = 0 then Ok()
        else errors |> StartBombingError |> Error
    with
    | ex -> ex |> CommunicationError |> Error

let getStatistics (sessionId: string, agents: AgentInfo[]) =
    try
        let responses = 
            agents 
            |> Array.map(fun x -> 
                let cmd = GetStatistics(sessionId)
                Http.sendMsg<AgentCommand,AgentResponse>(x.Url, cmd).Result)            
        
        let errors = responses |> Array.filter(fun x -> x.Error.IsSome) |> Array.map(fun x -> x.Error.Value)
        if errors.Length > 0 then
            errors |> GetStatisticsError |> Error
        else 
            responses 
            |> Array.filter(fun x -> x.Data.IsSome)
            |> Array.map(fun x -> x.Data.Value :?> NodeStats)
            |> Ok
    with
    | ex -> ex |> CommunicationError |> Error

