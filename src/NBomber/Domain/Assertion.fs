[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Assertion

open NBomber.Contracts
open NBomber.Errors

let create : IAssertion[] -> Assertion[] = 
    Array.map(fun x -> x :?> Assertion)

let matchStepAssertions :IAssertion[] -> StepAssertion[] =
    create
    >> Array.map(fun x ->
        match x with
        | Step s -> s)

let apply (stepsStats: Statistics[]) (assertions: Assertion[]) =
    let errors = 
        assertions
        |> Array.mapi(fun i assertion ->
            let asrtNum = i + 1
            match assertion with
            | Step asrt -> 
                let stats = stepsStats |> Array.find(fun x -> x.ScenarioName = asrt.ScenarioName && x.StepName = asrt.StepName)
                if asrt.AssertFunc(stats) then
                    None
                else
                    Some <| AssertionError(asrtNum, Step(asrt), stats))
        |> Array.filter(Option.isSome)
    
    if Array.isEmpty(errors) then Array.empty
    else errors |> Array.map(Option.get)
    