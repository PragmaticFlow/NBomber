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
    |> Scenario.withThresholds(
        thresholds {
            request_all_count (fun x -> x > 1290) "request all count > 290"
            request_ok_count (fun x -> x > 180)
            request_failed_count (fun x -> x <= 10)
            request_failed_rate (fun x -> x > 0.1)
            rps (fun x -> x > 15.0)
            latency_min (fun x -> x < 100)
            latency_mean (fun x -> x < 200)
            latency_max (fun x -> x < 700)
            latency_std_dev (fun x -> x > 50 && x < 100) "latency standard deviation > 50 and < 100"
            latency_p50 (fun x -> x < 200)
            latency_p75 (fun x -> x < 250)
            latency_p95 (fun x -> x < 400)
            latency_p99 (fun x -> x < 400)
            data_transfer_all_bytes (fun x -> x < 10000)
        }
    )
//    |> Scenario.withThresholds [
//        RequestAllCount((fun x -> x > 1290), Some "request all count > 290")
//        RequestOkCount((fun x -> x > 180), None)
//    ]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.run
    |> ignore
