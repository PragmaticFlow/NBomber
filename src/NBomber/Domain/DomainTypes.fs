namespace NBomber.Domain

open System
open System.Threading
open System.Threading.Tasks
open NBomber.Contracts

module internal Constants =   

    [<Literal>]
    let DefaultScenarioDurationInSec = 20.0

    [<Literal>]
    let DefaultConcurrentCopies = 50

    [<Literal>]
    let DefaultWarmUpDurationInSec = 5.0

    [<Literal>]
    let DefaultConnectionsCount = 0
    
    [<Literal>]
    let PauseStepName = "pause"

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
    ConnectionsCount: int
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
} with interface IStep

type internal AssertFunc = Statistics -> bool

type internal StepAssertion = {
    StepName: StepName
    ScenarioName: ScenarioName
    AssertFunc: AssertFunc
    Label: string option
}

type internal Assertion = 
    | Step of StepAssertion    
    interface IAssertion

type internal Scenario = {    
    ScenarioName: ScenarioName
    TestInit: (CancellationToken -> Task) option  
    TestClean: (CancellationToken -> Task) option  
    Steps: Step[]
    Assertions: Assertion[]
    ConcurrentCopies: int
    CorrelationIds: CorrelationId[]
    WarmUpDuration: TimeSpan
    Duration: TimeSpan
}