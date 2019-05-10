module Tests.Configuration.ParseJson

open System.IO
open Xunit
open NBomber.Configuration

[<Fact>]
let ``NBomberConfig.parse() should read json file successfully`` () =
    "Configuration/config.json"
    |> File.ReadAllText
    |> NBomberConfig.parse
