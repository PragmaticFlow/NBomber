module internal NBomber.Domain.Errors

open System

type FlowError = {
    FlowName: string
    StepName: string
    Error: string
}

type DomainError =
    | InitStepError  of msg:string
    | WarmUpError    of errors:FlowError[]
    | AssertNotFound of scope:string * scopeName:string
    | AssertionError of scope:string * scopeName:string * assertNumber:int

let toString (error) =
    match error with
    | InitStepError msg -> msg
    
    | WarmUpError errors -> 
        let errorsStr = errors |> Array.map(fun error -> error.Error) |> String.concat "; "
        "warm up has failed:" + Environment.NewLine + errorsStr
                            
    | AssertNotFound (scope,scopeName) -> String.Format("Assertion is not found for {0} '{1}'", scope, scopeName)
    | AssertionError (scope,scopeName,assertNumber) -> String.Format("Assertion #{0} FAILED for {1} '{2}'", assertNumber, scope, scopeName)