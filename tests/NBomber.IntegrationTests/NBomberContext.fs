module Tests.NBomberContextTests

open System

open Xunit

open NBomber.Configuration
open NBomber.Contracts
open NBomber.DomainServices

let globalSettings = { 
    ScenariosSettings = List.empty
    TargetScenarios = Some ["1"]
    ReportFileName = None
    ReportFormats = None
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

let config = {
    GlobalSettings = None
    ClusterSettings = None
    LogSettings = None
}

let context = {
    Scenarios = [| scenario |]
    NBomberConfig = None
    ReportFileName = None
    ReportFormats = []
    StatisticsSink = None
}

[<Fact>]
let ``NBomberContext.getTargetScenarios should return all registered scenarios if TargetScenarios are empty`` () =    
    let glSettings = { globalSettings with TargetScenarios = None }
    let config = { config with GlobalSettings = Some glSettings }
    let context = { context with NBomberConfig = Some config }
       
    match NBomberContext.getTargetScenarios(context) with
    | scenarios when scenarios.Length = 1 -> ()
    | _ -> failwith ""

[<Fact>]
let ``NBomberContext.getTargetScenarios should return only target scenarios if TargetScenarios are not empty`` () =    
    let glSettings = { globalSettings with TargetScenarios = Some ["10"] }
    let config = { config with GlobalSettings = Some glSettings }
    
    let scn1 = { scenario with ScenarioName = "1" }
    let scn2 = { scenario with ScenarioName = "2" }    
    
    let context = { context with NBomberConfig = Some config
                                 Scenarios = [| scn1; scn2 |] }
       
    match NBomberContext.getTargetScenarios(context) with
    | scenarios when scenarios.Length = 1 && scenarios.[0] = "10" -> ()
    | _ -> failwith ""