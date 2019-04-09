namespace NBomber.Domain

open System
open NBomber.Contracts

type internal StepError = {
    ScenarioName: string
    StepName: string
    Error: string
}

type internal DomainError = 
    | HttpError of exn

    | InitScenarioError  of ex:exn    
    | CleanScenarioError of ex:exn    
    | AssertNotFound of assertNumber:int * assertion:Assertion
    | AssertionError of assertNumber:int * assertion:Assertion * stats:Statistics    

    // GlobalSettingsValidation errors
    | TargetScenarioIsEmpty
    | TargetScenarioNotFound  of notFoundScenarios:string[] * availableScenarios:string[]
    | DurationIsWrong         of scenarioNames:string[]
    | ConcurrentCopiesIsWrong of scenarioNames:string[]
    | EmptyReportName
    
    // ScenarioValidation errors
    | EmptyScenarioName
    | DuplicateScenarioName of scenarioNames:string[]
    | EmptyStepName         of scenarioNames:string[]
    | DuplicateStepName     of stepNames:string[]    

    // Cluster Coordinator errors
    | StartNewSessionError of agentErrors:DomainError[]        
    | StartWarmUpError     of agentErrors:DomainError[]    
    | StartBombingError    of agentErrors:DomainError[]    
    | GetStatisticsError   of agentErrors:DomainError[]

    // Cluster Agent errors 
    | AgentErrors        of errors:DomainError[]
    | AgentIsWorking    

    | OperationFailed of operationName:string * errors:DomainError[]

module internal Errors =

    let rec toString (error) =
        match error with    
        | InitScenarioError ex -> String.Format("init scenario error:'{0}'", ex.ToString())    
        | CleanScenarioError ex -> String.Format("clean scenario error:'{0}'", ex.ToString())    
    
        | AssertNotFound (assertNum,assertion) -> 
            match assertion with
            | Step s -> String.Format("Assertion #'{0}' is not found for step: '{1}' in scenario: '{2}'", assertNum, s.StepName, s.ScenarioName)        

        | AssertionError (assertNum,assertion,stats) ->
            let statsJson = sprintf "%A" stats        
            match assertion with
            | Step s -> let scenarioStr = String.Format("SCENARIO: '{0}' {1}", s.ScenarioName, Environment.NewLine)
                        let stepStr     = String.Format("STEP: '{0}' {1}", s.StepName, Environment.NewLine)                    
                        let labelStr = if s.Label.IsSome
                                       then String.Format("LABEL: '{0}' {1}", s.Label.Value, Environment.NewLine)
                                       else String.Empty 
                        let statsStr = String.Format("STATS: {0} {1} {2}", Environment.NewLine, statsJson, Environment.NewLine)
                        String.Format("Assertion #'{0}' FAILED for: {1} {2} {3} {4} {5}", assertNum, Environment.NewLine, scenarioStr, stepStr, labelStr, statsStr)

        | TargetScenarioNotFound (notFoundScenarios,availableScenarios) ->
            notFoundScenarios
            |> String.concatWithCommaAndQuotes
            |> sprintf "Target scenarios %s is not found. Available scenarios are %s" <| String.concatWithCommaAndQuotes(availableScenarios)
    
        | DurationIsWrong scenarioNames ->
            scenarioNames |> String.concatWithCommaAndQuotes |> sprintf "Duration for scenarios %s can not be less than 1 sec."

        | ConcurrentCopiesIsWrong scenarioNames ->
            scenarioNames |> String.concatWithCommaAndQuotes |> sprintf "Concurrent copies for scenarios %s can not be less than 1."

        | EmptyScenarioName -> "Scenario name can not be empty."
        | EmptyReportName -> "Report File Name can not be empty string."        

        //| DuplicateScenarios -> "Scenario names should be unique."
        //| DuplicateSteps scenarioNames ->
        //    scenarioNames |> String.concatWithCommaAndQuotes |> sprintf "Step names are not unique in scenarios: %s. Step names should be unique within scenario."

        //| EmptyStepName scenarioNames->
        //    scenarioNames |> String.concatWithCommaAndQuotes |> sprintf "Step names are empty in scenarios: %s. Step names should not be empty within scenario."
            
        | _ -> "undefined error"

    let getErrorsString (results: Result<_,DomainError>[]) =
        results 
        |> Array.filter(Result.isError)
        |> Array.map(Result.getError >> toString)    
        |> String.concat(Environment.NewLine)