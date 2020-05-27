namespace NBomber.CSharp

#nowarn "3211"

open System
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

open NBomber
open NBomber.Contracts
open NBomber.Configuration

type ConnectionPoolArgs =

    static member Create<'TConnection>(name: string,
                                       openConnection: Func<int,CancellationToken,Task<'TConnection>>,
                                       closeConnection: Func<'TConnection,CancellationToken,Task>,
                                       [<Optional;DefaultParameterValue(Constants.DefaultConnectionCount:int)>]connectionCount: int) =
        FSharp.ConnectionPoolArgs.create(name, openConnection.Invoke, closeConnection.Invoke, connectionCount)

    static member Empty = FSharp.ConnectionPoolArgs.empty

type Step =

    static member Create<'TConnection,'TFeedItem>
        (name: string,
         connectionPoolArgs: IConnectionPoolArgs<'TConnection>,
         feed: IFeed<'TFeedItem>,
         execute: Func<IStepContext<'TConnection,'TFeedItem>,Task<Response>>,
         [<Optional;DefaultParameterValue(Constants.DefaultDoNotTrack:bool)>]doNotTrack: bool) =

        FSharp.Step.create(name, connectionPoolArgs, feed, execute.Invoke, doNotTrack)

    static member Create<'TConnection>
        (name: string,
         connectionPoolArgs: IConnectionPoolArgs<'TConnection>,
         execute: Func<IStepContext<'TConnection,unit>,Task<Response>>,
         [<Optional;DefaultParameterValue(Constants.DefaultDoNotTrack:bool)>]doNotTrack: bool) =

        Step.Create(name, connectionPoolArgs, Feed.empty, execute, doNotTrack)

    static member Create<'TFeedItem>
        (name: string,
         feed: IFeed<'TFeedItem>,
         execute: Func<IStepContext<unit,'TFeedItem>,Task<Response>>,
         [<Optional;DefaultParameterValue(Constants.DefaultDoNotTrack:bool)>]doNotTrack: bool) =

        Step.Create(name, ConnectionPoolArgs.Empty, feed, execute, doNotTrack)

    static member Create(name: string,
                         execute: Func<IStepContext<unit,unit>,Task<Response>>,
                         [<Optional;DefaultParameterValue(Constants.DefaultDoNotTrack:bool)>]doNotTrack: bool) =

        Step.Create(name, ConnectionPoolArgs.Empty, Feed.empty, execute, doNotTrack)

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

[<Extension>]
type ScenarioBuilder =

    /// Creates scenario with steps which will be executed sequentially.
    static member CreateScenario(name: string, [<System.ParamArray>]steps: IStep[]) =
        FSharp.Scenario.create name (Seq.toList steps)

    [<Extension>]
    static member WithTestInit(scenario: Scenario, initFunc: Func<ScenarioContext,Task>) =
        { scenario with TestInit = Some initFunc.Invoke }

    [<Extension>]
    static member WithTestClean(scenario: Scenario, cleanFunc: Func<ScenarioContext,Task>) =
        { scenario with TestClean = Some cleanFunc.Invoke }

    [<Extension>]
    static member WithWarmUpDuration(scenario: Scenario, duration: TimeSpan) =
        scenario |> FSharp.Scenario.withWarmUpDuration(duration)

    [<Extension>]
    static member WithoutWarmUp(scenario: Scenario) =
        scenario |> FSharp.Scenario.withoutWarmUp

    [<Extension>]
    static member WithLoadSimulations (scenario: Scenario, [<System.ParamArray>]loadSimulations: LoadSimulation[]) =
        scenario |> FSharp.Scenario.withLoadSimulations(Seq.toList loadSimulations)

[<Extension>]
type NBomberRunner =

    /// Registers scenarios in NBomber environment. Scenarios will be run in parallel.
    static member RegisterScenarios([<System.ParamArray>]scenarios: Contracts.Scenario[]) =
        scenarios |> Seq.toList |> FSharp.NBomberRunner.registerScenarios

    /// Loads configuration.
    /// The following formats are supported:
    /// - json (.json),
    /// - yaml (.yml, .yaml).
    [<Extension>]
    static member LoadConfig(context: NBomberContext, path: string) =
        context |> FSharp.NBomberRunner.loadConfig(path)

    /// Loads infrastructure configuration.
    /// The following formats are supported:
    /// - json (.json),
    /// - yaml (.yml, .yaml).
    [<Extension>]
    static member LoadInfraConfig(context: NBomberContext, path: string) =
        context |> FSharp.NBomberRunner.loadInfraConfig(path)

    [<Extension>]
    static member WithReportFileName(context: NBomberContext, reportFileName: string) =
        context |> FSharp.NBomberRunner.withReportFileName(reportFileName)

    [<Extension>]
    static member WithReportFormats(context: NBomberContext, [<System.ParamArray>]reportFormats: ReportFormat[]) =
        let formats = reportFormats |> Seq.toList
        context |> FSharp.NBomberRunner.withReportFormats(formats)

    [<Extension>]
    static member WithoutReports(context: NBomberContext) =
        context |> FSharp.NBomberRunner.withoutReports

    [<Extension>]
    static member WithTestSuite(context: NBomberContext, testSuite: string) =
        context |> FSharp.NBomberRunner.withTestSuite(testSuite)

    [<Extension>]
    static member WithTestName(context: NBomberContext, testName: string) =
        context |> FSharp.NBomberRunner.withTestName(testName)

    [<Extension>]
    static member WithReportingSinks(context: NBomberContext, [<System.ParamArray>]reportingSinks: IReportingSink[], sendStatsInterval: TimeSpan) =
        let sinks = reportingSinks |> Seq.toList
        context |> FSharp.NBomberRunner.withReportingSinks(sinks, sendStatsInterval)

    [<Extension>]
    static member WithPlugins(context: NBomberContext, [<System.ParamArray>]plugins: IPlugin[]) =
        let pluginsList = plugins |> Seq.toList
        context |> FSharp.NBomberRunner.withPlugins(pluginsList)

    /// Sets application type.
    /// The following application types are supported:
    /// - Console: is suitable for interactive session (will display progress bar)
    /// - Process: is suitable for running tests under test runners (progress bar will not be shown)
    /// By default NBomber will automatically identify your environment: Process or Console.
    [<Extension>]
    static member WithApplicationType(context: NBomberContext, applicationType: ApplicationType) =
        context |> FSharp.NBomberRunner.withApplicationType(applicationType)

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
    /// [|"-c"; "config.yaml"; "-i"; "infra_config.yaml"|]
    /// [|"--config"; "config.yaml"; "--infra"; "infra_config.yaml"|]
    [<Extension>]
    static member Run(context: NBomberContext, [<System.ParamArray>]args: string[]) =
        match FSharp.NBomberRunner.runWithArgs args context with
        | Ok stats  -> stats
        | Error msg -> failwith msg

type Simulation =

    static member KeepConcurrentScenarios(copiesCount: int, during: TimeSpan) =
        LoadSimulation.KeepConcurrentScenarios(copiesCount, during)

    static member RampConcurrentScenarios(copiesCount: int, during: TimeSpan) =
        LoadSimulation.RampConcurrentScenarios(copiesCount, during)

    static member InjectScenariosPerSec(copiesCount: int, during: TimeSpan) =
        LoadSimulation.InjectScenariosPerSec(copiesCount, during)

    static member RampScenariosPerSec(copiesCount: int, during: TimeSpan) =
        LoadSimulation.RampScenariosPerSec(copiesCount, during)
