module Tests.ClientFactory

open System
open System.Threading.Tasks

open Xunit
open Swensen.Unquote

open NBomber
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Extensions.Internal

[<Fact>]
let ``should distribute client with one to one mapping if clientCount = loadSimulation copiesCount``() =

    let poolCount = 100

    let factory = ClientFactory.create(
        name = "test_pool",
        initClient = (fun (number,context) -> Task.FromResult number),
        clientCount = poolCount
    )

    let step = Step.create("step", clientFactory = factory, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)

        if context.ScenarioInfo.ThreadNumber <> context.Client then
            return Response.fail "distribution is not following one to one mapping"

        else return Response.ok()
    })

    Scenario.create "test" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = poolCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let stepStats = nodeStats.ScenarioStats[0]
        test <@ stepStats.OkCount > 0 @>
        test <@ stepStats.FailCount = 0 @>

[<Fact>]
let ``should distribute client using modulo if clientCount < loadSimulation copiesCount``() =

    let clientCount = 5
    let copiesCount = 10

    let pool = ClientFactory.create(
        name = "test_pool",
        initClient = (fun (number,context) -> Task.FromResult number),
        clientCount = clientCount
    )

    let step = Step.create("step", clientFactory = pool, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)

        let correctClient = context.ScenarioInfo.ThreadNumber % clientCount

        if correctClient <> context.Client then
            return Response.fail "distribution is not following mapping by modulo"

        else return Response.ok()
    })

    Scenario.create "test" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = copiesCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let stepStats = nodeStats.ScenarioStats[0]
        test <@ stepStats.OkCount > 0 @>
        test <@ stepStats.FailCount = 0 @>

[<Fact>]
let ``should be shared btw steps as singleton instance``() =

    let clientCount = 100

    let factory = ClientFactory.create(
        name = "test_pool",
        initClient = (fun (number,context) -> Guid.NewGuid() |> Task.FromResult),
        clientCount = clientCount
    )

    let step1 = Step.create("step_1", clientFactory = factory, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    let step2 = Step.create("step_2", clientFactory = factory, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)

        let stepResponse = context.GetPreviousStepResponse<Guid>()

        if stepResponse = context.Client then
            return Response.ok()

        else return Response.fail()
    })

    Scenario.create "test" [step1; step2]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = clientCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let step_2_stats = nodeStats.ScenarioStats[0].StepStats |> Seq.find(fun x -> x.StepName = "step_2")
        test <@ step_2_stats.Ok.Request.Count > 0 @>
        test <@ step_2_stats.Fail.Request.Count = 0 @>

