module Tests.CSVReporting

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

//This test might fail if you are using a region on your computer that uses , as a decimal sign
[<Fact>]
let ``CSV report should be formatted properly`` () =

    // delete all directories with all files
    if Directory.Exists "./my_csv_reports" then
        Directory.Delete("./my_csv_reports", recursive = true)

    let ok1Step = Step.create("ok step 1", fun _ -> task {
        do! Task.Delay(seconds 1)
        return Response.ok()
    })

    let ok2Step = Step.create("ok step 2", fun _ -> task {
        do! Task.Delay(seconds 1)
        return Response.ok()
    })

    let ok3Step = Step.create("ok step 3", fun _ -> task {
        do! Task.Delay(seconds 1)
        return Response.ok()
    })

    let scenario =
        Scenario.create "test1" [ok1Step]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 5, during = seconds 5)]

    let scenario2 =
        Scenario.create "test2" [ok2Step;ok3Step]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 5, during = seconds 5)]

    NBomberRunner.registerScenarios [scenario;scenario2]
    |> NBomberRunner.withReportFolder "./my_csv_reports"
    |> NBomberRunner.withReportFormats [ReportFormat.Csv]
    |> NBomberRunner.run
    |> ignore

    let files = Directory.GetFiles("./my_csv_reports", searchPattern = "*.*", searchOption = SearchOption.TopDirectoryOnly)

    let csvRows = 
        files
        |> Seq.map(FileInfo)
        |> Seq.find(fun s -> [".csv"] |> Seq.contains s.Extension)
        |> fun file -> file.FullName
        |> File.ReadAllLines

    test <@ csvRows.Length = 4 @>

    csvRows
        |> Seq.map(String.split [|","|])
        |> Seq.map(Array.length)
        |> fun a -> test<@ Seq.max a = Seq.min a @>



        
