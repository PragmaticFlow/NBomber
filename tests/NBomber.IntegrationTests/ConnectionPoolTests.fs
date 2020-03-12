module Tests.ConnectionPoolTests

open System

open Xunit
open FsCheck
open FsCheck.Xunit
open Swensen.Unquote
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

[<Fact>]
let ``ConnectionPool should distribute connection with one to one mapping if connectionPool.Count = loadSimulation copiesCount``() =

    let connectionCount = 100

    let pool = ConnectionPool.create("test_pool", connectionCount, fun connectionNumber -> connectionNumber)

    let step = Step.create("step", pool, fun context -> task {

        if context.CorrelationId.CopyNumber <> context.Connection then
            return Response.Fail "distribution is not following one to one mapping"

        else return Response.Ok()
    })

    let scenario =
        Scenario.create "test" [step]
        |> Scenario.withOutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(copiesCount = connectionCount, during = TimeSpan.FromSeconds 2.0)
        ]

    let result =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.runTest

    match result with
    | Ok allStats ->

        let stepStats = allStats |> Seq.find(fun x -> x.StepName = "step")
        test <@ stepStats.OkCount > 0 @>
        test <@ stepStats.FailCount = 0 @>

    | Error error -> failwith error

[<Fact>]
let ``ConnectionPool should distribute connection using modulo if connectionPool.Count < loadSimulation copiesCount``() =

    let connectionCount = 5
    let copiesCount = 10

    let pool = ConnectionPool.create("test_pool", connectionCount, fun connectionNumber -> connectionNumber)

    let step = Step.create("step", pool, fun context -> task {

        let correctConnection = context.CorrelationId.CopyNumber % connectionCount

        if correctConnection <> context.Connection then
            return Response.Fail "distribution is not following mapping by modulo"

        else return Response.Ok()
    })

    let scenario =
        Scenario.create "test" [step]
        |> Scenario.withOutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(copiesCount = copiesCount, during = TimeSpan.FromSeconds 2.0)
        ]

    let result =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.runTest

    match result with
    | Ok allStats ->

        let stepStats = allStats |> Seq.find(fun x -> x.StepName = "step")
        test <@ stepStats.OkCount > 0 @>
        test <@ stepStats.FailCount = 0 @>

    | Error error -> failwith error

[<Fact>]
let ``ConnectionPool should be shared btw steps as singlton instance``() =

    let connectionCount = 100
    let pool = ConnectionPool.create("test_pool", connectionCount, fun connectionNumber -> Guid.NewGuid())

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
        |> Scenario.withOutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(copiesCount = connectionCount, during = TimeSpan.FromSeconds 2.0)
        ]

    let result =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.runTest

    match result with
    | Ok allStats ->

        let step_2_stats = allStats |> Seq.find(fun x -> x.StepName = "step_2")
        test <@ step_2_stats.OkCount > 0 @>
        test <@ step_2_stats.FailCount = 0 @>

    | Error error -> failwith error

[<Fact>]
let ``ConnectionPool openConnection should stop test session in case of failure``() =

    let connectionCount = 100
    let pool = ConnectionPool.create("test_pool", connectionCount,
                                     fun connectionNumber -> failwith "unhandled exception")

    let step1 = Step.create("step_1", pool, fun context -> task {
        return Response.Ok(context.Connection)
    })

    let scenario =
        Scenario.create "test" [step1]
        |> Scenario.withOutWarmUp
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
let ``ConnectionPool openConnection should use try logic in case of some errors``() =

    let connectionCount = 1
    let mutable tryCount = 0

    let pool = ConnectionPool.create("test_pool", connectionCount,
                                     fun connectionNumber -> tryCount <- tryCount + 1
                                                             failwith "unhandled exception")

    let step1 = Step.create("step_1", pool, fun context -> task {
        return Response.Ok(context.Connection)
    })

    let scenario =
        Scenario.create "test" [step1]
        |> Scenario.withOutWarmUp
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
let ``ConnectionPool closeConnection should not affect test session in case of failure``() =

    let connectionCount = 10
    let mutable closeInvokeCount = 0
    let pool = ConnectionPool.create("test_pool", connectionCount,
                                     (fun connectionNumber -> connectionNumber),
                                     fun connection ->
                                         closeInvokeCount <- closeInvokeCount + 1
                                         failwith "unhandled exception")

    let step1 = Step.create("step_1", pool, fun context -> task {
        return Response.Ok(context.Connection)
    })

    let scenario =
        Scenario.create "test" [step1]
        |> Scenario.withOutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(copiesCount = connectionCount, during = TimeSpan.FromSeconds 2.0)
        ]

    let result =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.runTest

    match result with
    | Ok allStats -> test <@ closeInvokeCount >= 10 @>
    | Error error -> failwith "unhandled exception"
