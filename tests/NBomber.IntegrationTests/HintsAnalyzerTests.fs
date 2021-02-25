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
    Duration = TimeSpan.MinValue
}

let baseScnStats = {
    ScenarioName = "scenario"; RequestCount = 0; OkCount = 0; FailCount = 0;
    AllDataMB = 0.0; StepStats = Array.empty; LatencyCount = { LessOrEq800 = 0; More800Less1200 = 0; MoreOrEq1200 = 0 }
    LoadSimulationStats = { SimulationName = ""; Value = 0 }
    ErrorStats = Array.empty; CurrentOperation = OperationType.None; Duration = TimeSpan.MinValue
}

let baseStepStats = {
    StepName = "step"
    Ok = {
        Request = { Count = 0; RPS = 0.0 }
        Latency = { MinMs = 0.0; MeanMs = 0.0; MaxMs = 0.0
                    Percent50 = 0.0; Percent75 = 0.0; Percent95 = 0.0; Percent99 = 0.0; StdDev = 0.0
                    LatencyCount = { LessOrEq800 = 0; More800Less1200 = 0; MoreOrEq1200 = 0 } }
        DataTransfer = { MinKb = 0.0; MeanKb = 0.0; MaxKb = 0.0
                         Percent50 = 0.0; Percent75 = 0.0; Percent95 = 0.0; Percent99 = 0.0; StdDev = 0.0; AllMB = 0.0 }
    }
    Fail = {
        Request = { Count = 0; RPS = 0.0 }
        Latency = { MinMs = 0.0; MeanMs = 0.0; MaxMs = 0.0
                    Percent50 = 0.0; Percent75 = 0.0; Percent95 = 0.0; Percent99 = 0.0; StdDev = 0.0
                    LatencyCount = { LessOrEq800 = 0; More800Less1200 = 0; MoreOrEq1200 = 0 } }
        DataTransfer = { MinKb = 0.0; MeanKb = 0.0; MaxKb = 0.0
                         Percent50 = 0.0; Percent75 = 0.0; Percent95 = 0.0; Percent99 = 0.0; StdDev = 0.0; AllMB = 0.0 }
        ErrorStats = Array.empty
    }
}

[<Property>]
let ``analyze should return hint for case when DataTransfer.MinKb = 0`` (minKb: uint32) =

    let req = { baseStepStats.Ok.Request with RPS = 1.0 }
    let dt = { baseStepStats.Ok.DataTransfer with MinKb = float minKb }
    let stepStats = { baseStepStats with Ok = { Request = req; Latency = baseStepStats.Ok.Latency; DataTransfer = dt } }
    let scnStats = { baseScnStats with StepStats = [| stepStats |] }
    let nodeStats = { baseNodeStats with ScenarioStats = [| scnStats |] }

    match HintsAnalyzer.analyze nodeStats with
    | hint::tail when minKb = 0u -> ()
    | [] when minKb > 0u         -> ()
    | e                          -> failwith "analyzer finished with error"
