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

        {| TestInfo = testInfo
           Dep = Dependency.create ApplicationType.Process nodeType emptyContext
                 |> Dependency.init testInfo |}

    let createWithInMemoryLogger(nodeType: NodeType) =

        let testInfo = {
            SessionId = Dependency.createSessionId()
            TestSuite = Constants.DefaultTestSuite
            TestName = Constants.DefaultTestName
        }

        let emptyContext = NBomberContext.empty
        let dep = Dependency.create ApplicationType.Process nodeType emptyContext
                  |> Dependency.init testInfo

        let inMemorySink = InMemorySink()
        let inMemoryLogger = LoggerConfiguration().WriteTo.Sink(inMemorySink).CreateLogger()

        let dependency = {
            new IGlobalDependency with
                member x.NBomberVersion = dep.NBomberVersion
                member x.ApplicationType = dep.ApplicationType
                member x.NodeType = dep.NodeType
                member x.NBomberConfig = dep.NBomberConfig
                member x.InfraConfig = dep.InfraConfig
                member x.ProgressBarEnv = dep.ProgressBarEnv
                member x.Logger = inMemoryLogger :> ILogger
                member x.ReportingSinks = dep.ReportingSinks
                member x.Plugins = dep.Plugins
                member x.Dispose() = dep.Dispose() }

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
