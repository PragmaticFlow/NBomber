open NBomber
open NBomber.Examples.FSharp.Scenarios

[<EntryPoint>]
let main argv =
    
    HttpScenario.buildScenario() 
    |> ScenarioRunner.Run
    
    0 // return an integer exit code