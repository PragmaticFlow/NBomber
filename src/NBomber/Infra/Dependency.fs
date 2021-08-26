module internal NBomber.Infra.Dependency

open System
open System.IO
open System.Reflection
open System.Runtime.Versioning

open Microsoft.Extensions.Configuration
open Serilog
open Serilog.Events
open Serilog.Sinks.SpectreConsole

open NBomber.Configuration
open NBomber.Contracts
open NBomber.Contracts.Stats

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

    let create (folder: string)
               (testInfo: TestInfo)
               (createConfig: (unit -> LoggerConfiguration) option)
               (configPath: IConfiguration option) =

        let cleanFolder (folder) =
            try
                if Directory.Exists folder then
                    Directory.Delete(folder, recursive = true)
            with
            | ex -> ()

        let attachFileLogger (config: LoggerConfiguration) =

            cleanFolder folder

            config.WriteTo.File(
                path = $"{folder}/nbomber-log-.txt",
                outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [ThreadId:{ThreadId}] {Message:lj}{NewLine}{Exception}",
                rollingInterval = RollingInterval.Day
            )

        let attachAnsiConsoleLogger (config: LoggerConfiguration) =
            config.WriteTo.Logger(fun lc ->
                let outputTemplate = "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                lc.WriteTo.spectreConsole(outputTemplate, minLevel = LogEventLevel.Information)
                  .Filter.ByIncludingOnly(fun event -> event.Level = LogEventLevel.Information
                                                       || event.Level = LogEventLevel.Warning
                                                       || event.Level = LogEventLevel.Error)
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
            |> attachAnsiConsoleLogger

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
          CurrentOperation = OperationType.None
          OS = Environment.OSVersion
          DotNetVersion = dotNetVersion
          Processor = if isNull processor then String.Empty else processor
          CoresCount = Environment.ProcessorCount
          NBomberVersion = $"{version.Major}.{version.Minor}.{version.Build}" }

    let getApplicationType () =
        try
            if Console.WindowHeight <= 0 then ApplicationType.Process
            else ApplicationType.Console
        with
        | _ -> ApplicationType.Process

let createSessionId () =
    let date = DateTime.UtcNow.ToString("yyyy-MM-dd_HH.mm.ff")
    let guid = Guid.NewGuid().GetHashCode().ToString("x")
    $"{date}_session_{guid}"

let create (reportFolder: string) (testInfo: TestInfo)
           (appType: ApplicationType) (nodeType: NodeType)
           (context: NBomberContext) =

    let logger = Logger.create reportFolder testInfo context.CreateLoggerConfig context.InfraConfig
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
