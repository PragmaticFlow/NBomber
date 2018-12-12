open System
open NBomber.FSharp

[<EntryPoint>]
let main argv =
    
    FSharpHttpScenario.buildScenario()
    |> Scenario.withConcurrentCopies(50)
    |> Scenario.withDuration(TimeSpan.FromSeconds(5.0))    
    |> NBomberRunner.registerScenario
    // |> NBomberRunner.registerScenarios
    // |> NBomberRunner.loadConfig "config.json"
    |> NBomberRunner.runInConsole

    0
