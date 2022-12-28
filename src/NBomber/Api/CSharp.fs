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

/// Step represents a single user action like login, logout, etc.
type Step =

    /// Runs a step.
    /// Step represents a single user action like login, logout, etc.
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member Run(name: string, context: IScenarioContext, run: Func<Task<Response<'T>>>) =
        FSharp.Step.run(name, context, run.Invoke)

/// Scenario is basically a workflow that virtual users will follow. It helps you organize steps into user actions.
/// You should think about Scenario as a system thread.
[<Extension>]
type Scenario =

    /// Creates scenario.
    /// Scenario is basically a workflow that virtual users will follow. It helps you organize steps into user actions.
    /// You should think about Scenario as a system thread.
    static member Create(name: string, run: Func<IScenarioContext,Task<IResponse>>) =
        FSharp.Scenario.create(name, run.Invoke)

    /// Creates empty scenario.
    /// An empty scenario is useful when you want to create the scenario to do only initialization or cleaning and execute it separately.
    /// The need for this can be when you have a few scenarios with the same init logic, and you want to run this init logic only once.
    static member Empty(name: string) =
        FSharp.Scenario.empty name

    /// Initializes scenario.
    /// You can use it to for example to prepare your target system or to parse and apply configuration.
    [<Extension>]
    static member WithInit(scenario: ScenarioProps, initFunc: Func<IScenarioInitContext,Task>) =
        { scenario with Init = Some initFunc.Invoke }

    /// Cleans scenario's resources.
    [<Extension>]
    static member WithClean(scenario: ScenarioProps, cleanFunc: Func<IScenarioInitContext,Task>) =
        { scenario with Clean = Some cleanFunc.Invoke }

    /// Sets warm-up duration
    /// Warm-up will just simply start a scenario with a specified duration.
    [<Extension>]
    static member WithWarmUpDuration(scenario: ScenarioProps, duration: TimeSpan) =
        scenario |> FSharp.Scenario.withWarmUpDuration(duration)

    [<Extension>]
    static member WithoutWarmUp(scenario: ScenarioProps) =
        scenario |> FSharp.Scenario.withoutWarmUp

    /// Sets load simulations.
    /// Default value is: KeepConstant(copies = 1, during = minutes 1)
    /// NBomber is always running simulations in sequential order that you defined them.
    /// All defined simulations are represent the whole Scenario duration.
    [<Extension>]
    static member WithLoadSimulations(scenario: ScenarioProps, [<ParamArray>]loadSimulations: LoadSimulation[]) =
        scenario |> FSharp.Scenario.withLoadSimulations(Seq.toList loadSimulations)

    /// This method allows enabling or disabling the reset of Scenario iteration in case of step failure.
    /// By default, on fail Step response, NBomber will reset the current Scenario iteration.
    /// Sometimes, you would like to handle failed steps differently: retry, ignore or use a fallback.
    /// For such cases, you can disable scenario iteration reset.
    /// The default value is true.
    [<Extension>]
    static member WithResetIterationOnFail(scenario: ScenarioProps, shouldReset: bool) =
        scenario |> FSharp.Scenario.withResetIterationOnFail shouldReset

    /// Sets and overrides the default max fail count.
    /// When a scenario reaches max fail count, NBomber will stop the whole load test.
    /// By default MaxFailCount = 5_000
    [<Extension>]
    static member WithMaxFailCount(scenario: ScenarioProps, failCount: int) =
        scenario |> FSharp.Scenario.withMaxFailCount failCount

[<Extension>]
type NBomberRunner =

    /// Registers scenarios in NBomber environment.
    /// Scenarios will be run in parallel.
    static member RegisterScenarios([<ParamArray>]scenarios: ScenarioProps[]) =
        scenarios |> Seq.toList |> FSharp.NBomberRunner.registerScenarios

    /// Sets target scenarios among all registered that will execute
    [<Extension>]
    static member WithTargetScenarios(context: NBomberContext, [<ParamArray>]scenarioNames: string[]) =
        let names = scenarioNames |> Seq.toList
        context |> FSharp.NBomberRunner.withTargetScenarios(names)

    /// Sets test suite name
    /// Default value is: nbomber_default_test_suite_name.
    [<Extension>]
    static member WithTestSuite(context: NBomberContext, testSuite: string) =
        context |> FSharp.NBomberRunner.withTestSuite(testSuite)

    /// Sets test name
    /// Default value is: nbomber_default_test_name.
    [<Extension>]
    static member WithTestName(context: NBomberContext, testName: string) =
        context |> FSharp.NBomberRunner.withTestName(testName)

    /// Sets output report name.
    /// Default name: nbomber_report.
    [<Extension>]
    static member WithReportFileName(context: NBomberContext, reportFileName: string) =
        context |> FSharp.NBomberRunner.withReportFileName(reportFileName)

    /// Sets output report folder path.
    /// Default folder path: "./reports".
    [<Extension>]
    static member WithReportFolder(context: NBomberContext, reportFolderPath: string) =
        context |> FSharp.NBomberRunner.withReportFolder(reportFolderPath)

    [<Extension>]
    static member WithReportFormats(context: NBomberContext, [<ParamArray>]reportFormats: ReportFormat[]) =
        let formats = reportFormats |> Seq.toList
        context |> FSharp.NBomberRunner.withReportFormats(formats)

    /// Sets to run without reports
    [<Extension>]
    static member WithoutReports(context: NBomberContext) =
        context |> FSharp.NBomberRunner.withoutReports

    /// Sets real-time reporting interval.
    /// Default value: 10 seconds, min value: 5 sec
    [<Extension>]
    static member WithReportingInterval(context: NBomberContext, interval: TimeSpan) =
        context |> FSharp.NBomberRunner.withReportingInterval interval

    /// Sets reporting sinks.
    /// Reporting sink is used to save real-time metrics to correspond database.
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

    /// Loads configuration by full file path or by HTTP URL.
    /// The following formats are supported:
    /// - json (.json)
    [<Extension>]
    static member LoadConfig(context: NBomberContext, path: string) =
        context |> FSharp.NBomberRunner.loadConfig(path)

    /// Loads infrastructure configuration by full file path or by HTTP URL.
    /// The following formats are supported:
    /// - json (.json)
    [<Extension>]
    static member LoadInfraConfig(context: NBomberContext, path: string) =
        context |> FSharp.NBomberRunner.loadInfraConfig(path)

    /// Sets logger configuration.
    /// Make sure that you always return a new instance of LoggerConfiguration.
    /// You can also configure logger via configuration file.
    /// For this use NBomberRunner.loadInfraConfig
    [<Extension>]
    static member WithLoggerConfig(context: NBomberContext, buildLoggerConfig: Func<LoggerConfiguration>) =
        context |> FSharp.NBomberRunner.withLoggerConfig(buildLoggerConfig.Invoke)

    /// Enables or disables hints analyzer.
    /// Hints analyzer - analyze node stats to provide some hints in case of finding wrong usage or some other issue.
    /// The default value is false.
    [<Extension>]
    static member EnableHintsAnalyzer(context: NBomberContext, enable: bool) =
        context |> FSharp.NBomberRunner.enableHintsAnalyzer enable

    [<Extension>]
    static member Run(context: NBomberContext) =
        match FSharp.NBomberRunner.run context with
        | Ok stats  -> stats
        | Error msg -> failwith msg

    /// Runs scenarios with arguments.
    /// The following CLI commands are supported:
    /// -c or --config: loads configuration,
    /// -i or --infra: loads infrastructure configuration.
    /// Examples of possible args:
    /// -c config.json -i infra_config.json
    /// --config=config.json --infra=infra_config.json
    [<Extension>]
    static member Run(context: NBomberContext, [<ParamArray>]args: string[]) =
        match FSharp.NBomberRunner.runWithArgs args context with
        | Ok stats  -> stats
        | Error msg -> failwith msg

type Simulation =

    /// Injects a given number of scenario copies (threads) with a linear ramp over a given duration.
    /// Every single scenario copy will iterate while the specified duration.
    /// Use it for ramp up and rump down.
    static member RampingConstant(copies: int, during: TimeSpan) =
        LoadSimulation.RampingConstant(copies, during)

    /// A fixed number of scenario copies (threads) executes as many iterations as possible for a specified amount of time.
    /// Every single scenario copy will iterate while the specified duration.
    /// Use it when you need to run a specific amount of scenario copies (threads) for a certain amount of time.
    static member KeepConstant(copies: int, during: TimeSpan) =
        LoadSimulation.KeepConstant(copies, during)

    /// Injects a given number of scenario copies (threads) from the current rate to the target rate during a given duration.
    /// Every single scenario copy will run only once.
    static member RampingInject(rate: int, interval: TimeSpan, during: TimeSpan) =
        LoadSimulation.RampingInject(rate, interval, during)

    /// Injects a given number of scenario copies (threads) during a given duration.
    /// Every single scenario copy will run only once.
    /// Use it when you want to maintain a constant rate of requests without being affected by the performance of the system under test.
    static member Inject(rate: int, interval: TimeSpan, during: TimeSpan) =
        LoadSimulation.Inject(rate, interval, during)

    /// Injects a random number of scenario copies (threads) during a given duration.
    /// Every single scenario copy will run only once.
    /// Use it when you want to maintain a random rate of requests without being affected by the performance of the system under test.
    static member InjectRandom(minRate:int, maxRate:int, interval: TimeSpan, during:TimeSpan) =
        LoadSimulation.InjectRandom(minRate, maxRate, interval, during)

    /// Pause for a given duration
    static member Pause(during:TimeSpan) =
        LoadSimulation.Pause during

[<Extension>]
type OptionExtensions =

    [<Extension>]
    static member IsSome (option: Option<'T>) = option.IsSome

    [<Extension>]
    static member IsNone (option: Option<'T>) = option.IsNone

