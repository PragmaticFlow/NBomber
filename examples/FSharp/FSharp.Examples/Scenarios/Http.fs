module HttpScenario

open System
open System.Net.Http

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

let buildScenario () =

    let createHttpRequest () =
        let msg = new HttpRequestMessage()
        msg.RequestUri <- Uri("https://nbomber.com")
        msg.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8") |> ignore
        msg.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36") |> ignore
        msg

    let httpClient = new HttpClient()

    let step1 = Step.create("GET html", ConnectionPool.none, fun context -> task {        
        let! response = httpClient.SendAsync(createHttpRequest(), context.CancellationToken)        
        let responseSize =
            if response.Content.Headers.ContentLength.HasValue then 
               response.Content.Headers.ContentLength.Value |> Convert.ToInt32
            else
               0

        return if response.IsSuccessStatusCode then Response.Ok(sizeBytes = responseSize)
               else Response.Fail() 
    })
        
    Scenario.create "nbomber.com" [step1]