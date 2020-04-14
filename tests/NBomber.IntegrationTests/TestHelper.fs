namespace Tests.TestHelper

open Serilog
open Serilog.Sinks.InMemory

open NBomber
open NBomber.Contracts
open NBomber.Infra
open NBomber.Infra.Dependency

module internal Dependency =

    let createFor (nodeType: NodeType) =

        let testInfo = {
            SessionId = Dependency.createSessionId()
            TestSuite = Constants.DefaultTestSuite
            TestName = Constants.DefaultTestName
        }

        let emptyContext = NBomberContext.empty

        {| TestInfo = testInfo
           Dep = Dependency.init(ApplicationType.Process, nodeType, testInfo, emptyContext) |}

    let createWithInMemoryLogger(nodeType: NodeType) =

        let testInfo = {
            SessionId = Dependency.createSessionId()
            TestSuite = Constants.DefaultTestSuite
            TestName = Constants.DefaultTestName
        }

        let emptyContext = NBomberContext.empty
        let dep = Dependency.init(ApplicationType.Process, nodeType, testInfo, emptyContext)
        let inMemorySink = InMemorySink()
        let inMemoryLogger = LoggerConfiguration().WriteTo.Sink(inMemorySink).CreateLogger()
        let dependency = { dep with Logger = inMemoryLogger }

        {| TestInfo = testInfo
           Dep = dependency
           MemorySink = inMemorySink |}
