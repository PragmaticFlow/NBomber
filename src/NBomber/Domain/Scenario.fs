[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Scenario

open System
open System.Threading
open NBomber
open NBomber.Configuration
open NBomber.Errors

let updateConnectionPoolCount (concurrentCopies: int) (step: Step) =

    let updatePoolCount (pool) =
        let newCount = if pool.ConnectionsCount = Constants.DefaultConnectionsCount then concurrentCopies
                       else pool.ConnectionsCount
        { pool with ConnectionsCount = newCount }

    { step with ConnectionPool = updatePoolCount(step.ConnectionPool) }

let setConnectionPool (allPools: ConnectionPool<obj>[]) (step: Step) =
    let findPool (poolName) = allPools |> Array.find(fun x -> x.PoolName = poolName)
    { step with ConnectionPool = findPool(step.ConnectionPool.PoolName) }

let createCorrelationId (scnName: ScenarioName, concurrentCopies: int) =
    [|0 .. concurrentCopies - 1|]
    |> Array.map(fun i -> sprintf "%s_%i" scnName i)

let create (config: Contracts.Scenario) =
    let steps = config.Steps |> Step.create |> Array.map(fun x -> x |> updateConnectionPoolCount(config.ConcurrentCopies))
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
    |> Array.map(fun x -> x.ConnectionPool)
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
    | ex -> Error <| InitScenarioError ex

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

let filterTargetScenarios (targetScenarios: string[]) (allScenarios: Scenario[]) =
    match targetScenarios with
    | [||] -> allScenarios
    | _    ->
        allScenarios
        |> Array.filter(fun x -> targetScenarios |> Array.exists(fun target -> x.ScenarioName = target))

let applySettings (settings: ScenarioSetting[]) (scenarios: Scenario[]) =

    let updateScenario (scenario: Scenario, settings: ScenarioSetting) =
        { scenario with ConcurrentCopies = settings.ConcurrentCopies
                        WarmUpDuration = settings.WarmUpDuration.TimeOfDay
                        Duration = settings.Duration.TimeOfDay }

    scenarios
    |> Array.map(fun scn ->
        settings
        |> Array.tryPick(fun x ->
            if x.ScenarioName = scn.ScenarioName then Some(scn, x)
            else None)
        |> Option.map(updateScenario)
        |> Option.defaultValue(scn))
