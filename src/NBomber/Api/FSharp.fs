namespace NBomber.FSharp

open System
open System.IO
open System.Runtime
open System.Runtime.CompilerServices
open System.Threading.Tasks

open Serilog
open CommandLine
open FsToolkit.ErrorHandling
open Microsoft.Extensions.Configuration

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Contracts.Internal
open NBomber.Configuration
open NBomber.Extensions.Internal
open NBomber.Errors
open NBomber.Domain.ScenarioContext
open NBomber.DomainServices

type Response =

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member ok () = ResponseInternal.okEmpty

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member fail () = ResponseInternal.failEmpty<obj>

    static member inline ok<'T>(
        ?payload: 'T,
        ?statusCode: string,
        ?sizeBytes: int,
        ?message: string,
        ?latencyMs: float) =

        { StatusCode = statusCode |> Option.defaultValue ""
          IsError = false
          SizeBytes = sizeBytes |> Option.defaultValue 0
          LatencyMs = latencyMs |> Option.defaultValue 0
          Message = message |> Option.defaultValue ""
          Payload = payload }

    static member inline fail<'T>(
        ?statusCode: string,
        ?message: string,
        ?payload: 'T,
        ?sizeBytes: int,
        ?latencyMs: float) =

        { StatusCode = statusCode |> Option.defaultValue ""
          IsError = true
          SizeBytes = sizeBytes |> Option.defaultValue 0
          LatencyMs = latencyMs |> Option.defaultValue 0
          Message = message |> Option.defaultValue ""
          Payload = payload }

/// Step represents a single user action like login, logout, etc.
[<RequireQualifiedAccess>]
type Step =

    /// Runs a step.
    /// Step represents a single user action like login, logout, etc.
    static member run (name: string, context: IScenarioContext, run: unit -> Task<Response<'T>>) = backgroundTask {

        //todo: add validation on name <> Constants.ScenarioGlobalInfo

        let ctx = context :?> ScenarioContext

        let! response = Domain.Step.measure name ctx run

        if response.IsError && ctx.ResetIterationOnFail then
            return raise ResetScenarioIteration
        else
            return response
    }

