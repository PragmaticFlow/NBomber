open System
open NBomber.FSharp

[<EntryPoint>]
let main argv =
    
    //HttpScenario.buildScenario()
    HelloWorldScenario.buildScenario()
    |> Scenario.withConcurrentCopies(10)
    |> Scenario.withDuration(TimeSpan.FromSeconds(5.0))    
    |> NBomberRunner.registerScenario
    // |> NBomberRunner.registerScenarios
    // |> NBomberRunner.loadConfig "config.json"
    |> NBomberRunner.runInConsole

    0
