module rec NBomber.Examples.FSharp.Scenarios.HttpScenario

open System
open System.Threading.Tasks
open System.Net.Http

open FSharp.Control.Tasks.V2.ContextInsensitive
open NBomber
open NBomber.FSharp

let private createRequest () =
    let msg = new HttpRequestMessage()
    msg.RequestUri <- Uri("https://github.com/VIP-Logic/NBomber")
    msg.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8") |> ignore
    msg.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36") |> ignore
    msg
    
let private httpClient = new HttpClient()

let private getGithubStep = 
    Step.request("GET github.com/VIP-Logic/NBomber html",
        fun _ -> task { let! response = createRequest() |> httpClient.SendAsync
                        return if response.IsSuccessStatusCode then Response.Ok()
                               else Response.Fail(response.StatusCode.ToString()) })

//let private asserts = [|
//    All(Assertion(fun stats -> stats.OkCount > 95))
//    All(Assertion(fun stats -> stats.FailCount > 95))
//    All(Assertion(fun stats -> stats.OkCount > 0))
//    All(Assertion(fun stats -> stats.OkCount > 0))
//    Flow("GET flow", Assertion(fun stats -> stats.FailCount < 10))
//    Step("GET flow", "GET github.com/VIP-Logic/NBomber html", Assertion(fun stats -> stats.OkCount = 95))
//    Step("GET flow", "GET github.com/VIP-Logic/NBomber html", Assertion(fun stats -> Seq.exists (fun i -> i = stats.FailCount) [80;95]))
//    Step("GET flow", "GET github.com/VIP-Logic/NBomber html", Assertion(fun stats -> stats.OkCount > 80 && stats.OkCount > 95))
//|]

let buildScenario () =

    let concurrentCopies = 100
    let duration = TimeSpan.FromSeconds(10.0)

    Scenario.create("Test HTTP (https://github.com) with 100 concurrent users")
    |> Scenario.addTestFlow("GET flow", [getGithubStep], concurrentCopies)
    //|> Scenario.addAsserts(asserts)
    |> Scenario.build(duration)