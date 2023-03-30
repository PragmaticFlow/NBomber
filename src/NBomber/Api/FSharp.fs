namespace NBomber.FSharp

open System
open System.IO
open System.Runtime
open System.Runtime.CompilerServices
open System.Threading.Tasks

open Serilog
open Serilog.Events
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

/// Step represents a single user action like login, logout, etc. Step helps you granulate your Scenario execution on parts and measure them separately.
/// In case you don't need to split your Scenario on parts you can use just Scenario without any Step.
type Step =

    /// <summary>
    /// Runs a step.
    /// </summary>
    /// <param name="name">The name of the step. It can be any name except reserved name "global information".</param>
    /// <param name="context">ScenarioContext represents the execution context of the currently running Scenario. It provides functionality to log particular events, get information about the test, thread id, scenario copy/instance number, etc. Also, it provides the option to stop all or particular scenarios manually.</param>
    /// <param name="run">It's a function that represents user action that will be invoked and measured by NBomber.</param>
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member run (name: string, context: IScenarioContext, run: unit -> Task<Response<'T>>) = backgroundTask {

        if name = Constants.ScenarioGlobalInfo then
            context.StopCurrentTest $"The '{Constants.ScenarioGlobalInfo}' is a reserved name that can't be used for the step name. Please use any different name."

        let ctx = context :?> ScenarioContext

        let! response = Domain.Step.measure name ctx run

        if response.IsError && ctx.RestartIterationOnFail then
            return raise RestartScenarioIteration
        else
            return response
    }

