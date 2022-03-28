module FSharpDev.ClientFactory.HttpClientFactoryExample

open System.Net.Http
open NBomber
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Plugins.Network.Ping

let run () =

    let httpFactory = ClientFactory.create(
        name = "http_factory",
        clientCount = 1,
        initClient = fun (number,context) -> task { return new HttpClient() }
    )

    let step = Step.create("fetch_html_page",
                           clientFactory = httpFactory,
                           //timeout = milliseconds 500,
                           execute = fun context -> task {

        let! response = context.Client.GetAsync("https://nbomber.com",
                                                context.CancellationToken)

        match response.IsSuccessStatusCode with
        | true  -> let size = int response.Content.Headers.ContentLength.Value
                   return Response.ok(statusCode = int response.StatusCode, sizeBytes = size)
        | false -> return Response.fail(statusCode = int response.StatusCode)
    })

    let pingPluginConfig = PingPluginConfig.createDefault ["nbomber.com"]
    use pingPlugin = new PingPlugin(pingPluginConfig)

    Scenario.create "simple_http" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [InjectPerSec(rate = 100, during = minutes 3)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withWorkerPlugins [pingPlugin]
    |> NBomberRunner.run
    |> ignore
