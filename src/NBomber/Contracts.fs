namespace NBomber.Contracts

open System
open System.Data
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open Serilog
open Microsoft.Extensions.Configuration

open NBomber.Configuration
open NBomber.Extensions.InternalExtensions

type ScenarioThreadId = {
    Id: string
    ThreadNumber: int
}

[<Struct>]
type Response = {
    StatusCode: Nullable<int>
    IsError: bool
    ErrorMessage: string
    SizeBytes: int
    LatencyMs: float
    mutable Payload: obj
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
    MinKb: float
    MeanKb: float
    MaxKb: float
    Percent50: float
    Percent75: float
    Percent95: float
    Percent99: float
    StdDev: float
    AllMB: float
}

type OkStepStats = {
    Request: RequestStats
    Latency: LatencyStats
    DataTransfer: DataTransferStats
    StatusCodes: StatusCodeStats[]
}

type FailStepStats = {
    Request: RequestStats
    Latency: LatencyStats
    DataTransfer: DataTransferStats
    StatusCodes: StatusCodeStats[]
}

type StepStats = {
    StepName: string
    Ok: OkStepStats
    Fail: FailStepStats
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
    StatusCodes: StatusCodeStats[]
    CurrentOperation: OperationType
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
    Duration: TimeSpan
}

type IBaseContext =
    /// Gets current test info
    abstract TestInfo: TestInfo
    /// Gets current node info
    abstract NodeInfo: NodeInfo
    /// Cancellation token is a standard mechanics for canceling long-running operations.
    /// Cancellation token should be used to help NBomber stop scenarios when the test is finished.
    abstract CancellationToken: CancellationToken
    /// NBomber's logger
    abstract Logger: ILogger

type IClientFactory<'TClient> =
    abstract FactoryName: string
    abstract ClientCount: int
    abstract InitClient: number:int * context:IBaseContext -> Task<'TClient>
    abstract DisposeClient: client:'TClient * context:IBaseContext -> Task

type IFeed<'TFeedItem> =
    abstract FeedName: string
    abstract Init: context:IBaseContext -> Task
    abstract GetNextItem: scenarioThreadId:ScenarioThreadId * stepData:Dict<string,obj> -> 'TFeedItem

type IStepContext<'TClient,'TFeedItem> =
    /// It's unique identifier that represent current scenario thread.
    /// You can use it as a correlation id.
    abstract ScenarioThreadId: ScenarioThreadId
    /// Cancellation token is a standard mechanics for canceling long-running operations.
    /// Cancellation token should be used to help NBomber stop scenarios when the test is finished.
    abstract CancellationToken: CancellationToken
    /// Client which is taken from the ClientPool.
    abstract Client: 'TClient
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
    /// Gets current test info
    abstract TestInfo: TestInfo
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
    GetStepsOrder: unit -> int[]
}

type IReportingSink =
    inherit IDisposable
    abstract SinkName: string
    abstract Init: context:IBaseContext * infraConfig:IConfiguration -> Task
    abstract Start: unit -> Task
    abstract SaveRealtimeStats: stats:ScenarioStats[] -> Task
    abstract SaveFinalStats: stats:NodeStats[] -> Task
    abstract Stop: unit -> Task

type IWorkerPlugin =
    inherit IDisposable
    abstract PluginName: string
    abstract Init: context:IBaseContext * infraConfig:IConfiguration -> Task
    abstract Start: unit -> Task
    abstract GetStats: currentOperation:OperationType -> Task<DataSet>
    abstract GetHints: unit -> string[]
    abstract Stop: unit -> Task

type ApplicationType =
    | Process = 0
    | Console = 1

type ReportingContext = {
    FolderName: string option
    FileName: string option
    Sinks: IReportingSink list
    Formats: ReportFormat list
    SendStatsInterval: TimeSpan
}

type NBomberContext = {
    TestSuite: string
    TestName: string
    RegisteredScenarios: Scenario list
    NBomberConfig: NBomberConfig option
    InfraConfig: IConfiguration option
    CreateLoggerConfig: (unit -> LoggerConfiguration) option
    Reporting: ReportingContext
    WorkerPlugins: IWorkerPlugin list
    ApplicationType: ApplicationType option
    UseHintsAnalyzer: bool
}

type Response with

    [<CompiledName("Ok")>]
    static member ok([<Optional;DefaultParameterValue(null)>] payload: obj,
                     [<Optional;DefaultParameterValue(Nullable<int>())>] statusCode: Nullable<int>,
                     [<Optional;DefaultParameterValue(0)>] sizeBytes: int,
                     [<Optional;DefaultParameterValue(0.0)>] latencyMs: float) =

        { StatusCode = statusCode
          IsError = false
          SizeBytes = sizeBytes
          ErrorMessage = String.Empty
          LatencyMs = latencyMs
          Payload = payload }

    [<CompiledName("Ok")>]
    static member ok(payload: byte[],
                     [<Optional;DefaultParameterValue(Nullable<int>())>] statusCode: Nullable<int>,
                     [<Optional;DefaultParameterValue(0.0)>] latencyMs: float) =

        { StatusCode = statusCode
          IsError = false
          SizeBytes = if isNull payload then 0 else payload.Length
          ErrorMessage = String.Empty
          LatencyMs = latencyMs
          Payload = payload }

    [<CompiledName("Fail")>]
    static member fail([<Optional;DefaultParameterValue("")>] error: string,
                       [<Optional;DefaultParameterValue(Nullable<int>())>] statusCode: Nullable<int>,
                       [<Optional;DefaultParameterValue(0)>] sizeBytes: int,
                       [<Optional;DefaultParameterValue(0.0)>] latencyMs: float) =

        { StatusCode = statusCode
          IsError = true
          SizeBytes = sizeBytes
          ErrorMessage = if isNull error then String.Empty else error
          LatencyMs = latencyMs
          Payload = null }

    [<CompiledName("Fail")>]
    static member fail(error: Exception,
                       [<Optional;DefaultParameterValue(Nullable<int>())>] statusCode: Nullable<int>,
                       [<Optional;DefaultParameterValue(0)>] sizeBytes: int,
                       [<Optional;DefaultParameterValue(0.0)>] latencyMs: float) =

        { StatusCode = statusCode
          IsError = true
          SizeBytes = sizeBytes
          ErrorMessage = if isNull error then String.Empty else error.Message
          LatencyMs = latencyMs
          Payload = null }
