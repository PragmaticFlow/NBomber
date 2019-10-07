namespace Tests.TestHelper

open Serilog
open Serilog.Sinks.InMemory

open NBomber.Contracts
open NBomber.Infra
open NBomber.Infra.Dependency

module internal Dependency =
    
    let createFor (nodeType: NodeType) =
        Dependency.create(ApplicationType.Process, nodeType, None)
    
    let createWithInMemoryLogger(nodeType: NodeType) =
        let dep = Dependency.create(ApplicationType.Process, nodeType, None)
        let inMemorySink = InMemorySink()
        let inMemoryLogger = LoggerConfiguration().WriteTo.Sink(inMemorySink).CreateLogger()        
                        
        let dependency = { dep with Logger = inMemoryLogger }
        (dependency, inMemorySink)
        

