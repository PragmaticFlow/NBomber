module Tests.HintsAnalyzer

open System

open Xunit
open FsCheck
open FsCheck.Xunit
open Swensen.Unquote
open FsToolkit.ErrorHandling

open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Infra.Dependency
open NBomber.DomainServices.Reporting

let baseNodeStats = {
    RequestCount = 0
    OkCount = 0
    FailCount = 0
    AllDataMB = 0.0
    ScenarioStats = Array.empty
    PluginStats = Array.empty
    NodeInfo = NodeInfo.init()
    TestInfo = { SessionId = ""; TestSuite = ""; TestName = "" }
    ReportFiles = Array.empty
}

let baseScnStats = {
    ScenarioName = "scenario"; RequestCount = 0; OkCount = 0; FailCount = 0;
    AllDataMB = 0.0; StepStats = Array.empty; LatencyCount = { Less800 = 0; More800Less1200 = 0; More1200 = 0 }
    LoadSimulationStats = { SimulationName = ""; Value = 0 }
    ErrorStats = Array.empty; Duration = TimeSpan.MinValue
}

let baseStepStats = {
    StepName = "step"; RequestCount = 0; OkCount = 0; FailCount = 0
    Min = 0; Mean = 0; Max = 0; RPS = 0
    Percent50 = 0; Percent75 = 0; Percent95 = 0; Percent99 = 0; StdDev = 0
    MinDataKb = 0.0; MeanDataKb = 0.0; MaxDataKb = 0.0; AllDataMB = 0.0
    ErrorStats = Array.empty
}

[<Property>]
let ``analyze should return hint for case when FailCount > 0`` (failCount: uint32) =

    let scnStats = { baseScnStats with FailCount = int failCount }
    let nodeStats = { baseNodeStats with ScenarioStats = [| scnStats |] }

    match HintsAnalyzer.analyze nodeStats with
    | hint::tail when failCount > 0u -> ()
    | [] when failCount = 0u         -> ()
    | _                              -> failwith "analyzer finished with error"

[<Property>]
let ``analyze should return hint for case when RPS = 0`` (rps: uint32) =

    let stepStats = { baseStepStats with AllDataMB = 1.0; RPS = int rps }
    let scnStats = { baseScnStats with StepStats = [| stepStats |] }
    let nodeStats = { baseNodeStats with ScenarioStats = [| scnStats |] }

    match HintsAnalyzer.analyze nodeStats with
    | hint::tail when rps = 0u -> ()
    | [] when rps > 0u         -> ()
    | _                        -> failwith "analyzer finished with error"

[<Property>]
let ``analyze should return hint for case when AllDataMB = 0`` (allDataMB: uint32) =

    let stepStats = { baseStepStats with RPS = 1; AllDataMB = float allDataMB }
    let scnStats = { baseScnStats with StepStats = [| stepStats |] }
    let nodeStats = { baseNodeStats with ScenarioStats = [| scnStats |] }

    match HintsAnalyzer.analyze nodeStats with
    | hint::tail when allDataMB = 0u -> ()
    | [] when allDataMB > 0u         -> ()
    | e                              -> failwith "analyzer finished with error"
