namespace NBomber.Infra

open System
open System.IO
open System.Reflection
open System.Runtime.CompilerServices
open System.Runtime.Versioning

open Microsoft.Extensions.Configuration
open Serilog
open Serilog.Events
open Serilog.Sinks.SpectreConsole

open NBomber.Configuration
open NBomber.Contracts
open NBomber.Contracts.Stats

type internal IGlobalDependency =
    abstract ApplicationType: ApplicationType
    abstract NodeType: NodeType
    abstract NBomberConfig: NBomberConfig option
    abstract InfraConfig: IConfiguration option
    abstract CreateLoggerConfig: (unit -> LoggerConfiguration) option
    abstract Logger: ILogger
    abstract ConsoleLogger: ILogger
    abstract ReportingSinks: IReportingSink list
    abstract WorkerPlugins: IWorkerPlugin list

[<Extension>]
type internal LogExt =
        
    [<Extension>]
    static member LogInfo(dep: IGlobalDependency, msg) =
        dep.ConsoleLogger.Information msg
        dep.Logger.Information msg 
    
    [<Extension>]
    static member LogInfo(dep: IGlobalDependency, msg, [<ParamArray>]propertyValues: obj[]) =
        dep.ConsoleLogger.Information(msg, propertyValues)
        dep.Logger.Information(msg, propertyValues)
    
    [<Extension>]
    static member LogWarn(dep: IGlobalDependency, msg) =
        dep.ConsoleLogger.Warning msg
        dep.Logger.Warning msg
        
    [<Extension>]
    static member LogWarn(dep: IGlobalDependency, msg, [<ParamArray>]propertyValues: obj[]) =
        dep.ConsoleLogger.Warning(msg, propertyValues)
        dep.Logger.Warning(msg, propertyValues)        
        
    [<Extension>]
    static member LogWarn(dep: IGlobalDependency, ex: exn, msg) =        
        if dep.Logger.IsEnabled LogEventLevel.Verbose then
            dep.ConsoleLogger.Warning(ex, msg)
        else
            dep.ConsoleLogger.Warning(msg)
            
        dep.Logger.Warning(ex, msg)
        
    [<Extension>]
    static member LogWarn(dep: IGlobalDependency, ex: exn, msg, [<ParamArray>]propertyValues: obj[]) =
        if dep.Logger.IsEnabled LogEventLevel.Verbose then
            dep.ConsoleLogger.Warning(ex, msg, propertyValues)
        else
            dep.ConsoleLogger.Warning(msg, propertyValues)            
        
        dep.Logger.Warning(ex, msg, propertyValues)
        
    [<Extension>]
    static member LogError(dep: IGlobalDependency, msg) =
        dep.ConsoleLogger.Error msg
        dep.Logger.Error msg
        
    [<Extension>]
    static member LogError(dep: IGlobalDependency, msg, [<ParamArray>]propertyValues: obj[]) =
        dep.ConsoleLogger.Error(msg, propertyValues)
        dep.Logger.Error(msg, propertyValues)        
        
    [<Extension>]
    static member LogError(dep: IGlobalDependency, ex: exn, msg) =
        if dep.Logger.IsEnabled LogEventLevel.Verbose then
            dep.ConsoleLogger.Error(ex, msg)
        else
            dep.ConsoleLogger.Error msg
        
        dep.Logger.Error(ex, msg)
        
    [<Extension>]
    static member LogError(dep: IGlobalDependency, ex: exn, msg, [<ParamArray>]propertyValues: obj[]) =
        if dep.Logger.IsEnabled LogEventLevel.Verbose then
            dep.ConsoleLogger.Error(ex, msg, propertyValues)
        else
            dep.ConsoleLogger.Error(msg, propertyValues)
        
        dep.Logger.Error(ex, msg, propertyValues)
    
    [<Extension>]
    static member LogFatal(dep: IGlobalDependency, msg) =
        dep.ConsoleLogger.Fatal msg
        dep.Logger.Fatal msg
        
    [<Extension>]
    static member LogFatal(dep: IGlobalDependency, msg, [<ParamArray>]propertyValues: obj[]) =
        dep.ConsoleLogger.Fatal(msg, propertyValues)
        dep.Logger.Fatal(msg, propertyValues)        
        
    [<Extension>]
    static member LogFatal(dep: IGlobalDependency, ex: exn, msg) =
        if dep.Logger.IsEnabled LogEventLevel.Verbose then
            dep.ConsoleLogger.Fatal(ex, msg)
        else
            dep.ConsoleLogger.Fatal msg        
        
        dep.Logger.Fatal(ex, msg)
        
    [<Extension>]
    static member LogFatal(dep: IGlobalDependency, ex: exn, msg, [<ParamArray>]propertyValues: obj[]) =
        if dep.Logger.IsEnabled LogEventLevel.Verbose then
            dep.ConsoleLogger.Fatal(ex, msg, propertyValues)
        else
            dep.ConsoleLogger.Fatal(msg, propertyValues)
        
        dep.Logger.Fatal(ex, msg, propertyValues)

