module AdvancedHelloWorldScenario

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp
open NBomber.Extensions

type TestSocketClient = { Id: int }

[<CLIMutable>]
type CustomScenarioSettings = {
    TestField: int
}

let run () =

    let connectionPool =
        ConnectionPoolArgs.create(
            name = "test_pool",
            getConnectionCount = (fun _ -> 10),

            openConnection = (fun (number,token) -> task {
                do! Task.Delay(1_000)
                return { TestSocketClient.Id = number }
            }),

            closeConnection = (fun (connection,token) -> task {
                do! Task.Delay(1_000)
            })
        )

    let step1 = Step.create("step_1", connectionPool, fun context -> task {
        // you can do any logic here: go to http, websocket etc

        // context.CorrelationId
        // context.Connection
        // context.Logger

        do! Task.Delay(TimeSpan.FromSeconds(2.0))
        return Response.Ok(42) // this value will be passed as response for the next step
    })

    let step2 = Step.create("step_2", connectionPool, fun context -> task {
        // you can do any logic here: go to http, websocket etc

        let value = context.GetPreviousStepResponse<int>() // 42
        return Response.Ok()
    })

    let testInit = fun (context: ScenarioContext) -> task {
        let settings = context.CustomSettings.DeserializeJson<CustomScenarioSettings>()
        //let settings = context.CustomSettings.DeserializeYaml<CustomScenarioSettings>()
        context.Logger.Information("test init received CustomSettings.TestField '{TestField}'", settings.TestField)
        return ()
    }

    let testClean = fun (context: ScenarioContext) -> task {
        return ()
    }

    let scenario = Scenario.create "hello_world_scenario" [step1; step2]
                   |> Scenario.withTestInit(testInit)
                   |> Scenario.withTestClean(testClean)
                   |> Scenario.withLoadSimulations [
                       KeepConcurrentScenarios(copiesCount = 1, during = TimeSpan.FromSeconds 20.0)
                       //RampConcurrentScenarios(copiesCount = 1, during = TimeSpan.FromSeconds 20.0)
                       //InjectScenariosPerSec(copiesCount = 1, during = TimeSpan.FromSeconds 20.0)
                       //RampScenariosPerSec(copiesCount = 1, during = TimeSpan.FromSeconds 20.0)
                   ]

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.loadTestConfigJson("config.json")
    //|> NBomberRunner.loadTestConfigYaml("config.yaml")
    |> NBomberRunner.runInConsole
