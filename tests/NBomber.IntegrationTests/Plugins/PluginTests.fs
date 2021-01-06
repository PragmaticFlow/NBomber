module Tests.Plugin

open System
open System.Data
open System.Threading.Tasks

open FSharp.Control.Tasks.NonAffine
open Swensen.Unquote
open Xunit

open NBomber.Configuration
open NBomber.Contracts
open NBomber.Domain
open NBomber.DomainServices
open NBomber.Extensions
open NBomber.FSharp

module internal PluginTestHelper =

    let createScenarios () =
        let step1 = Step.create("step 1", fun _ -> task {
            do! Task.Delay(TimeSpan.FromSeconds(0.1))
            return Response.Ok()
        })

        let step2 = Step.create("step 2", fun _ -> task {
            do! Task.Delay(TimeSpan.FromSeconds(0.2))
            return Response.Ok()
        })

        let step3 = Step.create("step 3", fun _ -> task {
            do! Task.Delay(TimeSpan.FromSeconds(0.3))
            return Response.Ok()
        })

        let scenario1 =
            Scenario.create "plugin scenario 1" [step1; step2]
            |> Scenario.withoutWarmUp
            |> Scenario.withLoadSimulations [
                KeepConstant(copies = 2, during = TimeSpan.FromSeconds 3.0)
            ]

        let scenario2 =
            Scenario.create "plugin scenario 2" [step3]
            |> Scenario.withoutWarmUp
            |> Scenario.withLoadSimulations [
                KeepConstant(copies = 2, during = TimeSpan.FromSeconds 3.0)
            ]

        [scenario1; scenario2]

module internal PluginStatisticsHelper =

    let private getPluginStatisticsColumns (prefix: string) =
        let colKey = new DataColumn("Key", Type.GetType("System.String"))
        colKey.Caption <- $"{prefix}ColumnKey"

        let colValue = new DataColumn("Value", Type.GetType("System.String"))
        colValue.Caption <- $"{prefix}ColumnValue"

        let colType = new DataColumn("Type", Type.GetType("System.String"))
        colType.Caption <- $"{prefix}ColumnType"

        [| colKey; colValue; colType |]

    let private getPluginStatisticsRows (count: int) (prefix: string) (table: DataTable) = [|
        for i in 1 .. count do
            let row = table.NewRow()
            row.["Key"] <- $"{prefix}RowKey{i}"
            row.["Value"] <- $"{prefix}RowValue{i}"
            row.["Type"] <- $"{prefix}RowType{i}"
            yield row
    |]

    let private createTable (prefix: string) =
        let tableName = $"{prefix}Table"
        let table = new DataTable(tableName)

        prefix
        |> getPluginStatisticsColumns
        |> table.Columns.AddRange

        table
        |> getPluginStatisticsRows 10 prefix
        |> Array.iter table.Rows.Add

        table

    let createPluginStats () =
        let pluginStats = new DataSet()
        pluginStats.Tables.Add(createTable("PluginStatistics1"))
        pluginStats.Tables.Add(createTable("PluginStatistics2"))
        pluginStats

[<Fact>]
let ``Init should be invoked once`` () =

    let scenarios = PluginTestHelper.createScenarios()
    let mutable pluginInitInvokedCounter = 0

    let plugin = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin"

            member _.Init(_, _) =
                pluginInitInvokedCounter <- pluginInitInvokedCounter + 1
                Task.CompletedTask

            member _.Start() = Task.CompletedTask
            member _.GetStats(_) = new DataSet()
            member _.GetHints() = Array.empty
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.withWorkerPlugins [plugin]
    |> NBomberRunner.run
    |> Result.mapError failwith
    |> ignore

    test <@ pluginInitInvokedCounter = 1 @>

[<Fact>]
let ``StartTest should be invoked once`` () =

    let scenarios = PluginTestHelper.createScenarios()
    let mutable pluginStartTestInvokedCounter = 0

    let plugin = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin"
            member _.Init(_, _) = Task.CompletedTask

            member _.Start() =
                pluginStartTestInvokedCounter <- pluginStartTestInvokedCounter + 1
                Task.CompletedTask

            member _.GetHints() = Array.empty
            member _.GetStats(_) = new DataSet()
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.withWorkerPlugins [plugin]
    |> NBomberRunner.run
    |> Result.mapError failwith
    |> ignore

    test <@ pluginStartTestInvokedCounter = 1 @>


[<Fact>]
let ``StartTest should be invoked with infra config`` () =

    let scenarios = PluginTestHelper.createScenarios()
    let mutable pluginConfig = None

    let plugin = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin"

            member _.Init(logger, infraConfig) =
                pluginConfig <- infraConfig
                Task.CompletedTask

            member _.Start() = Task.CompletedTask
            member _.GetHints() = Array.empty
            member _.GetStats(_) = new DataSet()
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.loadInfraConfig "Configuration/infra_config.json"
    |> NBomberRunner.withWorkerPlugins [plugin]
    |> NBomberRunner.run
    |> Result.mapError failwith
    |> ignore

    test <@ pluginConfig.IsSome @>

