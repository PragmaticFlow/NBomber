﻿module internal NBomber.DomainServices.Validation

open System

open FsToolkit.ErrorHandling.Operator.Result

open NBomber
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Domain

let private getDuplicates (data: string[]) =
    data
    |> Array.groupBy(id)
    |> Array.choose(fun (key, set) -> if set.Length > 1 then Some key else None)

let private isDurationOk (duration: TimeSpan) =
    duration >= TimeSpan.FromSeconds 1.0

let private isConcurrentCopiesOk (value: int) = 
    value >= 1

module ScenarioValidation =

    let checkEmptyName (scenarios: Contracts.Scenario[]) =
        let emptyScn = scenarios |> Array.tryFind(fun x -> String.IsNullOrWhiteSpace(x.ScenarioName))
        if emptyScn.IsSome then Error EmptyScenarioName
        else Ok scenarios

    let checkDuplicateName (scenarios: Contracts.Scenario[]) =
        let duplicates = scenarios |> Array.map(fun x -> x.ScenarioName) |> getDuplicates
        if duplicates.Length > 0 then Error <| DuplicateScenarioName duplicates
        else Ok scenarios

    let checkEmptyStepName (scenarios: Contracts.Scenario[]) =
        let scnWithEmptySteps =
            scenarios
            |> Array.choose(fun x -> 
                let emptyStepExist = 
                    x.Steps |> Step.create
                    |> Array.exists(fun x -> String.IsNullOrWhiteSpace x.StepName)
            
                if emptyStepExist then Some x.ScenarioName else None)

        if Array.isEmpty(scnWithEmptySteps) then Ok scenarios
        else scnWithEmptySteps |> EmptyStepName |> Error

    let checkDuplicateStepName (scenarios: Contracts.Scenario[]) =    
        let duplicates = 
            scenarios
            |> Array.collect(fun x -> x.Steps |> Step.create |> Array.map(fun x -> x.StepName) |> getDuplicates)
    
        if Array.isEmpty(duplicates) then Ok scenarios
        else Error <| DuplicateStepName duplicates

    let checkDuration (scenarios: Contracts.Scenario[]) =
        let invalidScns = scenarios |> Array.choose(fun x -> if isDurationOk(x.Duration) then None else Some x.ScenarioName)
        if Array.isEmpty(invalidScns) then Ok scenarios
        else Error <| DurationIsWrong invalidScns

    let checkConcurrentCopies (scenarios: Contracts.Scenario[]) =
        let invalidScns = scenarios |> Array.choose(fun x -> if isConcurrentCopiesOk(x.ConcurrentCopies) then None else Some x.ScenarioName)
        if Array.isEmpty(invalidScns) then Ok scenarios
        else Error <| ConcurrentCopiesIsWrong invalidScns

    let validate (context: NBomberContext) =
        context.Scenarios 
        |> checkEmptyName
        >>= checkDuplicateName
        >>= checkEmptyStepName
        >>= checkDuplicateStepName
        >>= checkDuration
        >>= checkConcurrentCopies
        >>= fun _ -> Ok context

module GlobalSettingsValidation =

    let checkEmptyTarget (globalSettings: GlobalSettings) =
        let emptyTarget = globalSettings.TargetScenarios |> List.exists(String.IsNullOrWhiteSpace)
        if emptyTarget then Error TargetScenarioIsEmpty
        else Ok globalSettings

    let checkAvailableTarget (globalSettings: GlobalSettings) =
        let allScenarios = globalSettings.ScenariosSettings |> List.map(fun x -> x.ScenarioName)
        let notFoundScenarios = globalSettings.TargetScenarios |> List.except allScenarios
        if List.isEmpty(notFoundScenarios) then Ok globalSettings
        else Error <| TargetScenarioNotFound(List.toArray notFoundScenarios, List.toArray allScenarios)

    let checkDuration (globalSettings: GlobalSettings) =
        let invalidScns =
            globalSettings.ScenariosSettings
            |> List.choose(fun x -> if isDurationOk(x.Duration.TimeOfDay) then None else Some(x.ScenarioName))
            |> List.toArray                

        if Array.isEmpty(invalidScns) then Ok globalSettings
        else Error <| DurationIsWrong invalidScns

    let checkConcurrentCopies (globalSettings: GlobalSettings) =
        let invalidScns = 
            globalSettings.ScenariosSettings
            |> List.choose(fun x -> if isConcurrentCopiesOk(x.ConcurrentCopies) then None else Some x.ScenarioName)
            |> List.toArray
        
        if Array.isEmpty(invalidScns) then Ok globalSettings
        else Error <| ConcurrentCopiesIsWrong invalidScns       

    let checkEmptyReportName (globalSettings: GlobalSettings) =
        match globalSettings.ReportFileName with
        | Some name -> if String.IsNullOrWhiteSpace(name) then Error EmptyReportName
                       else Ok globalSettings
        | None      -> Ok globalSettings

    let validate (context: NBomberContext) =         
        context.NBomberConfig 
        |> Option.bind(fun x -> x.GlobalSettings)
        |> Option.map(fun glSettings -> 
            glSettings 
            |> checkEmptyTarget
            >>= checkAvailableTarget
            >>= checkDuration
            >>= checkConcurrentCopies
            >>= checkEmptyReportName
            >>= fun _ -> Ok context
        )
        |> Option.defaultValue(Ok context)        

let validateContext (context: NBomberContext) = 
    context
    |> ScenarioValidation.validate
    >>= GlobalSettingsValidation.validate
