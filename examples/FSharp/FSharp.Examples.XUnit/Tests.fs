module Tests

open System
open System.Threading.Tasks

open Xunit
open Swensen.Unquote
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

[<Fact>]
let ``XUnit test`` () =

    let step1 = Step.create("simple step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok(sizeBytes = 1024)
    })

    let scenario =
        Scenario.create "xunit hello world" [step1]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(copiesCount = 1, during = TimeSpan.FromSeconds 2.0)
        ]

    let result = NBomberRunner.registerScenarios [scenario]
                 |> NBomberRunner.run

    match result with
    | Ok nodeStats ->
        let stepStats = nodeStats.ScenarioStats.[0].StepStats.[0]
        test <@ stepStats.OkCount > 2 @>
        test <@ stepStats.RPS > 8 @>
        test <@ stepStats.Percent75 >= 100 @>
        test <@ stepStats.MinDataKb = 1.0 @>
        test <@ stepStats.AllDataMB >= 0.01 @>

    | Error msg -> failwith msg




