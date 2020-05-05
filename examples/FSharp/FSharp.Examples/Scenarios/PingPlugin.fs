module PingPluginScenario

open System
open System.Net.Http

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp
open NBomber.Plugins.Network.Ping

let run () =
    let httpClient = new HttpClient()

    let step = Step.create("pull html", fun context -> task {
        let! response = httpClient.GetAsync("https://nbomber.com", context.CancellationToken)

        match response.IsSuccessStatusCode with
        | true  -> let bodySize = int response.Content.Headers.ContentLength.Value
                   let headersSize = response.Headers.ToString().Length
                   return Response.Ok(sizeBytes = headersSize + bodySize)

        | false -> return Response.Fail()
    })

    let scenario = Scenario.create "test_nbomber" [step]
                   |> Scenario.withLoadSimulations [
                       InjectScenariosPerSec(copiesCount = 150, during = TimeSpan.FromMinutes 1.0)
                   ]

    let pingPluginConfig = PingPluginConfig.CreateDefault ["nbomber.com"]
    use pingPlugin = new PingPlugin(pingPluginConfig)

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.loadInfraConfigYaml "infra_config.yaml"
    |> NBomberRunner.withPlugins [pingPlugin]
    |> NBomberRunner.runInConsole
