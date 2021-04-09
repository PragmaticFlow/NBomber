module FSharpDev.ClientFactory.HttpClientFactoryExample

open System.Net.Http
open System.Threading.Tasks
open FSharp.Control.Tasks.NonAffine
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Plugins.Network.Ping

let run () =

    let httpFactory =
        ClientFactory.create(name = "http_factory",
                             clientCount = 1,
                             initClient = fun (number,context) -> task {
                                 return new HttpClient()
                             })

    let step = Step.create("fetch_html_page",
                           clientFactory = httpFactory,
                           execute = fun context -> task {

        let! response = context.Client.GetAsync("https://nbomber.com", cancellationToken = context.CancellationToken)

        return if response.IsSuccessStatusCode then Response.ok(statusCode = int response.StatusCode)
               else Response.fail(statusCode = int response.StatusCode)
    })

    let pingPluginConfig = PingPluginConfig.CreateDefault ["nbomber.com"]
    use pingPlugin = new PingPlugin(pingPluginConfig)

    Scenario.create "simple_http" [step]
    |> Scenario.withWarmUpDuration(seconds 5)
    |> Scenario.withLoadSimulations [InjectPerSec(rate = 100, during = seconds 30)]
    |> Scenario.withStepTimeout(milliseconds 500)
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withWorkerPlugins [pingPlugin]
    |> NBomberRunner.run
    |> ignore
