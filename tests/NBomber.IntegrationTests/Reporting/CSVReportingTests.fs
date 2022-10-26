module Tests.Reporting.CSVReporting

open System.IO
open System.Threading.Tasks

open Serilog
open Serilog.Events
open Serilog.Sinks.InMemory
open Swensen.Unquote
open Xunit

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Domain
open NBomber.Extensions.Internal
open NBomber.FSharp

// This test might fail if you are using a region on your computer that uses, as a decimal sign
[<Fact>]
let ``CSV report should be formatted properly`` () =

    // delete all directories with all files
    if Directory.Exists "./my_csv_reports" then
        Directory.Delete("./my_csv_reports", recursive = true)

    let scenario =
        Scenario.create("test1", fun ctx -> task {

            let! step1 = Step.run("ok step 1", ctx, fun () -> task {
                do! Task.Delay(seconds 1)
                return Response.ok()
            })

            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 5, during = seconds 5)]

    let scenario2 =
        Scenario.create("test2", fun ctx -> task {

            let! step2 = Step.run("ok step 2", ctx, fun () -> task {
                do! Task.Delay(seconds 1)
                return Response.ok()
            })

            let! step3 = Step.run("ok step 3", ctx, fun () -> task {
                do! Task.Delay(seconds 1)
                return Response.ok()
            })

            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 5, during = seconds 5)]

    NBomberRunner.registerScenarios [scenario; scenario2]
    |> NBomberRunner.withReportFolder "./my_csv_reports"
    |> NBomberRunner.withReportFormats [ReportFormat.Csv]
    |> NBomberRunner.run
    |> ignore

    let files = Directory.GetFiles("./my_csv_reports", searchPattern = "*.*", searchOption = SearchOption.TopDirectoryOnly)

    let csvRows =
        files
        |> Seq.map FileInfo
        |> Seq.find(fun s -> [".csv"] |> Seq.contains s.Extension)
        |> fun file -> file.FullName
        |> File.ReadAllLines

    test <@ csvRows.Length = 6 @>

    csvRows
    |> Seq.map(String.split [|","|])
    |> Seq.map Array.length
    |> fun a -> test<@ Seq.max a = Seq.min a @> // compare columns count
