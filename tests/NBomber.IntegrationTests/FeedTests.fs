module NBomber.IntegrationTests.FeedTests

open System
open FSharp.Control.Tasks.V2.ContextInsensitive
open Swensen.Unquote
open Xunit
open NBomber.Domain
open NBomber.Contracts
open NBomber.Extensions
open NBomber.FSharp

[<Fact>]
let ``Feed from array``() =
    let xs = [| 0 .. 10 |]
    let length = Array.length xs
    let feed = Feed.ofSeq "test" xs

    let actual = Array.init length (fun _ -> feed.GetNext().["feed.test"])
    test <@ actual
            |> Array.pairwise
            |> Array.forall(fun (a,b) -> a <> b) @>
    test <@ actual
            |> Array.zip xs
            |> Array.forall (fun (a, b) -> a = b) @>

[<Fact>]
let ``Feed from list``() =
    let xs = [ 0 .. 10 ]
    let length = List.length xs
    let feed = Feed.ofSeq "test" xs

    let actual = List.init length (fun _ -> feed.GetNext().["feed.test"])
    test <@ actual
            |> List.pairwise
            |> List.forall(fun (a,b) -> a <> b) @>
    test <@ actual
            |> List.zip xs
            |> List.forall (fun (a, b) -> a = b) @>

[<Fact>]
let ``Feed from infinite sequence``() =
    let xs = Seq.initInfinite id
    let feed = Feed.ofSeq "test" xs

    let actual = List.init 10000 (fun _ -> feed.GetNext().["feed.test"])
    test <@ actual
            |> List.pairwise
            |> List.forall(fun (a,b) -> a <> b) @>
    test <@ actual
            |> List.zip (xs |> Seq.truncate 10000 |> Seq.toList)
            |> List.forall (fun (a, b) -> a = b) @>

[<Fact>]
let ``Circular feed from infinite sequence``() =
    let xs = Seq.initInfinite id
    let feed = Feed.circular "test" xs

    let actual = List.init 10000 (fun _ -> feed.GetNext().["feed.test"])
    test <@ actual
            |> List.pairwise
            |> List.forall(fun (a,b) -> a <> b) @>
    test <@ actual
            |> List.zip (xs |> Seq.truncate 10000 |> Seq.toList)
            |> List.forall (fun (a, b) -> a = b) @>

[<Fact>]
let ``Circular feed from array``() =
    let xs = [| 0 .. 10 |]
    let length = 10 * Array.length xs
    let feed = Feed.circular "test" xs

    let zs =
        seq { while true do yield! xs }
        |> Seq.truncate length
        |> Seq.toArray

    test <@ Array.init length (fun _ -> feed.GetNext().["feed.test"])
            |> Array.zip zs
            |> Array.forall (fun (a, b) -> a = b && 0 <= a && a <= 10) @>

[<Fact>]
let ``Feed select from array feed``() =
    let xs = [| 0 .. 10 |]
    let length = 10 * Array.length xs
    let f i = i + 1
    let feed = Feed.circular "test" xs |> Feed.map f

    let zs =
        seq { while true do yield! xs }
        |> Seq.map f
        |> Seq.truncate length
        |> Seq.toArray

    test <@ Array.init length (fun _ -> feed.GetNext().["feed.test"])
            |> Array.zip zs
            |> Array.forall (fun (a, b) -> a = b && 1 <= a && a <= 11) @>

[<Fact>]
let ``Empty feed throws no errors``() =
    let xs = Seq.empty

    let emptyFeeds =
        [| Feed.empty
           Feed.circular "test" xs
           Feed.ofSeq "ofSeq" xs
           Feed.shuffle "shuffle" <| Array.ofSeq xs |]

    test <@ emptyFeeds
            |> Array.forall (fun feed ->
                Array.init 100 (fun _ -> feed.GetNext())
                |> Array.forall Dict.isEmpty )
         @>

[<Fact>]
let ``Feed values are same within one scenario``() =
    let feed = Seq.initInfinite id |> Feed.ofSeq "int"
    let sending1 = ref []
    let sending2 = ref []

    let step1 =
        Step.create("step1", fun ctx -> task {
            let j = ctx.Data.["feed.int"] :?> int
            ctx.Logger.Information("feed {i}", j)
            sending1 := j::!sending1
            return Response.Ok()
        })

    let step2 =
        Step.create("step2", fun ctx -> task {
            let k = ctx.Data.["feed.int"] :?> int
            ctx.Logger.Information("feed {i}", k)
            sending2 := k::!sending2
            return Response.Ok()
        })

    let scenario =
        [step1;step2]
        |> Scenario.create "feed test"
        |> Scenario.withFeed feed
        |> Scenario.withOutWarmUp
        |> Scenario.withConcurrentCopies 1
        |> Scenario.withDuration (TimeSpan.FromSeconds 10.0)

    let stats =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.runTest

    match stats with
    | Error err -> failwith err
    | Ok stats ->
        test <@ Array.length stats = 2 @>
        test <@ stats.[0].FailCount = 0 @>
        test <@ stats.[0].OkCount <> 0 @>
        test <@ stats.[1].FailCount = 0 @>
        test <@ stats.[1].OkCount <> 0 @> // TODO step 2 stats are empty ?!

        let received1 = !sending1
        let received2 = !sending2
        test <@ List.length received1 = List.length received2 @>

        test <@ received1
                |> List.zip received2
                |> List.forall(fun (i1,i2) -> i1 = i2) @>

[<Fact>]
let ``Feed values are ordered like in the feed source``() =
    let feed = Seq.initInfinite id |> Feed.ofSeq "int"
    let sending = ref []
    let step =
        Step.create("count feed data", fun ctx -> task {
            let i = ctx.Data.["feed.int"] :?> int
            ctx.Logger.Information("feed {i}", i)
            sending := i::!sending
            return Response.Ok()
        })

    let scenario =
        [step]
        |> Scenario.create "feed test"
        |> Scenario.withFeed feed
        |> Scenario.withOutWarmUp
        |> Scenario.withConcurrentCopies 1
        |> Scenario.withDuration (TimeSpan.FromSeconds 10.0)

    let stats =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.runTest

    match stats with
    | Error err -> failwith err
    | Ok stats ->
        test <@ Array.length stats = 1 @>
        let sent = stats |> Array.head
        test <@ sent.OkCount <> 0 @>
        test <@ sent.FailCount = 0 @>

        let received = !sending
        test <@ received |> List.isEmpty |> not @>
        test <@ received |> List.length >= sent.OkCount @> // TODO actually should be equal ?
        test <@ received
                |> List.rev
                |> List.mapi (fun i a -> i,a)
                |> List.forall(fun (a,b) -> a = b) @>
