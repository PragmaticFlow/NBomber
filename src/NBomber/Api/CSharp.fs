namespace NBomber.CSharp

#nowarn "3211"

open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

open Serilog

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Contracts.Thresholds

/// ClientFactory helps you create and initialize API clients to work with specific API or protocol (HTTP, WebSockets, gRPC, GraphQL).
type ClientFactory =

    /// Creates ClientFactory.
    /// ClientFactory helps create and initialize API clients to work with specific API or protocol (HTTP, WebSockets, gRPC, GraphQL).
    static member Create
        (name: string,
         initClient: Func<int,IBaseContext,Task<'TClient>>,
         [<Optional;DefaultParameterValue(null)>] disposeClient: Func<'TClient,IBaseContext,Task>,
         [<Optional;DefaultParameterValue(Constants.DefaultClientCount)>] clientCount: int) =

        let defaultDispose = (fun (client,context) ->
            match client :> obj with
            | :? IDisposable as d -> d.Dispose()
            | _ -> ()
            Task.CompletedTask
        )

        let dispose =
            if isNull(disposeClient :> obj) then defaultDispose
            else disposeClient.Invoke

        NBomber.Domain.ClientFactory.ClientFactory(name, clientCount, initClient.Invoke, dispose)
        :> IClientFactory<'TClient>

