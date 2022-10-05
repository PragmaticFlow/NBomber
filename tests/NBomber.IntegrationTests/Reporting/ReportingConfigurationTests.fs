module Tests.Reporting.ReportingConfiguration

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

[<Fact>]
let ``JSON config settings for ReportFileName and ReportFolder should be properly handled`` () =

    // test_config_2.json contains
    // "ReportFileName": "custom_report_name",
    // "ReportFolder": "./my_custom_reports",
    // "ReportFormats": ["Html", "Txt"],

    // delete all directories with all files
    if Directory.Exists "./my_custom_reports" then
        Directory.Delete("./my_custom_reports", recursive = true)

    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(seconds 1)
        return Response.ok()
    })

    let scenario =
        Scenario.create "test" [okStep]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 5, during = seconds 5)]

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.loadConfig("./Configuration/test_config_2.json")
    |> NBomberRunner.run
    |> ignore

    let dirExist = Directory.Exists "./my_custom_reports"
    let files = Directory.GetFiles("./my_custom_reports", searchPattern = "*.*", searchOption = SearchOption.TopDirectoryOnly)

    test <@ dirExist @>
    test <@ files.Length = 3 @> // here we check that only 2 report formats were generated + txt 1 log file

    files
    |> Seq.map(FileInfo)
    |> Seq.iter(fun file ->
        test <@ file.Name.Contains "custom_report_name" || file.Name.Contains "nbomber-log" @>
        test <@ [".html"; ".txt"] |> Seq.contains file.Extension  @>
        test <@ file.Length > 0L  @>
    )

[<Fact>]
let ``withReportFileName and withReportFolder should be properly handled`` () =

    // delete all directories with all files
    if Directory.Exists "./my_reports_2" then
        Directory.Delete("./my_reports_2", recursive = true)

    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(seconds 1)
        return Response.ok()
    })

    let scenario =
        Scenario.create "test" [okStep]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 5, during = seconds 5)]

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.withReportFileName "custom_report_name"
    |> NBomberRunner.withReportFolder "./my_reports_2"
    |> NBomberRunner.run
    |> ignore

    let dirExist = Directory.Exists "./my_reports_2"
    let files = Directory.GetFiles("./my_reports_2", searchPattern = "*.*", searchOption = SearchOption.TopDirectoryOnly)

    test <@ dirExist @>
    test <@ files.Length = 5 @> // here we check that all report formats were generated

    files
    |> Seq.map(FileInfo)
    |> Seq.iter(fun file ->
        test <@ file.Name.Contains "custom_report_name" || file.Name.Contains "nbomber-log" @>
        test <@ [".html"; ".csv"; ".txt"; ".md"] |> Seq.contains file.Extension  @>
        test <@ file.Length > 0L  @>
    )

[<Fact>]
let ``withoutReports should not print report files`` () =

    let inMemorySink = new InMemorySink()

    let createLoggerConfig = fun () ->
        LoggerConfiguration()
            .MinimumLevel.Is(LogEventLevel.Verbose)
            .WriteTo.Sink(inMemorySink)

    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(seconds 1)
        return Response.ok()
    })

    let scenario =
        Scenario.create "test" [okStep]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 5, during = seconds 5)]

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.withLoggerConfig createLoggerConfig
    |> NBomberRunner.run
    |> ignore

    let logEvents = inMemorySink.LogEvents |> Seq.toList
    let reportsBuilt = logEvents |> List.exists(fun x -> x.MessageTemplate.Text = "Report.build")
    let txtReportPrinted = logEvents |> List.exists(fun x -> x.MessageTemplate.Text = "TxtReport.print")
    let csvReportPrinted = logEvents |> List.exists(fun x -> x.MessageTemplate.Text = "CsvReport.print")
    let htmlReportPrinted = logEvents |> List.exists(fun x -> x.MessageTemplate.Text = "HtmlReport.print")
    let mdReportPrinted = logEvents |> List.exists(fun x -> x.MessageTemplate.Text = "MdReport.print")
    let consoleReportPrinted = logEvents |> List.exists(fun x -> x.MessageTemplate.Text = "ConsoleReport.print")

    test <@ reportsBuilt = true @>
    test <@ txtReportPrinted = false @>
    test <@ csvReportPrinted = false @>
    test <@ htmlReportPrinted = false @>
    test <@ mdReportPrinted = false @>
    test <@ consoleReportPrinted = true @>

[<Fact>]
let ``withoutReports should not save report files`` () =

    // delete all directories with all files
    if Directory.Exists "./no-reports/1" then
        Directory.Delete("./no-reports/1", recursive = true)

    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(seconds 1)
        return Response.ok()
    })

    let scenario =
        Scenario.create "test" [okStep]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 5, during = seconds 5)]

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.withReportFolder "./no-reports/1"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        test <@ stats.ReportFiles.Length = 0 @>

    let dirExist = Directory.Exists "./no-reports/1"
    let files = Directory.GetFiles("./no-reports/1", searchPattern = "*.*", searchOption = SearchOption.AllDirectories)

    test <@ dirExist @>
    test <@ files.Length = 1 @> // here we check that all report formats were generated
    test <@ FileInfo(files[0]).Name.Contains "nbomber-log" @>

