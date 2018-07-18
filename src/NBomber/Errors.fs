module internal rec NBomber.Errors

type FlowError = {
    FlowName: string
    StepName: string
    Error: string
}

type DomainError =
    | InitStepError of msg:string
    | FlowErrors    of errors:FlowError[]

let printError (error) =
    match error with
    | InitStepError msg -> msg
    | FlowErrors errors -> ""


type Result<'T,'TError> with    
    static member isOk(result) = 
        match result with
        | Ok _    -> true
        | Error _ -> false
    
    static member isError(result) = not(Result.isOk(result))
    
    static member getError(result) = 
        match result with
        | Ok _     -> failwith "result is not error"
        | Error er -> er