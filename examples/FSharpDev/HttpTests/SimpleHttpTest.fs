module FSharpDev.HttpTests.SimpleHttpTest

open System.Net.Http
open FSharp.Control.Tasks.NonAffine
open NBomber.Contracts
open NBomber.FSharp

let run () =

    use httpClient = new HttpClient()

    let step = Step.create("fetch_html_page", fun context -> task {

        let! response = httpClient.GetAsync("https://nbomber.com", context.CancellationToken)

        return if response.IsSuccessStatusCode then Response.ok(statusCode = int response.StatusCode)
               else Response.fail(statusCode = int response.StatusCode)
    })

    Scenario.create "simple_http" [step]
    |> Scenario.withWarmUpDuration(seconds 5)
    |> Scenario.withLoadSimulations [InjectPerSec(rate = 100, during = seconds 30)]
    |> Scenario.withStepTimeout(milliseconds 500)
    |> NBomberRunner.registerScenario
    |> NBomberRunner.run
    |> ignore
