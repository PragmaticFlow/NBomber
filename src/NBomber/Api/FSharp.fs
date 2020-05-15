namespace NBomber.FSharp

open System
open System.IO
open System.Threading
open System.Threading.Tasks

open CommandLine
open FsToolkit.ErrorHandling
open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Configuration.Yaml
open ShellProgressBar

open NBomber
open NBomber.Contracts
open NBomber.Configuration
open NBomber.Configuration.Yaml
open NBomber.Errors
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.ConnectionPool
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

type CommandLineArgs = {
    [<Option('c', "config", HelpText = "NBomber configuration")>] Config: string
    [<Option('i', "infra", HelpText = "NBomber infra configuration")>] InfraConfig: string
}

module Scenario =

    /// Creates scenario with steps which will be executed sequentially.
    let create (name: string) (steps: IStep list): Contracts.Scenario =
        { ScenarioName = name
          TestInit = Unchecked.defaultof<_>
          TestClean = Unchecked.defaultof<_>
          Steps = steps
          WarmUpDuration = Constants.DefaultWarmUpDuration
          LoadSimulations = [
            LoadSimulation.InjectScenariosPerSec(
                copiesCount = Constants.DefaultConcurrentCopiesCount,
                during = Constants.DefaultScenarioDuration
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
        { NBomberContext.empty with RegisteredScenarios = scenarios }

    /// Specifies report file names.
    let withReportFileName (reportFileName: string) (context: NBomberContext) =
        { context with ReportFileName = Some reportFileName }

    /// Specifies report formats to be generated.
    let withReportFormats (reportFormats: ReportFormat list) (context: NBomberContext) =
        { context with ReportFormats = reportFormats }

    /// Specifies test sute.
    let withTestSuite (testSuite: string) (context: NBomberContext) =
        { context with TestSuite = testSuite }

    /// Specifies test name.
    let withTestName (testName: string) (context: NBomberContext) =
        { context with TestName = testName }

    /// Loads configuration in json (by default) or yaml (.yml, .yaml) format.
    let loadConfig (path: string) (context: NBomberContext) =
        if String.IsNullOrWhiteSpace(path) then
            context
        else
            let config =
                match Path.GetExtension(path) with
                | ".yml"
                | ".yaml" -> path |> File.ReadAllText |> YamlConfig.unsafeParse
                | _       -> path |> File.ReadAllText |> JsonConfig.unsafeParse

            { context with NBomberConfig = Some config }

    /// Loads infrastructure configuration in json (default) or yaml (.yml, .yaml) format.
    let loadInfraConfig (path: string) (context: NBomberContext) =
        if String.IsNullOrWhiteSpace(path) then
            context
        else
            let config =
                match Path.GetExtension(path) with
                | ".yml"
                | ".yaml" -> ConfigurationBuilder().AddYamlFile(path).Build() :> IConfiguration
                | _       -> ConfigurationBuilder().AddJsonFile(path).Build() :> IConfiguration

            { context with InfraConfig = Some config }

    /// Specifies reporting sinks.
    let withReportingSinks (reportingSinks: IReportingSink list, sendStatsInterval: TimeSpan) (context: NBomberContext) =
        { context with ReportingSinks = reportingSinks
                       SendStatsInterval = sendStatsInterval }

    /// Specifies plugins.
    let withPlugins (plugins: IPlugin list) (context: NBomberContext) =
        { context with Plugins = plugins }

    /// Executes CLI commands.
    /// The following commands are supported:
    /// -c or --config: loads configuration
    /// -i or --infra: loads infrastructure configuration
    /// Order is important: it overrides configurations set by loadConfig, loadInfraConfig if it placed below them and vice-versa.
    /// Examples of possible args:
    /// [|"-c"; "config.yaml"; "-i"; "infra_config.yaml"|]
    /// [|"--config"; "config.yaml"; "--infra"; "infra_config.yaml"|]
    let internal executeCliArgs (args) (context: NBomberContext) =
        match CommandLine.Parser.Default.ParseArguments<CommandLineArgs>(args) with
        | :? Parsed<CommandLineArgs> as parsed ->
            let values = parsed.Value
            let execLoadConfigCmd = loadConfig values.Config
            let execLoadInfraConfigCmd = loadInfraConfig values.InfraConfig
            let execCmd = execLoadConfigCmd >> execLoadInfraConfigCmd

            context |> execCmd

        | _ -> context

    let internal getApplicationType() =
        try
            new ProgressBar(0, String.Empty) |> ignore
            Console
        with
        | _ -> Process

    /// Runs scenarios for given context.
    let run (context: NBomberContext) =
        let applicationType = getApplicationType()

        context
        |> NBomberRunner.run(applicationType)
        |> Result.mapError(AppError.toString)

    /// Runs scenarios for given arguments and context.
    let runWithArgs (args) (context: NBomberContext) =
        let applicationType = getApplicationType()

        context
        |> executeCliArgs args
        |> NBomberRunner.run(applicationType)
        |> Result.mapError(AppError.toString)

    let internal runWithResult (args) (context: NBomberContext) =
        context
        |> executeCliArgs args
        |> NBomberRunner.run(Process)
