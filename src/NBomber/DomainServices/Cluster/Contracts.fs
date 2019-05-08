module internal NBomber.DomainServices.Cluster.Contracts

open NBomber.Configuration
open NBomber.Errors

type AgentCommand =
    | NewSession  of sessionId:string * settings:ScenarioSetting[] * targetScenarios:string[]
    | IsWorking   of sessionId:string
    | StartWarmUp of sessionId:string
    | StartBombing  of sessionId:string
    | GetStatistics of sessionId:string

type AgentResponse = {
    AgentId: string
    Data: obj option
    Error: AppError option
}
