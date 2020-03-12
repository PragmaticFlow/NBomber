module AdvancedHelloWorldScenario

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

type TestSocketClient = { Id: int }

let run () =

    let connectionPool =
        ConnectionPool.create(
            name = "test_pool",
            connectionCount = 10,

            openConnection = (fun connectionNumber ->
                Task.Delay(1_000).Wait()
                { TestSocketClient.Id = connectionNumber }),

            closeConnection = fun connection -> Task.Delay(1_000).Wait()
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
        return ()
    }

    let testClean = fun (context: ScenarioContext) -> task {
        return ()
    }

    let scenario = Scenario.create "Hello World!" [step1; step2]
                   |> Scenario.withTestInit(testInit)
                   |> Scenario.withTestClean(testClean)
                   |> Scenario.withLoadSimulations [
                       KeepConcurrentScenarios(copiesCount = 1, during = TimeSpan.FromSeconds 20.0)
                       //RampConcurrentScenarios(copiesCount = 1, during = TimeSpan.FromSeconds 20.0)
                       //InjectScenariosPerSec(copiesCount = 1, during = TimeSpan.FromSeconds 20.0)
                       //RampScenariosPerSec(copiesCount = 1, during = TimeSpan.FromSeconds 20.0)
                   ]

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.runInConsole
