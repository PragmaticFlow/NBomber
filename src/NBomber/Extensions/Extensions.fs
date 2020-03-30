namespace NBomber.Extensions

open System
open System.Diagnostics
open System.Runtime.CompilerServices
open Newtonsoft.Json

[<Extension>]
type StringExtensions() =

    [<Extension>]
    static member DeserializeJson<'T>(json: string) =
        JsonConvert.DeserializeObject<'T>(json)

    [<Extension>]
    static member DeserializeYaml<'T>(yaml: string) =
        let deserializer = YamlDotNet.Serialization.Deserializer()
        yaml |> deserializer.Deserialize<'T>

type CurrentTime() =

    let _timer = Stopwatch()
    let _initTime = DateTime.UtcNow

    do _timer.Start()

    member _.UtcNow = _initTime + _timer.Elapsed
