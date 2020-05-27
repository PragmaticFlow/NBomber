namespace NBomber.FSharp

open System
open System.IO
open System.Threading
open System.Threading.Tasks

open CommandLine
open FSharp.Control.Tasks.V2.ContextInsensitive
open FsToolkit.ErrorHandling
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
open NBomber.DomainServices

type CommandLineArgs = {
    [<Option('c', "config", HelpText = "NBomber configuration")>] Config: string
    [<Option('i', "infra", HelpText = "NBomber infra configuration")>] InfraConfig: string
}

type ConnectionPoolArgs =

    static member create (name: string,
                          openConnection: int * CancellationToken -> Task<'TConnection>,
                          closeConnection: 'TConnection * CancellationToken -> Task,
                          ?connectionCount: int) =

        let count = defaultArg connectionCount Constants.DefaultConnectionCount
        ConnectionPoolArgs(name, count, openConnection, closeConnection)
        :> IConnectionPoolArgs<'TConnection>

    static member create (name: string,
                          openConnection: int * CancellationToken -> Task<'TConnection>,
                          closeConnection: 'TConnection * CancellationToken -> Task<unit>,
                          ?connectionCount: int) =

        let close = fun (connection,token) -> closeConnection(connection,token) :> Task
        let count = defaultArg connectionCount Constants.DefaultConnectionCount
        ConnectionPoolArgs.create(name, openConnection, close, count)

    static member empty =
        ConnectionPoolArgs.create(Constants.EmptyPoolName, (fun _ -> Task.singleton()), (fun _ -> Task.singleton()), 0)

