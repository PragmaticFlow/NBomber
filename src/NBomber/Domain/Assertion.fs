module internal NBomber.Domain.Assertion

open NBomber.Contracts
open NBomber.Domain.Errors
open NBomber.Domain.DomainTypes

let create (assertions: IAssertion[]) = 
    assertions |> Array.map(fun x -> x :?> Assertion)

let apply (assertions: Assertion[]) (stepsStats: Statistics[]) =
    assertions
    |> Array.mapi(fun i assertion ->
        let asrtNum = i + 1
        match assertion with
        | Step asrt -> 
            let stepStats = stepsStats |> Array.tryFind(fun x -> x.ScenarioName = asrt.ScenarioName && x.StepName = asrt.StepName)
            match stepStats with
            | Some v -> 
                let result = asrt.AssertFunc(v)
                if result then Ok <| Step(asrt)
                else Error <| AssertionError(asrtNum, Step(asrt), v)    
            | None -> 
                Error <| AssertNotFound(asrtNum, Step(asrt)))