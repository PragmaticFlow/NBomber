module NBomber.IntegrationTests.FeedTests

open Xunit
open NBomber.Domain
open NBomber.Extensions

[<Fact>]
let ``Feed from list``() =
    let xs = [| 0 .. 10 |]
    let length = Array.length xs
    let feed = Feed.ofSeq "test" xs

    Array.init length (fun _ -> feed.Next().["feed.test"])
    |> Array.zip xs
    |> Array.forall (fun (a, b) -> a = b)
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
