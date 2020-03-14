module internal NBomber.DomainServices.Validation

open System

open FsToolkit.ErrorHandling.Operator.Result
open NBomber.Extensions.Operator.Result

open NBomber
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Domain
open NBomber.Errors

let private getDuplicates (data: string list) =
    data
    |> List.groupBy(id)
    |> List.choose(fun (key, set) -> if set.Length > 1 then Some key else None)

let private isDurationOk (duration: TimeSpan) =
    duration >= TimeSpan.FromSeconds 1.0

let private isPositiveNumber (value: int) =
    value >= 1

module ScenarioValidation =

    let checkEmptyName (scenarios: Contracts.Scenario list) =
        let emptyScn = scenarios |> List.tryFind(fun x -> String.IsNullOrWhiteSpace(x.ScenarioName))
        if emptyScn.IsSome then Error <| EmptyScenarioName
        else Ok scenarios

    let checkDuplicateName (scenarios: Contracts.Scenario list) =
        let duplicates = scenarios |> List.map(fun x -> x.ScenarioName) |> getDuplicates
        if duplicates.Length > 0 then Error <| DuplicateScenarioName duplicates
        else Ok scenarios

    let checkEmptyStepName (scenarios: Contracts.Scenario list) =
        let scnWithEmptySteps =
            scenarios
            |> List.choose(fun x ->
                let emptyStepExist =
                    x.Steps
                    |> Seq.exists(fun x -> String.IsNullOrWhiteSpace x.StepName)

                if emptyStepExist then Some x.ScenarioName else None)

        if Seq.isEmpty(scnWithEmptySteps) then Ok scenarios
        else Error <| EmptyStepName scnWithEmptySteps

//    let checkDuration (scenarios: Contracts.Scenario[]) =
//        let invalidScns = scenarios |> Array.choose(fun x -> if isDurationOk(x.Duration) then None else Some x.ScenarioName)
//        if Array.isEmpty(invalidScns) then Ok scenarios
//        else Error <| DurationIsWrong invalidScns

//    let checkConcurrentCopies (scenarios: Contracts.Scenario[]) =
//        let invalidScns = scenarios |> Array.choose(fun x -> if isPositiveNumber(x.ConcurrentCopies) then None else Some x.ScenarioName)
//        if Array.isEmpty(invalidScns) then Ok scenarios
//        else Error <| ConcurrentCopiesIsWrong invalidScns

    let validateWarmUpStats (nodesStats: RawNodeStats list) =

        let okState = Ok()

        let folder (state) (stats: RawNodeStats) =
            state |> Result.bind(fun _ ->
                if stats.FailCount > stats.OkCount then
                    AppError.createResult(WarmUpErrorWithManyFailedSteps(stats.OkCount, stats.FailCount))
                else Ok()
            )

        nodesStats |> List.fold folder okState

    let validate (context: TestContext) =
        context.RegisteredScenarios
        |> checkEmptyName
        >>= checkDuplicateName
        >>= checkEmptyStepName
        //>>= checkDuration
        //>>= checkConcurrentCopies
        >>= fun _ -> Ok context
        |> Result.mapError(AppError.create)

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

    let validate (context: TestContext) =
        context.TestConfig
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
    ScenarioValidation.validate
    >=> GlobalSettingsValidation.validate
