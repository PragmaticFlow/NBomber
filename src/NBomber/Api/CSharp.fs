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
                                       getConnectionCount: Func<int>,
                                       openConnection: Func<int,CancellationToken,Task<'TConnection>>,
                                       closeConnection: Func<'TConnection,CancellationToken,Task>) =
        FSharp.ConnectionPoolArgs.create(name, getConnectionCount.Invoke, openConnection.Invoke, closeConnection.Invoke)

    static member Empty = FSharp.ConnectionPoolArgs.empty

type Step =

    static member Create<'TConnection,'TFeedItem>
        (name: string,
         connectionPoolArgs: IConnectionPoolArgs<'TConnection>,
         feed: IFeed<'TFeedItem>,
         execute: Func<IStepContext<'TConnection,'TFeedItem>,Task<Response>>,
         [<Optional;DefaultParameterValue(Constants.DefaultRepeatCount:int)>]repeatCount: int,
         [<Optional;DefaultParameterValue(Constants.DefaultDoNotTrack:bool)>]doNotTrack: bool) =

        FSharp.Step.create(name, connectionPoolArgs, feed, execute.Invoke, repeatCount, doNotTrack)

    static member Create<'TConnection>
        (name: string,
         connectionPoolArgs: IConnectionPoolArgs<'TConnection>,
         execute: Func<IStepContext<'TConnection,unit>,Task<Response>>,
         [<Optional;DefaultParameterValue(Constants.DefaultRepeatCount:int)>]repeatCount: int,
         [<Optional;DefaultParameterValue(Constants.DefaultDoNotTrack:bool)>]doNotTrack: bool) =

        Step.Create(name, connectionPoolArgs, Feed.empty, execute, repeatCount, doNotTrack)

    static member Create<'TFeedItem>
        (name: string,
         feed: IFeed<'TFeedItem>,
         execute: Func<IStepContext<unit,'TFeedItem>,Task<Response>>,
         [<Optional;DefaultParameterValue(Constants.DefaultRepeatCount:int)>]repeatCount: int,
         [<Optional;DefaultParameterValue(Constants.DefaultDoNotTrack:bool)>]doNotTrack: bool) =

        Step.Create(name, ConnectionPoolArgs.Empty, feed, execute, repeatCount, doNotTrack)

    static member Create(name: string,
                         execute: Func<IStepContext<unit,unit>,Task<Response>>,
                         [<Optional;DefaultParameterValue(Constants.DefaultRepeatCount:int)>]repeatCount: int,
                         [<Optional;DefaultParameterValue(Constants.DefaultDoNotTrack:bool)>]doNotTrack: bool) =

        Step.Create(name, ConnectionPoolArgs.Empty, Feed.empty, execute, repeatCount, doNotTrack)

    static member CreatePause(duration: TimeSpan) =
        FSharp.Step.createPause(duration)

[<Extension>]
type ScenarioBuilder =

    /// Creates scenario with steps which will be executed sequentially.
    static member CreateScenario(name: string, steps: IStep[]) =
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
    static member WithLoadSimulations (scenario: Scenario, loadSimulations: LoadSimulation[]) =
        scenario |> FSharp.Scenario.withLoadSimulations(Seq.toList loadSimulations)

[<Extension>]
type NBomberRunner =

    /// Registers scenarios in NBomber environment. Scenarios will be run in parallel.
    static member RegisterScenarios(scenarios: Contracts.Scenario[]) =
        scenarios |> Seq.toList |> FSharp.NBomberRunner.registerScenarios

    [<Extension>]
    static member LoadConfig(context: NBomberContext, path: string) =
        context |> FSharp.NBomberRunner.loadConfig(path)

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
    static member WithTestSuite(context: NBomberContext, testSuite: string) =
        context |> FSharp.NBomberRunner.withTestSuite(testSuite)

    [<Extension>]
    static member WithTestName(context: NBomberContext, testName: string) =
        context |> FSharp.NBomberRunner.withTestName(testName)

    [<Extension>]
    static member WithReportingSinks(context: NBomberContext, reportingSinks: IReportingSink[], sendStatsInterval: TimeSpan) =
        let sinks = reportingSinks |> Seq.toList
        context |> FSharp.NBomberRunner.withReportingSinks(sinks, sendStatsInterval)

    [<Extension>]
    static member WithPlugins(context: NBomberContext, plugins: IPlugin[]) =
        let pluginsList = plugins |> Seq.toList
        context |> FSharp.NBomberRunner.withPlugins(pluginsList)

    [<Extension>]
    static member Run(context: NBomberContext, args: string[]) =
        FSharp.NBomberRunner.run args context

    [<Extension>]
    static member RunInConsole(context: NBomberContext, args: string[]) =
        FSharp.NBomberRunner.runInConsole args context

    [<Extension>]
    static member RunTest(context: NBomberContext, args: string[]) =
        match FSharp.NBomberRunner.runTest args context with
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
