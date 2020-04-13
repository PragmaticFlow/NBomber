namespace Tests.Configuration

open System.IO

open Xunit
open FSharp.Json
open YamlDotNet.Core

open NBomber.Configuration
open NBomber.Configuration.Yaml
open NBomber.Extensions
open NBomber.FSharp

[<CLIMutable>]
type TestCustomSettings = {
    TargetHost: string
    MsgSizeInBytes: int
}

module JsonConfig =

    [<Fact>]
    let ``unsafeParse() should read json file successfully`` () =
        "Configuration/test_config.json" |> File.ReadAllText |> JsonConfig.unsafeParse |> ignore

    [<Fact>]
    let ``unsafeParse() should throw ex if mandatory json fields are missing`` () =
        Assert.Throws(typeof<JsonDeserializationError>,
                      fun _ -> "Configuration/missing_fields_config.json"
                                |> File.ReadAllText
                                |> JsonConfig.unsafeParse
                                |> ignore
        )

    [<Fact>]
    let ``unsafeParse() should parse custom settings successfully`` () =
        let config = "Configuration/test_config.json" |> File.ReadAllText |> JsonConfig.unsafeParse
        let testCustomSettings =
            config.GlobalSettings
            |> Option.bind(fun x -> x.ScenariosSettings)
            |> Option.bind(fun x -> Some x.[0])
            |> Option.bind(fun x -> x.CustomSettings)
            |> Option.map(fun x -> x.DeserializeJson<TestCustomSettings>())

        match testCustomSettings with
        | Some settings ->
            Assert.True(settings.TargetHost.Length > 0)
            Assert.True(settings.MsgSizeInBytes > 0)

        | None -> ()

module YamlConfig =

    [<Fact>]
    let ``unsafeParse() should read yaml file successfully`` () =
        "Configuration/test_config.yaml" |> File.ReadAllText |> YamlConfig.unsafeParse |> ignore

    [<Fact>]
    let ``unsafeParse() should throw ex if mandatory yaml fields are missing`` () =
        Assert.Throws(typeof<YamlException>,
                      fun _ -> "Configuration/missing_fields_config.yaml"
                                |> File.ReadAllText
                                |> YamlConfig.unsafeParse
                                |> ignore
        )

    [<Fact>]
    let ``unsafeParse() should parse custom settings successfully`` () =
        let config = "Configuration/test_config.yaml" |> File.ReadAllText |> YamlConfig.unsafeParse
        let testCustomSettings =
            config.GlobalSettings
            |> Option.bind(fun x -> x.ScenariosSettings)
            |> Option.bind(fun x -> Some x.[0])
            |> Option.bind(fun x -> x.CustomSettings)
            |> Option.map(fun x -> x.DeserializeYaml<TestCustomSettings>())

        match testCustomSettings with
        | Some settings ->
            Assert.True(settings.TargetHost.Length > 0)
            Assert.True(settings.MsgSizeInBytes > 0)

        | None -> ()

module NBomberRunner =

    [<Fact>]
    let ``loadInfraConfigJson should parse config successfully`` () =
        NBomberRunner.registerScenarios []
        |> NBomberRunner.loadInfraConfigJson "Configuration/infra_config.json"

    [<Fact>]
    let ``loadInfraConfigJson should throw ex if file is not found`` () =
        Assert.Throws(typeof<FileNotFoundException>,
                      fun _ -> NBomberRunner.registerScenarios []
                               |> NBomberRunner.loadInfraConfigJson "Configuration/infra_config_2.json"
                               |> ignore)

    [<Fact>]
    let ``loadInfraConfigYaml should parse config successfully`` () =
        NBomberRunner.registerScenarios []
        |> NBomberRunner.loadInfraConfigYaml "Configuration/infra_config.yaml"

    [<Fact>]
    let ``loadInfraConfigYaml should throw ex if file is not found`` () =
        Assert.Throws(typeof<FileNotFoundException>,
                      fun _ -> NBomberRunner.registerScenarios []
                               |> NBomberRunner.loadInfraConfigYaml "Configuration/infra_config_2.yaml"
                               |> ignore)
