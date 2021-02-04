namespace Tests.TestHelper

open Serilog
open Serilog.Sinks.InMemory

open NBomber
open NBomber.Contracts
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.DomainServices

module internal Dependency =

    let createFor (nodeType: NodeType) =

        let testInfo = {
            SessionId = Dependency.createSessionId()
            TestSuite = Constants.DefaultTestSuite
            TestName = Constants.DefaultTestName
        }

        let emptyContext = NBomberContext.empty
        let dep = Dependency.create ApplicationType.Process nodeType emptyContext
                  |> Dependency.init(testInfo)

        {| TestInfo = testInfo; Dep = dep |}

    let createWithInMemoryLogger (nodeType: NodeType) =

        let testInfo = {
            SessionId = Dependency.createSessionId()
            TestSuite = Constants.DefaultTestSuite
            TestName = Constants.DefaultTestName
        }

        let inMemorySink = InMemorySink()
        let loggerConfig = fun () -> LoggerConfiguration().WriteTo.Sink(inMemorySink)
        let context = { NBomberContext.empty with CreateLoggerConfig = Some loggerConfig }

        let dep = Dependency.create ApplicationType.Process nodeType context
                  |> Dependency.init(testInfo)

        let dependency = {
            new IGlobalDependency with
                member _.ApplicationType = dep.ApplicationType
                member _.NodeType = dep.NodeType
                member _.NBomberConfig = dep.NBomberConfig
                member _.InfraConfig = dep.InfraConfig
                member _.CreateLoggerConfig = dep.CreateLoggerConfig
                member _.Logger = dep.Logger
                member _.ReportingSinks = dep.ReportingSinks
                member _.WorkerPlugins = dep.WorkerPlugins }

        {| TestInfo = testInfo
           Dep = dependency
           MemorySink = inMemorySink |}

module List =

    /// Safe variant of `List.min`
    let minOrDefault defaultValue list =
        if List.isEmpty list then defaultValue
        else List.min list

    /// Safe variant of `List.max`
    let maxOrDefault defaultValue list =
        if List.isEmpty list then defaultValue
        else List.max list

    /// Safe variant of `List.average`
    let averageOrDefault (defaultValue: float) list =
        if List.isEmpty list then defaultValue
        else list |> List.average
