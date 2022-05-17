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
        RequestAllCount(fun x -> x > 290)
        RequestOkCount(fun x -> x > 180)
        RequestFailedCount(fun x -> x <= 10)
        RequestFailedRate(fun x -> x < 0.1)
        RPS(fun x -> x > 15.0)
        LatencyMin(fun x -> x < 100)
        LatencyMean(fun x -> x < 200)
        LatencyMax(fun x -> x < 700)
        LatencyStdDev(fun x -> x > 50 && x < 100)
        LatencyP50(fun x -> x < 200)
        LatencyP75(fun x -> x < 250)
        LatencyP95(fun x -> x < 400)
        LatencyP99(fun x -> x < 400)
    ]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.run
    |> ignore
