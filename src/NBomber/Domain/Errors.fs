module internal NBomber.Errors

open System.IO
open NBomber.Extensions.InternalExtensions

type DomainError =
    | InitScenarioError  of ex:exn
    | CleanScenarioError of ex:exn
    | WarmUpErrorWithManyFailedSteps of okCount:int * failedCount:int

type ValidationError =
    | TargetScenariosNotFound of notFoundScenarios:string list * registeredScenarios:string list
    | WarmUpConfigValueHasInvalidFormat of scnName:string * warmUpValue:string
    | LoadSimulationConfigValueHasInvalidFormat of scenarioName:string
    | DuplicatedPluginNames of pluginNames:string list

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
    | SendStatsValueSmallerThanMin
    | SendStatsConfigValueHasInvalidFormat of value:string
    | DuplicateConnectionPoolName of scenarioName:string * poolName:string

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
        | InitScenarioError ex  -> sprintf "Init scenario error:'%s'." (ex.ToString())
        | CleanScenarioError ex -> sprintf "Clean scenario error:'%s'." (ex.ToString())
        | WarmUpErrorWithManyFailedSteps (okCount, failedCount) ->
            sprintf "WarmUp scenario error: too many failed steps: OK:'%i', Failed:'%i'." okCount failedCount

    static member toString (error: ValidationError) =
        match error with
        | TargetScenariosNotFound (notFoundScenarios, registeredScenarios) ->
            notFoundScenarios
            |> String.concatWithCommaAndQuotes
            |> sprintf "Target scenarios %s is not found. Available scenarios are %s." <| String.concatWithCommaAndQuotes(registeredScenarios)

        | WarmUpConfigValueHasInvalidFormat (scnName, warmUpValue) ->
            sprintf """ScenariosSettings for scenario '%s' contains invalid WarmUpDuration '%s'. The value should be in this format: "00:00:00".""" scnName warmUpValue

        | LoadSimulationConfigValueHasInvalidFormat scenarioName ->
            sprintf """ScenariosSettings for scenario '%s' contains invalid duration value for LoadSimulationSettings. The value should be in this format: "00:00:00".""" scenarioName

        | EmptyReportName -> "Report file name cannot be empty string."
        | InvalidReportName -> sprintf "Report file name contains invalid chars %A" (Path.GetInvalidFileNameChars())

        | EmptyReportFolderPath -> "Report folder path cannot be empty string."
        | InvalidReportFolderPath -> sprintf "Report folder path contains invalid chars %A" (Path.GetInvalidFileNameChars())

        | EmptyScenarioName -> "Scenario name cannot be empty."
        | DuplicateScenarioName scenarioNames ->
            scenarioNames |> String.concatWithCommaAndQuotes |> sprintf "Scenario names are not unique: %s."

        | EmptyStepName scenarioName -> sprintf "Step names are empty in scenario: %s." scenarioName
        | EmptySteps scenarioName -> sprintf "Scenario '%s' has no steps." scenarioName

        | CurrentTargetGroupNotMatched currentTargetGroup ->
            sprintf "The current target group not matched, current target group is %s." currentTargetGroup

        | TargetGroupsAreNotFound notFoundGroups ->
            notFoundGroups
            |> String.concatWithCommaAndQuotes
            |> sprintf "Target groups are not found: %s"

        | SessionIsWrong ->
            "Session is wrong"

        | SendStatsValueSmallerThanMin ->
            sprintf "SendStatsInterval should be bigger than min value: '%i'." (int Constants.MinSendStatsInterval.TotalSeconds)

        | SendStatsConfigValueHasInvalidFormat value ->
            sprintf """SendStatsInterval config value: '%s' has invalid format. The value should be in this format: "00:00:00".""" value

        | DuplicateConnectionPoolName (scenarioName, poolName) ->
            sprintf "Scenario: '%s' contains connection pool with duplicated name: '%s'." scenarioName poolName

        | SimulationIsSmallerThanMin simulation ->
            sprintf "Simulation duration: '%A' is smaller than min value: '%s'." simulation (Constants.MinSimulationDuration.ToString("hh\:mm\:ss"))

        | SimulationIsBiggerThanMax simulation ->
            sprintf "Simulation duration: '%A' is bigger than max value: '%s'." simulation (Constants.MaxSimulationDuration.ToString("hh\:mm\:ss"))

        | CopiesCountIsZeroOrNegative simulation ->
            sprintf "Simulation: '%A' has invalid copiesCount value. The value should be bigger than 0." simulation

        | RateIsZeroOrNegative simulation ->
            sprintf "Simulation: '%A' has invalid rate value. The value should be bigger than 0." simulation


    static member toString (error: AppError) =
        match error with
        | Domain e        -> AppError.toString(e)
        | Validation e    -> AppError.toString(e)
