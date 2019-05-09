﻿[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Assertion

open NBomber.Contracts
open NBomber.Errors

let create (assertions: IAssertion[]) =
    assertions |> Array.map(fun x -> x :?> Assertion)

let apply (stepsStats: Statistics[]) (assertions: Assertion[]) =
        assertions
        |> Seq.indexed
        |> Seq.choose (fun (i, assertion) ->
            let asrtNum = i + 1
            match assertion with
            | Step asrt ->
                let stats = stepsStats |> Array.find(fun x -> x.ScenarioName = asrt.ScenarioName &&
                                                              x.StepName = asrt.StepName)
                if asrt.AssertFunc stats then
                    None
                else
                    AssertionError(asrtNum, Step asrt, stats) |> Some
        )
        |> Seq.toArray
