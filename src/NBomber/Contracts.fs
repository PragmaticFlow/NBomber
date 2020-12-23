namespace NBomber.Contracts

open System
open System.Data
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open Microsoft.Extensions.Configuration
open Serilog

open NBomber.Configuration
open NBomber.Extensions.InternalExtensions

type CorrelationId = {
    Id: string
    ScenarioName: string
    CopyNumber: int
}

type Response = {
    mutable Payload: obj
    SizeBytes: int
    Exception: exn option
    ErrorCode: int
    LatencyMs: int
}

type TestInfo = {
    SessionId: string
    TestSuite: string
    TestName: string
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
    NodeType: NodeType
    CurrentOperation: NodeOperationType
    OS: OperatingSystem
    DotNetVersion: string
    Processor: string
    CoresCount: int
    NBomberVersion: string
}

type ErrorCode = int

type ErrorStats = {
    ErrorCode: ErrorCode
    Message: string
    Count: int
}

type StepStats = {
    StepName: string
    RequestCount: int
    OkCount: int
    FailCount: int
    Min: int
    Mean: int
    Max: int
    RPS: int
    Percent50: int
    Percent75: int
    Percent95: int
    Percent99: int
    StdDev: int
    MinDataKb: float
    MeanDataKb: float
    MaxDataKb: float
    AllDataMB: float
    ErrorStats: ErrorStats[]
}

type LatencyCount = {
    Less800: int
    More800Less1200: int
    More1200: int
}

type LoadSimulationStats = {
    SimulationName: string
    Value: int
}

type ScenarioStats = {
    ScenarioName: string
    RequestCount: int
    OkCount: int
    FailCount: int
    AllDataMB: float
    StepStats: StepStats[]
    LatencyCount: LatencyCount
    LoadSimulationStats: LoadSimulationStats
    ErrorStats: ErrorStats[]
    Duration: TimeSpan
}

type ReportFile = {
    FilePath: string
    ReportFormat: ReportFormat
}

type NodeStats = {
    RequestCount: int
    OkCount: int
    FailCount: int
    AllDataMB: float
    ScenarioStats: ScenarioStats[]
    PluginStats: DataSet[]
    NodeInfo: NodeInfo
    TestInfo: TestInfo
    ReportFiles: ReportFile[]
}

type IConnectionPoolArgs<'TConnection> =
    abstract PoolName: string
    abstract ConnectionCount: int
    abstract OpenConnection: number:int * cancellationToken:CancellationToken -> Task<'TConnection>
    abstract CloseConnection: connection:'TConnection * cancellationToken:CancellationToken -> Task

type IFeedProvider<'TFeedItem> =
    abstract GetAllItems: unit -> 'TFeedItem seq

type IFeed<'TFeedItem> =
    abstract FeedName: string
    abstract Init: unit -> Task
    abstract GetNextItem: correlationId:CorrelationId * stepData:Dict<string,obj> -> 'TFeedItem

type IStepContext<'TConnection,'TFeedItem> =
    /// It's unique identifier which represent current scenario thread
    /// correlation_id = scenario_name + scenario_copy_number
    abstract CorrelationId: CorrelationId
    /// Cancellation token is a standard mechanics for canceling long-running operations.
    /// Cancellation token should be used to help NBomber stop scenarios when the test is finished.
    abstract CancellationToken: CancellationToken
    /// Connection which is taken from attached ConnectionPool.
    abstract Connection: 'TConnection
    /// Step's dictionary which you can use to share data between steps (within one scenario).
    abstract Data: Dict<string,obj>
    /// Feed item taken from attached feed.
    abstract FeedItem: 'TFeedItem
    /// NBomber's logger.
    abstract Logger: ILogger
    /// Returns the invocations number of the current step.
    abstract InvocationCount: int
    /// Returns response from previous step.
    abstract GetPreviousStepResponse: unit -> 'T
    /// Stops scenario by scenario name.
    /// It could be useful when you don't know the final scenario duration or it depends on some other criteria (notification event etc).
    abstract StopScenario: scenarioName:string * reason:string -> unit
    /// Stops all running scenarios.
    /// Use it when you don't see any sense to continue the current test.
    abstract StopCurrentTest: reason:string -> unit

