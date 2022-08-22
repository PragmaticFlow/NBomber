module internal NBomber.Domain.ClientPool

open System

open FsToolkit.ErrorHandling
open FSharp.Control.Reactive

open NBomber
open NBomber.Extensions.Internal
open NBomber.Contracts

type ClientPoolEvent =
    | StartedInit       of poolName:string
    | StartedDispose    of poolName:string
    | ClientInitialized of poolName:string * clientNumber:int
    | ClientDisposed    of poolName:string * clientNumber:int * error:exn option
    | InitFinished
    | InitFailed

type ClientPool(factory: ClientFactory<obj>) =

    let mutable _disposed = false
    let mutable _initializedClients = Array.empty
    let _eventStream = Subject.broadcast

    let initPool (context: IBaseContext) =

        let rec retryInit (clientNumber, tryCount, ctx) =
            try
                let client = factory.InitClient(clientNumber, ctx).Result
                Ok client
            with
            | ex ->
                if tryCount >= Constants.TryCount then Error ex
                else retryInit(clientNumber, tryCount + 1, ctx)

        let rec initClients (number, clientCount, context) = seq {
            if number < clientCount then
                match retryInit(number, 0, context) with
                | Ok client ->
                    yield Ok client
                    let displayNumber = number + 1
                    _eventStream.OnNext(ClientInitialized(factory.FactoryName, displayNumber))
                    yield! initClients(number + 1, clientCount, context)

                | Error ex -> yield Error ex
        }

        _eventStream.OnNext(StartedInit factory.FactoryName)

        let result = initClients(0, factory.ClientCount, context) |> Result.sequence
        match result with
        | Ok clients ->
            _initializedClients <- clients |> List.toArray
            _eventStream.OnNext(InitFinished)
            Ok() |> Task.singleton

        | Error exs ->
            _eventStream.OnNext(InitFailed)
            exs.Head |> Error |> Task.singleton

    let disposeAllClients (context: IBaseContext) =
        _eventStream.OnNext(StartedDispose factory.FactoryName)

        let mutable counter = 0
        for client in _initializedClients do
            counter <- counter + 1
            try
                factory.DisposeClient(client, context).Wait()
                _eventStream.OnNext(ClientDisposed(factory.FactoryName, counter, error = None))
            with
            | ex ->
                _eventStream.OnNext(ClientDisposed(factory.FactoryName, counter, error = Some ex))

    let disposePool (context: IBaseContext) =
        if not _disposed then
            _disposed <- true
            disposeAllClients context
            _eventStream.OnCompleted()
            _eventStream.Dispose()

    member _.PoolName = factory.FactoryName
    member _.ClientCount = factory.ClientCount
    member _.InitializedClients = _initializedClients
    member _.EventStream = _eventStream :> IObservable<_>
    member _.Init(context) = initPool context
    member _.DisposePool(context) = disposePool context
