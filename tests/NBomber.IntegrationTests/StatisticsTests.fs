module Tests.Statistics

open System
open System.IO
open System.Collections.Generic
open System.Threading.Tasks

open FSharp.UMX
open FsCheck.Xunit
open HdrHistogram
open Xunit
open Swensen.Unquote

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Contracts.Internal
open NBomber.Extensions.Internal
open NBomber.Extensions.Data
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Stats
open NBomber.Domain.Stats.StepStatsRawData
open NBomber.FSharp
open Tests.TestHelper

module ScenarioStatsTests =

    let internal baseSteps = {
        StepName = "name"
        ClientFactory = None
        ClientPool = None
        ClientInterception = None
        Execute =
          fun _ -> task { return Response.ok(sizeBytes = 100) }
          |> Step.StepContext.toUntypedExecute
        Feed = Some(Feed.createConstant "feed name" [ 1; 2 ])
        Timeout = seconds 0
        DoNotTrack = Constants.DefaultDoNotTrack
        IsPause = false
    }

    let internal baseScenario = {
        ScenarioName = "scenario name"
        Init = None
        Clean = None
        Steps = [ baseSteps ]
        LoadTimeLine = LoadTimeLine.Empty
        WarmUpDuration = None
        PlanedDuration = seconds 0
        ExecutedDuration = None
        CustomSettings = "settings"
        DefaultStepOrder = Array.empty
        StepOrderIndex = Dictionary<string, int>() // stepName * orderNumber
        CustomStepOrder = None
        StepInterception = None
        IsEnabled = false
        IsInitialized = false
    }

    let internal baseRawStepStats ={
        MinMicroSec = Int32.MaxValue
        MaxMicroSec = 0
        MinBytes = Int32.MaxValue
        MaxBytes = 0
        RequestCount = 0
        LessOrEq800 = 0
        More800Less1200 = 0
        MoreOrEq1200 = 0
        AllBytes = 0L
        LatencyHistogram = LongHistogram(highestTrackableValue = Constants.MaxTrackableStepLatency, numberOfSignificantValueDigits = 3)
        DataTransferHistogram =
            LongHistogram(
                highestTrackableValue = Constants.MaxTrackableStepResponseSize,
                numberOfSignificantValueDigits = 3
            )
        StatusCodes = Dictionary<int, RawStatusCodeStats>()
    }

    let internal baseStepStatsRawData = StepStatsRawData.createEmpty()

    let baseLoadSimulationStats = { SimulationName = "simulation name"; Value = 1 }

    [<Property>]
    let ``ScenarioStats RequestCount should sum of OkCount and FailCount from all steps``
        (okCount: int, failCount: int) =

        let okStats = { baseRawStepStats with RequestCount = okCount }
        let failStats = { baseRawStepStats with RequestCount = failCount }

        let stepsData = { OkStats = okStats; FailStats = failStats }

        let scenarioStats =
            Statistics.ScenarioStats.create
                baseScenario
                [| stepsData |]
                baseLoadSimulationStats
                OperationType.Complete
                (UMX.tag (seconds 1))
                (seconds 1)

        test <@ scenarioStats.RequestCount = okCount + failCount @>

    [<Property>]
    let ``ScenarioStats AllBytes should sum AllBytes of OkStats and FailStats from all steps``
        (okAllBytes: int64, failAllBytes: int64) =

        let okStats = { baseRawStepStats with AllBytes = okAllBytes }
        let failStats = { baseRawStepStats with AllBytes = failAllBytes }

        let stepsData = { OkStats = okStats; FailStats = failStats }

        let scenarioStats =
            Statistics.ScenarioStats.create
                baseScenario
                [| stepsData |]
                baseLoadSimulationStats
                OperationType.Complete
                (UMX.tag (seconds 1))
                (seconds 1)

        test <@ scenarioStats.AllBytes = okAllBytes + failAllBytes @>

    [<Property>]
    let ``ScenarioStats OkCount should sum of OkCount from all steps`` (okCount1: int, okCount2: int) =

        let okStats1 = { baseRawStepStats with RequestCount = okCount1 }
        let okStats2 = { baseRawStepStats with RequestCount = okCount2 }
        let stepsData1 = { baseStepStatsRawData with OkStats = okStats1 }
        let stepsData2 = { baseStepStatsRawData with OkStats = okStats2 }

        let scenarioStats =
            Statistics.ScenarioStats.create
                baseScenario
                [| stepsData1; stepsData2 |]
                baseLoadSimulationStats
                OperationType.Complete
                (UMX.tag (seconds 1))
                (seconds 1)

        test <@ scenarioStats.OkCount = okCount1 + okCount2 @>

    [<Property>]
    let ``ScenarioStats FailCount should sum of FailCount from all steps`` (failCount1: int, failCount2: int) =

        let okStats1 = { baseRawStepStats with RequestCount = failCount1 }
        let okStats2 = { baseRawStepStats with RequestCount = failCount2 }
        let stepsData1 = { baseStepStatsRawData with FailStats = okStats1 }
        let stepsData2 = { baseStepStatsRawData with FailStats = okStats2 }

        let scenarioStats =
            Statistics.ScenarioStats.create
                baseScenario
                [| stepsData1; stepsData2 |]
                baseLoadSimulationStats
                OperationType.Complete
                (UMX.tag (seconds 1))
                (seconds 1)

        test <@ scenarioStats.FailCount = failCount1 + failCount2 @>

    [<Property>]
    let ``ScenarioStats LatencyCount should sum of LatencyCount from all steps fro Ok and Fail stats``
        (less800: int, more800Less1200: int, more1200: int) =

        let okStats = {
            baseRawStepStats with
                LessOrEq800 = less800
                More800Less1200 = more800Less1200
                MoreOrEq1200 = more1200
        }

        let failStats = {
            baseRawStepStats with
                LessOrEq800 = less800
                More800Less1200 = more800Less1200
                MoreOrEq1200 = more1200
        }

        let stepStatsRawData =
            { baseStepStatsRawData with OkStats = okStats; FailStats = failStats }

        let scenarioStats =
            Statistics.ScenarioStats.create
                baseScenario
                [| stepStatsRawData |]
                baseLoadSimulationStats
                OperationType.Complete
                (UMX.tag (seconds 1))
                (seconds 1)

        test <@ scenarioStats.LatencyCount.LessOrEq800 = less800 * 2 @>
        test <@ scenarioStats.LatencyCount.More800Less1200 = more800Less1200 * 2 @>
        test <@ scenarioStats.LatencyCount.MoreOrEq1200 = more1200 * 2 @>

    [<Fact>]
    let ``ScenarioStats should not collect stats for steps market as DoNotTrack`` () =

        let doNotTrackStep = { baseSteps with DoNotTrack = true }
        let scenario = { baseScenario with Steps = [ doNotTrackStep ] }

        let scenarioStats =
            Statistics.ScenarioStats.create
                scenario
                [| baseStepStatsRawData |]
                baseLoadSimulationStats
                OperationType.Complete
                (UMX.tag (seconds 1))
                (seconds 1)

        test <@ scenarioStats.StepStats.Length = 0 @>

    [<Fact>]
    let ``ScenarioStats StepInfo should should fallback on default values`` () =

        let doNotTrackStep = { baseSteps with Feed = None }
        let scenario = { baseScenario with Steps = [ doNotTrackStep ] }

        let scenarioStats =
            Statistics.ScenarioStats.create
                scenario
                [| baseStepStatsRawData |]
                baseLoadSimulationStats
                OperationType.Complete
                (UMX.tag (seconds 1))
                (seconds 1)

        let stepStats = scenarioStats.StepStats[0]
        test <@ stepStats.StepInfo.FeedName = "none" @>
        test <@ stepStats.StepInfo.ClientFactoryName = "none" @>
        test <@ stepStats.StepInfo.ClientFactoryClientCount = 0 @>

    [<Fact>]
    let ``ScenarioStats StepStats should be collected converted for all trackable steps`` () =

        let scenarioStats =
            Statistics.ScenarioStats.create
                baseScenario
                [| baseStepStatsRawData |]
                baseLoadSimulationStats
                OperationType.Complete
                (UMX.tag (seconds 1))
                (seconds 1)

        let stepStats = scenarioStats.StepStats[0]
        test <@ stepStats.StepName = baseSteps.StepName @>
        test <@ stepStats.StepInfo.Timeout = baseSteps.Timeout @>

