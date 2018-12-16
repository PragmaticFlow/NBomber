module internal NBomber.Domain.DomainTypes

open System
open System.Threading.Tasks
open NBomber.Contracts

module Constants =   

    [<Literal>]
    let DefaultScenarioDurationInSec = 20.0

    [<Literal>]
    let DefaultConcurrentCopies = 50

    [<Literal>]
    let DefaultWarmUpDurationInSec = 5.0

    [<Literal>]
    let DefaultConnectionsCount = 0

type CorrelationId = string
type StepName = string
type FlowName = string
type ScenarioName = string
type Latency = int

[<CustomEquality; NoComparison>]
type ConnectionPool<'TConnection> = {
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

type UpdatesChannel() =
    
    let mutable tcs = TaskCompletionSource<Response>()

    member x.GetResponse() = 
        tcs <- TaskCompletionSource<Response>()
        tcs.Task

    interface IUpdatesChannel with
        member x.ReceivedUpdate(response: Response) = 
            if not tcs.Task.IsCompleted then tcs.SetResult(response)

type PullStep = {
    StepName: StepName    
    ConnectionPool: ConnectionPool<obj>
    Execute: PullContext<obj> -> Task<Response>
    CurrentContext: PullContext<obj> option
}

type PushStep = {
    StepName: StepName
    ConnectionPool: ConnectionPool<obj>        
    Handler: PushContext<obj> -> Task    
    CurrentContext: PushContext<obj> option
}

type Step =
    | Pull  of PullStep
    | Push  of PushStep
    | Pause of TimeSpan 
    interface IStep

type AssertFunc = AssertStats -> bool

type StepAssertion = {
    StepName: StepName
    ScenarioName: ScenarioName
    AssertFunc: AssertFunc
}

type ScenarioAssertion = {
    ScenarioName: ScenarioName
    AssertFunc: AssertFunc
}

type Assertion = 
    | Step     of StepAssertion    
    interface IAssertion

type Scenario = {    
    ScenarioName: ScenarioName
    TestInit: (unit -> unit) option  
    TestClean: (unit -> unit) option  
    Steps: Step[]
    Assertions: Assertion[]
    ConcurrentCopies: int
    CorrelationIds: CorrelationId[]
    Duration: TimeSpan
}

type DomainError =    
    | InitScenarioError  of ex:exn    
    | CleanScenarioError of ex:exn    
    | AssertNotFound of assertNumber:int * assertion:Assertion
    | AssertionError of assertNumber:int * assertion:Assertion * stats:AssertStats

type ValidationResult = {
    Number: int
    Result: Result<Assertion, DomainError>
}

type StepValidationResult = { 
    StepName: string
    Stats: AssertStats
    ValidationResults: ValidationResult[]
}

type ScenarioValidationResult = { 
    ScenarioName: string
    StepResults: StepValidationResult[]
}
