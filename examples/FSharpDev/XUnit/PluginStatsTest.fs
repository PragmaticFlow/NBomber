module FSharpDev.XUnit.PluginStatsTest

open System
open System.Data
open System.Linq
open System.Threading.Tasks

open FSharp.Control.Tasks.NonAffine
open Swensen.Unquote
open Xunit

open NBomber
open NBomber.Contracts
open NBomber.FSharp

// in this example we use:
// - XUnit (https://xunit.net/)
// - Unquote (https://github.com/SwensenSoftware/unquote)
// to get more info about test automation, please visit: (https://nbomber.com/docs/test-automation)

type Plugin1 () =

    let createTableCols () =
        let colKey = new DataColumn("Key", Type.GetType("System.String"))
        colKey.Caption <- "Key"

        let colValue = new DataColumn("Value", Type.GetType("System.String"))
        colValue.Caption <- "Value"

        [| colKey; colValue |]

    let createTableRows (count: int) (table: DataTable) = [|
        for i in 1 .. count do
            let row = table.NewRow()
            row.["Key"] <- sprintf "Key%i" i
            row.["Value"] <- sprintf "Value%i" i
            yield row
       |]

    let createTable (rowsCount) (tableName) =
        let table = new DataTable(tableName)
        table.Columns.AddRange(createTableCols())
        table |> createTableRows rowsCount |> Array.iter(table.Rows.Add)
        table

    let createPluginStats (currentOperation) =
        let pluginStats = new DataSet()
        pluginStats.Tables.Add(createTable 5 "PluginStats")
        Task.FromResult(pluginStats)

    static member TryGetValueForKey(key: string, pluginStats: DataSet) =
        pluginStats.Tables.Item("PluginStats")
        |> fun table -> table.Rows.Cast<DataRow>().ToArray()
        |> Array.tryFind(fun row -> row.["Key"].ToString() = key)
        |> Option.map(fun row -> row.["Value"].ToString())

    interface IWorkerPlugin with
        member _.PluginName = "Plugin1"
        member _.Init(_, _) = Task.CompletedTask
        member _.Start() = Task.CompletedTask
        member _.GetStats(currentOperation) = createPluginStats(currentOperation)
        member _.GetHints() = Array.empty
        member _.Stop() = Task.CompletedTask
        member _.Dispose() = ()

[<Fact>]
let ``Plugin stats test`` () =

    let plugin = new Plugin1()

    let step = Step.createAsync("step_1", fun context -> task {
        do! Task.Delay(seconds 1)
        return Response.ok()
    })

    let nodeStats =
        Scenario.create "scenario_1" [step]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 30)]
        |> NBomberRunner.registerScenario
        |> NBomberRunner.withWorkerPlugins [plugin]
        |> NBomberRunner.withTestSuite "assert_plugin_stats"
        |> NBomberRunner.withTestName "simple_test"
        |> NBomberRunner.run
        |> function
            | Ok stats -> stats
            | Error e -> failwith e

    let pluginStatsValue =
        nodeStats
        |> WorkerPluginStats.tryFindPluginStats plugin
        |> Option.bind(fun pluginStats -> Plugin1.TryGetValueForKey("Key1", pluginStats))
        |> Option.defaultWith(fun () -> failwith "No value was found")

    test <@ pluginStatsValue = "Value1" @>
