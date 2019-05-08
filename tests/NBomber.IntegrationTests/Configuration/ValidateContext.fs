module Tests.Configuration.ValidateSettings

open System
open System.Threading.Tasks

open Xunit

open NBomber.Extensions
open NBomber.FSharp
open NBomber.Configuration
open NBomber.Contracts
open NBomber.Errors
open NBomber.DomainServices.Validation

let globalSettings = {
    ScenariosSettings = List.empty
    TargetScenarios = ["1"]
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
    let glSettings = { globalSettings with TargetScenarios = [" "] }

    let error = GlobalSettingsValidation.checkEmptyTarget(glSettings) |> Result.getError
    match error with
    | TargetScenarioIsEmpty _ -> ()
    | _ -> failwith ""

[<Fact>]
let ``GlobalSettingsValidation.checkAvailableTarget should return fail if TargetScenarios has values which doesn't exist in registered scenarios`` () =
    let scenarios = [| scenario |]
    let glSettings = { globalSettings with TargetScenarios = ["3"] }

    let error = GlobalSettingsValidation.checkAvailableTarget scenarios glSettings |> Result.getError
    match error with
    | TargetScenarioNotFound _ -> ()
    | _ -> failwith ""

[<Fact>]
let ``GlobalSettingsValidation.checkDuration should return fail if Duration < 1 sec`` () =
    let scnSettings = { scenarioSettings with Duration = DateTime.MinValue.AddSeconds(0.5) }
    let glSettings = { globalSettings with ScenariosSettings = [scnSettings] }

    let error = GlobalSettingsValidation.checkDuration(glSettings) |> Result.getError
    match error with
    | DurationIsWrong _ -> ()
    | _ -> failwith ""

[<Fact>]
let ``GlobalSettingsValidation.checkConcurrentCopies should return fail if ConcurrentCopies < 1`` () =
    let scnSettings = { scenarioSettings with ConcurrentCopies = 0 }
    let glSettings = { globalSettings with ScenariosSettings = [scnSettings] }

    let error = GlobalSettingsValidation.checkConcurrentCopies(glSettings) |> Result.getError
    match error with
    | ConcurrentCopiesIsWrong _ -> ()
    | _ -> failwith ""

[<Fact>]
let ``GlobalSettingsValidation.checkEmptyReportName should return fail if ReportFileName is empty`` () =
    let glSettings = { globalSettings with ReportFileName = Some " " }

    let error = GlobalSettingsValidation.checkEmptyReportName(glSettings) |> Result.getError
    match error with
    | EmptyReportName _ -> ()
    | _ -> failwith ""

[<Fact>]
let ``ScenarioValidation.checkEmptyName should return fail if scenario has empty name`` () =
    let scn = { scenario with ScenarioName = " " } |> Array.singleton

    let error = ScenarioValidation.checkEmptyName(scn) |> Result.getError
    match error with
    | EmptyScenarioName -> ()
    | _ -> failwith ""

[<Fact>]
let ``ScenarioValidation.checkDuplicateName should return fail if scenario has duplicate name`` () =
    let scn1 = { scenario with ScenarioName = "1" }
    let scn2 = { scenario with ScenarioName = "1" }

    let error = ScenarioValidation.checkDuplicateName([|scn1; scn2|]) |> Result.getError
    match error with
    | DuplicateScenarioName _ -> ()
    | _ -> failwith ""

[<Fact>]
let ``ScenarioValidation.checkEmptyStepName should return fail if scenario has empty step name`` () =
    let step = NBomber.FSharp.Step.create(" ", ConnectionPool.none, fun _ -> Task.FromResult(Response.Ok()))
    let scn = { scenario with Steps = [|step|] }

    let error = ScenarioValidation.checkEmptyStepName([|scn|]) |> Result.getError
    match error with
    | EmptyStepName _ -> ()
    | _ -> failwith ""

[<Fact>]
let ``ScenarioValidation.checkDuplicateStepName should return fail if scenario has duplicate step name`` () =
    let step = NBomber.FSharp.Step.create("step_1", ConnectionPool.none, fun _ -> Task.FromResult(Response.Ok()))
    let scn = { scenario with Steps = [|step; step|] }

    let error = ScenarioValidation.checkDuplicateStepName([|scn|]) |> Result.getError
    match error with
    | DuplicateStepName _ -> ()
    | _ -> failwith ""

[<Fact>]
let ``ScenarioValidation.checkDuration should return fail if Duration < 1 sec`` () =
    let scn = { scenario with Duration = TimeSpan.FromSeconds(0.5) }

    let error = ScenarioValidation.checkDuration([|scn|]) |> Result.getError
    match error with
    | DurationIsWrong _ -> ()
    | _ -> failwith ""

[<Fact>]
let ``ScenarioValidation.checkConcurrentCopies should return fail if ConcurrentCopies < 1`` () =
    let scn = { scenario with ConcurrentCopies = 0 }

    let error = ScenarioValidation.checkConcurrentCopies([|scn|]) |> Result.getError
    match error with
    | ConcurrentCopiesIsWrong _ -> ()
    | _ -> failwith ""
