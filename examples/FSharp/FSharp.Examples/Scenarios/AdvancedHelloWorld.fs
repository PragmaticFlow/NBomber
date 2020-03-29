module AdvancedHelloWorldScenario

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Extensions

type FakeSocketClient = { Id: int }

[<CLIMutable>]
type CustomScenarioSettings = {
    TestField: int
}

let run () =

    let testInit = fun (context: ScenarioContext) -> task {
        if not (String.IsNullOrEmpty context.CustomSettings) then
            let settings = context.CustomSettings.DeserializeJson<CustomScenarioSettings>()
            context.Logger.Information("test init received CustomSettings.TestField '{TestField}'", settings.TestField)
        return ()
    }

    let testClean = fun (context: ScenarioContext) -> task {
        return ()
    }

    // you can find more examples of data feed in DataFeed.fs
    let dataFeed = FeedData.fromSeq [1; 2; 3]
                   |> Feed.createRandom "random_feed"

    let webSocketConnectionPool =
        ConnectionPoolArgs.create(
            name = "web_socket_pool",
            getConnectionCount = (fun _ -> 10),

            openConnection = (fun (number,token) -> task {
                do! Task.Delay(1_000)
                return { FakeSocketClient.Id = number }
            }),

            closeConnection = (fun (connection,token) -> task {
                do! Task.Delay(1_000)
            })
        )

    let step1 = Step.create("step_1", webSocketConnectionPool, dataFeed, fun context -> task {
        // you can do any logic here: go to http, websocket etc

        // context.CorrelationId - every copy of scenario has correlation id
        // context.Connection    - fake websocket connection taken from pool
        // context.FeedItem      - item taken from data feed
        // context.Logger
        // context.StopScenario("hello_world_scenario", reason = "")
        // context.StopTest(reason = "")

        do! Task.Delay(TimeSpan.FromSeconds(2.0))
        return Response.Ok(42) // this value will be passed as response for the next step

        // return Response.Ok(42, sizeBytes = 100, latencyMs = 100); - you can specify response size and custom latency
        // return Response.Fail();                                   - in case of fail, the next step will be skipped
    })

    let step2 = Step.create("step_2", webSocketConnectionPool, fun context -> task {
        // you can do any logic here: go to http, websocket etc

        do! Task.Delay(TimeSpan.FromMilliseconds 200.0)
        let value = context.GetPreviousStepResponse<int>() // 42
        return Response.Ok()
    })

    let scenario = Scenario.create "hello_world_scenario" [step1; step2]
                   |> Scenario.withTestInit(testInit)
                   |> Scenario.withTestClean(testClean)
                   |> Scenario.withWarmUpDuration(TimeSpan.FromSeconds 10.0)
                   //|> Scenario.withOutWarmUp - disable warm up
                   |> Scenario.withLoadSimulations [
                       RampConcurrentScenarios(copiesCount = 20, during = TimeSpan.FromSeconds 20.0)
                       KeepConcurrentScenarios(copiesCount = 20, during = TimeSpan.FromMinutes 1.0)
                       //InjectScenariosPerSec(copiesCount = 20, during = TimeSpan.FromSeconds 20.0)
                       //RampScenariosPerSec(copiesCount = 20, during = TimeSpan.FromMinutes 20.0)
                   ]

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.loadTestConfig("test_config.json")   // test config for test settings only
    //|> NBomberRunner.loadInfraConfig("infra_config.json") // infra config for infra settings only
    |> NBomberRunner.runInConsole
