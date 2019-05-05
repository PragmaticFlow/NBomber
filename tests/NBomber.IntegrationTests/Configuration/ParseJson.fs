module Tests.Configuration.ParseJson

open Xunit
open NBomber.Configuration

[<Fact>]
let ``NBomberConfig.load() should read json file successfully`` () =
    "Configuration/config.json"
    |> NBomberConfig.load
