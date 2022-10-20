module FSharpDev.HelloWorld.HelloWorldExample

open System
open System.Threading.Tasks
open NBomber
open NBomber.Contracts
open NBomber.FSharp

let run () =

    Scenario.create("flow", fun ctx -> task {

        // let ex = Exception("dssadsa")
        // let dd = FlowResponse.fail()
        // let dd = FlowResponse.fail(ex)
        // let dd = FlowResponse.fail("asdasd")

        let! response1 = Step.run("step_1", ctx, fun () -> task {
            return FlowResponse.ok("")
        })

        let! response1 = Step.run("step_1", ctx, fun () -> task {
            return FlowResponse.fail("")
        })

        return FlowResponse.ok()
    })
    |> Scenario.withoutWarmUp
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withTestSuite "example"
    |> NBomberRunner.withTestName "hello_world_test"
    |> NBomberRunner.run
    |> ignore
