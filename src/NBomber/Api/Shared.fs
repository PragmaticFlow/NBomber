namespace NBomber

open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open NBomber.Contracts
open NBomber.Extensions.Internal

type ClientPool<'T>() as this =

    let mutable _disposed = false
    let _clients = ResizeArray<'T>()

    member _.Clients = _clients :> IReadOnlyList<_>
    member _.AddClient(client) = _clients.Add client

    member _.GetClient(scenarioInfo: ScenarioInfo) =
        let index = scenarioInfo.ThreadNumber % _clients.Count
        _clients[index]

    member _.DisposeClients() =
        if not _disposed then
            _disposed <- true

            for c in _clients do
                match box c with
                | :? IDisposable as client -> client.Dispose()
                | _ -> ()

    member _.DisposeClients(disposeClient: Action<'T>) =
        if not _disposed then
            _disposed <- true

            for c in _clients do
                disposeClient.Invoke c

    interface IDisposable with
        member _.Dispose() =
            this.DisposeClients()

module Converter =

    [<CompiledName("FromMicroSecToMs")>]
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let fromMicroSecToMs (microSec: float) = Converter.fromMicroSecToMs microSec

    [<CompiledName("FromMsToMicroSec")>]
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let fromMsToMicroSec (ms: float) = Converter.fromMsToMicroSec ms

    [<CompiledName("FromBytesToKb")>]
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let fromBytesToKb (bytes: int64) = Converter.fromBytesToKb bytes

    [<CompiledName("FromBytesToMb")>]
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let fromBytesToMb (bytes: int64) = Converter.fromBytesToMb bytes

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
