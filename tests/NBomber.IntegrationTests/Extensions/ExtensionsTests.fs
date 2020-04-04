module Tests.ExtensionsTests

open System
open System.Data
open System.Threading.Tasks

open Swensen.Unquote
open Xunit
open FSharp.Control.Tasks.V2.ContextInsensitive

open System.IO
open NBomber.Contracts
open NBomber.Domain
open NBomber.Errors
open NBomber.Extensions
open NBomber.FSharp

module ExtensionsTestHelper =

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
            Scenario.create "extension scenario 1" [step1; step2]
            |> Scenario.withOutWarmUp
            |> Scenario.withLoadSimulations [
                KeepConcurrentScenarios(copiesCount = 2, during = TimeSpan.FromSeconds 3.0)
            ]

        let scenario2 =
            Scenario.create "extension scenario 2" [step3]
            |> Scenario.withOutWarmUp
            |> Scenario.withLoadSimulations [
                KeepConcurrentScenarios(copiesCount = 2, during = TimeSpan.FromSeconds 3.0)
            ]

        [scenario1; scenario2]

    let createCustomStatistics () =
        let colKey = new DataColumn("Key", Type.GetType("System.String"))
        colKey.Caption <- "CustomStatisticsColumnKey"

        let colValue = new DataColumn("Value", Type.GetType("System.String"))
        colValue.Caption <- "CustomStatisticsColumnValue"

        let colType = new DataColumn("Type", Type.GetType("System.String"))
        colType.Caption <- "CustomStatisticsColumnType"

        let table = new DataTable("CustomStatisticsTable")
        table.Columns.Add(colKey);
        table.Columns.Add(colValue);
        table.Columns.Add(colType);

        for i in 1 .. 10 do
            let row = table.NewRow()
            row.["Key"] <- sprintf "CustomStatisticsRowKey%i" i
            row.["Value"] <- sprintf "CustomStatisticsRowValue%i" i
            row.["Type"] <- sprintf "CustomStatisticsRowType%i" i
            table.Rows.Add(row)

        let ds = new DataSet()
        ds.Tables.Add(table)

        { Data = ds }

[<Fact>]
let ``Nbomber with no extensions should return an empty list of custom statistics`` () =

    let scenarios = ExtensionsTestHelper.createScenarios()

    let result =
        NBomberRunner.registerScenarios scenarios
        |> NBomberRunner.runWithResult
        |> Result.mapError(fun x -> x |> AppError.toString |> failwith)
        |> Result.getOk

    test <@ result.CustomStatistics.Length = 0 @>

[<Fact>]
let ``IExtension.Init should be invoked once`` () =

    let scenarios = ExtensionsTestHelper.createScenarios()
    let mutable extensionInitInvokedCounter = 0

    let extension = { new IExtension with
                            member x.Init(_, _) = extensionInitInvokedCounter <- extensionInitInvokedCounter + 1
                            member x.StartTest(_) = Task.CompletedTask
                            member x.FinishTest(_) = Task.FromResult(CustomStatistics.Create()) }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.withExtensions([extension])
    |> NBomberRunner.runTest
    |> Result.mapError(fun x -> failwith x)
    |> ignore

    test <@ extensionInitInvokedCounter = 1 @>

[<Fact>]
let ``IExtension.StartTest should be invoked once`` () =

    let scenarios = ExtensionsTestHelper.createScenarios()
    let mutable extensionStartTestInvokedCounter = 0

    let extension = { new IExtension with
                            member x.Init(_, _) = ()
                            member x.StartTest(_) =
                                extensionStartTestInvokedCounter <- extensionStartTestInvokedCounter + 1
                                Task.CompletedTask
                            member x.FinishTest(_) = Task.FromResult(CustomStatistics.Create()) }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.withExtensions([extension])
    |> NBomberRunner.runTest
    |> Result.mapError(fun x -> failwith x)
    |> ignore

    test <@ extensionStartTestInvokedCounter = 1 @>

[<Fact>]
let ``IExtension.FinishTest should be invoked once`` () =

    let scenarios = ExtensionsTestHelper.createScenarios()
    let mutable extensionFinishTestInvokedCounter = 0

    let extension = { new IExtension with
                            member x.Init(_, _) = ()
                            member x.StartTest(_) = Task.CompletedTask
                            member x.FinishTest(_) =
                                extensionFinishTestInvokedCounter <- extensionFinishTestInvokedCounter + 1
                                Task.FromResult(CustomStatistics.Create()) }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.withExtensions([extension])
    |> NBomberRunner.runTest
    |> Result.mapError(fun x -> failwith x)
    |> ignore

    test <@ extensionFinishTestInvokedCounter = 1 @>

[<Fact>]
let ``IExtension.FinishTest should pass custom statistics to ReportingSink`` () =

    let scenarios = ExtensionsTestHelper.createScenarios()
    let mutable reportingSinkCustomStats = [| CustomStatistics.Create() |]

    let extension = { new IExtension with
                            member x.Init(_, _) = ()
                            member x.StartTest(_) = Task.CompletedTask
                            member x.FinishTest(_) =
                                let customStatistics = ExtensionsTestHelper.createCustomStatistics()
                                Task.FromResult(customStatistics) }

    let reportingSink = { new IReportingSink with
                        member x.Init(_, _) = ()
                        member x.StartTest(_) = Task.CompletedTask
                        member x.SaveRealtimeStats(_, _) = Task.CompletedTask
                        member x.SaveFinalStats(_, _, customStats, _) =
                            reportingSinkCustomStats <- customStats
                            Task.CompletedTask
                        member x.FinishTest(_) = Task.CompletedTask }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.withReportingSinks([reportingSink], TimeSpan.FromSeconds 5.0)
    |> NBomberRunner.withExtensions([extension])
    |> NBomberRunner.runTest
    |> Result.mapError(fun x -> failwith x)
    |> ignore

    let customStats = reportingSinkCustomStats.[0]
    let table = customStats.Data.Tables.["CustomStatisticsTable"]

    test <@ table.Columns.Count > 0 @>
    test <@ table.Rows.Count > 0 @>


[<Fact>]
let ``IExtension.FinishTest should save custom statistics into reports`` () =

    let scenarios = ExtensionsTestHelper.createScenarios()
    let mutable reportingSinkReportFiles = Array.empty

    let extension = { new IExtension with
                            member x.Init(_, _) = ()
                            member x.StartTest(_) = Task.CompletedTask
                            member x.FinishTest(_) =
                                let customStatistics = ExtensionsTestHelper.createCustomStatistics()
                                Task.FromResult(customStatistics) }

    let reportingSink = { new IReportingSink with
                        member x.Init(_, _) = ()
                        member x.StartTest(_) = Task.CompletedTask
                        member x.SaveRealtimeStats(_, _) = Task.CompletedTask
                        member x.SaveFinalStats(_, _, _, reportFiles) =
                            reportingSinkReportFiles <- reportFiles
                            Task.CompletedTask
                        member x.FinishTest(_) = Task.CompletedTask }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.withReportingSinks([reportingSink], TimeSpan.FromSeconds 5.0)
    |> NBomberRunner.withExtensions([extension])
    |> NBomberRunner.runTest
    |> Result.mapError(fun x -> failwith x)
    |> ignore

    reportingSinkReportFiles
    |> Array.map(fun x -> x.FilePath |> File.ReadAllText)
    |> Array.iter(fun x -> test <@ x.Contains("CustomStatisticsColumn") @>
                           test <@ x.Contains("CustomStatisticsRow") @>)
