module Tests.ConnectionPoolTests

open Xunit
open FsCheck
open FsCheck.Xunit
open Swensen.Unquote
open FSharp.Control.Tasks.V2.ContextInsensitive

open System
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
