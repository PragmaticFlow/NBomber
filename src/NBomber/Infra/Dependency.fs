﻿module internal NBomber.Infra.Dependency

open System
open System.Threading.Tasks
open System.Reflection
open System.Runtime.Versioning

open Serilog
open Serilog.Sinks.TestCorrelator
open ShellProgressBar
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.Infra.ResourceManager
open NBomber.Configuration

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

type Dependency = {    
    SessionId: string    
    NBomberVersion: string
    ApplicationType: ApplicationType
    NodeType: NodeType
    MachineInfo: MachineInfo
    Assets: Assets
    ShowProgressBar: TimeSpan -> unit
    CreateProgressBar: int -> ProgressBar
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

    let show (scenarioDuration: TimeSpan) = task {    
        let totalSeconds = int(scenarioDuration.TotalSeconds)                                        
        use pbar = new ProgressBar(totalSeconds, String.Empty, options)
    
        for i = 1 to totalSeconds do        
            do! Task.Delay(TimeSpan.FromSeconds(1.0))
            pbar.Tick()
    }

module Logger =

    let createLogger (config: LoggerConfiguration) =
        config.CreateLogger()

    let withConsoleOutput appType (config: LoggerConfiguration) =
        match appType with
        | Console -> config.WriteTo.Console()
        | _       -> config

    let withFileOutput (logSettings: LogSettings option) (config: LoggerConfiguration) =
        match logSettings with
        | Some v when v.FileName |> String.IsNullOrEmpty |> not ->                        
                config.WriteTo.File(v.FileName, v.MinimumLevel)
        | _  -> config
        
    let withInMemory appType (config: LoggerConfiguration) =
        match appType with
        | Test -> config.WriteTo.TestCorrelator()
        | _    -> config

    let initLogger (appType: ApplicationType, logSettings: LogSettings option) =
        Log.Logger <- 
            LoggerConfiguration()
            |> withConsoleOutput appType
            |> withFileOutput logSettings
            |> withInMemory appType
            |> createLogger

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

let create (appType: ApplicationType, nodeType: NodeType) =
    let version = typeof<ApplicationType>.Assembly.GetName().Version
    { SessionId = createSessionId()
      NBomberVersion = sprintf "%i.%i.%i" version.Major version.Minor version.Build
      ApplicationType = appType
      NodeType = nodeType
      MachineInfo = retrieveMachineInfo()
      Assets = ResourceManager.loadAssets()
      ShowProgressBar = ProgressBar.show >> ignore
      CreateProgressBar = fun ticks -> ProgressBar.create(ticks) }