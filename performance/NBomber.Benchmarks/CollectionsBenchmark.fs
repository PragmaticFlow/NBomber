namespace NBomber.Benchmarks

open BenchmarkDotNet.Attributes
open FSharpx.Collections
open Nessos.Streams

[<MemoryDiagnoser>]
type CollectionsBenchmark() as this =

    let mutable _data = Array.empty
    let mutable _dataResizeArray = ResizeArray.ofList []
    let mutable _dataList = List.empty
    let mutable _dataStream = Stream.empty

    //[<Params(100_000, 1_000_000, 10_000_000)>]
    [<Params(10_000_000)>]
    member val N = 0 with get, set

    [<GlobalSetup>]
    member _.Setup() =
        _data <- Array.create this.N -1
        _dataResizeArray <- ResizeArray(_data)
        _dataList <- _data |> List.ofArray
        _dataStream <- _data |> Stream.ofArray

    [<Benchmark>]
    member _.ResizeArrayAdd() =
        let resizeArray = ResizeArray()
        for i = 0 to this.N do
            resizeArray.Add(i)

    [<Benchmark>]
    member _.ResizeArrayIterate() =
        _dataResizeArray
        |> ResizeArray.iter ignore

    [<Benchmark>]
    member _.FSharpListAdd() =
        let mutable items = []
        for i = 0 to this.N do
            items <- i :: items

    [<Benchmark>]
    member _.FSharpListIterate() =
        _dataList
        |> List.iter ignore

    [<Benchmark>]
    member _.SeqIterate() =
        _dataList
        |> Seq.iter ignore

    [<Benchmark>]
    member _.StreamIterate() =
        _dataStream
        |> Stream.iter ignore

    [<Benchmark>]
    member _.StreamInitAndIterate() =
        _data
        |> Stream.ofArray
        |> Stream.iter ignore
