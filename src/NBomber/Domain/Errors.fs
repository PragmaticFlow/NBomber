namespace NBomber.Errors

open System
open NBomber.Contracts
open NBomber.Extensions
open NBomber.Domain

type internal DomainError =
    | AssertionError     of assertNumber:int * assertion:Assertion * stats:Statistics
    | InitScenarioError  of ex:exn
    | CleanScenarioError of ex:exn

type internal ValidationError =
    | TargetScenarioIsEmpty
    | TargetScenarioNotFound  of notFoundScenarios:string[] * registeredScenarios:string[]
    | DurationIsWrong         of scenarioNames:string[]
    | ConcurrentCopiesIsWrong of scenarioNames:string[]
    | EmptyReportName
    | EmptyScenarioName
    | DuplicateScenarioName of scenarioNames:string[]
    | EmptyStepName         of scenarioNames:string[]
    | DuplicateStepName     of stepNames:string[]
    | AssertNotFound        of assertNumber:int * assertion:Assertion

type internal CommunicationError =
    | HttpError            of url:Uri * message:string
    | AgentIsWorking
    | OperationFailed      of operationName:string * CommunicationError[]

type internal AppError =
    | Domain        of DomainError
    | Validation    of ValidationError
    | Communication of CommunicationError
    static member create (e: DomainError) = Domain e
    static member create (e: ValidationError) = Validation e
    static member create (e: CommunicationError) = Communication e
    static member createResult (e: ValidationError) = Error(Validation e)
    static member createResult (e: CommunicationError) = Error(Communication e)
    static member createResult (e: DomainError) = Error(Domain e)
    static member createResult (e: AppError) =
        match e with
        | Domain e        -> AppError.createResult e
        | Validation e    -> AppError.createResult e
        | Communication e -> AppError.createResult e

    static member toString (error: DomainError) =
        match error with
        | AssertionError (assertNumber, assertion, stats) ->
            match assertion with
            | Step s ->
                [ sprintf "Assertion #'%i' FAILED for:" assertNumber
                  sprintf "SCENARIO: '%s'" s.ScenarioName
                  sprintf "STEP: '%s'" s.StepName
                  s.Label |> Option.map (fun label -> sprintf "LABEL: '%s'" label)
                          |> Option.defaultValue ""
                  "STATS:"
                  sprintf "%A" stats
                ]|> List.filter ((<>) "")
                |> String.concat Environment.NewLine

        | InitScenarioError ex  -> sprintf "Init scenario error:'%s'." (ex.ToString())
        | CleanScenarioError ex -> sprintf "Clean scenario error:'%s'." (ex.ToString())

    static member toString (error: ValidationError) =
        match error with
        | TargetScenarioIsEmpty -> "Target scenario can't be empty."

        | TargetScenarioNotFound (notFoundScenarios, registeredScenarios) ->
            let notFound = notFoundScenarios |> String.concatWithCommaAndQuotes
            let available = registeredScenarios |> String.concatWithCommaAndQuotes
            sprintf "Target scenarios %s is not found. Available scenarios are %s."
                    notFound available

        | DurationIsWrong scenarioNames ->
            scenarioNames
            |> String.concatWithCommaAndQuotes
            |> sprintf "Duration for scenarios %s can not be less than 1 sec."

        | ConcurrentCopiesIsWrong scenarioNames ->
            scenarioNames
            |> String.concatWithCommaAndQuotes
            |> sprintf "Concurrent copies for scenarios %s can not be less than 1."

        | EmptyReportName -> "Report File Name can not be empty string."
        | EmptyScenarioName -> "Scenario name can not be empty."

        | DuplicateScenarioName scenarioNames ->
            scenarioNames
            |> String.concatWithCommaAndQuotes
            |> sprintf "Scenario names are not unique: %s."

        | EmptyStepName scenarioNames ->
            scenarioNames
            |> String.concatWithCommaAndQuotes
            |> sprintf "Step names are empty in scenarios: %s."

        | DuplicateStepName stepNames ->
            stepNames
            |> String.concatWithCommaAndQuotes
            |> sprintf "Step names are not unique: %s."

        | AssertNotFound (assertNumber, assertion) ->
            match assertion with
            | Step s -> sprintf "Assertion #'%i' is not found for step: '%s' in scenario: '%s'"
                                assertNumber s.StepName s.ScenarioName

    static member toString (error: CommunicationError) =
        match error with
        | HttpError (url, msg) -> sprintf "HttpError url: '%s', error: '%s'." (url.ToString()) msg
        | AgentIsWorking -> "AgentError error: agent is working"
        | OperationFailed (operationName, ers) ->
            ers
            |> Array.map AppError.toString
            |> String.concatWithCommaAndQuotes
            |> sprintf "OperationFailed: '%s', %s %s" operationName Environment.NewLine

    static member toString (error: AppError) =
        match error with
        | Domain e        -> AppError.toString e
        | Validation e    -> AppError.toString e
        | Communication e -> AppError.toString e
