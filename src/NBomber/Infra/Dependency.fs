module internal NBomber.Infra.Dependency

open System
open System.Reflection
open System.Runtime.Versioning

open Serilog
open ShellProgressBar
open Microsoft.Extensions.Configuration

open NBomber.Contracts
open NBomber.Infra.ResourceManager

type ApplicationType =
    | Process
    | Console
    | Test

type MachineInfo = {
    MachineName: string
    OS: OperatingSystem
    DotNetVersion: string
    Processor: string
    CoresCount: int
}

type GlobalDependency = {
    NBomberVersion: string
    ApplicationType: ApplicationType
    NodeType: NodeType
    MachineInfo: MachineInfo
    Assets: Assets
    CreateProgressBar: int -> ProgressBar
    Logger: ILogger
    ReportingSinks: IReportingSink list
}

module ProgressBar =

    let private options =
        ProgressBarOptions(ProgressBarOnBottom = true,
                           ForegroundColor = ConsoleColor.Yellow,
                           ForegroundColorDone = Nullable<ConsoleColor>(ConsoleColor.DarkGreen),
                           BackgroundColor = Nullable<ConsoleColor>(ConsoleColor.DarkGray),
                           BackgroundCharacter = Nullable<char>('\u2593'))

    let create (ticks: int) =
        new ProgressBar(ticks, String.Empty, options)

let private retrieveMachineInfo () =

    let dotNetVersion = Assembly.GetEntryAssembly()
                                .GetCustomAttribute<TargetFrameworkAttribute>()
                                .FrameworkName

    let processor = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER")

    { MachineName = Environment.MachineName
      OS = Environment.OSVersion
      DotNetVersion = dotNetVersion
      Processor = if isNull processor then String.Empty else processor
      CoresCount = Environment.ProcessorCount }

let createSessionId () =
    let date = DateTime.UtcNow.ToString("dd.MM.yyyy_HH.mm.ff")
    let guid = Guid.NewGuid().GetHashCode().ToString("x")
    date + "_" + guid

let create (appType: ApplicationType,
            nodeType: NodeType,
            testInfo: TestInfo,
            infraConfig: IConfiguration option) =

    let logger = Logger.createLogger(testInfo, infraConfig)
    let version = typeof<ApplicationType>.Assembly.GetName().Version

    Serilog.Log.Logger <- logger

    { NBomberVersion = sprintf "%i.%i.%i" version.Major version.Minor version.Build
      ApplicationType = appType
      NodeType = nodeType
      MachineInfo = retrieveMachineInfo()
      Assets = ResourceManager.loadAssets()
      CreateProgressBar = ProgressBar.create
      Logger = logger
      ReportingSinks = List.empty }
