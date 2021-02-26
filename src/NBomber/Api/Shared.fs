namespace NBomber

open System.IO
open System.Globalization
open System.Runtime.CompilerServices

open CsvHelper

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
