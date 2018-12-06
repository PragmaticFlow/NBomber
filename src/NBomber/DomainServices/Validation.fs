module internal NBomber.DomainServices.Validation

open System

open NBomber.Contracts
open NBomber.Configuration
    
let targetScenarioIsNotPresent (globalSettings: GlobalSettings) =
    let availableScenarios = globalSettings.ScenariosSettings |> Array.map(fun x -> x.ScenarioName)
    let notFound = globalSettings.TargetScenarios |> Array.except availableScenarios
    
    match notFound with
    | [||] -> Ok(globalSettings)
    | _ -> sprintf "Target scenarios %A is not found. Available scenarios are %A" notFound availableScenarios
            |> Error

let durationGreaterThenSecond (globalSettings: GlobalSettings) =
    globalSettings.ScenariosSettings
    |> Array.filter(fun x -> x.Duration < TimeSpan.FromSeconds(1.0))
    |> Array.map(fun x -> x.ScenarioName)
    |> function
    | [||] -> Ok(globalSettings)
    | scenariosWithIncorrectDuration -> sprintf "Duration for scenarios %A can not be less than 1 sec" scenariosWithIncorrectDuration
                                        |> Error

let concurrentCopiesGreaterThenOne (globalSettings: GlobalSettings) =
    globalSettings.ScenariosSettings
    |> Array.filter(fun x -> x.ConcurrentCopies < 1)
    |> Array.map(fun x -> x.ScenarioName)
    |> function
    | [||] -> Ok(globalSettings)
    | scenariosWithIncorrectConcurrentCopies -> sprintf "Concurrent copies for scenarios %A can not be less than 1" scenariosWithIncorrectConcurrentCopies
                                                |> Error

let validateRunnerContext(context: NBomberRunnerContext) = 
    let globalSettings = context.NBomberConfig |> Option.bind (fun config -> config.GlobalSettings)
    match globalSettings with
    | Some globalSettings -> globalSettings
                            |> targetScenarioIsNotPresent 
                            |> Result.bind durationGreaterThenSecond
                            |> Result.bind concurrentCopiesGreaterThenOne
                            |> function
                            | Ok _ -> Ok(context)
                            | Error msg -> Error(msg)
    | None -> Ok(context)