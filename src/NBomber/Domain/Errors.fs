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

let getErrorsString (results: Result<_,DomainError>[]) =
    results 
    |> Array.filter(Result.isError)
    |> Array.map(Result.getError >> toString)    
    |> String.concat(Environment.NewLine)