type IScenarioContext =
    /// Gets current node info
    abstract NodeInfo: NodeInfo
    /// Gets client settings content from configuration file
    abstract CustomSettings: IConfiguration
    /// Cancellation token is a standard mechanics for canceling long-running operations.
    /// Cancellation token should be used to help NBomber stop scenarios when the test is finished.
    abstract CancellationToken: CancellationToken
    /// NBomber's logger
    abstract Logger: ILogger

type IStep =
    abstract StepName: string
    abstract DoNotTrack: bool

type LoadSimulation =
    /// Injects a given number of scenario copies with a linear ramp over a given duration. Use it for ramp up and rump down.
    | RampConstant of copies:int * during:TimeSpan
    /// Injects a given number of scenario copies at once and keep them running, during a given duration.
    | KeepConstant of copies:int * during:TimeSpan
    /// Injects a given number of scenario copies from the current rate to target rate, defined in scenarios per second, during a given duration.
    | RampPerSec   of rate:int * during:TimeSpan
    /// Injects a given number of scenario copies at a constant rate, defined in scenarios per second, during a given duration.
    | InjectPerSec of rate:int * during:TimeSpan
    /// Injects a random number of scenario copies at a constant rate, defined in scenarios per second, during a given duration.
    | InjectPerSecRandom of minRate:int * maxRate:int * during:TimeSpan

type Scenario = {
    ScenarioName: string
    Init: (IScenarioContext -> Task) option
    Clean: (IScenarioContext -> Task) option
    Steps: IStep list
    WarmUpDuration: TimeSpan
    LoadSimulations: LoadSimulation list
}

type IReportingSink =
    inherit IDisposable
    abstract SinkName: string
    abstract Init: logger:ILogger * infraConfig:IConfiguration option -> unit
    abstract Start: testInfo:TestInfo -> Task
    abstract SaveStats: stats:NodeStats[] -> Task
    abstract Stop: unit -> Task

type IWorkerPlugin =
    inherit IDisposable
    abstract PluginName: string
    abstract Init: logger:ILogger * infraConfig:IConfiguration option -> unit
    abstract Start: testInfo:TestInfo -> Task
    abstract GetStats: unit -> DataSet
    abstract GetHints: unit -> string[]
    abstract Stop: unit -> Task

type ApplicationType =
    | Process = 0
    | Console = 1

type NBomberContext = {
    TestSuite: string
    TestName: string
    RegisteredScenarios: Scenario list
    NBomberConfig: NBomberConfig option
    InfraConfig: IConfiguration option
    CreateLoggerConfig: (unit -> LoggerConfiguration) option
    ReportFileName: string option
    ReportFolder: string option
    ReportFormats: ReportFormat list
    ReportingSinks: IReportingSink list
    SendStatsInterval: TimeSpan
    WorkerPlugins: IWorkerPlugin list
    ApplicationType: ApplicationType option
    UseHintsAnalyzer: bool
}

type Response with

    static member Ok([<Optional;DefaultParameterValue(null:obj)>]payload: obj,
                     [<Optional;DefaultParameterValue(0:int)>]sizeBytes: int,
                     [<Optional;DefaultParameterValue(0:int)>]latencyMs: int) =
        { Payload = payload
          SizeBytes = sizeBytes
          Exception = None
          ErrorCode = 0
          LatencyMs = latencyMs }

    static member Ok(payload: byte[],
                     [<Optional;DefaultParameterValue(0:int)>]latencyMs: int) =
        { Payload = payload
          SizeBytes = if isNull payload then 0 else payload.Length
          Exception = None
          ErrorCode = 0
          LatencyMs = latencyMs }

    static member Fail() =
        { Payload = null
          SizeBytes = 0
          Exception = Some(Exception "unknown client's error")
          ErrorCode = 0
          LatencyMs = 0 }

    static member Fail(ex: Exception,
                       [<Optional;DefaultParameterValue(0:int)>]errorCode: int) =
        { Payload = null
          SizeBytes = 0
          Exception = Some(ex)
          ErrorCode = errorCode
          LatencyMs = 0 }

    static member Fail(reason: string,
                       [<Optional;DefaultParameterValue(0:int)>]errorCode: int) =
        { Payload = null
          SizeBytes = 0
          Exception = Some(Exception reason)
          ErrorCode = errorCode
          LatencyMs = 0 }
