module Tests.ConnectionPool

open System
open System.Threading.Tasks

open Xunit
open FsCheck
open FsCheck.Xunit
open Swensen.Unquote
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

[<Fact>]
let ``should distribute connection with one to one mapping if connectionPool.Count = loadSimulation copiesCount``() =

    let connectionCount = 100

    let pool = ConnectionPoolArgs.create("test_pool",
                                         getConnectionCount = (fun _ -> connectionCount),
                                         openConnection = (fun (number,token) -> Task.FromResult number),
                                         closeConnection = (fun _ -> Task.CompletedTask))

    let step = Step.create("step", pool, fun context -> task {

        if context.CorrelationId.CopyNumber <> context.Connection then
            return Response.Fail "distribution is not following one to one mapping"

        else return Response.Ok()
    })

    let scenario =
        Scenario.create "test" [step]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(copiesCount = connectionCount, during = TimeSpan.FromSeconds 2.0)
        ]

    let result =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.runTest

    match result with
    | Ok nodeStats ->
        let stepStats = nodeStats.ScenarioStats.[0]
        test <@ stepStats.OkCount > 0 @>
        test <@ stepStats.FailCount = 0 @>

    | Error error -> failwith error

[<Fact>]
let ``should distribute connection using modulo if connectionPool.Count < loadSimulation copiesCount``() =

    let connectionCount = 5
    let copiesCount = 10

    let pool = ConnectionPoolArgs.create("test_pool",
                                         getConnectionCount = (fun _ -> connectionCount),
                                         openConnection = (fun (number,token) -> Task.FromResult number),
                                         closeConnection = (fun _ -> Task.CompletedTask))

    let step = Step.create("step", pool, fun context -> task {

        let correctConnection = context.CorrelationId.CopyNumber % connectionCount

        if correctConnection <> context.Connection then
            return Response.Fail "distribution is not following mapping by modulo"

        else return Response.Ok()
    })

    let scenario =
        Scenario.create "test" [step]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(copiesCount = copiesCount, during = TimeSpan.FromSeconds 2.0)
        ]

    let result =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.runTest

    match result with
    | Ok nodeStats ->
        let stepStats = nodeStats.ScenarioStats.[0]
        test <@ stepStats.OkCount > 0 @>
        test <@ stepStats.FailCount = 0 @>

    | Error error -> failwith error

[<Fact>]
let ``should be shared btw steps as singlton instance``() =

    let connectionCount = 100

    let pool = ConnectionPoolArgs.create("test_pool",
                                         getConnectionCount = (fun _ -> connectionCount),
                                         openConnection = (fun (number,token) -> Guid.NewGuid() |> Task.FromResult),
                                         closeConnection = (fun _ -> Task.CompletedTask))

    let step1 = Step.create("step_1", pool, fun context -> task {
        return Response.Ok(context.Connection)
    })

    let step2 = Step.create("step_2", pool, fun context -> task {
        let stepResponse = context.GetPreviousStepResponse<Guid>()

        if stepResponse = context.Connection then
            return Response.Ok()

        else return Response.Fail()
    })

    let scenario =
        Scenario.create "test" [step1; step2]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(copiesCount = connectionCount, during = TimeSpan.FromSeconds 2.0)
        ]

    let result =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.runTest

    match result with
    | Ok nodeStats ->

        let step_2_stats = nodeStats.ScenarioStats.[0].StepStats |> Seq.find(fun x -> x.StepName = "step_2")
        test <@ step_2_stats.OkCount > 0 @>
        test <@ step_2_stats.FailCount = 0 @>

    | Error error -> failwith error

[<Fact>]
let ``openConnection should stop test session in case of failure``() =

    let connectionCount = 100

    let pool = ConnectionPoolArgs.create("test_pool",
                                         getConnectionCount = (fun _ -> connectionCount),
                                         openConnection = (fun (number,token) -> failwith "unhandled exception"),
                                         closeConnection = (fun _ -> Task.CompletedTask))

    let step1 = Step.create("step_1", pool, fun context -> task {
        return Response.Ok(context.Connection)
    })

    let scenario =
        Scenario.create "test" [step1]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(copiesCount = connectionCount, during = TimeSpan.FromSeconds 2.0)
        ]

    let result =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.runTest

    match result with
    | Ok allStats -> failwith "unhandled exception"
    | Error error -> ()

