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
    Assertions: AssertionScope[]
}

type AssertionInfo = {
    StepName: string
    FlowName: string    
    OkCount: int
    FailCount: int
    ExceptionCount: int
    ThrownException: exn option
} with
     static member Create (stepName, flowName, okCount, failCount, exceptionCount, exn) =
         { StepName = stepName; FlowName = flowName; OkCount = okCount; FailCount = failCount;
            ExceptionCount = exceptionCount; ThrownException = exn}

type AssertionFunc = Func<AssertionInfo, bool>

type AssertionScope = 
    | Step     of string * string * AssertionFunc
    | TestFlow of string * AssertionFunc
    | Scenario of AssertionFunc

type Response with
    static member Ok([<Optional;DefaultParameterValue(null:obj)>]payload: obj) = { IsOk = true; Payload = payload }
    static member Fail(error: string) = { IsOk = false; Payload = error }