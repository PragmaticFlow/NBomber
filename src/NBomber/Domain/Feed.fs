module internal NBomber.Domain.Feed

open NBomber.Contracts
open NBomber.Extensions

let private toRow name a = dict [ "feed." + name, a ]

/// Empty data feed for all cases where it is not set
let empty =
    { new IFeed<'T> with
        member x.Name = ""
        member x.GetNext() = dict [] }

/// Generates values from specified sequence
let ofSeq name (xs: 'T seq) =
    if xs |> Seq.isEmpty then
        empty
    else
        let e = xs.GetEnumerator()

        let rec next() =
            if e.MoveNext() then e.Current else failwithf "End of data feed"

        { new IFeed<'T> with
            member x.Name = name
            member x.GetNext() = next() |> toRow name }

/// Generates values from shuffled collection
let shuffle name (xs: 'T []): IFeed<'T> =
    if Array.isEmpty xs then
        empty
    else
        xs |> Array.shuffle |> ofSeq name

/// Circulate iterate over the specified collection
let circular name (xs: 'T seq) =
    if xs |> Seq.isEmpty then
        empty
    else
        seq { while true do yield! xs }
        |> ofSeq name

/// Read a line from file path
let fromFile name (filePath: string) =
    System.IO.File.ReadLines filePath
    |> circular name

/// json file feed
let fromJson name (filePath: string) =
    System.IO.File.ReadAllText filePath
    |> Newtonsoft.Json.JsonConvert.DeserializeObject<'T []>
    |> ofSeq name

/// Converts values
let map (f: 'T -> 'b) (feed: IFeed<'T>): IFeed<'b> =
    { new IFeed<'b> with
        member x.Name = feed.Name
        member x.GetNext() = feed.GetNext() |> Dict.mapValues f }
