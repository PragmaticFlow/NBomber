module Tests.Plugin

open System
open System.Data
open System.Threading.Tasks

open Serilog
open Serilog.Sinks.InMemory
open Serilog.Sinks.InMemory.Assertions
open Swensen.Unquote
open Xunit

open NBomber
open NBomber.Extensions.Internal
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Domain
open NBomber.FSharp
open Tests.TestHelper

[<Fact>]
let ``WorkerPlugin Init, Start, Stop should be invoked once for Warmup and once for Bombing`` () =

    let mutable invocationOrder = List.empty
    let scenarios = PluginTestHelper.createScenarios()

    let plugin = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin"

            member _.Init(_, _) =
                invocationOrder <- "init" :: invocationOrder
                Task.CompletedTask

            member _.Start() =
                invocationOrder <- "start" :: invocationOrder
                Task.CompletedTask

            member _.GetStats(_) =
                invocationOrder <- "get_stats" :: invocationOrder
                Task.FromResult(new DataSet())

            member _.GetHints() =
                invocationOrder <- "get_hints" :: invocationOrder
                Array.empty

            member _.Stop() =
                invocationOrder <- "stop" :: invocationOrder
                Task.CompletedTask

            member _.Dispose() =
                invocationOrder <- "dispose" :: invocationOrder
    }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.withWorkerPlugins [plugin]
    |> NBomberRunner.enableHintsAnalyzer true
    |> NBomberRunner.run
    |> Result.mapError failwith
    |> ignore

    test <@ List.rev invocationOrder = ["init"; "start"; "stop"; "start"; "stop"; "get_stats"; "get_hints"; "dispose"] @>

[<Fact>]
let ``StartTest should be invoked once`` () =

    let scenarios = PluginTestHelper.createScenarios()
    let mutable pluginStartTestInvokedCounter = 0

    let plugin = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin"
            member _.Init(_, _) = Task.CompletedTask

            member _.Start() =
                pluginStartTestInvokedCounter <- pluginStartTestInvokedCounter + 1
                Task.CompletedTask

            member _.GetHints() = Array.empty
            member _.GetStats(_) = Task.FromResult(new DataSet())
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.withWorkerPlugins [plugin]
    |> NBomberRunner.run
    |> Result.mapError(fun x -> failwith x)
    |> ignore

    test <@ pluginStartTestInvokedCounter = 1 @>


[<Fact>]
let ``StartTest should be invoked with infra config`` () =

    let scenarios = PluginTestHelper.createScenarios()
    let mutable pluginConfig = Unchecked.defaultof<_>

    let plugin = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin"

            member _.Init(logger, infraConfig) =
                pluginConfig <- infraConfig
                Task.CompletedTask

            member _.Start() = Task.CompletedTask
            member _.GetHints() = Array.empty
            member _.GetStats(_) = Task.FromResult(new DataSet())
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.loadInfraConfig "Configuration/infra_config.json"
    |> NBomberRunner.withWorkerPlugins [plugin]
    |> NBomberRunner.run
    |> Result.mapError(fun x -> failwith x)
    |> ignore

    let serilogConfig = pluginConfig.GetSection("Serilog")

    test <@ isNull serilogConfig = false @>

[<Fact>]
let ``GetStats should be invoked only one time when final stats fetching`` () =

    let scenarios = PluginTestHelper.createScenarios()
    let mutable pluginGetStatsInvokedCounter = 0

    let plugin = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask

            member _.GetStats(stats) =
                pluginGetStatsInvokedCounter <- pluginGetStatsInvokedCounter + 1
                Task.FromResult(new DataSet())

            member _.GetHints() = Array.empty
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.withWorkerPlugins [plugin]
    |> NBomberRunner.run
    |> Result.mapError(fun x -> failwith x)
    |> ignore

    test <@ pluginGetStatsInvokedCounter = 1 @>

[<Fact>]
let ``StopTest should be invoked once`` () =

    let scenarios = PluginTestHelper.createScenarios()
    let mutable pluginFinishTestInvokedCounter = 0

    let plugin = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask
            member _.GetStats(_) = Task.FromResult(new DataSet())
            member _.GetHints() = Array.empty
            member _.Stop() =
                pluginFinishTestInvokedCounter <- pluginFinishTestInvokedCounter + 1
                Task.CompletedTask
            member _.Dispose() = ()
    }

    NBomberRunner.registerScenarios scenarios
    |> NBomberRunner.withWorkerPlugins [plugin]
    |> NBomberRunner.run
    |> Result.mapError(fun x -> failwith x)
    |> ignore

    test <@ pluginFinishTestInvokedCounter = 1 @>

[<Fact>]
let ``PluginStats should return empty data set in case of execution timeout`` () =
    let inMemorySink = new InMemorySink()
    let loggerConfig = fun () -> LoggerConfiguration().WriteTo.Sink(inMemorySink)

    let timeoutPlugin = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin"

            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask
            member _.GetStats(_) = task {
                do! Task.Delay(seconds 10) // we waiting more than default timeout = 5 sec
                return new DataSet()
            }
            member _.GetHints() = Array.empty
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    Scenario.create("1", fun ctx -> task {
        return Response.ok()
    })
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(1, seconds 10)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withLoggerConfig loggerConfig
    |> NBomberRunner.withWorkerPlugins [timeoutPlugin]
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        test <@ Array.isEmpty nodeStats.PluginStats @>
        inMemorySink.Should().HaveMessage("Getting plugin stats failed with the timeout error", "because timeout has been reached") |> ignore

[<Fact>]
let ``PluginStats should return empty data set in case of internal exception`` () =
    let inMemorySink = new InMemorySink()
    let loggerConfig = fun () -> LoggerConfiguration().WriteTo.Sink(inMemorySink)

    let exceptionPlugin = {
        new IWorkerPlugin with
            member _.PluginName = "TestPlugin"
            member _.Init(_, _) = Task.CompletedTask
            member _.Start() = Task.CompletedTask
            member _.GetStats(_) = failwith "test exception" // we throw exception
            member _.GetHints() = Array.empty
            member _.Stop() = Task.CompletedTask
            member _.Dispose() = ()
    }

    Scenario.create("1", fun ctx -> task {
        return Response.ok()
    })
    |> Scenario.withoutWarmUp
    |> Scenario.withLoadSimulations [KeepConstant(1, seconds 2)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withLoggerConfig loggerConfig
    |> NBomberRunner.withWorkerPlugins [exceptionPlugin]
    |> NBomberRunner.run
    |> Result.getOk
    |> fun nodeStats ->
        test <@ Array.isEmpty nodeStats.PluginStats @>
        inMemorySink.Should().HaveMessage("Getting plugin stats failed", "because exception was thrown") |> ignore
