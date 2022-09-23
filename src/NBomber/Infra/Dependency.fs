namespace NBomber.Infra

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

module internal Logger =

    type LoggerInitSettings = {
        Folder: string
        TestInfo: TestInfo
        NodeType: NodeType
        AgentGroup: string
    }

    let create (logSettings: LoggerInitSettings)
               (infraConfig: IConfiguration option)
               (createLoggerConfig: (unit -> LoggerConfiguration) option) =

        let cleanFolder (folder) =
            try
                if Directory.Exists folder then
                    Directory.Delete(folder, recursive = true)
            with
            | ex -> ()

        let attachFileLogger (config: LoggerConfiguration) =

            cleanFolder logSettings.Folder

            config.WriteTo.File(
                path = $"{logSettings.Folder}/nbomber-log-.txt",
                outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [ThreadId:{ThreadId}] {Message:lj}{NewLine}{Exception}",
                rollingInterval = RollingInterval.Day
            )

        let attachAnsiConsoleLogger (config: LoggerConfiguration) =
            config.WriteTo.Logger(fun lc ->
                let outputTemplate = "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                lc.WriteTo.SpectreConsole(outputTemplate, minLevel = LogEventLevel.Information)
                  .Filter.ByIncludingOnly(fun event -> event.Level = LogEventLevel.Information
                                                       || event.Level = LogEventLevel.Warning
                                                       || event.Level = LogEventLevel.Error)
                |> ignore
            )

        let testInfo = logSettings.TestInfo

        let loggerConfig =
            createLoggerConfig
            |> Option.map(fun tryGet -> tryGet())
            |> Option.defaultValue(LoggerConfiguration().MinimumLevel.Debug())
            |> fun config -> config.Enrich.WithProperty(nameof testInfo.SessionId, testInfo.SessionId)
                                   .Enrich.WithProperty(nameof testInfo.TestSuite, testInfo.TestSuite)
                                   .Enrich.WithProperty(nameof testInfo.TestName, testInfo.TestName)
                                   .Enrich.WithProperty(nameof logSettings.NodeType, logSettings.NodeType)
                                   .Enrich.WithProperty(nameof testInfo.ClusterId, testInfo.ClusterId)
                                   .Enrich.WithProperty(nameof logSettings.AgentGroup, logSettings.AgentGroup)
                                   .Enrich.WithThreadId()
            |> attachFileLogger
            |> attachAnsiConsoleLogger

        match infraConfig with
        | Some path -> loggerConfig.ReadFrom.Configuration(path).CreateLogger() :> ILogger
        | None      -> loggerConfig.CreateLogger() :> ILogger

module internal ResourceManager =

    let readResource (name) =
        let assembly = typedefof<ReportingContext>.Assembly
        assembly.GetManifestResourceNames()
        |> Array.tryFind(fun x -> x.Contains name)
        |> Option.map(fun resourceName ->
            use stream = assembly.GetManifestResourceStream(resourceName)
            use reader = new StreamReader(stream)
            reader.ReadToEnd()
        )

module internal NodeInfo =

    let init (nbVersion: Version option) =

        let version = nbVersion |> Option.defaultValue(typeof<ReportingContext>.Assembly.GetName().Version)

        let dotNetVersion =
            let assembly =
                if isNull(Assembly.GetEntryAssembly()) then Assembly.GetCallingAssembly()
                else Assembly.GetEntryAssembly()

            assembly.GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName

        let processor = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER")

        { MachineName = Environment.MachineName
          NodeType = NodeType.SingleNode
          CurrentOperation = OperationType.None
          OS = Environment.OSVersion.ToString()
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

module internal Dependency =

    open Logger

    type IGlobalDependency =
        abstract ApplicationType: ApplicationType
        abstract NodeType: NodeType
        abstract NBomberConfig: NBomberConfig option
        abstract InfraConfig: IConfiguration option
        abstract CreateLoggerConfig: (unit -> LoggerConfiguration) option
        abstract Logger: ILogger
        abstract ReportingSinks: IReportingSink list
        abstract WorkerPlugins: IWorkerPlugin list

    let createSessionId () =
        let date = DateTime.UtcNow.ToString("yyyy-MM-dd_HH.mm.ff")
        let guid = Guid.NewGuid().GetHashCode().ToString("x")
        $"{date}_session_{guid}"

    let create (appType: ApplicationType) (logSettings: LoggerInitSettings) (context: NBomberContext) =

        let logger = Logger.create logSettings context.InfraConfig context.CreateLoggerConfig
        Log.Logger <- logger

        { new IGlobalDependency with
            member _.ApplicationType = appType
            member _.NodeType = logSettings.NodeType
            member _.NBomberConfig = context.NBomberConfig
            member _.InfraConfig = context.InfraConfig
            member _.CreateLoggerConfig = context.CreateLoggerConfig
            member _.Logger = logger
            member _.ReportingSinks = context.Reporting.Sinks
            member _.WorkerPlugins = context.WorkerPlugins }
