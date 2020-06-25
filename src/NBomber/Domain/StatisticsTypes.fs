module internal NBomber.Domain.StatisticsTypes

open System

open Nessos.Streams

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
    Responses: Stream<StepResponse>
    DataTransfer: DataTransferCount
}

type RawStepStats = {
    StepName: string
    OkLatencies: Stream<Latency>
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
    Percent99: Latency
    Percent999: Latency
    StdDev: int
    DataTransfer: DataTransferCount
}

type RawScenarioStats = {
    ScenarioName: string
    RequestCount: int
    OkCount: int
    FailCount: int
    AllDataMB: float
    RawStepsStats: Stream<RawStepStats>
    LatencyCount: LatencyCount
    Duration: TimeSpan
}
