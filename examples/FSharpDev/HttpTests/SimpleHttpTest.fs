module FSharpDev.HttpTests.SimpleHttpTest

open System.Net.Http
open FSharp.Control.Tasks.NonAffine
open NBomber.Contracts
open NBomber.FSharp
open NBomber.FSharp.SyncApi

let run () =

    use httpClient = new HttpClient()



    let step = Step.create("fetch_html_page", fun context -> task {

        let! response = httpClient.GetAsync("https://nbomber.com")

        return if response.IsSuccessStatusCode then Response.ok()
               else Response.fail()
    })

    Scenario.create "simple_http" [step]
    |> Scenario.withWarmUpDuration(seconds 5)
    |> Scenario.withLoadSimulations [InjectPerSec(rate = 20, during = seconds 30)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.run
    |> ignore
