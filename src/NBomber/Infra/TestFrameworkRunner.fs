module internal NBomber.DomainServices.TestFrameworkRunner

open System
open NBomber.Errors

type TestFramework =
    | Xunit of Type
    | Nunit of Type

let private printErrorMessage (framework: TestFramework) (errorsMessage: string) =
    match framework with
    | Xunit xType ->
        let m = xType.GetMethod("True", [|typeof<bool>; typeof<string>|])
        let func = Delegate.CreateDelegate(typeof<Action<bool,string>>, m) :?> (Action<bool,string>)
        func.Invoke(false, errorsMessage)

    | Nunit nType ->
        let m = nType.GetMethod("Fail", [|typeof<string>|])
        let func = Delegate.CreateDelegate(typeof<Action<string>>, m) :?> (Action<string>)
        func.Invoke errorsMessage

let tryGetCurrentFramework () =
    let xUnit1 =
        "Xunit.Assert, xunit"
        |> Type.GetType
        |> Option.ofObj
        |> Option.map Xunit

    let xUnit2 =
        "Xunit.Assert, xunit.assert"
        |> Type.GetType
        |> Option.ofObj
        |> Option.map Xunit

    let nUnit =
        "NUnit.Framework.Assert, nunit.framework"
        |> Type.GetType
        |> Option.ofObj
        |> Option.map Nunit


    [xUnit1; xUnit2; nUnit] |> List.tryPick id

let showErrors (error: AppError[]) =
    match tryGetCurrentFramework() with
    | None -> failwith "Unknown framework"
    | Some framework ->
        error
        |> Array.map AppError.toString
        |> String.concat ", "
        |> printErrorMessage framework
