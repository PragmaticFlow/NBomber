module Tests.Configuration.ValidationTests

open System
open System.Threading.Tasks

open Xunit

open NBomber.FSharp
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Errors
open NBomber.DomainServices.Validation

let globalSettings = { 
    ScenariosSettings = None
    TargetScenarios = Some ["1"]
    ReportFileName = None
    ReportFormats = None
}

let scenarioSettings = { 
    ScenarioName = "1"
    ConcurrentCopies = 1    
    WarmUpDuration = DateTime.MinValue.AddSeconds(10.)
    Duration = DateTime.MinValue.AddSeconds(10.) 
}

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
let ``GlobalSettingsValidation.checkEmptyTarget should return fail if TargetScenarios has empty value`` () =    
    let glSettings = { globalSettings with TargetScenarios = Some [" "] }
       
    match GlobalSettingsValidation.checkEmptyTarget(glSettings) with
    | Error (TargetScenarioIsEmpty _) -> ()
    | _ -> failwith ""

[<Fact>]
let ``GlobalSettingsValidation.checkEmptyTarget should return ok if TargetScenarios is not specified`` () =    
    let glSettings = { globalSettings with TargetScenarios = None }
       
    match GlobalSettingsValidation.checkEmptyTarget(glSettings) with
    | Ok _ -> ()
    | _    -> failwith ""
    
[<Fact>]
let ``GlobalSettingsValidation.checkAvailableTarget should return fail if TargetScenarios has values which doesn't exist in registered scenarios`` () =    
    let scenarios = [| scenario |]    
    let glSettings = { globalSettings with TargetScenarios = Some ["3"] }    
        
    match GlobalSettingsValidation.checkAvailableTarget scenarios glSettings with
    | Error (TargetScenariosNotFound _) -> ()
    | _ -> failwith ""

[<Fact>]
let ``GlobalSettingsValidation.checkDuration should return fail if Duration < 1 sec`` () =
    let scnSettings = { scenarioSettings with Duration = DateTime.MinValue.AddSeconds(0.5) }
    let glSettings = { globalSettings with ScenariosSettings = Some [scnSettings] }
    
    match GlobalSettingsValidation.checkDuration(glSettings) with
    | Error (DurationIsWrong _) -> ()
    | _ -> failwith ""
    
[<Fact>]
let ``GlobalSettingsValidation.checkConcurrentCopies should return fail if ConcurrentCopies < 1`` () =
    let scnSettings = { scenarioSettings with ConcurrentCopies = 0 }
    let glSettings = { globalSettings with ScenariosSettings = Some [scnSettings] }
    
    match GlobalSettingsValidation.checkConcurrentCopies(glSettings) with
    | Error (ConcurrentCopiesIsWrong _) -> ()
    | _ -> failwith ""

[<Fact>]
let ``GlobalSettingsValidation.checkEmptyReportName should return fail if ReportFileName is empty`` () =    
    let glSettings = { globalSettings with ReportFileName = Some " " }
    
    match GlobalSettingsValidation.checkEmptyReportName(glSettings) with
    | Error (EmptyReportName _) -> ()
    | _ -> failwith ""

[<Fact>]
let ``ScenarioValidation.checkEmptyName should return fail if scenario has empty name`` () =    
    let scn = { scenario with ScenarioName = " " } |> Array.singleton    
    
    match ScenarioValidation.checkEmptyName(scn) with
    | Error EmptyScenarioName -> ()
    | _ -> failwith ""

[<Fact>]
let ``ScenarioValidation.checkDuplicateName should return fail if scenario has duplicate name`` () =    
    let scn1 = { scenario with ScenarioName = "1" }
    let scn2 = { scenario with ScenarioName = "1" }    
    
    match ScenarioValidation.checkDuplicateName([|scn1; scn2|]) with
    | Error (DuplicateScenarioName _) -> ()
    | _ -> failwith ""

[<Fact>]
let ``ScenarioValidation.checkEmptyStepName should return fail if scenario has empty step name`` () =    
    let step = NBomber.FSharp.Step.create(" ", fun _ -> Task.FromResult(Response.Ok()))
    let scn = { scenario with Steps = [|step|] }    
    
    match ScenarioValidation.checkEmptyStepName([|scn|]) with
    | Error (EmptyStepName _) -> ()
    | _ -> failwith ""
    
//[<Fact>]
//let ``ScenarioValidation.checkDuplicateStepName should return fail if scenario has duplicate step name`` () =    
//    let step = NBomber.FSharp.Step.create("step_1", fun _ -> Task.FromResult(Response.Ok()))
//    let scn = { scenario with Steps = [|step; step|] }            
//    
//    match ScenarioValidation.checkDuplicateStepName([|scn|]) with
//    | Error (DuplicateStepName _) -> ()
//    | _ -> failwith ""

[<Fact>]
let ``ScenarioValidation.checkDuration should return fail if Duration < 1 sec`` () =        
    let scn = { scenario with Duration = TimeSpan.FromSeconds(0.5) }        
    
    match ScenarioValidation.checkDuration([|scn|]) with
    | Error (DurationIsWrong _) -> ()
    | _ -> failwith ""

[<Fact>]
let ``ScenarioValidation.checkConcurrentCopies should return fail if ConcurrentCopies < 1`` () =        
    let scn = { scenario with ConcurrentCopies = 0 }        
    
    match ScenarioValidation.checkConcurrentCopies([|scn|]) with
    | Error (ConcurrentCopiesIsWrong _) -> ()
    | _ -> failwith ""

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