module internal NBomber.DomainServices.TestHost.TestHostScenario

open FsToolkit.ErrorHandling
open Spectre.Console
open NBomber.Contracts
open NBomber.Contracts.Internal
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Errors
open NBomber.Infra

let getTargetScenarios (sessionArgs: SessionArgs) (regScenarios: Scenario list) =
    regScenarios
    |> Scenario.filterTargetScenarios (sessionArgs.GetTargetScenarios())
    |> Scenario.applySettings (sessionArgs.GetScenariosSettings())

let initEnabledScenarios (dep: IGlobalDependency)
                         (consoleStatus: StatusContext option)
                         (baseContext: IBaseContext)
                         (enabledScenarios: Scenario list) =

    let defaultScnContext = Scenario.ScenarioInitContext.create baseContext

    backgroundTask {
        try
            for scn in enabledScenarios do
                if scn.Init.IsSome then
                    dep.LogInfo("Start init scenario: {0}", scn.ScenarioName)

                    if consoleStatus.IsSome then
                        consoleStatus.Value.Status <- $"Initializing scenario: {Console.okColor scn.ScenarioName}"
                        consoleStatus.Value.Refresh()

                    let scnContext = Scenario.ScenarioInitContext.setCustomSettings defaultScnContext scn.CustomSettings
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

        return
            targetScenarios
            |> List.map(fun scenario -> { scenario with IsInitialized = true })
    with
    | ex -> return! AppError.createResult(InitScenarioError ex)
}

let cleanScenarios (dep: IGlobalDependency)
                   (consoleStatus: StatusContext option)
                   (baseContext: IBaseContext)
                   (enabledScenarios: Scenario list) = backgroundTask {

    let defaultScnContext = Scenario.ScenarioInitContext.create baseContext

    for scn in enabledScenarios do
        if scn.Clean.IsSome then
            dep.LogInfo("Start cleaning scenario: {0}", scn.ScenarioName)

            if consoleStatus.IsSome then
                consoleStatus.Value.Status <- $"Cleaning scenario: {Console.okColor scn.ScenarioName}"
                consoleStatus.Value.Refresh()

            let context = Scenario.ScenarioInitContext.setCustomSettings defaultScnContext scn.CustomSettings
            try
                do! scn.Clean.Value context
            with
            | ex -> dep.LogWarn(ex, "Cleaning scenario failed: {0}", scn.ScenarioName)
}
