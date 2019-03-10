module internal NBomber.DomainServices.TestFrameworkRunner

open System

open NBomber.Domain
open NBomber.Domain.Errors

type TestFramework =
    | Xunit of Type
    | Nunit of Type

let private printErrorMessage errorsMessage framework =
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
    [ "Xunit.Assert, xunit", Xunit
      "Xunit.Assert, xunit.assert", Xunit
      "NUnit.Framework.Assert, nunit.framework", Nunit
    ] |> List.tryPick( fun (typeName, ctor) ->
        Type.GetType typeName
        |> Option.ofObj
        |> Option.map ctor
    )

let showErrors (errors: DomainError[]) =
    let errorMsg =
        errors
        |> Array.map Errors.toString
        |> String.concat Environment.NewLine
    if errorMsg |> String.IsNullOrWhiteSpace |> not then
        match tryGetCurrentFramework() with
        | None -> failwith "Unknown test framework"
        | Some framework -> printErrorMessage errorMsg framework
