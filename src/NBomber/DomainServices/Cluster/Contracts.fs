module internal NBomber.DomainServices.Cluster.Contracts

open NBomber.Configuration
open NBomber.Errors
open NBomber.Domain
open NBomber.DomainServices.ScenariosHost

type ClientId = string

type ClusterNodeInfo = {    
    MachineName: string
    TargetGroup: string
    HostStatus: ScenarioHostStatus
}

type Request =
    | GetAgentInfo
    | NewSession of scnSettings:ScenarioSetting[] * agentSettings:TargetGroupSettings[]
    | StartWarmUp
    | StartBombing
    | GetStatistics

type Response =
    | AgentInfo of ClusterNodeInfo
    | AgentStats of NodeStats

type MessageHeaders = {
    CorrelationId: string 
    ClientId: string
    SessionId: string
}

type RequestMessage = {
    Headers: MessageHeaders
    Payload: Request
}

type ResponseMessage = {
    Headers: MessageHeaders
    Payload: Response option
    Error: AppError option
}

let createAgentsTopic (clusterId) = sprintf "nbomber/clusters/%s/agents" clusterId
let createCoordinatorTopic (clusterId) = sprintf "nbomber/clusters/%s/coordinator" clusterId

let createRequestMsg (clientId, sessionId, req: Request) =
    let correlationId = System.Guid.NewGuid().ToString("N")
    let headers = { CorrelationId = correlationId; ClientId = clientId; SessionId = sessionId }
    { Headers = headers; Payload = req }    

let createResponseMsg (correlationId, clientId, sessionId, res: Response option, error: AppError option) =
    let headers = { CorrelationId = correlationId; ClientId = clientId; SessionId = sessionId }
    { Headers = headers; Payload = res; Error = error }