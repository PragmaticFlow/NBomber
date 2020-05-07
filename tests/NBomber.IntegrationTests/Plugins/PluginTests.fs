module Tests.Plugin

open System
open System.Data
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Swensen.Unquote
open Xunit

open NBomber.Contracts
open NBomber.Domain
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
                KeepConcurrentScenarios(copiesCount = 2, during = TimeSpan.FromSeconds 3.0)
            ]

        let scenario2 =
            Scenario.create "plugin scenario 2" [step3]
            |> Scenario.withoutWarmUp
            |> Scenario.withLoadSimulations [
                KeepConcurrentScenarios(copiesCount = 2, during = TimeSpan.FromSeconds 3.0)
            ]

        [scenario1; scenario2]

module internal PluginStatisticsHelper =

    let private getPluginStatisticsColumns (prefix: string) =
        let colKey = new DataColumn("Key", Type.GetType("System.String"))
        colKey.Caption <- sprintf "%sColumnKey" prefix

        let colValue = new DataColumn("Value", Type.GetType("System.String"))
        colValue.Caption <- sprintf "%sColumnValue" prefix

        let colType = new DataColumn("Type", Type.GetType("System.String"))
        colType.Caption <- sprintf "%sColumnType" prefix

        [| colKey; colValue; colType |]

    let private getPluginStatisticsRows (count: int) (prefix: string) (table: DataTable) = [|
        for i in 1 .. count do
            let row = table.NewRow()
            row.["Key"] <- sprintf "%sRowKey%i" prefix i
            row.["Value"] <- sprintf "%sRowValue%i" prefix i
            row.["Type"] <- sprintf "%sRowType%i" prefix i
            yield row
    |]

    let private createTable (prefix: string) =
        let tableName = sprintf "%sTable" prefix
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

[<Fact>]
let ``Init should be invoked once`` () =

    let scenarios = PluginTestHelper.createScenarios()
    let mutable pluginInitInvokedCounter = 0

    let plugin = {
        new IPlugin with
            member x.PluginName = "TestPlugin"
            member x.Init(_, _) = pluginInitInvokedCounter <- pluginInitInvokedCounter + 1
            member x.StartTest(_) = Task.CompletedTask
            member x.GetStats() = new DataSet()
            member x.StopTest() = Task.CompletedTask
            member x.Dispose() = ()
    }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.withPlugins [plugin]
    |> NBomberRunner.runTest Array.empty
    |> Result.mapError(fun x -> failwith x)
    |> ignore

    test <@ pluginInitInvokedCounter = 1 @>

[<Fact>]
let ``StartTest should be invoked once`` () =

    let scenarios = PluginTestHelper.createScenarios()
    let mutable pluginStartTestInvokedCounter = 0

    let plugin = {
        new IPlugin with
            member x.PluginName = "TestPlugin"
            member x.Init(_, _) = ()

            member x.StartTest(_) =
                pluginStartTestInvokedCounter <- pluginStartTestInvokedCounter + 1
                Task.CompletedTask

            member x.GetStats() = new DataSet()
            member x.StopTest() = Task.CompletedTask
            member x.Dispose() = ()
    }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.withPlugins [plugin]
    |> NBomberRunner.runTest Array.empty
    |> Result.mapError(fun x -> failwith x)
    |> ignore

    test <@ pluginStartTestInvokedCounter = 1 @>


[<Fact>]
let ``StartTest should be invoked with infra config`` () =

    let scenarios = PluginTestHelper.createScenarios()
    let mutable pluginConfig = None

    let plugin = {
        new IPlugin with
            member x.PluginName = "TestPlugin"
            member x.Init(logger, infraConfig) = pluginConfig <- infraConfig
            member x.StartTest(_) = Task.CompletedTask
            member x.GetStats() = new DataSet()
            member x.StopTest() = Task.CompletedTask
            member x.Dispose() = ()
    }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.loadInfraConfig "Configuration/infra_config.yaml"
    |> NBomberRunner.withPlugins [plugin]
    |> NBomberRunner.runTest Array.empty
    |> Result.mapError(fun x -> failwith x)
    |> ignore

    test <@ pluginConfig.IsSome @>

[<Fact>]
let ``GetStats should be invoked 2 times (warm-up and compleate bombing) if no IReporingSinks were registered`` () =

    let scenarios = PluginTestHelper.createScenarios()
    let mutable pluginGetStatsInvokedCounter = 0

    let plugin = {
        new IPlugin with
            member x.PluginName = "TestPlugin"
            member x.Init(_, _) = ()
            member x.StartTest(_) = Task.CompletedTask

            member x.GetStats() =
                pluginGetStatsInvokedCounter <- pluginGetStatsInvokedCounter + 1
                new DataSet()

            member x.StopTest() = Task.CompletedTask
            member x.Dispose() = ()
    }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.withPlugins [plugin]
    |> NBomberRunner.runTest Array.empty
    |> Result.mapError(fun x -> failwith x)
    |> ignore

    test <@ pluginGetStatsInvokedCounter = 2 @>

[<Fact>]
let ``StopTest should be invoked once`` () =

    let scenarios = PluginTestHelper.createScenarios()
    let mutable pluginFinishTestInvokedCounter = 0

    let plugin = {
        new IPlugin with
            member x.PluginName = "TestPlugin"
            member x.Init(_, _) = ()
            member x.StartTest(_) = Task.CompletedTask
            member x.GetStats() = new DataSet()

            member x.StopTest() =
                pluginFinishTestInvokedCounter <- pluginFinishTestInvokedCounter + 1
                Task.CompletedTask

            member x.Dispose() = ()
    }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.withPlugins [plugin]
    |> NBomberRunner.runTest Array.empty
    |> Result.mapError(fun x -> failwith x)
    |> ignore

    test <@ pluginFinishTestInvokedCounter = 1 @>

[<Fact>]
let ``stats should be passed to IReportingSink`` () =

    let scenarios = PluginTestHelper.createScenarios()
    let mutable _nodeStats = Array.empty

    let plugin = {
        new IPlugin with
            member x.PluginName = "TestPlugin"
            member x.Init(_, _) = ()
            member x.StartTest(_) = Task.CompletedTask
            member x.GetStats() = PluginStatisticsHelper.createPluginStats()
            member x.StopTest() = Task.CompletedTask
            member x.Dispose() = ()
    }

    let reportingSink = {
        new IReportingSink with
            member x.SinkName = "TestSink"
            member x.Init(_, _) = ()
            member x.StartTest(_) = Task.CompletedTask
            member x.SaveRealtimeStats(_) = Task.CompletedTask

            member x.SaveFinalStats(stats) =
                _nodeStats <- stats
                Task.CompletedTask

            member x.StopTest() = Task.CompletedTask
            member x.Dispose() = ()
    }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.withReportingSinks([reportingSink], TimeSpan.FromSeconds 10.0)
    |> NBomberRunner.withPlugins [plugin]
    |> NBomberRunner.runTest Array.empty
    |> Result.mapError(fun x -> failwith x)
    |> ignore

    let pluginStats = _nodeStats.[0].PluginStats.[0]
    let table1 = pluginStats.Tables.["PluginStatistics1Table"]
    let table2 = pluginStats.Tables.["PluginStatistics2Table"]

    // assert on IReportingSink
    test <@ table1.Columns.Count > 0 @>
    test <@ table1.Rows.Count > 0 @>
    test <@ table2.Columns.Count > 0 @>
    test <@ table2.Rows.Count > 0 @>
