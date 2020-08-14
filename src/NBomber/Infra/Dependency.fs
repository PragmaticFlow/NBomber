module internal NBomber.Infra.Dependency

open System
open System.IO
open System.Reflection
open System.Runtime.Versioning

open Microsoft.Extensions.Configuration
open Serilog
open Serilog.Events
open ShellProgressBar

open NBomber.Configuration
open NBomber.Contracts

type IProgressBarEnv =
    abstract CreateManualProgressBar: tickCount:int -> IProgressBar
    abstract CreateAutoProgressBar: duration:TimeSpan -> IProgressBar

type IGlobalDependency =
    inherit IDisposable
    abstract NBomberVersion: string
    abstract ApplicationType: ApplicationType
    abstract NodeType: NodeType
    abstract NBomberConfig: NBomberConfig option
    abstract InfraConfig: IConfiguration option
    abstract CreateLoggerConfig: (unit -> LoggerConfiguration) option
    abstract ProgressBarEnv: IProgressBarEnv
    abstract Logger: ILogger
    abstract ReportingSinks: IReportingSink list
    abstract WorkerPlugins: IWorkerPlugin list

module Logger =

    let create (testInfo: TestInfo)
               (createConfig: (unit -> LoggerConfiguration) option)
               (configPath: IConfiguration option) =

        let attachFileLogger (config: LoggerConfiguration) =
            config.WriteTo.File(
                path = "./logs/nbomber-log-" + testInfo.SessionId + ".txt",
                outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [ThreadId:{ThreadId}] {Message:lj}{NewLine}{Exception}",
                rollingInterval = RollingInterval.Day
            )

        let attachConsoleLogger (config: LoggerConfiguration) =
            config.WriteTo.Logger(fun lc ->
                lc.WriteTo.Console()
                  .Filter.ByIncludingOnly(fun event -> event.Level = LogEventLevel.Information
                                                       || event.Level = LogEventLevel.Warning)
                |> ignore
            )

        let loggerConfig =
            createConfig
            |> Option.map(fun tryGet -> tryGet())
            |> Option.defaultValue(LoggerConfiguration().MinimumLevel.Debug())
            |> fun config -> config.Enrich.WithProperty("SessionId", testInfo.SessionId)
                                   .Enrich.WithProperty("TestSuite", testInfo.TestSuite)
                                   .Enrich.WithProperty("TestName", testInfo.TestName)
                                   .Enrich.WithThreadId()
            |> attachFileLogger
            |> attachConsoleLogger

        match configPath with
        | Some path -> loggerConfig.ReadFrom.Configuration(path).CreateLogger() :> ILogger
        | None      -> loggerConfig.CreateLogger() :> ILogger

module ResourceManager =

    let readResource (name) =
        let assembly = typedefof<IGlobalDependency>.Assembly
        assembly.GetManifestResourceNames()
        |> Array.tryFind(fun x -> x.Contains name)
        |> Option.map(fun resourceName ->
            use stream = assembly.GetManifestResourceStream(resourceName)
            use reader = new StreamReader(stream)
            reader.ReadToEnd()
        )

module NodeInfo =

    let init () =
        let dotNetVersion = Assembly.GetEntryAssembly()
                                    .GetCustomAttribute<TargetFrameworkAttribute>()
                                    .FrameworkName

        let processor = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER")

        { MachineName = Environment.MachineName
          NodeType = NodeType.SingleNode
          CurrentOperation = NodeOperationType.None
          OS = Environment.OSVersion
          DotNetVersion = dotNetVersion
          Processor = if isNull processor then String.Empty else processor
          CoresCount = Environment.ProcessorCount }

module ProgressBarEnv =

    let private options =
        ProgressBarOptions(ProgressBarOnBottom = true,
                           ForegroundColor = ConsoleColor.Yellow,
                           ForegroundColorDone = Nullable<ConsoleColor>(ConsoleColor.DarkGreen),
                           BackgroundColor = Nullable<ConsoleColor>(ConsoleColor.DarkGray),
                           BackgroundCharacter = Nullable<char>('\u2593'),
                           CollapseWhenFinished = false)

    let create () =
        { new IProgressBarEnv with
            member x.CreateManualProgressBar(ticks) =
                new ProgressBar(ticks, String.Empty, options) :> IProgressBar

            member x.CreateAutoProgressBar(duration) =
                new FixedDurationBar(duration, String.Empty, options) :> IProgressBar }

let createSessionId () =
    let date = DateTime.UtcNow.ToString("yyyy-MM-dd_HH.mm.ff")
    let guid = Guid.NewGuid().GetHashCode().ToString("x")
    date + "_" + guid

let create (appType: ApplicationType) (nodeType: NodeType) (context: NBomberContext) =
    let emptyTestInfo = { SessionId = ""; TestSuite = ""; TestName = "" }
    let logger = Logger.create emptyTestInfo context.CreateLoggerConfig context.InfraConfig
    let version = typeof<ApplicationType>.Assembly.GetName().Version

    Serilog.Log.Logger <- logger

    { new IGlobalDependency with
        member x.NBomberVersion = sprintf "%i.%i.%i" version.Major version.Minor version.Build
        member x.ApplicationType = appType
        member x.NodeType = nodeType
        member x.NBomberConfig = context.NBomberConfig
        member x.InfraConfig = context.InfraConfig
        member x.CreateLoggerConfig = context.CreateLoggerConfig
        member x.ProgressBarEnv = ProgressBarEnv.create()
        member x.Logger = logger
        member x.ReportingSinks = context.ReportingSinks
        member x.WorkerPlugins = context.Plugins
        member x.Dispose() =
            x.ReportingSinks |> Seq.iter(fun x -> x.Dispose())
            x.WorkerPlugins |> Seq.iter(fun x -> x.Dispose()) }

let init (testInfo: TestInfo) (dep: IGlobalDependency) =
    let logger = Logger.create testInfo dep.CreateLoggerConfig dep.InfraConfig
    Serilog.Log.Logger <- logger

    dep.ReportingSinks |> Seq.iter(fun x -> x.Init(logger, dep.InfraConfig))
    dep.WorkerPlugins |> Seq.iter(fun x -> x.Init(logger, dep.InfraConfig))

    { new IGlobalDependency with
        member x.NBomberVersion = dep.NBomberVersion
        member x.ApplicationType = dep.ApplicationType
        member x.NodeType = dep.NodeType
        member x.NBomberConfig = dep.NBomberConfig
        member x.InfraConfig = dep.InfraConfig
        member x.CreateLoggerConfig = dep.CreateLoggerConfig
        member x.ProgressBarEnv = dep.ProgressBarEnv
        member x.Logger = logger
        member x.ReportingSinks = dep.ReportingSinks
        member x.WorkerPlugins = dep.WorkerPlugins
        member x.Dispose() = dep.Dispose() }
