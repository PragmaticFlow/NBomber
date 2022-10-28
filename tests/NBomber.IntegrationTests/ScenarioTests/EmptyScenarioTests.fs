module Tests.Scenario.EmptyScenarioTests

open System.Threading.Tasks

open Xunit
open Swensen.Unquote

open NBomber
open NBomber.FSharp
open NBomber.Extensions.Internal
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Domain
open NBomber.Domain.DomainTypes

[<Fact>]
let internal ``check that EmptyScenario should fail if it has no Init and no Clean`` () =
    Scenario.empty "my_empty_scenario"
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.runWithResult []
    |> Result.getError
    |> function
        | Errors.Validation error ->

            match error with
            | Errors.EmptyScenarioWithEmptyInitAndClean _ -> ()
            | _ -> failwith "invalid error type"

        | _ -> failwith "invalid error type"

[<Fact>]
let ``check that EmptyScenario should be ok if it has no Init but Clean function exist`` () =
    Scenario.empty "my_empty_scenario"
    |> Scenario.withClean(fun _ -> task { return () })
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> ignore

[<Fact>]
let ``check that EmptyScenario should be ok if it has no Clean but Init function exist`` () =
    Scenario.empty "my_empty_scenario"
    |> Scenario.withInit(fun _ -> task { return () })
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> ignore

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