[<Fact>]
let ``GetStats should be invoked many times even if no IReporingSinks were registered`` () =

    let scenarios = PluginTestHelper.createScenarios()
    let mutable pluginGetStatsInvokedCounter = 0

    let plugin = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask

            member _.GetStats(_) =
                pluginGetStatsInvokedCounter <- pluginGetStatsInvokedCounter + 1
                new DataSet()

            member _.GetHints() = Array.empty
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.withWorkerPlugins [plugin]
    |> NBomberRunner.run
    |> Result.mapError failwith
    |> ignore

    test <@ pluginGetStatsInvokedCounter >= 2 @>

[<Fact>]
let ``StopTest should be invoked once`` () =

    let scenarios = PluginTestHelper.createScenarios()
    let mutable pluginFinishTestInvokedCounter = 0

    let plugin = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask
            member _.GetStats(_) = new DataSet()
            member _.GetHints() = Array.empty
            member _.Stop() =
                pluginFinishTestInvokedCounter <- pluginFinishTestInvokedCounter + 1
                Task.CompletedTask
            member _.Dispose() = ()
    }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.withWorkerPlugins [plugin]
    |> NBomberRunner.run
    |> Result.mapError failwith
    |> ignore

    test <@ pluginFinishTestInvokedCounter = 1 @>

