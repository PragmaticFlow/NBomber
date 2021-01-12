module Tests.ConnectionPool

open System
open System.Threading.Tasks

open Xunit
open FsCheck
open FsCheck.Xunit
open Swensen.Unquote
open FSharp.Control.Tasks.NonAffine

open NBomber.Contracts
open NBomber.FSharp
open NBomber.Extensions.InternalExtensions

[<Fact>]
let ``should distribute connection with one to one mapping if connectionPool.Count = loadSimulation copiesCount``() =

    let poolCount = 100

    let pool =
        ConnectionPoolArgs.create(
            name = "test_pool",
            openConnection = (fun (number,context) -> Task.FromResult number),
            closeConnection = (fun _ -> Task.CompletedTask),
            connectionCount = poolCount
        )

    let step = Step.create("step", pool, fun context -> task {
        do! Task.Delay(milliseconds 100)

        if context.CorrelationId.CopyNumber <> context.Connection then
            return Response.fail "distribution is not following one to one mapping"

        else return Response.ok()
    })

    Scenario.create "test" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = poolCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./connection-pool/1/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let stepStats = nodeStats.ScenarioStats.[0]
        test <@ stepStats.OkCount > 0 @>
        test <@ stepStats.FailCount = 0 @>

[<Fact>]
let ``should distribute connection using modulo if connectionPool.Count < loadSimulation copiesCount``() =

    let poolCount = 5
    let copiesCount = 10

    let pool =
        ConnectionPoolArgs.create(
            name = "test_pool",
            openConnection = (fun (number,context) -> Task.FromResult number),
            closeConnection = (fun _ -> Task.CompletedTask),
            connectionCount = poolCount
        )

    let step = Step.create("step", pool, fun context -> task {
        do! Task.Delay(milliseconds 100)

        let correctConnection = context.CorrelationId.CopyNumber % poolCount

        if correctConnection <> context.Connection then
            return Response.fail "distribution is not following mapping by modulo"

        else return Response.ok()
    })

    Scenario.create "test" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = copiesCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./connection-pool/2/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let stepStats = nodeStats.ScenarioStats.[0]
        test <@ stepStats.OkCount > 0 @>
        test <@ stepStats.FailCount = 0 @>

[<Fact>]
let ``should be shared btw steps as singlton instance``() =

    let poolCount = 100

    let pool =
        ConnectionPoolArgs.create(
            name = "test_pool",
            openConnection = (fun (number,context) -> Guid.NewGuid() |> Task.FromResult),
            closeConnection = (fun _ -> Task.CompletedTask),
            connectionCount = poolCount
        )

    let step1 = Step.create("step_1", pool, fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Connection)
    })

    let step2 = Step.create("step_2", pool, fun context -> task {
        do! Task.Delay(milliseconds 100)

        let stepResponse = context.GetPreviousStepResponse() :?> Guid

        if stepResponse = context.Connection then
            return Response.ok()

        else return Response.fail()
    })

    Scenario.create "test" [step1; step2]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = poolCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./connection-pool/3/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let step_2_stats = nodeStats.ScenarioStats.[0].StepStats |> Seq.find(fun x -> x.StepName = "step_2")
        test <@ step_2_stats.OkCount > 0 @>
        test <@ step_2_stats.FailCount = 0 @>

[<Fact>]
let ``openConnection should stop test session in case of failure``() =

    let poolCount = 100

    let pool =
        ConnectionPoolArgs.create(
            name = "test_pool",
            openConnection = (fun (number,context) -> failwith "unhandled exception"),
            closeConnection = (fun _ -> Task.CompletedTask),
            connectionCount = poolCount
        )

    let step1 = Step.create("step_1", pool, fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Connection)
    })

    Scenario.create "test" [step1]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = poolCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./connection-pool/4/"
    |> NBomberRunner.run
    |> Result.getError
    |> ignore

[<Fact>]
let ``openConnection should use try logic in case of some errors``() =

    let poolCount = 1
    let mutable tryCount = 0

    let pool =
        ConnectionPoolArgs.create(
            name = "test_pool",
            openConnection = (fun (number,token) ->
                tryCount <- tryCount + 1
                failwith "unhandled exception"),
            closeConnection = (fun _ -> Task.CompletedTask),
            connectionCount = poolCount
        )

    let step1 = Step.create("step_1", pool, fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Connection)
    })

    Scenario.create "test" [step1]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = poolCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./connection-pool/5/"
    |> NBomberRunner.run
    |> Result.getError
    |> fun error -> test <@ tryCount >= 5 @>

