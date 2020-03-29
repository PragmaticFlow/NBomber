module internal NBomber.Errors

open NBomber.Extensions

type DomainError =
    | InitScenarioError  of ex:exn
    | CleanScenarioError of ex:exn
    | WarmUpErrorWithManyFailedSteps of okCount:int * failedCount:int

type ValidationError =
    | TargetScenarioIsEmpty
    | TargetScenariosNotFound of notFoundScenarios:string list * registeredScenarios:string list
    | DurationIsWrong         of scenarioNames:string[]
    | ConcurrentCopiesIsWrong of scenarioNames:string[]

    // ScenarioValidation errors
    | EmptyReportName
    | EmptyScenarioName
    | DuplicateScenarioName of scenarioNames:string list
    | EmptyStepName         of scenarioName:string
    | EmptySteps            of scenarioName:string
    | CurrentTargetGroupNotMatched  of currentTargetGroup:string
    | TargetGroupsAreNotFound of notFoundTargetGroups:string[]
    | SessionIsWrong
    | SendStatsIntervalIsWrong of minSendStatsInterval:float

    // ConcurrencyScheduler
    | DurationIsLessThan1Sec of simulation:string
    | DurationIsBiggerThan10Days of simulation:string
    | CopiesCountIsZeroOrNegative of simulation:string

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
            sprintf "WarmUp scenario error: to many failed steps: OK:'%i', Failed:'%i'" okCount failedCount

    static member toString (error: ValidationError) =
        match error with
        | TargetScenarioIsEmpty -> "Target scenario can't be empty."

        | TargetScenariosNotFound (notFoundScenarios, registeredScenarios) ->
            notFoundScenarios
            |> String.concatWithCommaAndQuotes
            |> sprintf "Target scenarios %s is not found. Available scenarios are %s." <| String.concatWithCommaAndQuotes(registeredScenarios)

        | DurationIsWrong scenarioNames ->
            scenarioNames |> String.concatWithCommaAndQuotes |> sprintf "Duration for scenarios %s can not be less than 1 sec."

        | ConcurrentCopiesIsWrong scenarioNames ->
            scenarioNames |> String.concatWithCommaAndQuotes |> sprintf "Concurrent copies for scenarios %s can not be less than 1."

        | EmptyReportName -> "Report File Name can not be empty string."
        | EmptyScenarioName -> "Scenario name can not be empty."

        | DuplicateScenarioName scenarioNames ->
            scenarioNames |> String.concatWithCommaAndQuotes |> sprintf "Scenario names are not unique: %s."

        | EmptyStepName scenarioName ->
            sprintf "Step names are empty in scenario: %s." scenarioName

        | EmptySteps scenarioName ->
            sprintf "Scenario '%s' has no steps" scenarioName

        | CurrentTargetGroupNotMatched currentTargetGroup ->
            sprintf "The current target group not matched, current target group is %s" currentTargetGroup

        | TargetGroupsAreNotFound notFoundGroups ->
            notFoundGroups
            |> String.concatWithCommaAndQuotes
            |> sprintf "Target groups are not found: %s"

        | SessionIsWrong ->
            "Session is wrong"

        | SendStatsIntervalIsWrong minSendStatsInterval ->
            sprintf "SendStatsInterval should be bigger than min value: '%f'" minSendStatsInterval

        | DurationIsLessThan1Sec simulation
        | DurationIsBiggerThan10Days simulation
        | CopiesCountIsZeroOrNegative simulation ->
            sprintf "Simulation error: '%s'" simulation

    static member toString (error: AppError) =
        match error with
        | Domain e        -> AppError.toString(e)
        | Validation e    -> AppError.toString(e)