module NodeStatsTests =

    let baseNodeInfo = {
        MachineName = "machine name"
        NodeType = NodeType.SingleNode
        CurrentOperation = OperationType.Complete
        OS = Environment.OSVersion.ToString()
        DotNetVersion = "6.0"
        Processor = "processor"
        CoresCount = 4
        NBomberVersion = "2.0"
    }

    let baseTestInfo =
        { SessionId = "session id"; TestSuite = "test suite"; TestName = "test name"; ClusterId = "" }

    let baseLatencyCount = { LessOrEq800 = 1; More800Less1200 = 1; MoreOrEq1200 = 1 }

    let baseLoadSimulationStats = { SimulationName = "simulation name"; Value = 1 }

    let baseScenarioStats = {
          ScenarioName = "scenario name"
          RequestCount = 1
          OkCount = 1
          FailCount = 1
          AllBytes = 1
          StepStats = Array.empty
          LatencyCount = baseLatencyCount
          LoadSimulationStats = baseLoadSimulationStats
          StatusCodes = Array.empty
          CurrentOperation = OperationType.Complete
          Duration = TimeSpan.Zero
    }

    [<Property>]
    let ``NodeStats RequestCount should be sum of RequestCount of each scenario``
        (requestCount1: int, requestCount2: int) =

        let scenario1 = { baseScenarioStats with RequestCount = requestCount1 }

        let scenario2 = { baseScenarioStats with RequestCount = requestCount2 }

        let nodeStats =
            Statistics.NodeStats.create baseTestInfo baseNodeInfo [| scenario1; scenario2 |]

        test <@ nodeStats.RequestCount = requestCount1 + requestCount2 @>

    [<Property>]
    let ``NodeStats OkCount should be sum of OkCount of each scenario`` (okCount1: int, okCount2: int) =

        let scenario1 = { baseScenarioStats with OkCount = okCount1 }
        let scenario2 = { baseScenarioStats with OkCount = okCount2 }

        let nodeStats =
            Statistics.NodeStats.create baseTestInfo baseNodeInfo [| scenario1; scenario2 |]

        test <@ nodeStats.OkCount = okCount1 + okCount2 @>

    [<Property>]
    let ``NodeStats FailCount should be sum of FailCount of each scenario`` (failCount1: int, failCount2: int) =

        let scenario1 = { baseScenarioStats with FailCount = failCount1 }
        let scenario2 = { baseScenarioStats with FailCount = failCount2 }

        let nodeStats =
            Statistics.NodeStats.create baseTestInfo baseNodeInfo [| scenario1; scenario2 |]

        test <@ nodeStats.FailCount = failCount1 + failCount2 @>

    [<Property>]
    let ``NodeStats AllBytes should be sum of AllBytes of each scenario`` (allBytes1: int64, allBytes2: int64) =

        let scenario1 = { baseScenarioStats with AllBytes = allBytes1 }
        let scenario2 = { baseScenarioStats with AllBytes = allBytes2 }

        let nodeStats =
            Statistics.NodeStats.create baseTestInfo baseNodeInfo [| scenario1; scenario2 |]

        test <@ nodeStats.AllBytes = allBytes1 + allBytes2 @>

    [<Fact>]
    let ``NodeStats Duration should be duration of the longest scenario`` () =
        let scenario1 = { baseScenarioStats with Duration = seconds 10 }
        let scenario2 = { baseScenarioStats with Duration = seconds 20 }

        let nodeStats =
            Statistics.NodeStats.create baseTestInfo baseNodeInfo [| scenario1; scenario2 |]

        test <@ nodeStats.Duration = seconds 20 @>

    [<Fact>]
    let ``NodeStats should be calculated during test execution`` () =

        let okStep = Step.create("ok step", timeout = seconds 2, execute = fun _ -> task {
            do! Task.Delay(milliseconds 500)
            return Response.ok(sizeBytes = 100)
        })

        let failStep = Step.create("fail step", timeout = seconds 2, execute = fun _ -> task {
            do! Task.Delay(milliseconds 500)
            return Response.fail(error = "reason", statusCode = 10, sizeBytes = 10)
        })

        let okScenario =
            Scenario.create "realtime stats ok scenario" [ okStep ]
            |> Scenario.withoutWarmUp
            |> Scenario.withLoadSimulations [ KeepConstant(copies = 1, during = seconds 10) ]

        let failScenario =
            Scenario.create "realtime stats fail scenario" [ failStep ]
            |> Scenario.withoutWarmUp
            |> Scenario.withLoadSimulations [ KeepConstant(copies = 1, during = seconds 10) ]

        NBomberRunner.registerScenarios [ okScenario; failScenario ]
        |> NBomberRunner.withoutReports
        |> NBomberRunner.run
        |> Result.getOk
        |> fun stats ->
            let sc0 = stats.ScenarioStats[0]
            let sc1 = stats.ScenarioStats[1]
            test <@ stats.Duration = seconds 10 @>

            test <@ stats.RequestCount >= 20 && stats.RequestCount <= 40 @>

            test <@ stats.OkCount >= 10 && stats.OkCount <= 20 @>
            test <@ stats.FailCount >= 10 && stats.FailCount <= 20 @>
            test <@ stats.AllBytes = sc0.AllBytes + sc1.AllBytes @>

