module internal NBomber.Infra.Dependency

open System
open System.Threading.Tasks
open System.Reflection
open System.Runtime.Versioning

open Serilog
open ShellProgressBar
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.Domain.DomainTypes
open NBomber.Infra.ResourceManager

type EnvironmentInfo = {
    OS: OperatingSystem
    DotNetVersion: string
    Processor: string
    ProcessorArchitecture: string
    AssemblyName: string
    AssemblyVersion: Version
}

type Dependency = {
    SessionId: string
    Scenario: Scenario    
    EnvironmentInfo: EnvironmentInfo
    Assets: Assets
}

let private getEnvironmentInfo () =
    let assembly = Assembly.GetAssembly(typedefof<Request>)
    let processor = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER")
    let processorArchitecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE")

    { OS = Environment.OSVersion
      DotNetVersion = assembly.GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName 
      Processor = if isNull(processor) then String.Empty else processor
      ProcessorArchitecture = if isNull(processorArchitecture) then String.Empty else processorArchitecture
      AssemblyName = assembly.GetName().Name
      AssemblyVersion = assembly.GetName().Version }

let create (scenario: Scenario) = 

    let createSessionId () =
        let date = DateTime.UtcNow.ToString("dd.MM.yyyy-HH.mm.ff")
        let guid = Guid.NewGuid().GetHashCode().ToString("x")
        date + "-" + guid
    
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