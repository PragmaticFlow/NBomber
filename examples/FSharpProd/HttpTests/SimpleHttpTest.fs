module FSharpProd.HttpTests.SimpleHttpTest

open NBomber
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Plugins.Http.FSharp
open NBomber.Plugins.Network.Ping

// in this example we use:
// - NBomber.Http (https://nbomber.com/docs/plugins-http)

let run () =

    let httpFactory = HttpClientFactory.create()

    let step = Step.create("fetch_html_page",
                           clientFactory = httpFactory,
                           execute = fun context ->

        Http.createRequest "GET" "https://nbomber.com"
        |> Http.withHeader "Accept" "text/html"
        |> Http.send context
    )

    // it's optional Ping plugin that brings additional reporting data
    let pingPluginConfig = PingPluginConfig.CreateDefault ["nbomber.com"]
    use pingPlugin = new PingPlugin(pingPluginConfig)

    Scenario.create "simple_http" [step]
    |> Scenario.withWarmUpDuration(seconds 5)
    |> Scenario.withLoadSimulations [InjectPerSec(rate = 100, during = seconds 30)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withWorkerPlugins [pingPlugin]
    |> NBomberRunner.withTestSuite "http"
    |> NBomberRunner.withTestName "simple_test"
    |> NBomberRunner.run
    |> ignore
