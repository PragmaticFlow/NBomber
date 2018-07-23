open NBomber
open NBomber.FSharp
open NBomber.Examples.FSharp.Scenarios

[<EntryPoint>]
let main argv =
    
    HttpScenario.buildScenario() 
    |> Scenario.run
    
    0 // return an integer exit code