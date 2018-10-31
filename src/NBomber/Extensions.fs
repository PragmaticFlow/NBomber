[<AutoOpen>]
module internal Extensions

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

module String =

    let replace (oldValue: string, newValue: string) (str: string) =
        str.Replace(oldValue, newValue)
        

