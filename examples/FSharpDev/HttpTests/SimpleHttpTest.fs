module FSharpDev.HttpTests.SimpleHttpTest

open System.Net.Http
open FSharp.Control.Tasks.NonAffine

open NBomber
open NBomber.Contracts
open NBomber.FSharp

let run () =

    use httpClient = new HttpClient()

    let step = Step.create("fetch_html_page", fun context -> task {

        let! response = httpClient.GetAsync("https://nbomber.com", context.CancellationToken)

        return
            if response.IsSuccessStatusCode then
                Response.ok(statusCode = int response.StatusCode, message = "test ok message")
            else
                Response.fail(statusCode = int response.StatusCode, error = "test error message")
    })

    Scenario.create "simple_http" [step]
    |> Scenario.withWarmUpDuration(seconds 5)
    |> Scenario.withLoadSimulations [InjectPerSec(rate = 20, during = seconds 30)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.run
    |> ignore
