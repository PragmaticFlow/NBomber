module internal NBomber.DomainServices.Cluster.ClusterCoordinator

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.Configuration
open NBomber.Domain
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
    abstract GetStatistics: unit -> Result<NodeStats[],DomainError>

let runCoordinator (cluster: IClusterCoordinator, scnHost: IScenariosHost,
                    settings: ScenarioSetting[], targetScns: string[]) = trial {
    
    do! cluster.StartNewSession(settings)
    do! scnHost.InitScenarios(settings, targetScns).Result    
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

    return allStats    
}

let createStats (sessionId: string, nodeName: string, 
                 registeredScenarios: Scenario[],
                 scnSettings: ScenarioSetting[], allNodeStats: NodeStats[]) = 
    
    let meta = { SessionId = sessionId
                 NodeName = nodeName
                 Sender = NodeType.Cluster }
                    
    registeredScenarios 
    |> ScenarioBuilder.applyScenariosSettings(scnSettings)
    |> NodeStats.merge(meta, allNodeStats)

type ClusterCoordinator(sessionId: string, scnHost: IScenariosHost, agents: AgentInfo[]) as this =       

    member x.Run(settings, targetScns) = runCoordinator(this, scnHost, settings, targetScns)

    interface IClusterCoordinator with        
        member x.StartNewSession(settings) = Communication.startNewSession(sessionId, settings, agents)
        member x.WaitAllAgentsReady() = Communication.waitAllAgentsReady(sessionId, agents)
        member x.StartWarmUp() = Communication.startWarmUp(sessionId, agents)
        member x.StartBombing() = Communication.startBombing(sessionId, agents)
        member x.GetStatistics() = Communication.getStatistics(sessionId, agents)        

let create (dep: Dependency, registeredScenarios: Scenario[], settings: CoordinatorSettings) =
    let scnHost = ScenariosHost(dep, registeredScenarios)
    let agents = settings.Agents |> List.toArray |> Array.map(AgentInfo.create(settings.ClusterId))
    ClusterCoordinator(dep.SessionId, scnHost, agents) 

let run (scnSettings, targetScns) (coordinator: ClusterCoordinator) = 
    coordinator.Run(scnSettings, targetScns)