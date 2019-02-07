module internal NBomber.DomainServices.Cluster.ClusterCoordinator

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Configuration
open NBomber.Domain.Errors
open NBomber.Domain.DomainTypes
open NBomber.Domain.StatisticsTypes
open NBomber.Domain.Statistics
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices.Cluster.Contracts
open NBomber.DomainServices.Cluster.ClusterAgent
open NBomber.DomainServices.ScenariosHost
open NBomber.DomainServices

type IClusterCoordinator =
    abstract StartNewSession: ScenarioSetting[] -> Result<unit,DomainError>    
    abstract WaitAllAgentsReady: unit -> Task<Result<unit,DomainError>>
    abstract StartWarmUp: unit -> Result<unit,DomainError>
    abstract StartBombing: unit -> Result<unit,DomainError>
    abstract GetStatistics: unit -> Result<GlobalStats[],DomainError>

let startNewSession (sessionId: string, settings:ScenarioSetting[], agents: AgentInfo[]) =
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
            |> Array.map(fun x -> x.Data.Value :?> GlobalStats)
            |> Ok
    with
    | ex -> ex |> CommunicationError |> Error

let run (cluster: IClusterCoordinator, scnHost: IScenariosHost,
         settings: ScenarioSetting[], targetScenarios: string[]) = trial {
    
    do! cluster.StartNewSession(settings)
    do! scnHost.InitScenarios(settings, targetScenarios).Result    
    do! cluster.WaitAllAgentsReady().Result
    
    do! cluster.StartWarmUp()
    do scnHost.WarmUpScenarios().Wait()
    do! cluster.WaitAllAgentsReady().Result

    do! cluster.StartBombing()
    do scnHost.RunBombing().Wait()
    do! cluster.WaitAllAgentsReady().Result

    let localStats = scnHost.GetStatistics()
    let! agentsStats = cluster.GetStatistics()
    let allStats = Array.append [|localStats|] agentsStats 
    
    scnHost.StopScenarios()

    return scnHost.GetRegisteredScenarios()
           |> ScenarioBuilder.applyScenariosSettings(settings)
           |> GlobalStats.merge(allStats)
}

type ClusterCoordinator(sessionId: string, scnHost: IScenariosHost, agents: AgentInfo[]) as this =
    
    member x.Run(settings, targetScenarios) = run(this, scnHost, settings, targetScenarios)

    interface IClusterCoordinator with        
        member x.StartNewSession(settings) = startNewSession(sessionId, settings, agents)
        member x.WaitAllAgentsReady() = waitAllAgentsReady(sessionId, agents)
        member x.StartWarmUp() = startWarmUp(sessionId, agents)
        member x.StartBombing() = startBombing(sessionId, agents)
        member x.GetStatistics() = getStatistics(sessionId, agents)        

let create (dep: Dependency, registeredScenarios: Scenario[], settings: CoordinatorSettings) =
    let scnHost = ScenariosHost(dep, registeredScenarios)
    let agents = settings.Agents |> Array.map(AgentInfo.create(settings.ClusterId))
    ClusterCoordinator(dep.SessionId, scnHost, agents) 