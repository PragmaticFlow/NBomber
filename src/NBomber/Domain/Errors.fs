module internal NBomber.Domain.Errors

open System
open NBomber.Contracts
open NBomber.Domain.DomainTypes

type StepError = {
    ScenarioName: string
    StepName: string
    Error: string
}

type DomainError =    
    | InitScenarioError  of ex:exn    
    | CleanScenarioError of ex:exn    
    | AssertNotFound of assertNumber:int * assertion:Assertion
    | AssertionError of assertNumber:int * assertion:Assertion * stats:AssertStats

    // Validation errors
    | DurationLessThanOneSecond of scenarioNames:string[]
    | ConcurrentCopiesLessThanOne of scenarioNames:string[]
    | EmptyReportFileName
    | UnsupportedReportFormat of reportFormats:string[]
    | DuplicateScenarios
    | DuplicateSteps of scenarioNames:string[]
    | EmptyScenarioName
    | EmptyStepName of scenarioNames:string[]
    | ScenariosNotFound of notFoundScenarios:string[] * availableScenarios:string[]

let toString (error) =
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

    | ScenariosNotFound (notFoundScenarios,availableScenarios) ->
        notFoundScenarios
        |> String.concatWithCommaAndQuotes
        |> sprintf "Target scenarios %s is not found. Available scenarios are %s" <| String.concatWithCommaAndQuotes(availableScenarios)
    
    | DurationLessThanOneSecond scenarioNames ->
        scenarioNames |> String.concatWithCommaAndQuotes |> sprintf "Duration for scenarios %s can not be less than 1 sec."

    | ConcurrentCopiesLessThanOne scenarioNames ->
        scenarioNames |> String.concatWithCommaAndQuotes |> sprintf "Concurrent copies for scenarios %s can not be less than 1."

    | EmptyScenarioName -> "Scenario name can not be empty."
    | EmptyReportFileName -> "Report File Name can not be empty string."
    | UnsupportedReportFormat reportFormats ->
        reportFormats |> String.concatWithCommaAndQuotes |> sprintf "Unknown Report Formats %s. Allowed formats: Txt, Html or Csv."

    | DuplicateScenarios -> "Scenario names should be unique."
    | DuplicateSteps scenarioNames ->
        scenarioNames |> String.concatWithCommaAndQuotes |> sprintf "Step names are not unique in scenarios: %s. Step names should be unique within scenario."

    | EmptyStepName scenarioNames->
        scenarioNames |> String.concatWithCommaAndQuotes |> sprintf "Step names are empty in scenarios: %s. Step names should not be empty within scenario."

let toHtmlString (error) =
    match error with
    | AssertNotFound (_,assertion) -> 
        match assertion with
        | Step s ->
            let labelStr = if s.Label.IsSome then s.Label.Value else String.Empty
            sprintf "<li class=\"list-group-item list-group-item-danger\">Assertion <strong>'%s'</strong> is not found for step <strong>'%s'</strong></li>" labelStr s.StepName
    | AssertionError (_,assertion,_) ->
        match assertion with
        | Step s ->
            let labelStr = if s.Label.IsSome then s.Label.Value else String.Empty
            sprintf "<li class=\"list-group-item list-group-item-danger\">Failed assertion <strong>'%s'</strong> for step <strong>'%s'</strong></li>" labelStr s.StepName
    | _ -> String.Empty
 
let getErrorsString (results: Result<_,DomainError>[]) =
    results 
    |> Array.filter(Result.isError)
    |> Array.map(Result.getError >> toString)    
    |> String.concat(Environment.NewLine)