module internal NBomber.DomainServices.TestHost.TestHostScenario

open FsToolkit.ErrorHandling
open Spectre.Console

open NBomber.Contracts
open NBomber.Contracts.Internal
open NBomber.Configuration
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Errors
open NBomber.Infra
open NBomber.Infra.Dependency

let getTargetScenarios (sessionArgs: SessionArgs) (regScenarios: Scenario list) =
    regScenarios
    |> Scenario.filterTargetScenarios (sessionArgs.GetTargetScenarios())
    |> Scenario.applySettings (sessionArgs.GetScenariosSettings()) (sessionArgs.GetDefaultStepTimeout())

let initClientFactories (dep: IGlobalDependency)
                        (consoleStatus: StatusContext option)
                        (context: IBaseContext)
                        (clientFactorySettings: ClientFactorySetting list)
                        (enabledScenarios: Scenario list) =

    let factories =
        enabledScenarios
        |> Scenario.ClientFactory.filterDistinct
        |> Scenario.ClientFactory.applySettings clientFactorySettings

    taskResult {
        for f in factories do
            dep.Logger.Information $"Start init client factory: {Console.okColor f.FactoryName}, client count: {Console.blueColor f.ClientCount}"

            for i = 0 to f.ClientCount - 1 do

                if consoleStatus.IsSome then
                    consoleStatus.Value.Status <- $"Initializing client factory: {Console.okColor f.FactoryName}, initialized client: {Console.blueColor(i + 1)}"
                    consoleStatus.Value.Refresh()

                do! ClientFactory.safeInitClient f.InitClient i context
    }
    |> TaskResult.mapError(InitScenarioError >> AppError.create)

let initDataFeeds (dep: IGlobalDependency)
                  (consoleStatus: StatusContext option)
                  (context: IBaseContext)
                  (enabledScenarios: Scenario list) =

    let feeds = Scenario.Feed.filterDistinctFeeds enabledScenarios

    backgroundTask {
        try
            for feed in feeds do

                if consoleStatus.IsSome then
                    consoleStatus.Value.Status <- $"Initializing data feed: {Console.okColor feed.FeedName}"
                    consoleStatus.Value.Refresh()

                do! feed.Init context
                dep.Logger.Information("Initialized data feed: {0}", feed.FeedName)

            return Ok()
        with
        | ex -> return AppError.createResult(InitScenarioError ex)
    }

let initEnabledScenarios (dep: IGlobalDependency)
                         (consoleStatus: StatusContext option)
                         (baseContext: IBaseContext)
                         (enabledScenarios: Scenario list) =

    let defaultScnContext = Scenario.ScenarioContext.create baseContext

    backgroundTask {
        try
            for scn in enabledScenarios do
                if scn.Init.IsSome then
                    dep.Logger.Information("Start init scenario: {Scenario}", scn.ScenarioName)

                    if consoleStatus.IsSome then
                        consoleStatus.Value.Status <- $"Initializing scenario: {Console.okColor scn.ScenarioName}"
                        consoleStatus.Value.Refresh()

                    let scnContext = Scenario.ScenarioContext.setCustomSettings defaultScnContext scn.CustomSettings
                    do! scn.Init.Value scnContext

            return Ok()
        with
        | ex -> return AppError.createResult(InitScenarioError ex)
    }

let initScenarios (dep: IGlobalDependency)
                  (consoleStatus: StatusContext option)
                  (baseContext: IBaseContext)
                  (sessionArgs: SessionArgs)
                  (regScenarios: Scenario list) = taskResult {
    try
        let targetScenarios = regScenarios |> getTargetScenarios sessionArgs
        let enabledScenarios = targetScenarios |> List.filter(fun x -> x.IsEnabled)

        TestHostConsole.printTargetScenarios dep enabledScenarios

        do! enabledScenarios
            |> initEnabledScenarios dep consoleStatus baseContext

        do! enabledScenarios
            |> initClientFactories dep consoleStatus baseContext sessionArgs.UpdatedClientFactorySettings

        do! enabledScenarios
            |> initDataFeeds dep consoleStatus baseContext

        return
            targetScenarios
            |> List.map(fun scenario -> { scenario with IsInitialized = true })
    with
    | ex -> return! AppError.createResult(InitScenarioError ex)
}

let disposeClientFactories (dep: IGlobalDependency)
                           (consoleStatus: StatusContext option)
                           (context: IBaseContext)
                           (enabledScenarios: Scenario list) =

    let factories = enabledScenarios |> Scenario.ClientFactory.filterDistinct

    backgroundTask {
        for f in factories do
            dep.Logger.Information("Start dispose client factory: {0}", f.FactoryName)

            if consoleStatus.IsSome then
                consoleStatus.Value.Status <- $"Disposing client factory: {Console.okColor f.FactoryName}"

            for i = 0 to f.ClientCount - 1 do
                try
                    do! f.DisposeClient(i, context)
                with
                | ex -> dep.Logger.Warning(ex, "Dispose client factory error: {0}", f.FactoryName)
    }

let cleanScenarios (dep: IGlobalDependency)
                   (consoleStatus: StatusContext option)
                   (baseContext: IBaseContext)
                   (enabledScenarios: Scenario list) = backgroundTask {

    let defaultScnContext = Scenario.ScenarioContext.create baseContext

    for scn in enabledScenarios do
        if scn.Clean.IsSome then
            dep.Logger.Information("Start cleaning scenario: {0}", scn.ScenarioName)

            if consoleStatus.IsSome then
                consoleStatus.Value.Status <- $"Cleaning scenario: {Console.okColor scn.ScenarioName}"
                consoleStatus.Value.Refresh()

            let context = Scenario.ScenarioContext.setCustomSettings defaultScnContext scn.CustomSettings
            try
                do! scn.Clean.Value context
            with
            | ex -> dep.Logger.Warning(ex, "Cleaning scenario failed: {0}", scn.ScenarioName)

    do! enabledScenarios
        |> disposeClientFactories dep consoleStatus baseContext
}
