namespace NBomber.Domain

open System
open System.Threading.Tasks

open NBomber.Contracts
open NBomber.Domain.ConnectionPool

//todo: use opaque types
type internal StepName = string
type internal ScenarioName = string
type internal Latency = int

type internal Step = {
    StepName: StepName
    ConnectionPoolArgs: ConnectionPoolArgs<obj>
    ConnectionPool: ConnectionPool option
    Execute: StepContext<obj,obj> -> Task<Response>
    Context: StepContext<obj,obj> option
    Feed: IFeed<obj>
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
