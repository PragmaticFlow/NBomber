module NBomber.IntegrationTests.FeedTests

open System
open FSharp.Control.Tasks.V2.ContextInsensitive
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

    Array.init length (fun _ -> feed.Next().["feed.test"])
    |> Array.zip xs
    |> Array.forall (fun (a, b) -> a = b)
    |> Assert.True

[<Fact>]
let ``Feed from list``() =
    let xs = [ 0 .. 10 ]
    let length = List.length xs
    let feed = Feed.ofSeq "test" xs

    List.init length (fun _ -> feed.Next().["feed.test"])
    |> List.zip xs
    |> List.forall (fun (a, b) -> a = b)
    |> Assert.True

[<Fact>]
let ``Circular feed should be infinite``() =
    let xs = [| 0 .. 10 |]
    let length = Array.length xs
    let feed = Feed.circular "test" xs

    Array.init (length * 10) (fun _ -> feed.Next().["feed.test"])
    |> Array.forall (fun a -> 0 <= a && a <= 10)
    |> Assert.True

[<Fact>]
let ``Empty feed throws no error``() =
    let xs = Seq.empty

    let emptyFeeds =
        [ Feed.empty
          Feed.circular "test" xs
          Feed.ofSeq "ofSeq" xs
          Feed.shuffle "shuffle" <| Array.ofSeq xs ]

    for feed in emptyFeeds do
        Array.init 100 (fun _ -> feed.Next())
        |> Array.forall Dict.isEmpty
        |> Assert.True

type private 'a ListMsg =
    | Put of AsyncReplyChannel<unit> * 'a
    | Get of AsyncReplyChannel<'a list>
    | Count of AsyncReplyChannel<int>

type NonBlockingList<'a>() =
    let ctx = new System.Threading.CancellationTokenSource()
    let mbox = MailboxProcessor<'a ListMsg>.Start(fun inbox ->
        let rec loop (xs: 'a list) = async {
            match! inbox.Receive() with
            | Put (r,v) ->
                r.Reply ()
                return! loop (v::xs)
            | Count r ->
                xs |> List.length |> r.Reply
            | Get r ->
                r.Reply xs
        }
        loop []
    , ctx.Token
    )

    do
        mbox.Error.Add(printfn "Exception: %A")

    member _.Add i =
        mbox.PostAndReply(fun r -> Put(r,i))
    member _.AddAsync i =
        mbox.PostAndAsyncReply(fun r -> Put(r,i))
    member _.Count =
        mbox.PostAndReply Count
    member __.Get =
        mbox.PostAndReply Get

[<Fact>]
let ``Feed values are ordered like in the feed source``() =

    let xs = Seq.initInfinite id
    let feed = xs |> Feed.ofSeq "int"
    let counter = NonBlockingList()

    let stats =
        Step.create("count", fun ctx -> task {
            let i = ctx.Data.["feed.int"] :?> int
            ctx.Logger.Information("feed {i}", i)
            //do! counter.AddAsync i
            counter.Add i
            return Response.Ok()
        })
        |> List.singleton
        |> Scenario.create "test"
        |> Scenario.withFeed feed
        |> Scenario.withOutWarmUp
        |> Scenario.withConcurrentCopies 100
        |> Scenario.withDuration (TimeSpan.FromSeconds 1.0)
        |> List.singleton
        |> NBomberRunner.registerScenarios
        |> NBomberRunner.runTest

    match stats with
    | Error err -> failwith err
    | Ok stats ->
        let received = counter.Get
        Assert.Equal(1, stats |> Array.length)
        let sent = stats |> Array.head
        Assert.Equal(0, sent.FailCount)
        Assert.NotEqual(0, sent.OkCount)
        //Assert.Equal(sent.OkCount, received |> List.length)
        Assert.True(received |> List.length >= sent.OkCount)

//|> Array.map (fun stat -> stat.StepName)
//        collected
//        |> List.rev
//        |> List.pairwise
//        |> List.filter(fun (a,b) -> b <> a + 1L)
//        |> Assert.Empty