[<Fact>]
let ``openConnection should use try logic in case of some errors``() =

    let connectionCount = 1
    let mutable tryCount = 0

    let pool = ConnectionPoolArgs.create("test_pool",
                                         getConnectionCount = (fun _ -> connectionCount),
                                         openConnection = (fun (number,token) ->
                                             tryCount <- tryCount + 1
                                             failwith "unhandled exception"),
                                         closeConnection = (fun _ -> Task.CompletedTask))

    let step1 = Step.create("step_1", pool, fun context -> task {
        return Response.Ok(context.Connection)
    })

    let scenario =
        Scenario.create "test" [step1]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(copiesCount = connectionCount, during = TimeSpan.FromSeconds 2.0)
        ]

    let result =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.runTest

    match result with
    | Ok allStats -> failwith "unhandled exception"
    | Error error -> test <@ tryCount >= 5 @>

[<Fact>]
let ``closeConnection should not affect test session in case of failure``() =

    let connectionCount = 10
    let mutable closeInvokeCount = 0
    let pool = ConnectionPoolArgs.create("test_pool",
                                         getConnectionCount = (fun _ -> connectionCount),
                                         openConnection = (fun (number,token) -> Task.FromResult number),
                                         closeConnection = (fun _ ->
                                             closeInvokeCount <- closeInvokeCount + 1
                                             Task.CompletedTask))

    let step1 = Step.create("step_1", pool, fun context -> task {
        return Response.Ok(context.Connection)
    })

    let scenario =
        Scenario.create "test" [step1]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(copiesCount = connectionCount, during = TimeSpan.FromSeconds 2.0)
        ]

    let result =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.runTest

    match result with
    | Ok allStats -> test <@ closeInvokeCount >= 10 @>
    | Error error -> failwith "unhandled exception"

[<Fact>]
let ``should be initialized only once``() =

    let connectionCount = 10
    let mutable initedConnections = 0

    let pool = ConnectionPoolArgs.create("test_pool",
                                         getConnectionCount = (fun _ -> connectionCount),
                                         openConnection = (fun (number,token) ->
                                             initedConnections <- initedConnections + 1
                                             Task.FromResult number),
                                         closeConnection = (fun _ -> Task.CompletedTask))

    let step1 = Step.create("step_1", pool, fun context -> task {
        return Response.Ok(context.Connection)
    })

    let scenario1 =
        Scenario.create "scenario 1" [step1]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(copiesCount = connectionCount, during = TimeSpan.FromSeconds 2.0)
        ]

    let scenario2 =
        Scenario.create "scenario 2" [step1]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(copiesCount = connectionCount, during = TimeSpan.FromSeconds 2.0)
        ]

    let result =
        NBomberRunner.registerScenarios [scenario1; scenario2]
        |> NBomberRunner.runTest

    match result with
    | Ok nodeStats -> test <@ initedConnections = connectionCount @>
                      test <@ nodeStats.ScenarioStats.Length = 2 @>
    | Error error -> failwith "unhandled exception"

[<Fact>]
let ``should be initialized after scenario init``() =

    let connectionCount = 1
    let mutable lastInvokeComponent = ""

    let initTest (context: ScenarioContext) = task {
        lastInvokeComponent <- "scenario"
    }

    let pool =
        ConnectionPoolArgs.create(
            name = "test_pool",
            getConnectionCount = (fun _ -> connectionCount),
            openConnection = (fun (number,token) ->
                lastInvokeComponent <- "connection_pool"
                Task.FromResult number),
            closeConnection = (fun _ -> Task.CompletedTask)
        )

    let step1 = Step.create("step_1", pool, fun context -> task {
        return Response.Ok(context.Connection)
    })

    let scenario =
        Scenario.create "test" [step1]
        |> Scenario.withTestInit initTest
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(copiesCount = connectionCount, during = TimeSpan.FromSeconds 2.0)
        ]

    let result =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.runTest

    match result with
    | Ok allStats -> test <@ lastInvokeComponent = "connection_pool" @>
    | Error error -> failwith "unhandled exception"

[<Fact>]
let ``should support 65K of connections``() =

    let connectionCount = 65_000
    let mutable invokeCount = 0

    let pool =
        ConnectionPoolArgs.create(
            name = "test_pool",
            getConnectionCount = (fun _ -> connectionCount),
            openConnection = (fun (number,token) ->
                invokeCount <- invokeCount + 1
                Task.FromResult number),
            closeConnection = (fun _ -> Task.CompletedTask)
        )

    let step1 = Step.create("step_1", pool, fun context -> task {
        return Response.Ok(context.Connection)
    })

    let scenario =
        Scenario.create "test" [step1]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(copiesCount = connectionCount, during = TimeSpan.FromSeconds 2.0)
        ]

    let result =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.runTest

    match result with
    | Ok allStats -> test <@ invokeCount = connectionCount @>
    | Error error -> failwith "unhandled exception"
