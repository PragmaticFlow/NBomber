module internal NBomber.Domain.Scenario

open System
open System.Diagnostics

open NBomber
open NBomber.Domain
open NBomber.Domain.Errors
open NBomber.Domain.DomainTypes

let updateConnectionPoolCount (concurrentCopies: int) (step) =
    
    let updatePoolCount (pool) =
        let newCount = if pool.ConnectionsCount = Constants.DefaultConnectionsCount then concurrentCopies
                       else pool.ConnectionsCount        
        { pool with ConnectionsCount = newCount }
    
    match step with
    | Pull s -> Pull { s with ConnectionPool = updatePoolCount(s.ConnectionPool) }    
    | Push s -> Push { s with ConnectionPool = updatePoolCount(s.ConnectionPool) }
    | Pause s -> Pause s

let setConnectionPool (allPools: ConnectionPool<obj>[]) (step) =    
    let findPool (poolName) = allPools |> Array.find(fun x -> x.PoolName = poolName)
    match step with
    | Pull s -> Pull { s with ConnectionPool = findPool(s.ConnectionPool.PoolName) }    
    | Push s -> Push { s with ConnectionPool = findPool(s.ConnectionPool.PoolName) }
    | Pause s -> Pause s  

let createCorrelationId (scnName: ScenarioName, concurrentCopies: int) =
    [|0 .. concurrentCopies - 1|] 
    |> Array.map(fun i -> System.String.Format("{0}_{1}", scnName, i))    

let create (config: Contracts.Scenario) =    
    { ScenarioName = config.ScenarioName
      TestInit = config.TestInit
      TestClean = config.TestClean
      Steps = config.Steps |> Array.map(fun x -> x :?> Step |> updateConnectionPoolCount(config.ConcurrentCopies)) |> Seq.toArray
      Assertions = config.Assertions |> Array.map(fun x -> x :?> Assertion) 
      ConcurrentCopies = config.ConcurrentCopies      
      CorrelationIds = createCorrelationId(config.ScenarioName, config.ConcurrentCopies)
      WarmUpDuration = config.WarmUpDuration
      Duration = config.Duration }
          
let getDistinctPools (scenario: Scenario) =
    scenario.Steps
    |> Array.map(Step.getConnectionPool) 
    |> Array.filter(Option.isSome)
    |> Array.map(Option.get)
    |> Array.distinct    

let runInit (scenario: Scenario) =    

    let initConnectionPool (pool: ConnectionPool<obj>) =
        let connections = System.Collections.Generic.List<obj>()
        for i = 1 to pool.ConnectionsCount do
            let connection = pool.OpenConnection()
            connections.Add(connection)
        { pool with AliveConnections = connections.ToArray() }

    let initAllConnectionPools () = 
        getDistinctPools(scenario)
        |> Array.map(initConnectionPool)    

    try
        if scenario.TestInit.IsSome then scenario.TestInit.Value()        
        let allPools = initAllConnectionPools()            
        let steps = scenario.Steps |> Array.map(setConnectionPool(allPools))
        Ok { scenario with Steps = steps }
    with 
    | ex -> ex |> InitScenarioError |> Error

let clean (scenario: Scenario) =     

    let invokeDispose (connection: obj) =
        if connection :? IDisposable then (connection :?> IDisposable).Dispose()

    let closePoolConnections (pool) =
        for connection in pool.AliveConnections do
            try 
                match pool.CloseConnection with
                | Some close -> close(connection)
                                invokeDispose(connection)
                | None       -> invokeDispose(connection)
            with
            | ex -> Serilog.Log.Error(ex, "CloseConnection")
    
    getDistinctPools(scenario)
    |> Array.iter(closePoolConnections) 

    try
        if scenario.TestClean.IsSome then
            scenario.TestClean.Value()
    with
    | ex -> Serilog.Log.Error(ex, "TestClean")
        