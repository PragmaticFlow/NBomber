namespace NBomber.Domain

open System
open System.Threading.Tasks
open NBomber.Contracts

module internal Constants =
    open NBomber.Configuration

    [<Literal>]
    let DefaultScenarioDurationInSec = 60.0

    [<Literal>]
    let DefaultConcurrentCopies = 50

    [<Literal>]
    let DefaultWarmUpDurationInSec = 10.0

    [<Literal>]
    let DefaultRepeatCount = 0

    [<Literal>]
    let DefaultDoNotTrack = false

    let AllReportFormats = [|ReportFormat.Txt; ReportFormat.Html; ReportFormat.Csv; ReportFormat.Md|]

    [<Literal>]
    let EmptyPoolName = "nbomber_empty_pool"

    [<Literal>]
    let DefaultTestSuite = "NBomberTestSuite"

    [<Literal>]
    let DefaultTestName = "NBomberLoadTest"

    [<Literal>]
    let MinSendStatsIntervalSec = 5.0

type internal CorrelationId = string
type internal StepName = string
type internal FlowName = string
type internal ScenarioName = string
type internal Latency = int

[<CustomEquality; NoComparison>]
type internal ConnectionPool<'TConnection> = {
    PoolName: string
    OpenConnection: unit -> 'TConnection
    CloseConnection: ('TConnection -> unit) option
    ConnectionsCount: int option
    AliveConnections: 'TConnection[]
} with
  interface IConnectionPool<'TConnection>
  override x.GetHashCode() = x.PoolName.GetHashCode()
  override x.Equals(b) =
    match b with
    | :? ConnectionPool<'TConnection> as pool -> x.PoolName = pool.PoolName
    | _ -> false

type internal Step = {
    StepName: StepName
    ConnectionPool: ConnectionPool<obj>
    Execute: StepContext<obj> -> Task<Response>
    CurrentContext: StepContext<obj> option
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

type internal Scenario = {
    ScenarioName: ScenarioName
    TestInit: (ScenarioContext -> Task) option
    TestClean: (ScenarioContext -> Task) option
    Feed : IFeed<obj>
    Steps: Step[]
    ConcurrentCopies: int
    CorrelationIds: CorrelationId[]
    WarmUpDuration: TimeSpan
    Duration: TimeSpan
}
