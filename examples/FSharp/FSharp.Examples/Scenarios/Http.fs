module HttpScenario

open System.Net.Http

open FSharp.Control.Tasks.V2.ContextInsensitive

open System
open System.Threading.Tasks
open NBomber.Contracts
open NBomber.FSharp

let run () =

    // it's a very basic HTTP example, don't use it for production testing
    // for production purposes use NBomber.Http which use performance optimizations
    // you can find more here: https://github.com/PragmaticFlow/NBomber.Http

    let httpClient = new HttpClient()

    let step = Step.create("pull html", fun context -> task {
        let! response = httpClient.GetAsync("https://nbomber.com",
                                            context.CancellationToken)

        //do! Task.Delay(TimeSpan.FromSeconds(20.0))

        match response.IsSuccessStatusCode with
        | true  -> let size = int response.Content.Headers.ContentLength.Value
                   return Response.Ok(sizeBytes = size)
        | false -> return Response.Fail()
    })

    let scenario = Scenario.create "test_nbomber" [step]
                   |> Scenario.withWarmUpDuration(seconds 20)
                   |> Scenario.withLoadSimulations [
                       LoadSimulation.RampConcurrentScenarios(copies 300, during = minutes 1)
                       LoadSimulation.KeepConcurrentScenarios(copies 300, during = minutes 1)
                       LoadSimulation.InjectScenariosPerSec(copies 400, during = minutes 1)
                   ]

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.loadInfraConfig "infra_config.json"
    |> NBomberRunner.runInConsole
