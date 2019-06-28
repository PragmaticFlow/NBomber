[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.ConnectionPool

open System
open Serilog

let setConnectionPool (allPools: ConnectionPool<obj>[]) (step: Step) =
    let findPool (poolName) = 
        allPools |> Array.find(fun x -> x.PoolName = poolName)

    { step with ConnectionPool = findPool(step.ConnectionPool.PoolName) }

let getDistinctPools (scenario: Scenario) =
    scenario.Steps
    |> Array.map(fun x -> x.ConnectionPool)
    |> Array.distinct

let getPoolCount (scenario: Scenario, pool: ConnectionPool<obj>) =    
    match pool.ConnectionsCount with
    | Some v -> v
    | None   -> scenario.ConcurrentCopies

let init (scenario: Scenario, 
          onStartedInitPool: (string * int) -> unit, // PoolName * ConnectionsCount
          onConnectionOpened: int -> unit,           // ConnectionNumber
          onFinishInitPool: string -> unit) =        // PoolName

    let initPool (pool: ConnectionPool<obj>) =
        let connectionCount = getPoolCount(scenario, pool)
        Log.Information("initializing connection pool: '{0}', connections count '{1}'", pool.PoolName, connectionCount)
        Log.Information("opening connections...")
        onStartedInitPool(pool.PoolName, connectionCount)

        let connections = Array.init connectionCount (fun i -> 
            let connection = pool.OpenConnection()
            onConnectionOpened(i)
            connection
        )
                
        onFinishInitPool(pool.PoolName)

        { pool with AliveConnections = connections }

    getDistinctPools(scenario)
    |> Array.map(initPool)

let clean (scenario: Scenario) =

    let invokeDispose (connection: obj) =
        if connection :? IDisposable then (connection :?> IDisposable).Dispose()

    let closeConnections (pool: ConnectionPool<obj>) =
        Log.Information("closing connections for connection pool: '{0}'", pool.PoolName)

        for connection in pool.AliveConnections do
            try 
                match pool.CloseConnection with
                | Some close -> close(connection)
                                invokeDispose(connection)
                | None       -> invokeDispose(connection)
            with
            | ex -> Log.Error(ex, "CloseConnection")
    
    scenario |> getDistinctPools |> Array.iter(closeConnections)