module StepStatsRawData =

    [<Property>]
    let ``addResponse should take client response latency if it set`` (isClient: bool, latencyMs: uint32) =

        let emptyData = StepStatsRawData.createEmpty()
        let clientResMs = if isClient then float latencyMs else 0.0
        let stepResMs = 42.0

        let clientResponse = {
            StatusCode = Nullable()
            IsError = false
            Message = ""
            SizeBytes = 10
            LatencyMs = clientResMs
            Payload = null
        }

        let stepResponse = {
            StepIndex = 0
            ClientResponse = clientResponse
            EndTimeMs = Double.MaxValue
            LatencyMs = stepResMs
        }

        let data = StepStatsRawData.addResponse emptyData stepResponse

        let realMin = if clientResMs > 0.0 then clientResMs else stepResMs

        let min =
            data.OkStats.MinMicroSec
            |> float
            |> Converter.fromMicroSecToMs

        test <@ min = realMin @>

    [<Property>]
    let ``addResponse should properly calc latency count`` (latencies: uint32 list) =

        let mutable data = StepStatsRawData.createEmpty()

        let latencies = latencies |> List.filter(fun x -> x > 0u)

        latencies
        |> List.iter(fun latency ->
            let clientResponse = {
                StatusCode = Nullable()
                IsError = false
                Message = ""
                SizeBytes = 10
                LatencyMs = float latency
                Payload = null
            }

            let stepResponse = {
                StepIndex = 0
                ClientResponse = clientResponse
                EndTimeMs = Double.MaxValue
                LatencyMs = 0.0
            }

            data <- StepStatsRawData.addResponse data stepResponse
        )

        let lessOrEq800 = latencies |> Seq.filter(fun x -> x <= 800u) |> Seq.length
        let more800Less1200 = latencies |> Seq.filter(fun x -> x > 800u && x < 1200u) |> Seq.length
        let moreOrEq1200 = latencies |> Seq.filter(fun x -> x >= 1200u) |> Seq.length

        test <@ data.OkStats.LessOrEq800 = lessOrEq800 @>
        test <@ data.OkStats.More800Less1200 = more800Less1200 @>
        test <@ data.OkStats.MoreOrEq1200 = moreOrEq1200 @>

    [<Property>]
    let ``addResponse should properly handle OkStats and FailStats`` (latencies: (bool * uint32) list) =

        let mutable data = StepStatsRawData.createEmpty()

        let latencies =
            latencies
            |> List.filter(fun (_, latency) -> latency > 0u)
            |> List.map(fun (isOk, latency) -> isOk, latency |> float)

        latencies
        |> Seq.iter(fun (isOk, latency) ->
            let clientResponse = {
                StatusCode = Nullable()
                IsError = not isOk
                Message = ""
                SizeBytes = 10
                LatencyMs = 0.0
                Payload = null
            }

            let stepResponse = { // only stepResponse latency will be included
                StepIndex = 0
                ClientResponse = clientResponse
                EndTimeMs = Double.MaxValue
                LatencyMs = latency |> float
            }

            data <- StepStatsRawData.addResponse data stepResponse
        )

        // calc OkStats
        let okLatencies = latencies |> Seq.filter(fun (isOk, _) -> isOk)
        let okCount = okLatencies |> Seq.length
        let okLessOrEq800 =
            okLatencies
            |> Seq.filter(fun (_, latency) -> latency <= 800.0)
            |> Seq.length

        let okMinStats = if okCount > 0 then okLatencies |> Seq.map snd |> Seq.min else 0.0
        let okMaxStats = if okCount > 0 then okLatencies |> Seq.map snd |> Seq.max else 0.0

        // calc FailStats
        let failLatencies = latencies |> Seq.filter(fun (isOk, _) -> isOk = false)
        let failCount = failLatencies |> Seq.length

        let failLessOrEq800 =
            failLatencies
            |> Seq.filter(fun (_, latency) -> latency <= 800.0)
            |> Seq.length

        let failMinStats = if failCount > 0 then failLatencies |> Seq.map snd |> Seq.min else 0.0
        let failMaxStats = if failCount > 0 then failLatencies |> Seq.map snd |> Seq.max else 0.0
        let okMin = if okCount > 0 then data.OkStats.MinMicroSec |> float |> Converter.fromMicroSecToMs else 0.0
        let okMax = if okCount > 0 then data.OkStats.MaxMicroSec |> float |> Converter.fromMicroSecToMs else 0.0
        let failMin = if failCount > 0 then data.FailStats.MinMicroSec |> float |> Converter.fromMicroSecToMs else 0.0
        let failMax = if failCount > 0 then data.FailStats.MaxMicroSec |> float |> Converter.fromMicroSecToMs else 0.0

        test <@ data.OkStats.RequestCount = okCount @>
        test <@ data.OkStats.LessOrEq800 = okLessOrEq800 @>
        test <@ okMin = okMinStats @>
        test <@ okMax = okMaxStats @>

        test <@ data.FailStats.RequestCount = failCount @>
        test <@ data.FailStats.LessOrEq800 = failLessOrEq800 @>
        test <@ failMin = failMinStats @>
        test <@ failMax = failMaxStats @>

    [<Property>]
    let ``addResponse should properly calc response sizes`` (responseSizes: (bool * uint32) list) =

        let mutable data = StepStatsRawData.createEmpty()

        let responseSizes =
            responseSizes
            |> List.filter(fun (_, resSize) -> resSize > 0u)
            |> List.map(fun (isOk, resSize) -> isOk, resSize |> int)

        responseSizes
        |> Seq.iter(fun (isOk, resSize) ->
            let clientResponse = {
                StatusCode = Nullable()
                IsError = not isOk
                Message = ""
                SizeBytes = resSize
                LatencyMs = 1.0
                Payload = null
            }

            let stepResponse = {
                StepIndex = 0
                ClientResponse = clientResponse
                EndTimeMs = Double.MaxValue
                LatencyMs = 0.0
            }

            data <- StepStatsRawData.addResponse data stepResponse
        )

        // calc OkStatsData
        let okResponses = responseSizes |> Seq.filter(fun (isOk, _) -> isOk)

        let okCount = okResponses |> Seq.length

        let okMinBytes = if okCount > 0 then okResponses |> Seq.map snd |> Seq.min else 0
        let okMaxBytes = if okCount > 0 then okResponses |> Seq.map snd |> Seq.max else 0
        let okAllBytes = int64(if okCount > 0 then okResponses |> Seq.map snd |> Seq.sum else 0)

        // calc FailStatsData
        let failResponses = responseSizes |> Seq.filter(fun (isOk, _) -> isOk = false)
        let failCount = failResponses |> Seq.length
        let failMinBytes = if failCount > 0 then failResponses |> Seq.map snd |> Seq.min else 0
        let failMaxBytes = if failCount > 0 then failResponses |> Seq.map snd |> Seq.max else 0
        let failAllBytes = int64(if failCount > 0 then failResponses |> Seq.map snd |> Seq.sum else 0)
        let okMin = if okCount > 0 then data.OkStats.MinBytes else 0
        let okMax = if okCount > 0 then data.OkStats.MaxBytes else 0
        let failMin = if failCount > 0 then data.FailStats.MinBytes else 0
        let failMax = if failCount > 0 then data.FailStats.MaxBytes else 0

        test <@ data.OkStats.RequestCount = okCount @>
        test <@ okMin = okMinBytes @>
        test <@ okMax = okMaxBytes @>
        test <@ data.OkStats.AllBytes = okAllBytes @>

        test <@ data.FailStats.RequestCount = failCount @>
        test <@ failMin = failMinBytes @>
        test <@ failMax = failMaxBytes @>
        test <@ data.FailStats.AllBytes = failAllBytes @>

