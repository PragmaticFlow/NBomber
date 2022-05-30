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
    EnableHintsAnalyzer: bool
    TargetScenarios: string list option
    DefaultStepTimeoutMs: int
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
        EnableHintsAnalyzer = true
        TargetScenarios = None
        DefaultStepTimeoutMs = Constants.DefaultStepTimeoutMs
    }

namespace NBomber.Contracts.Internal

open System
open CommandLine
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Contracts.Stats

type CommandLineArgs = {
    [<Option('c', "config", HelpText = "NBomber configuration")>] Config: string
    [<Option('i', "infra", HelpText = "NBomber infra configuration")>] InfraConfig: string
    [<Option('t', "target", HelpText = "Target Scenarios")>] TargetScenarios: string seq
}

type StepResponse = {
    StepIndex: int
    ClientResponse: Response
    EndTimeMs: float
    LatencyMs: float
}

type ScenarioRawStats = {
    ScenarioName: string
    StepResponses: StepResponse list
    Duration: TimeSpan
}

// we keep ClientFactorySettings settings here instead of take them from ScenariosSettings
// since after init (for case when the same ClientFactory assigned to several Scenarios)
// factoryName = factoryName + scenarioName
// and it's more convenient to prepare it for usage

type SessionArgs = {
    TestInfo: TestInfo
    NBomberConfig: NBomberConfig
    UpdatedClientFactorySettings: ClientFactorySetting list
} with

    static member empty = {
        TestInfo = { SessionId = ""; TestSuite = ""; TestName = "" }
        NBomberConfig = Unchecked.defaultof<_>
        UpdatedClientFactorySettings = List.empty
    }

    member this.GetReportingInterval() = TimeSpan.Parse this.NBomberConfig.GlobalSettings.Value.ReportingInterval.Value
    member this.GetReportFolder() = this.NBomberConfig.GlobalSettings.Value.ReportFolder.Value
    member this.GetTargetScenarios() = this.NBomberConfig.TargetScenarios.Value

    member this.SetTargetScenarios(targetScenarios) =
        let nbConfig = { this.NBomberConfig with TargetScenarios = Some targetScenarios }
        { this with NBomberConfig = nbConfig }

    member this.GetScenariosSettings() = this.NBomberConfig.GlobalSettings.Value.ScenariosSettings.Value
    member this.GetUseHintsAnalyzer() = this.NBomberConfig.GlobalSettings.Value.EnableHintsAnalyzer.Value
    member this.GetDefaultStepTimeout() = this.NBomberConfig.GlobalSettings.Value.DefaultStepTimeoutMs.Value |> TimeSpan.FromMilliseconds
