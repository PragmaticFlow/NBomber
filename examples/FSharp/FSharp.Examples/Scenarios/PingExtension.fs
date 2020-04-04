module PingExtensionScenario

open System
open System.Threading.Tasks
open System.Net.Http

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

// it's a very basic PingExtension example to give you a playground for writing your own custom extension

type PingExtension () =

    interface IExtension with
        member x.Init(logger, infraConfig) = ()

        member x.StartTest(testInfo: TestInfo) =
            Task.CompletedTask

        member x.FinishTest(testInfo: TestInfo) =
            Task.FromResult(CustomStatistics.Create())

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

    let pingExtension = PingExtension()

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.loadInfraConfig "infra_config.json"
    |> NBomberRunner.withExtensions([pingExtension])
    |> NBomberRunner.runInConsole
