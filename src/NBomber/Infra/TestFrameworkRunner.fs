module internal NBomber.Infra.TestFrameworkRunner

open System
open NBomber.Errors

type TestFramework =
    | Xunit of Type
    | Nunit of Type
    | Expecto of Type

let private printErrorMessage (framework: TestFramework) (errorsMessage: string) =
    match framework with
    | Xunit xType ->
        let m = xType.GetMethod("True", [|typeof<bool>; typeof<string>|])
        let func = Delegate.CreateDelegate(typeof<Action<bool,string>>, m) :?> (Action<bool,string>)
        func.Invoke(false, errorsMessage)

    | Nunit nType ->
        let m = nType.GetMethod("Fail", [|typeof<string>|])
        let func = Delegate.CreateDelegate(typeof<Action<string>>, m) :?> (Action<string>)
        func.Invoke(errorsMessage)

    | Expecto assertException ->
        let ctor = assertException.GetConstructor [|typeof<string>|]
        ctor.Invoke [| errorsMessage|] :?> exn |> raise

let tryGetCurrentFramework () =
    ["Xunit.Assert, xunit", Xunit
     "Xunit.Assert, xunit.assert", Xunit
     "NUnit.Framework.Assert, nunit.framework", Nunit
     "Expecto.AssertException, expecto", Expecto
    ] |> List.tryPick(fun (typeName, framework) ->
        typeName |> Type.GetType |> Option.ofObj |> Option.map(framework))

let showErrors (error: AppError[]) =
    match tryGetCurrentFramework() with
    | None -> failwith "No supported test framework found. Supported are Xunit, NUnit, Expecto"
    | Some framework ->
        error
        |> Array.map(AppError.toString)
        |> String.concat(", ")
        |> printErrorMessage(framework)
