module NBomber.IntegrationTests.CustomStepExecControl

open System
open System.Threading.Tasks

open Xunit
open FsCheck.Xunit
open Swensen.Unquote
open FSharp.Control.Tasks.NonAffine
open Microsoft.Extensions.Configuration

open NBomber
open NBomber.Extensions.InternalExtensions
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.FSharp
open NBomber.Domain
open NBomber.Domain.DomainTypes

[<Fact>]
let ``should work correctly`` () =

    let mutable initialStepCount = 0
    let mutable step1Count = 0
    let mutable step2Count = 0

    let step1 = Step.create("step_1", fun context -> task {
        step1Count <- step1Count + 1
        do! Task.Delay(milliseconds 10)
        return Response.ok()
    })

    let step2 = Step.create("step_2", fun context -> task {
        step2Count <- step2Count + 1
        do! Task.Delay(milliseconds 10)
        return Response.ok()
    })

    Scenario.create "scenario" [step1; step2]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 1)]
    |> Scenario.withCustomStepExecControl(fun context ->
        match context with
        | ValueSome ctx ->
            if ctx.PrevStepResponse.IsError then ValueNone
            else ValueSome "step_1"

        | ValueNone ->
            initialStepCount <- initialStepCount + 1
            ValueSome "step_1"
    )
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./steps-tests/11/"
    |> NBomberRunner.run
    |> ignore

    test <@ initialStepCount = 1 @>
    test <@ step1Count > 0 @>
    test <@ step2Count = 0 @>

[<Fact>]
let ``should restart execution when ValueNone returned`` () =

    let mutable counter = 0

    let step1 = Step.create("step_1", fun context -> task {
        do! Task.Delay(milliseconds 10)
        return Response.ok()
    })

    let step2 = Step.create("step_2", fun context -> task {
        do! Task.Delay(milliseconds 10)
        return Response.ok()
    })

    Scenario.create "scenario" [step1; step2]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 1)]
    |> Scenario.withCustomStepExecControl(fun context ->
        match context with
        | ValueSome c ->
            counter <- counter + 1
            ValueSome c.PrevStepContext.StepName

        | ValueNone   -> ValueNone
    )
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./steps-tests/11/"
    |> NBomberRunner.run
    |> ignore

    test <@ counter = 0 @>

[<Fact>]
let ``should restart execution when invalid step name returned`` () =

    let mutable counter = 0

    let step1 = Step.create("step_1", fun context -> task {
        do! Task.Delay(milliseconds 10)
        return Response.ok()
    })

    let step2 = Step.create("step_2", fun context -> task {
        do! Task.Delay(milliseconds 10)
        return Response.ok()
    })

    Scenario.create "scenario" [step1; step2]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 1)]
    |> Scenario.withCustomStepExecControl(fun context -> ValueSome "invalid_name")
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./steps-tests/11/"
    |> NBomberRunner.run
    |> ignore

    test <@ counter = 0 @>

[<Fact>]
let ``should correctly populate previous step response`` () =

    let mutable step1RespReceived = false
    let mutable step2RespReceived = false

    let step1 = Step.create("step_1", fun context -> task {
        do! Task.Delay(milliseconds 10)
        return Response.ok(sizeBytes = 20)
    })

    let step2 = Step.create("step_2", fun context -> task {
        do! Task.Delay(milliseconds 10)
        return Response.fail(latencyMs = 10)
    })

    Scenario.create "scenario" [step1; step2]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 1)]
    |> Scenario.withCustomStepExecControl(fun context ->
        match context with
        | ValueSome c when c.PrevStepContext.StepName = "step_1" && not c.PrevStepResponse.IsError && c.PrevStepResponse.SizeBytes = 20 ->
            step1RespReceived <- true
            ValueSome "step_2"

        | ValueSome c when c.PrevStepContext.StepName = "step_2" && c.PrevStepResponse.LatencyMs = 10 && c.PrevStepResponse.IsError ->
            step2RespReceived <- true
            ValueNone

        | ValueSome _ -> failwith "bug"
        | ValueNone   -> ValueSome "step_1"
    )
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./steps-tests/11/"
    |> NBomberRunner.run
    |> ignore

    test <@ step1RespReceived = true @>
    test <@ step2RespReceived = true @>

[<Fact>]
let ``should allow jumping into step even in case of previous step error`` () =

    let mutable step3Invoked = false

    let step1 = Step.create("step_1", fun context -> task {
        do! Task.Delay(milliseconds 10)
        return Response.ok(sizeBytes = 20)
    })

    let step2 = Step.create("step_2", fun context -> task {
        do! Task.Delay(milliseconds 10)
        return Response.fail(latencyMs = 10) // by default, step_3 shouldn't be invoked
    })

    let step3 = Step.create("step_3", fun context -> task {
        do! Task.Delay(milliseconds 10)
        return Response.ok(latencyMs = 10)
    })

    Scenario.create "scenario" [step1; step2; step3]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 1)]
    |> Scenario.withCustomStepExecControl(fun context ->
        match context with
        | ValueSome c when c.PrevStepContext.StepName = "step_1" -> ValueSome "step_2"
        | ValueSome c when c.PrevStepContext.StepName = "step_2" -> ValueSome "step_3"
        | ValueSome c when c.PrevStepContext.StepName = "step_3" ->
            step3Invoked <- true
            ValueNone

        | ValueSome _ -> failwith "bug"
        | ValueNone   -> ValueSome "step_1"
    )
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withReportFolder "./steps-tests/11/"
    |> NBomberRunner.run
    |> ignore

    test <@ step3Invoked = true @>
