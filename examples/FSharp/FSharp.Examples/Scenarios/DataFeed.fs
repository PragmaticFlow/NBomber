module DataFeedScenario

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

let run () =
    let feedVariant = 42
    let feed =
        match feedVariant with
        | 0 -> Feed.empty
        | 1 -> Feed.ofSeq "collection" [ 1 .. 1000000 ]
        | 2 -> Feed.circular "circular" [ 1 .. 10 ]
        | 3 -> Feed.shuffle "random" [| 1 .. 10 |]
        | 4 -> Feed.fromJson "persons" "C:/DataFiles/persons.json"
        | 5 -> Feed.fromFile "account" "C:/DataFiles/session-recording.txt"
               |> Feed.map String.length
        | _ -> Feed.circular "numbers" (seq { 1 .. 10 })

    let step = Step.create("simple step", fun context -> task {
        // to access feed data use feed name as a key
        let number = unbox context.Data.["feed.numbers"]
        printfn "Data from feed: %i" number

        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    let scenario =
        [ step ]
        |> Scenario.create "Hello World!"
        |> Scenario.withFeed feed

    NBomberRunner.registerScenarios [ scenario ]
    |> NBomberRunner.runInConsole
