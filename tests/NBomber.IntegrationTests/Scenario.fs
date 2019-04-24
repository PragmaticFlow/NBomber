module Tests.Scenario

open System
open System.Threading.Tasks

open Xunit
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.Domain
open NBomber.FSharp

[<Fact>]
let ``withTestClean should be invoked only once and not fail runner`` () =
    
    let mutable invokeCounter = 0

    let testClean = fun _ -> task {
        invokeCounter <- invokeCounter + 1
        failwith "exception was not handled"        
    }

    let pool = ConnectionPool.none

    let okStep = Step.create("ok step", pool, fun context -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    Scenario.create "withTestClean test" [okStep]
    |> Scenario.withTestClean testClean
    |> Scenario.withDuration(TimeSpan.FromSeconds 1.0)
    |> NBomberRunner.registerScenario
    |> NBomberRunner.runTest

    Assert.Equal(1, invokeCounter)