[<Fact>]
let ``closeConnection should not affect test session in case of failure``() =

    let poolCount = 10
    let mutable closeInvokeCount = 0
    let pool =
        ConnectionPoolArgs.create(
            name = "test_pool",
            openConnection = (fun (number,context) -> Task.FromResult number),
            closeConnection = (fun _ ->
                closeInvokeCount <- closeInvokeCount + 1
                Task.CompletedTask),
            connectionCount = poolCount
        )

    let step1 = Step.create("step_1", pool, fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Connection)
    })

    Scenario.create "test" [step1]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = poolCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./connection-pool/6/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun allStats -> test <@ closeInvokeCount >= 10 @>

[<Fact>]
let ``should be initialized one time per scenario``() =

    let poolCount = 10
    let mutable initializedConnections = 0

    let pool =
        ConnectionPoolArgs.create(
            name = "test_pool",
            openConnection = (fun (number,context) ->
                initializedConnections <- initializedConnections + 1
                Task.FromResult number),
            closeConnection = (fun _ -> Task.CompletedTask),
            connectionCount = poolCount
        )

    let step1 = Step.create("step_1", pool, fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Connection)
    })

    let step2 = Step.create("step_2", pool, fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Connection)
    })

    let scenario1 =
        Scenario.create "scenario 1" [step1; step2]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = poolCount, during = seconds 2)]

    let scenario2 =
        Scenario.create "scenario 2" [step1]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = poolCount, during = seconds 2)]

    NBomberRunner.registerScenarios [scenario1; scenario2]
    |> NBomberRunner.withReportFolder "./connection-pool/7/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        test <@ initializedConnections = 20 @>
        test <@ nodeStats.ScenarioStats.Length = 2 @>

[<Fact>]
let ``should be initialized after scenario init``() =

    let poolCount = 1
    let mutable lastInvokeComponent = ""

    let initScenario (context: IScenarioContext) = task {
        lastInvokeComponent <- "scenario"
    }

    let pool =
        ConnectionPoolArgs.create(
            name = "test_pool",
            openConnection = (fun (number,context) ->
                lastInvokeComponent <- "connection_pool"
                Task.FromResult number),
            closeConnection = (fun _ -> Task.CompletedTask),
            connectionCount = poolCount
        )

    let step1 = Step.create("step_1", pool, fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Connection)
    })

    Scenario.create "test" [step1]
    |> Scenario.withInit initScenario
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = poolCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./connection-pool/8/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun allStats -> test <@ lastInvokeComponent = "connection_pool" @>

[<Fact>]
let ``should support 65K of connections``() =

    let poolCount = 65_000
    let mutable invokeCount = 0

    let pool =
        ConnectionPoolArgs.create(
            name = "test_pool",
            openConnection = (fun (number,context) ->
                invokeCount <- invokeCount + 1
                Task.FromResult number),
            closeConnection = (fun _ -> Task.CompletedTask),
            connectionCount = poolCount
        )

    let step1 = Step.create("step_1", pool, fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Connection)
    })

    Scenario.create "test" [step1]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = poolCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./connection-pool/9/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun allStats -> test <@ invokeCount = poolCount @>

[<Fact>]
let ``should not allow to have duplicates with the same name but different implementation``() =

    let pool1 =
        ConnectionPoolArgs.create(
            name = "duplicate_pool_name",
            openConnection = (fun (number,context) -> Task.FromResult number),
            closeConnection = (fun _ -> Task.CompletedTask),
            connectionCount = 50
        )

    let pool2 =
        ConnectionPoolArgs.create(
            name = "duplicate_pool_name",
            openConnection = (fun (number,context) -> Task.FromResult(number + 1)),
            closeConnection = (fun _ -> Task.CompletedTask),
            connectionCount = 100
        )

    let step1 = Step.create("step_1", pool1, fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Connection)
    })

    let step2 = Step.create("step_2", pool2, fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Connection)
    })

    Scenario.create "test" [step1; step2]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 100, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./connection-pool/10/"
    |> NBomberRunner.run
    |> Result.getError
    |> ignore
