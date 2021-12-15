open BenchmarkDotNet.Running
open NBomber.Benchmarks
open NBomber.Benchmarks.Actors

[<EntryPoint>]
let main argv =

//    BenchmarkRunner.Run<CollectionsBenchmark>()
//    |> ignore

    BenchmarkRunner.Run<ActorsBenchmark>()
    |> ignore

    0 // return an integer exit code

