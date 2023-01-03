namespace Tests.TestHelper

open System
open System.Data

open Serilog
open Serilog.Sinks.InMemory

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Infra
open NBomber.Infra.Dependency
open NBomber.Infra.Logger
open NBomber.DomainServices

module internal Dependency =

    let createFor (nodeType: NodeType) =

        let testInfo = {
            SessionId = Dependency.createSessionId()
            TestSuite = Constants.DefaultTestSuite
            TestName = Constants.DefaultTestName
            ClusterId = ""
        }

        let emptyContext = NBomberContext.empty

        let logSettings = {
            Folder = "./reports"
            TestInfo = testInfo
            NodeType = nodeType
            AgentGroup = ""
        }

        let dep = Dependency.create ApplicationType.Process logSettings emptyContext
        {| TestInfo = testInfo; Dep = dep |}

    let createWithInMemoryLogger (nodeType: NodeType) =

        let testInfo = {
            SessionId = Dependency.createSessionId()
            TestSuite = Constants.DefaultTestSuite
            TestName = Constants.DefaultTestName
            ClusterId = ""
        }

        let inMemorySink = new InMemorySink()
        let loggerConfig = fun () -> LoggerConfiguration().WriteTo.Sink(inMemorySink)
        let context = { NBomberContext.empty with CreateLoggerConfig = Some loggerConfig }

        let logSettings = {
            Folder = "./reports"
            TestInfo = testInfo
            NodeType = nodeType
            AgentGroup = ""
        }

        let dep = Dependency.create ApplicationType.Process logSettings context

        let dependency = {
            new IGlobalDependency with
                member _.ApplicationType = dep.ApplicationType
                member _.NodeType = dep.NodeType
                member _.NBomberConfig = dep.NBomberConfig
                member _.InfraConfig = dep.InfraConfig
                member _.CreateLoggerConfig = dep.CreateLoggerConfig
                member _.Logger = dep.Logger
                member _.ConsoleLogger = dep.ConsoleLogger
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

module internal PluginStatisticsHelper =

    let private getPluginStatisticsColumns (prefix: string) =
        let colKey = new DataColumn("Key", Type.GetType("System.String"))
        colKey.Caption <- $"%s{prefix}ColumnKey"

        let colValue = new DataColumn("Value", Type.GetType("System.String"))
        colValue.Caption <- $"%s{prefix}ColumnValue"

        let colType = new DataColumn("Type", Type.GetType("System.String"))
        colType.Caption <- $"%s{prefix}ColumnType"

        [| colKey; colValue; colType |]

    let private getPluginStatisticsRows (count: int) (prefix: string) (table: DataTable) = [|
        for i in 1 .. count do
            let row = table.NewRow()
            row["Key"] <- $"%s{prefix}RowKey%i{i}"
            row["Value"] <- $"%s{prefix}RowValue%i{i}"
            row["Type"] <- $"%s{prefix}RowType%i{i}"
            yield row
    |]

    let private createTable (prefix: string) =
        let tableName = $"%s{prefix}Table"
        let table = new DataTable(tableName)

        prefix
        |> getPluginStatisticsColumns
        |> table.Columns.AddRange

        table
        |> getPluginStatisticsRows 10 prefix
        |> Array.iter(fun x -> x |> table.Rows.Add)

        table

    let createPluginStats () =
        let pluginStats = new DataSet()
        pluginStats.Tables.Add(createTable("PluginStatistics1"))
        pluginStats.Tables.Add(createTable("PluginStatistics2"))
        pluginStats
