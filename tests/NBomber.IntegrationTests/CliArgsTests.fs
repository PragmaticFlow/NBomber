module Tests.CliArgs

open FSharp.Control.Tasks.V2.ContextInsensitive
open Swensen.Unquote
open Xunit

open NBomber.Contracts
open NBomber.Domain
open NBomber.FSharp

let okStep = Step.create("ok step", fun _ -> task {
    return Response.Ok()
})

let scenario = Scenario.create "scenario" [okStep] |> Scenario.withoutWarmUp

[<Theory>]
[<InlineData("-c")>]
[<InlineData("--config")>]
let ``correct CLI commands should load config`` (command) =
    let context =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.executeCliArgs [command; "Configuration/test_config.yaml"]

    test <@ context.NBomberConfig.IsSome @>
    test <@ context.InfraConfig.IsNone @>

[<Theory>]
[<InlineData("-C")>]
[<InlineData("--Config")>]
[<InlineData("")>]
[<InlineData("-")>]
[<InlineData("--")>]
[<InlineData("-w")>]
let ``incorrect CLI command should not load config`` (command) =
    let context =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.executeCliArgs [command; "Configuration/test_config.yaml"]

    test <@ context.NBomberConfig.IsNone @>
    test <@ context.InfraConfig.IsNone @>

[<Theory>]
[<InlineData("-i")>]
[<InlineData("--infra")>]
let ``correct CLI commands should load infra config`` (command) =
    let context =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.executeCliArgs [command; "Configuration/infra_config.yaml"]

    test <@ context.NBomberConfig.IsNone @>
    test <@ context.InfraConfig.IsSome @>

[<Theory>]
[<InlineData("-I")>]
[<InlineData("--Infra")>]
[<InlineData("")>]
[<InlineData("-")>]
[<InlineData("--")>]
[<InlineData("-w")>]
let ``incorrect CLI command should not load infra config`` (command) =
    let context =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.executeCliArgs [command; "Configuration/infra_config.yaml"]

    test <@ context.NBomberConfig.IsNone @>
    test <@ context.InfraConfig.IsNone @>
