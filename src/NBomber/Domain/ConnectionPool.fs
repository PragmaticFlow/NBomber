module internal NBomber.Domain.ConnectionPool

open System
open System.Threading
open System.Threading.Tasks

open FsToolkit.ErrorHandling
open FSharp.Control.Reactive
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber
open NBomber.Extensions.InternalExtensions
open NBomber.Contracts

type ConnectionPoolArgs<'TConnection>(name: string, connectionCount: int,
                                      openConnection: int * IBaseContext -> Task<'TConnection>,
                                      closeConnection: 'TConnection * IBaseContext -> Task) =
    let untypedArgs = lazy (
        ConnectionPoolArgs<obj>(
            name, connectionCount,
            openConnection = (fun (number, token) -> task {
                let! connection = openConnection(number, token)
                return connection :> obj
            }),
            closeConnection = (fun (connection, token) -> closeConnection(connection :?> 'TConnection, token))
        )
    )

    member _.PoolName = name
    member _.ConnectionCount = connectionCount
    member _.OpenConnection(number, context) = openConnection(number, context)
    member _.CloseConnection(connection, context) = closeConnection(connection, context)
    member _.GetUntyped() = untypedArgs
    member _.Clone(newName: string) = ConnectionPoolArgs<'TConnection>(newName, connectionCount, openConnection, closeConnection)
    member _.Clone(newConnectionCount: int) = ConnectionPoolArgs<'TConnection>(name, newConnectionCount, openConnection, closeConnection)

    interface IConnectionPoolArgs<'TConnection> with
        member _.PoolName = name
        member _.ConnectionCount = connectionCount
        member _.OpenConnection(number, context) = openConnection(number, context)
        member _.CloseConnection(connection, context) = closeConnection(connection, context)

type ConnectionPoolEvent =
    | StartedInit           of poolName:string
    | StartedStop           of poolName:string
    | ConnectionOpened      of poolName:string * connectionNumber:int
    | ConnectionClosed      of error:exn option
    | InitFinished
    | InitFailed

type ConnectionPool(args: ConnectionPoolArgs<obj>) =

    let mutable _disposed = false
    let mutable _aliveConnections = Array.empty
    let _eventStream = Subject.broadcast

    let initPool (context: IBaseContext) =

        let rec retryOpenConnection (connectionNumber, tryCount, ctx) =
            try
                let connection = args.OpenConnection(connectionNumber, ctx).Result
                Ok connection
            with
            | ex ->
                if tryCount >= Constants.TryCount then Error ex
                else retryOpenConnection(connectionNumber, tryCount + 1, ctx)

        let rec openConnections (connectionNumber, connectionCount, ctx) = seq {
            if connectionNumber < connectionCount then
                match retryOpenConnection(connectionNumber, 0, ctx) with
                | Ok connection ->
                    yield Ok connection
                    let displayNumber = connectionNumber + 1
                    _eventStream.OnNext(ConnectionOpened(args.PoolName, displayNumber))
                    yield! openConnections(connectionNumber + 1, connectionCount, ctx)

                | Error ex -> yield Error ex
        }

        _eventStream.OnNext(StartedInit args.PoolName)

        let result = openConnections(0, args.ConnectionCount, context) |> Result.sequence
        match result with
        | Ok connections ->
            _aliveConnections <- connections |> List.toArray
            _eventStream.OnNext(InitFinished)
            Ok() |> Task.singleton

        | Error exs ->
            _eventStream.OnNext(InitFailed)
            exs.Head |> Error |> Task.singleton

    let closeAllConnections (context: IBaseContext) =
        _eventStream.OnNext(StartedStop args.PoolName)

        for connection in _aliveConnections do
            try
                args.CloseConnection(connection, context).Wait(Constants.OperationTimeOut) |> ignore
                _eventStream.OnNext(ConnectionClosed(error = None))
            with
            | ex ->
                _eventStream.OnNext(ConnectionClosed(error = Some ex))

    let destroy (context: IBaseContext) =
        if not _disposed then
            _disposed <- true
            closeAllConnections context
            _eventStream.OnCompleted()
            use e = _eventStream
            e |> ignore

        Task.CompletedTask

    member _.PoolName = args.PoolName
    member _.ConnectionCount = args.ConnectionCount
    member _.AliveConnections = _aliveConnections
    member _.EventStream = _eventStream :> IObservable<_>
    member _.Init(context) = initPool context
    member _.Destroy(context) = destroy context
