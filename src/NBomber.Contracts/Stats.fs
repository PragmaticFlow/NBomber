namespace NBomber.Contracts.Stats

open System
open System.Data

open FSharp.Json
open MessagePack

open NBomber.Contracts.Internal.Serialization.JsonTransforms

type ReportFormat =
    | Txt = 0
    | Html = 1
    | Csv = 2
    | Md = 3

[<CLIMutable>]
[<MessagePackObject>]
type TestInfo = {
    [<Key 0>] SessionId: string
    [<Key 1>] TestSuite: string
    [<Key 2>] TestName: string
    [<Key 3>] ClusterId: string
} with
    [<CompiledName("Empty")>]
    static member empty = { SessionId = ""; TestSuite = ""; TestName = ""; ClusterId = "" }

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

[<CLIMutable>]
[<MessagePackObject>]
type NodeInfo = {
    [<Key 0>] MachineName: string
    [<Key 1>] NodeType: NodeType
    [<Key 2>] CurrentOperation: OperationType
    [<Key 3>] OS: string
    [<Key 4>] DotNetVersion: string
    [<Key 5>] Processor: string
    [<Key 6>] CoresCount: int
    [<Key 7>] NBomberVersion: string
} with
    [<CompiledName("Empty")>]
    static member empty = {
        MachineName = ""; NodeType = NodeType.SingleNode; CurrentOperation = OperationType.None
        OS = ""; DotNetVersion = ""; Processor = ""; CoresCount = 0; NBomberVersion = ""
    }

[<CLIMutable>]
[<MessagePackObject>]
type StatusCodeStats = {
    [<Key 0>] StatusCode: int
    [<Key 1>] IsError: bool
    [<Key 2>] Message: string
    [<Key 3>] Count: int
}

[<CLIMutable>]
[<MessagePackObject>]
type RequestStats = {
    [<Key 0>] Count: int
    [<Key 1>] RPS: float
}

[<CLIMutable>]
[<MessagePackObject>]
type LatencyCount = {
    [<Key 0>] LessOrEq800: int
    [<Key 1>] More800Less1200: int
    [<Key 2>] MoreOrEq1200: int
}

[<CLIMutable>]
[<MessagePackObject>]
type LatencyStats = {
    [<Key 0>] MinMs: float
    [<Key 1>] MeanMs: float
    [<Key 2>] MaxMs: float
    [<Key 3>] Percent50: float
    [<Key 4>] Percent75: float
    [<Key 5>] Percent95: float
    [<Key 6>] Percent99: float
    [<Key 7>] StdDev: float
    [<Key 8>] LatencyCount: LatencyCount
}

[<CLIMutable>]
[<MessagePackObject>]
type DataTransferStats = {
    [<Key 0>] MinBytes: int
    [<Key 1>] MeanBytes: int
    [<Key 2>] MaxBytes: int
    [<Key 3>] Percent50: int
    [<Key 4>] Percent75: int
    [<Key 5>] Percent95: int
    [<Key 6>] Percent99: int
    [<Key 7>] StdDev: float
    [<Key 8>] AllBytes: int64
}

[<CLIMutable>]
[<MessagePackObject>]
type StepStatsData = {
    [<Key 0>] Request: RequestStats
    [<Key 1>] Latency: LatencyStats
    [<Key 2>] DataTransfer: DataTransferStats
    [<Key 3>] StatusCodes: StatusCodeStats[]
}

[<CLIMutable>]
[<MessagePackObject>]
type StepInfo = {
    [<Key 0>] [<JsonField(Transform=typeof<TimeSpanTransform>)>] Timeout: TimeSpan
    [<Key 1>] ClientFactoryName: string
    [<Key 2>] ClientFactoryClientCount: int
    [<Key 3>] FeedName: string
}

[<CLIMutable>]
[<MessagePackObject>]
type StepStats = {
    [<Key 0>] StepName: string
    [<Key 1>] Ok: StepStatsData
    [<Key 2>] Fail: StepStatsData
    [<Key 3>] StepInfo: StepInfo
}

[<CLIMutable>]
[<MessagePackObject>]
type LoadSimulationStats = {
    [<Key 0>] SimulationName: string
    [<Key 1>] Value: int
}

[<CLIMutable>]
[<MessagePackObject>]
type ScenarioStats = {
    [<Key 0>] ScenarioName: string
    [<Key 1>] RequestCount: int
    [<Key 2>] OkCount: int
    [<Key 3>] FailCount: int
    [<Key 4>] AllBytes: int64
    [<Key 5>] StepStats: StepStats[]
    [<Key 6>] LatencyCount: LatencyCount
    [<Key 7>] LoadSimulationStats: LoadSimulationStats
    [<Key 8>] StatusCodes: StatusCodeStats[]
    [<Key 9>] CurrentOperation: OperationType
    [<Key 10>] [<JsonField(Transform=typeof<TimeSpanTransform>)>] Duration: TimeSpan
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

[<CLIMutable>]
[<MessagePackObject>]
type NodeStats = {
    [<Key 0>] RequestCount: int
    [<Key 1>] OkCount: int
    [<Key 2>] FailCount: int
    [<Key 3>] AllBytes: int64
    [<Key 4>] ScenarioStats: ScenarioStats[]
    [<IgnoreMember>] [<JsonField(Transform=typeof<DateTableTransform>)>] PluginStats: DataSet[]
    [<Key 6>] NodeInfo: NodeInfo
    [<Key 7>] TestInfo: TestInfo
    [<IgnoreMember>] ReportFiles: ReportFile[]
    [<Key 9>] [<JsonField(Transform=typeof<TimeSpanTransform>)>] Duration: TimeSpan
} with

    member this.GetScenarioStats(scenarioName: string) = NodeStats.getScenarioStats scenarioName this

    [<CompiledName("Empty")>]
    static member empty = {
        RequestCount = 0; OkCount = 0; FailCount = 0; AllBytes = 0
        ScenarioStats = Array.empty; PluginStats = Array.empty
        NodeInfo = NodeInfo.empty; TestInfo = TestInfo.empty
        ReportFiles = Array.empty; Duration = TimeSpan.Zero
    }

    [<CompiledName("GetScenarioStats")>]
    static member getScenarioStats (scenarioName: string) (nodeStats: NodeStats) =
        nodeStats.ScenarioStats |> Array.find(fun x -> x.ScenarioName = scenarioName)

type TimeLineHistoryRecord = {
    ScenarioStats: ScenarioStats[]
    [<JsonField(Transform=typeof<TimeSpanTransform>)>] Duration: TimeSpan
}

[<JsonField(EnumValue = EnumMode.Value)>]
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
