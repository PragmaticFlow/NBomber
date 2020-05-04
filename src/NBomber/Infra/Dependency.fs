module internal NBomber.Infra.Dependency

open System
open System.IO

open System.Reflection
open System.Runtime.Versioning
open Microsoft.Extensions.Configuration
open Serilog
open ShellProgressBar

open NBomber.Contracts

type ApplicationType =
    | Process
    | Console
    | Test

type IProgressBarEnv =
    abstract CreateManualProgressBar: tickCount:int -> IProgressBar
    abstract CreateAutoProgressBar: duration:TimeSpan -> IProgressBar

type IGlobalDependency =
    inherit IDisposable
    abstract NBomberVersion: string
    abstract ApplicationType: ApplicationType
    abstract NodeType: NodeType
    abstract InfraConfig: IConfiguration option
    abstract ProgressBarEnv: IProgressBarEnv
    abstract Logger: ILogger
    abstract ReportingSinks: IReportingSink list
    abstract Plugins: IPlugin list

module Logger =

    let create (testInfo: TestInfo) (configPath: IConfiguration option) =
        let loggerConfig = LoggerConfiguration()
                            .Enrich.WithProperty("SessionId", testInfo.SessionId)
                            .Enrich.WithProperty("TestSuite", testInfo.TestSuite)
                            .Enrich.WithProperty("TestName", testInfo.TestName)
        match configPath with
        | Some path -> loggerConfig.ReadFrom.Configuration(path).CreateLogger() :> ILogger
        | None      -> loggerConfig.WriteTo.Console().CreateLogger() :> ILogger

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
    let date = DateTime.UtcNow.ToString("dd.MM.yyyy_HH.mm.ff")
    let guid = Guid.NewGuid().GetHashCode().ToString("x")
    date + "_" + guid

let create (appType: ApplicationType) (nodeType: NodeType) (context: NBomberContext) =
    let emptyTestInfo = { SessionId = ""; TestSuite = ""; TestName = "" }
    let logger = Logger.create emptyTestInfo context.InfraConfig
    let version = typeof<ApplicationType>.Assembly.GetName().Version

    Serilog.Log.Logger <- logger

    { new IGlobalDependency with
        member x.NBomberVersion = sprintf "%i.%i.%i" version.Major version.Minor version.Build
        member x.ApplicationType = appType
        member x.NodeType = nodeType
        member x.InfraConfig = context.InfraConfig
        member x.ProgressBarEnv = ProgressBarEnv.create()
        member x.Logger = logger
        member x.ReportingSinks = context.ReportingSinks
        member x.Plugins = context.Plugins
        member x.Dispose() =
            x.ReportingSinks |> Seq.iter(fun x -> x.Dispose())
            x.Plugins |> Seq.iter(fun x -> x.Dispose()) }

let init (testInfo: TestInfo) (dep: IGlobalDependency) =
    let logger = Logger.create testInfo dep.InfraConfig
    Serilog.Log.Logger <- logger

    dep.ReportingSinks |> Seq.iter(fun x -> x.Init(logger, dep.InfraConfig))
    dep.Plugins |> Seq.iter(fun x -> x.Init(logger, dep.InfraConfig))

    { new IGlobalDependency with
        member x.NBomberVersion = dep.NBomberVersion
        member x.ApplicationType = dep.ApplicationType
        member x.NodeType = dep.NodeType
        member x.InfraConfig = dep.InfraConfig
        member x.ProgressBarEnv = dep.ProgressBarEnv
        member x.Logger = logger
        member x.ReportingSinks = dep.ReportingSinks
        member x.Plugins = dep.Plugins
        member x.Dispose() = dep.Dispose() }
