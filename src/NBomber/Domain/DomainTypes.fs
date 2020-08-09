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
                                          execStopCommand: StopCommand -> unit) =
    interface IStepContext<'TConnection,'TFeedItem> with
        member x.CorrelationId = correlationId
        member x.CancellationToken = cancellationToken
        member x.Connection = connection
        member x.Data = data
        member x.FeedItem = feedItem
        member x.Logger = logger

        member x.GetPreviousStepResponse<'T>() =
            try
                data.[Constants.StepResponseKey] :?> 'T
            with
            | ex -> Unchecked.defaultof<'T>

        member x.StopScenario(scenarioName, reason) = StopScenario(scenarioName, reason) |> execStopCommand
        member x.StopCurrentTest(reason) = StopTest(reason) |> execStopCommand

    member x.ExecStopCommand(command) = execStopCommand(command)

type Step = {
    StepName: StepName
    ConnectionPoolArgs: ConnectionPoolArgs<obj> option
    ConnectionPool: ConnectionPool option
    Execute: StepContext<obj,obj> -> Task<Response>
    Context: StepContext<obj,obj> option
    Feed: IFeed<obj>
    DoNotTrack: bool
} with
    interface IStep with
        member x.StepName = x.StepName

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
