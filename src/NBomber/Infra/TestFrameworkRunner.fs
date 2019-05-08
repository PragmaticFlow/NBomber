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
        func.Invoke(errorsMessage)

let tryGetCurrentFramework () =
    let xUnit1 =
        let xunitType = Type.GetType("Xunit.Assert, xunit")
        if not (isNull xunitType) then Xunit(xunitType) |> Some
        else None

    let xUnit2 =
        let xunitType = Type.GetType("Xunit.Assert, xunit.assert")
        if not (isNull xunitType) then Xunit(xunitType) |> Some
        else None

    let nUnit =
        let nunitType = Type.GetType("NUnit.Framework.Assert, nunit.framework")
        if not (isNull nunitType) then Nunit(nunitType) |> Some
        else None

    [xUnit1; xUnit2; nUnit]
    |> List.tryFind(Option.isSome)
    |> Option.flatten

let showErrors (error: AppError[]) =
    let framework = tryGetCurrentFramework()
    if framework.IsNone then failwith("Unknown framework")
    error |> Array.map(AppError.toString) |> String.concat(", ") |> printErrorMessage(framework.Value)
