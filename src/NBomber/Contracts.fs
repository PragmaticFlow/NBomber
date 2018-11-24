﻿namespace NBomber.Contracts

open System
open System.Runtime.InteropServices
open NBomber.Configuration

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

type Scenario = {
    ScenarioName: string
    TestInit: IStep option
    Steps: IStep[]
    Assertions: IAssertion[]
    ConcurrentCopies: int
    Duration: TimeSpan
}

type AssertStats = {       
    OkCount: int
    FailCount: int
}
    
type Response with
    static member Ok([<Optional;DefaultParameterValue(null:obj)>]payload: obj) = { IsOk = true; Payload = payload }
    static member Fail(error: string) = { IsOk = false; Payload = error }

type NBomberRunnerContext = {
    Scenarios: Scenario[]
    NBomberConfig: NBomberConfig option
}