[<Fact>]
let ``NodeStats should be calculated properly`` () =

    let okStep = Step.create("ok step", timeout = seconds 2, execute = fun _ -> task {
        do! Task.Delay(milliseconds 500)
        return Response.ok(sizeBytes = 100)
    })

    let failStep = Step.create("fail step 1",timeout = seconds 2, execute = fun _ -> task {
        do! Task.Delay(milliseconds 500)
        return Response.fail(error = "reason 1", statusCode = 10, sizeBytes = 10)
    })

    let scenario =
        Scenario.create "realtime stats scenario" [ okStep; failStep ]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [ KeepConstant(copies = 1, during = seconds 10) ]

    NBomberRunner.registerScenarios [ scenario ]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        let scnStats = stats.ScenarioStats[0]
        let st1 = scnStats.StepStats[0]
        let st2 = scnStats.StepStats[1]

        test <@ st1.Ok.Request.Count >= 5 && st1.Ok.Request.Count <= 10 @>

        test <@ st1.Ok.DataTransfer.MinBytes = 100 @>
        test <@ st1.Fail.Request.Count = 0 @>

        test <@ st2.Fail.Request.Count >= 5 && st2.Fail.Request.Count <= 10 @>

        test <@ st2.Fail.DataTransfer.MinBytes = 10 @>
        test <@ st2.Ok.Request.Count = 0 @>

