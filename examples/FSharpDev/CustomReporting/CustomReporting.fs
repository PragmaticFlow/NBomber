module FSharpDev.CustomReportingExample

open System.Threading.Tasks
open FSharp.Control.Tasks.NonAffine

open NBomber.Configuration
open NBomber.Contracts
open NBomber.FSharp

let reportingSink = {
    new IReportingSink with
        member _.SinkName = "TestSink"
        member _.Init(context, infraConfig) = Task.CompletedTask
        member _.Start() = Task.CompletedTask
        member _.SaveStats(stats) = Task.CompletedTask
        member _.SaveReports(files) = Task.CompletedTask
        member _.Stop() = Task.CompletedTask
        member _.Dispose() = ()
}

let run () =

    let step = Step.create("step", fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.Ok(sizeBytes = 100)
    })

    Scenario.create "simple_scenario" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = minutes 1)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withTestSuite "reporting"
    |> NBomberRunner.withTestName "custom_reporting_test"
    |> NBomberRunner.withReportingSinks [reportingSink] (seconds 10)
    |> NBomberRunner.withReportFolder "./custom_reports"
    |> NBomberRunner.withReportFormats [ReportFormat.Html; ReportFormat.Md]
    |> NBomberRunner.run
    |> ignore
