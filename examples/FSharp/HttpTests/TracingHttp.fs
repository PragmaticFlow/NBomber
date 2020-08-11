module FSharp.HttpTests.TracingHttp

open Serilog

open NBomber
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Plugins.Http.FSharp
open NBomber.Plugins.Network.Ping

let run () =

    let step = HttpStep.create("fetch_html_page", fun context ->
        Http.createRequest "GET" "https://nbomber.com"
        |> Http.withHeader "Accept" "text/html"
    )

    let pingPluginConfig = PingPluginConfig.CreateDefault ["nbomber.com"]
    let pingPlugin = new PingPlugin(pingPluginConfig)

    Scenario.create "nbomber_web_site" [step]
    |> Scenario.withWarmUpDuration(seconds 5)
    |> Scenario.withLoadSimulations [InjectPerSec(rate = 100, during = seconds 30)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withPlugins [pingPlugin]
    |> NBomberRunner.withTestSuite "http"
    |> NBomberRunner.withTestName "tracing_test"
    |> NBomberRunner.withLoggerConfig(fun () -> LoggerConfiguration().MinimumLevel.Verbose())
    |> NBomberRunner.run
    |> ignore
