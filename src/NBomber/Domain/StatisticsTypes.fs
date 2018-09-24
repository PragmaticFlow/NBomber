module internal NBomber.Domain.StatisticsTypes

open System
open NBomber.Contracts

type LatencyCount = {
    Less800: int
    More800Less1200: int
    More1200: int
}

type LatencyDetails = {
    RPS: int64
    Min: Latency
    Mean: Latency
    Max: Latency
    Percent50: Latency
    Percent75: Latency
    Percent95: Latency
    StdDev: int64
}

type StepStats = {     
    StepName: string
    OkLatencies: Latency[]
    FailLatencies: Latency[]    
    OkCount: int
    FailCount: int    
    LatencyDetails: LatencyDetails option
}

type TestFlowStats = {
    FlowName: string
    StepsStats: StepStats[]
    ConcurrentCopies: int
    OkCount: int
    FailCount: int
    LatencyCount: LatencyCount
}

type ScenarioStats = {
    ScenarioName: string
    TestFlowsStats: TestFlowStats[]
    ActiveTime: TimeSpan
    OkCount: int
    FailCount: int
    LatencyCount: LatencyCount
}