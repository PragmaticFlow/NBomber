namespace rec NBomber.Contracts

open System
open System.Runtime.InteropServices

[<Struct>]
type Request = {
    CorrelationId: string
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
    abstract Notify: correlationId:string * response:Response -> unit

type TestFlow = {
    FlowName: string
    Steps: IStep seq
    ConcurrentCopies: int
}

type Scenario = {
    ScenarioName: string
    TestInit: IStep option
    TestFlows: TestFlow[]   
    Duration: TimeSpan
    Assertions: IAssertion[]
}

type AssertionStats = {
    StepName: string
    FlowName: string    
    OkCount: int
    FailCount: int
    ExceptionCount: int
    ThrownException: exn option
}
    
type Response with
    static member Ok([<Optional;DefaultParameterValue(null:obj)>]payload: obj) = { IsOk = true; Payload = payload }
    static member Fail(error: string) = { IsOk = false; Payload = error }