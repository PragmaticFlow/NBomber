module Tests.ClientFactory

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
let ``should distribute client with one to one mapping if clientCount = loadSimulation copiesCount``() =

    let poolCount = 100

    let factory =
        ClientFactory.create(
            name = "test_pool",
            initClient = (fun (number,context) -> Task.FromResult number),
            clientCount = poolCount
        )

    let step = Step.create("step", clientFactory = factory, exec = fun context -> task {
        do! Task.Delay(milliseconds 100)

        if context.ScenarioId.Number <> context.Client then
            return Response.fail "distribution is not following one to one mapping"

        else return Response.ok()
    })

    Scenario.create "test" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = poolCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./client-pool/1/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let stepStats = nodeStats.ScenarioStats.[0]
        test <@ stepStats.OkCount > 0 @>
        test <@ stepStats.FailCount = 0 @>

[<Fact>]
let ``should distribute client using modulo if clientCount < loadSimulation copiesCount``() =

    let clientCount = 5
    let copiesCount = 10

    let pool =
        ClientFactory.create(
            name = "test_pool",
            initClient = (fun (number,context) -> Task.FromResult number),
            clientCount = clientCount
        )

    let step = Step.create("step", clientFactory = pool, exec = fun context -> task {
        do! Task.Delay(milliseconds 100)

        let correctClient = context.ScenarioId.Number % clientCount

        if correctClient <> context.Client then
            return Response.fail "distribution is not following mapping by modulo"

        else return Response.ok()
    })

    Scenario.create "test" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = copiesCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./client-pool/2/"
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
        ClientFactory.create(
            name = "test_pool",
            initClient = (fun (number,context) -> Guid.NewGuid() |> Task.FromResult),
            clientCount = poolCount
        )

    let step1 = Step.create("step_1", clientFactory = pool, exec = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    let step2 = Step.create("step_2", clientFactory = pool, exec = fun context -> task {
        do! Task.Delay(milliseconds 100)

        let stepResponse = context.GetPreviousStepResponse<Guid>()

        if stepResponse = context.Client then
            return Response.ok()

        else return Response.fail()
    })

    Scenario.create "test" [step1; step2]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = poolCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./client-pool/3/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let step_2_stats = nodeStats.ScenarioStats.[0].StepStats |> Seq.find(fun x -> x.StepName = "step_2")
        test <@ step_2_stats.Ok.Request.Count > 0 @>
        test <@ step_2_stats.Fail.Request.Count = 0 @>

[<Fact>]
let ``initClient should stop test session in case of failure``() =

    let clientCount = 100

    let pool =
        ClientFactory.create(
            name = "test_pool",
            initClient = (fun (number,context) -> failwith "unhandled exception"),
            clientCount = clientCount
        )

    let step1 = Step.create("step_1", clientFactory = pool, exec = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    Scenario.create "test" [step1]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = clientCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./client-pool/4/"
    |> NBomberRunner.run
    |> Result.getError
    |> ignore

[<Fact>]
let ``initClient should use try logic in case of some errors``() =

    let clientCount = 1
    let mutable tryCount = 0

    let pool =
        ClientFactory.create(
            name = "test_pool",
            initClient = (fun (number,token) ->
                tryCount <- tryCount + 1
                failwith "unhandled exception"),
            clientCount = clientCount
        )

    let step1 = Step.create("step_1", clientFactory = pool, exec = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    Scenario.create "test" [step1]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = clientCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./client-pool/5/"
    |> NBomberRunner.run
    |> Result.getError
    |> fun error -> test <@ tryCount >= 5 @>

[<Fact>]
let ``should be initialized one time per scenario``() =

    let clientCount = 10
    let mutable initializedClients = 0

    let factory =
        ClientFactory.create(
            name = "test_pool",
            initClient = (fun (number,context) ->
                initializedClients <- initializedClients + 1
                Task.FromResult number),
            clientCount = clientCount
        )

    let step1 = Step.create("step_1", clientFactory = factory, exec = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    let step2 = Step.create("step_2", clientFactory = factory, exec = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    let scenario1 =
        Scenario.create "scenario 1" [step1; step2]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = clientCount, during = seconds 2)]

    let scenario2 =
        Scenario.create "scenario 2" [step1]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = clientCount, during = seconds 2)]

    NBomberRunner.registerScenarios [scenario1; scenario2]
    |> NBomberRunner.withReportFolder "./client-pool/7/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        test <@ initializedClients = 20 @>
        test <@ nodeStats.ScenarioStats.Length = 2 @>

[<Fact>]
let ``should be initialized after scenario init``() =

    let clientCount = 1
    let mutable lastInvokeClient = ""

    let initScenario (context: IScenarioContext) = task {
        lastInvokeClient <- "scenario"
    }

    let pool =
        ClientFactory.create(
            name = "test_pool",
            initClient = (fun (number,context) ->
                lastInvokeClient <- "client_pool"
                Task.FromResult number),
            clientCount = clientCount
        )

    let step1 = Step.create("step_1", clientFactory = pool, exec = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    Scenario.create "test" [step1]
    |> Scenario.withInit initScenario
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = clientCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./client-pool/8/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun allStats -> test <@ lastInvokeClient = "client_pool" @>

[<Fact>]
let ``should support 2K of clients``() =

    let clientCount = 2_000
    let mutable invokeCount = 0

    let pool =
        ClientFactory.create(
            name = "test_pool",
            initClient = (fun (number,context) ->
                invokeCount <- invokeCount + 1
                Task.FromResult number),
            clientCount = clientCount
        )

    let step1 = Step.create("step_1", clientFactory = pool, exec = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    Scenario.create "test" [step1]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = clientCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./client-pool/9/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun allStats -> test <@ invokeCount = clientCount @>

[<Fact>]
let ``should not allow to have duplicates with the same name but different implementation``() =

    let pool1 =
        ClientFactory.create(
            name = "duplicate_pool_name",
            initClient = (fun (number,context) -> Task.FromResult number),
            clientCount = 50
        )

    let pool2 =
        ClientFactory.create(
            name = "duplicate_pool_name",
            initClient = (fun (number,context) -> Task.FromResult(number + 1)),
            clientCount = 100
        )

    let step1 = Step.create("step_1", clientFactory = pool1, exec = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    let step2 = Step.create("step_2", clientFactory = pool2, exec = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    Scenario.create "test" [step1; step2]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 100, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./client-pool/10/"
    |> NBomberRunner.run
    |> Result.getError
    |> ignore

[<Fact>]
let ``should provide default dispose client``() =

    let mutable defaultDisposeInvoked = false

    let client = {
        new IDisposable with
            member _.Dispose() =
                defaultDisposeInvoked <- true
    }

    let factory =
        ClientFactory.create(
            name = "factory_1",
            initClient = (fun (number,context) -> Task.FromResult client)
        )

    let step = Step.create("step_1", clientFactory = factory, exec = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    Scenario.create "test" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 100, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./client-pool/11/"
    |> NBomberRunner.run
    |> ignore

    test <@ defaultDisposeInvoked = true @>

[<Fact>]
let ``should provide custom dispose client``() =

    let mutable customDisposeInvoked = false

    let factory =
        ClientFactory.create(
            name = "factory_1",
            initClient = (fun (number,context) -> Task.FromResult 1),
            disposeClient = (fun (client,context) -> task {
                customDisposeInvoked <- true
            })
        )

    let step = Step.create("step_1", clientFactory = factory, exec = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    Scenario.create "test" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 100, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./client-pool/12/"
    |> NBomberRunner.run
    |> ignore

    test <@ customDisposeInvoked = true @>
