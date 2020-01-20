module internal NBomber.DomainServices.Cluster.Contracts

open System
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Errors
open NBomber.Domain
open NBomber.DomainServices.TestHost

type ClientId = string

type AgentNodeInfo = {
    NodeInfo: NodeInfo
    TargetGroup: string
}

type Request =
    | GetAgentInfo of onlyForSessionId:string option
    | NewSession of sessionArgs:TestSessionArgs * agentSettings:TargetGroupSettings[]
    | StartWarmUp
    | StartBombing
    | GetStatistics of executionTime:TimeSpan option

type Response =
    | AgentInfo of AgentNodeInfo
    | AgentStats of RawNodeStats

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
