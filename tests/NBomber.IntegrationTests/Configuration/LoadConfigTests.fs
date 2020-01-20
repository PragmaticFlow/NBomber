module Tests.Configuration.LoadConfigTests

open System.IO
open Xunit
open FSharp.Json
open NBomber.Configuration
open NBomber.FSharp

[<Fact>]
let ``NBomberConfig.parse() should read json file successfully`` () =
    "Configuration/config.json"
    |> File.ReadAllText
    |> TestConfig.parse

[<Fact>]
let ``NBomberConfig.parse() should throw ex if mandatory json fields are missing`` () =
    Assert.Throws(typeof<JsonDeserializationError>,
                  fun _ -> "Configuration/missing_fields_config.json"
                            |> File.ReadAllText
                            |> TestConfig.parse
                            |> ignore
    )

[<Fact>]
let ``NBomberRunner.loadInfraConfig should parse config successfully`` () =
    NBomberRunner.registerScenarios []
    |> NBomberRunner.loadInfraConfig "Configuration/infra_config.json"

[<Fact>]
let ``NBomberRunner.loadInfraConfig should throw ex if file is not found`` () =
    Assert.Throws(typeof<FileNotFoundException>,
                  fun _ -> NBomberRunner.registerScenarios []
                           |> NBomberRunner.loadInfraConfig "Configuration/infra_config_2.json"
                           |> ignore)
