namespace Tests.Configuration

open System.IO

open Xunit
open FSharp.Json

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
        Assert.Throws(typeof<JsonDeserializationError>,
                      fun _ -> "Configuration/missing_fields_config.json"
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
    let ``loadInfraConfig should parse json config successfully`` () =
        NBomberRunner.registerScenarios []
        |> NBomberRunner.loadInfraConfig "Configuration/infra_config.json"

    [<Fact>]
    let ``loadInfraConfig should throw ex if json file is not found`` () =
        Assert.Throws(
            typeof<FileNotFoundException>,
            fun _ -> NBomberRunner.registerScenarios []
                     |> NBomberRunner.loadInfraConfig "Configuration/infra_config_2.json"
                     |> ignore
        )
