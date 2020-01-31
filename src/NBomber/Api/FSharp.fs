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
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices
open NBomber.Errors

type ConnectionPool =

    static member create<'TConnection>(name: string,
                                       openConnection: unit -> 'TConnection,
                                       ?closeConnection: 'TConnection -> unit,
                                       ?connectionsCount: int) =
        { PoolName = name
          OpenConnection = openConnection
          CloseConnection = closeConnection
          ConnectionsCount = connectionsCount
          AliveConnections = Array.empty }
          :> IConnectionPool<'TConnection>

    static member none =
        { PoolName = Constants.EmptyPoolName
          OpenConnection = ignore
          CloseConnection = None
          ConnectionsCount = None
          AliveConnections = Array.empty }
          :> IConnectionPool<unit>

    static member internal internalNone<'TConnection> () =
        { PoolName = Constants.EmptyPoolName
          OpenConnection = fun _ -> Unchecked.defaultof<'TConnection>
          CloseConnection = None
          ConnectionsCount = None
          AliveConnections = Array.empty }
          :> IConnectionPool<'TConnection>

type Step =

    static member private getRepeatCount (repeatCount: int option) =
        match defaultArg repeatCount Constants.DefaultRepeatCount with
        | x when x <= 0 -> 1
        | x             -> x

    static member create (name: string,
                          connectionPool: IConnectionPool<'TConnection>,
                          execute: StepContext<'TConnection> -> Task<Response>,
                          ?repeatCount: int,
                          ?doNotTrack: bool) =

        let p = connectionPool :?> ConnectionPool<'TConnection>

        let newOpen = fun () -> p.OpenConnection() :> obj

        let newClose =
            match p.CloseConnection with
            | Some func -> Some <| fun (c: obj) -> c :?> 'TConnection |> func
            | None      -> None

        let newPool = { PoolName = p.PoolName
                        OpenConnection = newOpen
                        CloseConnection = newClose
                        ConnectionsCount = p.ConnectionsCount
                        AliveConnections = Array.empty }

        let newExecute =
            fun (context: StepContext<obj>) ->
                let newContext = { CorrelationId = context.CorrelationId
                                   CancellationToken = context.CancellationToken
                                   Connection = context.Connection :?> 'TConnection
                                   Data = context.Data
                                   Logger = context.Logger }
                execute(newContext)

        { StepName = name
          ConnectionPool = newPool
          Execute = newExecute
          CurrentContext = None
          RepeatCount = Step.getRepeatCount(repeatCount)
          DoNotTrack = defaultArg doNotTrack Constants.DefaultDoNotTrack }
          :> IStep

    static member create (name: string,
                          execute: StepContext<'TConnection> -> Task<Response>,
                          ?repeatCount: int,
                          ?doNotTrack: bool) =

        Step.create(name,
                    ConnectionPool.internalNone<'TConnection>(),
                    execute,
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
          Feed = Feed.empty "empty"
          Steps = Seq.toArray(steps)
          ConcurrentCopies = Constants.DefaultConcurrentCopies
          WarmUpDuration = TimeSpan.FromSeconds(Constants.DefaultWarmUpDurationInSec)
          Duration = TimeSpan.FromSeconds(Constants.DefaultScenarioDurationInSec) }


    let withFeed(feed : IFeed<'a>) (scenario : Contracts.Scenario) =
       { scenario with Feed = feed |> Feed.map box  }

    let withTestInit (initFunc: ScenarioContext -> Task<unit>) (scenario: Contracts.Scenario) =
        { scenario with TestInit = Some(fun token -> initFunc(token) :> Task) }

    let withTestClean (cleanFunc: ScenarioContext -> Task<unit>) (scenario: Contracts.Scenario) =
        { scenario with TestClean = Some(fun token -> cleanFunc(token) :> Task) }

    let withConcurrentCopies (concurrentCopies: int) (scenario: Contracts.Scenario) =
        { scenario with ConcurrentCopies = concurrentCopies }

    let withWarmUpDuration (duration: TimeSpan) (scenario: Contracts.Scenario) =
        { scenario with WarmUpDuration = duration }

    let withOutWarmUp (scenario: Contracts.Scenario) =
        { scenario with WarmUpDuration = TimeSpan.Zero }

    let withDuration (duration: TimeSpan) (scenario: Contracts.Scenario) =
        { scenario with Duration = duration }

module NBomberRunner =

    /// Registers scenarios in NBomber environment. Scenarios will be run in parallel.
    let registerScenarios (scenarios: Contracts.Scenario list) =
        { TestContext.empty with RegisteredScenarios = scenarios |> Seq.toArray  }

    let withReportFileName (reportFileName: string) (context: TestContext) =
        { context with ReportFileName = Some reportFileName }

    let withReportFormats (reportFormats: ReportFormat list) (context: TestContext) =
        { context with ReportFormats = Seq.toArray reportFormats }

    let withTestSuite (testSuite: string) (context: TestContext) =
        { context with TestSuite = testSuite }

    let withTestName (testName: string) (context: TestContext) =
        { context with TestName = testName }

    let loadTestConfig (path: string) (context: TestContext) =
        let config = path |> File.ReadAllText |> TestConfig.parse
        { context with TestConfig = Some config }

    let loadInfraConfig (path: string) (context: TestContext) =
        let config = ConfigurationBuilder().AddJsonFile(path).Build() :> IConfiguration
        { context with InfraConfig = Some config }

    let withReportingSinks (reportingSinks: IReportingSink list, sendStatsInterval: TimeSpan) (context: TestContext) =
        { context with ReportingSinks = Seq.toArray reportingSinks
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
