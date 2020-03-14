namespace NBomber.Contracts

open System
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open Serilog
open Microsoft.Extensions.Configuration

open NBomber
open NBomber.Extensions
open NBomber.Configuration

type CorrelationId = {
    Id: string
    ScenarioName: string
    CopyNumber: int
}

[<Struct>]
type Response = {
    Payload: obj
    SizeBytes: int
    Exception: exn voption
    LatencyMs: int
}

type NodeType =
    | SingleNode
    | Coordinator
    | Agent
    | Cluster

type NodeOperationType =
    | None = 0
    | Init = 1
    | WarmUp = 2
    | Bombing = 3
    | Stop = 4
    | Complete = 5

type NodeInfo = {
    MachineName: string
    Sender: NodeType
    CurrentOperation: NodeOperationType
}

type TestInfo = {
    SessionId: string
    TestSuite: string
    TestName: string
}

type Statistics = {
    ScenarioName: string
    StepName: string
    OkCount: int
    FailCount: int
    Min: int
    Mean: int
    Max: int
    RPS: int
    Percent50: int
    Percent75: int
    Percent95: int
    StdDev: int
    DataMinKb: float
    DataMeanKb: float
    DataMaxKb: float
    AllDataMB: float
    NodeInfo: NodeInfo
}

[<CustomEquality; NoComparison>]
type ConnectionPoolArgs<'TConnection> = {
    PoolName: string
    OpenConnection: int -> 'TConnection
    CloseConnection: ('TConnection -> unit) option
    ConnectionCount: int
} with
    override x.GetHashCode() = x.PoolName.GetHashCode()
    override x.Equals(b) =
        match b with
        | :? ConnectionPoolArgs<'TConnection> as pool -> x.PoolName = pool.PoolName
        | _ -> false

type IFeedProvider<'TFeedItem> =
    abstract GetAllItems: unit -> 'TFeedItem[]

type IFeed<'TFeedItem> =
    abstract Name: string
    abstract GetNextItem: correlationId:CorrelationId * stepData:Dict<string,obj> -> 'TFeedItem

type StepContext<'TConnection,'TFeedItem> = {
    CorrelationId: CorrelationId
    CancellationToken: CancellationToken
    Connection: 'TConnection
    Data: Dict<string,obj>
    FeedItem: 'TFeedItem
    Logger: ILogger
} with
  member x.GetPreviousStepResponse<'T>() =
      x.Data.[Constants.StepResponseKey] :?> 'T

type ScenarioContext = {
    NodeInfo: NodeInfo
    CustomSettings: string
    CancellationToken: CancellationToken
    Logger: ILogger
}

type IStep =
    abstract StepName: string

type LoadSimulation =
    | KeepConcurrentScenarios of copiesCount:int * during:TimeSpan
    | RampConcurrentScenarios of copiesCount:int * during:TimeSpan
    | InjectScenariosPerSec   of copiesCount:int * during:TimeSpan
    | RampScenariosPerSec     of copiesCount:int * during:TimeSpan

type Scenario = {
    ScenarioName: string
    TestInit: (ScenarioContext -> Task) option
    TestClean: (ScenarioContext -> Task) option
    Steps: IStep list
    WarmUpDuration: TimeSpan
    LoadSimulations: LoadSimulation list
}

type ReportFile = {
    FilePath: string
    ReportFormat: ReportFormat
}

type IReportingSink =
    abstract Init: logger:ILogger * infraConfig:IConfiguration option -> unit
    abstract StartTest: testInfo:TestInfo -> Task
    abstract SaveRealtimeStats: testInfo:TestInfo * stats:Statistics[] -> Task
    abstract SaveFinalStats: testInfo:TestInfo * stats:Statistics[] * reportFiles:ReportFile[] -> Task
    abstract FinishTest: testInfo:TestInfo -> Task

type TestContext = {
    TestSuite: string
    TestName: string
    RegisteredScenarios: Scenario list
    TestConfig: TestConfig option
    InfraConfig: IConfiguration option
    ReportFileName: string option
    ReportFormats: ReportFormat list
    ReportingSinks: IReportingSink list
    SendStatsInterval: TimeSpan
}

type Response with

    static member Ok([<Optional;DefaultParameterValue(null:obj)>]payload: obj,
                     [<Optional;DefaultParameterValue(0:int)>]sizeBytes: int,
                     [<Optional;DefaultParameterValue(0:int)>]latencyMs: int) =
        { Payload = payload
          SizeBytes = sizeBytes
          Exception = ValueNone
          LatencyMs = latencyMs }

    static member Ok(payload: byte[],
                     [<Optional;DefaultParameterValue(0:int)>]latencyMs: int) =
        { Payload = payload
          SizeBytes = if isNull payload then 0 else payload.Length
          Exception = ValueNone
          LatencyMs = latencyMs }

    static member Fail() =
        { Payload = null
          SizeBytes = 0
          Exception = ValueSome(Exception "unknown client's error")
          LatencyMs = 0 }

    static member Fail(ex: Exception) =
        { Payload = null
          SizeBytes = 0
          Exception = ValueSome(ex)
          LatencyMs = 0 }

    static member Fail(reason: string) =
        { Payload = null
          SizeBytes = 0
          Exception = ValueSome(Exception reason)
          LatencyMs = 0 }
