module Tests.Statistics

open System

open Xunit
open FsCheck.Xunit

open NBomber.Domain

[<Property>]
let ``calcRPS() should not fail and calculate correctly for any args values`` (latencies: int[], scnDuration: TimeSpan) =
    let result = Statistics.calcRPS(latencies, scnDuration)

    if latencies.Length = 0 then
        Assert.Equal(0, result)    
    elif latencies.Length <> 0 && scnDuration.TotalSeconds < 1.0 then
        let expected = latencies.Length / 1
        Assert.Equal(expected, result)
    else
        let expected = latencies.Length / int(scnDuration.TotalSeconds)
        Assert.Equal(expected, result)

[<Property>]
let ``calcMin() should not fail and calculate correctly for any args values`` (latencies: int[]) =
    let result   = Statistics.calcMin(latencies)    
    let expected = if latencies.Length > 0 then Array.min(latencies)
                   else 0
    Assert.Equal(expected, result)

[<Property>]
let ``calcMean() should not fail and calculate correctly for any args values`` (latencies: int[]) =
    let result = latencies |> Statistics.calcMean    
    let expected = if latencies.Length > 0 then latencies |> Array.map(float) |> Array.average |> int
                   else 0
    Assert.Equal(expected, result)

[<Property>]
let ``calcMax() should not fail and calculate correctly for any args values`` (latencies: int[]) =
    let result = latencies |> Statistics.calcMax    
    let expected = if latencies.Length > 0 then Array.max(latencies)
                   else 0
    Assert.Equal(expected, result)