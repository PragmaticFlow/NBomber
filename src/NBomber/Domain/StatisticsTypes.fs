namespace NBomber.Domain

open System
open NBomber.Contracts

type internal DataTransferCount = {
    MinKb: float
    MeanKb: float
    MaxKb: float
    AllMB: float
}

type internal LatencyCount = {
    Less800: int
    More800Less1200: int
    More1200: int
}

type internal StepResults = {
    StepName: string
    Results: (Response*Latency)[]
    DataTransfer: DataTransferCount
}

type internal StepStats = {
    StepName: string
    OkLatencies: Latency[]
    ReqeustCount: int
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

type internal ScenarioStats = {
    ScenarioName: string
    StepsStats: StepStats[]
    RPS: int
    ConcurrentCopies: int
    OkCount: int
    FailCount: int
    LatencyCount: LatencyCount
    Duration: TimeSpan
}

type internal NodeStats = {
    AllScenariosStats: ScenarioStats[]
    OkCount: int
    FailCount: int
    LatencyCount: LatencyCount
    Meta: StatisticsMeta
}
