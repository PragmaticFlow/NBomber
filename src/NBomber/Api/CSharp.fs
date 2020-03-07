namespace NBomber.CSharp

#nowarn "3211"

open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

open NBomber
open NBomber.Domain
open NBomber.Contracts
open NBomber.Configuration

type ConnectionPool =

    static member Create<'TConnection>(name: string,
                                       connectionsCount: int,
                                       openConnection: Func<int,'TConnection>,
                                       [<Optional;DefaultParameterValue(null:obj)>] closeConnection: Action<'TConnection>) =

        let close = if isNull closeConnection then (Action<'TConnection>(ignore))
                    else closeConnection

        FSharp.ConnectionPool.create(name, connectionsCount, openConnection.Invoke, close.Invoke)


    static member Empty = FSharp.ConnectionPool.empty

type Step =

    static member Create<'TConnection,'TFeedItem>
        (name: string,
         connectionPool: IConnectionPool<'TConnection>,
         feed: IFeed<'TFeedItem>,
         execute: Func<StepContext<'TConnection,'TFeedItem>,Task<Response>>,
         [<Optional;DefaultParameterValue(Constants.DefaultRepeatCount:int)>]repeatCount: int,
         [<Optional;DefaultParameterValue(Constants.DefaultDoNotTrack:bool)>]doNotTrack: bool) =

        FSharp.Step.create(name, connectionPool, feed, execute.Invoke, repeatCount, doNotTrack)

    static member Create<'TConnection>
        (name: string,
         connectionPool: IConnectionPool<'TConnection>,
         execute: Func<StepContext<'TConnection,unit>,Task<Response>>,
         [<Optional;DefaultParameterValue(Constants.DefaultRepeatCount:int)>]repeatCount: int,
         [<Optional;DefaultParameterValue(Constants.DefaultDoNotTrack:bool)>]doNotTrack: bool) =

        Step.Create(name, connectionPool, Feed.empty, execute, repeatCount, doNotTrack)

    static member Create<'TFeedItem>
        (name: string,
         feed: IFeed<'TFeedItem>,
         execute: Func<StepContext<unit,'TFeedItem>,Task<Response>>,
         [<Optional;DefaultParameterValue(Constants.DefaultRepeatCount:int)>]repeatCount: int,
         [<Optional;DefaultParameterValue(Constants.DefaultDoNotTrack:bool)>]doNotTrack: bool) =

        Step.Create(name, ConnectionPool.Empty, feed, execute, repeatCount, doNotTrack)

    static member Create(name: string,
                         execute: Func<StepContext<unit,unit>,Task<Response>>,
                         [<Optional;DefaultParameterValue(Constants.DefaultRepeatCount:int)>]repeatCount: int,
                         [<Optional;DefaultParameterValue(Constants.DefaultDoNotTrack:bool)>]doNotTrack: bool) =

        Step.Create(name, ConnectionPool.Empty, Feed.empty, execute, repeatCount, doNotTrack)

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
    static member WithOutWarmUp(scenario: Scenario) =
        scenario |> FSharp.Scenario.withOutWarmUp

    [<Extension>]
    static member WithLoadSimulations (scenario: Scenario, loadSimulations: LoadSimulation[]) =
        scenario |> FSharp.Scenario.withLoadSimulations(Seq.toList loadSimulations)

[<Extension>]
type NBomberRunner =

    /// Registers scenarios in NBomber environment. Scenarios will be run in parallel.
    static member RegisterScenarios([<System.ParamArray>]scenarios: Contracts.Scenario[]) =
        scenarios |> Seq.toList |> FSharp.NBomberRunner.registerScenarios

    [<Extension>]
    static member LoadTestConfig(context: TestContext, path: string) =
        context |> FSharp.NBomberRunner.loadTestConfig(path)

    [<Extension>]
    static member LoadInfraConfig(context: TestContext, path: string) =
        context |> FSharp.NBomberRunner.loadInfraConfig(path)

    [<Extension>]
    static member WithReportFileName(context: TestContext, reportFileName: string) =
        context |> FSharp.NBomberRunner.withReportFileName(reportFileName)

    [<Extension>]
    static member WithReportFormats(context: TestContext, [<System.ParamArray>]reportFormats: ReportFormat[]) =
        let formats = reportFormats |> Seq.toList
        context |> FSharp.NBomberRunner.withReportFormats(formats)

    [<Extension>]
    static member WithTestSuite(context: TestContext, testSuite: string) =
        context |> FSharp.NBomberRunner.withTestSuite(testSuite)

    [<Extension>]
    static member WithTestName(context: TestContext, testName: string) =
        context |> FSharp.NBomberRunner.withTestName(testName)

    [<Extension>]
    static member WithReportingSinks(context: TestContext, reportingSinks: IReportingSink[], sendStatsInterval: TimeSpan) =
        let sinks = reportingSinks |> Seq.toList
        context |> FSharp.NBomberRunner.withReportingSinks(sinks, sendStatsInterval)

    [<Extension>]
    static member Run(context: TestContext) =
        FSharp.NBomberRunner.run(context)

    [<Extension>]
    static member RunInConsole(context: TestContext) =
        FSharp.NBomberRunner.runInConsole(context)

    [<Extension>]
    static member RunTest(context: TestContext) =
        match FSharp.NBomberRunner.runTest(context) with
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
