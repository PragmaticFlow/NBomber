module internal NBomber.DomainServices.Validation

open NBomber.Contracts
open NBomber.Configuration

let configNotPreset (context : NBomberRunnerContext) =
    match context.NBomberConfig with 
    | Some config -> Ok(config)
    | None -> Error "Config file is not present"

let globalSettingsNotPresent (config : NBomberConfig) =
    match config.GlobalSettings with 
    | Some globalSettings -> Ok(globalSettings)
    | None -> Error "Global settings is not present"
    
let targetScenarioIsNotPresent (globalSettings : GlobalSettings) =
    let declaredScenarios = globalSettings.ScenariosSettings |> Array.map(fun x -> x.ScenarioName)
    globalSettings.TargetScenarios |> Array.except declaredScenarios
    |> function
    | [||] -> Ok(globalSettings)
    | _ -> Error "Target scenario is not declared"

let validateRunnerContext(context: NBomberRunnerContext) = 
    // check on same scenario name
    // check on same step name within scenario

    context
    |> configNotPreset
    |> Result.bind globalSettingsNotPresent
    |> Result.bind targetScenarioIsNotPresent
    |> function
    | Ok _ -> Ok(context)
    | Error msg -> Error(msg)