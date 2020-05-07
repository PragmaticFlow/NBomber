module CliArgumentsScenario

open System
open System.Net.Http

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

// run the following command in command line to test CLI:
// dotnet FSharp.Examples.dll -c config.yaml -i infra_config.yaml
// or
// dotnet FSharp.Examples.dll --config config.yaml --infra infra_config.yaml
let run (argv: string[]) =

    let httpClient = new HttpClient()

    let step = Step.create("pull html", fun context -> task {
        let! response = httpClient.GetAsync("https://nbomber.com",
                                            context.CancellationToken)

        match response.IsSuccessStatusCode with
        | true  -> let bodySize = int response.Content.Headers.ContentLength.Value
                   let headersSize = response.Headers.ToString().Length
                   return Response.Ok(sizeBytes = headersSize + bodySize)
        | false -> return Response.Fail()
    })

    let scenario = Scenario.create "test_nbomber" [step]
                   |> Scenario.withLoadSimulations [
                       InjectScenariosPerSec(copiesCount = 150, during = TimeSpan.FromMinutes 1.0)
                       //RampScenariosPerSec(copiesCount = 100, during = TimeSpan.FromSeconds 20.0)
                       //RampConcurrentScenarios(copiesCount = 100, during = TimeSpan.FromSeconds 20.0)
                       //KeepConcurrentScenarios(copiesCount = 100, during = TimeSpan.FromMinutes 1.0)
                   ]

    let cliArgs =
        if argv.Length > 0 then argv
        else [|"-c"; "config.yaml"; "-i"; "infra_config.yaml"|]
        //else [|"--config"; "config.yaml"; "--infra"; "infra_config.yaml"|]

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.executeCliArgs cliArgs
    |> NBomberRunner.runInConsole
