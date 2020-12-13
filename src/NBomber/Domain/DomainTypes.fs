module internal NBomber.Domain.DomainTypes

open System
open System.Threading.Tasks

open NBomber
open NBomber.Contracts
open NBomber.Domain.ConnectionPool

//todo: use opaque types
type StepName = string
type ScenarioName = string
type Latency = int

type StopCommand =
    | StopScenario of ScenarioName * reason:string
    | StopTest of reason:string

type StepContext<'TConnection,'TFeedItem>(correlationId, cancellationToken,
                                          connection, data, feedItem, logger,
                                          invocationCount, execStopCommand: StopCommand -> unit) =
    interface IStepContext<'TConnection,'TFeedItem> with
        member _.CorrelationId = correlationId
        member _.CancellationToken = cancellationToken
        member _.Connection = connection
        member _.Data = data
        member _.FeedItem = feedItem
        member _.Logger = logger
        member _.InvocationCount = invocationCount

        member _.GetPreviousStepResponse<'T>() =
            try
                let prevStepResponse = data.[Constants.StepResponseKey]
                if isNull prevStepResponse then
                    Unchecked.defaultof<'T>
                else
                    prevStepResponse :?> 'T
            with
            | ex -> Unchecked.defaultof<'T>

        member _.StopScenario(scenarioName, reason) = StopScenario(scenarioName, reason) |> execStopCommand
        member _.StopCurrentTest(reason) = StopTest(reason) |> execStopCommand

    member _.ExecStopCommand(command) = execStopCommand(command)

type Step = {
    StepName: StepName
    ConnectionPoolArgs: ConnectionPoolArgs<obj> option
    ConnectionPool: ConnectionPool option
    Execute: StepContext<obj,obj> -> Task<Response>
    Feed: IFeed<obj>
    DoNotTrack: bool
} with
    interface IStep with
        member this.StepName = this.StepName
        member this.DoNotTrack = this.DoNotTrack

type RunningStep = {
    Value: Step
    mutable Context: StepContext<obj,obj>
    mutable InvocationCount: int
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
}
