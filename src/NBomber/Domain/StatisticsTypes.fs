module internal NBomber.Domain.StatisticsTypes

open System

open Nessos.Streams

open NBomber.Contracts
open NBomber.Domain.DomainTypes

[<Measure>] type kb
[<Measure>] type mb

type DataTransferCount = {
    MinKb: float<kb>
    MeanKb: float<kb>
    MaxKb: float<kb>
    AllMB: float<mb>
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
    StdDev: int
    DataTransfer: DataTransferCount
    ErrorStats: Stream<ErrorStats>
}

type RawScenarioStats = {
    ScenarioName: string
    RequestCount: int
    OkCount: int
    FailCount: int
    AllDataMB: float
    RawStepsStats: Stream<RawStepStats>
    LatencyCount: LatencyCount
    LoadSimulationStats: LoadSimulationStats
    Duration: TimeSpan
    ErrorStats: Stream<ErrorStats>
}
