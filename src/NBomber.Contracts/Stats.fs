namespace NBomber.Contracts.Stats

open System
open System.Data

open Newtonsoft.Json
open Newtonsoft.Json.Converters

open NBomber.Contracts.Thresholds

type ReportFormat =
    | Txt = 0
    | Html = 1
    | Csv = 2
    | Md = 3

type TestInfo = {
    SessionId: string
    TestSuite: string
    TestName: string
}

type NodeType =
    | SingleNode
    | Coordinator
    | Agent

type OperationType =
    | None = 0
    | Init = 1
    | WarmUp = 2
    | Bombing = 3
    | Stop = 4
    | Complete = 5

type NodeInfo = {
    MachineName: string
    NodeType: NodeType
    CurrentOperation: OperationType
    OS: OperatingSystem
    DotNetVersion: string
    Processor: string
    CoresCount: int
    NBomberVersion: string
}

type StatusCodeStats = {
    StatusCode: int
    IsError: bool
    Message: string
    mutable Count: int
}

type RequestStats = {
    Count: int
    RPS: float
}

type LatencyCount = {
    LessOrEq800: int
    More800Less1200: int
    MoreOrEq1200: int
}

type LatencyStats = {
    MinMs: float
    MeanMs: float
    MaxMs: float
    Percent50: float
    Percent75: float
    Percent95: float
    Percent99: float
    StdDev: float
    LatencyCount: LatencyCount
}

type DataTransferStats = {
    MinBytes: int
    MeanBytes: int
    MaxBytes: int
    Percent50: int
    Percent75: int
    Percent95: int
    Percent99: int
    StdDev: float
    AllBytes: int64
}

type StepStatsData = {
    Request: RequestStats
    Latency: LatencyStats
    DataTransfer: DataTransferStats
    StatusCodes: StatusCodeStats[]
}

type StepInfo = {
    Timeout: TimeSpan
    ClientFactoryName: string
    ClientFactoryClientCount: int
    FeedName: string
}

type StepStats = {
    StepName: string
    Ok: StepStatsData
    Fail: StepStatsData
    StepInfo: StepInfo
}

type LoadSimulationStats = {
    SimulationName: string
    Value: int
}

type ThresholdStatus =
    | Passed
    | Failed
with
    static member map value =
        if value then Passed else Failed

type ThresholdStats =
    | RequestCountStats of (RequestCountThreshold * ThresholdStatus) list
    | LatencyStats of (LatencyThreshold * ThresholdStatus) list
    | LatencyPercentileStats of (LatencyPercentileThreshold * ThresholdStatus) list

type ScenarioStats = {
    ScenarioName: string
    RequestCount: int
    OkCount: int
    FailCount: int
    AllBytes: int64
    StepStats: StepStats[]
    LatencyCount: LatencyCount
    LoadSimulationStats: LoadSimulationStats
    StatusCodes: StatusCodeStats[]
    CurrentOperation: OperationType
    Duration: TimeSpan
    ThresholdStats: ThresholdStats[] option
} with

    member this.GetStepStats(stepName: string) = ScenarioStats.getStepStats stepName this

    [<CompiledName("GetStepStats")>]
    static member getStepStats (stepName: string) (scenarioStats: ScenarioStats) =
        scenarioStats.StepStats |> Array.find(fun x -> x.StepName = stepName)

type ReportFile = {
    FilePath: string
    ReportFormat: ReportFormat
    ReportContent: string
}

type NodeStats = {
    RequestCount: int
    OkCount: int
    FailCount: int
    AllBytes: int64
    ScenarioStats: ScenarioStats[]
    PluginStats: DataSet[]
    NodeInfo: NodeInfo
    TestInfo: TestInfo
    ReportFiles: ReportFile[]
    Duration: TimeSpan
} with

    member this.GetScenarioStats(scenarioName: string) = NodeStats.getScenarioStats scenarioName this

    [<CompiledName("Empty")>]
    static member empty = {
        RequestCount = 0; OkCount = 0; FailCount = 0; AllBytes = 0
        ScenarioStats = Array.empty; PluginStats = Array.empty
        NodeInfo = Unchecked.defaultof<_>; TestInfo = Unchecked.defaultof<_>
        ReportFiles = Array.empty; Duration = TimeSpan.MinValue
    }

    [<CompiledName("GetScenarioStats")>]
    static member getScenarioStats (scenarioName: string) (nodeStats: NodeStats) =
        nodeStats.ScenarioStats |> Array.find(fun x -> x.ScenarioName = scenarioName)

type TimeLineHistoryRecord = {
    ScenarioStats: ScenarioStats[]
    Duration: TimeSpan
}

[<JsonConverter(typeof<StringEnumConverter>)>]
type HintSourceType =
    | Scenario = 0
    | WorkerPlugin = 1

type HintResult = {
    SourceName: string
    SourceType: HintSourceType
    Hint: string
}

type NodeSessionResult = {
    FinalStats: NodeStats
    TimeLineHistory: TimeLineHistoryRecord[]
    Hints: HintResult[]
} with

    [<CompiledName("Empty")>]
    static member empty = {
        FinalStats = NodeStats.empty
        TimeLineHistory = Array.empty
        Hints = Array.empty
    }
