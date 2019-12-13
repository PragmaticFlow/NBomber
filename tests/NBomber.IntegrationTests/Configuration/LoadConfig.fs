module Tests.Configuration.LoadConfigTests

open System.IO
open Xunit
open NBomber.Configuration
open NBomber.FSharp

[<Fact>]
let ``NBomberConfig.parse() should read json file successfully`` () =
    "Configuration/config.json"
    |> File.ReadAllText
    |> NBomberConfig.parse
    
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