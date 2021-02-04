module internal NBomber.Infra.Dependency

open System
open System.IO
open System.Reflection
open System.Runtime.Versioning

open Microsoft.Extensions.Configuration
open Serilog
open Serilog.Events

open NBomber.Configuration
open NBomber.Contracts

type IGlobalDependency =
    abstract ApplicationType: ApplicationType
    abstract NodeType: NodeType
    abstract NBomberConfig: NBomberConfig option
    abstract InfraConfig: IConfiguration option
    abstract CreateLoggerConfig: (unit -> LoggerConfiguration) option
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
                                                       || event.Level = LogEventLevel.Warning
                                                       || event.Level = LogEventLevel.Fatal)
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

        let dotNetVersion =
            let assembly =
                if isNull(Assembly.GetEntryAssembly()) then Assembly.GetCallingAssembly()
                else Assembly.GetEntryAssembly()

            assembly.GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName

        let processor = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER")
        let version = typeof<ApplicationType>.Assembly.GetName().Version

        { MachineName = Environment.MachineName
          NodeType = NodeType.SingleNode
          CurrentOperation = NodeOperationType.None
          OS = Environment.OSVersion
          DotNetVersion = dotNetVersion
          Processor = if isNull processor then String.Empty else processor
          CoresCount = Environment.ProcessorCount
          NBomberVersion = sprintf "%i.%i.%i" version.Major version.Minor version.Build }

let createSessionId () =
    let date = DateTime.UtcNow.ToString("yyyy-MM-dd_HH.mm.ff")
    let guid = Guid.NewGuid().GetHashCode().ToString("x")
    date + "_" + guid

let create (appType: ApplicationType) (nodeType: NodeType) (context: NBomberContext) =
    let emptyTestInfo = { SessionId = ""; TestSuite = ""; TestName = "" }
    let logger = Logger.create emptyTestInfo context.CreateLoggerConfig context.InfraConfig
    Log.Logger <- logger

    { new IGlobalDependency with
        member _.ApplicationType = appType
        member _.NodeType = nodeType
        member _.NBomberConfig = context.NBomberConfig
        member _.InfraConfig = context.InfraConfig
        member _.CreateLoggerConfig = context.CreateLoggerConfig
        member _.Logger = logger
        member _.ReportingSinks = context.Reporting.Sinks
        member _.WorkerPlugins = context.WorkerPlugins }

let init (testInfo: TestInfo) (dep: IGlobalDependency) =
    let logger = Logger.create testInfo dep.CreateLoggerConfig dep.InfraConfig
    Log.Logger <- logger

    { new IGlobalDependency with
        member _.ApplicationType = dep.ApplicationType
        member _.NodeType = dep.NodeType
        member _.NBomberConfig = dep.NBomberConfig
        member _.InfraConfig = dep.InfraConfig
        member _.CreateLoggerConfig = dep.CreateLoggerConfig
        member _.Logger = logger
        member _.ReportingSinks = dep.ReportingSinks
        member _.WorkerPlugins = dep.WorkerPlugins }
