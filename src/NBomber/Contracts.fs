﻿namespace NBomber.Contracts

open System
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open Serilog
open Microsoft.Extensions.Configuration

open NBomber.Configuration
open NBomber.Extensions

type Response = {
    Payload: obj
    SizeBytes: int
    Exception: exn option
}

type NodeType =
    | SingleNode
    | Coordinator
    | Agent
    | Cluster

type NodeOperationType =
    | None = 0
    | Init = 1
    | WarmUp = 2
    | Bombing = 3
    | Stop = 4
    | Complete = 5

type NodeInfo = {
    MachineName: string
    Sender: NodeType
    CurrentOperation: NodeOperationType
}

type TestInfo = {
    SessionId: string
    TestSuite: string
    TestName: string
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
    NodeInfo: NodeInfo
}

type IConnectionPool<'TConnection> = interface end

/// Data provider
type IFeed<'T> =
    /// Feed name, which is also the key of step data dictionary
    abstract member Name: string with get
    /// Gets data for the next step
    abstract member GetNext: unit -> Dict<string,'T>

type StepContext<'TConnection> = {
    CorrelationId: string
    CancellationToken: CancellationToken
    Connection: 'TConnection
    Data: Dict<string,obj>
    Logger: ILogger
}

type ScenarioContext = {
    NodeType: NodeType
    CustomSettings: string
    CancellationToken: CancellationToken
    Logger: ILogger
}

type IStep =
    abstract StepName: string

type Scenario = {
    ScenarioName: string
    TestInit: (ScenarioContext -> Task) option
    TestClean: (ScenarioContext -> Task) option
    Feed: IFeed<obj>
    Steps: IStep[]
    ConcurrentCopies: int
    WarmUpDuration: TimeSpan
    Duration: TimeSpan
}

type ReportFile = {
    FilePath: string
    ReportFormat: ReportFormat
}

type IReportingSink =
    abstract Init: logger:ILogger * infraConfig:IConfiguration option -> unit
    abstract StartTest: testInfo:TestInfo -> Task
    abstract SaveRealtimeStats: testInfo:TestInfo * stats:Statistics[] -> Task
    abstract SaveFinalStats: testInfo:TestInfo * stats:Statistics[] * reportFiles:ReportFile[] -> Task
    abstract FinishTest: testInfo:TestInfo -> Task

type TestContext = {
    TestSuite: string
    TestName: string
    RegisteredScenarios: Scenario[]
    TestConfig: TestConfig option
    InfraConfig: IConfiguration option
    ReportFileName: string option
    ReportFormats: ReportFormat[]
    ReportingSinks: IReportingSink[]
    SendStatsInterval: TimeSpan
}

type Response with
    static member Ok([<Optional;DefaultParameterValue(null:obj)>]payload: obj,
                     [<Optional;DefaultParameterValue(0:int)>]sizeBytes: int) =
        { Payload = payload
          SizeBytes = sizeBytes
          Exception = None }

    static member Ok(payload: byte[]) =
        { Payload = payload
          SizeBytes = if isNull payload then 0 else payload.Length
          Exception = None }

    static member Fail() =
        { Payload = null
          SizeBytes = 0
          Exception = Some(Exception("unknown client's error")) }

    static member Fail(ex: Exception) =
        { Payload = null
          SizeBytes = 0
          Exception = Some(ex) }

    static member Fail(reason: string) =
        { Payload = null
          SizeBytes = 0
          Exception = Some(Exception(reason)) }
