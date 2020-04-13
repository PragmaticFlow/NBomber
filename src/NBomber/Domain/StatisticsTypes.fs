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

type RawStepResults = {
    StepName: string
    Responses: StepResponse[]
    DataTransfer: DataTransferCount
}

type RawStepStats = {
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

type RawScenarioStats = {
    ScenarioName: string
    RequestCount: int
    OkCount: int
    FailCount: int
    AllDataMB: float
    RawStepsStats: RawStepStats[]
    LatencyCount: LatencyCount
    Duration: TimeSpan
}
