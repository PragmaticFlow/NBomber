namespace NBomber.Contracts

open System
open System.Runtime.InteropServices

type CorrelationId = string
type Latency = int64

[<Struct>]
type Request = {
    CorrelationId: CorrelationId
    Payload: obj
}

[<Struct>]
type Response = {
    IsOk: bool
    Payload: obj
}

type IStep = interface end   

type IAssertion = interface end  

type IStepListenerChannel =
    abstract Notify: correlationId:CorrelationId * response:Response -> unit

type TestFlow = {
    FlowName: string
    Steps: IStep seq
    ConcurrentCopies: int
}

type Scenario = {
    ScenarioName: string
    TestInit: IStep option
    TestFlows: TestFlow[]   
    Assertions: IAssertion[]
    Duration: TimeSpan    
}

type AssertStats = {       
    OkCount: int
    FailCount: int
}
    
type Response with
    static member Ok([<Optional;DefaultParameterValue(null:obj)>]payload: obj) = { IsOk = true; Payload = payload }
    static member Fail(error: string) = { IsOk = false; Payload = error }