/// DataFeed helps inject test data into your load test. It represents a data source.
type Feed =

    /// Creates Feed that picks constant value per Step copy.
    /// Every Step copy will have unique constant value.
    static member CreateConstant (name, data: 'T seq) =
        FSharp.Feed.createConstant name data

    /// Creates Feed (in lazy mode) that picks constant value per Step copy.
    /// Every Step copy will have unique constant value.
    static member CreateConstantLazy (name, getData: Func<IBaseContext,'T seq>) =
        FSharp.Feed.createConstantLazy name getData.Invoke

    /// Creates Feed that goes back to the top of the sequence once the end is reached.
    static member CreateCircular (name, data: 'T seq) =
        FSharp.Feed.createCircular name data

    /// Creates Feed (in lazy mode) that goes back to the top of the sequence once the end is reached.
    static member CreateCircularLazy (name, getData: Func<IBaseContext,'T seq>) =
        FSharp.Feed.createCircularLazy name getData.Invoke

    /// Creates Feed that randomly picks an item per Step invocation.
    static member CreateRandom (name, data: 'T seq) =
        FSharp.Feed.createRandom name data

    /// Creates Feed (in lazy mode) that randomly picks an item per Step invocation.
    static member CreateRandomLazy (name, getData: Func<IBaseContext,'T seq>) =
        FSharp.Feed.createRandomLazy name getData.Invoke

/// Step represents a single user action like login, logout, etc.
type Step =

    /// Creates Step.
    /// Step represents a single user action like login, logout, etc.
    static member Create
        (name: string,
         clientFactory: IClientFactory<'TClient>,
         clientDistribution: Func<IStepClientContext<'TFeedItem>,int>,
         feed: IFeed<'TFeedItem>,
         execute: Func<IStepContext<'TClient,'TFeedItem>,Task<Response>>,
         [<Optional;DefaultParameterValue(null)>] timeout: Nullable<TimeSpan>,
         [<Optional;DefaultParameterValue(Constants.DefaultDoNotTrack)>] doNotTrack: bool) =

        let timeout = Option.ofNullable timeout
        FSharp.Step.create(name, execute.Invoke, clientFactory, clientDistribution.Invoke, feed, ?timeout = timeout, doNotTrack = doNotTrack)

    /// Creates Step.
    /// Step represents a single user action like login, logout, etc.
    static member Create
        (name: string,
         clientFactory: IClientFactory<'TClient>,
         clientDistribution: Func<IStepClientContext<unit>,int>,
         execute: Func<IStepContext<'TClient,unit>,Task<Response>>,
         [<Optional;DefaultParameterValue(null)>] timeout: Nullable<TimeSpan>,
         [<Optional;DefaultParameterValue(Constants.DefaultDoNotTrack)>] doNotTrack: bool) =

        let timeout = Option.ofNullable timeout
        FSharp.Step.create(name, execute.Invoke, clientFactory, clientDistribution.Invoke, ?timeout = timeout, doNotTrack = doNotTrack)

    /// Creates Step.
    /// Step represents a single user action like login, logout, etc.
    static member Create
        (name: string,
         clientFactory: IClientFactory<'TClient>,
         execute: Func<IStepContext<'TClient,unit>,Task<Response>>,
         [<Optional;DefaultParameterValue(null)>] timeout: Nullable<TimeSpan>,
         [<Optional;DefaultParameterValue(Constants.DefaultDoNotTrack)>] doNotTrack: bool) =

        let timeout = Option.ofNullable timeout
        FSharp.Step.create(name, execute.Invoke, clientFactory, ?timeout = timeout, doNotTrack = doNotTrack)

    /// Creates Step.
    /// Step represents a single user action like login, logout, etc.
    static member Create
        (name: string,
         feed: IFeed<'TFeedItem>,
         execute: Func<IStepContext<unit,'TFeedItem>,Task<Response>>,
         [<Optional;DefaultParameterValue(null)>] timeout: Nullable<TimeSpan>,
         [<Optional;DefaultParameterValue(Constants.DefaultDoNotTrack)>] doNotTrack: bool) =

        let timeout = Option.ofNullable timeout
        FSharp.Step.create(name, execute.Invoke, feed = feed, ?timeout = timeout, doNotTrack = doNotTrack)

    /// Creates Step.
    /// Step represents a single user action like login, logout, etc.
    static member Create
        (name: string,
         execute: Func<IStepContext<unit,unit>,Task<Response>>,
         [<Optional;DefaultParameterValue(null)>] timeout: Nullable<TimeSpan>,
         [<Optional;DefaultParameterValue(Constants.DefaultDoNotTrack)>] doNotTrack: bool) =

        let timeout = Option.ofNullable timeout
        FSharp.Step.create(name, execute.Invoke, ?timeout = timeout, doNotTrack = doNotTrack)

    /// Creates pause step with specified duration.
    static member CreatePause(duration: TimeSpan) =
        FSharp.Step.createPause(duration)

    /// Creates pause step with specified duration in milliseconds.
    static member CreatePause(milliseconds: int) =
        FSharp.Step.createPause(milliseconds)

    /// Creates pause step with specified duration in lazy mode.
    /// It's useful when you want to fetch value from some configuration.
    static member CreatePause(getDuration: Func<TimeSpan>) =
        FSharp.Step.createPause(getDuration.Invoke)

    /// Creates pause step in milliseconds in lazy mode.
    /// It's useful when you want to fetch value from some configuration.
    static member CreatePause(getDuration: Func<int>) =
        FSharp.Step.createPause(getDuration.Invoke)

/// Scenario is basically a workflow that virtual users will follow. It helps you organize steps into user actions.
/// Scenarios are always running in parallel (it's opposite to steps that run sequentially).
/// You should think about Scenario as a system thread.
[<Extension>]
type ScenarioBuilder =

    /// Creates scenario with steps which will be executed sequentially.
    /// Scenario is basically a workflow that virtual users will follow. It helps you organize steps into user actions.
    /// Scenarios are always running in parallel (it's opposite to steps that run sequentially).
    /// You should think about Scenario as a system thread.
    static member CreateScenario(name: string, [<ParamArray>]steps: IStep[]) =
        FSharp.Scenario.create name (Seq.toList steps)

    /// Initializes scenario.
    /// You can use it to for example to prepare your target system or to parse and apply configuration.
    [<Extension>]
    static member WithInit(scenario: Scenario, initFunc: Func<IScenarioContext,Task>) =
        { scenario with Init = Some initFunc.Invoke }

    /// Cleans scenario's resources.
    [<Extension>]
    static member WithClean(scenario: Scenario, cleanFunc: Func<IScenarioContext,Task>) =
        { scenario with Clean = Some cleanFunc.Invoke }

    /// Sets warm-up duration
    /// Warm-up will just simply start a scenario with a specified duration.
    [<Extension>]
    static member WithWarmUpDuration(scenario: Scenario, duration: TimeSpan) =
        scenario |> FSharp.Scenario.withWarmUpDuration(duration)

    [<Extension>]
    static member WithoutWarmUp(scenario: Scenario) =
        scenario |> FSharp.Scenario.withoutWarmUp

    /// Sets load simulations.
    /// Default value is: KeepConstant(copies = 1, during = minutes 1)
    /// NBomber is always running simulations in sequential order that you defined them.
    /// All defined simulations are represent the whole Scenario duration.
    [<Extension>]
    static member WithLoadSimulations(scenario: Scenario, [<ParamArray>]loadSimulations: LoadSimulation[]) =
        scenario |> FSharp.Scenario.withLoadSimulations(Seq.toList loadSimulations)

    /// Sets custom steps order that will be used by NBomber Scenario executor.
    /// By default, all steps are executing sequentially but you can inject your custom order.
    /// getStepsOrder function will be invoked on every turn before steps list execution.
    [<Extension>]
    static member WithCustomStepOrder(scenario: Scenario, getStepsOrder: Func<string[]>) =
        scenario |> FSharp.Scenario.withCustomStepOrder(getStepsOrder.Invoke)

    /// Sets custom steps execution control.
    /// It introduces more granular execution control of your steps than you can achieve with CustomStepOrder.
    /// By default, all steps are executing sequentially but you can inject your custom execution control to change
    /// default order per step iteration.
    /// execControl function will be invoked before each step. You can think about execControl like a callback before step invocation
    /// where you can specify what step should be invoked.
    [<Extension>]
    static member WithCustomStepExecControl(scenario: Scenario, execControl: Func<IStepExecControlContext voption, string voption>) =
        scenario |> FSharp.Scenario.withCustomStepExecControl(execControl.Invoke)

    [<Extension>]
    static member WithThresholds(scenario: Scenario, [<ParamArray>]thresholds: Threshold[]) =
        scenario |> FSharp.Scenario.withThresholds(Seq.toList thresholds)

[<Extension>]
type NBomberRunner =

    /// Registers scenarios in NBomber environment.
    /// Scenarios will be run in parallel.
    static member RegisterScenarios([<ParamArray>]scenarios: Scenario[]) =
        scenarios |> Seq.toList |> FSharp.NBomberRunner.registerScenarios

    /// Sets target scenarios among all registered that will execute
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

    /// Sets worker plugins.
    /// Worker plugin is a plugin that starts at the test start and works as a background worker.
    [<Extension>]
    static member WithWorkerPlugins(context: NBomberContext, [<ParamArray>]plugins: IWorkerPlugin[]) =
        let pluginsList = plugins |> Seq.toList
        context |> FSharp.NBomberRunner.withWorkerPlugins(pluginsList)

    /// Loads configuration.
    /// The following formats are supported:
    /// - json (.json)
    [<Extension>]
    static member LoadConfig(context: NBomberContext, path: string) =
        context |> FSharp.NBomberRunner.loadConfig(path)

    /// Loads infrastructure configuration.
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

    /// Disables hints analyzer.
    /// Hints analyzer - analyze node stats to provide some hints in case of finding wrong usage or some other issue.
    [<Extension>]
    static member DisableHintsAnalyzer(context: NBomberContext) =
        context |> FSharp.NBomberRunner.disableHintsAnalyzer

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
    static member RampConstant(copies: int, during: TimeSpan) =
        LoadSimulation.RampConstant(copies, during)

    /// A fixed number of scenario copies (threads) executes as many iterations as possible for a specified amount of time.
    /// Every single scenario copy will iterate while the specified duration.
    /// Use it when you need to run a specific amount of scenario copies (threads) for a certain amount of time.
    static member KeepConstant(copies: int, during: TimeSpan) =
        LoadSimulation.KeepConstant(copies, during)

    /// Injects a given number of scenario copies (threads) per 1 sec from the current rate to target rate during a given duration.
    /// Every single scenario copy will run only once.
    static member RampPerSec(rate: int, during: TimeSpan) =
        LoadSimulation.RampPerSec(rate, during)

    /// Injects a given number of scenario copies (threads) per 1 sec during a given duration.
    /// Every single scenario copy will run only once.
    /// Use it when you want to maintain a constant rate of requests without being affected by the performance of the system under test.
    static member InjectPerSec(rate: int, during: TimeSpan) =
        LoadSimulation.InjectPerSec(rate, during)

    /// Injects a random number of scenario copies (threads) per 1 sec during a given duration.
    /// Every single scenario copy will run only once.
    /// Use it when you want to maintain a random rate of requests without being affected by the performance of the system under test.
    static member InjectPerSecRandom(minRate:int, maxRate:int, during:TimeSpan) =
        LoadSimulation.InjectPerSecRandom(minRate, maxRate, during)

type ValueOption =

    static member Some(value: 'T) = ValueSome value
    static member None() = ValueNone

type Threshold =

    static member RequestAllCount(predicate: Func<float, bool>) =
        RequestAllCount(predicate.Invoke)

    static member RequestOkCount(predicate: Func<float, bool>) =
        RequestOkCount(predicate.Invoke)

    static member RequestFailedCount(predicate: Func<float, bool>) =
        RequestFailedCount(predicate.Invoke)

    static member RequestFailedRate(predicate: Func<float, bool>) =
        RequestFailedRate(predicate.Invoke)

    static member RPS(predicate: Func<float, bool>) =
        RPS(predicate.Invoke)

    static member LatencyMin(predicate: Func<float, bool>) =
        LatencyMin(predicate.Invoke)

    static member LatencyMean(predicate: Func<float, bool>) =
        LatencyMean(predicate.Invoke)

    static member LatencyMax(predicate: Func<float, bool>) =
        LatencyMax(predicate.Invoke)

    static member LatencyStdDev(predicate: Func<float, bool>) =
        LatencyStdDev(predicate.Invoke)

    static member LatencyPercent50(predicate: Func<float, bool>) =
        LatencyPercent50(predicate.Invoke)

    static member LatencyPercent75(predicate: Func<float, bool>) =
        LatencyPercent75(predicate.Invoke)

    static member LatencyPercent95(predicate: Func<float, bool>) =
        LatencyPercent95(predicate.Invoke)

    static member LatencyPercent99(predicate: Func<float, bool>) =
        LatencyPercent99(predicate.Invoke)

    static member DataTransferMinBytes(predicate: Func<int, bool>) =
        DataTransferMinBytes(predicate.Invoke)

    static member DataTransferMeanBytes(predicate: Func<int, bool>) =
        DataTransferMeanBytes(predicate.Invoke)

    static member DataTransferMaxBytes(predicate: Func<int, bool>) =
        DataTransferMaxBytes(predicate.Invoke)

    static member DataTransferPercent50(predicate: Func<int, bool>) =
        DataTransferPercent50(predicate.Invoke)

    static member DataTransferPercent75(predicate: Func<int, bool>) =
        DataTransferPercent75(predicate.Invoke)

    static member DataTransferPercent95(predicate: Func<int, bool>) =
        DataTransferPercent95(predicate.Invoke)

    static member DataTransferPercent99(predicate: Func<int, bool>) =
        DataTransferPercent99(predicate.Invoke)

    static member DataTransferStdDev(predicate: Func<float, bool>) =
        DataTransferStdDev(predicate.Invoke)

    static member DataTransferAllBytes(predicate: Func<int64, bool>) =
        DataTransferAllBytes(predicate.Invoke)
