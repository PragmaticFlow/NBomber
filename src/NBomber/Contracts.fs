namespace NBomber.Contracts

open System

open Microsoft.Extensions.Configuration
open Serilog

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
    RegisteredScenarios: Scenario list
    NBomberConfig: NBomberConfig option
    InfraConfig: IConfiguration option
    CreateLoggerConfig: (unit -> LoggerConfiguration) option
    Reporting: ReportingContext
    WorkerPlugins: IWorkerPlugin list
    UseHintsAnalyzer: bool
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
        UseHintsAnalyzer = true
    }

namespace NBomber.Contracts.Internal

open CommandLine
open NBomber.Contracts

type CommandLineArgs = {
    [<Option('c', "config", HelpText = "NBomber configuration")>] Config: string
    [<Option('i', "infra", HelpText = "NBomber infra configuration")>] InfraConfig: string
}

type StepResponse = {
    StepIndex: int
    ClientResponse: Response
    EndTimeMs: float
    LatencyMs: float
}
