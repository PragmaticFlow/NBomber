namespace NBomber.Errors

open System
open NBomber.Contracts
open NBomber.Extensions
open NBomber.Domain

type internal DomainError =
    | AssertionError     of assertNumber:int * assertion:Assertion * stats:Statistics
    | InitScenarioError  of ex:exn           
    | CleanScenarioError of ex:exn
    | WarmUpErrorWithManyFailedSteps of okCount:int * failedCount:int

type internal ValidationError =
    | TargetScenarioIsEmpty
    | TargetScenariosNotFound  of notFoundScenarios:string[] * registeredScenarios:string[]
    | DurationIsWrong         of scenarioNames:string[]
    | ConcurrentCopiesIsWrong of scenarioNames:string[]    
    
    // ScenarioValidation errors
    | EmptyReportName 
    | EmptyScenarioName
    | DuplicateScenarioName of scenarioNames:string[]
    | EmptyStepName         of scenarioNames:string[]      
    | AssertsNotFound       of Assertion[]
    | CurrentTargetGroupNotMatched  of currentTargetGroup:string
    | TargetGroupsAreNotFound of notFoundTargetGroups:string[]
    | SessionIsWrong
    | SendStatsIntervalIsWrong of minSendStatsInterval:float

type internal CommunicationError =
    | SendMqttMsgFailed    
    | NotAllStatsReceived

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
        | Domain e        -> AppError.createResult(e)
        | Validation e    -> AppError.createResult(e)
        | Communication e -> AppError.createResult(e)
    
    static member toString (error: DomainError) = 
        match error with
        | AssertionError (assertNumber, asrt, stats) -> 
            let statsJson = sprintf "%A" stats                    
            let scenarioStr = sprintf "SCENARIO: '%s' %s" asrt.ScenarioName Environment.NewLine
            let stepStr     = sprintf "STEP: '%s' %s" asrt.StepName Environment.NewLine
            let labelStr = if asrt.Label.IsSome then sprintf "LABEL: '%s' %s" asrt.Label.Value Environment.NewLine
                            else String.Empty 
            let statsStr = sprintf "STATS: %s %s %s" Environment.NewLine statsJson Environment.NewLine
            sprintf "Assertion #'%i' FAILED for: %s %s %s %s %s" assertNumber Environment.NewLine scenarioStr stepStr labelStr statsStr
        
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
        
        | EmptyStepName scenarioNames -> 
            scenarioNames |> String.concatWithCommaAndQuotes |> sprintf "Step names are empty in scenarios: %s."

        | AssertsNotFound asserts ->
            asserts
            |> Array.map(fun a ->  sprintf "Assertion is not found for step: '%s' in scenario: '%s'" a.StepName a.ScenarioName)
            |> String.concatWithCommaAndQuotes

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

    static member toString (error: CommunicationError) = 
        match error with
        | SendMqttMsgFailed -> "Error during sending request."        
        | NotAllStatsReceived -> "Not all agents statistics received."

    static member toString (error: AppError) =
        match error with
        | Domain e        -> AppError.toString(e)
        | Validation e    -> AppError.toString(e)
        | Communication e -> AppError.toString(e)