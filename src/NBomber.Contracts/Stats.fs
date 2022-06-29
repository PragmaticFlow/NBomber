namespace NBomber.Contracts.Stats

open System
open System.Data
open System.Runtime.Serialization

open Newtonsoft.Json
open Newtonsoft.Json.Converters

type ReportFormat =
    | Txt = 0
    | Html = 1
    | Csv = 2
    | Md = 3

[<CLIMutable>]
[<DataContract>]
type TestInfo = {
    [<DataMember(Order = 0)>] SessionId: string
    [<DataMember(Order = 1)>] TestSuite: string
    [<DataMember(Order = 2)>] TestName: string
    [<DataMember(Order = 3)>] ClusterId: string
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

[<CLIMutable>]
[<DataContract>]
type NodeInfo = {
    [<DataMember(Order = 0)>] MachineName: string
    [<DataMember(Order = 1)>] NodeType: NodeType
    [<DataMember(Order = 2)>] CurrentOperation: OperationType
    [<DataMember(Order = 3)>] OS: string
    [<DataMember(Order = 4)>] DotNetVersion: string
    [<DataMember(Order = 5)>] Processor: string
    [<DataMember(Order = 6)>] CoresCount: int
    [<DataMember(Order = 7)>] NBomberVersion: string
}

[<CLIMutable>]
[<DataContract>]
type StatusCodeStats = {
    [<DataMember(Order = 0)>] StatusCode: int
    [<DataMember(Order = 1)>] IsError: bool
    [<DataMember(Order = 2)>] Message: string
    [<DataMember(Order = 3)>] Count: int
}

[<CLIMutable>]
[<DataContract>]
type RequestStats = {
    [<DataMember(Order = 0)>] Count: int
    [<DataMember(Order = 1)>] RPS: float
}

[<CLIMutable>]
[<DataContract>]
type LatencyCount = {
    [<DataMember(Order = 0)>] LessOrEq800: int
    [<DataMember(Order = 1)>] More800Less1200: int
    [<DataMember(Order = 2)>] MoreOrEq1200: int
}

[<CLIMutable>]
[<DataContract>]
type LatencyStats = {
    [<DataMember(Order = 0)>] MinMs: float
    [<DataMember(Order = 1)>] MeanMs: float
    [<DataMember(Order = 2)>] MaxMs: float
    [<DataMember(Order = 3)>] Percent50: float
    [<DataMember(Order = 4)>] Percent75: float
    [<DataMember(Order = 5)>] Percent95: float
    [<DataMember(Order = 6)>] Percent99: float
    [<DataMember(Order = 7)>] StdDev: float
    [<DataMember(Order = 8)>] LatencyCount: LatencyCount
}

[<CLIMutable>]
[<DataContract>]
type DataTransferStats = {
    [<DataMember(Order = 0)>] MinBytes: int
    [<DataMember(Order = 1)>] MeanBytes: int
    [<DataMember(Order = 2)>] MaxBytes: int
    [<DataMember(Order = 3)>] Percent50: int
    [<DataMember(Order = 4)>] Percent75: int
    [<DataMember(Order = 5)>] Percent95: int
    [<DataMember(Order = 6)>] Percent99: int
    [<DataMember(Order = 7)>] StdDev: float
    [<DataMember(Order = 8)>] AllBytes: int64
}

[<CLIMutable>]
[<DataContract>]
type StepStatsData = {
    [<DataMember(Order = 0)>] Request: RequestStats
    [<DataMember(Order = 1)>] Latency: LatencyStats
    [<DataMember(Order = 2)>] DataTransfer: DataTransferStats
    [<DataMember(Order = 3)>] StatusCodes: StatusCodeStats[]
}

[<CLIMutable>]
[<DataContract>]
type StepInfo = {
    [<DataMember(Order = 0)>] Timeout: TimeSpan
    [<DataMember(Order = 1)>] ClientFactoryName: string
    [<DataMember(Order = 2)>] ClientFactoryClientCount: int
    [<DataMember(Order = 3)>] FeedName: string
}

[<CLIMutable>]
[<DataContract>]
type StepStats = {
    [<DataMember(Order = 0)>] StepName: string
    [<DataMember(Order = 1)>] Ok: StepStatsData
    [<DataMember(Order = 2)>] Fail: StepStatsData
    [<DataMember(Order = 3)>] StepInfo: StepInfo
}

[<CLIMutable>]
[<DataContract>]
type LoadSimulationStats = {
    [<DataMember(Order = 0)>] SimulationName: string
    [<DataMember(Order = 1)>] Value: int
}

[<CLIMutable>]
[<DataContract>]
type ScenarioStats = {
    [<DataMember(Order = 0)>] ScenarioName: string
    [<DataMember(Order = 1)>] RequestCount: int
    [<DataMember(Order = 2)>] OkCount: int
    [<DataMember(Order = 3)>] FailCount: int
    [<DataMember(Order = 4)>] AllBytes: int64
    [<DataMember(Order = 5)>] StepStats: StepStats[]
    [<DataMember(Order = 6)>] LatencyCount: LatencyCount
    [<DataMember(Order = 7)>] LoadSimulationStats: LoadSimulationStats
    [<DataMember(Order = 8)>] StatusCodes: StatusCodeStats[]
    [<DataMember(Order = 9)>] CurrentOperation: OperationType
    [<DataMember(Order = 10)>] Duration: TimeSpan
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
[<DataContract>]
type NodeStats = {
    [<DataMember(Order = 0)>] RequestCount: int
    [<DataMember(Order = 1)>] OkCount: int
    [<DataMember(Order = 2)>] FailCount: int
    [<DataMember(Order = 3)>] AllBytes: int64
    [<DataMember(Order = 4)>] ScenarioStats: ScenarioStats[]
    [<IgnoreDataMember>] PluginStats: DataSet[]
    [<DataMember(Order = 6)>] NodeInfo: NodeInfo
    [<DataMember(Order = 7)>] TestInfo: TestInfo
    [<IgnoreDataMember>] ReportFiles: ReportFile[]
    [<DataMember(Order = 9)>] Duration: TimeSpan
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
