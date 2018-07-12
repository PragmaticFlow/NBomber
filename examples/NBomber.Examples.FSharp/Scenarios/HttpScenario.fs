module rec NBomber.Examples.FSharp.Scenarios.HttpScenario

open System
open System.Threading.Tasks
open System.Net.Http

open FSharp.Control.Tasks.V2.ContextInsensitive
open NBomber
open NBomber.FSharpAPI

let private createRequest () =
    let msg = new HttpRequestMessage()
    msg.RequestUri <- Uri("https://github.com/VIP-Logic/NBomber")
    msg.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8") |> ignore
    msg.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36") |> ignore
    msg
    
let private httpClient = new HttpClient()

let private getGithubStep = 
    Step.Create("GET github.com/VIP-Logic/NBomber html",
                fun () -> task { let! response = createRequest() |> httpClient.SendAsync                                 
                                 return if response.IsSuccessStatusCode then StepResult.Ok
                                        else StepResult.Fail })

let buildScenario () =
    scenario("test HTTP (https://github.com) with 100 concurrent users")
    |> addTestFlow({ FlowName = "GET flow"; Steps = [|getGithubStep|]; ConcurrentCopies = 100 })
    |> build(TimeSpan.FromSeconds(10.0))