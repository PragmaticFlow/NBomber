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
    let nonDeclaredScenarios = globalSettings.ScenariosSettings
                                |> Array.map(fun x -> x.ScenarioName)
                                |> Array.except <| globalSettings.TargetScenarios

    match nonDeclaredScenarios with
    | [||] -> Ok(globalSettings)
    | _ -> Error "Target scenario is not declared"

let validateRunnerContext(context: NBomberRunnerContext) = 
    // check on same scenario name
    // check on same step name within scenario

    let bind = Result.bind
    let validationResult = configNotPreset
                                >> bind globalSettingsNotPresent
                                >> bind targetScenarioIsNotPresent <| context
    
    match validationResult with
    | Ok _ -> Ok(context)
    | Error msg -> Error(msg)