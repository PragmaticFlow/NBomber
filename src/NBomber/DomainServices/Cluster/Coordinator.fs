module internal NBomber.DomainServices.Cluster.ClusterCoordinator

open FsToolkit.ErrorHandling

open NBomber.Contracts
open NBomber.Configuration
open NBomber.Domain
open NBomber.Domain.Statistics
open NBomber.Errors
open NBomber.Infra.Dependency
open NBomber.DomainServices.ScenariosHost
open NBomber.DomainServices.Cluster.ClusterAgent

type IClusterCoordinator =
    abstract SendStartNewSession: ScenarioSetting[] -> Async<Result<unit,AppError>>
    abstract WaitOnAllAgentsReady:             unit -> Async<Result<unit,AppError>>
    abstract SendStartWarmUp:                  unit -> Async<Result<unit,AppError>>
    abstract SendStartBombing:                 unit -> Async<Result<unit,AppError>>
    abstract GetStatistics:                    unit -> Async<Result<NodeStats[],AppError>>

let runCoordinator (cluster: IClusterCoordinator, localHost: IScenariosHost,
                    settings: ScenarioSetting[], targetScns: string[]) =

    asyncResult {
        do! cluster.SendStartNewSession(settings)
        do! localHost.InitScenarios(settings, targetScns)
        do! cluster.WaitOnAllAgentsReady()

        do! cluster.SendStartWarmUp()
        do! localHost.WarmUpScenarios()
        do! cluster.WaitOnAllAgentsReady()

        do! cluster.SendStartBombing()
        do! localHost.StartBombing()
        do! cluster.WaitOnAllAgentsReady()

        let localStats = localHost.GetStatistics()
        let! agentsStats = cluster.GetStatistics()
        let allStats = Array.append [|localStats|] agentsStats
        localHost.StopScenarios()
        return allStats
    }

let createStats (sessionId: string, nodeName: string,
                 registeredScenarios: Scenario[],
                 scnSettings: ScenarioSetting[], allNodeStats: NodeStats[]) =

    let meta = { SessionId = sessionId
                 NodeName = nodeName
                 Sender = NodeType.Cluster }

    registeredScenarios
    |> Scenario.applySettings(scnSettings)
    |> NodeStats.merge(meta, allNodeStats)

type ClusterCoordinator(sessionId: string, scnHost: IScenariosHost, agents: AgentInfo[]) as this =

    member x.Run(settings, targetScns) = runCoordinator(this, scnHost, settings, targetScns)

    interface IClusterCoordinator with
        member x.SendStartNewSession(settings) = Communication.startNewSession(sessionId, settings, agents)
        member x.WaitOnAllAgentsReady() = Communication.waitOnAllAgentsReady(sessionId, agents)
        member x.SendStartWarmUp() = Communication.startWarmUp(sessionId, agents)
        member x.SendStartBombing() = Communication.startBombing(sessionId, agents)
        member x.GetStatistics() = Communication.getStatistics(sessionId, agents)

let create (dep: Dependency, registeredScenarios: Scenario[], settings: CoordinatorSettings) =
    let scnHost = ScenariosHost(dep, registeredScenarios)
    let agents = settings.Agents |> List.toArray |> Array.map(AgentInfo.create(settings.ClusterId))
    ClusterCoordinator(dep.SessionId, scnHost, agents)

let run (scnSettings, targetScns) (coordinator: ClusterCoordinator) =
    coordinator.Run(scnSettings, targetScns)
