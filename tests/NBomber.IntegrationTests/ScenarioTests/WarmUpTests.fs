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
open NBomber.Domain
//
// [<Fact>]
// let ``Warmup should have no effect on stats`` () =
//
//     let okStep = Step.create("ok step", fun _ -> task {
//         do! Task.Delay(milliseconds 100)
//         return Response.ok()
//     })
//
//     let failStep = Step.create("fail step", fun _ -> task {
//         do! Task.Delay(milliseconds 100)
//         return Response.fail()
//     })
//
//     Scenario.create "warmup test" [okStep; failStep]
//     |> Scenario.withWarmUpDuration(seconds 3)
//     |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 1)]
//     |> NBomberRunner.registerScenario
//     |> NBomberRunner.withoutReports
//     |> NBomberRunner.run
//     |> Result.getOk
//     |> fun nodeStats ->
//         let allStepStats = nodeStats.ScenarioStats |> Seq.collect(fun x -> x.StepStats)
//         let okSt = allStepStats |> Seq.find(fun x -> x.StepName = "ok step")
//         let failSt = allStepStats |> Seq.find(fun x -> x.StepName = "fail step")
//
//         test <@ okSt.Ok.Request.Count <= 10 @>
//         test <@ okSt.Fail.Request.Count = 0 @>
//         test <@ failSt.Ok.Request.Count = 0 @>
//         test <@ failSt.Fail.Request.Count <= 10 @>
//
// [<Fact>]
// let ``withoutWarmUp should hide the warmup info on the console`` () =
//
//     let mutable warmupRun = false
//
//     let step1 = Step.create("step_1", fun context -> task {
//
//         if context.ScenarioInfo.ScenarioOperation = ScenarioOperation.WarmUp then
//             warmupRun <- true
//
//         do! Task.Delay(seconds 0.5)
//         return Response.ok()
//     })
//
//     let inMemorySink = new InMemorySink()
//
//     Scenario.create "1" [step1]
//     |> Scenario.withoutWarmUp
//     |> Scenario.withLoadSimulations [KeepConstant(1, seconds 2)]
//     |> NBomberRunner.registerScenario
//     |> NBomberRunner.withLoggerConfig(fun () -> LoggerConfiguration().WriteTo.Sink(inMemorySink))
//     |> NBomberRunner.withoutReports
//     |> NBomberRunner.run
//     |> ignore
//
//     test <@ warmupRun = false @>
//     test <@ inMemorySink.LogEvents |> Seq.exists(fun x -> x.MessageTemplate.Text.Contains "Starting warm up...") |> not @>
//     test <@ inMemorySink.LogEvents |> Seq.exists(fun x -> x.MessageTemplate.Text.Contains "Starting bombing...") @>
//
// [<Fact>]
// let ``withWarmUpDuration should run warmup only for specified scenarios`` () =
//
//     let mutable warmupStep1 = false
//     let mutable warmupStep2 = false
//     let mutable bombingStep1 = false
//     let mutable bombingStep2 = false
//
//     let step1 = Step.create("step_1", fun context -> task {
//
//         if context.ScenarioInfo.ScenarioOperation = ScenarioOperation.WarmUp then
//             warmupStep1 <- true
//
//         if context.ScenarioInfo.ScenarioOperation = ScenarioOperation.Bombing then
//             bombingStep1 <- true
//
//         do! Task.Delay(seconds 0.5)
//         return Response.ok()
//     })
//
//     let step2 = Step.create("step_2", fun context -> task {
//
//         if context.ScenarioInfo.ScenarioOperation = ScenarioOperation.WarmUp then
//             warmupStep2 <- true
//
//         if context.ScenarioInfo.ScenarioOperation = ScenarioOperation.Bombing then
//             bombingStep2 <- true
//
//         do! Task.Delay(seconds 0.5)
//         return Response.ok()
//     })
//
//     let scn1 =
//         Scenario.create "1" [step1]
//         |> Scenario.withWarmUpDuration(seconds 2)
//         |> Scenario.withLoadSimulations [KeepConstant(1, seconds 2)]
//
//     let scn2 =
//         Scenario.create "2" [step2]
//         |> Scenario.withoutWarmUp
//         |> Scenario.withLoadSimulations [KeepConstant(1, seconds 2)]
//
//     NBomberRunner.registerScenarios [scn1; scn2]
//     |> NBomberRunner.withoutReports
//     |> NBomberRunner.run
//     |> ignore
//
//     test <@ warmupStep1 = true @>
//     test <@ warmupStep2 = false @>
//     test <@ bombingStep1 = true @>
//     test <@ bombingStep2 = true @>
