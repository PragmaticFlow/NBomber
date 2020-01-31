module NBomber.Feed

open System.Collections.Generic
open NBomber.Contracts
open NBomber.Extensions

let private rnd = System.Random()
let private toRow name a = dict [ name, a ]


/// generate random float between min and max
let private randomFloat min max = rnd.NextDouble() * (max - min) + min

/// Empty data feed for all cases where it is not set
[<CompiledName "Empty">]
let empty name =
    { new IFeed<'a> with
        member __.Name = name
        member __.Next() = null }

/// generate random float between min and max
[<CompiledName "RandomFloats">]
let randomFloats name min max =
    { new IFeed<float> with
        member __.Name = name
        member __.Next() = randomFloat min max |> toRow name }

[<CompiledName "Random">]
let random name (xs: #ICollection<'a>) =
    let xs' = Array.ofSeq xs
    if xs' |> Array.isEmpty then failwith "Empty data feed source"
    let length = xs' |> Array.length
    { new IFeed<'a> with
        member __.Name = name
        member __.Next() =
            let i = rnd.Next(0, length)
            xs'.[i] |> toRow name }

let sequence name (xs: 'a seq) =
    let e = xs.GetEnumerator()

    let rec next() =
        if e.MoveNext() then e.Current else failwithf "End of data feed"

    { new IFeed<'a> with
        member __.Name = name
        member __.Next() = next() |> toRow name }

/// sequentially iterate between min and max
let circular name (xs: 'a seq) =
    if xs |> Seq.isEmpty then failwithf "Empty data feed source"
    let e = xs.GetEnumerator()

    let rec next() =
        if e.MoveNext() then
            e.Current
        else
            e.Reset()
            next()
    { new IFeed<'a> with
        member __.Name = name
        member __.Next() = next() |> toRow name }

/// converts input values
let map (f: 'a -> 'b) (feed: IFeed<'a>): IFeed<'b> =
    { new IFeed<'b> with
        member __.Name = feed.Name
        member __.Next() = feed.Next() |> Dict.mapValues f }

/// reads a line from file path
let fromFile name (filePath: string) = System.IO.File.ReadLines filePath |> circular name

/// json file
let json name (filePath: string) =
    System.IO.File.ReadAllText filePath
    |> Newtonsoft.Json.JsonConvert.DeserializeObject<'a []>
    |> circular name
