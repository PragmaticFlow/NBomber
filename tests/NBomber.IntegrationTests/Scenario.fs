module Tests.ScenarioTests

open System
open System.Threading.Tasks

open Xunit
open FSharp.Control.Tasks.V2.ContextInsensitive
open Swensen.Unquote

open NBomber.Contracts
open NBomber.FSharp

[<Fact>]
let ``withTestClean should be invoked only once and not fail runner`` () =

    let mutable invokeCounter = 0

    let testClean = fun _ -> task {
        invokeCounter <- invokeCounter + 1
        failwith "exception was not handled"
    }

    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    let scenario =
        Scenario.create "withTestClean test" [okStep]
        |> Scenario.withTestClean testClean
        //|> Scenario.withDuration(TimeSpan.FromSeconds 1.0)

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.runTest
    |> ignore

    test <@ invokeCounter = 1 @>
