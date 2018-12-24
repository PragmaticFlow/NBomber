namespace NBomber.Contracts

open System
open System.Runtime.InteropServices
open NBomber.Configuration

type Response = {
    IsOk: bool
    Payload: obj
    SizeKB: int
}

type IUpdatesChannel =
    abstract ReceivedUpdate: update:Response -> unit

type IConnectionPool<'TConnection> = interface end

type PullContext<'TConnection> = {
    CorrelationId: string
    Connection: 'TConnection
    mutable Payload: obj
}

type PushContext<'TConnection> = {
    CorrelationId: string
    Connection: 'TConnection
    UpdatesChannel: IUpdatesChannel
}

type IStep = interface end
type IAssertion = interface end  

type Scenario = {
    ScenarioName: string    
    TestInit: (unit -> unit) option
    TestClean: (unit -> unit) option
    Steps: IStep[]
    Assertions: IAssertion[]
    ConcurrentCopies: int
    Duration: TimeSpan
}

type AssertStats = {
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
    DataMinKB: int
    DataMeanKB: int
    DataMaxKB: int
    AllDataMB: float
}
    
type Response with
    static member Ok([<Optional;DefaultParameterValue(null:obj)>]payload: obj, [<Optional;DefaultParameterValue(0:int)>]sizeKB: int) = { IsOk = true; Payload = payload; SizeKB = sizeKB }
    static member Fail(error: string) = { IsOk = false; Payload = error; SizeKB = 0 }

type ReportFormat = 
    | Txt = 0
    | Html = 1
    | Csv = 2

type NBomberRunnerContext = {
    Scenarios: Scenario[]
    NBomberConfig: NBomberConfig option  
    ReportFileName: string option
    ReportFormats: ReportFormat[]
}