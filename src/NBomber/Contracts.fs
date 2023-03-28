namespace NBomber.Contracts

open System
open Serilog
open Serilog.Events
open Microsoft.Extensions.Configuration
open NBomber
open NBomber.Configuration
open NBomber.Contracts.Stats

type ReportingContext = {
    FolderName: string option
    FileName: string option
    Sinks: IReportingSink list
    Formats: ReportFormat list
    ReportingInterval: TimeSpan
}

type NBomberContext = {
    TestSuite: string
    TestName: string
    RegisteredScenarios: ScenarioProps list
    NBomberConfig: NBomberConfig option
    InfraConfig: IConfiguration option
    CreateLoggerConfig: (unit -> LoggerConfiguration) option
    Reporting: ReportingContext
    WorkerPlugins: IWorkerPlugin list
    EnableHintsAnalyzer: bool
    TargetScenarios: string list option
    MinimumLogLevel: LogEventLevel option
} with

    [<CompiledName("Empty")>]
    static member empty = {
        TestSuite = Constants.DefaultTestSuite
        TestName = Constants.DefaultTestName
        RegisteredScenarios = List.empty
        NBomberConfig = None
        InfraConfig = None
        CreateLoggerConfig = None
        Reporting = {
            FolderName = None
            FileName = None
            Formats = Constants.AllReportFormats
            Sinks = List.empty
            ReportingInterval = Constants.DefaultReportingInterval
        }
        WorkerPlugins = List.empty
        EnableHintsAnalyzer = false
        TargetScenarios = None
        MinimumLogLevel = None
    }

namespace NBomber.Contracts.Internal

open System
open CommandLine
open NBomber
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Contracts.Stats

type CommandLineArgs = {
    [<Option('c', "config", HelpText = "NBomber configuration")>] Config: string
    [<Option('i', "infra", HelpText = "NBomber infra configuration")>] InfraConfig: string
    [<Option('t', "target", HelpText = "Target Scenarios")>] TargetScenarios: string seq
}

type Measurement = {
    Name: string
    ClientResponse: IResponse
    CurrentTimeBucket: TimeSpan
    Latency: TimeSpan
}

type SessionArgs = {
    TestInfo: TestInfo
    NBomberConfig: NBomberConfig
    ScenarioPartitions: Map<string,ScenarioPartition> // ScenarioName * ScenarioPartition
} with

    static member empty = {
        TestInfo = { SessionId = ""; TestSuite = ""; TestName = ""; ClusterId = "" }
        NBomberConfig = Unchecked.defaultof<_>
        ScenarioPartitions = Map.empty
    }

    member this.GetReportingInterval() = this.NBomberConfig.GlobalSettings.Value.ReportingInterval.Value
    member this.GetReportFolder() = this.NBomberConfig.GlobalSettings.Value.ReportFolder.Value
    member this.GetTargetScenarios() = this.NBomberConfig.TargetScenarios.Value

    member this.SetTargetScenarios(targetScenarios) =
        let nbConfig = { this.NBomberConfig with TargetScenarios = Some targetScenarios }
        { this with NBomberConfig = nbConfig }

    member this.GetScenariosSettings() = this.NBomberConfig.GlobalSettings.Value.ScenariosSettings.Value
    member this.GetUseHintsAnalyzer() = this.NBomberConfig.GlobalSettings.Value.EnableHintsAnalyzer.Value

module internal ResponseInternal =

    let okEmpty : Response<obj> =
        { StatusCode = ""; IsError = false; SizeBytes = 0; LatencyMs = 0; Message = ""; Payload = None }

    let failEmpty<'T> : Response<'T> =
        { StatusCode = ""; IsError = true; SizeBytes = 0; LatencyMs = 0; Message = ""; Payload = None }

    let failUnhandled<'T> (ex: Exception) : Response<'T> =
        { StatusCode = Constants.UnhandledExceptionCode; IsError = true; SizeBytes = 0; LatencyMs = 0; Message = ex.Message; Payload = None }

    let failTimeout<'T> : Response<'T> =
        { StatusCode = Constants.TimeoutStatusCode; IsError = true; SizeBytes = 0; LatencyMs = 0; Message = Constants.OperationTimeoutMessage; Payload = None }
