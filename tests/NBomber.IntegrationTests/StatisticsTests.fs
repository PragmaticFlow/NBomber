module Tests.Statistics

open System

open Xunit
open FsCheck.Xunit
open Swensen.Unquote
open Nessos.Streams

open NBomber.Extensions
open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.DomainTypes
open Tests.TestHelper

let private latencyCount = { Less800 = 1; More800Less1200 = 1; More1200 = 1 }

let private scenario = {
    ScenarioName = "Scenario1"
    TestInit = None
    TestClean = None
    Steps = List.empty
    LoadSimulations = [KeepConcurrentScenarios(copiesCount = 1, during = TimeSpan.FromSeconds(1.0))]
    WarmUpDuration = TimeSpan.FromSeconds(1.0)
}

[<Fact>]
let ``calcRPS() should not calculate latency which is bigger than 1 sec`` () =
    let latencies = [2_000; 3_000; 4_000]
    let scnDuration = TimeSpan.FromSeconds(5.0)

    let result = latencies |> Stream.ofList |> Statistics.calcRPS(scnDuration)

    test <@ result = 0 @>

[<Property>]
let ``calcRPS() should not fail and calculate correctly for any args values`` (latencies: Latency list, scnDuration: TimeSpan) =
    let result = latencies |> Stream.ofList |> Statistics.calcRPS(scnDuration)

    if latencies.Length = 0 then
        test <@ result = 0 @>

    elif latencies.Length <> 0 && scnDuration.TotalSeconds < 1.0 then
        test <@ result = latencies.Length @>

    else
        let allLatenciesIn1SecCount = latencies |> List.filter(fun x -> x <= 1_000) |> List.length
        let expected = allLatenciesIn1SecCount / int(scnDuration.TotalSeconds)
        test <@ result = expected @>

[<Property>]
let ``calcMin() should not fail and calculate correctly for any args values`` (latencies: Latency list) =
    let result   = latencies |> Stream.ofList |> Statistics.calcMin
    let expected = List.minOrDefault 0 latencies
    test <@ result = expected @>

[<Property>]
let ``calcMean() should not fail and calculate correctly for any args values`` (latencies: Latency list) =
    let result = latencies |> Stream.ofList |> Statistics.calcMean
    let expected = latencies |> List.map float |> List.averageOrDefault 0.0 |> int
    test <@ result = expected @>

[<Property>]
let ``calcMax() should not fail and calculate correctly for any args values`` (latencies: Latency list) =
    let result = latencies |> Stream.ofList |> Statistics.calcMax
    let expected = List.maxOrDefault 0 latencies
    test <@ result = expected @>
