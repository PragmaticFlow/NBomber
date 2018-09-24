module internal NBomber.DomainServices.TestRunner

open System
open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.Errors
open NBomber.Domain.StatisticsTypes

type TestFramework =
    | Xunit of Type
    | Nunit of Type

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

let run (assertions: IAssertion[], scnStats: ScenarioStats) =

    let printErrors (errors: DomainError[], framework: TestFramework) =
        let errorsStr = errors 
                        |> Array.map(Errors.toString)
                        |> String.concat(Environment.NewLine)

        match framework with
        | Xunit xType -> 
            let m = xType.GetMethod("True", [|typeof<bool>; typeof<string>|])
            let func = Delegate.CreateDelegate(typeof<Action<bool,string>>, m) :?> (Action<bool,string>)
            func.Invoke(false, errorsStr)            

        | Nunit nType -> 
            let m = nType.GetMethod("Fail", [|typeof<string>|])
            let func = Delegate.CreateDelegate(typeof<Action<string>>, m) :?> (Action<string>)
            func.Invoke(errorsStr)

    let framework = tryGetCurrentFramework()
    if framework.IsNone then failwith("Unknown framework")

    let errors = 
        Assertion.create(assertions)
        |> Assertion.run(scnStats)
        |> Array.filter(Result.isError)
        |> Array.map(Result.getError)

    if errors.Length > 0 then printErrors(errors, framework.Value)