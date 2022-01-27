module Tests.ClientDistribution

open System
open System.Threading.Tasks

open Xunit
open Swensen.Unquote
open FsToolkit.ErrorHandling
open FSharp.Control.Tasks.NonAffine

open NBomber
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Extensions.InternalExtensions

[<Fact>]
let ``should allow distribute client with custom distribution``() =

    let factory = ClientFactory.create(
        name = "test_pool",
        initClient = (fun (number,context) -> Task.FromResult number),
        clientCount = 10
    )

    let step = Step.create("step",
                           clientFactory = factory,
                           clientDistribution = (fun context -> 5), // always return client with ID = 5
                           execute = fun context -> task {

        do! Task.Delay(milliseconds 100)

        return
            if context.Client = 5 then Response.ok()
            else Response.fail()
    })

    Scenario.create "test" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 100, during = seconds 3)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let stepStats = nodeStats.ScenarioStats[0]
        test <@ stepStats.OkCount > 0 @>
        test <@ stepStats.FailCount = 0 @>

[<Fact>]
let ``should support default distribution = ScenarioInfo.ThreadNumber % InitializedClients.Length``() =

    let clientCount = 50

    let factory = ClientFactory.create(
        name = "test_pool",
        initClient = (fun (number,context) -> Task.FromResult number),
        clientCount = clientCount
    )

    let step = Step.create("step",
                           clientFactory = factory,
                           execute = fun context -> task {

        do! Task.Delay(milliseconds 100)

        let clientId = context.ScenarioInfo.ThreadNumber % clientCount

        return
            if context.Client = clientId then Response.ok()
            else Response.fail()
    })

    Scenario.create "test" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 100, during = seconds 5)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let stepStats = nodeStats.ScenarioStats[0]
        test <@ stepStats.OkCount > 0 @>
        test <@ stepStats.FailCount = 0 @>

[<Fact>]
let ``should validate if ClientFactory.IsNone && ClientDistribution.IsSome``() =

    try
        Step.create("step",
                    clientDistribution = (fun context -> 5),
                    execute = fun context -> task {

            do! Task.Delay(milliseconds 100)

            return Response.ok()
        })
        |> ignore

        failwith "validation bug"

    with
    | ex ->
        test <@ ex.Message.Contains("clientFactory") @>

[<Fact>]
let ``should provide corresponding FeedItem``() =

    let feed = [0;1;2;3;4;5] |> Feed.createCircular "test"

    let factory = ClientFactory.create(
        name = "test_pool",
        initClient = (fun (number,context) -> Task.FromResult number),
        clientCount = 6
    )

    let step = Step.create("step",
                           feed = feed,
                           clientFactory = factory,
                           clientDistribution = (fun context -> context.FeedItem),
                           execute = fun context -> task {

        do! Task.Delay(milliseconds 100)

        if context.Client = context.FeedItem
            then return Response.ok()
        else
            return Response.fail()
    })

    Scenario.create "test" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let stepStats = nodeStats.ScenarioStats[0]
        test <@ stepStats.OkCount > 0 @>
        test <@ stepStats.FailCount = 0 @>

[<Fact>]
let ``should handle errors without stopping the test``() =

    let factory = ClientFactory.create(
        name = "test_pool",
        initClient = (fun (number,context) -> Task.FromResult number),
        clientCount = 5
    )

    let step = Step.create("step",
                           clientFactory = factory,
                           clientDistribution = (fun context ->
                               failwith "client exception"
                               context.FeedItem
                           ),
                           execute = fun context -> task {

        do! Task.Delay(milliseconds 100)

        if context.Client = context.FeedItem
            then return Response.ok()
        else
            return Response.fail()
    })

    Scenario.create "test" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 1)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let stepStats = nodeStats.ScenarioStats[0]
        test <@ stepStats.OkCount = 0 @>
        test <@ stepStats.FailCount > 0 @>
