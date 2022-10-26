module Tests.Scenario.TestHostScenario

open System

open Xunit
open Swensen.Unquote
open FsToolkit.ErrorHandling

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Internal
open NBomber.Extensions.Internal
open NBomber.DomainServices
open NBomber.DomainServices.TestHost
open NBomber.FSharp
//
// let okStep = Step.create("ok step", fun _ -> Response.ok() |> Task.singleton)
// let baseScenario = Scenario.create "1" [okStep] |> Scenario.withoutWarmUp
// let context = NBomberRunner.registerScenario baseScenario
//
// [<Fact>]
// let ``getTargetScenarios should update Step.Timeout to default value if it was not set`` () =
//
//     let data = taskResult {
//         let testInfo = SessionArgs.empty.TestInfo
//         let! scenarios  = context |> NBomberContext.createScenarios
//         let! sessionArgs = context |> NBomberContext.createSessionArgs testInfo scenarios
//         return sessionArgs, scenarios
//     }
//
//     let sessionArgs, regScenarios = data.Result |> Result.getOk
//
//     let targetScn = TestHostScenario.getTargetScenarios sessionArgs regScenarios
//
//     test <@ regScenarios[0].Steps[0].Timeout = TimeSpan.Zero @>
//     test <@ targetScn[0].Steps[0].Timeout.TotalMilliseconds = Constants.DefaultStepTimeoutMs @>

