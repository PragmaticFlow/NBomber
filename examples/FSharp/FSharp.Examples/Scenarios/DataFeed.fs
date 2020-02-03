module DataFeedScenario

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

let run () =
    let step = Step.create("simple step", fun context -> task {
        // you can do any logic here: go to http, websocket etc

        let number = context.Data.["feed.numbers"]
        printfn "Data from feed: %A" number

        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    let feedVariant = 42
    let feed =
        match feedVariant with
        | 0 -> Feed.empty
        | 1 -> Feed.ofSeq "circular" [1 .. 10]
        | 2 -> Feed.shuffle "random" [| 1 .. 10 |]
        | 3 -> Feed.fromJson "persons" "C:/DataFiles/persons.csv"
        | 4 -> Feed.fromFile "persons" "C:/DataFiles/persons.csv"
               |> Feed.map String.length
        | _ -> Feed.circular "numbers" (seq { 1 .. 10 })

    let scenario =
        [step]
        |> Scenario.create "Hello World!"
        |> Scenario.withFeed feed

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.runInConsole