[<Fact>]
let ``stats should be passed to IReportingSink`` () =

    let scenarios = PluginTestHelper.createScenarios()
    let mutable _nodeStats = Array.empty

    let plugin = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask
            member _.GetStats(_) = PluginStatisticsHelper.createPluginStats()
            member _.GetHints() = Array.empty
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    let reportingSink = {
        new IReportingSink with
            member _.SinkName = "TestSink"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask

            member _.SaveStats(stats) =
                _nodeStats <- stats
                Task.CompletedTask

            member _.SaveReports(_) = Task.CompletedTask
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.withReportingSinks [reportingSink] (seconds 10)
    |> NBomberRunner.withWorkerPlugins [plugin]
    |> NBomberRunner.run
    |> Result.mapError failwith
    |> ignore

    let pluginStats = _nodeStats.[0].PluginStats.[0]
    let table1 = pluginStats.Tables.["PluginStatistics1Table"]
    let table2 = pluginStats.Tables.["PluginStatistics2Table"]

    // assert on IReportingSink
    test <@ table1.Columns.Count > 0 @>
    test <@ table1.Rows.Count > 0 @>
    test <@ table2.Columns.Count > 0 @>
    test <@ table2.Rows.Count > 0 @>

[<Fact>]
let ``stats' names should be the same as plugin names`` () =

    let scenarios = PluginTestHelper.createScenarios()

    let plugin = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask

            member _.GetStats(_) =
                let stats = new DataSet()
                stats.DataSetName <- "Stats" // should be overwritten with "TestPlugin"
                stats

            member _.GetHints() = Array.empty
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    let pluginStats =
        NBomberRunner.registerScenarios scenarios
        |> NBomberRunner.withWorkerPlugins [plugin]
        |> NBomberRunner.run
        |> function
            | Ok nodeStats -> nodeStats.PluginStats.[0]
            | Error e -> failwith e

    test <@ pluginStats.DataSetName = "TestPlugin" @>

[<Fact>]
let ``tryFindDataSetByName() should work properly`` () =

    let scenarios = PluginTestHelper.createScenarios()

    let plugin1 = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin1"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask
            member _.GetStats(_) = new DataSet()
            member _.GetHints() = Array.empty
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    let plugin2 = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin2"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask
            member _.GetStats(_) = new DataSet()
            member _.GetHints() = Array.empty
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    let nodeStats =
        NBomberRunner.registerScenarios scenarios
        |> NBomberRunner.withWorkerPlugins [plugin1; plugin2]
        |> NBomberRunner.run
        |> function
            | Ok stats -> stats
            | Error e -> failwith e

    let pluginStats1 = nodeStats.PluginStats |> PluginStats.tryFindDataSetByName "TestPlugin1"
    let pluginStats2 = nodeStats.PluginStats |> PluginStats.tryFindDataSetByName "TestPlugin2"
    let pluginStats3 = nodeStats.PluginStats |> PluginStats.tryFindDataSetByName "TestPlugin3"

    test <@ pluginStats1.IsSome @>
    test <@ pluginStats2.IsSome @>
    test <@ pluginStats3.IsNone @>

[<Fact>]
let ``NBomber should not throw ex for empty plugin stats tables`` () =

    let scenarios = PluginTestHelper.createScenarios()

    let plugin = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask

            member _.GetStats(_) =
                let stats = new DataSet()
                stats.Tables.Add(new DataTable())
                stats

            member _.GetHints() = Array.empty
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    test <@ NBomberRunner.registerScenarios scenarios
            |> NBomberRunner.withWorkerPlugins [plugin]
            |> NBomberRunner.run
            |> function
                | Ok _    -> true
                | Error _ -> false
    @>

[<Fact>]
let ``stats should be passed to reports`` () =

    let scenarios = PluginTestHelper.createScenarios()

    let plugin = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask
            member _.GetStats(_) = PluginStatisticsHelper.createPluginStats()
            member _.GetHints() = Array.empty
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    let reports =
        NBomberRunner.registerScenarios scenarios
        |> NBomberRunner.withWorkerPlugins [plugin]
        |> NBomberRunner.run
        |> Result.mapError failwith
        |> function
            | Ok nodeStats -> nodeStats.ReportFiles
            | Error _      -> Array.empty
        |> Seq.filter(fun report ->
             [ReportFormat.Txt; ReportFormat.Md; ReportFormat.Html]
             |> Seq.contains report.ReportFormat
        )
        |> Seq.map(fun report -> System.IO.File.ReadAllText(report.FilePath))

    test <@ reports |> Seq.forall(fun report -> report.Contains("PluginStatistics1")) @>
    test <@ reports |> Seq.forall(fun report -> report.Contains("PluginStatistics2")) @>

[<Fact>]
let ``table should not be passed to reports if table name starts with "."`` () =

    let scenarios = PluginTestHelper.createScenarios()

    let plugin = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask
            member _.GetStats(_) = PluginStatisticsHelper.createPluginStats()
            member _.GetHints() = Array.empty
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    let reports =
        NBomberRunner.registerScenarios scenarios
        |> NBomberRunner.withWorkerPlugins [plugin]
        |> NBomberRunner.run
        |> Result.mapError failwith
        |> function
            | Ok nodeStats -> nodeStats.ReportFiles
            | Error _      -> Array.empty
        |> Seq.filter(fun report ->
             [ReportFormat.Txt; ReportFormat.Md; ReportFormat.Html]
             |> Seq.contains report.ReportFormat
        )
        |> Seq.map(fun report -> System.IO.File.ReadAllText(report.FilePath))

    test <@ reports |> Seq.forall(fun report -> not(report.Contains(".PluginStatistics3"))) @>

[<Fact>]
let ``tables should not be passed to reports if no rows in table`` () =

    let scenarios = PluginTestHelper.createScenarios()

    let plugin = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask
            member _.GetStats(_) =
                let ds = new DataSet()
                let table = new DataTable()
                table.TableName <- "EmptyTable"
                ds.Tables.Add(table)
                ds

            member _.GetHints() = Array.empty
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    let reports =
        NBomberRunner.registerScenarios scenarios
        |> NBomberRunner.withWorkerPlugins [plugin]
        |> NBomberRunner.run
        |> Result.mapError failwith
        |> function
            | Ok nodeStats -> nodeStats.ReportFiles
            | Error _      -> Array.empty
        |> Seq.filter(fun report ->
             [ReportFormat.Txt; ReportFormat.Md; ReportFormat.Html]
             |> Seq.contains report.ReportFormat
        )
        |> Seq.map(fun report -> System.IO.File.ReadAllText(report.FilePath))

    test <@ reports |> Seq.forall(fun report -> not(report.Contains("EmptyTable"))) @>

[<Fact>]
let ``.plugin-report table should be passed to html report`` () =

    let scenarios = PluginTestHelper.createScenarios()

    let head = "<style>.plugin-html { margin-left: 300px; }</style>"
    let body = "<h3 class=\"plugin-html\">plugin html</h3>"

    let createPluginStats (currentOperation: NodeOperationType) =
        let pluginStats = new DataSet()

        if currentOperation = NodeOperationType.Complete then
            NBomber.PluginReport.create()
            |> NBomber.PluginReport.addToHtmlReportHead head
            |> NBomber.PluginReport.addToHtmlReportBody body
            |> pluginStats.Tables.Add

        pluginStats

    let plugin = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask
            member _.GetStats(currentOperation) = createPluginStats(currentOperation)
            member _.GetHints() = Array.empty
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    let reports =
        NBomberRunner.registerScenarios scenarios
        |> NBomberRunner.withWorkerPlugins [plugin]
        |> NBomberRunner.run
        |> Result.mapError failwith
        |> function
            | Ok nodeStats -> nodeStats.ReportFiles
            | Error _      -> Array.empty
        |> Seq.filter(fun report -> report.ReportFormat = ReportFormat.Html)
        |> Seq.map(fun report -> System.IO.File.ReadAllText(report.FilePath))

    test <@ reports |> Seq.forall(fun report -> report.Contains(head)) @>
    test <@ reports |> Seq.forall(fun report -> report.Contains(body)) @>
