module Tests.CliArgs

open System
open System.IO

open Swensen.Unquote
open Xunit

open NBomber.Contracts
open NBomber.Domain
open NBomber.FSharp

let scenario = Scenario.create("scenario", fun ctx -> task { return Response.ok() })  |> Scenario.withoutWarmUp
let scenario2 = Scenario.create("scenario2", fun ctx -> task { return Response.ok() }) |> Scenario.withoutWarmUp

[<Theory>]
[<InlineData("-c")>]
[<InlineData("--config")>]
let ``correct CLI commands should load config`` (command) =
    let context =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.executeCliArgs [command; "Configuration/test_config.json"]

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
        |> NBomberRunner.executeCliArgs [command; "Configuration/test_config.json"]

    test <@ context.NBomberConfig.IsNone @>
    test <@ context.InfraConfig.IsNone @>

[<Theory>]
[<InlineData("-i")>]
[<InlineData("--infra")>]
let ``correct CLI commands should load infra config`` (command) =
    let context =
        NBomberRunner.registerScenarios [scenario]
        |> NBomberRunner.executeCliArgs [command; "Configuration/infra_config.json"]

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
        |> NBomberRunner.executeCliArgs [command; "Configuration/infra_config.json"]

    test <@ context.NBomberConfig.IsNone @>
    test <@ context.InfraConfig.IsNone @>

[<Theory>]
[<InlineData("-c")>]
[<InlineData("--config")>]
let ``CLI commands should throw ex if config file is not found`` (command) =
    Assert.Throws(typeof<FileNotFoundException>,
                  fun _ -> NBomberRunner.registerScenarios [scenario]
                           |> NBomberRunner.executeCliArgs [command; "not_found_config.json"]
                           |> ignore)

[<Theory>]
[<InlineData("-i")>]
[<InlineData("--infra")>]
let ``CLI commands should throw ex if infra config file is not found`` (command) =
    Assert.Throws(typeof<FileNotFoundException>,
                  fun _ -> NBomberRunner.registerScenarios [scenario]
                           |> NBomberRunner.executeCliArgs [command; "not_found_infra_config.json"]
                           |> ignore)

[<Theory>]
[<InlineData("-t")>]
[<InlineData("--target")>]
let ``TargetScenarios should update NBomberContext`` (command) =
    let context =
        NBomberRunner.registerScenarios [scenario; scenario2]
        |> NBomberRunner.executeCliArgs [command; "scenario2"]

    test <@ context.TargetScenarios.Value.Length = 1 @>
    test <@ context.TargetScenarios.Value.Head = "scenario2" @>
