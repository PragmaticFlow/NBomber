module internal NBomber.Infra.Dependency

open System
open System.Threading.Tasks
open System.Reflection
open System.Runtime.Versioning

open Serilog
open ShellProgressBar
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.Infra.ResourceManager

type ApplicationType =
    | Process
    | Console
    | Test

type NodeInfo = {
    NodeName: string
    OS: OperatingSystem
    DotNetVersion: string
    Processor: string
    CoresCount: int
}

type Dependency = {
    SessionId: string
    ApplicationType: ApplicationType
    NodeType: NodeType
    NodeInfo: NodeInfo
    Assets: Assets
    ShowProgressBar: TimeSpan -> unit
}

module ProgressBar =

    let show (scenarioDuration: TimeSpan) = task {
        let options = ProgressBarOptions(ProgressBarOnBottom = true,
                                         ForegroundColor = ConsoleColor.Yellow,
                                         ForegroundColorDone = Nullable<ConsoleColor> ConsoleColor.DarkGreen,
                                         BackgroundColor = Nullable<ConsoleColor> ConsoleColor.DarkGray,
                                         BackgroundCharacter = Nullable<char> '\u2593')

        let totalSeconds = int scenarioDuration.TotalSeconds
        use pbar = new ProgressBar(totalSeconds, String.Empty, options)

        for i = 1 to totalSeconds do
            do! Task.Delay(TimeSpan.FromSeconds 1.0)
            pbar.Tick()
    }

module Logger =
    let initLogger (appType: ApplicationType) =
        Log.Logger <-
            match appType with
            | Console -> LoggerConfiguration().WriteTo.Console().CreateLogger()
            | _       -> LoggerConfiguration().CreateLogger()

let private retrieveNodeInfo () =

    let dotNetVersion = Assembly.GetEntryAssembly()
                                .GetCustomAttribute<TargetFrameworkAttribute>()
                                .FrameworkName;

    let processor = Environment.GetEnvironmentVariable "PROCESSOR_IDENTIFIER"

    { NodeName = Environment.MachineName
      OS = Environment.OSVersion
      DotNetVersion = dotNetVersion
      Processor = if isNull processor then String.Empty else processor
      CoresCount = Environment.ProcessorCount }

let createSessionId () =
    let date = DateTime.UtcNow.ToString "dd.MM.yyyy_HH.mm.ff"
    let guid = Guid.NewGuid().GetHashCode().ToString "x"
    date + "_" + guid

let create (appType: ApplicationType, nodeType: NodeType) =
    { SessionId = createSessionId()
      ApplicationType = appType
      NodeType = nodeType
      NodeInfo = retrieveNodeInfo()
      Assets = ResourceManager.loadAssets()
      ShowProgressBar = ProgressBar.show >> ignore }
