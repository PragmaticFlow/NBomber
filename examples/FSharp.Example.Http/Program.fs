open NBomber.FSharp

[<EntryPoint>]
let main argv =    
    
    HttpScenario.buildScenario() 
    |> Scenario.run

    0 // return an integer exit code