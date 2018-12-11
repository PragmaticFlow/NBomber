module Tests.Step.Push

open System

open Xunit

open NBomber.Contracts
open NBomber.FSharp

[<Fact>]
let ``Ok and Fail should be properly count`` () =
    
    let udaptesChannel = GlobalUpdatesChannel.Instance

    let correlationId = "push_test_0"
    let push1 = Step.createPush("ok")
    let push2 = Step.createPush("fail")

    let updatesTask = async {
        do! Async.Sleep(300)
        udaptesChannel.ReceivedUpdate("warm_up_step", push1.Name, Response.Ok())
        do! Async.Sleep(300)
        udaptesChannel.ReceivedUpdate("warm_up_step", push2.Name, Response.Ok())

        while true do
            do! Async.Sleep(100)
            udaptesChannel.ReceivedUpdate(correlationId, push1.Name, Response.Ok())
            
            do! Async.Sleep(100)
            udaptesChannel.ReceivedUpdate(correlationId, push2.Name, Response.Fail(""))
        return ()
    }

    let assertions = [
       Assertion.forStep("ok", fun stats -> stats.OkCount >= 5)
       Assertion.forStep("fail", fun stats -> stats.FailCount >= 5)
    ]

    let runner =
        Scenario.create("push_test", [push1; push2])
        |> Scenario.withConcurrentCopies(1)
        |> Scenario.withAssertions(assertions)
        |> Scenario.withDuration(TimeSpan.FromSeconds(5.0))
        |> NBomberRunner.registerScenario
    
    Async.Start(updatesTask)
    NBomberRunner.runTest(runner)