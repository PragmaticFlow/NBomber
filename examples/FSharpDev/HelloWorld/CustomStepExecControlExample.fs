module FSharpDev.HelloWorld.CustomStepExecControlExample

open System.Threading.Tasks

open FSharp.Control.Tasks.NonAffine

open NBomber
open NBomber.Contracts
open NBomber.FSharp

let run () =

    let step1 = Step.create("step_1", fun context -> task {
        context.Logger.Information($"{context.StepName} invoked")
        do! Task.Delay(milliseconds 500)
        return Response.ok()
    })

    let step2 = Step.create("step_2", fun context -> task {
        context.Logger.Information($"{context.StepName} invoked")
        do! Task.Delay(milliseconds 500)
        return Response.ok()
    })

    let step3 = Step.create("step_3", fun context -> task {
        context.Logger.Information($"{context.StepName} invoked")
        do! Task.Delay(milliseconds 500)
        return Response.ok()
    })

    Scenario.create "scenario" [step1; step2; step3]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 10)]
    |> Scenario.withCustomStepExecControl(fun execControl ->
        // step_1 will never be invoked
        match execControl with
        | ValueSome exec when not exec.PrevStepResponse.IsError -> ValueSome "step_2"
        | _ -> ValueSome "step_3"
    )
    |> NBomberRunner.registerScenario
    |> NBomberRunner.run
    |> ignore
