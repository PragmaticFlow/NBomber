namespace NBomber.Contracts

open System

open Microsoft.Extensions.Configuration
open Serilog

open NBomber.Configuration
open NBomber.Contracts.Stats

type ReportingContext = {
    FolderName: string option
    FileName: string option
    Sinks: IReportingSink list
    Formats: ReportFormat list
    SendStatsInterval: TimeSpan
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
}

namespace NBomber.Contracts.Internal

open CommandLine
open NBomber.Contracts

type CommandLineArgs = {
    [<Option('c', "config", HelpText = "NBomber configuration")>] Config: string
    [<Option('i', "infra", HelpText = "NBomber infra configuration")>] InfraConfig: string
}

type StepResponse = {
    ClientResponse: Response
    EndTimeMs: float
    LatencyMs: float
}
