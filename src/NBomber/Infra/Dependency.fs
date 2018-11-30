module internal NBomber.Infra.Dependency

open System
open System.Threading.Tasks
open System.Reflection
open System.Runtime.Versioning

open Serilog
open ShellProgressBar
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Domain.DomainTypes
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

let private globalUpdatesChannel = GlobalUpdatesChannel()

type Dependency = {
    SessionId: string    
    ApplicationType: ApplicationType
    MachineInfo: MachineInfo
    Assets: Assets
} with
  static member GlobalUpdatesChannel = globalUpdatesChannel

let private getMachineInfo () =

    let dotNetVersion = Assembly.GetEntryAssembly()
                                .GetCustomAttribute<TargetFrameworkAttribute>()
                                .FrameworkName;

    let processor = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER")    

    { MachineName = Environment.MachineName      
      OS = Environment.OSVersion
      DotNetVersion = dotNetVersion
      Processor = if isNull(processor) then String.Empty else processor      
      CoresCount = Environment.ProcessorCount }

let createSessionId () =
    let date = DateTime.UtcNow.ToString("dd.MM.yyyy_HH.mm.ff")
    let guid = Guid.NewGuid().GetHashCode().ToString("x")
    date + "_" + guid

let create (appType: ApplicationType) =
    { SessionId = createSessionId()
      ApplicationType = appType
      MachineInfo = getMachineInfo()
      Assets = ResourceManager.loadAssets() }

module Logger =

    let initLogger (appType: ApplicationType) =
        Log.Logger <- 
            match appType with            
            | Console -> LoggerConfiguration().WriteTo.Console().CreateLogger()
            | _       -> LoggerConfiguration().CreateLogger()
        

module ProgressBar =

    let show (scenarioDuration: TimeSpan) = task {    
        let options = ProgressBarOptions(ProgressBarOnBottom = true,                                     
                                         ForegroundColor = ConsoleColor.Yellow,
                                         ForegroundColorDone = Nullable<ConsoleColor>(ConsoleColor.DarkGreen),
                                         BackgroundColor = Nullable<ConsoleColor>(ConsoleColor.DarkGray),
                                         BackgroundCharacter = Nullable<char>('\u2593'))

        let totalSeconds = int(scenarioDuration.TotalSeconds)                                        
        use pbar = new ProgressBar(totalSeconds, String.Empty, options)
    
        for i = 0 to totalSeconds do        
            do! Task.Delay(TimeSpan.FromSeconds(1.0))
            pbar.Tick()
    }