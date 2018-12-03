module internal NBomber.Domain.Errors

open System
open NBomber.Domain.DomainTypes

type StepError = {
    ScenarioName: string
    StepName: string
    Error: string
}

type DomainError =
    | InitStepError  of msg:string
    | WarmUpError    of error:StepError
    | AssertNotFound of assertNumber:int * assertion:Assertion
    | AssertionError of assertNumber:int * assertion:Assertion

let toString (error) =
    match error with
    | InitStepError msg -> msg    
    | WarmUpError error -> String.Format("warm up has failed:'{0}'", error)    

    | AssertNotFound (assertNum,assertion) -> 
        match assertion with
        | Step s -> String.Format("Assertion #'{0}' is not found for step: '{1}' in scenario: '{2}'", assertNum, s.StepName, s.ScenarioName)        

    | AssertionError (assertNum,assertion) -> 
        match assertion with
        | Step s -> String.Format("Assertion #'{0}' FAILED for step: '{1}' in scenario: '{2}'", assertNum, s.StepName, s.ScenarioName)        

let getErrorsString (results: Result<_,DomainError>[]) =
    results 
    |> Array.filter(Result.isError)
    |> Array.map(Result.getError)
    |> Array.map(toString)
    |> String.concat(Environment.NewLine)