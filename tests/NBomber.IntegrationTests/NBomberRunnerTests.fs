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

    let scn1 =
        Scenario.create("scn_1", fun ctx -> task {
            do! Task.Delay(milliseconds 100)
            return Response.ok()
        })
        |> Scenario.withInit(fun _ -> task { scn1Started <- true })
        |> Scenario.withLoadSimulations [KeepConstant(1, seconds 1)]

    let scn2 =
        Scenario.create("scn_2", fun ctx -> task {
            do! Task.Delay(milliseconds 100)
            return Response.ok()
        })
        |> Scenario.withInit(fun _ -> task { scn2Started <- true })
        |> Scenario.withLoadSimulations [KeepConstant(1, seconds 1)]

    NBomberRunner.registerScenarios [scn1; scn2]
    |> NBomberRunner.withTargetScenarios ["scn_2"]
    |> NBomberRunner.run
    |> ignore

    test <@ scn1Started = false @>
    test <@ scn2Started @>

// [<Fact>]
// let ``withDefaultStepTimeout should overrides default step timeout`` () =
//
//     Scenario.create("timeout tests", fun ctx -> task {
//
//         let! st = Step.run("step 1", ctx, fun () -> task {
//             do! Task.Delay(milliseconds 100)
//             return Response.ok()
//         })
//
//         let! st = Step.run("step 2", ctx, fun () -> task {
//             do! Task.Delay(milliseconds 600)
//             return Response.ok()
//         })
//
//         return Response.ok()
//     })
//     |> Scenario.withoutWarmUp
//     |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 5)]
//     |> NBomberRunner.registerScenario
//     |> NBomberRunner.withoutReports
//     |> NBomberRunner.withDefaultStepTimeout(milliseconds 500)
//     |> NBomberRunner.run
//     |> Result.getOk
//     |> fun stats ->
//         test <@ stats.ScenarioStats[0].GetStepStats("step 1").Fail.Request.Count = 0 @>
//         test <@ stats.ScenarioStats[0].GetStepStats("step 2").Fail.Request.Count > 0 @>
//         test <@ stats.ScenarioStats[0].GetStepStats("step 2").Fail.StatusCodes[0].StatusCode = Constants.TimeoutStatusCode @>
