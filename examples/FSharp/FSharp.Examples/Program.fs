open System
open NBomber.FSharp
open NBomber.Contracts

[<EntryPoint>]
let main argv =
    
    //HttpScenario.buildScenario()
    HelloWorldScenario.buildScenario()
    |> Scenario.withConcurrentCopies(10)
    |> Scenario.withDuration(TimeSpan.FromSeconds(5.0))    
    |> NBomberRunner.registerScenario
    // |> NBomberRunner.registerScenarios
    // |> NBomberRunner.loadConfig "config.json"
    // |> NBomberRunner.withOutputFilename "custom_report_name"
    // |> NBomberRunner.withOutputFileTypes [| FileType.Csv |]
    |> NBomberRunner.runInConsole

    0
