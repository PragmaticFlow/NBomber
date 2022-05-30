module Tests.NBomberRunner

open System.Threading.Tasks
open Xunit
open Swensen.Unquote

open NBomber
open NBomber.Extensions.Internal
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Domain

[<Fact>]
let ``withTargetScenarios should run only specified scenarios`` () =

    let mutable scn1Started = false
    let mutable scn2Started = false

    let okStep = Step.create("ok step", fun ctx -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })

    let scn1 =
        Scenario.create "scn_1" [okStep]
        |> Scenario.withInit(fun _ -> task { scn1Started <- true })
        |> Scenario.withLoadSimulations [KeepConstant(1, seconds 1)]

    let scn2 =
        Scenario.create "scn_2" [okStep]
        |> Scenario.withInit(fun _ -> task { scn2Started <- true })
        |> Scenario.withLoadSimulations [KeepConstant(1, seconds 1)]

    NBomberRunner.registerScenarios [scn1; scn2]
    |> NBomberRunner.withTargetScenarios ["scn_2"]
    |> NBomberRunner.run
    |> ignore

    test <@ scn1Started = false @>
    test <@ scn2Started @>

[<Fact>]
let ``withDefaultStepTimeout should overrides default step timeout`` () =

    let step1 = Step.create("step 1", timeout = milliseconds 500, execute = fun context -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })

    let step2 = Step.create("step 2", execute = fun context -> task {
        do! Task.Delay(600)
        return Response.ok()
    })

    Scenario.create "timeout tests" [step1; step2]
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 5)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.withDefaultStepTimeout(milliseconds 500)
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        test <@ stats.ScenarioStats[0].GetStepStats("step 1").Fail.Request.Count = 0 @>
        test <@ stats.ScenarioStats[0].GetStepStats("step 2").Fail.Request.Count > 0 @>
        test <@ stats.ScenarioStats[0].GetStepStats("step 2").Fail.StatusCodes[0].StatusCode = Constants.TimeoutStatusCode @>
