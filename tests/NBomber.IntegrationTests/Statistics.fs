module Tests.Statistics

open System
open Xunit
open FsCheck.Xunit

open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.Statistics

let private meta = { SessionId = "1"; NodeName = "1"; Sender = NodeType.SingleNode }

let private latencyCount = { Less800 = 1; More800Less1200 = 1; More1200 = 1 }

let private stepStats = {
    StepName = "step1"; OkLatencies = Array.empty; ReqeustCount = 1; OkCount = 1; FailCount = 1
    RPS = 1; Min = 1; Mean = 1; Max = 1; Percent50 = 1; Percent75 = 1; Percent95 = 1; StdDev = 1;
    DataTransfer = { MinKb = 1.0; MeanKb = 1.0; MaxKb = 1.0; AllMB = 1.0 }
}

let private scenarioStats = { 
    ScenarioName = "Scenario1"; StepsStats = [| stepStats |]; RPS = 1;
    ConcurrentCopies = 1; ThreadCount = 1;
    OkCount = 1; FailCount = 1; LatencyCount = latencyCount
    Duration = TimeSpan.FromSeconds(1.0) 
}

let private nodeStats = {
    AllScenariosStats = [| scenarioStats |]; OkCount = 1; FailCount = 1
    LatencyCount = latencyCount
    Meta = meta 
}

let private scenario = { 
    ScenarioName = "Scenario1"; TestInit = None; TestClean = None; Steps = Array.empty
    Assertions = Array.empty; ConcurrentCopies = 1; ThreadCount = 1
    CorrelationIds = Array.empty;
    WarmUpDuration = TimeSpan.FromSeconds(1.0)
    Duration = TimeSpan.FromSeconds(1.0) 
}

[<Property>]
let ``calcRPS() should not fail and calculate correctly for any args values`` (latencies: Latency[], scnDuration: TimeSpan) =
    let result = Statistics.calcRPS(latencies, scnDuration)

    if latencies.Length = 0 then
        Assert.Equal(0, result)    
    elif latencies.Length <> 0 && scnDuration.TotalSeconds < 1.0 then        
        Assert.Equal(latencies.Length, result)
    else
        let expected = latencies.Length / int(scnDuration.TotalSeconds)
        Assert.Equal(expected, result)

[<Property>]
let ``calcMin() should not fail and calculate correctly for any args values`` (latencies: Latency[]) =
    let result   = Statistics.calcMin(latencies)    
    let expected = if Array.isEmpty latencies then 0
                   else Array.min(latencies)
    Assert.Equal(expected, result)

[<Property>]
let ``calcMean() should not fail and calculate correctly for any args values`` (latencies: Latency[]) =
    let result = latencies |> Statistics.calcMean    
    let expected = if Array.isEmpty latencies then 0
                   else (Array.averageBy float latencies) |> int
    Assert.Equal(expected, result)

[<Property>]
let ``calcMax() should not fail and calculate correctly for any args values`` (latencies: Latency[]) =
    let result = latencies |> Statistics.calcMax    
    let expected = if Array.isEmpty latencies then 0
                   else Array.max(latencies)
    Assert.Equal(expected, result)

[<Fact>]
let ``NodeStats.merge should correctly calculate concurrency counters`` () =
    
    let meta = { meta with SessionId = "1"; NodeName = "1"; Sender = NodeType.Cluster }    
    
    let scn = { scenario with ScenarioName = "merge_test"
                              ConcurrentCopies = 50
                              ThreadCount = 10 }

    let scnStats = { scenarioStats with ScenarioName = scn.ScenarioName } 
    let agentNode1 = { nodeStats with AllScenariosStats = [| scnStats |]}
    let agentNode2 = { nodeStats with AllScenariosStats = [| scnStats |]}

    let allNodesStats = [| agentNode1; agentNode2 |]
    let allScenarios = [| scn |]

    let mergedStats = NodeStats.merge(meta, allNodesStats) allScenarios
    
    Assert.Equal(100, mergedStats.AllScenariosStats.[0].ConcurrentCopies)
    Assert.Equal(20, mergedStats.AllScenariosStats.[0].ThreadCount)

