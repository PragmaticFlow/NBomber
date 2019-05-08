namespace NBomber.Contracts

open System
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks
open NBomber.Configuration

type Response = {
    IsOk: bool
    Payload: obj
    SizeBytes: int
}

type NodeType =
    | SingleNode
    | Coordinator
    | Agent
    | Cluster

type StatisticsMeta = {
    SessionId: string
    NodeName: string
    Sender: NodeType
}

type Statistics = {
    ScenarioName: string
    StepName: string
    OkCount: int
    FailCount: int
    Min: int
    Mean: int
    Max: int
    RPS: int
    Percent50: int
    Percent75: int
    Percent95: int
    StdDev: int
    DataMinKb: float
    DataMeanKb: float
    DataMaxKb: float
    AllDataMB: float
    Meta: StatisticsMeta
}

type IConnectionPool<'TConnection> = interface end

type StepContext<'TConnection> = {
    CorrelationId: string
    CancellationToken: CancellationToken
    Connection: 'TConnection
    mutable Payload: obj
}

type IStep = interface end
type IAssertion = interface end

type Scenario = {
    ScenarioName: string
    TestInit: (CancellationToken -> Task) option
    TestClean: (CancellationToken -> Task) option
    Steps: IStep[]
    Assertions: IAssertion[]
    ConcurrentCopies: int
    WarmUpDuration: TimeSpan
    Duration: TimeSpan
}

type IStatisticsSink =
    abstract SaveStatistics: Statistics[] -> Task

type NBomberContext = {
    Scenarios: Scenario[]
    NBomberConfig: NBomberConfig option
    ReportFileName: string option
    ReportFormats: ReportFormat list
    StatisticsSink: IStatisticsSink option
}

type Response with
    static member Ok([<Optional;DefaultParameterValue(null:obj)>]payload: obj, [<Optional;DefaultParameterValue(0:int)>]sizeBytes: int) = { IsOk = true; Payload = payload; SizeBytes = sizeBytes }
    static member Ok(payload: byte[]) = { IsOk = true; Payload = payload; SizeBytes = if isNull payload then 0 else Array.length payload}
    static member Fail() = { IsOk = false; Payload = null; SizeBytes = 0 }
