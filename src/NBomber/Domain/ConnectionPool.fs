module internal rec NBomber.Domain.ConnectionPool

open System

open FSharp.Control.Reactive
open FsToolkit.ErrorHandling

open NBomber
open NBomber.Extensions
open NBomber.Contracts

type ConnectionPoolArgs<'TConnection> = {
    PoolName: string
    OpenConnection: int -> 'TConnection
    CloseConnection: ('TConnection -> unit) option
    ConnectionCount: int
} with
    static member toUntyped (args: ConnectionPoolArgs<'TConnection>) =

        let newOpen = fun (connectionNumber) -> args.OpenConnection(connectionNumber) :> obj

        let newClose =
            match args.CloseConnection with
            | Some func -> Some(fun (connection: obj) -> func(connection :?> 'TConnection))
            | None      -> None

        { PoolName = args.PoolName
          OpenConnection = newOpen
          CloseConnection = newClose
          ConnectionCount = args.ConnectionCount }

type ConnectionPoolObj<'TConnection> = {
    Instance: ConnectionPool
} with
    interface IConnectionPool<'TConnection> with
        member x.PoolName = x.Instance.PoolName

type ConnectionPoolEvent =
    | StartedInit           of poolName:string
    | StartedStop           of poolName:string
    | ConnectionOpened      of poolName:string * connectionNumber:int
    | ConnectionClosed
    | CloseConnectionFailed of error:exn
    | InitFinished
    | InitFailed

type ConnectionPool(args: ConnectionPoolArgs<obj>) =

    let mutable _disposed = false
    let mutable _aliveConnections = Array.empty
    let _eventStream = Subject.broadcast

    let initPool () =

        let rec tryOpenConnection (connectionNumber, tryCount) =
            try
                let connection = args.OpenConnection(connectionNumber)
                Ok connection
            with
            | ex ->
                if tryCount >= Constants.ReTryCount then Error ex
                else tryOpenConnection(connectionNumber, tryCount + 1)

        let rec openConnections (connectionNumber, connectionCount) = seq {
            if connectionNumber < connectionCount then
                match tryOpenConnection(connectionNumber, 0) with
                | Ok connection ->
                    yield Ok connection
                    let displayNumber = connectionNumber + 1
                    _eventStream.OnNext(ConnectionOpened(args.PoolName, displayNumber))
                    yield! openConnections(connectionNumber + 1, connectionCount)

                | Error ex -> yield Error ex
        }

        _eventStream.OnNext(StartedInit(args.PoolName))

        let result = openConnections(0, args.ConnectionCount) |> Result.sequence
        match result with
        | Ok connections ->
            _aliveConnections <- connections |> List.toArray
            _eventStream.OnNext(InitFinished)
            Ok() |> Async.singleton

        | Error exs ->
            _eventStream.OnNext(InitFailed)
            exs.Head |> Error |> Async.singleton

    let closeAllConnections () =

        let invokeDispose (connection: obj) =
            if connection :? IDisposable then (connection :?> IDisposable).Dispose()

        let tryCloseConnection(closeFn, connection) =
            try
                closeFn(connection)
                invokeDispose(connection)
            with
            | ex -> ()

        _eventStream.OnNext(StartedStop args.PoolName)

        for connection in _aliveConnections do
            match args.CloseConnection with
            | Some close -> tryCloseConnection(close, connection)
            | None       -> invokeDispose(connection)
            _eventStream.OnNext(ConnectionClosed)

    let dispose () =
        if not _disposed then
            _disposed <- true
            closeAllConnections()
            _eventStream.OnCompleted()
            _eventStream.Dispose()

    member x.PoolName = args.PoolName
    member x.ConnectionCount = args.ConnectionCount
    member x.AliveConnections = _aliveConnections
    member x.EventStream = _eventStream :> IObservable<_>
    member x.Init() = initPool()
    member x.Dispose() = dispose()

    override x.GetHashCode() = x.PoolName.GetHashCode()
    override x.Equals(b) =
        match b with
        | :? ConnectionPool as pool -> x.PoolName = pool.PoolName
        | _ -> false

    interface IDisposable with
        member x.Dispose() = dispose() |> ignore
