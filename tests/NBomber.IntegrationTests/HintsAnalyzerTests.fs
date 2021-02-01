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
    StepName = "step"
    Ok = {
        Request = { Count = 0; RPS = 0.0 }
        Latency = { MinMs = 0.0; MeanMs = 0.0; MaxMs = 0.0
                    Percent50 = 0.0; Percent75 = 0.0; Percent95 = 0.0; Percent99 = 0.0; StdDev = 0.0
                    LatencyCount = { Less800 = 0; More800Less1200 = 0; More1200 = 0 } }
        DataTransfer = { MinKb = 0.0; MeanKb = 0.0; MaxKb = 0.0
                         Percent50 = 0.0; Percent75 = 0.0; Percent95 = 0.0; Percent99 = 0.0; StdDev = 0.0; AllMB = 0.0 }
    }
    Fail = {
        Request = { Count = 0; RPS = 0.0 }
        Latency = { MinMs = 0.0; MeanMs = 0.0; MaxMs = 0.0
                    Percent50 = 0.0; Percent75 = 0.0; Percent95 = 0.0; Percent99 = 0.0; StdDev = 0.0
                    LatencyCount = { Less800 = 0; More800Less1200 = 0; More1200 = 0 } }
        DataTransfer = { MinKb = 0.0; MeanKb = 0.0; MaxKb = 0.0
                         Percent50 = 0.0; Percent75 = 0.0; Percent95 = 0.0; Percent99 = 0.0; StdDev = 0.0; AllMB = 0.0 }
        ErrorStats = Array.empty
    }
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

    let req = { baseStepStats.Ok.Request with RPS = float rps }
    let dt = { baseStepStats.Ok.DataTransfer with AllMB = 1.0 }
    let stepStats = { baseStepStats with Ok = { Request = req; Latency = baseStepStats.Ok.Latency; DataTransfer = dt } }

    let scnStats = { baseScnStats with StepStats = [| stepStats |] }
    let nodeStats = { baseNodeStats with ScenarioStats = [| scnStats |] }

    match HintsAnalyzer.analyze nodeStats with
    | hint::tail when rps = 0u -> ()
    | [] when rps > 0u         -> ()
    | _                        -> failwith "analyzer finished with error"

[<Property>]
let ``analyze should return hint for case when AllDataMB = 0`` (allDataMB: uint32) =

    let req = { baseStepStats.Ok.Request with RPS = 1.0 }
    let dt = { baseStepStats.Ok.DataTransfer with AllMB = float allDataMB }
    let stepStats = { baseStepStats with Ok = { Request = req; Latency = baseStepStats.Ok.Latency; DataTransfer = dt } }
    let scnStats = { baseScnStats with StepStats = [| stepStats |] }
    let nodeStats = { baseNodeStats with ScenarioStats = [| scnStats |] }

    match HintsAnalyzer.analyze nodeStats with
    | hint::tail when allDataMB = 0u -> ()
    | [] when allDataMB > 0u         -> ()
    | e                              -> failwith "analyzer finished with error"
