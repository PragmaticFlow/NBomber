module internal rec NBomber.Domain.ConnectionPool

open System
open System.Threading

open FSharp.Control.Reactive
open FsToolkit.ErrorHandling
open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber
open NBomber.Extensions
open NBomber.Contracts

module ConnectionPoolArgs =

    let toUntyped (args: IConnectionPoolArgs<'TConnection>) =

        { new IConnectionPoolArgs<obj> with
            member x.PoolName = args.PoolName
            member x.GetConnectionCount() = args.GetConnectionCount()

            member x.OpenConnection(number, token) = task {
                let! connection = args.OpenConnection(number,token)
                return connection :> obj
            }

            member x.CloseConnection(connection, token) = args.CloseConnection(connection :?> 'TConnection, token) }

    let cloneWith (connectionCount) (args: IConnectionPoolArgs<obj>) =
        { new Contracts.IConnectionPoolArgs<obj> with
            member _.PoolName = args.PoolName
            member _.GetConnectionCount() = connectionCount
            member _.OpenConnection(number,token) = args.OpenConnection(number,token)
            member _.CloseConnection(connection,token) = args.CloseConnection(connection,token) }

type ConnectionPoolEvent =
    | StartedInit           of poolName:string
    | StartedStop           of poolName:string
    | ConnectionOpened      of poolName:string * connectionNumber:int
    | ConnectionClosed      of error:exn option
    | InitFinished
    | InitFailed

type ConnectionPool(args: IConnectionPoolArgs<obj>) =

    let mutable _disposed = false
    let mutable _aliveConnections = Array.empty
    let _eventStream = Subject.broadcast

    let initPool (token: CancellationToken) =

        let rec retryOpenConnection (connectionNumber, tryCount, token) =
            try
                let connection = args.OpenConnection(connectionNumber, token).Result
                Ok connection
            with
            | ex ->
                if tryCount >= Constants.ReTryCount then Error ex
                else retryOpenConnection(connectionNumber, tryCount + 1, token)

        let rec openConnections (connectionNumber, connectionCount, token) = seq {
            if connectionNumber < connectionCount then
                match retryOpenConnection(connectionNumber, 0, token) with
                | Ok connection ->
                    yield Ok connection
                    let displayNumber = connectionNumber + 1
                    _eventStream.OnNext(ConnectionOpened(args.PoolName, displayNumber))
                    yield! openConnections(connectionNumber + 1, connectionCount, token)

                | Error ex -> yield Error ex
        }

        _eventStream.OnNext(StartedInit(args.PoolName))

        let result = openConnections(0, args.GetConnectionCount(), token) |> Result.sequence
        match result with
        | Ok connections ->
            _aliveConnections <- connections |> List.toArray
            _eventStream.OnNext(InitFinished)
            Ok() |> Async.singleton

        | Error exs ->
            _eventStream.OnNext(InitFailed)
            exs.Head |> Error |> Async.singleton

    let closeAllConnections (token: CancellationToken) =
        _eventStream.OnNext(StartedStop args.PoolName)

        for connection in _aliveConnections do
            try
                args.CloseConnection(connection, token).Wait(Constants.OperationTimeOut) |> ignore
                _eventStream.OnNext(ConnectionClosed(error = None))
            with
            | ex ->
                _eventStream.OnNext(ConnectionClosed(error = Some ex))

    let destroy (token: CancellationToken) =
        if not _disposed then
            _disposed <- true
            closeAllConnections(token)
            _eventStream.OnCompleted()
            use e = _eventStream
            e |> ignore

    member _.PoolName = args.PoolName
    member _.ConnectionCount = args.GetConnectionCount()
    member _.AliveConnections = _aliveConnections
    member _.EventStream = _eventStream :> IObservable<_>
    member _.Init(token) = initPool(token)
    member _.Destroy(token) = destroy(token)

    interface IDisposable with
        member _.Dispose() = destroy(CancellationToken.None)
