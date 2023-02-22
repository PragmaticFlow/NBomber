namespace NBomber.CSharp

#nowarn "3211"

open System
open System.Runtime.InteropServices
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Serilog
open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Contracts.Internal

type Response =

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member Ok() = ResponseInternal.okEmpty

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member Fail() = ResponseInternal.failEmpty<obj>

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member Ok(
        [<Optional;DefaultParameterValue("")>] statusCode: string,
        [<Optional;DefaultParameterValue(0)>] sizeBytes: int,
        [<Optional;DefaultParameterValue("")>] message: string,
        [<Optional;DefaultParameterValue(0.0)>] latencyMs: float) : Response<obj> =

        { StatusCode = statusCode
          IsError = false
          SizeBytes = sizeBytes
          LatencyMs = latencyMs
          Message = if isNull message then String.Empty else message
          Payload = None }

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member Ok<'T>(
        [<Optional;DefaultParameterValue("")>] statusCode: string,
        [<Optional;DefaultParameterValue(0)>] sizeBytes: int,
        [<Optional;DefaultParameterValue("")>] message: string,
        [<Optional;DefaultParameterValue(0.0)>] latencyMs: float) : Response<'T> =

        { StatusCode = statusCode
          IsError = false
          SizeBytes = sizeBytes
          LatencyMs = latencyMs
          Message = if isNull message then String.Empty else message
          Payload = None }

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member Ok<'T>(
        payload: 'T,
        [<Optional;DefaultParameterValue("")>] statusCode: string,
        [<Optional;DefaultParameterValue(0)>] sizeBytes: int,
        [<Optional;DefaultParameterValue("")>] message: string,
        [<Optional;DefaultParameterValue(0.0)>] latencyMs: float) : Response<'T> =

        { StatusCode = statusCode
          IsError = false
          SizeBytes = sizeBytes
          LatencyMs = latencyMs
          Message = if isNull message then String.Empty else message
          Payload = Some payload }

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member Fail(
        [<Optional;DefaultParameterValue("")>] statusCode: string,
        [<Optional;DefaultParameterValue("")>] message: string,
        [<Optional;DefaultParameterValue(0)>] sizeBytes: int,
        [<Optional;DefaultParameterValue(0.0)>] latencyMs: float) : Response<obj> =

        { StatusCode = statusCode
          IsError = true
          SizeBytes = sizeBytes
          LatencyMs = latencyMs
          Message = if isNull message then String.Empty else message
          Payload = None }

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member Fail<'T>(
        [<Optional;DefaultParameterValue("")>] statusCode: string,
        [<Optional;DefaultParameterValue("")>] message: string,
        [<Optional;DefaultParameterValue(0)>] sizeBytes: int,
        [<Optional;DefaultParameterValue(0.0)>] latencyMs: float) : Response<'T> =

        { StatusCode = statusCode
          IsError = true
          SizeBytes = sizeBytes
          LatencyMs = latencyMs
          Message = if isNull message then String.Empty else message
          Payload = None }

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member Fail<'T>(
        payload: 'T,
        [<Optional;DefaultParameterValue("")>] statusCode: string,
        [<Optional;DefaultParameterValue("")>] message: string,
        [<Optional;DefaultParameterValue(0)>] sizeBytes: int,
        [<Optional;DefaultParameterValue(0.0)>] latencyMs: float) : Response<'T> =

        { StatusCode = statusCode
          IsError = true
          SizeBytes = sizeBytes
          LatencyMs = latencyMs
          Message = if isNull message then String.Empty else message
          Payload = Some payload }

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
    static member Run(name: string, context: IScenarioContext, run: Func<Task<Response<'T>>>) =
        FSharp.Step.run(name, context, run.Invoke)

/// Scenario play the most crucial role in building load tests with NBomber.
/// Scenario represents typical user behavior.
/// In other words - it’s a workflow that virtual users will follow.
/// Technically speaking, each Scenario instance works as a dedicated .NET Task.
[<Extension>]
type Scenario =

    /// <summary>
    /// Creates a scenario.
    /// </summary>
    /// <param name="run">It's a function that represents user flow that will be invoked and measured by NBomber.</param>
    /// <param name="name">The name of the scenario.</param>
    static member Create(name: string, run: Func<IScenarioContext,Task<IResponse>>) =
        FSharp.Scenario.create(name, run.Invoke)

    /// <summary>
    /// Creates an empty scenario.
    /// An empty scenario is useful when you want to create the scenario to do only initialization or cleaning and execute it separately.
    /// The need for this can be when you have a few scenarios with the same init logic, and you want to run this init logic only once.
    /// </summary>
    /// <param name="name">The name of the scenario.</param>
    static member Empty(name: string) =
        FSharp.Scenario.empty name

    /// <summary>
    /// Initializes scenario and all its dependencies.
    /// You can use it to prepare your target system, populate the database, or read and apply the JSON configuration for your scenario.
    /// Scenario init will be invoked before warm-up and bombing phases.
    /// If Scenario init throws an exception, the NBomber load test will stop the execution. 
    /// </summary>
    /// <param name="scenario">Represent configuration data that is needed to build a scenario.</param>
    /// <param name="initFunc">Represent lambda function that will be invoked to start Scenario initialization.
    /// If this lambda function throws an exception, the NBomber load test will stop the execution.</param>
    [<Extension>]
    static member WithInit(scenario: ScenarioProps, initFunc: Func<IScenarioInitContext,Task>) =
        { scenario with Init = Some initFunc.Invoke }

    /// <summary>
    /// Cleans scenario's resources and all its dependencies.
    /// This method should be used to clean the scenario's resources after the test finishes.
    /// Scenario clean will be invoked after warm-up and bombing phases.
    /// If Scenario clean throws an exception, the NBomber logs it and continues execution. 
    /// </summary>
    /// <param name="scenario">Represent configuration data that is needed to build a scenario.</param>
    /// <param name="cleanFunc">Represent lambda function that will be invoked to start Scenario cleaning.
    /// If this lambda function throws an exception, the NBomber logs it and continues execution.</param>
    [<Extension>]
    static member WithClean(scenario: ScenarioProps, cleanFunc: Func<IScenarioInitContext,Task>) =
        { scenario with Clean = Some cleanFunc.Invoke }

    /// <summary>
    /// This method sets duration of warm-up phase.    
    /// </summary>
    /// <param name="scenario">Represent configuration data that is needed to build a scenario.</param>
    /// <param name="duration">By default warm-up duration is 30 seconds.</param>
    [<Extension>]
    static member WithWarmUpDuration(scenario: ScenarioProps, duration: TimeSpan) =
        scenario |> FSharp.Scenario.withWarmUpDuration(duration)

    /// <summary>
    /// This method disables warm-up.
    /// </summary>
    /// <param name="scenario">Represent configuration data that is needed to build a scenario.</param>
    [<Extension>]
    static member WithoutWarmUp(scenario: ScenarioProps) =
        scenario |> FSharp.Scenario.withoutWarmUp

    /// <summary>
    /// This method allows configuring the load simulations for the current Scenario.
    /// Load simulation allows configuring parallelism and workload profiles.
    /// </summary>
    /// <param name="scenario">Represent configuration data that is needed to build a scenario.</param>
    /// <param name="loadSimulations">Default value is: Simulation.KeepConstant(copies: 1, during: TimeSpan.FromMinutes(1))</param>
    /// <example>
    /// <code>        
    /// scenario.WithLoadSimulations(
    ///     Simulation.RampingConstant(copies: 50, during: TimeSpan.FromMinutes(1)) // ramp-up from 0 to 50 copies
    ///     Simulation.KeepConstant(copies: 50, during: TimeSpan.FromMinutes(1))
    /// );
    /// </code>
    /// </example>
    [<Extension>]
    static member WithLoadSimulations(scenario: ScenarioProps, [<ParamArray>]loadSimulations: LoadSimulation[]) =
        scenario |> FSharp.Scenario.withLoadSimulations(Seq.toList loadSimulations)

    /// <summary>
    /// This method allows enabling or disabling the auto restart of Scenario iteration in case of Step failure.
    /// Sometimes, you would like to handle failed steps differently: retry, ignore or use a fallback.
    /// For such cases, you can disable Scenario iteration auto restart.
    /// By default, when a Step returns a failed Response or unhandled exception was thrown,
    /// NBomber will automatically mark the whole iteration as failed and restart it.
    /// If you want to disable auto restart of Scenario iteration you should set 'shouldRestart: false'.    
    /// </summary>
    /// <param name="scenario">Represent configuration data that is needed to build a scenario.</param>
    /// <param name="shouldRestart">The default value is true.
    /// If you want to disable auto restart of Scenario iteration you should set 'shouldRestart = false'.</param>
    [<Extension>]
    static member WithRestartIterationOnFail(scenario: ScenarioProps, shouldRestart: bool) =
        scenario |> FSharp.Scenario.withRestartIterationOnFail shouldRestart

    /// <summary>
    /// This method overrides the default value of MaxFailCount for Scenario.
    /// MaxFailCount is incremented on every failure or failed Response.        
    /// When a scenario reaches MaxFailCount, NBomber will stop the whole load test.
    /// In the case of cluster mode, MaxFailCount is tracked per each NBomber instance exclusively.
    /// It doesn't aggregate across the cluster. So if on any NBomber node MaxFailCount is reached, NBomber will stop the whole load test. 
    /// </summary>
    /// <param name="scenario">Represent configuration data that is needed to build a scenario.</param>
    /// <param name="maxFailCount">The default value is 5_000.</param>
    [<Extension>]
    static member WithMaxFailCount(scenario: ScenarioProps, maxFailCount: int) =
        scenario |> FSharp.Scenario.withMaxFailCount maxFailCount

/// NBomberRunner is responsible for registering and running scenarios.
/// Also it provides configuration points related to infrastructure, reporting, loading plugins.
[<Extension>]
type NBomberRunner =

    /// Registers scenarios in NBomber environment.    
    static member RegisterScenarios([<ParamArray>]scenarios: ScenarioProps[]) =
        scenarios |> Seq.toList |> FSharp.NBomberRunner.registerScenarios

    /// <summary>
    /// Sets target scenarios among all registered that will execute during the session.
    /// </summary>
    /// <param name="context">NBomberContext</param>
    /// <param name="scenarioNames">Names of scenarios that should be started during the session.</param>
    /// <example>
    /// <code>
    /// NBomberRunner
    ///     .RegisterScenarios(scenario_1, scenario_2, scenario_3)
    ///     .WithTargetScenarios("scenario_1") // only scenario_1 will be executed
    /// </code>
    /// </example>
    [<Extension>]
    static member WithTargetScenarios(context: NBomberContext, [<ParamArray>]scenarioNames: string[]) =
        let names = scenarioNames |> Seq.toList
        context |> FSharp.NBomberRunner.withTargetScenarios(names)

    /// <summary>
    /// Sets test suite name.    
    /// </summary>
    /// <param name="context">NBomberContext</param>
    /// <param name="testSuite">Default value is: "nbomber_default_test_suite_name".</param>
    [<Extension>]
    static member WithTestSuite(context: NBomberContext, testSuite: string) =
        context |> FSharp.NBomberRunner.withTestSuite(testSuite)

    /// <summary>
    /// Sets test name.    
    /// </summary>
    /// <param name="context">NBomberContext</param>
    /// <param name="testName">Default value is: "nbomber_default_test_name".</param>
    [<Extension>]
    static member WithTestName(context: NBomberContext, testName: string) =
        context |> FSharp.NBomberRunner.withTestName(testName)

    /// <summary>
    /// Sets output report file name.    
    /// </summary>
    /// <param name="context">NBomberContext</param>
    /// <param name="reportFileName">Default name: "nbomber_report-{CurrentTime}"</param>
    [<Extension>]
    static member WithReportFileName(context: NBomberContext, reportFileName: string) =
        context |> FSharp.NBomberRunner.withReportFileName(reportFileName)

    /// <summary>
    /// Sets output report folder path.    
    /// </summary>
    /// <param name="context">NBomberContext</param>
    /// <param name="reportFolderPath">Default folder path: "./reports".</param>
    [<Extension>]
    static member WithReportFolder(context: NBomberContext, reportFolderPath: string) =
        context |> FSharp.NBomberRunner.withReportFolder(reportFolderPath)

    /// <summary>
    /// Sets output report formats.
    /// </summary>
    /// <param name="context">NBomberContext</param>
    /// <param name="reportFormats">The default value is: [Txt; Html; Csv; Md]</param>
    [<Extension>]
    static member WithReportFormats(context: NBomberContext, [<ParamArray>]reportFormats: ReportFormat[]) =
        let formats = reportFormats |> Seq.toList
        context |> FSharp.NBomberRunner.withReportFormats(formats)

    /// Sets to run without reports
    [<Extension>]
    static member WithoutReports(context: NBomberContext) =
        context |> FSharp.NBomberRunner.withoutReports

    /// <summary>
    /// Sets real-time reporting interval.    
    /// </summary>
    /// <param name="context">NBomberContext</param>
    /// <param name="interval">Default value: 10 sec, min value: 5 sec</param>
    [<Extension>]
    static member WithReportingInterval(context: NBomberContext, interval: TimeSpan) =
        context |> FSharp.NBomberRunner.withReportingInterval interval

    /// Sets reporting sinks.    
    /// ReportingSink provides functionality for saving real-time and final statistics.
    [<Extension>]
    static member WithReportingSinks(context: NBomberContext, [<ParamArray>]reportingSinks: IReportingSink[]) =
        let sinks = reportingSinks |> Seq.toList
        context |> FSharp.NBomberRunner.withReportingSinks sinks

    /// Sets worker plugins.
    /// Worker plugin is a plugin that starts at the test start and works as a background worker.
    [<Extension>]
    static member WithWorkerPlugins(context: NBomberContext, [<ParamArray>]plugins: IWorkerPlugin[]) =
        let pluginsList = plugins |> Seq.toList
        context |> FSharp.NBomberRunner.withWorkerPlugins(pluginsList)

    /// <summary>
    /// Loads configuration by file path or by HTTP URL.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="path">File path or HTTP URL to JSON config.</param>
    /// <example>
    /// <code>
    /// // load by file path
    /// NBomberRunner.LoadConfig("./my-test/config.json")
    /// // load by URL
    /// NBomberRunner.LoadConfig("https://my-test/config.json")
    /// </code>
    /// </example>
    [<Extension>]
    static member LoadConfig(context: NBomberContext, path: string) =
        context |> FSharp.NBomberRunner.loadConfig(path)

    /// <summary>
    /// Loads infrastructure configuration by file path or by HTTP URL.     
    /// </summary>    
    /// <param name="context"></param>
    /// <param name="path">File path or HTTP URL to JSON config.</param>
    /// <example>
    /// <code>
    /// // load by file path
    /// NBomberRunner.LoadInfraConfig("./my-test/infra-config.json")
    /// // load by URL
    /// NBomberRunner.LoadInfraConfig("https://my-test/infra-config.json") 
    /// </code>
    /// </example>
    [<Extension>]
    static member LoadInfraConfig(context: NBomberContext, path: string) =
        context |> FSharp.NBomberRunner.loadInfraConfig(path)

    /// <summary>
    /// Sets logger configuration.
    /// Make sure that you always return a new instance of LoggerConfiguration.
    /// You can also configure the logger via JSON infrastructure config file.
    /// For this use NBomberRunner.LoadInfraConfig
    /// </summary>
    /// <param name="context">context</param>
    /// <param name="createLoggerConfig">creates a new instance of LoggerConfiguration</param>
    /// <example>
    /// <code>
    /// NBomberRunner.WithLoggerConfig(() => new LoggerConfiguration())
    /// </code>
    /// </example>
    [<Extension>]
    static member WithLoggerConfig(context: NBomberContext, createLoggerConfig: Func<LoggerConfiguration>) =
        context |> FSharp.NBomberRunner.withLoggerConfig(createLoggerConfig.Invoke)

    /// <summary>
    /// This method enables or disables hints analyzer.
    /// Hints analyzer - analyzes statistics at the end of the test to provide hints in case of finding the wrong usage of NBomber or some environmental issues.    
    /// </summary>    
    /// <param name="context">NBomberContext</param>
    /// <param name="enable">The default value is false.</param>
    [<Extension>]
    static member EnableHintsAnalyzer(context: NBomberContext, enable: bool) =
        context |> FSharp.NBomberRunner.enableHintsAnalyzer enable

    /// Runs scenarios.
    [<Extension>]
    static member Run(context: NBomberContext) =
        match FSharp.NBomberRunner.run context with
        | Ok stats  -> stats
        | Error msg -> failwith msg

    /// <summary>
    /// Runs scenarios with CLI arguments.    
    /// </summary>
    /// <param name="context">NBomberContext</param>
    /// <param name="args">CLI args</param>
    /// <example>
    /// <code>
    /// Example:
    /// --config=config.json --infra=infra_config.json --target=scenario_1
    /// </code>
    /// </example>
    [<Extension>]
    static member Run(context: NBomberContext, [<ParamArray>]args: string[]) =
        match FSharp.NBomberRunner.runWithArgs args context with
        | Ok stats  -> stats
        | Error msg -> failwith msg

/// Represents Load Simulation.
/// Load Simulation allows configuring parallelism and workload profiles.
type Simulation =

    /// <summary>
    /// Adds or removes a given number of Scenario copies(instances) with a linear ramp over a given duration.    
    /// Each Scenario copy behaves like a long-running thread that runs continually(by specified duration) and will be destroyed when the current load simulation stops.
    /// Use it for a smooth ramp up and ramp down.
    /// Usually, this simulation type is used to test databases, message brokers, or any other system that works with a static client's pool of connections and reuses them.
    /// </summary>
    /// <param name="copies">The number of concurrent Scenario copies that will be running in parallel.</param>
    /// <param name="during">The duration of load simulation.</param>
    static member RampingConstant(copies: int, during: TimeSpan) =
        LoadSimulation.RampingConstant(copies, during)

    /// <summary>
    /// Keeps activated(constantly running) a fixed number of Scenario copies(instances) which executes as many iterations as possible for a specified duration.
    /// Each Scenario copy behaves like a long-running thread that runs continually(by specified duration) and will be destroyed when the current load simulation stops.
    /// Use it when you need to run and keep a constant amount of Scenario copies for a specific period.
    /// Usually, this simulation type is used to test databases, message brokers, or any other system that works with a static client's pool of connections and reuses them.
    /// </summary>
    /// <param name="copies">The number of concurrent Scenario copies that will be running in parallel.</param>
    /// <param name="during">The duration of load simulation.</param>
    static member KeepConstant(copies: int, during: TimeSpan) =
        LoadSimulation.KeepConstant(copies, during)

    /// <summary>
    /// Injects a given number of Scenario copies(instances) with a linear ramp over a given duration.
    /// Each Scenario copy behaves like a short-running thread that runs only once and then is destroyed.
    /// With this simulation, you control the Scenario injection rate and injection interval.
    /// Use it for a smooth ramp up and ramp down.
    /// Usually, this simulation type is used to test HTTP API.
    /// </summary>
    /// <param name="rate">The injection rate of Scenario copies. It configures how many concurrent copies will be injected at a time.</param>
    /// <param name="interval">The injection interval. It configures the interval between injections. </param>
    /// <param name="during">The duration of load simulation.</param>
    static member RampingInject(rate: int, interval: TimeSpan, during: TimeSpan) =
        LoadSimulation.RampingInject(rate, interval, during)

    /// <summary>
    /// Injects a given number of Scenario copies(instances) during a given duration.
    /// Each Scenario copy behaves like a short-running thread that runs only once and then is destroyed.
    /// With this simulation, you control the Scenario injection rate and injection interval.
    /// Use it when you want to maintain a constant rate of requests without being affected by the performance of the system you load test.
    /// Usually, this simulation type is used to test HTTP API.
    /// </summary>
    /// <param name="rate">The injection rate of Scenario copies. It configures how many concurrent copies will be injected at a time.</param>
    /// <param name="interval">The injection interval. It configures the interval between injections. </param>
    /// <param name="during">The duration of load simulation.</param> 
    static member Inject(rate: int, interval: TimeSpan, during: TimeSpan) =
        LoadSimulation.Inject(rate, interval, during)

    /// <summary>
    /// Injects a given random number of Scenario copies(instances) during a given duration.
    /// Each Scenario copy behaves like a short-running thread that runs only once and then is destroyed.
    /// With this simulation, you control the Scenario injection rate and injection interval.
    /// Use it when you want to maintain a random rate of requests without being affected by the performance of the system you load test.
    /// Usually, this simulation type is used to test HTTP API.
    /// </summary>
    /// <param name="minRate">The min injection rate of Scenario copies.</param>
    /// <param name="maxRate">The max injection rate of Scenario copies.</param>
    /// <param name="interval">The injection interval. It configures the interval between injections.</param>
    /// <param name="during">The duration of load simulation.</param>
    static member InjectRandom(minRate:int, maxRate:int, interval: TimeSpan, during:TimeSpan) =
        LoadSimulation.InjectRandom(minRate, maxRate, interval, during)

    /// <summary>
    /// Introduces Scenario pause simulation for a given duration.
    /// It's useful for cases when some Scenario start should be delayed or paused in the middle of execution.
    /// </summary>
    /// <param name="during">The duration of load simulation.</param>
    static member Pause(during:TimeSpan) =
        LoadSimulation.Pause during

[<Extension>]
type OptionExtensions =

    [<Extension>]
    static member IsSome (option: Option<'T>) = option.IsSome

    [<Extension>]
    static member IsNone (option: Option<'T>) = option.IsNone

