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
        |> Scenario.withoutWarmUp

    let scn2 =
        Scenario.create("scn_2", fun ctx -> task {
            do! Task.Delay(milliseconds 100)
            return Response.ok()
        })
        |> Scenario.withInit(fun _ -> task { scn2Started <- true })
        |> Scenario.withLoadSimulations [KeepConstant(1, seconds 1)]
        |> Scenario.withoutWarmUp

    NBomberRunner.registerScenarios [scn1; scn2]
    |> NBomberRunner.withTargetScenarios ["scn_2"]
    |> NBomberRunner.run
    |> ignore

    test <@ scn1Started = false @>
    test <@ scn2Started @>
