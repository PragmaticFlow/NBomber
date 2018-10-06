# Testing HTTP via Pull Scenario

```fsharp
open System
open System.Net.Http

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

let buildScenario () =

    let createHttpRequest () =
        let msg = new HttpRequestMessage()
        msg.RequestUri <- Uri("https://github.com/PragmaticFlow/NBomber")
        msg.Headers.TryAddWithoutValidation("Accept", "text/html") |> ignore        
        msg
    
    let httpClient = new HttpClient()

    let step1 = Step.createRequest("GET html", fun _ -> task {        
        let! response = createHttpRequest() |> httpClient.SendAsync
        return if response.IsSuccessStatusCode then Response.Ok()
               else Response.Fail(response.StatusCode.ToString()) 
    })

    let testFlow = { FlowName = "GET flow"
                     Steps = [step1]
                     ConcurrentCopies = 100 }

    Scenario.create("Test HTTP https://github.com")
    |> Scenario.addTestFlow(flow)
    |> Scenario.withDuration(TimeSpan.FromSeconds(10.0))

[<EntryPoint>]
let main argv =    
    
    buildScenario() 
    |> Scenario.runInConsole

    0 // return an integer exit code
```