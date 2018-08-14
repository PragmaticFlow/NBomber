module internal rec NBomber.AssertIntegration

open System
open System.Runtime.Serialization

type TestFramework =
    | Xunit of Type
    | Nunit of Type
    | Generic

let test (results: string[]) =
    match results with
    | [||] -> ()
    | errors ->
        try
            testFailed(errors)
        with 
        | e -> raise e

let private testFailed =
    let outputGenericTestFailed (msg: string) =
        raise (Exception("Test failed:" + msg))
        ()
    
    let outputFailedAssertions (output: string->unit) (messages: string[]) = 
        let errorMessage = messages |> String.concat Environment.NewLine 
        (Environment.NewLine + errorMessage + Environment.NewLine) |> output

    let framework = 
        let ty = Type.GetType("Xunit.Assert, xunit") //xunit v1
        if not (isNull ty) then Xunit ty else
        
        let ty = Type.GetType("Xunit.Assert, xunit.assert") //xunit v2
        if not (isNull ty) then Xunit ty else

        let ty = Type.GetType("NUnit.Framework.Assert, nunit.framework")
        if not (isNull ty) then Nunit ty else

        Generic

    match framework with
    | Xunit ty -> 
        let mi = ty.GetMethod("True", [|typeof<bool>; typeof<string>|])
        let func = Delegate.CreateDelegate(typeof<Action<bool,string>>, mi) :?> (Action<bool,string>)
        outputFailedAssertions(fun msg -> func.Invoke(false,msg))

    | Nunit ty -> 
        let mi = ty.GetMethod("Fail", [|typeof<string>|])
        let func = Delegate.CreateDelegate(typeof<Action<string>>, mi) :?> (Action<string>)
        outputFailedAssertions(fun msg -> func.Invoke(msg))

    | Generic ->
        outputGenericTestFailed |> outputFailedAssertions