open System
open NBomber.FSharp

[<EntryPoint>]
let main argv =

    HttpScenario.buildScenario()
    //HelloWorldScenario.buildScenario()
    |> Scenario.withConcurrentCopies 10
    //|> Scenario.withWarmUpDuration(TimeSpan.FromSeconds 10.0)
    |> Scenario.withDuration(TimeSpan.FromSeconds 5.0)
    |> NBomberRunner.registerScenario
    // |> NBomberRunner.registerScenarios
    // |> NBomberRunner.loadConfig "config.json"
    // |> NBomberRunner.withReportFileName "custom_report_name"
    // |> NBomberRunner.withReportFormats [ReportFormat.Txt; ReportFormat.Html; ReportFormat.Csv]
    |> NBomberRunner.runInConsole

    0
