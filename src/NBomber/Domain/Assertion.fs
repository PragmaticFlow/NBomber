[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Assertion

open NBomber.Contracts
open NBomber.Errors

let create (assertions:IAssertion[]) = 
    assertions
    |> Array.map(fun x ->
        match x :?> Assertion with
        | Step s -> s)

let apply (stepsStats: Statistics[]) (assertions: StepAssertion[]) =
    let errors = 
        assertions
        |> Array.mapi(fun i asrt ->
            let stats = stepsStats |> Array.find(fun x -> x.ScenarioName = asrt.ScenarioName && x.StepName = asrt.StepName)
            if asrt.AssertFunc(stats) then
                None
            else
                Some <| AssertionError(i + 1, Step(asrt), stats))
        |> Array.filter(Option.isSome)
    
    if Array.isEmpty(errors) then Array.empty
    else errors |> Array.map(Option.get)
    