module Tests.Configuration.AssertionValidation

open System
open System.Threading.Tasks

open Xunit

open NBomber.FSharp
open NBomber.Contracts
open NBomber.Errors
open NBomber.DomainServices.Validation

let scenario = {
    ScenarioName = "1"
    TestInit = None
    TestClean = None
    Steps = Array.empty
    Assertions = Array.empty
    ConcurrentCopies = 1    
    WarmUpDuration = TimeSpan.FromSeconds(10.)
    Duration = TimeSpan.FromSeconds(10.)
}

[<Fact>]
let ``AssertionValidation.checkInvalidAsserts should return fail if Assertions were created for non-exist steps`` () =
    let asrt = Assertion.forStep("undefined_step_name", fun _ -> true)
    let step = NBomber.FSharp.Step.create("step_1", fun _ -> Task.FromResult(Response.Ok()))
    
    let scn = { scenario with Steps = [|step|] }
              |> Scenario.withAssertions([asrt])

    match AssertionValidation.checkInvalidAsserts([|scn|]) with
    | Error (AssertsNotFound _) -> ()
    | _ -> failwith ""

[<Fact>]
let ``AssertionValidation.checkInvalidAsserts should Not return error if Assertions were created for exist steps`` () =
    let step = NBomber.FSharp.Step.create("step_1", fun _ -> Task.FromResult(Response.Ok()))
    let asrt = Assertion.forStep("step_1", fun _ -> true)
    
    let scn = { scenario with Steps = [|step|] }
              |> Scenario.withAssertions([asrt])
    
    match AssertionValidation.checkInvalidAsserts([|scn|]) with
    | Error _ -> failwith ""
    | _       -> ()