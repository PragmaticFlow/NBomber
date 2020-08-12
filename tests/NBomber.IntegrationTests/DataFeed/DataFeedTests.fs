module Tests.Feed

open Xunit
open FsCheck
open FsCheck.Xunit
open Swensen.Unquote

open NBomber
open NBomber.Domain

[<CLIMutable>]
type User = {
    Id: int
    Name: string
}

[<Property>]
let ``createCircular iterate over array sequentially``(length: int) =

    length > 10 ==> lazy

    let orderedList = [0 .. length - 1]

    let feed = FeedData.fromSeq orderedList
               |> Feed.createCircular "circular"

    let actual = List.init length (fun i ->
        let correlationId = Scenario.createCorrelationId("test_scn", i)
        feed.GetNextItem(correlationId, null)
    )

    test <@ actual = orderedList @>

[<Property>]
let ``createConstant returns next value from seq for the same correlationId``(length: int) =

    length > 10 ==> lazy

    let orderedList = [0 .. length - 1]
    let sameValues = orderedList |> List.map(fun i -> i, i)

    let feed = FeedData.fromSeq orderedList
               |> Feed.createCircular "circular"

    let actual = List.init length (fun i ->
        let correlationId = Scenario.createCorrelationId("test_scn", i)
        feed.GetNextItem(correlationId, null), feed.GetNextItem(correlationId, null)
    )

    test <@ actual <> sameValues @>

[<Property>]
let ``createConstant returns the same value for the same correlationId``(length: int) =

    length > 10 ==> lazy

    let orderedList = [0 .. length - 1]
    let sameValues = orderedList |> List.map(fun i -> i, i)

    let feed = FeedData.fromSeq orderedList
               |> Feed.createConstant "constant"

    let actual = List.init length (fun i ->
        let correlationId = Scenario.createCorrelationId("test_scn", i)
        feed.GetNextItem(correlationId, null), feed.GetNextItem(correlationId, null)
    )

    test <@ actual = sameValues @>

[<Fact>]
let ``createRandom returns the random numbers list for each full iteration``() =

    let numbers = [1;2;3;4;5;6;7;8]

    let feed1 = FeedData.fromSeq numbers
                |> Feed.createRandom "random"

    let feed2 = FeedData.fromSeq numbers
                |> Feed.createRandom "random"

    let actual1 = List.init numbers.Length (fun i ->
        let correlationId = Scenario.createCorrelationId("test_scn", i)
        feed1.GetNextItem(correlationId, null)
    )

    let actual2 = List.init numbers.Length (fun i ->
        let correlationId = Scenario.createCorrelationId("test_scn", i)
        feed2.GetNextItem(correlationId, null)
    )

    test <@ actual1 <> actual2 @>

[<Property>]
let ``provides infinite iteration``(numbers: int list, iterationTimes: uint32) =

    (numbers.Length > 0 && numbers.Length < 200 && iterationTimes > 0u && iterationTimes < 5000u) ==> lazy

    let data = FeedData.fromSeq numbers

    let correlationId = Scenario.createCorrelationId("test_scn", numbers.Length)

    let circular = data |> Feed.createCircular "circular"
    let constant = data |> Feed.createConstant "constant"
    let random   = data |> Feed.createRandom "random"

    for i = 0 to int iterationTimes do
        circular.GetNextItem(correlationId, null) |> ignore
        constant.GetNextItem(correlationId, null) |> ignore
        random.GetNextItem(correlationId, null) |> ignore

[<Fact>]
let ``FeedData.fromJson works correctly``() =

    let data = FeedData.fromJson<User> "./DataFeed/users-feed-data.json"
    let users = data.GetAllItems()

    test <@ users.Length > 0 @>

[<Fact>]
let ``FeedData.fromCsv works correctly``() =

    let data = FeedData.fromCsv<User> "./DataFeed/users-feed-data.csv"
    let users = data.GetAllItems()

    test <@ users.Length > 0 @>
