namespace NBomber

open System
open System.Runtime.CompilerServices
open NBomber.Extensions.Internal

module Converter =

    [<CompiledName("FromMicroSecToMs")>]
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let fromMicroSecToMs (microSec: float) = Converter.fromMicroSecToMs microSec

    [<CompiledName("FromMsToMicroSec")>]
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let fromMsToMicroSec (ms: float) = Converter.fromMsToMicroSec ms

    [<CompiledName("FromBytesToKb")>]
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let fromBytesToKb (bytes) = Converter.fromBytesToKb bytes

    [<CompiledName("FromBytesToMb")>]
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let fromBytesToMb (bytes) = Converter.fromBytesToMb bytes

[<AutoOpen>]
module Time =

    [<CompiledName("Milliseconds")>]
    let inline milliseconds (value) = value |> float |> TimeSpan.FromMilliseconds

    [<CompiledName("Seconds")>]
    let inline seconds (value) = value |> float |> TimeSpan.FromSeconds

    [<CompiledName("Minutes")>]
    let inline minutes (value) = value |> float |> TimeSpan.FromMinutes

    [<CompiledName("Hours")>]
    let inline hours (value) = value |> float |> TimeSpan.FromHours