module internal Logger =

    type LoggerInitSettings = {
        Folder: string
        TestInfo: TestInfo
        NodeType: NodeType
        AgentGroup: string
    }

    let createConsoleLogger () =
        let outputTemplate = "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        let config = LoggerConfiguration()
        config.WriteTo.SpectreConsole(outputTemplate, minLevel = LogEventLevel.Information) |> ignore
        config.CreateLogger() :> ILogger

    let create (logSettings: LoggerInitSettings) (context: NBomberContext) =

        let cleanFolder (folder) =
            try
                if Directory.Exists folder then
                    Directory.Delete(folder, recursive = true)
            with
            | ex -> ()

        let attachDefaultFileLogger (config: LoggerConfiguration) =
            config.WriteTo.File(
                path = $"{logSettings.Folder}/nbomber-log-.txt",
                outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [ThreadId:{ThreadId}] {Message:lj}{NewLine}{Exception}",
                rollingInterval = RollingInterval.Day
            )

        cleanFolder logSettings.Folder
        let testInfo = logSettings.TestInfo

        let loggerConfig =
            context.CreateLoggerConfig
            |> Option.map(fun tryGet -> tryGet())
            |> Option.defaultWith(fun () ->
                let logLevel =
                    context.MinimumLogLevel
                    |> Option.defaultValue LogEventLevel.Debug                
                
                LoggerConfiguration().MinimumLevel.Is logLevel
                |> attachDefaultFileLogger
            )
            |> fun config -> config.Enrich.WithProperty(nameof testInfo.SessionId, testInfo.SessionId)
                                   .Enrich.WithProperty(nameof testInfo.TestSuite, testInfo.TestSuite)
                                   .Enrich.WithProperty(nameof testInfo.TestName, testInfo.TestName)
                                   .Enrich.WithProperty(nameof logSettings.NodeType, logSettings.NodeType)
                                   .Enrich.WithProperty(nameof testInfo.ClusterId, testInfo.ClusterId)
                                   .Enrich.WithProperty(nameof logSettings.AgentGroup, logSettings.AgentGroup)
                                   .Enrich.WithThreadId()

        match context.InfraConfig with
        | Some path -> loggerConfig.ReadFrom.Configuration(path).CreateLogger() :> ILogger
        | None      -> loggerConfig.CreateLogger() :> ILogger

module internal Dependency =

    open Logger

    let createSessionId () =
        let date = DateTime.UtcNow.ToString("yyyy-MM-dd_HH.mm.ff")
        let guid = Guid.NewGuid().GetHashCode().ToString("x")
        $"{date}_session_{guid}"

    let private disposeLogger (logger: ILogger) =
        if logger :? IDisposable then
            (logger :?> IDisposable).Dispose()    
    
    let dispose (dep: IGlobalDependency) =
        disposeLogger dep.Logger                
    
    let create (appType: ApplicationType) (logSettings: LoggerInitSettings) (context: NBomberContext) =

        let consoleLogger = createConsoleLogger()
        let logger = Logger.create logSettings context
        Log.Logger <- logger

        { new IGlobalDependency with
            member _.ApplicationType = appType
            member _.NodeType = logSettings.NodeType
            member _.NBomberConfig = context.NBomberConfig
            member _.InfraConfig = context.InfraConfig
            member _.CreateLoggerConfig = context.CreateLoggerConfig
            member _.Logger = logger
            member _.ConsoleLogger = consoleLogger
            member _.ReportingSinks = context.Reporting.Sinks
            member _.WorkerPlugins = context.WorkerPlugins }

    let withNewLogger (dep: IGlobalDependency) (logger: ILogger) =        
        disposeLogger dep.Logger
        
        { new IGlobalDependency with
            member _.ApplicationType = dep.ApplicationType
            member _.NodeType = dep.NodeType
            member _.NBomberConfig = dep.NBomberConfig
            member _.InfraConfig = dep.InfraConfig
            member _.CreateLoggerConfig = dep.CreateLoggerConfig
            member _.Logger = logger
            member _.ConsoleLogger = dep.ConsoleLogger
            member _.ReportingSinks = dep.ReportingSinks
            member _.WorkerPlugins = dep.WorkerPlugins }

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
