module FSharpDev.HttpTests.SimpleHttpTest

open System.Net.Http
open FSharp.Control.Tasks.NonAffine
open NBomber
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Plugins.Network.Ping

// in this example we use:
// - NBomber.Http (https://nbomber.com/docs/plugins-http)

let run () =

    use httpClient = new HttpClient()

    let step = Step.create("fetch_html_page", fun context -> task {
        let! response = httpClient.GetAsync("https://nbomber.com")
        return if response.IsSuccessStatusCode then Response.Ok()
               else Response.Fail()
    })

    Scenario.create "nbomber_web_site_scenario" [step]
    |> Scenario.withWarmUpDuration(seconds 5)
    |> Scenario.withLoadSimulations [InjectPerSec(rate = 20, during = seconds 30)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withWorkerPlugins [new PingPlugin()]
    |> NBomberRunner.withTestSuite "http"
    |> NBomberRunner.withTestName "simple_test"
    |> NBomberRunner.loadInfraConfig "./HttpTests/infra-config.json"
    |> NBomberRunner.run
    |> ignore
