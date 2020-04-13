namespace NBomber.FSharp

open System
open System.IO
open System.Threading
open System.Threading.Tasks

open FsToolkit.ErrorHandling
open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Configuration.Yaml

open NBomber
open NBomber.Contracts
open NBomber.Configuration
open NBomber.Configuration.Yaml
open NBomber.Errors
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.ConnectionPool
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices

type ConnectionPoolArgs =

    static member create (name: string,
                          getConnectionCount: unit -> int,
                          openConnection: int * CancellationToken -> Task<'TConnection>,
                          closeConnection: 'TConnection * CancellationToken -> Task) =

        { new IConnectionPoolArgs<'TConnection> with
            member x.PoolName = name
            member x.GetConnectionCount() = getConnectionCount()
            member x.OpenConnection(number,token) = openConnection(number,token)
            member x.CloseConnection(connection,token) = closeConnection(connection,token) }

    static member create (name: string,
                          getConnectionCount: unit -> int,
                          openConnection: int * CancellationToken -> Task<'TConnection>,
                          closeConnection: 'TConnection * CancellationToken -> Task<unit>) =

        let close = fun (connection,token) -> closeConnection(connection,token) :> Task
        ConnectionPoolArgs.create(name, getConnectionCount, openConnection, close)

    static member empty =
        ConnectionPoolArgs.create(Constants.EmptyPoolName, (fun _ -> 0), (fun _ -> Task.FromResult()), fun _ -> Task.FromResult())

type Step =

    static member private getRepeatCount (repeatCount: int option) =
        match defaultArg repeatCount Constants.DefaultRepeatCount with
        | x when x <= 0 -> 1
        | x             -> x

    static member create (name: string,
                          connectionPoolArgs: IConnectionPoolArgs<'TConnection>,
                          feed: IFeed<'TFeedItem>,
                          execute: IStepContext<'TConnection,'TFeedItem> -> Task<Response>,
                          ?repeatCount: int, ?doNotTrack: bool) =
        { StepName = name
          ConnectionPoolArgs = ConnectionPoolArgs.toUntyped(connectionPoolArgs)
          ConnectionPool = None
          Execute = Step.toUntypedExec(execute)
          Context = None
          Feed = Feed.toUntypedFeed(feed)
          RepeatCount = Step.getRepeatCount(repeatCount)
          DoNotTrack = defaultArg doNotTrack Constants.DefaultDoNotTrack }
          :> IStep

    static member create (name: string,
                          connectionPoolArgs: IConnectionPoolArgs<'TConnection>,
                          execute: IStepContext<'TConnection,unit> -> Task<Response>,
                          ?repeatCount: int,
                          ?doNotTrack: bool) =

        Step.create(name, connectionPoolArgs, Feed.empty, execute,
                    Step.getRepeatCount(repeatCount),
                    defaultArg doNotTrack Constants.DefaultDoNotTrack)

    static member create (name: string,
                          feed: IFeed<'TFeedItem>,
                          execute: IStepContext<unit,'TFeedItem> -> Task<Response>,
                          ?repeatCount: int,
                          ?doNotTrack: bool) =

        Step.create(name, ConnectionPoolArgs.empty, feed, execute,
                    Step.getRepeatCount(repeatCount),
                    defaultArg doNotTrack Constants.DefaultDoNotTrack)

    static member create (name: string,
                          execute: IStepContext<unit,unit> -> Task<Response>,
                          ?repeatCount: int,
                          ?doNotTrack: bool) =

        Step.create(name, ConnectionPoolArgs.empty, Feed.empty, execute,
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

    let withoutWarmUp (scenario: Contracts.Scenario) =
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

    let loadConfigJson (path: string) (context: TestContext) =
        let config = path |> File.ReadAllText |> JsonConfig.unsafeParse
        { context with NBomberConfig = Some config }

    let loadConfigYaml (path: string) (context: TestContext) =
        let config = path |> File.ReadAllText |> YamlConfig.unsafeParse
        { context with NBomberConfig = Some config }

    let loadInfraConfigJson (path: string) (context: TestContext) =
        let config = ConfigurationBuilder().AddJsonFile(path).Build() :> IConfiguration
        { context with InfraConfig = Some config }

    let loadInfraConfigYaml (path: string) (context: TestContext) =
        let config = ConfigurationBuilder().AddYamlFile(path).Build() :> IConfiguration
        { context with InfraConfig = Some config }

    let withReportingSinks (reportingSinks: IReportingSink list, sendStatsInterval: TimeSpan) (context: TestContext) =
        { context with ReportingSinks = reportingSinks
                       SendStatsInterval = sendStatsInterval }

    let withPlugins (plugins: IPlugin list) (context: TestContext) =
        { context with Plugins = plugins }

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
        |> Result.mapError(AppError.toString)

    let internal runWithResult (context: TestContext) =
        NBomberRunner.runAs(Process, context)
