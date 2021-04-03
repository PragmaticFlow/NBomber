module internal NBomber.Domain.DomainTypes

open System
open System.Collections.Generic
open System.Data
open System.Threading
open System.Threading.Tasks

open HdrHistogram
open Serilog

open NBomber.Contracts
open NBomber.Domain.ClientPool
open NBomber.Extensions.InternalExtensions

[<Measure>] type ticks
[<Measure>] type ms
[<Measure>] type bytes
[<Measure>] type kb
[<Measure>] type mb

type StopCommand =
    | StopScenario of scenarioName:string * reason:string
    | StopTest of reason:string

type UntypedStepContext = {
    ScenarioThreadId: ScenarioThreadId
    CancellationToken: CancellationToken
    Client: obj
    Logger: ILogger
    mutable FeedItem: obj
    mutable Data: Dict<string,obj>
    mutable InvocationCount: int
    StopScenario: string * string -> unit // scenarioName * reason
    StopCurrentTest: string -> unit       // reason
}

type StepExecution =
    | SyncExec  of (UntypedStepContext -> Response)
    | AsyncExec of (UntypedStepContext -> Task<Response>)

type Step = {
    StepName: string
    ClientFactory: ClientFactory<obj> option
    ClientPool: ClientPool option
    Execute: StepExecution
    Feed: IFeed<obj> option
    DoNotTrack: bool
} with
    interface IStep with
        member this.StepName = this.StepName
        member this.DoNotTrack = this.DoNotTrack

type RawStepStats = {
    mutable RequestCount: int
    mutable LessOrEq800: int
    mutable More800Less1200: int
    mutable MoreOrEq1200: int
    mutable AllMB: float<mb>
    LatencyHistogramTicks: LongHistogram
    DataTransferBytes: LongHistogram
    StatusCodes: Dictionary<int,StatusCodeStats>
}

type StepExecutionData = {
    OkStats: RawStepStats
    FailStats: RawStepStats
}

type RunningStep = {
    Value: Step
    Context: UntypedStepContext
    mutable ExecutionData: StepExecutionData
}

[<Struct>]
type StepResponse = {
    ClientResponse: Response
    EndTimeTicks: int64<ticks>
    LatencyTicks: int64<ticks>
}

type LoadTimeSegment = {
    StartTime: TimeSpan
    EndTime: TimeSpan
    Duration: TimeSpan
    PrevSegmentCopiesCount: int
    LoadSimulation: LoadSimulation
}

type LoadTimeLine = LoadTimeSegment list

type TimeLineHistoryRecord = {
    Duration: TimeSpan
    ScenarioStats: ScenarioStats[]
    PluginStats: DataSet[]
}

type Scenario = {
    ScenarioName: string
    Init: (IScenarioContext -> Task) option
    Clean: (IScenarioContext -> Task) option
    Steps: Step list
    LoadTimeLine: LoadTimeLine
    WarmUpDuration: TimeSpan
    PlanedDuration: TimeSpan
    ExecutedDuration: TimeSpan option
    CustomSettings: string
    GetStepsOrder: unit -> int[]
}
