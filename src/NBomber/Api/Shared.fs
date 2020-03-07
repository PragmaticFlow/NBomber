namespace NBomber

open System.IO
open System.Globalization
open System.Runtime.CompilerServices

open CsvHelper

open NBomber.Contracts
open NBomber.Extensions

[<Extension>]
type FeedData() =

    [<CompiledName("FromSeq")>]
    static member fromSeq (items: 'T seq) =
        { new IFeedProvider<'T> with
            member x.GetAllItems() = Seq.toArray items }

    [<CompiledName("FromCsv")>]
    static member fromCsv<'T> (filePath: string) =
        use reader = new StreamReader(filePath)
        use csv = new CsvReader(reader, CultureInfo.InvariantCulture)
        csv.GetRecords<'T>()
        |> Seq.toArray
        |> FeedData.fromSeq

    [<CompiledName("FromJson")>]
    static member fromJson<'T> (filePath: string) =
        System.IO.File.ReadAllText filePath
        |> Newtonsoft.Json.JsonConvert.DeserializeObject<'T[]>
        |> FeedData.fromSeq

    [<Extension; CompiledName("ShuffleData")>]
    static member shuffleData (provider: IFeedProvider<'T>) =
        { new IFeedProvider<'T> with
            member x.GetAllItems() = provider.GetAllItems() |> Array.shuffle }

module Feed =

    [<CompiledName("CreateConstant")>]
    let createConstant (name) (provider: IFeedProvider<'T>) =
        NBomber.Domain.Feed.constant(name, provider)

    [<CompiledName("CreateCircular")>]
    let createCircular (name) (provider: IFeedProvider<'T>) =
        NBomber.Domain.Feed.circular(name, provider)

    [<CompiledName("CreateRandom")>]
    let createRandom (name) (provider: IFeedProvider<'T>) =
        NBomber.Domain.Feed.random(name, provider)

    [<CompiledName("Empty")>]
    let empty =
        NBomber.Domain.Feed.empty<unit>
