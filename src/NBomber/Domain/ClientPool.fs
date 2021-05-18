module internal NBomber.Domain.ClientPool

open System
open System.Threading.Tasks

open FsToolkit.ErrorHandling
open FSharp.Control.Reactive
open FSharp.Control.Tasks.NonAffine

open NBomber
open NBomber.Extensions.InternalExtensions
open NBomber.Contracts

type ClientFactory<'TClient>(name: string,
                             clientCount: int,
                             initClient: int * IBaseContext -> Task<'TClient>, // number * context
                             disposeClient: 'TClient * IBaseContext -> Task) =

    // we use lazy to have the ability to check on duplicates (that has the same name but different instances) within one scenario
    let untypedFactory = lazy (
        ClientFactory<obj>(name, clientCount,
            initClient = (fun (number,token) -> task {
                let! client = initClient(number, token)
                return client :> obj
            }),
            disposeClient = (fun (client,context) -> disposeClient(client :?> 'TClient, context))
        )
    )

    member _.FactoryName = name
    member _.GetUntyped() = untypedFactory.Value
    member _.Clone(newName: string) = ClientFactory<'TClient>(newName, clientCount, initClient, disposeClient)
    member _.Clone(newClientCount: int) = ClientFactory<'TClient>(name, newClientCount, initClient, disposeClient)

    interface IClientFactory<'TClient> with
        member _.FactoryName = name
        member _.ClientCount = clientCount
        member _.InitClient(number, context) = initClient(number, context)
        member _.DisposeClient(client, context) = disposeClient(client, context)

type ClientPoolEvent =
    | StartedInit       of poolName:string
    | StartedDispose    of poolName:string
    | ClientInitialized of poolName:string * clientNumber:int
    | ClientDisposed    of poolName:string * clientNumber:int * error:exn option
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
            disposeAllClients(context)
            _eventStream.OnCompleted()
            _eventStream.Dispose()

    member _.PoolName = factory.FactoryName
    member _.ClientCount = factory.ClientCount
    member _.InitializedClients = _initializedClients
    member _.EventStream = _eventStream :> IObservable<_>
    member _.Init(context) = initPool(context)
    member _.DisposePool(context) = disposePool(context)
