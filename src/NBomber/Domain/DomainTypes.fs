namespace NBomber.Domain

open System
open System.Threading
open System.Threading.Tasks

open Serilog
open NBomber.Contracts
open NBomber.Extensions
open NBomber.Domain.ConnectionPool

//todo: use opaque types
type internal StepName = string
type internal ScenarioName = string
type internal Latency = int

type internal UntypedStepContext = {
    CorrelationId: CorrelationId
    CancellationToken: CancellationToken
    Connection: obj
    mutable Data: Dict<string,obj>
    FeedItem: obj
    Logger: ILogger
}

type internal UntypedFeed = {
    Name: string
    GetNextItem: CorrelationId * Dict<string,obj> -> obj
}

type internal Step = {
    StepName: StepName
    ConnectionPool: ConnectionPool
    Execute: UntypedStepContext -> Task<Response>
    Context: UntypedStepContext option
    Feed: UntypedFeed
    RepeatCount: int
    DoNotTrack: bool
} with
    interface IStep with
        member x.StepName = x.StepName

type internal StepResponse = {
    Response: Response
    StartTimeMs: float
    LatencyMs: int
}

type internal LoadTimeLineItem = { EndTime: TimeSpan; LoadSimulation: LoadSimulation }
type internal LoadTimeLine = LoadTimeLineItem list

type internal Scenario = {
    ScenarioName: ScenarioName
    TestInit: (ScenarioContext -> Task) option
    TestClean: (ScenarioContext -> Task) option
    Steps: Step[]
    LoadTimeLine: LoadTimeLine
    WarmUpDuration: TimeSpan
    Duration: TimeSpan
}
