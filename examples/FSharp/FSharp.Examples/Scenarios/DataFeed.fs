module DataFeedScenario

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber
open NBomber.Contracts
open NBomber.FSharp

[<CLIMutable>]
type User = {
    Id: int
    Name: string
}

let run () =

    let data = FeedData.fromSeq [1; 2; 3; 4; 5]
               |> FeedData.shuffleData

    //let data = FeedData.fromJson<User>("users_feed_data.json")
    //let data = FeedData.fromCsv<User>("users_feed_data.csv")

    let feed = Feed.createConstant "numbers" data
    //let feed = Feed.createConstant "numbers" data
    //let feed = Feed.createRandom "numbers" data

    let step = Step.create("simple step", feed, fun context -> task {

        do! Task.Delay(TimeSpan.FromSeconds 1.0)

        context.Logger.Information("Data from feed: {FeedItem}", context.FeedItem)
        return Response.Ok()
    })

    let scenario = Scenario.create "Hello World!" [step]

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.run
    |> ignore
