module internal NBomber.DomainServices.TestHost.TestHostScenario

open FSharp.Control.Tasks.NonAffine
open FsToolkit.ErrorHandling

open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.ConnectionPool
open NBomber.DomainServices.NBomberContext
open NBomber.Errors
open NBomber.Infra.Dependency

let getTargetScenarios (sessionArgs: SessionArgs) (registeredScenarios: Scenario list) =
    registeredScenarios
    |> Scenario.filterTargetScenarios sessionArgs.TargetScenarios
    |> Scenario.applySettings sessionArgs.ScenariosSettings

let initConnectionPools (dep: IGlobalDependency) (context: IBaseContext) (pools: ConnectionPool list) = taskResult {
    try
        for pool in pools do
            dep.Logger.Information("Start opening {ConnectionCount} connections for connection pool: '{PoolName}'.", pool.ConnectionCount, pool.PoolName)
            let progressTask = TestHostConsole.displayConnectionPoolsProgress(dep, [pool])
            do! pool.Init(context) |> TaskResult.mapError(InitScenarioError >> AppError.create)
            progressTask.Wait()

        return pools
    with
    | ex -> return! AppError.createResult(InitScenarioError ex)
}

let initDataFeeds (dep: IGlobalDependency) (context: IBaseContext) (feeds: IFeed<obj> list) = taskResult {
    try
        for feed in feeds do
            do! feed.Init(context)
            dep.Logger.Information("Initialized data feed: '{FeedName}'.", feed.FeedName)

        return feeds
    with
    | ex -> return! AppError.createResult(InitScenarioError ex)
}

let initScenarios (dep: IGlobalDependency)
                  (baseContext: IBaseContext)
                  (defaultScnContext: IScenarioContext)
                  (sessionArgs: SessionArgs)
                  (targetScenarios: Scenario list) = taskResult {
    try
        TestHostConsole.printTargetScenarios dep targetScenarios

        // scenario init
        for scn in targetScenarios do
            match scn.Init with
            | Some initFunc ->

                dep.Logger.Information("Start init scenario: '{Scenario}'.", scn.ScenarioName)
                let scnContext = Scenario.ScenarioContext.setCustomSettings defaultScnContext scn.CustomSettings
                do! initFunc scnContext

            | None -> ()

        // connection pools init
        let! pools =
            targetScenarios
            |> Scenario.ConnectionPool.createConnectionPools sessionArgs.ConnectionPoolSettings
            |> initConnectionPools dep baseContext

        // data feed init
        do! targetScenarios
            |> Scenario.Feed.filterDistinctAndEmptyFeeds
            |> initDataFeeds dep baseContext
            |> TaskResult.ignore

        return targetScenarios
               |> Scenario.ConnectionPool.setConnectionPools pools
    with
    | ex -> return! AppError.createResult(InitScenarioError ex)
}

let cleanScenarios (dep: IGlobalDependency)
                   (baseContext: IBaseContext)
                   (defaultScnContext: IScenarioContext)
                   (scenarios: Scenario list) = task {

    let destroyConnectionPools (dep: IGlobalDependency) (context: IBaseContext) (pools: ConnectionPool list) =
        for pool in pools do
            dep.Logger.Information("Start closing {ConnectionCount} connections for connection pool: '{PoolName}'.", pool.ConnectionCount, pool.PoolName)
            let progressTask = TestHostConsole.displayConnectionPoolsProgress(dep, List.singleton(pool))
            pool.Destroy(context).Wait()
            progressTask.Wait()

    scenarios
    |> Scenario.ConnectionPool.filterDistinctConnectionPools
    |> destroyConnectionPools dep baseContext

    for scn in scenarios do
        match scn.Clean with
        | Some cleanFunc ->
            dep.Logger.Information("Start cleaning scenario: '{Scenario}'.", scn.ScenarioName)

            let context = Scenario.ScenarioContext.setCustomSettings defaultScnContext scn.CustomSettings
            try
                do! cleanFunc context
            with
            | ex -> dep.Logger.Warning(ex, "Cleaning scenario failed: '{Scenario}'.", scn.ScenarioName)

        | None -> ()
}
