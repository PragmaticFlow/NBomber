namespace NBomber.Contracts

open System
open System.Data
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open Microsoft.Extensions.Configuration
open Serilog

open NBomber.Configuration
open NBomber.Extensions

type CorrelationId = {
    Id: string
    ScenarioName: string
    CopyNumber: int
}

type Response = {
    mutable Payload: obj
    SizeBytes: int
    Exception: exn option
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
    Duration: TimeSpan
    NodeInfo: NodeInfo
}

type PluginStatistics = DataSet

type IConnectionPoolArgs<'TConnection> =
    abstract PoolName: string
    abstract GetConnectionCount: unit -> int
    abstract OpenConnection: number:int * cancellationToken:CancellationToken -> Task<'TConnection>
    abstract CloseConnection: connection:'TConnection * cancellationToken:CancellationToken -> Task

type IFeedProvider<'TFeedItem> =
    abstract GetAllItems: unit -> 'TFeedItem[]

type IFeed<'TFeedItem> =
    abstract Name: string
    abstract GetNextItem: correlationId:CorrelationId * stepData:Dict<string,obj> -> 'TFeedItem

type IStepContext<'TConnection,'TFeedItem> =
    abstract CorrelationId: CorrelationId
    abstract CancellationToken: CancellationToken
    abstract Connection: 'TConnection
    abstract Data: Dict<string,obj>
    abstract FeedItem: 'TFeedItem
    abstract Logger: ILogger
    abstract GetPreviousStepResponse: unit -> 'T
    abstract StopScenario: scenarioName:string * reason:string -> unit
    abstract StopTest: reason:string -> unit

type ScenarioContext = {
    NodeInfo: NodeInfo
    CustomSettings: string
    CancellationToken: CancellationToken
    Logger: ILogger
}

type IStep =
    abstract StepName: string

type LoadSimulation =
    | RampConcurrentScenarios of copiesCount:int * during:TimeSpan
    | KeepConcurrentScenarios of copiesCount:int * during:TimeSpan
    | RampScenariosPerSec     of copiesCount:int * during:TimeSpan
    | InjectScenariosPerSec   of copiesCount:int * during:TimeSpan

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
    abstract SaveFinalStats: testInfo:TestInfo * stats:Statistics[] * pluginStats:PluginStatistics[] * reportFiles:ReportFile[] -> Task
    abstract FinishTest: testInfo:TestInfo -> Task

type IPlugin =
    abstract Init: logger:ILogger * infraConfig:IConfiguration option -> unit
    abstract StartTest: testInfo:TestInfo -> Task
    abstract GetStats: testInfo:TestInfo -> Task<PluginStatistics>
    abstract FinishTest: testInfo:TestInfo -> Task

type TestContext = {
    TestSuite: string
    TestName: string
    RegisteredScenarios: Scenario list
    NBomberConfig: NBomberConfig option
    InfraConfig: IConfiguration option
    ReportFileName: string option
    ReportFormats: ReportFormat list
    ReportingSinks: IReportingSink list
    SendStatsInterval: TimeSpan
    Plugins: IPlugin list
}

type Response with

    static member Ok([<Optional;DefaultParameterValue(null:obj)>]payload: obj,
                     [<Optional;DefaultParameterValue(0:int)>]sizeBytes: int,
                     [<Optional;DefaultParameterValue(0:int)>]latencyMs: int) =
        { Payload = payload
          SizeBytes = sizeBytes
          Exception = None
          LatencyMs = latencyMs }

    static member Ok(payload: byte[],
                     [<Optional;DefaultParameterValue(0:int)>]latencyMs: int) =
        { Payload = payload
          SizeBytes = if isNull payload then 0 else payload.Length
          Exception = None
          LatencyMs = latencyMs }

    static member Fail() =
        { Payload = null
          SizeBytes = 0
          Exception = Some(Exception "unknown client's error")
          LatencyMs = 0 }

    static member Fail(ex: Exception) =
        { Payload = null
          SizeBytes = 0
          Exception = Some(ex)
          LatencyMs = 0 }

    static member Fail(reason: string) =
        { Payload = null
          SizeBytes = 0
          Exception = Some(Exception reason)
          LatencyMs = 0 }
