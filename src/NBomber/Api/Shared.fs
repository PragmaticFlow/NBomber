namespace NBomber

open System
open System.IO
open System.Globalization
open System.Runtime.CompilerServices

open CsvHelper

open NBomber.Domain
open NBomber.Extensions.InternalExtensions

[<Extension>]
type FeedData() =

    [<CompiledName("FromCsv")>]
    static member fromCsv<'T> (filePath: string) =
        use reader = new StreamReader(filePath)
        use csv = new CsvReader(reader, CultureInfo.InvariantCulture)
        csv.GetRecords<'T>()
        |> Seq.toArray

    [<CompiledName("FromJson")>]
    static member fromJson<'T> (filePath: string) =
        File.ReadAllText filePath
        |> Newtonsoft.Json.JsonConvert.DeserializeObject<'T[]>

    [<Extension; CompiledName("ShuffleData")>]
    static member shuffleData (data: 'T seq) =
        data |> Seq.toArray |> Array.shuffle

[<AutoOpen>]
module Converter =

    [<CompiledName("KB")>]
    let inline kb (bytes) = Statistics.Converter.fromBytesToKb(bytes)

    [<CompiledName("MB")>]
    let inline mb (bytes) = Statistics.Converter.fromBytesToMb(bytes)

[<AutoOpen>]
module Time =

    [<CompiledName("Milliseconds")>]
    let inline milliseconds (value) = value |> float |> TimeSpan.FromMilliseconds

    [<CompiledName("Seconds")>]
    let inline seconds (value) = value |> float |> TimeSpan.FromSeconds

    [<CompiledName("Minutes")>]
    let inline minutes (value) = value |> float |> TimeSpan.FromMinutes

