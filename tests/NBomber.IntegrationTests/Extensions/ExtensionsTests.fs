module Tests.ExtensionsTests

open System
open System.Data
open System.IO
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Swensen.Unquote
open Xunit

open NBomber.Configuration
open NBomber.Contracts
open NBomber.Domain
open NBomber.Errors
open NBomber.Extensions
open NBomber.FSharp

module internal ExtensionsTestHelper =

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
            |> Scenario.withoutWarmUp
            |> Scenario.withLoadSimulations [
                KeepConcurrentScenarios(copiesCount = 2, during = TimeSpan.FromSeconds 3.0)
            ]

        let scenario2 =
            Scenario.create "extension scenario 2" [step3]
            |> Scenario.withoutWarmUp
            |> Scenario.withLoadSimulations [
                KeepConcurrentScenarios(copiesCount = 2, during = TimeSpan.FromSeconds 3.0)
            ]

        [scenario1; scenario2]

  module internal CustomStatisticsHelper =

    let private getCustomStatisticsColumns (prefix: string) =
        let colKey = new DataColumn("Key", Type.GetType("System.String"))
        colKey.Caption <- sprintf "%sColumnKey" prefix

        let colValue = new DataColumn("Value", Type.GetType("System.String"))
        colValue.Caption <- sprintf "%sColumnValue" prefix

        let colType = new DataColumn("Type", Type.GetType("System.String"))
        colType.Caption <- sprintf "%sColumnType" prefix

        [| colKey; colValue; colType |]

    let private getCustomStatisticsRows (count: int) (prefix: string) (table: DataTable) = [|
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
        |> getCustomStatisticsColumns
        |> table.Columns.AddRange

        table
        |> getCustomStatisticsRows 10 prefix
        |> Array.iter(fun x -> x |> table.Rows.Add)

        table

    let createCustomStatistics () =
        let customStats = new CustomStatistics()
        customStats.Tables.Add(createTable("CustomStatistics1"))
        customStats.Tables.Add(createTable("CustomStatistics2"))
        customStats

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
                            member x.FinishTest(_) = Task.FromResult(new CustomStatistics()) }

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
                            member x.FinishTest(_) = Task.FromResult(new CustomStatistics()) }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.withExtensions([extension])
    |> NBomberRunner.runTest
    |> Result.mapError(fun x -> failwith x)
    |> ignore

    test <@ extensionStartTestInvokedCounter = 1 @>


[<Fact>]
let ``IExtension.StartTest should be invoked with infra config`` () =

    let scenarios = ExtensionsTestHelper.createScenarios()
    let mutable extensionConfig = None

    let extension = { new IExtension with
                            member x.Init(logger, infraConfig) =
                                extensionConfig <- infraConfig
                                ()
                            member x.StartTest(_) = Task.CompletedTask
                            member x.FinishTest(_) = Task.FromResult(new CustomStatistics()) }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.loadInfraConfigYaml "Configuration/infra_config.yaml"
    |> NBomberRunner.withExtensions([extension])
    |> NBomberRunner.runTest
    |> Result.mapError(fun x -> failwith x)
    |> ignore

    test <@ extensionConfig.IsSome @>

[<Fact>]
let ``IExtension.FinishTest should be invoked once`` () =

    let scenarios = ExtensionsTestHelper.createScenarios()
    let mutable extensionFinishTestInvokedCounter = 0

    let extension = { new IExtension with
                            member x.Init(_, _) = ()
                            member x.StartTest(_) = Task.CompletedTask
                            member x.FinishTest(_) =
                                extensionFinishTestInvokedCounter <- extensionFinishTestInvokedCounter + 1
                                Task.FromResult(new CustomStatistics()) }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.withExtensions([extension])
    |> NBomberRunner.runTest
    |> Result.mapError(fun x -> failwith x)
    |> ignore

    test <@ extensionFinishTestInvokedCounter = 1 @>

[<Fact>]
let ``IExtension.FinishTest should pass custom statistics to ReportingSink`` () =

    let scenarios = ExtensionsTestHelper.createScenarios()
    let mutable reportingSinkCustomStats = [| new CustomStatistics() |]

    let extension = { new IExtension with
                            member x.Init(_, _) = ()
                            member x.StartTest(_) = Task.CompletedTask
                            member x.FinishTest(_) =
                                let customStatistics = CustomStatisticsHelper.createCustomStatistics()
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
    let table1 = customStats.Tables.["CustomStatistics1Table"]
    let table2 = customStats.Tables.["CustomStatistics2Table"]

    test <@ table1.Columns.Count > 0 @>
    test <@ table1.Rows.Count > 0 @>
    test <@ table2.Columns.Count > 0 @>
    test <@ table2.Rows.Count > 0 @>

[<Fact>]
let ``IExtension.FinishTest should save custom statistics into .txt report`` () =

    let scenarios = ExtensionsTestHelper.createScenarios()
    let mutable reportingSinkReportFiles = Array.empty

    let extension = { new IExtension with
                            member x.Init(_, _) = ()
                            member x.StartTest(_) = Task.CompletedTask
                            member x.FinishTest(_) =
                                let customStatistics = CustomStatisticsHelper.createCustomStatistics()
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
    |> Array.filter(fun x -> [ ReportFormat.Txt ] |> List.contains x.ReportFormat)
    |> Array.map(fun x -> x.FilePath |> File.ReadAllText)
    |> Array.iter(fun x -> test <@ x.Contains("CustomStatistics1Column") @>
                           test <@ x.Contains("CustomStatistics1Row") @>
                           test <@ x.Contains("CustomStatistics2Column") @>
                           test <@ x.Contains("CustomStatistics2Row") @>)
