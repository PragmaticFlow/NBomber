﻿module internal NBomber.DomainServices.Validation

open System

open FsToolkit.ErrorHandling.Operator.Result
open NBomber.Extensions.Operator.Result

open NBomber
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Domain
open NBomber.Errors

let private getDuplicates (data: string[]) =
    data
    |> Array.groupBy(id)
    |> Array.choose(fun (key, set) -> if set.Length > 1 then Some key else None)

let private isDurationOk (duration: TimeSpan) =
    duration >= TimeSpan.FromSeconds 1.0

let private isPositiveNumber (value: int) = 
    value >= 1

module ScenarioValidation =

    let checkEmptyName (scenarios: Contracts.Scenario[]) =
        let emptyScn = scenarios |> Array.tryFind(fun x -> String.IsNullOrWhiteSpace(x.ScenarioName))
        if emptyScn.IsSome then Error <| EmptyScenarioName
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
                    x.Steps
                    |> Array.exists(fun x -> String.IsNullOrWhiteSpace x.StepName)
            
                if emptyStepExist then Some x.ScenarioName else None)

        if Array.isEmpty(scnWithEmptySteps) then Ok scenarios
        else Error <| EmptyStepName scnWithEmptySteps

    let checkDuration (scenarios: Contracts.Scenario[]) =
        let invalidScns = scenarios |> Array.choose(fun x -> if isDurationOk(x.Duration) then None else Some x.ScenarioName)
        if Array.isEmpty(invalidScns) then Ok scenarios
        else Error <| DurationIsWrong invalidScns

    let checkConcurrentCopies (scenarios: Contracts.Scenario[]) =
        let invalidScns = scenarios |> Array.choose(fun x -> if isPositiveNumber(x.ConcurrentCopies) then None else Some x.ScenarioName)
        if Array.isEmpty(invalidScns) then Ok scenarios
        else Error <| ConcurrentCopiesIsWrong invalidScns
        
    let validateWarmUpStats (nodeStats: RawNodeStats) =
        if nodeStats.FailCount > nodeStats.OkCount then
            AppError.createResult(WarmUpErrorWithManyFailedSteps(nodeStats.OkCount, nodeStats.FailCount))
        else Ok()              

    let validate (context: TestContext) =
        context.Scenarios 
        |> checkEmptyName
        >>= checkDuplicateName
        >>= checkEmptyStepName        
        >>= checkDuration
        >>= checkConcurrentCopies        
        >>= fun _ -> Ok context
        |> Result.mapError(AppError.create)

module AssertionValidation =
    
    let checkInvalidAsserts (scenarios: Contracts.Scenario[]) =
        
        let notFoundAsserts = 
            scenarios
            |> Array.collect(fun x ->
                let stepNames =
                    x.Steps
                    |> Array.map(fun step -> step.StepName)

                x.Assertions
                |> Assertion.cast                
                |> Array.filter(fun asrt -> stepNames |> Array.contains(asrt.StepName) |> not))

        if Array.isEmpty(notFoundAsserts) then Ok scenarios
        else Error <| AssertsNotFound notFoundAsserts

    let validate (context: TestContext) =
        context.Scenarios
        |> checkInvalidAsserts
        >>= fun _ -> Ok context
        |> Result.mapError(AppError.create)

module GlobalSettingsValidation =

    let checkEmptyTarget (globalSettings: GlobalSettings) =
        let emptyTarget = globalSettings.TargetScenarios
                          |> Option.map(fun x -> x |> List.exists String.IsNullOrWhiteSpace)
                          |> Option.defaultValue false

        if emptyTarget then Error <| TargetScenarioIsEmpty
        else Ok globalSettings

    let checkAvailableTarget (registeredScns: Contracts.Scenario[]) (globalSettings: GlobalSettings) =
        let allScenarios = registeredScns |> Array.map(fun x -> x.ScenarioName)        
        let targetScn = defaultArg globalSettings.TargetScenarios []
        let notFoundScenarios = targetScn |> List.except allScenarios

        if List.isEmpty(notFoundScenarios) then Ok globalSettings
        else Error <| TargetScenariosNotFound(List.toArray notFoundScenarios, allScenarios)

    let checkDuration (globalSettings: GlobalSettings) =
        let invalidScns =
            globalSettings.ScenariosSettings
            |> Option.defaultValue List.empty
            |> List.choose(fun x -> if isDurationOk(x.Duration.TimeOfDay) then None else Some(x.ScenarioName))
            |> List.toArray                

        if Array.isEmpty(invalidScns) then Ok globalSettings
        else Error <| DurationIsWrong invalidScns

    let checkConcurrentCopies (globalSettings: GlobalSettings) =
        let invalidScns = 
            globalSettings.ScenariosSettings
            |> Option.defaultValue List.empty
            |> List.choose(fun x -> if isPositiveNumber(x.ConcurrentCopies) then None else Some x.ScenarioName)
            |> List.toArray
        
        if Array.isEmpty(invalidScns) then Ok globalSettings
        else Error <| ConcurrentCopiesIsWrong invalidScns    

    let checkEmptyReportName (globalSettings: GlobalSettings) =
        match globalSettings.ReportFileName with
        | Some name -> if String.IsNullOrWhiteSpace(name) then Error <| EmptyReportName
                       else Ok globalSettings
        | None      -> Ok globalSettings
        
    let checkSendStatsInterval (globalSettings: GlobalSettings) =
        match globalSettings.SendStatsInterval with
        | Some interval ->
            if interval.TimeOfDay.TotalMinutes < NBomber.Domain.Constants.MinSendStatsIntervalSec
                then Error <| SendStatsIntervalIsWrong(NBomber.Domain.Constants.MinSendStatsIntervalSec)
            else
                Ok globalSettings
        | None -> Ok globalSettings

    let validate (context: TestContext) =        
        context.NBomberConfig 
        |> Option.bind(fun x -> x.GlobalSettings)
        |> Option.map(fun glSettings -> 
            glSettings
            |> checkEmptyTarget             
            >>= checkAvailableTarget(context.Scenarios)
            >>= checkDuration
            >>= checkConcurrentCopies            
            >>= checkEmptyReportName
            >>= checkSendStatsInterval
            >>= fun _ -> Ok context
            |> Result.mapError(AppError.create)
        )
        |> Option.defaultValue(Ok context)        

module ClusterValidation =

    let validateTargetGroups (allTargetGroups: string[], agentTargetGroups: string[]) = 
        let all = Set.ofArray allTargetGroups
        let agents = Set.ofArray agentTargetGroups        
        
        let notFoundGroups = 
            Set.difference agents all
            |> Set.toArray

        match Array.isEmpty notFoundGroups with
        | true  -> Ok()
        | false -> AppError.createResult(TargetGroupsAreNotFound notFoundGroups)

    let validateTargetGroupScenarios (targetScenarios: string[], registeredScenarios: string[]) = 
        let targetScenarios = targetScenarios |> Set.ofSeq
        let agentScenarios = registeredScenarios |> Set.ofSeq
        
        let notFoundScenarios = 
            Set.difference targetScenarios agentScenarios
            |> Set.toArray
        
        match Array.isEmpty notFoundScenarios with
        | true  -> Ok()
        | false -> AppError.createResult(TargetScenariosNotFound(notFoundScenarios, registeredScenarios)) 

let validateContext = 
    ScenarioValidation.validate
    >=> AssertionValidation.validate
    >=> GlobalSettingsValidation.validate