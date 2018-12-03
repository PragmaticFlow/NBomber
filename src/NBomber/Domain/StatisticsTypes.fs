module internal NBomber.Domain.StatisticsTypes

open System
open NBomber.Contracts

type LatencyCount = {
    Less800: int
    More800Less1200: int
    More1200: int
}

type Percentiles = {
    RPS: int
    Min: Latency
    Mean: Latency
    Max: Latency
    Percent50: Latency
    Percent75: Latency
    Percent95: Latency
    StdDev: int
}

type StepStats = {     
    StepName: string
    OkLatencies: Latency[]
    FailLatencies: Latency[]    
    ReqeustCount: int
    OkCount: int
    FailCount: int    
    Percentiles: Percentiles option
}

type ScenarioStats = {
    ScenarioName: string    
    StepsStats: StepStats[]
    ConcurrentCopies: int    
    OkCount: int
    FailCount: int
    LatencyCount: LatencyCount
    ActiveTime: TimeSpan
    Duration: TimeSpan
}

type GlobalStats = {
    AllScenariosStats: ScenarioStats[]
    OkCount: int
    FailCount: int
    LatencyCount: LatencyCount    
}