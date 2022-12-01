﻿module Tests.Scenario.EmptyScenarioTests

open System.Threading.Tasks
open Xunit
open Swensen.Unquote
open NBomber
open NBomber.Errors
open NBomber.FSharp
open NBomber.Extensions.Internal
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Domain

[<Fact>]
let internal ``check that EmptyScenario should fail if it has no Init and no Clean`` () =
    Scenario.empty "my_empty_scenario"
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.runWithResult []
    |> Result.getError
    |> function
        | Scenario error ->
            match error with
            | EmptyScenarioWithEmptyInitAndClean _ -> ()
            | _ -> failwith "invalid error type"

        | _ -> failwith "invalid error type"

[<Fact>]
let ``check that EmptyScenario should be ok if it has no Init but Clean function exist`` () =

    let mutable cleanInvoked = false

    Scenario.empty "my_empty_scenario"
    |> Scenario.withClean(fun _ -> task {
        cleanInvoked <- true
        return ()
    })
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        test <@ cleanInvoked @>

[<Fact>]
let ``check that EmptyScenario should be ok if it has no Clean but Init function exist`` () =

    let mutable initInvoked = false

    Scenario.empty "my_empty_scenario"
    |> Scenario.withInit(fun _ -> task {
        initInvoked <- true
        return ()
    })
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        test <@ initInvoked @>

[<Fact>]
let ``check that EmptyScenario is not available in FinalStats`` () =

    let mutable initInvoked = false

    let scn1 =
        Scenario.create("scenario_1", fun ctx -> task {
            do! Task.Delay(milliseconds 10)
            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(1, seconds 1)]

    let emptyScn =
        Scenario.empty "my_empty_scenario"
        |> Scenario.withInit(fun _ -> task {
            initInvoked <- true
            return ()
        })

    NBomberRunner.registerScenarios [scn1; emptyScn]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        test <@ initInvoked @>
        test <@ stats.ScenarioStats.Length = 1 @>
        test <@ stats.ScenarioStats[0].ScenarioName = "scenario_1" @>

