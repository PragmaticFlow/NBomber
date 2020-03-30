module Tests.ScenarioTests

open System
open System.Threading.Tasks

open Xunit
open Swensen.Unquote
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Extensions
open NBomber.Contracts
open NBomber.FSharp

[<CLIMutable>]
type TestCustomSettings = {
    TargetMqttBrokerHost: string
    MsgPayloadSizeInBytes: int
}

[<Fact>]
let ``TestClean should be invoked only once and not fail runner`` () =

    let mutable cleanInvokeCounter = 0

    let testClean = fun _ -> task {
        cleanInvokeCounter <- cleanInvokeCounter + 1
        failwith "exception was not handled"
    }

    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    let scenario =
        Scenario.create "withTestClean test" [okStep]
        |> Scenario.withTestClean testClean
        |> Scenario.withOutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(2,  TimeSpan.FromSeconds(1.0))
        ]

    let allStats =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.runTest
        |> Result.getOk

    test <@ 1 = cleanInvokeCounter @>

[<Fact>]
let ``TestInit should propagate CustomSettings from config.json`` () =

    let mutable scnContext = Option.None

    let testInit = fun context -> task {
        scnContext <- Some context
    }

    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(TimeSpan.FromSeconds(0.1))
        return Response.Ok()
    })

    let scenario =
        Scenario.create "test_youtube" [okStep]
        |> Scenario.withTestInit testInit
        |> Scenario.withOutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConcurrentScenarios(2,  TimeSpan.FromSeconds(2.0))
        ]


    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.loadTestConfigJson "Configuration/config.json"
    |> NBomberRunner.runTest
    |> ignore

    let cusomSettings = scnContext.Value.CustomSettings.DeserializeJson<TestCustomSettings>()

    test <@ cusomSettings.TargetMqttBrokerHost = "localhost" @>
    test <@ cusomSettings.MsgPayloadSizeInBytes = 1000 @>
