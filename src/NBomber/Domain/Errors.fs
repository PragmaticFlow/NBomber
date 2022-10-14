module internal NBomber.Errors

open System.IO
open NBomber.Extensions.Internal

type DomainError =
    | InitScenarioError  of ex:exn
    | CleanScenarioError of ex:exn

type ValidationError =
    | EmptyRegisterScenarios
    | TargetScenariosNotFound of notFoundScenarios:string list * regScenarios:string list

    // ScenarioValidation errors
    | EmptyScenarioName
    | DuplicateScenarioName of scenarioNames:string list
    | DuplicateScenarioNamesInConfig of scenarioNames:string list

    // ReportingValidation errors
    | EmptyReportName
    | InvalidReportName
    | EmptyReportFolderPath
    | InvalidReportFolderPath
    | ReportingIntervalSmallerThanMin

    // ConcurrencyScheduler
    | SimulationIsSmallerThanMin of simulation:string
    | SimulationIsBiggerThanMax of simulation:string
    | CopiesCountIsZeroOrNegative of simulation:string
    | RateIsZeroOrNegative of simulation:string

type AppError =
    | Domain        of DomainError
    | Validation    of ValidationError
    static member create (e: DomainError) = Domain e
    static member create (e: ValidationError) = Validation e
    static member createResult (e: ValidationError) = Error(Validation e)
    static member createResult (e: DomainError) = Error(Domain e)
    static member createResult (e: AppError) =
        match e with
        | Domain e        -> AppError.createResult(e)
        | Validation e    -> AppError.createResult(e)

    static member toString (error: DomainError) =
        match error with
        | InitScenarioError ex  -> $"Init scenario error: '{ex.ToString()}'"
        | CleanScenarioError ex -> $"Clean scenario error: '{ex.ToString()}'"

    static member toString (error: ValidationError) =
        match error with
        | EmptyRegisterScenarios -> "No scenarios were registered. Please use NBomberRunner.RegisterScenarios(scenarios) to register scenarios"
        | TargetScenariosNotFound (notFoundScenarios, regScenarios) ->
            $"Target scenarios: '{String.concatWithComma regScenarios}' are not found. Available scenarios are: '{String.concatWithComma notFoundScenarios}'"

        | EmptyReportName -> "Report file name cannot be empty string"
        | InvalidReportName -> $"Report file name contains invalid chars: '%A{Path.GetInvalidFileNameChars()}'"

        | EmptyReportFolderPath -> "Report folder path cannot be empty string"
        | InvalidReportFolderPath -> $"Report folder path contains invalid chars: '%A{Path.GetInvalidFileNameChars()}'"

        | EmptyScenarioName -> "Scenario name cannot be empty"

        | DuplicateScenarioName scenarioNames ->
            $"Scenario names are not unique: '{String.concatWithComma scenarioNames}'"

        | ReportingIntervalSmallerThanMin ->
            $"ReportingInterval should be bigger than min value: '{int Constants.MinReportingInterval.TotalSeconds}'"

        | SimulationIsSmallerThanMin simulation ->
            sprintf "Simulation duration: '%A' is smaller than min value: '%s'" simulation (Constants.MinSimulationDuration.ToString("hh\:mm\:ss"))

        | SimulationIsBiggerThanMax simulation ->
            sprintf "Simulation duration: '%A' is bigger than max value: '%s'" simulation (Constants.MaxSimulationDuration.ToString("hh\:mm\:ss"))

        | CopiesCountIsZeroOrNegative simulation ->
            $"Simulation: '{simulation}' has invalid copiesCount value. The value should be bigger than 0"

        | RateIsZeroOrNegative simulation ->
            $"Simulation: '{simulation}' has invalid rate value. The value should be bigger than 0"

        | DuplicateScenarioNamesInConfig scenarioNames ->
            $"Scenario names are not unique in JSON config: '{String.concatWithComma scenarioNames}'"

    static member toString (error: AppError) =
        match error with
        | Domain e        -> AppError.toString(e)
        | Validation e    -> AppError.toString(e)
