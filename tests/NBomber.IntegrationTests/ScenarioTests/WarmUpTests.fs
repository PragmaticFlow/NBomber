module Tests.Scenario.WarmUpTests

open System.Threading.Tasks

open Serilog
open Serilog.Sinks.InMemory
open Swensen.Unquote
open Xunit

open NBomber
open NBomber.FSharp
open NBomber.Extensions.Internal
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Errors
open NBomber.Domain

[<Fact>]
let ``Warmup should have no effect on stats`` () =

    Scenario.create("warmup test", fun ctx -> task {

        let! okStep = Step.run("ok step", ctx, fun _ -> task {
            do! Task.Delay(milliseconds 100)
            return Response.ok()
        })

        let! failStep = Step.run("fail step", ctx, fun _ -> task {
            do! Task.Delay(milliseconds 100)
            return Response.fail()
        })

        return Response.ok()
    })
    |> Scenario.withWarmUpDuration(seconds 1)
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 1)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        let okSt = nodeStats.ScenarioStats[0].GetStepStats("ok step")
        let failSt = nodeStats.ScenarioStats[0].GetStepStats("fail step")

        test <@ okSt.Ok.Request.Count <= 10 @>
        test <@ okSt.Fail.Request.Count = 0 @>
        test <@ failSt.Ok.Request.Count = 0 @>
        test <@ failSt.Fail.Request.Count <= 10 @>

[<Fact>]
let ``withoutWarmUp should hide the warmup info on the console`` () =

    let mutable warmupRun = false
    let inMemorySink = new InMemorySink()

    Scenario.create("1", fun ctx -> task {
        if ctx.ScenarioInfo.ScenarioOperation = ScenarioOperation.WarmUp then
            warmupRun <- true

        do! Task.Delay(seconds 0.5)
        return Response.ok()
    })
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(1, seconds 1)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withLoggerConfig(fun () -> LoggerConfiguration().WriteTo.Sink(inMemorySink))
    |> NBomberRunner.withoutReports
    |> NBomberRunner.runWithArgs ["disposeLogger=false"]
    |> ignore

    test <@ warmupRun = false @>
    test <@ inMemorySink.LogEvents |> Seq.exists(fun x -> x.MessageTemplate.Text.Contains "Starting warm up...") |> not @>
    test <@ inMemorySink.LogEvents |> Seq.exists(fun x -> x.MessageTemplate.Text.Contains "Starting bombing...") @>

[<Fact>]
let ``withWarmUpDuration should run warmup only for specified scenarios`` () =

    let mutable warmupStep1 = false
    let mutable warmupStep2 = false
    let mutable bombingStep1 = false
    let mutable bombingStep2 = false

    let scn1 =
        Scenario.create("1", fun ctx -> task {
            if ctx.ScenarioInfo.ScenarioOperation = ScenarioOperation.WarmUp then
                warmupStep1 <- true

            if ctx.ScenarioInfo.ScenarioOperation = ScenarioOperation.Bombing then
                bombingStep1 <- true

            do! Task.Delay(seconds 0.5)
            return Response.ok()
        })
        |> Scenario.withWarmUpDuration(seconds 2)
        |> Scenario.withLoadSimulations [KeepConstant(1, seconds 2)]

    let scn2 =
        Scenario.create("2", fun ctx -> task {
            if ctx.ScenarioInfo.ScenarioOperation = ScenarioOperation.WarmUp then
                warmupStep2 <- true

            if ctx.ScenarioInfo.ScenarioOperation = ScenarioOperation.Bombing then
                bombingStep2 <- true

            do! Task.Delay(seconds 0.5)
            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(1, seconds 2)]

    NBomberRunner.registerScenarios [scn1; scn2]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> ignore

    test <@ warmupStep1 = true @>
    test <@ warmupStep2 = false @>
    test <@ bombingStep1 = true @>
    test <@ bombingStep2 = true @>

[<Fact>]
let ``warm-up duration should be equal or smaller that scenario's duration`` () =

    let scn =
        Scenario.create("1", fun ctx -> task {
            do! Task.Delay(seconds 0.5)
            return Response.ok()
        })
        |> Scenario.withWarmUpDuration(seconds 5) // we set bigger duration than the scnDuration
        |> Scenario.withLoadSimulations [KeepConstant(1, seconds 2)]

    NBomberRunner.registerScenarios [scn]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.runWithResult []
    |> Result.getError
    |> function
        | Scenario error ->
            match error with
            | WarmUpDurationIsBiggerScnDuration _ -> ()
            | _ -> failwith "invalid error type"

        | _ -> failwith "invalid error type"
