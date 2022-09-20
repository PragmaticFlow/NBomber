module internal NBomber.DomainServices.TestHost.TestHostScenario

open FsToolkit.ErrorHandling
open Spectre.Console

open NBomber.Contracts
open NBomber.Contracts.Internal
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.ClientPool
open NBomber.Errors
open NBomber.Infra
open NBomber.Infra.Dependency

let getTargetScenarios (sessionArgs: SessionArgs) (regScenarios: Scenario list) =
    regScenarios
    |> Scenario.filterTargetScenarios (sessionArgs.GetTargetScenarios())
    |> Scenario.applySettings (sessionArgs.GetScenariosSettings()) (sessionArgs.GetDefaultStepTimeout())

let initClientPools (dep: IGlobalDependency) (consoleStatus: StatusContext) (context: IBaseContext) (pools: ClientPool list) = taskResult {
    try
        for pool in pools do
            TestHostConsole.displayClientPoolProgress(dep, consoleStatus, pool)

            do! pool.Init(context)
                |> TaskResult.mapError(InitScenarioError >> AppError.create)

        return pools
    with
    | ex -> return! AppError.createResult(InitScenarioError ex)
}

let initDataFeeds (dep: IGlobalDependency) (consoleStatus: StatusContext) (context: IBaseContext) (feeds: IFeed<obj> list) = taskResult {
    try
        for feed in feeds do
            consoleStatus.Status <- $"Initializing data feed: {Console.okColor feed.FeedName}"
            consoleStatus.Refresh()
            do! feed.Init(context)
            dep.Logger.Information("Initialized data feed: {0}", feed.FeedName)

        return feeds
    with
    | ex -> return! AppError.createResult(InitScenarioError ex)
}

let initScenarios (dep: IGlobalDependency,
                   consoleStatus: StatusContext,
                   baseContext: IBaseContext,
                   sessionArgs: SessionArgs,
                   regScenarios: Scenario list) = taskResult {
    try
        let targetScenarios = regScenarios |> getTargetScenarios sessionArgs
        let defaultScnContext = Scenario.ScenarioContext.create baseContext

        let enabledScenarios = targetScenarios |> List.filter(fun x -> x.IsEnabled)
        let disabledScenarios = targetScenarios |> List.filter(fun x -> not x.IsEnabled)

        TestHostConsole.printTargetScenarios dep enabledScenarios

        // scenario init
        for scn in enabledScenarios do
            match scn.Init with
            | Some initFunc ->
                dep.Logger.Information("Start init scenario: {Scenario}", scn.ScenarioName)
                consoleStatus.Status <- $"Initializing scenario: {Console.okColor scn.ScenarioName}"
                consoleStatus.Refresh()
                let scnContext = Scenario.ScenarioContext.setCustomSettings defaultScnContext scn.CustomSettings
                do! initFunc scnContext

            | None -> ()

        // client pools init
        let! pools =
            enabledScenarios
            |> Scenario.ClientPool.createPools sessionArgs.UpdatedClientFactorySettings
            |> initClientPools dep consoleStatus baseContext

        // data feed init
        do! enabledScenarios
            |> Scenario.Feed.filterDistinctFeeds
            |> initDataFeeds dep consoleStatus baseContext
            |> TaskResult.ignore

        return
            enabledScenarios
            |> Scenario.ClientPool.setPools pools
            |> List.append disabledScenarios
            |> List.map(fun scenario -> { scenario with IsInitialized = true })
    with
    | ex -> return! AppError.createResult(InitScenarioError ex)
}

let disposeClientPools (dep: IGlobalDependency) (consoleStatus: StatusContext) (baseContext: IBaseContext) (pools: ClientPool list) =
    for pool in pools do
        TestHostConsole.displayClientPoolProgress(dep, consoleStatus, pool)
        pool.DisposePool(baseContext)

let cleanScenarios (dep: IGlobalDependency)
                   (consoleStatus: StatusContext)
                   (baseContext: IBaseContext)
                   (defaultScnContext: IScenarioContext)
                   (scenarios: Scenario list) = backgroundTask {

    scenarios
    |> Scenario.ClientPool.filterDistinct
    |> disposeClientPools dep consoleStatus baseContext

    for scn in scenarios do
        match scn.Clean with
        | Some cleanFunc ->
            dep.Logger.Information("Start cleaning scenario: {Scenario}", scn.ScenarioName)

            let context = Scenario.ScenarioContext.setCustomSettings defaultScnContext scn.CustomSettings
            try
                do! cleanFunc context
            with
            | ex -> dep.Logger.Warning(ex, "Cleaning scenario failed: {Scenario}", scn.ScenarioName)

        | None -> ()
}
