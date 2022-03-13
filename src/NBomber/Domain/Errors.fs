module internal NBomber.Errors

open System.IO
open NBomber.Extensions.InternalExtensions

type DomainError =
    | InitScenarioError  of ex:exn
    | CleanScenarioError of ex:exn

type ValidationError =
    | TargetScenariosNotFound of notFoundScenarios:string list * registeredScenarios:string list
    | WarmUpConfigValueHasInvalidFormat of scnName:string * warmUpValue:string
    | LoadSimulationConfigValueHasInvalidFormat of scenarioName:string

    // ScenarioValidation errors
    | EmptyReportName
    | InvalidReportName
    | EmptyReportFolderPath
    | InvalidReportFolderPath
    | EmptyScenarioName
    | DuplicateScenarioName of scenarioNames:string list
    | EmptyStepName         of scenarioName:string
    | EmptySteps            of scenarioName:string
    | CurrentTargetGroupNotMatched  of currentTargetGroup:string
    | TargetGroupsAreNotFound       of notFoundTargetGroups:string[]
    | SessionIsWrong
    | ReportingIntervalSmallerThanMin
    | ReportingIntervalConfigInvalidFormat of value:string
    | InvalidClientFactoryName of factoryName:string
    | DuplicateClientFactoryName of scenarioName:string * factoryName:string
    | DuplicateStepNameButDiffImpl of scenarioName:string * stepName:string

    // ScenarioSettings
    | CustomStepOrderContainsNotFoundStepName of scenarioName:string * stepName:string
    | DuplicateScenarioNamesInConfig of scenarioNames:string list

    // ConcurrencyScheduler
    | SimulationIsSmallerThanMin of simulation:string
    | SimulationIsBiggerThanMax of simulation:string
    | CopiesCountIsZeroOrNegative of simulation:string
    | RateIsZeroOrNegative of simulation:string
    | EnterpriseOnlyFeature of message:string

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
        | InitScenarioError ex  -> $"Init scenario error: {ex.ToString()}"
        | CleanScenarioError ex -> $"Clean scenario error: {ex.ToString()}"

    static member toString (error: ValidationError) =
        match error with
        | TargetScenariosNotFound (notFoundScenarios, registeredScenarios) ->
            $"Target scenarios: '{String.concatWithComma registeredScenarios}' are not found. Available scenarios are: '{String.concatWithComma notFoundScenarios}'"

        | WarmUpConfigValueHasInvalidFormat (scnName, warmUpValue) ->
            $"""ScenariosSettings for Scenario: '{scnName}' contains invalid WarmUpDuration: '{warmUpValue}'. The value should be in this format: "00:00:00" """

        | LoadSimulationConfigValueHasInvalidFormat scenarioName ->
            $"""ScenariosSettings for Scenario: '{scenarioName}' contains invalid duration value for LoadSimulationSettings. The value should be in this format: "00:00:00" """

        | EmptyReportName -> "Report file name cannot be empty string"
        | InvalidReportName -> $"Report file name contains invalid chars: '%A{Path.GetInvalidFileNameChars()}'"

        | EmptyReportFolderPath -> "Report folder path cannot be empty string"
        | InvalidReportFolderPath -> $"Report folder path contains invalid chars: '%A{Path.GetInvalidFileNameChars()}'"

        | EmptyScenarioName -> "Scenario name cannot be empty"

        | DuplicateScenarioName scenarioNames ->
            $"Scenario names are not unique: '{String.concatWithComma scenarioNames}'"

        | EmptyStepName scenarioName -> $"Step names are empty in Scenario: '{scenarioName}'"
        | EmptySteps scenarioName -> $"Scenario: '{scenarioName}' has no steps"

        | CurrentTargetGroupNotMatched currentTargetGroup ->
            $"The current target group not matched, current target group is: '{currentTargetGroup}'"

        | TargetGroupsAreNotFound notFoundGroups ->
            notFoundGroups
            |> String.concatWithComma
            |> sprintf "Target groups are not found: '%s'"

        | SessionIsWrong -> "Session is wrong"

        | ReportingIntervalSmallerThanMin ->
            $"ReportingInterval should be bigger than min value: '{int Constants.MinReportingInterval.TotalSeconds}'"

        | ReportingIntervalConfigInvalidFormat value ->
            $"""ReportingInterval config value: '{value}' has invalid format. The value should be in this format: "00:00:00" """

        | InvalidClientFactoryName factoryName ->
            $"ClientFactory: '{factoryName}' contains not allowed symbol '@'"

        | DuplicateClientFactoryName (scenarioName, factoryName) ->
            $"Scenario: '{scenarioName}' contains client factories with duplicated name: '{factoryName}'"

        | DuplicateStepNameButDiffImpl (scenarioName, stepName) ->
            $"Scenario: '{scenarioName}' contains steps that has duplicate name and different implementations: '{stepName}'. You registered several steps with the same name but different implementations"

        | SimulationIsSmallerThanMin simulation ->
            sprintf "Simulation duration: '%A' is smaller than min value: '%s'" simulation (Constants.MinSimulationDuration.ToString("hh\:mm\:ss"))

        | SimulationIsBiggerThanMax simulation ->
            sprintf "Simulation duration: '%A' is bigger than max value: '%s'" simulation (Constants.MaxSimulationDuration.ToString("hh\:mm\:ss"))

        | CopiesCountIsZeroOrNegative simulation ->
            $"Simulation: '{simulation}' has invalid copiesCount value. The value should be bigger than 0"

        | RateIsZeroOrNegative simulation ->
            $"Simulation: '{simulation}' has invalid rate value. The value should be bigger than 0"

        | CustomStepOrderContainsNotFoundStepName (scenarioName, stepName) ->
            $"Scenario: '{scenarioName}' contains not found step: '{stepName}' in CustomStepOrder"

        | DuplicateScenarioNamesInConfig scenarioNames ->
            $"Scenario names are not unique in JSON config: '{String.concatWithComma scenarioNames}'"

        | EnterpriseOnlyFeature message -> message

    static member toString (error: AppError) =
        match error with
        | Domain e        -> AppError.toString(e)
        | Validation e    -> AppError.toString(e)
