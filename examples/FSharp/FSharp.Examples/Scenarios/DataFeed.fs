module DataFeedScenario

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber
open NBomber.Contracts
open NBomber.FSharp

let run () =
    let step = Step.create("simple step", fun context -> task {
        // you can do any logic here: go to http, websocket etc

        let person = context.Data.["persons"]
        printfn "Person from feed: %A" person

        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    let feedVariants = 42
    let feed =
        match feedVariants with
        | 0 -> Feed.ofSeq "just" (seq { 1 .. 10 })
        | 1 -> Feed.circular "circular" [1 .. 10]
        | 2 -> Feed.shuffle "random" [| 1 .. 10 |]
        | 3 -> Feed.randomFloats "random" 1.0 10.0
               |> Feed.map int
        | 4 -> Feed.fromFile "persons" "C:/DataFiles/persons.csv"
               |> Feed.map String.length
        | _ -> Feed.fromJson "persons" "C:/DataFiles/persons.csv"
    let scenario =
        [step]
        |> Scenario.create "Hello World!"
        |> Scenario.withFeed feed

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.runInConsole
