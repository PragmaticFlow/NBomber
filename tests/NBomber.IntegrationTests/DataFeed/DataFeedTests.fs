module Tests.Feed

open System
open System.Threading
open System.Threading.Tasks

open Xunit
open FsCheck
open FsCheck.Xunit
open Swensen.Unquote
open FSharp.Control.Tasks.NonAffine

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Extensions.InternalExtensions
open NBomber.DomainServices
open NBomber.FSharp
open NBomber.Infra.Dependency
open Tests.TestHelper

[<CLIMutable>]
type User = {
    Id: int
    Name: string
}

let createBaseContext () =
    let nodeInfo = NodeInfo.init()
    let token = CancellationToken.None
    let dep = Dependency.createFor NodeType.SingleNode
    NBomberContext.createBaseContext(dep.TestInfo, nodeInfo, token, dep.Dep.Logger)

[<Property>]
let ``createCircular iterate over array sequentially``(length: int) =

    length > 10 ==> lazy

    let orderedList = [0 .. length - 1]

    let feed = orderedList |> Feed.createCircular "circular"

    let context = createBaseContext()
    feed.Init(context).Wait()

    let iterateLength = length + length // increase original length

    let actual = List.init iterateLength (fun i ->
        let scenarioInfo = NBomber.Domain.Scenario.createScenarioInfo("test_scn", seconds 10, i)
        feed.GetNextItem(scenarioInfo, null)
    )

    let final = orderedList |> List.append(orderedList)

    test <@ actual = final @>

[<Property>]
let ``createConstant returns next value from seq for the same scenarioThreadId``(length: int) =

    length > 10 ==> lazy

    let orderedList = [0 .. length - 1]
    let sameValues = orderedList |> List.map(fun i -> i, i)

    let feed = orderedList |> Feed.createCircular "circular"

    let context = createBaseContext()
    feed.Init(context).Wait()

    let actual = List.init length (fun i ->
        let scenarioInfo = NBomber.Domain.Scenario.createScenarioInfo("test_scn", seconds 10, i)
        feed.GetNextItem(scenarioInfo, null), feed.GetNextItem(scenarioInfo, null)
    )

    test <@ actual <> sameValues @>

[<Property>]
let ``createConstant returns the same value for the same scenarioInfo``(length: int) =

    length > 10 ==> lazy

    let orderedList = [0 .. length - 1]
    let sameValues = orderedList |> List.map(fun i -> i, i)

    let feed = orderedList |> Feed.createConstant "constant"

    let context = createBaseContext()
    feed.Init(context).Wait()

    let actual = List.init length (fun i ->
        let scenarioInfo = NBomber.Domain.Scenario.createScenarioInfo("test_scn", seconds 10, i)
        feed.GetNextItem(scenarioInfo, null), feed.GetNextItem(scenarioInfo, null)
    )

    test <@ actual = sameValues @>

[<Fact>]
let ``createRandom returns the random numbers list for each full iteration``() =

    let numbers = [1;2;3;4;5;6;7;8]

    let feed1 = numbers |> Feed.createRandom "random"
    let feed2 = numbers |> Feed.createRandom "random"

    let context = createBaseContext()
    feed1.Init(context).Wait()
    feed2.Init(context).Wait()

    let actual1 = List.init numbers.Length (fun i ->
        let scenarioInfo = NBomber.Domain.Scenario.createScenarioInfo("test_scn", seconds 10, i)
        feed1.GetNextItem(scenarioInfo, null)
    )

    let actual2 = List.init numbers.Length (fun i ->
        let scenarioInfo = NBomber.Domain.Scenario.createScenarioInfo("test_scn", seconds 10, i)
        feed2.GetNextItem(scenarioInfo, null)
    )

    test <@ actual1 <> actual2 @>

[<Property>]
let ``provides infinite iteration``(numbers: int list, iterationTimes: uint32) =

    (numbers.Length > 0 && numbers.Length < 200 && iterationTimes > 0u && iterationTimes < 5000u) ==> lazy

    let scenarioInfo = NBomber.Domain.Scenario.createScenarioInfo("test_scn", seconds 10, numbers.Length)

    let circular = numbers |> Feed.createCircular "circular"
    let constant = numbers |> Feed.createConstant "constant"
    let random   = numbers |> Feed.createRandom "random"

    let context = createBaseContext()
    circular.Init(context).Wait()
    constant.Init(context).Wait()
    random.Init(context).Wait()

    for i = 0 to int iterationTimes do
        circular.GetNextItem(scenarioInfo, null) |> ignore
        constant.GetNextItem(scenarioInfo, null) |> ignore
        random.GetNextItem(scenarioInfo, null) |> ignore

[<Fact>]
let ``FeedData.fromJson works correctly``() =

    let data = FeedData.fromJson<User> "./DataFeed/users-feed-data.json"
    let users = data |> Seq.toArray

    test <@ users.Length > 0 @>

[<Fact>]
let ``FeedData.fromCsv works correctly``() =

    let data = FeedData.fromCsv<User> "./DataFeed/users-feed-data.csv"
    let users = data |> Seq.toArray

    test <@ users.Length > 0 @>

[<Fact>]
let ``FeedData fromSeq should support lazy initialize``() =

    let mutable data = [-1; -2; -3]

    let feed = Feed.createRandomLazy "my_feed" (fun context -> seq { yield! data })

    data <- [1; 2; 3]

    let step = Step.create("ok step", feed = feed, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)
        if context.FeedItem > 0 then return Response.ok()
        else return Response.fail()
    })

    Scenario.create "feed test scenario" [step]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 10)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats -> test <@ stats.FailCount = 0 @>

[<Fact>]
let ``Feed with the same name should be supported``() =

    let mutable feed_1_initCount = 0
    let mutable feed_2_initCount = 0

    let feed1 = { new IFeed<int> with
        member _.FeedName = "same_name"

        member _.Init(context) =
            feed_1_initCount <- feed_1_initCount + 1
            Task.CompletedTask

        member _.GetNextItem(scenarioInfo, stepData) = 1
    }

    let feed2 = { new IFeed<int> with
        member _.FeedName = "same_name"

        member _.Init(context) =
            feed_2_initCount <- feed_2_initCount + 1
            Task.CompletedTask

        member _.GetNextItem(scenarioInfo, stepData) = 1
    }

    let step1 = Step.create("step_1", feed = feed1, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })

    let step2 = Step.create("step_2", feed = feed2, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })

    let scn1 =
        Scenario.create "scenario_1" [step1]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 5)]

    let scn2 =
        Scenario.create "scenario_2" [step2]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 5)]

    NBomberRunner.registerScenarios [scn1; scn2]
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        test <@ feed_1_initCount = 1 @>
        test <@ feed_2_initCount = 1 @>

[<Fact>]
let ``Init for the same instance shared btw steps and scenarios should be invoked only once``() =

    let mutable feedInitCount = 0

    let feed = { new IFeed<int> with
        member _.FeedName = "same_name"

        member _.Init(context) =
            feedInitCount <- feedInitCount + 1
            Task.CompletedTask

        member _.GetNextItem(scenarioInfo, stepData) = 1
    }

    let step1 = Step.create("step_1", feed = feed, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })

    let step2 = Step.create("step_2", feed = feed, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })

    let scn1 =
        Scenario.create "scenario_1" [step1; step2]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 5)]

    let scn2 =
        Scenario.create "scenario_2" [step1]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 5)]

    NBomberRunner.registerScenarios [scn1; scn2]
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats -> test <@ feedInitCount = 1 @>
