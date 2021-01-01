module internal NBomber.Domain.DomainTypes

open System
open System.Threading
open System.Threading.Tasks

open Serilog

open NBomber.Contracts
open NBomber.Domain.ConnectionPool
open NBomber.Extensions.InternalExtensions

//todo: use opaque types
type StepName = string
type ScenarioName = string
type Latency = int

type StopCommand =
    | StopScenario of ScenarioName * reason:string
    | StopTest of reason:string

type UntypedStepContext = {
    CorrelationId: CorrelationId
    CancellationToken: CancellationToken
    Connection: obj
    Logger: ILogger
    mutable FeedItem: obj
    mutable Data: Dict<string,obj>
    mutable InvocationCount: int
    StopScenario: string * string -> unit // scenarioName * reason
    StopCurrentTest: string -> unit       // reason
}

type Step = {
    StepName: StepName
    ConnectionPoolArgs: ConnectionPoolArgs<obj> option
    ConnectionPool: ConnectionPool option
    Execute: UntypedStepContext -> Task<Response>
    Feed: IFeed<obj> option
    DoNotTrack: bool
} with
    interface IStep with
        member this.StepName = this.StepName
        member this.DoNotTrack = this.DoNotTrack

type RunningStep = {
    Value: Step
    Context: UntypedStepContext
}

type StepResponse = {
    Response: Response
    StartTimeMs: float
    LatencyMs: int
}

type LoadTimeSegment = {
    StartTime: TimeSpan
    EndTime: TimeSpan
    Duration: TimeSpan
    PrevSegmentCopiesCount: int
    LoadSimulation: LoadSimulation
}

type LoadTimeLine = LoadTimeSegment list

type Scenario = {
    ScenarioName: ScenarioName
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
