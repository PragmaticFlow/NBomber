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
    | AssertNotFound of assertNumber:int * assertion:Assertion
    | AssertionError of assertNumber:int * assertion:Assertion * stats:AssertStats

let toString (error) =
    match error with    
    | InitScenarioError ex -> String.Format("init scenario error:'{0}'", ex.ToString())    
    
    | AssertNotFound (assertNum,assertion) -> 
        match assertion with
        | Step s -> String.Format("Assertion #'{0}' is not found for step: '{1}' in scenario: '{2}'", assertNum, s.StepName, s.ScenarioName)        

    | AssertionError (assertNum,assertion,stats) ->
        let statsStr = sprintf "%A" stats
        match assertion with
        | Step s -> String.Format("Assertion #'{0}' FAILED for step: '{1}' in scenario: '{2}', stats: '{3}'", assertNum, s.StepName, s.ScenarioName, statsStr)

let getErrorsString (results: Result<_,DomainError>[]) =
    results 
    |> Array.filter(Result.isError)
    |> Array.map(Result.getError >> toString)    
    |> String.concat(Environment.NewLine)