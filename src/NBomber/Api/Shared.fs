namespace NBomber

open System.IO
open System.Globalization
open System.Runtime.CompilerServices

open CsvHelper

open NBomber.Extensions.InternalExtensions
open NBomber.Contracts

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

/// Data feed helps you to inject dynamic data into your test.
module Feed =

    /// Creates Feed that picks constant value per Step copy.
    /// Every Step copy will have unique constant value.
    [<CompiledName("CreateConstant")>]
    let createConstant (name) (provider: IFeedProvider<'T>) =
        NBomber.Domain.Feed.constant(name, provider)

    /// Creates Feed that randomly picks an item per Step invocation.
    [<CompiledName("CreateCircular")>]
    let createCircular (name) (provider: IFeedProvider<'T>) =
        NBomber.Domain.Feed.circular(name, provider)

    /// Creates Feed that returns values from  value on every Step invocation.
    [<CompiledName("CreateRandom")>]
    let createRandom (name) (provider: IFeedProvider<'T>) =
        NBomber.Domain.Feed.random(name, provider)

    [<CompiledName("Empty")>]
    let empty =
        NBomber.Domain.Feed.empty<unit>
