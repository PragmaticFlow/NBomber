namespace NBomber.Contracts

open System
open System.Runtime.InteropServices
open NBomber.Configuration

type Response = {
    IsOk: bool
    Payload: obj
    SizeBytes: int
}

type IConnectionPool<'TConnection> = interface end

type StepContext<'TConnection> = {
    CorrelationId: string
    Connection: 'TConnection
    mutable Payload: obj
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
    WarmUpDuration: TimeSpan
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
    DataMinKb: float
    DataMeanKb: float
    DataMaxKb: float
    AllDataMB: float
}
    
type Response with
    static member Ok([<Optional;DefaultParameterValue(null:obj)>]payload: obj, [<Optional;DefaultParameterValue(0:int)>]sizeBytes: int) = { IsOk = true; Payload = payload; SizeBytes = sizeBytes }
    static member Ok(payload: byte[]) = { IsOk = true; Payload = payload; SizeBytes = if isNull(payload) then 0 else Array.length(payload)}
    static member Fail() = { IsOk = false; Payload = null; SizeBytes = 0 }

type ReportFormat = 
    | Txt = 0
    | Html = 1
    | Csv = 2
    | Md = 3

type NBomberContext = {
    Scenarios: Scenario[]
    NBomberConfig: NBomberConfig option  
    ReportFileName: string option
    ReportFormats: ReportFormat[]
}
