module internal NBomber.Infra.Dependency

open System
open System.Reflection
open System.Runtime.Versioning

open Serilog
open ShellProgressBar
open Microsoft.Extensions.Configuration

open NBomber.Contracts

type ApplicationType =
    | Process
    | Console
    | Test

type GlobalDependency = {
    NBomberVersion: string
    ApplicationType: ApplicationType
    NodeType: NodeType
    CreateManualProgressBar: int -> IProgressBar
    CreateAutoProgressBar: TimeSpan -> IProgressBar
    Logger: ILogger
    ReportingSinks: IReportingSink list
    Plugins: IPlugin list
}

module ProgressBar =

    let private options =
        ProgressBarOptions(ProgressBarOnBottom = true,
                           ForegroundColor = ConsoleColor.Yellow,
                           ForegroundColorDone = Nullable<ConsoleColor>(ConsoleColor.DarkGreen),
                           BackgroundColor = Nullable<ConsoleColor>(ConsoleColor.DarkGray),
                           BackgroundCharacter = Nullable<char>('\u2593'),
                           CollapseWhenFinished = false)

    let createManual (ticks: int) =
        new ProgressBar(ticks, String.Empty, options) :> IProgressBar

    let createAuto (duration: TimeSpan) =
        new FixedDurationBar(duration, String.Empty, options) :> IProgressBar

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
      CreateManualProgressBar = ProgressBar.createManual
      CreateAutoProgressBar = ProgressBar.createAuto
      Logger = logger
      ReportingSinks = List.empty
      Plugins = List.empty }
