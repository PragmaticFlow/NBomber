module FSharpDev.HttpTests.SimpleHttpTest

open System.Net.Http
open NBomber
open NBomber.Contracts
open NBomber.Contracts.Thresholds
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
    |> Scenario.withLoadSimulations [ InjectPerSec(rate = 20, during = seconds 30) ]
    |> Scenario.withThresholds [
        RequestCount [
            AllCount(fun x -> x > 290)
            OkCount(fun x -> x > 180)
            FailedCount(fun x -> x <= 10)
            FailedRate(fun x -> x < 0.1)
            RPS(fun x -> x > 15.0)
        ]
        Latency [
            Min(fun x -> x < 100)
            Mean(fun x -> x < 200)
            Max(fun x -> x < 700)
            StdDev(fun x -> x > 50 && x < 100)
        ]
        LatencyPercentile [
            P50(fun x -> x < 200)
            P75(fun x -> x < 250)
            P95(fun x -> x < 400)
            P99(fun x -> x < 400)
        ]
    ]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.run
    |> ignore