/// Scenario is basically a workflow that virtual users will follow. It helps you organize steps into user actions.
/// Scenarios are always running in parallel (it's opposite to steps that run sequentially).
/// You should think about Scenario as a system thread.
[<RequireQualifiedAccess>]
module Scenario =

    /// Creates scenario.
    /// Scenario is basically a workflow that virtual users will follow. It helps you organize steps into user actions.
    /// You should think about Scenario as a system thread.
    let create (name: string, run: IScenarioContext -> Task<#IResponse>) : ScenarioProps =

        let typedRun =
            fun ctx ->
                run ctx
                |> Task.map(fun x -> x :> IResponse)

        { ScenarioName = name
          Init = None
          Clean = None
          Run = Some typedRun
          WarmUpDuration = Some Constants.DefaultWarmUpDuration
          LoadSimulations = [LoadSimulation.KeepConstant(copies = Constants.DefaultCopiesCount, during = Constants.DefaultSimulationDuration)]
          ResetIterationOnFail = true
          MaxFailCount = Constants.ScenarioMaxFailCount }

    /// Creates empty scenario.
    /// An empty scenario is useful when you want to create the scenario to do only initialization or cleaning and execute it separately.
    /// The need for this can be when you have a few scenarios with the same init logic, and you want to run this init logic only once.
    let empty (name: string) : ScenarioProps =
        { ScenarioName = name
          Init = None
          Clean = None
          Run = None
          WarmUpDuration = None
          LoadSimulations = [LoadSimulation.KeepConstant(copies = Constants.DefaultCopiesCount, during = Constants.DefaultSimulationDuration)]
          ResetIterationOnFail = true
          MaxFailCount = Constants.ScenarioMaxFailCount }

    /// Initializes scenario.
    /// You can use it to for example to prepare your target system or to parse and apply configuration.
    let withInit (initFunc: IScenarioInitContext -> Task<unit>) (scenario: ScenarioProps) =
        { scenario with Init = Some(fun token -> initFunc(token) :> Task) }

    /// Cleans scenario's resources.
    let withClean (cleanFunc: IScenarioInitContext -> Task<unit>) (scenario: ScenarioProps) =
        { scenario with Clean = Some(fun token -> cleanFunc(token) :> Task) }

    /// Sets warm-up duration
    /// Warm-up will just simply start a scenario with a specified duration.
    let withWarmUpDuration (duration: TimeSpan) (scenario: ScenarioProps) =
        { scenario with WarmUpDuration = Some duration }

    let withoutWarmUp (scenario: ScenarioProps) =
        { scenario with WarmUpDuration = None }

    /// Sets load simulations.
    /// Default value is: KeepConstant(copies = 1, during = minutes 1)
    /// NBomber is always running simulations in sequential order that you defined them.
    /// All defined simulations are represent the whole Scenario duration.
    let withLoadSimulations (loadSimulations: LoadSimulation list) (scenario: ScenarioProps) =
        { scenario with LoadSimulations = loadSimulations }

    /// With this configuration, you can enable or disable Scenario iteration reset.
    /// By default, on fail Step response, NBomber will reset the current Scenario iteration.
    /// Sometimes, you would like to handle failed steps differently: retry, ignore or use a fallback.
    /// For such cases, you can disable scenario iteration reset.
    /// The default value is true.
    let withResetIterationOnFail (shouldReset: bool) (scenario: ScenarioProps) =
        { scenario with ResetIterationOnFail = shouldReset }

    /// Sets and overrides the default max fail count.
    /// When a scenario reaches max fail count, NBomber will stop the whole load test.
    /// By default MaxFailCount = 5_000
    let withMaxFailCount (failCount: int) (scenario: ScenarioProps) =
        { scenario with MaxFailCount = failCount }

/// NBomberRunner is responsible for registering and running scenarios.
/// Also it provides configuration points related to infrastructure, reporting, loading plugins.
[<RequireQualifiedAccess>]
module NBomberRunner =

    /// Registers scenario in NBomber environment.
    let registerScenario (scenario: ScenarioProps) =
        { NBomberContext.empty with RegisteredScenarios = [scenario] }

    /// Registers scenarios in NBomber environment.
    /// Scenarios will be run in parallel.
    let registerScenarios (scenarios: ScenarioProps list) =
        { NBomberContext.empty with RegisteredScenarios = scenarios }

    /// Sets target scenarios among all registered that will execute
    let withTargetScenarios (scenarioNames: string list) (context: NBomberContext) =
        context |> NBomberContext.setTargetScenarios scenarioNames

    /// Sets test suite name
    /// Default value is: nbomber_default_test_suite_name.
    let withTestSuite (testSuite: string) (context: NBomberContext) =
        { context with TestSuite = testSuite }

    /// Sets test name
    /// Default value is: nbomber_default_test_name.
    let withTestName (testName: string) (context: NBomberContext) =
        { context with TestName = testName }

    /// Sets output report name.
    /// Default name: nbomber_report.
    let withReportFileName (reportFileName: string) (context: NBomberContext) =
        let report = { context.Reporting with FileName = Some reportFileName }
        { context with Reporting = report }

    /// Sets output report folder path.
    /// Default folder path: "./reports".
    let withReportFolder (reportFolderPath: string) (context: NBomberContext) =
        let report = { context.Reporting with FolderName = Some reportFolderPath }
        { context with Reporting = report }

    let withReportFormats (reportFormats: ReportFormat list) (context: NBomberContext) =
        let report = { context.Reporting with Formats = reportFormats }
        { context with Reporting  = report }

    /// Sets to run without reports
    let withoutReports (context: NBomberContext) =
        let report = { context.Reporting with Formats = [] }
        { context with Reporting = report }

    /// Sets real-time reporting interval.
    /// Default value: 10 seconds, min value: 5 sec
    let withReportingInterval (interval: TimeSpan) (context: NBomberContext) =
        let report = { context.Reporting with ReportingInterval = interval }
        { context with Reporting = report }

    /// Sets reporting sinks.
    /// Reporting sink is used to save real-time metrics to correspond database
    let withReportingSinks (reportingSinks: IReportingSink list) (context: NBomberContext) =
        let report = { context.Reporting with Sinks = reportingSinks }
        { context with Reporting = report }

    /// Sets worker plugins.
    /// Worker plugin is a plugin that starts at the test start and works as a background worker.
    let withWorkerPlugins (plugins: IWorkerPlugin list) (context: NBomberContext) =
        { context with WorkerPlugins = plugins }

    /// Loads configuration by full file path or by HTTP URL.
    /// The following formats are supported:
    /// - json (.json)
    let loadConfig (path: string) (context: NBomberContext) =

        let config =
            if Uri.IsWellFormedUriString(path, UriKind.Absolute) then
                use client = new System.Net.Http.HttpClient()
                let configJson = client.GetStringAsync(path).Result
                JsonExt.deserialize<NBomberConfig> configJson
            else
                match Path.GetExtension path with
                | ".json" -> path |> File.ReadAllText |> JsonExt.deserialize<NBomberConfig>
                | _       -> failwith "Unsupported config format"

        if config.GlobalSettings.IsSome || config.TargetScenarios.IsSome || config.TestName.IsSome || config.TestSuite.IsSome then
            { context with NBomberConfig = Some config }
        else
            failwith "NBomberConfig file is empty or doesn't follow the config format. Please read the documentation about NBomberConfig (JSON) format."

    /// Loads infrastructure configuration by full file path or by HTTP URL.
    /// The following formats are supported:
    /// - json (.json)
    let loadInfraConfig (path: string) (context: NBomberContext) =

        let config =
            if Uri.IsWellFormedUriString(path, UriKind.Absolute) then
                use client = new System.Net.Http.HttpClient()
                let configJson = client.GetStreamAsync(path).Result
                ConfigurationBuilder().AddJsonStream(configJson).Build() :> IConfiguration
            else
                match Path.GetExtension path with
                | ".json" -> ConfigurationBuilder().AddJsonFile(path).Build() :> IConfiguration
                | _       -> failwith "Unsupported config format"

        { context with InfraConfig = Some config }

    /// Sets logger configuration.
    /// Make sure that you always return a new instance of LoggerConfiguration.
    /// You can also configure logger via configuration file.
    /// For this use NBomberRunner.loadInfraConfig
    let withLoggerConfig (createLoggerConfig: unit -> LoggerConfiguration) (context: NBomberContext) =
        try
            // this is limitation of Serilog
            // to invoke CreateLogger() twice on the same instance of LoggerConfiguration
            // it's why we can't just accept LoggerConfiguration
            createLoggerConfig().CreateLogger() |> ignore
            createLoggerConfig().CreateLogger() |> ignore
        with
        | :? InvalidOperationException ->
            failwith "createLoggerConfig should always return a new instance of LoggerConfiguration"

        { context with CreateLoggerConfig = Some createLoggerConfig }

    /// Enables or disables hints analyzer.
    /// Hints analyzer - analyze node stats to provide some hints in case of finding wrong usage or some other issue.
    /// The default value is false.
    let enableHintsAnalyzer (enable: bool) (context: NBomberContext) =
        { context with EnableHintsAnalyzer = enable }

    let internal executeCliArgs (args) (context: NBomberContext) =

        let loadConfigFn (loadConfig) (configPath) (context) =
            if String.IsNullOrWhiteSpace configPath then context
            else loadConfig configPath context

        let setTargetScenarios (targetScenarios: string seq) (context: NBomberContext) =
            if Seq.isEmpty targetScenarios then context
            else NBomberContext.setTargetScenarios (List.ofSeq targetScenarios) context

        match CommandLine.Parser.Default.ParseArguments<CommandLineArgs>(args) with
        | :? Parsed<CommandLineArgs> as parsed ->
            let cliArgs = parsed.Value
            let loadCnf = loadConfigFn loadConfig cliArgs.Config
            let loadInfra = loadConfigFn loadInfraConfig cliArgs.InfraConfig

            let run =
                loadCnf
                >> loadInfra
                >> setTargetScenarios cliArgs.TargetScenarios

            run context

        | _ -> context

    let internal runWithResult (args) (context: NBomberContext) =
        GCSettings.LatencyMode <- GCLatencyMode.SustainedLowLatency

        if Seq.isEmpty args then
            NBomberRunner.run context
        else
            context |> executeCliArgs args |> NBomberRunner.run

    let run (context: NBomberContext) =
        context
        |> runWithResult List.empty
        |> Result.map(fun x -> x.FinalStats)
        |> Result.mapError AppError.toString

    /// Runs scenarios with arguments.
    /// The following CLI commands are supported:
    /// -c or --config: loads configuration,
    /// -i or --infra: loads infrastructure configuration.
    /// Examples of possible args:
    /// -c config.json -i infra_config.json
    /// --config=config.json --infra=infra_config.json
    let runWithArgs (args) (context: NBomberContext) =
        context
        |> runWithResult args
        |> Result.map(fun x -> x.FinalStats)
        |> Result.mapError AppError.toString
