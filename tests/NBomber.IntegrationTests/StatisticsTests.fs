module Tests.StatisticsTests

open System
open Xunit
open FsCheck.Xunit

open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.Statistics
open NBomber.Extensions

// todo: add test on NodeStats.merge with custom execution duration

let private nodeStatsInfo = {
    MachineName = "1"
    Sender = NodeType.SingleNode
    CurrentOperation = NodeOperationType.Bombing
}

let private latencyCount = { Less800 = 1; More800Less1200 = 1; More1200 = 1 }

let private stepStats = {
    StepName = "step1"; OkLatencies = Array.empty; ReqeustCount = 1; OkCount = 1; FailCount = 1
    RPS = 1; Min = 1; Mean = 1; Max = 1; Percent50 = 1; Percent75 = 1; Percent95 = 1; StdDev = 1;
    DataTransfer = { MinKb = 1.0; MeanKb = 1.0; MaxKb = 1.0; AllMB = 1.0 }
}

let private scenarioStats = {
    ScenarioName = "Scenario1"; StepsStats = [| stepStats |]; RPS = 1;
    ConcurrentCopies = 1; OkCount = 1; FailCount = 1; LatencyCount = latencyCount
    Duration = TimeSpan.FromSeconds(1.0)
}

let private nodeStats = {
    AllScenariosStats = [| scenarioStats |]; OkCount = 1; FailCount = 1
    LatencyCount = latencyCount
    NodeStatsInfo = nodeStatsInfo
}

let private scenario = {
    ScenarioName = "Scenario1"; TestInit = None; TestClean = None
    Steps = Array.empty; ConcurrentCopies = 1; CorrelationIds = Array.empty
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
    let expected = Array.minOrDefault 0 latencies
    Assert.Equal(expected, result)

[<Property>]
let ``calcMean() should not fail and calculate correctly for any args values`` (latencies: Latency[]) =
    let result = latencies |> Statistics.calcMean
    let expected = latencies |> Array.averageByOrDefault 0.0 float |> int
    Assert.Equal(expected, result)

[<Property>]
let ``calcMax() should not fail and calculate correctly for any args values`` (latencies: Latency[]) =
    let result = latencies |> Statistics.calcMax
    let expected = Array.maxOrDefault 0 latencies
    Assert.Equal(expected, result)

[<Fact>]
let ``NodeStats.merge should correctly calculate concurrency counters`` () =

    let meta = { nodeStatsInfo with MachineName = "1"; Sender = NodeType.Cluster }

    let scn = { scenario with ScenarioName = "merge_test"
                              ConcurrentCopies = 50 }

    let scnStats = { scenarioStats with ScenarioName = scn.ScenarioName }
    let agentNode1 = { nodeStats with AllScenariosStats = [| scnStats |]}
    let agentNode2 = { nodeStats with AllScenariosStats = [| scnStats |]}

    let allNodesStats = [| agentNode1; agentNode2 |]
    let allScenarios = [| scn |]

    let mergedStats = NodeStats.merge meta allNodesStats None allScenarios

    Assert.Equal(100, mergedStats.AllScenariosStats.[0].ConcurrentCopies)
