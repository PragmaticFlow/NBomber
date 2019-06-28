namespace NBomber.Domain

open System
open System.Threading
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

    let AllReportFormats = [ReportFormat.Txt; ReportFormat.Html; ReportFormat.Csv; ReportFormat.Md]

    [<Literal>]
    let EmptyPoolName = "nbomber_empty_pool"

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
} with interface IStep

type internal StepResponse = {
    Response: Response
    StartTimeMs: float    
    LatencyMs: int
}

type internal AssertFunc = Statistics -> bool

type internal Assertion = {
    StepName: StepName
    ScenarioName: ScenarioName
    AssertFunc: AssertFunc
    Label: string option
} with interface IAssertion

type internal Scenario = {    
    ScenarioName: ScenarioName
    TestInit: (CancellationToken -> Task) option  
    TestClean: (CancellationToken -> Task) option  
    Steps: Step[]
    Assertions: Assertion[]
    ConcurrentCopies: int
    ThreadCount: int
    CorrelationIds: CorrelationId[]
    WarmUpDuration: TimeSpan
    Duration: TimeSpan
}