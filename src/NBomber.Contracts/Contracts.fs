namespace NBomber.Contracts

open System
open System.Collections.Generic
open System.Data
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open Serilog
open MessagePack
open Microsoft.Extensions.Configuration

open NBomber.Contracts.Stats

[<CLIMutable>]
[<MessagePackObject>]
type Response = {
    [<Key 0>] StatusCode: Nullable<int>
    [<Key 1>] IsError: bool
    [<Key 2>] SizeBytes: int
    [<Key 3>] LatencyMs: float
    [<IgnoreMember>] Message: string
    [<IgnoreMember>] Payload: obj
}

type ScenarioOperation =
    | WarmUp = 0
    | Bombing = 1

type ScenarioInfo = {
    /// Gets the current scenario thread id.
    /// You can use it as correlation id.
    ThreadId: string
    ThreadNumber: int
    ScenarioName: string
    ScenarioDuration: TimeSpan
    /// Returns info about current operation type.
    /// It can be: WarmUp or Bombing.
    ScenarioOperation: ScenarioOperation
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
    abstract GetNextItem: scenarioInfo:ScenarioInfo * stepData:Dictionary<string,obj> -> 'TFeedItem

type IStepContext<'TClient,'TFeedItem> =
    /// Gets the currently running step name.
    abstract StepName: string
    /// Gets info about the currently running scenario.
    /// You can use ScenarioInfo.ThreadId as correlation id.
    abstract ScenarioInfo: ScenarioInfo
    /// Cancellation token is a standard mechanics for canceling long-running operations.
    /// Cancellation token should be used to help NBomber stop scenarios when the test is finished.
    abstract CancellationToken: CancellationToken
    /// Client which is taken from the ClientPool.
    abstract Client: 'TClient
    /// Step's dictionary which you can use to share data between steps (within one scenario).
    abstract Data: Dictionary<string,obj>
    /// Feed item taken from attached feed.
    abstract FeedItem: 'TFeedItem
    /// NBomber's logger.
    abstract Logger: ILogger
    /// Returns the invocations number of the current step instance located in the current scenario instance.
    abstract InvocationCount: int
    /// Returns response from previous step.
    abstract GetPreviousStepResponse: unit -> 'T
    /// Stops scenario by scenario name.
    /// It could be useful when you don't know the final scenario duration or it depends on some other criteria (notification event etc).
    abstract StopScenario: scenarioName:string * reason:string -> unit
    /// Stops all running scenarios.
    /// Use it when you don't see any sense to continue the current test.
    abstract StopCurrentTest: reason:string -> unit

type IStepExecControlContext =
    abstract PrevStepContext: IStepContext<obj,obj>
    abstract PrevStepResponse: Response

type IStepClientContext<'TFeedItem> =
    abstract StepName: string
    abstract ScenarioInfo: ScenarioInfo
    abstract Logger: ILogger
    abstract Data: Dictionary<string,obj>
    abstract FeedItem: 'TFeedItem
    abstract InvocationNumber: int
    abstract ClientCount: int

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
    /// Injects a given number of scenario copies (threads) with a linear ramp over a given duration.
    /// Every single scenario copy will iterate while the specified duration.
    /// Use it for ramp up and rump down.
    | RampConstant of copies:int * during:TimeSpan
    /// A fixed number of scenario copies (threads) executes as many iterations as possible for a specified amount of time.
    /// Every single scenario copy will iterate while the specified duration.
    /// Use it when you need to run a specific amount of scenario copies (threads) for a certain amount of time.
    | KeepConstant of copies:int * during:TimeSpan
    /// Injects a given number of scenario copies (threads) per 1 sec from the current rate to target rate during a given duration.
    /// Every single scenario copy will run only once.
    | RampPerSec   of rate:int * during:TimeSpan
    /// Injects a given number of scenario copies (threads) per 1 sec during a given duration.
    /// Every single scenario copy will run only once.
    /// Use it when you want to maintain a constant rate of requests without being affected by the performance of the system under test.
    | InjectPerSec of rate:int * during:TimeSpan
    /// Injects a random number of scenario copies (threads) per 1 sec during a given duration.
    /// Every single scenario copy will run only once.
    /// Use it when you want to maintain a random rate of requests without being affected by the performance of the system under test.
    | InjectPerSecRandom of minRate:int * maxRate:int * during:TimeSpan

type Scenario = {
    ScenarioName: string
    Init: (IScenarioContext -> Task) option
    Clean: (IScenarioContext -> Task) option
    Steps: IStep list
    WarmUpDuration: TimeSpan option
    LoadSimulations: LoadSimulation list
    CustomStepOrder: (unit -> string[]) option
    CustomStepExecControl: (IStepExecControlContext voption -> string voption) option
}

type IReportingSink =
    inherit IDisposable
    abstract SinkName: string
    abstract Init: context:IBaseContext * infraConfig:IConfiguration -> Task
    abstract Start: unit -> Task
    abstract SaveRealtimeStats: stats:ScenarioStats[] -> Task
    abstract SaveFinalStats: stats:NodeStats -> Task
    abstract Stop: unit -> Task

type IWorkerPlugin =
    inherit IDisposable
    abstract PluginName: string
    abstract Init: context:IBaseContext * infraConfig:IConfiguration -> Task
    abstract Start: unit -> Task
    abstract GetStats: stats:NodeStats -> Task<DataSet>
    abstract GetHints: unit -> string[]
    abstract Stop: unit -> Task

type ApplicationType =
    | Process = 0
    | Console = 1

type Response with

    [<CompiledName("Ok")>]
    static member ok([<Optional;DefaultParameterValue(null)>] payload: obj,
                     [<Optional;DefaultParameterValue(Nullable<int>())>] statusCode: Nullable<int>,
                     [<Optional;DefaultParameterValue(0)>] sizeBytes: int,
                     [<Optional;DefaultParameterValue(0.0)>] latencyMs: float,
                     [<Optional;DefaultParameterValue("")>] message: string) =

        { StatusCode = statusCode
          IsError = false
          SizeBytes = sizeBytes
          Message = if isNull message then String.Empty else message
          LatencyMs = latencyMs
          Payload = payload }

    [<CompiledName("Ok")>]
    static member ok(payload: byte[],
                     [<Optional;DefaultParameterValue(Nullable<int>())>] statusCode: Nullable<int>,
                     [<Optional;DefaultParameterValue(0.0)>] latencyMs: float,
                     [<Optional;DefaultParameterValue("")>] message: string) =

        { StatusCode = statusCode
          IsError = false
          SizeBytes = if isNull payload then 0 else payload.Length
          Message = if isNull message then String.Empty else message
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
          Message = if isNull error then String.Empty else error
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
          Message = if isNull error then String.Empty else error.Message
          LatencyMs = latencyMs
          Payload = null }
