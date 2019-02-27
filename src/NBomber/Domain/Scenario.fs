[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Scenario

open System
open System.Threading
open NBomber

let updateConnectionPoolCount (concurrentCopies: int) (step) =
    
    let updatePoolCount (pool) =
        let newCount = if pool.ConnectionsCount = Constants.DefaultConnectionsCount then concurrentCopies
                       else pool.ConnectionsCount        
        { pool with ConnectionsCount = newCount }
    
    match step with
    | Action s -> Action { s with ConnectionPool = updatePoolCount(s.ConnectionPool) }        
    | Pause s -> Pause s

let setConnectionPool (allPools: ConnectionPool<obj>[]) (step) =    
    let findPool (poolName) = allPools |> Array.find(fun x -> x.PoolName = poolName)
    match step with
    | Action s -> Action { s with ConnectionPool = findPool(s.ConnectionPool.PoolName) }        
    | Pause s -> Pause s  

let createCorrelationId (scnName: ScenarioName, concurrentCopies: int) =
    [|0 .. concurrentCopies - 1|] 
    |> Array.map(fun i -> sprintf "%s_%i" scnName i)    

let create (config: Contracts.Scenario) =
    let steps = config.Steps |> Array.map(fun x -> x :?> Step |> updateConnectionPoolCount(config.ConcurrentCopies)) |> Seq.toArray        
    let assertions = config.Assertions |> Array.map(fun x -> x :?> Assertion) 

    { ScenarioName = config.ScenarioName
      TestInit = config.TestInit
      TestClean = config.TestClean
      Steps = steps
      Assertions = assertions
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

let init (scenario: Scenario) =    

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
        // todo: refactor, pass token
        if scenario.TestInit.IsSome then
            let cancelToken = new CancellationTokenSource()
            scenario.TestInit.Value(cancelToken.Token).Wait()        

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
            // todo: refacto, pass token
            let cancelToken = new CancellationTokenSource()
            scenario.TestClean.Value(cancelToken.Token).Wait()
    with
    | ex -> Serilog.Log.Error(ex, "TestClean")