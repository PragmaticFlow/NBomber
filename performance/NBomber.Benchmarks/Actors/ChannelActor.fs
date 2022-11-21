namespace NBomber.Benchmarks.Actors

open System
open System.Threading.Channels
open FSharp.Control.Tasks.NonAffine

type ChannelActor<'TState,'TMessage>(initialState: 'TState, handler: 'TState -> 'TMessage -> 'TState) =

    let _channel = Channel.CreateUnbounded<'TMessage>()
    let mutable _currentState = initialState
    let mutable _stop = false

    let loop () = backgroundTask {
        while not _stop do
            let! msg = _channel.Reader.ReadAsync()
            _currentState <- handler _currentState msg
    }

    do loop() |> ignore

    member _.Publish(message: 'TMessage) =
        _channel.Writer.TryWrite(message) |> ignore

type ChannelActorPly<'TState,'TMessage>(initialState: 'TState, handler: 'TState -> 'TMessage -> 'TState) =

    let _channel = Channel.CreateUnbounded<'TMessage>()
    let mutable _currentState = initialState
    let mutable _stop = false

    let loop () = vtask {
        while not _stop do
            let! msg = _channel.Reader.ReadAsync()
            _currentState <- handler _currentState msg
    }

    do loop() |> ignore

    member _.Publish(message: 'TMessage) =
        _channel.Writer.TryWrite(message) |> ignore
