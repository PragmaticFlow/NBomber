namespace Tests.Configuration

open System
open System.IO

open Microsoft.FSharp.Core
open Xunit
open FSharp.Json
open Swensen.Unquote

open NBomber.Contracts
open NBomber.Configuration
open NBomber.Extensions.Internal
open NBomber.FSharp

[<CLIMutable>]
type TestCustomSettings = {
    TargetHost: string
    MsgSizeInBytes: int
    PauseMs: int
}

module JsonConfig =

    [<Fact>]
    let ``parse() should parse json file successfully`` () =
        "Configuration/test_config.json" |> File.ReadAllText |> JsonExt.deserialize<NBomberConfig> |> ignore
        "Configuration/scenario_init_only_config.json" |> File.ReadAllText |> JsonExt.deserialize<NBomberConfig> |> ignore

    [<Fact>]
    let ``parse() should throw ex if mandatory json fields are missing`` () =
        Assert.Throws(
            typeof<JsonDeserializationError>,
            fun _ ->
                "Configuration/missing_fields_config.json"
                |> File.ReadAllText
                |> JsonExt.deserialize<NBomberConfig>
                |> ignore
        )

    [<Fact>]
    let ``parse() should parse custom settings successfully`` () =
        let config = "Configuration/test_config.json" |> File.ReadAllText |> JsonExt.deserialize<NBomberConfig>
        let testCustomSettings =
            config.GlobalSettings
            |> Option.bind(fun x -> x.ScenariosSettings)
            |> Option.bind(fun x -> Some x[0])
            |> Option.bind(fun x -> x.CustomSettings)
            |> Option.map(Newtonsoft.Json.JsonConvert.DeserializeObject<TestCustomSettings>)

        match testCustomSettings with
        | Some settings ->
            Assert.True(settings.TargetHost.Length > 0)
            Assert.True(settings.MsgSizeInBytes > 0)

        | None -> ()

module NBomberRunner =

    [<Fact>]
    let ``loadConfig should support load config via HTTP URL`` () =

        let url = "https://raw.githubusercontent.com/PragmaticFlow/NBomber/dev/examples/CSharpProd/HttpTests/Configs/config.json"

        Scenario.create("scenario_1", fun ctx -> task {
            return Response.ok()
        })
        |> NBomberRunner.registerScenario
        |> NBomberRunner.loadConfig url
        |> fun nbContext ->
            test <@ nbContext.NBomberConfig.IsSome @>

    [<Fact>]
    let ``loadConfig should throw exception if config is empty or doesn't follow NBomberConfig format`` () =
        Assert.Throws(
            typeof<Exception>,
            fun _ ->
                let url = "https://raw.githubusercontent.com/PragmaticFlow/NBomber.Enterprise.Examples/main/examples/ClusterSimpleHttpDemo/coordinator-config.json"

                Scenario.create("scenario_1", fun ctx -> task {
                    return Response.ok()
                })
                |> NBomberRunner.registerScenario
                |> NBomberRunner.loadConfig url
                |> ignore
        )

    [<Fact>]
    let ``loadInfraConfig should parse json config successfully`` () =
        NBomberRunner.registerScenarios []
        |> NBomberRunner.withoutReports
        |> NBomberRunner.loadInfraConfig "Configuration/infra_config.json"

    [<Fact>]
    let ``loadInfraConfig should throw ex if json file is not found`` () =
        Assert.Throws(
            typeof<FileNotFoundException>,
            fun _ -> NBomberRunner.registerScenarios []
                     |> NBomberRunner.withoutReports
                     |> NBomberRunner.loadInfraConfig "Configuration/infra_config_2.json"
                     |> ignore
        )

    [<Fact>]
    let ``loadInfraConfig should support load config via HTTP URL`` () =

        let url = "https://raw.githubusercontent.com/PragmaticFlow/NBomber/dev/examples/CSharpProd/HttpTests/Configs/infra-config.json"

        Scenario.create("scenario_1", fun ctx -> task {
            return Response.ok()
        })
        |> NBomberRunner.registerScenario
        |> NBomberRunner.loadInfraConfig url
        |> fun nbContext ->
            test <@ nbContext.InfraConfig.IsSome @>
            test <@ not (String.IsNullOrEmpty(nbContext.InfraConfig.Value.GetSection("PingPlugin:Ttl").Value)) @>