/// Scenario play the most crucial role in building load tests with NBomber.
/// Scenario represents typical user behavior.
/// In other words - it’s a workflow that virtual users will follow.
/// Technically speaking, each Scenario instance works as a dedicated .NET Task.
module Scenario =

    /// <summary>
    /// Creates a scenario.
    /// </summary>
    /// <param name="name">The name of the scenario.</param>
    /// <param name="run">It's a function that represents user flow that will be invoked and measured by NBomber.</param>
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
          RestartIterationOnFail = true
          MaxFailCount = Constants.ScenarioMaxFailCount }

    /// <summary>
    /// Creates empty scenario.
    /// An empty scenario is useful when you want to create the scenario to do only initialization or cleaning and execute it separately.
    /// The need for this can be when you have a few scenarios with the same init logic, and you want to run this init logic only once.
    /// </summary>
    /// <param name="name">The name of the scenario.</param>
    let empty (name: string) : ScenarioProps =
        { ScenarioName = name
          Init = None
          Clean = None
          Run = None
          WarmUpDuration = None
          LoadSimulations = [LoadSimulation.KeepConstant(copies = Constants.DefaultCopiesCount, during = Constants.DefaultSimulationDuration)]
          RestartIterationOnFail = true
          MaxFailCount = Constants.ScenarioMaxFailCount }

    /// <summary>
    /// Initializes scenario and all its dependencies.
    /// You can use it to prepare your target system, populate the database, or read and apply the JSON configuration for your scenario.
    /// Scenario init will be invoked before warm-up and bombing phases.
    /// If Scenario init throws an exception, the NBomber load test will stop the execution.
    /// </summary>
    /// <param name="initFunc">Represent lambda function that will be invoked to start Scenario initialization.
    /// If this lambda function throws an exception, the NBomber load test will stop the execution.</param>
    /// <param name="scenario">Represent configuration data that is needed to build a scenario.</param>
    let withInit (initFunc: IScenarioInitContext -> Task<unit>) (scenario: ScenarioProps) =
        { scenario with Init = Some(fun token -> initFunc(token) :> Task) }

    /// <summary>
    /// Cleans scenario's resources and all its dependencies.
    /// This function should be used to clean the scenario's resources after the test finishes.
    /// Scenario clean will be invoked after warm-up and bombing phases.
    /// If Scenario clean throws an exception, the NBomber logs it and continues execution.
    /// </summary>
    /// <param name="cleanFunc">Represent lambda function that will be invoked to start Scenario cleaning.
    /// If this lambda function throws an exception, the NBomber logs it and continues execution.</param>
    /// <param name="scenario">Represent configuration data that is needed to build a scenario.</param>
    let withClean (cleanFunc: IScenarioInitContext -> Task<unit>) (scenario: ScenarioProps) =
        { scenario with Clean = Some(fun token -> cleanFunc(token) :> Task) }

    /// <summary>
    /// This function sets duration of warm-up phase.
    /// </summary>
    /// <param name="duration">By default warm-up duration is 30 seconds.</param>
    /// <param name="scenario">Represent configuration data that is needed to build a scenario.</param>
    let withWarmUpDuration (duration: TimeSpan) (scenario: ScenarioProps) =
        { scenario with WarmUpDuration = Some duration }

    /// <summary>
    /// This function disables warm-up.
    /// </summary>
    /// <param name="scenario">Represent configuration data that is needed to build a scenario.</param>
    let withoutWarmUp (scenario: ScenarioProps) =
        { scenario with WarmUpDuration = None }

    /// <summary>
    /// This function allows configuring the load simulations for the current Scenario.
    /// Load simulation allows configuring parallelism and workload profiles.
    /// </summary>
    /// <param name="loadSimulations">Default value is: [KeepConstant(copies = 1, during = minutes 1)]</param>
    /// <param name="scenario">Represent configuration data that is needed to build a scenario.</param>
    /// <example>
    /// <code>
    /// scenario
    /// |> withLoadSimulations [
    ///     RampingConstant(copies = 50, during = seconds 30) // ramp-up from 0 to 50 copies
    ///     KeepConstant(copies = 50, during = seconds 30)
    /// ]
    /// </code>
    /// </example>
    let withLoadSimulations (loadSimulations: LoadSimulation list) (scenario: ScenarioProps) =
        { scenario with LoadSimulations = loadSimulations }

    /// <summary>
    /// This function allows enabling or disabling the auto restart of Scenario iteration in case of Step failure.
    /// Sometimes, you would like to handle failed steps differently: retry, ignore or use a fallback.
    /// For such cases, you can disable Scenario iteration auto restart.
    /// By default, when a Step returns a failed Response or unhandled exception was thrown,
    /// NBomber will automatically mark the whole iteration as failed and restart it.
    /// If you want to disable auto restart of Scenario iteration you should set 'shouldRestart = false'.
    /// </summary>
    /// <param name="shouldRestart">The default value is true.
    /// If you want to disable auto restart of Scenario iteration you should set 'shouldRestart = false'.</param>
    /// <param name="scenario">Represent configuration data that is needed to build a scenario.</param>
    let withRestartIterationOnFail (shouldRestart: bool) (scenario: ScenarioProps) =
        { scenario with RestartIterationOnFail = shouldRestart }

    /// <summary>
    /// This function overrides the default value of MaxFailCount for Scenario.
    /// MaxFailCount is incremented on every failure or failed Response.
    /// When a scenario reaches MaxFailCount, NBomber will stop the whole load test.
    /// In the case of cluster mode, MaxFailCount is tracked per each NBomber instance exclusively.
    /// It doesn't aggregate across the cluster. So if on any NBomber node MaxFailCount is reached, NBomber will stop the whole load test.
    /// </summary>
    /// <param name="maxFailCount">The default value is 5_000.</param>
    /// <param name="scenario">Represent configuration data that is needed to build a scenario.</param>
    let withMaxFailCount (maxFailCount: int) (scenario: ScenarioProps) =
        { scenario with MaxFailCount = maxFailCount }