[<Fact>]
let ``NodeStats ReportFiles should contain report content`` () =

    let okStep = Step.create("ok step", timeout = seconds 2, execute = fun context -> task {
        do! Task.Delay(milliseconds 500)
        return Response.ok(sizeBytes = 100)
    })

    let failStep = Step.create("fail step 1", timeout = seconds 2, execute = fun context -> task {
        do! Task.Delay(milliseconds 500)
        return Response.fail(error = "reason 1", statusCode = 10, sizeBytes = 10)
    })

    let scenario =
        Scenario.create "realtime stats scenario" [okStep; failStep]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 5)]

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.withReportFolder "./reports/node_stats/1"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        let reportFile = stats.ReportFiles[0]
        let fileContent = File.ReadAllText(reportFile.FilePath)

        test <@ reportFile.ReportContent = fileContent @>

[<Fact>]
let ``status codes should be calculated properly`` () =

    let okStep = Step.create("ok step", fun _ -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok(statusCode = 10)
    })

    let okStepNoStatus = Step.create("ok step no status", fun _ -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })

    let failStep = Step.create("fail step", fun _ -> task {
        do! Task.Delay(milliseconds 100)
        return Response.fail(statusCode = -10)
    })

    let scenario =
        Scenario.create "realtime stats scenario" [ okStep; okStepNoStatus; failStep ]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [ KeepConstant(copies = 2, during = seconds 10) ]

    NBomberRunner.registerScenarios [ scenario ]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        let allCodes = stats.ScenarioStats[0].StatusCodes
        let okStCodes = stats.ScenarioStats[0].StepStats[0].Ok.StatusCodes
        let okNoStatusStCodes = stats.ScenarioStats[0].StepStats[1].Ok.StatusCodes
        let failStCodes = stats.ScenarioStats[0].StepStats[2].Fail.StatusCodes

        test <@ allCodes
               |> Seq.find(fun x -> x.StatusCode = 10 || x.StatusCode = -10)
               |> fun x -> x.Count > 10 @>

        test <@ okStCodes
               |> Seq.find(fun x -> x.StatusCode = 10)
               |> fun error -> error.Count > 10 @>

        test <@ failStCodes
               |> Seq.find(fun x -> x.StatusCode = -10)
               |> fun error -> error.Count > 10 @>

        test <@ Array.isEmpty okNoStatusStCodes @>

[<Fact>]
let ``StatusCodeStats merge function returns sorted results`` () =

    let stats: StatusCodeStats[] = [|
       { StatusCode = 50; IsError = false; Message = String.Empty; Count = 1 }
       { StatusCode = 80; IsError = false; Message = String.Empty; Count = 1 }
       { StatusCode = 10; IsError = false; Message = String.Empty; Count = 1 }
    |]

    let result =
        stats
        |> Statistics.StatusCodeStats.merge
        |> Array.map(fun x -> x.StatusCode)

    test <@ [| 10; 50; 80 |] = result @>
