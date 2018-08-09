module NBomber.Assertions

open System
open System.Runtime.Serialization

[<Serializable>]
type AssertionFailedException =
    inherit exn
    new () = { inherit exn() }
    new (message: string) = { inherit exn(message) }
    new (message: string, innerException: Exception) = { inherit exn(message, innerException) }
    new (info: SerializationInfo, context: StreamingContext) = { inherit exn(info, context) }

type private TestFramework =
    | Xunit of Type
    | Nunit of Type
    | Generic

let private testFailed =
    let outputGenericTestFailed (msg: string) =
        raise (AssertionFailedException("Test failed:" + msg))
        ()
    
    let outputFailedAssertions (output: string->unit) (messages: string[]) = 
        let errorMessage = messages |> String.concat Environment.NewLine 
        sprintf "%s%s%s" Environment.NewLine errorMessage Environment.NewLine |> output

    let framework = 
        let ty = Type.GetType("Xunit.Assert, xunit") //xunit v1
        if ty <> null then Xunit ty else
        
        let ty = Type.GetType("Xunit.Assert, xunit.assert") //xunit v2
        if ty <> null then Xunit ty else

        let ty = Type.GetType("NUnit.Framework.Assert, nunit.framework")
        if ty <> null then Nunit ty else

        Generic

    match framework with
        | Xunit ty -> 
            let mi = ty.GetMethod("True", [|typeof<bool>; typeof<string>|])
            let del = Delegate.CreateDelegate(typeof<Action<bool,string>>, mi) :?> (Action<bool,string>)
            outputFailedAssertions (fun msg -> del.Invoke(false,msg))

        | Nunit ty -> 
            let mi = ty.GetMethod("Fail", [|typeof<string>|])
            let del = Delegate.CreateDelegate(typeof<Action<string>>, mi) :?> (Action<string>)
            outputFailedAssertions (fun msg -> del.Invoke(msg))

        | Generic ->
            outputGenericTestFailed |> outputFailedAssertions

let test (results: string[]) =
    match results with
    | [||] -> ()
    | errors ->
        try
            testFailed errors
        with 
        | e -> raise e