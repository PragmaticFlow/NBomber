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

type EnvironmentInfo = {
    MachineName: string    
    OS: OperatingSystem
    DotNetVersion: string    
    Processor: string    
    CoresCount: int    
}

type Dependency = {
    SessionId: string
    Scenario: Scenario    
    EnvironmentInfo: EnvironmentInfo
    Assets: Assets
}

let private getEnvironmentInfo () =

    let dotNetVersion = Assembly.GetEntryAssembly()
                                .GetCustomAttribute<TargetFrameworkAttribute>()
                                .FrameworkName;

    let processor = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER")    

    { MachineName = Environment.MachineName      
      OS = Environment.OSVersion
      DotNetVersion = dotNetVersion
      Processor = if isNull(processor) then String.Empty else processor      
      CoresCount = Environment.ProcessorCount }

let create (scenario: Scenario) = 

    let createSessionId () =
        let date = DateTime.UtcNow.ToString("dd.MM.yyyy_HH.mm.ff")
        let guid = Guid.NewGuid().GetHashCode().ToString("x")
        date + "_" + guid
    
    { SessionId = createSessionId()
      Scenario  = scenario      
      EnvironmentInfo = getEnvironmentInfo()
      Assets = ResourceManager.loadAssets() }

module Logger =

    let initLogger () =
        Log.Logger <- LoggerConfiguration()
                        .WriteTo.Console()
                        .CreateLogger()

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