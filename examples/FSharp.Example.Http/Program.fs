open System
open System.Net.Http

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

let buildScenario () =

    let createRequest () =
        let msg = new HttpRequestMessage()
        msg.RequestUri <- Uri("https://github.com/PragmaticFlow/NBomber")
        msg.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8") |> ignore
        msg.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36") |> ignore
        msg
    
    let httpClient = new HttpClient()

    let step1 = Step.createRequest("GET html", fun _ -> task {        
        let! response = createRequest() |> httpClient.SendAsync
        return if response.IsSuccessStatusCode then Response.Ok()
               else Response.Fail(response.StatusCode.ToString()) 
    })

    Scenario.create("Test HTTP (https://github.com) with 100 concurrent users")
    |> Scenario.addTestFlow({ FlowName = "GET flow"; Steps = [step1]; ConcurrentCopies = 100 })   
    |> Scenario.withDuration(TimeSpan.FromSeconds(10.0))

[<EntryPoint>]
let main argv =    
    
    buildScenario() 
    |> Scenario.runInConsole

    0 // return an integer exit code