﻿[<AutoOpen>]
module internal Extensions

type Result<'T,'TError> with    
    static member isOk(result) = 
        match result with
        | Ok _    -> true
        | Error _ -> false

    static member getOk(result) = 
        match result with
        | Ok v    -> v
        | Error _ -> failwith "result is error"
    
    static member isError(result) = not(Result.isOk(result))
    
    static member getError(result) = 
        match result with
        | Ok _     -> failwith "result is not error"
        | Error er -> er

[<Struct>]
type MaybeBuilder =
    
    member x.Bind(m, bind) =
        match m with
        | Some value -> bind value
        | None       -> None

    member x.Return(value) = Some value
    member x.ReturnFrom(value) = value

let maybe = MaybeBuilder()

module String =

    let replace (oldValue: string, newValue: string) (str: string) =
        str.Replace(oldValue, newValue)

    let concatWithCommaAndQuotes (strings: string[]) =
        "'" + (strings |> String.concat("', '")) + "'"
        

