namespace NBomber.CSharp

#nowarn "3211"

open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

open NBomber
open NBomber.Contracts
open NBomber.Configuration

type ConnectionPool =
    static member Create<'TConnection>(name: string, openConnection: Func<'TConnection>, [<Optional;DefaultParameterValue(null:obj)>] closeConnection: Action<'TConnection>, [<Optional;DefaultParameterValue(0:int)>] connectionsCount: int) =
        let close = if isNull closeConnection then (new Action<'TConnection>(ignore))
                    else closeConnection

        if connectionsCount = 0 then
            FSharp.ConnectionPool.create(name, openConnection.Invoke, close.Invoke)
        else
            FSharp.ConnectionPool.create(name, openConnection.Invoke, close.Invoke, connectionsCount)

    static member None = FSharp.ConnectionPool.none

type Step =
    static member Create(name: string,
                         execute: Func<StepContext<'TConnection>,Task<Response>>,
                         connectionPool: IConnectionPool<'TConnection>,
                         [<Optional;DefaultParameterValue(Domain.Constants.DefaultRepeatCount:int)>]repeatCount: int,
                         [<Optional;DefaultParameterValue(Domain.Constants.DefaultDoNotTrack:bool)>]doNotTrack: bool) =
        FSharp.Step.create(name, connectionPool, execute.Invoke, repeatCount, doNotTrack)

    static member Create(name: string,
                         execute: Func<StepContext<unit>,Task<Response>>,
                         [<Optional;DefaultParameterValue(Domain.Constants.DefaultRepeatCount:int)>]repeatCount: int,
                         [<Optional;DefaultParameterValue(Domain.Constants.DefaultDoNotTrack:bool)>]doNotTrack: bool) =
        Step.Create(name, execute, ConnectionPool.None, repeatCount, doNotTrack)

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

//    [<Extension>]
//    static member WithConcurrentCopies(scenario: Scenario, concurrentCopies: int) =
//        scenario |> FSharp.Scenario.withConcurrentCopies(concurrentCopies)

    [<Extension>]
    static member WithWarmUpDuration(scenario: Scenario, duration: TimeSpan) =
        scenario |> FSharp.Scenario.withWarmUpDuration(duration)

    [<Extension>]
    static member WithOutWarmUp(scenario: Scenario) =
        scenario |> FSharp.Scenario.withOutWarmUp

//    [<Extension>]
//    static member WithDuration(scenario: Scenario, duration: TimeSpan) =
//        scenario |> FSharp.Scenario.withDuration(duration)

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