/// NBomberRunner is responsible for registering and running scenarios.
/// Also it provides configuration points related to infrastructure, reporting, loading plugins.
module NBomberRunner =

    /// Registers scenario in NBomber environment.
    let registerScenario (scenario: ScenarioProps) =
        { NBomberContext.empty with RegisteredScenarios = [scenario] }

    /// Registers scenarios in NBomber environment.
    let registerScenarios (scenarios: ScenarioProps list) =
        { NBomberContext.empty with RegisteredScenarios = scenarios }

    /// <summary>
    /// Sets target scenarios among all registered that will execute during the session.
    /// </summary>
    /// <param name="scenarioNames">Names of scenarios that should be started during the session.</param>
    /// <param name="context">NBomberContext</param>
    /// <example>
    /// <code>
    /// NBomberRunner.registerScenarios [scenario_1; scenario_2; scenario_3]
    /// |> NBomberRunner.withTargetScenarios ["scenario_1"] // only scenario_1 will be executed
    /// </code>
    /// </example>
    let withTargetScenarios (scenarioNames: string list) (context: NBomberContext) =
        context |> NBomberContext.setTargetScenarios scenarioNames

    /// <summary>
    /// Sets test suite name.
    /// </summary>
    /// <param name="testSuite">Default value is: "nbomber_default_test_suite_name".</param>
    /// <param name="context">NBomberContext</param>
    let withTestSuite (testSuite: string) (context: NBomberContext) =
        { context with TestSuite = testSuite }

    /// <summary>
    /// Sets test name.
    /// </summary>
    /// <param name="testName">Default value is: "nbomber_default_test_name".</param>
    /// <param name="context">NBomberContext</param>
    let withTestName (testName: string) (context: NBomberContext) =
        { context with TestName = testName }

    /// <summary>
    /// Sets output report file name.
    /// </summary>
    /// <param name="reportFileName">Default name: "nbomber_report-{CurrentTime}"</param>
    /// <param name="context">NBomberContext</param>
    let withReportFileName (reportFileName: string) (context: NBomberContext) =
        let report = { context.Reporting with FileName = Some reportFileName }
        { context with Reporting = report }

    /// <summary>
    /// Sets output report folder path.
    /// </summary>
    /// <param name="reportFolderPath">Default folder path: "./reports".</param>
    /// <param name="context">NBomberContext</param>
    let withReportFolder (reportFolderPath: string) (context: NBomberContext) =
        let report = { context.Reporting with FolderName = Some reportFolderPath }
        { context with Reporting = report }

    /// <summary>
    /// Sets output report formats.
    /// </summary>
    /// <param name="reportFormats">The default value is: [Txt; Html; Csv; Md]</param>
    /// <param name="context">NBomberContext</param>
    let withReportFormats (reportFormats: ReportFormat list) (context: NBomberContext) =
        let report = { context.Reporting with Formats = reportFormats }
        { context with Reporting  = report }

    /// Sets to run without reports
    let withoutReports (context: NBomberContext) =
        let report = { context.Reporting with Formats = [] }
        { context with Reporting = report }

    /// <summary>
    /// Sets real-time reporting interval.
    /// </summary>
    /// <param name="interval">Default value: 10 sec, min value: 5 sec</param>
    /// <param name="context">NBomberContext</param>
    let withReportingInterval (interval: TimeSpan) (context: NBomberContext) =
        let report = { context.Reporting with ReportingInterval = interval }
        { context with Reporting = report }

    /// Sets reporting sinks.
    /// ReportingSink provides functionality for saving real-time and final statistics.
    let withReportingSinks (reportingSinks: IReportingSink list) (context: NBomberContext) =
        let report = { context.Reporting with Sinks = reportingSinks }
        { context with Reporting = report }

    /// Sets worker plugins.
    /// Worker plugin is a plugin that starts at the test start and works as a background worker.
    let withWorkerPlugins (plugins: IWorkerPlugin list) (context: NBomberContext) =
        { context with WorkerPlugins = plugins }

    /// <summary>
    /// Loads configuration by file path or by HTTP URL.
    /// </summary>
    /// <param name="path">File path or HTTP URL to JSON config.</param>
    /// <param name="context">NBomberContext</param>
    /// <example>
    /// <code>
    /// // load by file path
    /// NBomberRunner.loadConfig "./my-test/config.json"
    /// // load by URL
    /// NBomberRunner.loadConfig "https://my-test/config.json"
    /// </code>
    /// </example>
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

    /// <summary>
    /// Loads infrastructure configuration by file path or by HTTP URL.
    /// </summary>
    /// <param name="path">File path or HTTP URL to JSON config.</param>
    /// <param name="context"></param>
    /// <example>
    /// <code>
    /// // load by file path
    /// NBomberRunner.loadInfraConfig "./my-test/infra-config.json"
    /// // load by URL
    /// NBomberRunner.loadInfraConfig "https://my-test/infra-config.json"
    /// </code>
    /// </example>
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

    /// <summary>
    /// Sets minimum log level.
    /// </summary>
    /// <param name="level">The default value is Debug</param>
    /// <param name="context">NBomberContext</param>
    let withMinimumLogLevel (level: LogEventLevel) (context: NBomberContext) =
        { context with MinimumLogLevel = Some level }

    /// <summary>
    /// Sets logger configuration.
    /// Make sure that you always return a new instance of LoggerConfiguration.
    /// You can also configure the logger via JSON infrastructure config file.
    /// For this use NBomberRunner.loadInfraConfig
    /// </summary>
    /// <param name="createLoggerConfig">creates a new instance of LoggerConfiguration</param>
    /// <param name="context">NBomberContext</param>
    /// <example>
    /// <code>
    /// NBomberRunner.withLoggerConfig (fun () -> LoggerConfiguration())
    /// </code>
    /// </example>
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

    /// <summary>
    /// This function enables or disables hints analyzer.
    /// Hints analyzer - analyzes statistics at the end of the test to provide hints in case of finding the wrong usage of NBomber or some environmental issues.
    /// </summary>
    /// <param name="enable">The default value is false.</param>
    /// <param name="context">NBomberContext</param>
    let enableHintsAnalyzer (enable: bool) (context: NBomberContext) =
        { context with EnableHintsAnalyzer = enable }

    /// <summary>
    /// This function enables or disables to forcibly halt all tests at the end of the simulation even though NBomber is overloaded.
    /// Use when you need to stop your test at a precise time even if your NBomber is overloaded and can have a short lag.
    /// </summary>
    /// <param name="enable">The default value is false.</param>
    /// <param name="context">NBomberContext</param>
    let enableStopTestForcibly (enable: bool) (context: NBomberContext) =
        { context with EnableStopTestForcibly = enable }

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

    let internal runWithResult (args: string seq) (context: NBomberContext) =
        GCSettings.LatencyMode <- GCLatencyMode.SustainedLowLatency

        let disposeLogger = true

        if Seq.isEmpty args then
            NBomberRunner.run disposeLogger context

        elif args |> String.contains "disposeLogger=false" then
            context |> executeCliArgs args |> NBomberRunner.run(false)

        else
            context |> executeCliArgs args |> NBomberRunner.run(disposeLogger)

    /// Runs scenarios.
    let run (context: NBomberContext) =
        context
        |> runWithResult List.empty
        |> Result.map(fun x -> x.FinalStats)
        |> Result.mapError AppError.toString

    /// <summary>
    /// Runs scenarios with CLI arguments.
    /// </summary>
    /// <param name="args">CLI args</param>
    /// <param name="context">NBomberContext</param>
    /// <example>
    /// <code>
    /// Example:
    /// --config=config.json --infra=infra_config.json --target=scenario_1
    /// </code>
    /// </example>
    let runWithArgs (args) (context: NBomberContext) =
        context
        |> runWithResult args
        |> Result.map(fun x -> x.FinalStats)
        |> Result.mapError AppError.toString
