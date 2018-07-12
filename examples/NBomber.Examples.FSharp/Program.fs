open NBomber
open NBomber.Examples.FSharp.Scenarios

[<EntryPoint>]
let main argv =
    
    HttpScenario.buildScenario() 
    |> ScenarioRunner.run
    
    0 // return an integer exit code