[<Fact>]
let ``initClient should stop test session in case of failure``() =

    let clientCount = 100

    let pool = ClientFactory.create(
        name = "test_pool",
        initClient = (fun (number,context) -> failwith "unhandled exception"),
        clientCount = clientCount
    )

    let step1 = Step.create("step_1", clientFactory = pool, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    Scenario.create "test" [step1]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = clientCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getError
    |> ignore

[<Fact>]
let ``initClient should use try logic in case of some errors``() =

    let clientCount = 1
    let mutable tryCount = 0

    let pool = ClientFactory.create(
        name = "test_pool",
        initClient = (fun (number,token) ->
            tryCount <- tryCount + 1
            failwith "unhandled exception"),
        clientCount = clientCount
    )

    let step1 = Step.create("step_1", clientFactory = pool, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    Scenario.create "test" [step1]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = clientCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getError
    |> fun error -> test <@ tryCount >= 5 @>

[<Fact>]
let ``should validate on multiple scenario assignment``() =

    let factory1 = ClientFactory.create(
        name = "factory_1",
        initClient = (fun (number,context) -> Task.FromResult number)
    )

    let step1 = Step.create("step_1", clientFactory = factory1, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    let step2 = Step.create("step_2", clientFactory = factory1, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    let scn1 = Scenario.create "scn1" [step1]
    let scn2 = Scenario.create "scn2" [step2]

    NBomberRunner.registerScenarios [scn1; scn2]
    |> NBomberRunner.run
    |> Result.getError
    |> fun error ->
        test <@ error.Contains "assigned to multiple scenarios" @>

[<Fact>]
let ``should validate on duplicate name``() =

    let factory1 = ClientFactory.create(
        name = "factory_1",
        initClient = (fun (number,context) -> Task.FromResult number)
    )

    // duplicate name
    let factory2 = ClientFactory.create(
        name = "factory_1",
        initClient = (fun (number,context) -> Task.FromResult number)
    )

    let step1 = Step.create("step_1", clientFactory = factory1, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    let step2 = Step.create("step_2", clientFactory = factory2, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    let scn1 = Scenario.create "scn1" [step1]
    let scn2 = Scenario.create "scn2" [step2]

    NBomberRunner.registerScenarios [scn1; scn2]
    |> NBomberRunner.run
    |> Result.getError
    |> fun error ->
        test <@ error.Contains "contains duplicated name" @>

[<Fact>]
let ``should be initialized one time per scenario``() =

    let clientCount = 10
    let mutable initializedClients = 0

    let factory = ClientFactory.create(
        name = "test_pool",
        initClient = (fun (number,context) ->
            initializedClients <- initializedClients + 1
            Task.FromResult number),
        clientCount = clientCount
    )

    let step1 = Step.create("step_1", clientFactory = factory, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    let step2 = Step.create("step_2", clientFactory = factory, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    let scenario1 =
        Scenario.create "scenario 1" [step1; step2]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = clientCount, during = seconds 2)]

    NBomberRunner.registerScenarios [scenario1]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        test <@ initializedClients = 10 @>

[<Fact>]
let ``should be initialized after scenario init``() =

    let clientCount = 1
    let mutable lastInvokeClient = ""

    let initScenario (context: IScenarioContext) = task {
        lastInvokeClient <- "scenario"
    }

    let pool = ClientFactory.create(
        name = "test_pool",
        initClient = (fun (number,context) ->
            lastInvokeClient <- "client_pool"
            Task.FromResult number),
        clientCount = clientCount
    )

    let step1 = Step.create("step_1", clientFactory = pool, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    Scenario.create "test" [step1]
    |> Scenario.withInit initScenario
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = clientCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun allStats -> test <@ lastInvokeClient = "client_pool" @>

[<Fact>]
let ``should be disposed after scenario clean``() =

    let clientCount = 1
    let mutable _actionsHistory = List.empty<string>

    let cleanScenario (context: IScenarioContext) = task {
        _actionsHistory <- "clean scenario" :: _actionsHistory
    }

    let factory = ClientFactory.create(
        name = "test_factory",
        initClient = (fun (number,context) -> Task.FromResult number),
        disposeClient = (fun (client, context) ->
            _actionsHistory <- "dispose client" :: _actionsHistory
            Task.FromResult()
        ),
        clientCount = clientCount
    )

    let step1 = Step.create("step_1", clientFactory = factory, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    Scenario.create "test" [step1]
    |> Scenario.withClean cleanScenario
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = clientCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun allStats ->
        test <@ List.rev _actionsHistory = ["clean scenario"; "dispose client"] @>

[<Fact>]
let ``InitializedClients should be accessible at scenario clean``() =

    let clientCount = 1
    let mutable initializedClientsAccessible = false
    let clientName = "test_client"

    let factory = ClientFactory.create(
        name = "test_factory",
        initClient = (fun (number,context) -> Task.FromResult clientName),
        clientCount = clientCount
    )

    let cleanScenario (context: IScenarioContext) = task {
        if factory.InitializedClients[0] = clientName then
            initializedClientsAccessible <- true
    }

    let step1 = Step.create("step_1", clientFactory = factory, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    Scenario.create "test" [step1]
    |> Scenario.withClean cleanScenario
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = clientCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun allStats ->
        test <@ initializedClientsAccessible @>

[<Fact>]
let ``should support 2K of clients``() =

    let clientCount = 2_000
    let mutable invokeCount = 0

    let pool = ClientFactory.create(
        name = "test_pool",
        initClient = (fun (number,context) ->
            invokeCount <- invokeCount + 1
            Task.FromResult number),
        clientCount = clientCount
    )

    let step1 = Step.create("step_1", clientFactory = pool, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    Scenario.create "test" [step1]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = clientCount, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun allStats -> test <@ invokeCount = clientCount @>

[<Fact>]
let ``should provide default dispose client``() =

    let mutable defaultDisposeInvoked = false

    let client = {
        new IDisposable with
            member _.Dispose() =
                defaultDisposeInvoked <- true
    }

    let factory = ClientFactory.create(
        name = "factory_1",
        initClient = (fun (number,context) -> Task.FromResult client)
    )

    let step = Step.create("step_1", clientFactory = factory, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    Scenario.create "test" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 100, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> ignore

    test <@ defaultDisposeInvoked = true @>

[<Fact>]
let ``should provide custom dispose client``() =

    let mutable customDisposeInvoked = false

    let factory = ClientFactory.create(
        name = "factory_1",
        initClient = (fun (number,context) -> Task.FromResult 1),
        disposeClient = (fun (client,context) -> task {
            customDisposeInvoked <- true
        })
    )

    let step = Step.create("step_1", clientFactory = factory, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    Scenario.create "test" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 100, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> ignore

    test <@ customDisposeInvoked = true @>

[<Fact>]
let ``should validate factory name``() =

    let factory = ClientFactory.create(
        name = "factory@name", // invalid symbol '@'
        initClient = (fun (number,context) -> Task.FromResult 1)
    )

    let step = Step.create("step_1", clientFactory = factory, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(context.Client)
    })

    Scenario.create "test" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 100, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getError
    |> fun error -> test <@ error.Contains("") @>
