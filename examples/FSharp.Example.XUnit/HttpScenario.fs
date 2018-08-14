module rec HttpScenario

open System
open System.Threading.Tasks
open System.Net.Http

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

let private createRequest () =
    let msg = new HttpRequestMessage()
    msg.RequestUri <- Uri("https://github.com/VIP-Logic/NBomber")
    msg.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8") |> ignore
    msg.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36") |> ignore
    msg
    
let private httpClient = new HttpClient()

let private step1 = 
    Step.createRequest("GET github.com/VIP-Logic/NBomber html",
        fun _ -> task { let! response = createRequest() |> httpClient.SendAsync
                        return if response.IsSuccessStatusCode then Response.Ok()
                               else Response.Fail(response.StatusCode.ToString()) })

let buildScenario () =
    Scenario.create("Test HTTP (https://github.com) with 100 concurrent users")
    |> Scenario.addTestFlow({ FlowName = "GET flow"; Steps = [step1]; ConcurrentCopies = 100 })   
    |> Scenario.withDuration(TimeSpan.FromSeconds(10.0))
