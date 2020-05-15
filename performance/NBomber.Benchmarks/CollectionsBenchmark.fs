namespace NBomber.Benchmarks

open BenchmarkDotNet.Attributes
open FSharpx.Collections
open Nessos.Streams

[<MemoryDiagnoser>]
type CollectionsBenchmark() =

    let mutable _data = Array.empty
    let mutable _dataResizeArray = ResizeArray.ofList []
    let mutable _dataList = List.empty
    let mutable _dataStream = Stream.empty

    //[<Params(100_000, 1_000_000, 10_000_000)>]
    [<Params(10_000_000)>]
    member val N = 0 with get, set

    [<GlobalSetup>]
    member x.Setup() =
        _data <- Array.create x.N -1
        _dataResizeArray <- ResizeArray(_data)
        _dataList <- _data |> List.ofArray
        _dataStream <- _data |> Stream.ofArray

    [<Benchmark>]
    member x.ResizeArrayAdd() =
        let resizeArray = ResizeArray()
        for i = 0 to x.N do
            resizeArray.Add(i)

    [<Benchmark>]
    member x.ResizeArrayIterate() =
        _dataResizeArray
        |> ResizeArray.iter(fun x -> ())

    [<Benchmark>]
    member x.FSharpListAdd() =
        let mutable items = []
        for i = 0 to x.N do
            items <- i :: items

    [<Benchmark>]
    member x.FSharpListIterate() =
        _dataList
        |> List.iter(fun x -> ())

    [<Benchmark>]
    member x.SeqIterate() =
        _dataList
        |> Seq.iter(fun x -> ())

    [<Benchmark>]
    member x.StreamIterate() =
        _dataStream
        |> Stream.iter(fun x -> ())

    [<Benchmark>]
    member x.StreamInitAndIterate() =
        _data
        |> Stream.ofArray
        |> Stream.iter(fun x -> ())
