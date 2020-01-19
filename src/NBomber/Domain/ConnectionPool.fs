[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.ConnectionPool

open System

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
          onConnectionOpened: int -> unit,
          onFinishInitPool: string -> unit,
          logger: Serilog.ILogger) =

    let initPool (pool: ConnectionPool<obj>) =
        let connectionCount = getPoolCount(scenario, pool)
        logger.Information("initializing connection pool: '{0}', connections count '{1}'", pool.PoolName, connectionCount)
        logger.Information("opening connections...")
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

let clean (scenario: Scenario, logger: Serilog.ILogger) =

    let invokeDispose (connection: obj) =
        if connection :? IDisposable then (connection :?> IDisposable).Dispose()

    let closeConnections (pool: ConnectionPool<obj>) =
        logger.Information("closing connections for connection pool: '{0}'", pool.PoolName)

        for connection in pool.AliveConnections do
            try
                match pool.CloseConnection with
                | Some close -> close(connection)
                                invokeDispose(connection)
                | None       -> invokeDispose(connection)
            with
            | ex -> logger.Error(ex, "CloseConnection")

    scenario |> getDistinctPools |> Array.iter(closeConnections)
