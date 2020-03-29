module internal NBomber.Domain.StatisticsTypes

open System
open NBomber.Contracts
open NBomber.Domain.DomainTypes

type DataTransferCount = {
    MinKb: float
    MeanKb: float
    MaxKb: float
    AllMB: float
}

type LatencyCount = {
    Less800: int
    More800Less1200: int
    More1200: int
}

type StepResults = {
    StepName: string
    Responses: StepResponse[]
    DataTransfer: DataTransferCount
}

type StepStats = {
    StepName: string
    OkLatencies: Latency[]
    RequestCount: int
    OkCount: int
    FailCount: int
    RPS: int
    Min: Latency
    Mean: Latency
    Max: Latency
    Percent50: Latency
    Percent75: Latency
    Percent95: Latency
    StdDev: int
    DataTransfer: DataTransferCount
}

type ScenarioStats = {
    ScenarioName: string
    StepsStats: StepStats[]
    RPS: int
    OkCount: int
    FailCount: int
    LatencyCount: LatencyCount
    Duration: TimeSpan
}

type RawNodeStats = {
    AllScenariosStats: ScenarioStats[]
    OkCount: int
    FailCount: int
    LatencyCount: LatencyCount
    NodeStatsInfo: NodeInfo
}
