namespace NBomber.FSharp

open System
open System.IO
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.Extensions.Configuration

open NBomber
open NBomber.Contracts
open NBomber.Configuration
open NBomber.Domain
open NBomber.Domain.ConnectionPool
open NBomber.Errors
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices

type ConnectionPool =

    static member create(name: string,
                         connectionCount: int,
                         openConnection: int -> 'TConnection,
                         ?closeConnection: 'TConnection -> unit) =
        { PoolName = name
          OpenConnection = openConnection
          CloseConnection = closeConnection
          ConnectionCount = connectionCount }

    static member empty =
        ConnectionPool.create(Constants.EmptyPoolName, connectionCount = 0, openConnection = ignore)

type Step =

    static member private getRepeatCount (repeatCount: int option) =
        match defaultArg repeatCount Constants.DefaultRepeatCount with
        | x when x <= 0 -> 1
        | x             -> x

    static member create (name: string,
                          connectionPool: ConnectionPoolArgs<'TConnection>,
                          feed: IFeed<'TFeedItem>,
                          execute: StepContext<'TConnection,'TFeedItem> -> Task<Response>,
                          ?repeatCount: int, ?doNotTrack: bool) =
        { StepName = name
          ConnectionPoolArgs = ConnectionPoolArgs.toUntyped(connectionPool)
          ConnectionPool = None
          Execute = Step.toUntypedExec(execute)
          Context = None
          Feed = Feed.toUntypedFeed(feed)
          RepeatCount = Step.getRepeatCount(repeatCount)
          DoNotTrack = defaultArg doNotTrack Constants.DefaultDoNotTrack }
          :> IStep

    static member create (name: string,
                          connectionPool: ConnectionPoolArgs<'TConnection>,
                          execute: StepContext<'TConnection,unit> -> Task<Response>,
                          ?repeatCount: int,
                          ?doNotTrack: bool) =

        Step.create(name, connectionPool, Feed.empty, execute,
                    Step.getRepeatCount(repeatCount),
                    defaultArg doNotTrack Constants.DefaultDoNotTrack)

    static member create (name: string,
                          feed: IFeed<'TFeedItem>,
                          execute: StepContext<unit,'TFeedItem> -> Task<Response>,
                          ?repeatCount: int,
                          ?doNotTrack: bool) =

        Step.create(name, ConnectionPool.empty, feed, execute,
                    Step.getRepeatCount(repeatCount),
                    defaultArg doNotTrack Constants.DefaultDoNotTrack)

    static member create (name: string,
                          execute: StepContext<unit,unit> -> Task<Response>,
                          ?repeatCount: int,
                          ?doNotTrack: bool) =

        Step.create(name, ConnectionPool.empty, Feed.empty, execute,
                    Step.getRepeatCount(repeatCount),
                    defaultArg doNotTrack Constants.DefaultDoNotTrack)

    static member createPause (duration: TimeSpan) =
        Step.create(name = sprintf "pause %A" duration,
                    execute = (fun _ -> task { do! Task.Delay(duration)
                                               return Response.Ok() }),
                    doNotTrack = true)

module Scenario =

    /// Creates scenario with steps which will be executed sequentially.
    let create (name: string) (steps: IStep list): Contracts.Scenario =
        { ScenarioName = name
          TestInit = Unchecked.defaultof<_>
          TestClean = Unchecked.defaultof<_>
          Steps = steps
          WarmUpDuration = TimeSpan.FromSeconds(Constants.DefaultWarmUpDurationInSec)
          LoadSimulations = [
            LoadSimulation.InjectScenariosPerSec(
                copiesCount = Constants.DefaultConcurrentCopiesCount,
                during = TimeSpan.FromSeconds Constants.DefaultScenarioDurationInSec
            )
          ] }

    let withTestInit (initFunc: ScenarioContext -> Task<unit>) (scenario: Contracts.Scenario) =
        { scenario with TestInit = Some(fun token -> initFunc(token) :> Task) }

    let withTestClean (cleanFunc: ScenarioContext -> Task<unit>) (scenario: Contracts.Scenario) =
        { scenario with TestClean = Some(fun token -> cleanFunc(token) :> Task) }

    let withWarmUpDuration (duration: TimeSpan) (scenario: Contracts.Scenario) =
        { scenario with WarmUpDuration = duration }

    let withOutWarmUp (scenario: Contracts.Scenario) =
        { scenario with WarmUpDuration = TimeSpan.Zero }

    let withLoadSimulations (loadSimulations: LoadSimulation list) (scenario: Contracts.Scenario) =
        { scenario with LoadSimulations = loadSimulations }

module NBomberRunner =

    /// Registers scenarios in NBomber environment. Scenarios will be run in parallel.
    let registerScenarios (scenarios: Contracts.Scenario list) =
        { TestContext.empty with RegisteredScenarios = scenarios }

    let withReportFileName (reportFileName: string) (context: TestContext) =
        { context with ReportFileName = Some reportFileName }

    let withReportFormats (reportFormats: ReportFormat list) (context: TestContext) =
        { context with ReportFormats = reportFormats }

    let withTestSuite (testSuite: string) (context: TestContext) =
        { context with TestSuite = testSuite }

    let withTestName (testName: string) (context: TestContext) =
        { context with TestName = testName }

    let loadTestConfig (path: string) (context: TestContext) =
        let config = path |> File.ReadAllText |> TestConfig.unsafeParse
        { context with TestConfig = Some config }

    let loadInfraConfig (path: string) (context: TestContext) =
        let config = ConfigurationBuilder().AddJsonFile(path).Build() :> IConfiguration
        { context with InfraConfig = Some config }

    let withReportingSinks (reportingSinks: IReportingSink list, sendStatsInterval: TimeSpan) (context: TestContext) =
        { context with ReportingSinks = reportingSinks
                       SendStatsInterval = sendStatsInterval }

    let run (context: TestContext) =
        NBomberRunner.runAs(Process, context)
        |> ignore

    let rec runInConsole (context: TestContext) =
        NBomberRunner.runAs(Console, context)
        |> ignore
        Serilog.Log.Information("Repeat the same test one more time? (y/n)")

        let userInput = Console.ReadLine()
        let repeat = Seq.contains userInput ["y"; "Y"; "yes"; "Yes"]
        if repeat then runInConsole context

    let runTest (context: TestContext) =
        NBomberRunner.runAs(Test, context)
        |> Result.map(fun x -> x.Statistics)
        |> Result.mapError(AppError.toString)

    let internal runWithResult (context: TestContext) =
        NBomberRunner.runAs(Process, context)
