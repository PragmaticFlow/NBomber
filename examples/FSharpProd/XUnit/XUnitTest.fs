module FSharpProd.DataFeed.XUnitTest

open System.Threading.Tasks

open Swensen.Unquote
open Xunit

open NBomber
open NBomber.Contracts
open NBomber.FSharp

// in this example we use:
// - XUnit (https://xunit.net/)
// - Unquote (https://github.com/SwensenSoftware/unquote)
// to get more info about test automation, please visit: (https://nbomber.com/docs/test-automation)

[<Fact>]
let ``XUnit test`` () =

    let step = Step.create("simple step", fun _ -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(sizeBytes = 1024)
    })

    let scenario =
        Scenario.create "xunit_hello_world" [step]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 2)]

    let result = NBomberRunner.registerScenarios [scenario]
                 |> NBomberRunner.run

    match result with
    | Ok nodeStats ->
        let stepStats = nodeStats.ScenarioStats[0].StepStats[0]
        //test <@ stepStats.OkCount > 2 @>
        // todo are all of these ok?
        // TODO - how do you get this?
        //test <@ stepStats.Ok.StatusCodes.Length > 2 @>
        test <@ stepStats.Ok.Request.RPS > 8. @>
        test <@ stepStats.Ok.DataTransfer.Percent75 >= 100 @>
        test <@ stepStats.Ok.DataTransfer.MinBytes = 1024 @>
        test <@ stepStats.Ok.DataTransfer.AllBytes >= 17408L @>

    | Error msg -> failwith msg
