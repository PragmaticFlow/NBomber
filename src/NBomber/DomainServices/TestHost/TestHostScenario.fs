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
                         (sessionArgs: SessionArgs)
                         (enabledScenarios: Scenario list) = backgroundTask {    
    try 
        for scn in enabledScenarios do
            if scn.Init.IsSome then
                dep.LogInfo("Start init scenario: {0}", scn.ScenarioName)

                let scnInfo = Scenario.createScenarioInfo(scn.ScenarioName, scn.PlanedDuration, 0, ScenarioOperation.Init)
                
                let initScnContext =
                    sessionArgs.ScenarioPartitions.TryFind scn.ScenarioName
                    |> Option.defaultValue ScenarioPartition.empty
                    |> Scenario.ScenarioInitContext.create scnInfo baseContext scn.CustomSettings
                
                if consoleStatus.IsSome then
                    consoleStatus.Value.Status <- $"Initializing scenario: {Console.okColor scn.ScenarioName}"
                    consoleStatus.Value.Refresh()
                
                do! scn.Init.Value initScnContext

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
            |> initEnabledScenarios dep consoleStatus baseContext sessionArgs

        return
            targetScenarios
            |> List.map(fun scenario -> { scenario with IsInitialized = true })
    with
    | ex -> return! AppError.createResult(InitScenarioError ex)
}

let cleanScenarios (dep: IGlobalDependency)
                   (consoleStatus: StatusContext option)
                   (baseContext: IBaseContext)
                   (sessionArgs: SessionArgs)
                   (enabledScenarios: Scenario list) = backgroundTask {    

    for scn in enabledScenarios do
        if scn.Clean.IsSome then
            dep.LogInfo("Start cleaning scenario: {0}", scn.ScenarioName)

            if consoleStatus.IsSome then
                consoleStatus.Value.Status <- $"Cleaning scenario: {Console.okColor scn.ScenarioName}"
                consoleStatus.Value.Refresh()
                
            let duration = Scenario.getExecutedDuration scn                
            let scnInfo = Scenario.createScenarioInfo(scn.ScenarioName, duration, 0, ScenarioOperation.Clean)                

            let initScnContext =
                sessionArgs.ScenarioPartitions.TryFind scn.ScenarioName
                |> Option.defaultValue ScenarioPartition.empty
                |> Scenario.ScenarioInitContext.create scnInfo baseContext scn.CustomSettings            
            
            try
                do! scn.Clean.Value initScnContext
            with
            | ex -> dep.LogWarn(ex, "Cleaning scenario failed: {0}", scn.ScenarioName)
}
