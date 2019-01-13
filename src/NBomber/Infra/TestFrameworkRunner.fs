module internal NBomber.DomainServices.TestFrameworkRunner

open System

open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Errors

type TestFramework =
    | Xunit of Type
    | Nunit of Type

let private printErrorMessage (errorsMessage: string, framework: TestFramework) =
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

let showAssertionErrors (results: Result<Assertion,DomainError>[]) =
    let framework = tryGetCurrentFramework()
    if framework.IsNone then failwith("Unknown framework")

    let errorMsg = 
        results
        |> Array.filter(Result.isError)
        |> Array.map(Result.getError)
        |> Array.map(Errors.toString)
        |> String.concat(Environment.NewLine)

    if not <| String.IsNullOrEmpty(errorMsg) then printErrorMessage(errorMsg, framework.Value)

let showValidationErrors (errorMsg: string) =
    let framework = tryGetCurrentFramework()
    if framework.IsNone then failwith("Unknown framework")
    if not <| String.IsNullOrEmpty(errorMsg) then printErrorMessage(errorMsg, framework.Value)