module internal rec NBomber.Infra.EnvironmentInfo

open System
open System.Reflection
open System.Runtime.Versioning

open NBomber.Contracts

type EnvironmentInfo = {
    OS: OperatingSystem
    DotNetVersion: string
    Processor: string
    ProcessorArchitecture: string
    AssemblyName: string
    AssemblyVersion: Version
}

let getEnvironmentInfo () =
    let assembly = Assembly.GetAssembly(typedefof<Request>)
    let processor = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER")
    let processorArchitecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE")

    { OS = Environment.OSVersion
      DotNetVersion = assembly.GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName 
      Processor = if isNull(processor) then String.Empty else processor
      ProcessorArchitecture = if isNull(processorArchitecture) then String.Empty else processorArchitecture
      AssemblyName = assembly.GetName().Name
      AssemblyVersion = assembly.GetName().Version }