type Step =

    static member create (name: string,
                          connectionPoolArgs: IConnectionPoolArgs<'TConnection>,
                          feed: IFeed<'TFeedItem>,
                          execute: IStepContext<'TConnection,'TFeedItem> -> Task<Response>,
                          ?doNotTrack: bool) =

        let poolArgs = if connectionPoolArgs.PoolName = Constants.EmptyPoolName then None
                       else Some((connectionPoolArgs :?> ConnectionPoolArgs<'TConnection>).GetUntyped().Value)

        { StepName = name
          ConnectionPoolArgs = poolArgs
          ConnectionPool = None
          Execute = Step.toUntypedExec(execute)
          Context = None
          Feed = Feed.toUntypedFeed(feed)
          DoNotTrack = defaultArg doNotTrack Constants.DefaultDoNotTrack }
          :> IStep

    static member create (name: string,
                          connectionPoolArgs: IConnectionPoolArgs<'TConnection>,
                          execute: IStepContext<'TConnection,unit> -> Task<Response>,
                          ?doNotTrack: bool) =

        Step.create(name, connectionPoolArgs, Feed.empty, execute,
                    defaultArg doNotTrack Constants.DefaultDoNotTrack)

    static member create (name: string,
                          feed: IFeed<'TFeedItem>,
                          execute: IStepContext<unit,'TFeedItem> -> Task<Response>,
                          ?doNotTrack: bool) =

        Step.create(name, ConnectionPoolArgs.empty, feed, execute,
                    defaultArg doNotTrack Constants.DefaultDoNotTrack)

    static member create (name: string,
                          execute: IStepContext<unit,unit> -> Task<Response>,
                          ?doNotTrack: bool) =

        Step.create(name, ConnectionPoolArgs.empty, Feed.empty, execute,
                    defaultArg doNotTrack Constants.DefaultDoNotTrack)

    /// Creates pause step with specified duration.
    static member createPause (duration: TimeSpan) =
        Step.create(name = sprintf "pause %A" duration,
                    execute = (fun _ -> task { do! Task.Delay(duration)
                                               return Response.Ok() }),
                    doNotTrack = true)

    /// Creates pause step with specified duration in milliseconds.
    static member createPause (milliseconds: int) =
        Step.createPause(TimeSpan.FromMilliseconds(float milliseconds))

    /// Creates pause step with specified duration in lazy mode.
    /// It's useful when you want to fetch value from some configuration.
    static member createPause (getValue: unit -> TimeSpan) =
        getValue() |> Step.createPause

    /// Creates pause step in milliseconds in lazy mode.
    /// It's useful when you want to fetch value from some configuration.
    static member createPause (getValue: unit -> int) =
        getValue() |> float |> TimeSpan.FromMilliseconds |> Step.createPause

module Scenario =

    /// Creates scenario with steps which will be executed sequentially.
    let create (name: string) (steps: IStep list): Contracts.Scenario = {
          ScenarioName = name
          TestInit = None
          TestClean = None
          Steps = steps
          WarmUpDuration = Constants.DefaultWarmUpDuration
          LoadSimulations = [
            LoadSimulation.InjectScenariosPerSec(copiesCount = Constants.DefaultConcurrentCopiesCount,
                                                 during = Constants.DefaultScenarioDuration)
          ]
    }

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

    let withReportFileName (reportFileName: string) (context: NBomberContext) =
        let report =
            context.Report
            |> Option.defaultValue ReporterConfig.Default
        { context with
            Report = Some { report with FileName = Some reportFileName} }

    let withReportFormats (reportFormats: ReportFormat list) (context: NBomberContext) =
        let report =
            context.Report
            |> Option.defaultValue ReporterConfig.Default
        { context with
            Report = Some { report with Formats = reportFormats } }

    /// Sets context without reports
    let withoutReports (context: NBomberContext) =
        let report =
            context.Report
            |> Option.defaultValue ReporterConfig.Default
        { context with
            Report = Some { report with Formats = [] } }

    let withTestSuite (testSuite: string) (context: NBomberContext) =
        { context with TestSuite = testSuite }

    let withTestName (testName: string) (context: NBomberContext) =
        { context with TestName = testName }

    /// Loads configuration.
    /// The following formats are supported:
    /// - json (.json),
    /// - yaml (.yml, .yaml).
    let loadConfig (path: string) (context: NBomberContext) =
        let config =
            match Path.GetExtension(path) with
            | ".json" -> path |> File.ReadAllText |> JsonConfig.unsafeParse
            | ".yml"
            | ".yaml" -> path |> File.ReadAllText |> YamlConfig.unsafeParse
            | _       -> failwith "unsupported config format"

        { context with NBomberConfig = Some config }

    /// Loads infrastructure configuration.
    /// The following formats are supported:
    /// - json (.json),
    /// - yaml (.yml, .yaml).
    let loadInfraConfig (path: string) (context: NBomberContext) =
        let config =
            match Path.GetExtension(path) with
            | ".json" -> ConfigurationBuilder().AddJsonFile(path).Build() :> IConfiguration
            | ".yml"
            | ".yaml" -> ConfigurationBuilder().AddYamlFile(path).Build() :> IConfiguration
            | _       -> failwith "unsupported config format"

        { context with InfraConfig = Some config }

    let withReportingSinks (reportingSinks: IReportingSink list, sendStatsInterval: TimeSpan) (context: NBomberContext) =
        let report =
            context.Report
            |> Option.defaultValue ReporterConfig.Default
        { context with
            Report = Some {
                report with
                    Sinks = reportingSinks
                    SendStatsInterval = sendStatsInterval
            }}

    let withPlugins (plugins: IPlugin list) (context: NBomberContext) =
        { context with Plugins = plugins }

    /// Sets application type.
    /// The following application types are supported:
    /// - Console: is suitable for interactive session (will display progress bar)
    /// - Process: is suitable for running tests under test runners (progress bar will not be shown)
    /// By default NBomber will automatically identify your environment: Process or Console.
    let withApplicationType (applicationType: ApplicationType) (context: NBomberContext) =
        { context with ApplicationType = Some applicationType }

    let internal executeCliArgs (args) (context: NBomberContext) =
        let invokeConfigLoader (configName) (configLoader) (config) (context) =
            if config = String.Empty then sprintf "%s is empty" configName |> failwith
            elif String.IsNullOrEmpty(config) then context
            else configLoader config context

        match CommandLine.Parser.Default.ParseArguments<CommandLineArgs>(args) with
        | :? Parsed<CommandLineArgs> as parsed ->
            let values = parsed.Value
            let execLoadConfigCmd = invokeConfigLoader "config" loadConfig values.Config
            let execLoadInfraConfigCmd = invokeConfigLoader "infra config" loadInfraConfig values.InfraConfig
            let execCmd = execLoadConfigCmd >> execLoadInfraConfigCmd

            context |> execCmd

        | _ -> context

    let internal runWithResult (args) (context: NBomberContext) =
        context
        |> executeCliArgs args
        |> NBomberRunner.run

    let run (context: NBomberContext) =
        context
        |> runWithResult Array.empty
        |> Result.mapError(AppError.toString)

    /// Runs scenarios with arguments.
    /// The following CLI commands are supported:
    /// -c or --config: loads configuration,
    /// -i or --infra: loads infrastructure configuration.
    /// Examples of possible args:
    /// [|"-c"; "config.yaml"; "-i"; "infra_config.yaml"|]
    /// [|"--config"; "config.yaml"; "--infra"; "infra_config.yaml"|]
    let runWithArgs (args) (context: NBomberContext) =
        context
        |> runWithResult args
        |> Result.mapError(AppError.toString)
