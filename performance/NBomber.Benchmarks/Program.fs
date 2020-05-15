open BenchmarkDotNet.Running
open NBomber.Benchmarks

[<EntryPoint>]
let main argv =

    BenchmarkRunner.Run<CollectionsBenchmark>()
    |> ignore

    0 // return an integer exit code

