module internal NBomber.DomainServices.Validation

open System

open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result

open NBomber
open NBomber.Extensions
open NBomber.Extensions.Operator.Result
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Errors

module ScenarioValidation =

    let validate (context: NBomberContext) =
        context.RegisteredScenarios
        |> NBomber.Domain.Scenario.createScenarios
        |> Result.map(fun _ -> context)

module GlobalSettingsValidation =

    let checkEmptyTarget (globalSettings: GlobalSettings) =
        let emptyTarget = globalSettings.TargetScenarios
                          |> Option.map(fun x -> x |> List.exists String.IsNullOrWhiteSpace)
                          |> Option.defaultValue false

        if emptyTarget then Error <| TargetScenarioIsEmpty
        else Ok globalSettings

    let checkAvailableTarget (registeredScns: Contracts.Scenario list) (globalSettings: GlobalSettings) =
        let allScenarios = registeredScns |> List.map(fun x -> x.ScenarioName)
        let targetScn = defaultArg globalSettings.TargetScenarios []
        let notFoundScenarios = targetScn |> List.except allScenarios

        if List.isEmpty(notFoundScenarios) then Ok globalSettings
        else Error <| TargetScenariosNotFound(notFoundScenarios, allScenarios)

//    let checkDuration (globalSettings: GlobalSettings) =
//        let invalidScns =
//            globalSettings.ScenariosSettings
//            |> Option.defaultValue List.empty
//            |> List.choose(fun x -> if isDurationOk(x.Duration.TimeOfDay) then None else Some(x.ScenarioName))
//            |> List.toArray
//
//        if Array.isEmpty(invalidScns) then Ok globalSettings
//        else Error <| DurationIsWrong invalidScns

//    let checkConcurrentCopies (globalSettings: GlobalSettings) =
//        let invalidScns =
//            globalSettings.ScenariosSettings
//            |> Option.defaultValue List.empty
//            |> List.choose(fun x -> if isPositiveNumber(x.ConcurrentCopies) then None else Some x.ScenarioName)
//            |> List.toArray
//
//        if Array.isEmpty(invalidScns) then Ok globalSettings
//        else Error <| ConcurrentCopiesIsWrong invalidScns

    let checkEmptyReportName (globalSettings: GlobalSettings) =
        match globalSettings.ReportFileName with
        | Some name -> if String.IsNullOrWhiteSpace(name) then Error <| EmptyReportName
                       else Ok globalSettings
        | None      -> Ok globalSettings

    let checkSendStatsInterval (globalSettings: GlobalSettings) =
        match globalSettings.SendStatsInterval with
        | Some interval ->
            if interval.TimeOfDay.TotalMinutes < Constants.MinSendStatsIntervalSec
                then Error <| SendStatsIntervalIsWrong(Constants.MinSendStatsIntervalSec)
            else
                Ok globalSettings
        | None -> Ok globalSettings

    let validate (context: NBomberContext) =
        context.NBomberConfig
        |> Option.bind(fun x -> x.GlobalSettings)
        |> Option.map(fun glSettings ->
            glSettings
            |> checkEmptyTarget
            >>= checkAvailableTarget(context.RegisteredScenarios)
            //>>= checkDuration
            //>>= checkConcurrentCopies
            >>= checkEmptyReportName
            >>= checkSendStatsInterval
            >>= fun _ -> Ok context
            |> Result.mapError(AppError.create)
        )
        |> Option.defaultValue(Ok context)

let validateContext =
    ScenarioValidation.validate >=> GlobalSettingsValidation.validate
