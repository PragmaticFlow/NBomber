module Tests.Logging

open System
open System.Threading.Tasks

open Serilog
open Serilog.Events
open Serilog.Sinks.InMemory
open Xunit
open Swensen.Unquote

open NBomber
open NBomber.Contracts
open NBomber.FSharp

[<Fact>]
let ``set min logger level should work correctly`` () =

    let inMemorySink1 = new InMemorySink()
    let inMemorySink2 = new InMemorySink()

    let createLoggerConfig1 = fun () ->
        LoggerConfiguration()
            .MinimumLevel.Is(LogEventLevel.Error)
            .WriteTo.Sink(inMemorySink1)

    let createLoggerConfig2 = fun () ->
        LoggerConfiguration()
            .MinimumLevel.Is(LogEventLevel.Verbose)
            .WriteTo.Sink(inMemorySink2)

    Scenario.create("scenario1", fun ctx -> task {
        do! Task.Delay(seconds 0.1)
        ctx.Logger.Information "this message should not be printed"
        return Response.ok()
    })
    |> Scenario.withLoadSimulations [KeepConstant(1, TimeSpan.FromSeconds 3.0)]
    |> Scenario.withoutWarmUp
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withLoggerConfig createLoggerConfig1
    |> NBomberRunner.runWithArgs ["disposeLogger=false"]
    |> ignore

    Scenario.create("scenario2", fun ctx -> task {
        do! Task.Delay(seconds 0.1)
        ctx.Logger.Information "this message should not be printed"
        return Response.ok()
    })
    |> Scenario.withLoadSimulations [KeepConstant(1, TimeSpan.FromSeconds 3.0)]
    |> Scenario.withoutWarmUp
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withLoggerConfig createLoggerConfig2
    |> NBomberRunner.runWithArgs ["disposeLogger=false"]
    |> ignore

    let scenario1LogPrinted =
        inMemorySink1.LogEvents
        |> Seq.exists(fun x -> x.MessageTemplate.Text = "this message should not be printed")

    let scenario2LogPrinted =
        inMemorySink2.LogEvents
        |> Seq.exists(fun x -> x.MessageTemplate.Text = "this message should not be printed")

    test <@ scenario1LogPrinted = false @>
    test <@ scenario2LogPrinted = true @>
