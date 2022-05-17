module internal NBomber.Domain.DomainTypes

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks

open HdrHistogram
open Serilog

open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Contracts.Thresholds
open NBomber.Domain.ClientFactory
open NBomber.Domain.ClientPool

type SessionId = string
type ScenarioName = string

type StopCommand =
    | StopScenario of scenarioName:string * reason:string
    | StopTest of reason:string

type UntypedStepContext = {
    StepName: string
    ScenarioInfo: ScenarioInfo
    mutable CancellationTokenSource: CancellationTokenSource
    Logger: ILogger
    mutable Client: obj
    mutable FeedItem: obj
    mutable Data: Dictionary<string,obj>
    mutable InvocationCount: int
    StopScenario: string * string -> unit // scenarioName * reason
    StopCurrentTest: string -> unit       // reason
}

type Step = {
    StepName: string
    ClientFactory: ClientFactory<obj> option
    ClientDistribution: (IStepClientContext<obj> -> int) option
    ClientPool: ClientPool option
    Execute: UntypedStepContext -> Task<Response>
    Feed: IFeed<obj> option
    Timeout: TimeSpan
    DoNotTrack: bool
    IsPause: bool
} with

    interface IStep with
        member this.StepName = this.StepName
        member this.DoNotTrack = this.DoNotTrack

type RawStepStats = {
    mutable MinMicroSec: int
    mutable MaxMicroSec: int
    mutable MinBytes: int
    mutable MaxBytes: int
    mutable RequestCount: int
    mutable LessOrEq800: int
    mutable More800Less1200: int
    mutable MoreOrEq1200: int
    mutable AllBytes: int64
    LatencyHistogram: LongHistogram
    DataTransferHistogram: LongHistogram
    StatusCodes: Dictionary<int,StatusCodeStats>
}

type StepStatsRawData = {
    OkStats: RawStepStats
    FailStats: RawStepStats
}

type RunningStep = {
    StepIndex: int
    Value: Step
    Context: UntypedStepContext
}

type LoadTimeSegment = {
    StartTime: TimeSpan
    EndTime: TimeSpan
    Duration: TimeSpan
    PrevSegmentCopiesCount: int
    LoadSimulation: LoadSimulation
}

type LoadTimeLine = LoadTimeSegment list

[<Measure>] type scenarioDuration

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
    DefaultStepOrder: int[]
    StepOrderIndex: Dictionary<string,int> // stepName * orderNumber
    CustomStepOrder: (unit -> string[]) option
    CustomStepExecControl: (IStepExecControlContext voption -> string voption) option
    IsEnabled: bool // used for stats in the cluster mode
    Thresholds: Threshold list option
}
