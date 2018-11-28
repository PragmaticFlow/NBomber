open System
open System.Net.Http

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

let buildScenario () =

    let createHttpRequest () =
        let msg = new HttpRequestMessage()
        msg.RequestUri <- Uri("https://github.com/PragmaticFlow/NBomber")
        msg.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8") |> ignore
        msg.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36") |> ignore
        msg
    
    let httpClient = new HttpClient()

    let step1 = Step.createRequest("GET html", fun _ -> task {        
        let! response = createHttpRequest() |> httpClient.SendAsync
        return if response.IsSuccessStatusCode then Response.Ok()
               else Response.Fail(response.StatusCode.ToString()) 
    })
        
    Scenario.create("test github", [step1])

[<EntryPoint>]
let main argv =    
    
    let scenario = buildScenario()
    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.loadConfig "config.json"
    |> NBomberRunner.runInConsole

    0 // return an integer exit code