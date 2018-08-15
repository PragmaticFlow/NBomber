module internal rec NBomber.AssertIntegration

open System

type TestFramework =
    | Xunit of Type
    | Nunit of Type

let check (results: string[]) =
    match results with
    | [||] -> ()
    | errors ->
        try
            testFailed(errors)
        with 
        | e -> raise e

let private testFailed =    
    let outputFailedAssertions (output: string->unit) (messages: string[]) = 
        let errorMessage = messages |> String.concat Environment.NewLine 
        (Environment.NewLine + errorMessage + Environment.NewLine) |> output

    let framework = 
        let ty = Type.GetType("Xunit.Assert, xunit") //xunit v1
        if not (isNull ty) then Xunit ty |> Ok else
        
        let ty = Type.GetType("Xunit.Assert, xunit.assert") //xunit v2
        if not (isNull ty) then Xunit ty |> Ok else

        let ty = Type.GetType("NUnit.Framework.Assert, nunit.framework")
        if not (isNull ty) then Nunit ty |> Ok else

        Error("Unknown framework")

    match framework with
    | Ok(Xunit ty) -> 
        let mi = ty.GetMethod("True", [|typeof<bool>; typeof<string>|])
        let func = Delegate.CreateDelegate(typeof<Action<bool,string>>, mi) :?> (Action<bool,string>)
        outputFailedAssertions(fun msg -> func.Invoke(false,msg))

    | Ok(Nunit ty) -> 
        let mi = ty.GetMethod("Fail", [|typeof<string>|])
        let func = Delegate.CreateDelegate(typeof<Action<string>>, mi) :?> (Action<string>)
        outputFailedAssertions(fun msg -> func.Invoke(msg))

    | Error message -> raise (Exception("Test failed: " + message)) 