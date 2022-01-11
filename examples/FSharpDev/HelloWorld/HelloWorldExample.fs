module FSharpDev.HelloWorld.HelloWorldExample

open System.Threading.Tasks

open FSharp.Control.Tasks.NonAffine

open NBomber
open NBomber.Contracts
open NBomber.FSharp

let run () =

    let step1 = Step.create("step_1", fun context -> task {

        // you can do any logic here: go to http, websocket etc
        do! Task.Delay(milliseconds 100)
        return Response.ok() // this value will be passed as response for the next step
    })

    let pause = Step.createPause(milliseconds 100)

    // here you create scenario and define (default) step order
    // you also can define them in opposite direction, like [step2; step1]
    // or even repeat [step1; step1; step1; step2]
    Scenario.create "hello_world_scenario" [step1]
    |> Scenario.withoutWarmUp    
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withTestSuite "example"
    |> NBomberRunner.withTestName "hello_world_test"
    |> NBomberRunner.run
    |> ignore
