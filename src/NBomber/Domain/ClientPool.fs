module internal NBomber.Domain.ClientPool

open System
open System.Threading.Tasks

open FsToolkit.ErrorHandling
open FSharp.Control.Reactive
open FSharp.Control.Tasks.NonAffine

open NBomber
open NBomber.Extensions.InternalExtensions
open NBomber.Contracts

type ClientFactory<'TClient>(name: string, clientCount: int, initClient: int * IBaseContext -> Task<'TClient>) =

    // we use lazy to have the ability to check on duplicates (that has the same name but different instances) within one scenario
    let untypedFactory = lazy (
        ClientFactory<obj>(name, clientCount,
            initClient = (fun (number,token) -> task {
                let! client = initClient(number, token)
                return client :> obj
            })
        )
    )

    member _.FactoryName = name
    member _.GetUntyped() = untypedFactory.Value
    member _.Clone(newName: string) = ClientFactory<'TClient>(newName, clientCount, initClient)
    member _.Clone(newClientCount: int) = ClientFactory<'TClient>(name, newClientCount, initClient)

    interface IClientFactory<'TClient> with
        member _.FactoryName = name
        member _.ClientCount = clientCount
        member _.InitClient(number, context) = initClient(number, context)

type ClientPoolEvent =
    | StartedInit       of poolName:string
    | StartedDispose    of poolName:string
    | ClientInitialized of poolName:string * clientNumber:int
    | ClientDisposed    of error:exn option
    | InitFinished
    | InitFailed

type ClientPool(factory: IClientFactory<obj>) =

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

    let disposeAllClients () =
        _eventStream.OnNext(StartedDispose factory.FactoryName)

        for client in _initializedClients do
            try
                if client :? IDisposable then (client :?> IDisposable).Dispose()
                _eventStream.OnNext(ClientDisposed(error = None))
            with
            | ex ->
                _eventStream.OnNext(ClientDisposed(error = Some ex))

    let dispose () =
        if not _disposed then
            _disposed <- true
            disposeAllClients()
            _eventStream.OnCompleted()
            _eventStream.Dispose()

    member _.PoolName = factory.FactoryName
    member _.ClientCount = factory.ClientCount
    member _.InitializedClients = _initializedClients
    member _.EventStream = _eventStream :> IObservable<_>
    member _.Init(context) = initPool context

    interface IDisposable with
        member _.Dispose() = dispose()
