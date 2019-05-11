module internal NBomber.DomainServices.Cluster.ClusterAgent

open System

open NBomber.Configuration
open NBomber.Domain
open NBomber.Errors
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.Infra.Http
open NBomber.DomainServices.Cluster.Contracts
open NBomber.DomainServices.ScenariosHost

type IClusterAgent = 
    abstract AgentId: string
    abstract Receive: AgentCommand -> AgentResponse   

type AgentInfo = {
    Url: Uri
    TargetScenarios: string[]
} with
  static member create (clusterId: string) (settings: AgentInfoSettings) = 
    let url = Http.createUrl(settings.Host, settings.Port, clusterId)
    { Url = Uri(url); TargetScenarios = settings.TargetScenarios |> List.toArray }

let generateAgentId () = Guid.NewGuid().ToString("N") 

type ClusterAgent(agentId: string, scnHost: IScenariosHost) =
    
    let mutable curSessionId = ""
    let emptyResponse = { AgentId = agentId; Data = None; Error = None }
    let response (data) = { AgentId = agentId; Data = Some data; Error = None }    

    member private x.ErrorResponse (e: CommunicationError) = 
        { AgentId = agentId; Data = None; Error = Some(Communication e) }

    member private x.ErrorResponse (e: AppError) = 
        { AgentId = agentId; Data = None; Error = Some e }

    interface IClusterAgent with
        member x.AgentId = agentId  
        
        member x.Receive(cmd) = 
            match cmd with
            | NewSession (sessionId, scnSettings, targetScns) -> 
                curSessionId <- sessionId
                scnHost.StopScenarios()
                scnHost.InitScenarios(scnSettings, targetScns) |> ignore
                emptyResponse

            | IsWorking sessionId ->
                match scnHost.IsWorking() with
                | Ok w    -> response(w)
                | Error e -> x.ErrorResponse(e) 
                
            | StartWarmUp sessionId ->
                match scnHost.IsWorking() with
                | Ok w when w = false -> scnHost.WarmUpScenarios() |> ignore
                                         response(w)                
                | Ok _    -> x.ErrorResponse(AgentIsWorking)
                | Error e -> x.ErrorResponse(e)

            | StartBombing sessionId ->
                match scnHost.IsWorking() with
                | Ok w when w = false -> scnHost.StartBombing() |> ignore
                                         response(w)
                | Ok _    -> x.ErrorResponse(AgentIsWorking)
                | Error e -> x.ErrorResponse(e)

            | GetStatistics sessionId ->
                match scnHost.IsWorking() with
                | Ok w when w = false -> scnHost.GetStatistics()
                                         |> response
                | Ok _    -> x.ErrorResponse(AgentIsWorking)
                | Error e -> x.ErrorResponse(e)

let runAgentListener (settings: AgentSettings) (agent: IClusterAgent) =        
    let listenUrl = Http.createUrl("*", settings.Port, settings.ClusterId)
    let httpHandler = Http.createHandler(agent.Receive)
    let server = HttpServer(listenUrl, httpHandler)
    server.Start().Wait()
    server.Stop()

let create (dep: Dependency, registeredScns: Scenario[]) =
    let scnsWithoutInit = registeredScns |> Array.map(fun x -> { x with TestInit = None; TestClean = None } )
    let scnHost = ScenariosHost(dep, scnsWithoutInit)
    let agentId = generateAgentId()    
    ClusterAgent(agentId, scnHost)