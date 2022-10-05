module Tests.Step.CustomStepOrder

open System.Threading.Tasks

open Xunit
open Swensen.Unquote

open NBomber
open NBomber.Extensions.Internal
open NBomber.Contracts
open NBomber.FSharp

[<Fact>]
let ``withCustomStepOrder should allow to run steps with custom order`` () =

    let step1 = Step.create("step_1", fun context -> task {
        do! Task.Delay(milliseconds 10)
        return Response.ok()
    })

    let step2 = Step.create("step_2", fun context -> task {
        do! Task.Delay(milliseconds 10)
        return Response.ok()
    })

    Scenario.create "1" [step1; step2]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(1, seconds 2)]
    |> Scenario.withCustomStepOrder(fun () -> [| "step_2" |])
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        let stepsStats = stats.GetScenarioStats("1").StepStats
        test <@ stepsStats[0].Ok.Request.Count = 0 @>
        test <@ stepsStats[1].Ok.Request.Count > 0 @>

