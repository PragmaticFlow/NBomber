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

type GlobalDependency = {
    NBomberVersion: string
    ApplicationType: ApplicationType
    NodeType: NodeType
    ProgressBarEnv: IProgressBarEnv
    Logger: ILogger
    ReportingSinks: IReportingSink list
    Plugins: IPlugin list
}

module Logger =

    let create (testInfo: TestInfo) (configPath: IConfiguration option) =
        let loggerConfig = LoggerConfiguration()
                            .Enrich.WithProperty("SessionId", testInfo.SessionId)
                            .Enrich.WithProperty("TestSuite", testInfo.TestSuite)
                            .Enrich.WithProperty("TestName", testInfo.TestName)
        match configPath with
        | Some path -> loggerConfig.ReadFrom.Configuration(path).CreateLogger()
        | None      -> loggerConfig.WriteTo.Console().CreateLogger()

module ResourceManager =

    let readResource (name) =
        let assembly = typedefof<GlobalDependency>.Assembly
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

let init (appType: ApplicationType, nodeType: NodeType, testInfo: TestInfo, context: NBomberContext) =

    let logger = context.InfraConfig |> Logger.create(testInfo)
    let version = typeof<ApplicationType>.Assembly.GetName().Version

    context.ReportingSinks |> Seq.iter(fun x -> x.Init(logger, context.InfraConfig))
    context.Plugins |> Seq.iter(fun x -> x.Init(logger, context.InfraConfig))

    Serilog.Log.Logger <- logger

    { NBomberVersion = sprintf "%i.%i.%i" version.Major version.Minor version.Build
      ApplicationType = appType
      NodeType = nodeType
      ProgressBarEnv = ProgressBarEnv.create()
      Logger = logger
      ReportingSinks = context.ReportingSinks
      Plugins = context.Plugins }
