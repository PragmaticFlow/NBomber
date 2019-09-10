namespace Tests.TestHelper

open Serilog
open Serilog.Configuration
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
        let logger = LoggerConfiguration().WriteTo.Sink(inMemorySink).CreateLogger()
        
        // todo: please replace on instance only logger
        Log.Logger <- logger        
        let dependency = { dep with Logger = logger }
        (dependency, inMemorySink)
        

