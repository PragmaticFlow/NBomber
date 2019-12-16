namespace Tests.TestHelper

open Serilog
open Serilog.Sinks.InMemory

open NBomber.Contracts
open NBomber.Domain
open NBomber.Infra
open NBomber.Infra.Dependency

module internal Dependency =
    
    let createFor (nodeType: NodeType) =
        let testInfo = {
            SessionId = Dependency.createSessionId()
            TestSuite = Constants.DefaultTestSuite
            TestName = Constants.DefaultTestName
        }
        {| TestInfo = testInfo
           Dep = Dependency.create(ApplicationType.Process, nodeType, testInfo, None) |}
    
    let createWithInMemoryLogger(nodeType: NodeType) =
        let testInfo = {
            SessionId = Dependency.createSessionId()
            TestSuite = Constants.DefaultTestSuite
            TestName = Constants.DefaultTestName
        }
        let dep = Dependency.create(ApplicationType.Process, nodeType, testInfo, None)
        let inMemorySink = InMemorySink()
        let inMemoryLogger = LoggerConfiguration().WriteTo.Sink(inMemorySink).CreateLogger()
        let dependency = { dep with Logger = inMemoryLogger }
        {| TestInfo = testInfo
           Dep = dependency
           MemorySink = inMemorySink |}