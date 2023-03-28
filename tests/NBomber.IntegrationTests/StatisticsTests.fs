module Tests.Statistics

open System
open System.IO
open System.Collections.Generic
open System.Threading.Tasks

open FsCheck.Xunit
open HdrHistogram
open Xunit
open Swensen.Unquote

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Contracts.Internal
open NBomber.Extensions.Internal
open NBomber.Domain
open NBomber.Domain.Stats
open NBomber.Domain.Stats.Statistics
open NBomber.Domain.Stats.RawMeasurementStats
open NBomber.FSharp
open Tests.TestHelper

module ScenarioStatsTests =

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
        StatusCodes = Dictionary<string, RawStatusCodeStats>()
    }

    let internal baseStepStatsRawData = RawMeasurementStats.empty("step")

    let baseLoadSimulationStats = { SimulationName = "simulation name"; Value = 1 }

    [<Property>]
    let ``ScenarioStats RequestCount should sum of OkCount and FailCount from all steps``
        (okCount: int, failCount: int) =

        let okStats = { baseRawStepStats with RequestCount = okCount }
        let failStats = { baseRawStepStats with RequestCount = failCount }

        let stepsData = { Name = Constants.ScenarioGlobalInfo; OkStats = okStats; FailStats = failStats }

        let scenarioStats =
            ScenarioStats.create
                "scenario"
                [| stepsData |]
                baseLoadSimulationStats
                OperationType.Complete
                (seconds 1)
                (seconds 1)
                TimeSpan.Zero

        test <@ scenarioStats.Ok.Request.Count + scenarioStats.Fail.Request.Count = okCount + failCount @>

    [<Property>]
    let ``ScenarioStats AllBytes should sum AllBytes of OkStats and FailStats from all steps``
        (okAllBytes: int64, failAllBytes: int64) =

        let okStats = { baseRawStepStats with AllBytes = okAllBytes }
        let failStats = { baseRawStepStats with AllBytes = failAllBytes }

        let stepsData = { Name = Constants.ScenarioGlobalInfo; OkStats = okStats; FailStats = failStats }

        let scenarioStats =
            ScenarioStats.create
                "scenario"
                [| stepsData |]
                baseLoadSimulationStats
                OperationType.Complete
                (seconds 1)
                (seconds 1)
                TimeSpan.Zero

        test <@ ScenarioStats.calcAllBytes scenarioStats = okAllBytes + failAllBytes @>

    [<Property>]
    let ``ScenarioStats OkCount should be separated from OkCount of steps`` (okCount1: int, okCount2: int) =

        let okStats1 = { baseRawStepStats with RequestCount = okCount1 }
        let okStats2 = { baseRawStepStats with RequestCount = okCount2 }
        let stepsData1 = { baseStepStatsRawData with Name = Constants.ScenarioGlobalInfo; OkStats = okStats1 }
        let stepsData2 = { baseStepStatsRawData with OkStats = okStats2 }

        let scenarioStats =
            ScenarioStats.create
                "scenario"
                [| stepsData1; stepsData2 |]
                baseLoadSimulationStats
                OperationType.Complete
                (seconds 1)
                (seconds 1)
                TimeSpan.Zero

        test <@ scenarioStats.Ok.Request.Count = okCount1 @>
        test <@ scenarioStats.StepStats[0].Ok.Request.Count = okCount2 @>

    [<Property>]
    let ``ScenarioStats FailCount should be separated from FailCount of steps`` (failCount1: int, failCount2: int) =

        let okStats1 = { baseRawStepStats with RequestCount = failCount1 }
        let okStats2 = { baseRawStepStats with RequestCount = failCount2 }
        let stepsData1 = { baseStepStatsRawData with Name = Constants.ScenarioGlobalInfo; FailStats = okStats1 }
        let stepsData2 = { baseStepStatsRawData with FailStats = okStats2 }

        let scenarioStats =
            ScenarioStats.create
                "scenario"
                [| stepsData1; stepsData2 |]
                baseLoadSimulationStats
                OperationType.Complete
                (seconds 1)
                (seconds 1)
                TimeSpan.Zero

        test <@ scenarioStats.Fail.Request.Count = failCount1 @>
        test <@ scenarioStats.StepStats[0].Fail.Request.Count = failCount2 @>


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
            { baseStepStatsRawData with Name = Constants.ScenarioGlobalInfo; OkStats = okStats; FailStats = failStats }

        let scenarioStats =
            ScenarioStats.create
                "scenario"
                [| stepStatsRawData |]
                baseLoadSimulationStats
                OperationType.Complete
                (seconds 1)
                (seconds 1)
                TimeSpan.Zero

        test <@ scenarioStats.Ok.Latency.LatencyCount.LessOrEq800 = less800 @>
        test <@ scenarioStats.Ok.Latency.LatencyCount.More800Less1200 = more800Less1200 @>
        test <@ scenarioStats.Ok.Latency.LatencyCount.MoreOrEq1200 = more1200 @>

        test <@ scenarioStats.Fail.Latency.LatencyCount.LessOrEq800 = less800 @>
        test <@ scenarioStats.Fail.Latency.LatencyCount.More800Less1200 = more800Less1200 @>
        test <@ scenarioStats.Fail.Latency.LatencyCount.MoreOrEq1200 = more1200 @>

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
          ScenarioStats.ScenarioName = "scenario name"
          Ok = MeasurementStats.empty
          Fail = MeasurementStats.empty
          StepStats = Array.empty
          LoadSimulationStats = baseLoadSimulationStats
          CurrentOperation = OperationType.Complete
          AllRequestCount = 0
          AllOkCount = 0
          AllFailCount = 0
          AllBytes = 0
          Duration = TimeSpan.Zero
    }

    [<Property>]
    let ``NodeStats RequestCount should be sum of Ok + Fail of each scenario``
        (okCount: int, failCount: int, allBytes: int64) =

        let scenario1 = { baseScenarioStats with AllRequestCount = okCount; AllOkCount = okCount; AllBytes = allBytes }
        let scenario2 = { baseScenarioStats with AllRequestCount = failCount; AllFailCount = failCount; AllBytes = allBytes }

        let nodeStats =
            NodeStats.create baseTestInfo baseNodeInfo [| scenario1; scenario2 |]

        test <@ nodeStats.AllRequestCount = okCount + failCount @>
        test <@ nodeStats.AllOkCount = okCount @>
        test <@ nodeStats.AllFailCount = failCount @>
        test <@ nodeStats.AllBytes = allBytes + allBytes @>

    [<Fact>]
    let ``NodeStats Duration should be duration of the longest scenario`` () =
        let scenario1 = { baseScenarioStats with Duration = seconds 10 }
        let scenario2 = { baseScenarioStats with Duration = seconds 20 }

        let nodeStats =
            NodeStats.create baseTestInfo baseNodeInfo [| scenario1; scenario2 |]

        test <@ nodeStats.Duration = seconds 20 @>

    [<Fact>]
    let ``NodeStats should be calculated during test execution`` () =

        let okScenario =
            Scenario.create("ok scenario", fun ctx -> task {
                do! Task.Delay(milliseconds 500)
                return Response.ok(sizeBytes = 100)
            })
            |> Scenario.withoutWarmUp
            |> Scenario.withLoadSimulations [Inject(rate = 1, interval = seconds 0.5, during = seconds 10)]

        let failScenario =
            Scenario.create("fail scenario", fun ctx -> task {
                do! Task.Delay(milliseconds 500)
                return Response.fail(statusCode = "10", sizeBytes = 10, message = "reason")
            })
            |> Scenario.withoutWarmUp
            |> Scenario.withLoadSimulations [Inject(rate = 1, interval = seconds 0.5, during = seconds 10)]

        NBomberRunner.registerScenarios [okScenario; failScenario]
        |> NBomberRunner.withoutReports
        |> NBomberRunner.run
        |> Result.getOk
        |> fun stats ->
            let sc0 = stats.GetScenarioStats("ok scenario")
            let sc1 = stats.GetScenarioStats("fail scenario")
            test <@ stats.Duration = seconds 10 @>

            test <@ stats.AllRequestCount = 40 @>

            test <@ stats.AllOkCount = 20 @>
            test <@ stats.AllFailCount = 20 @>
            test <@ stats.AllBytes = sc0.Ok.DataTransfer.AllBytes + sc1.Fail.DataTransfer.AllBytes @>

module StepStatsRawData =

    [<Property>]
    let ``addStepResult should take client response latency if it set`` (isClient: bool, latencyMs: uint32) =

        let emptyData = RawMeasurementStats.empty("step_name")
        let clientResMs = if isClient then float latencyMs else 0.0
        let stepResMs = 42.0

        let clientResponse = {
            StatusCode = ""
            IsError = false
            Message = ""
            SizeBytes = 10
            LatencyMs = clientResMs
            Payload = None
        }

        let stepResult = {
            Name = "step_name"
            ClientResponse = clientResponse
            CurrentTimeBucket = TimeSpan.Zero            
            Latency = milliseconds stepResMs
        }

        RawMeasurementStats.addMeasurement emptyData stepResult 10

        let realMin = if clientResMs > 0.0 then clientResMs else stepResMs

        let min =
            emptyData.OkStats.MinMicroSec
            |> float
            |> NBomber.Converter.fromMicroSecToMs

        test <@ min = realMin @>

    [<Property>]
    let ``addStepResult should properly calc latency count`` (latencies: uint32 list) =

        let data = RawMeasurementStats.empty("step_name")

        let latencies = latencies |> List.filter(fun x -> x > 0u)

        latencies
        |> List.iter(fun latency ->
            let clientResponse = {
                StatusCode = ""
                IsError = false
                Message = ""
                SizeBytes = 10
                LatencyMs = float latency
                Payload = None
            }

            let stepResponse = {
                Name = "step_name"
                ClientResponse = clientResponse
                CurrentTimeBucket = TimeSpan.Zero                
                Latency = seconds 0
            }

            RawMeasurementStats.addMeasurement data stepResponse 10
        )

        let lessOrEq800 = latencies |> Seq.filter(fun x -> x <= 800u) |> Seq.length
        let more800Less1200 = latencies |> Seq.filter(fun x -> x > 800u && x < 1200u) |> Seq.length
        let moreOrEq1200 = latencies |> Seq.filter(fun x -> x >= 1200u) |> Seq.length

        test <@ data.OkStats.LessOrEq800 = lessOrEq800 @>
        test <@ data.OkStats.More800Less1200 = more800Less1200 @>
        test <@ data.OkStats.MoreOrEq1200 = moreOrEq1200 @>

    [<Property>]
    let ``addStepResult should properly handle OkStats and FailStats`` (latencies: (bool * uint32) list) =

        let data = RawMeasurementStats.empty("step_name")

        let latencies =
            latencies
            |> List.filter(fun (_, latency) -> latency > 0u)
            |> List.map(fun (isOk, latency) -> isOk, latency |> float)

        latencies
        |> Seq.iter(fun (isOk, latency) ->
            let clientResponse = {
                StatusCode = ""
                IsError = not isOk
                Message = ""
                SizeBytes = 10
                LatencyMs = 0
                Payload = None
            }

            let stepResponse = { // only stepResponse latency will be included
                Name = "step_name"
                ClientResponse = clientResponse
                CurrentTimeBucket = TimeSpan.Zero                
                Latency = milliseconds latency
            }

            RawMeasurementStats.addMeasurement data stepResponse 10
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
        let okMin = if okCount > 0 then data.OkStats.MinMicroSec |> float |> NBomber.Converter.fromMicroSecToMs else 0.0
        let okMax = if okCount > 0 then data.OkStats.MaxMicroSec |> float |> NBomber.Converter.fromMicroSecToMs else 0.0
        let failMin = if failCount > 0 then data.FailStats.MinMicroSec |> float |> NBomber.Converter.fromMicroSecToMs else 0.0
        let failMax = if failCount > 0 then data.FailStats.MaxMicroSec |> float |> NBomber.Converter.fromMicroSecToMs else 0.0

        test <@ data.OkStats.RequestCount = okCount @>
        test <@ data.OkStats.LessOrEq800 = okLessOrEq800 @>
        test <@ okMin = okMinStats @>
        test <@ okMax = okMaxStats @>

        test <@ data.FailStats.RequestCount = failCount @>
        test <@ data.FailStats.LessOrEq800 = failLessOrEq800 @>
        test <@ failMin = failMinStats @>
        test <@ failMax = failMaxStats @>

    [<Property>]
    let ``addStepResult should properly calc response sizes`` (responseSizes: (bool * uint32) list) =

        let data = RawMeasurementStats.empty("step")

        let responseSizes =
            responseSizes
            |> List.filter(fun (_, resSize) -> resSize > 0u)
            |> List.map(fun (isOk, resSize) -> isOk, resSize |> int)

        responseSizes
        |> Seq.iter(fun (isOk, resSize) ->
            let clientResponse = {
                StatusCode = ""
                IsError = not isOk
                Message = ""
                SizeBytes = resSize
                LatencyMs = 1.0
                Payload = None
            }

            let stepResponse = {
                Name = "step"
                ClientResponse = clientResponse
                CurrentTimeBucket = TimeSpan.Zero                
                Latency = seconds 0
            }

            RawMeasurementStats.addMeasurement data stepResponse resSize
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

    let scenario =
        Scenario.create("realtime stats scenario", fun ctx -> task {

            let! okStep = Step.run("ok step", ctx, fun () -> task {
                do! Task.Delay(milliseconds 500)
                return Response.ok(sizeBytes = 100)
            })

            let! failStep = Step.run("fail step", ctx, fun () -> task {
                do! Task.Delay(milliseconds 500)
                return Response.fail(message = "reason 1", statusCode = "10", sizeBytes = 10)
            })

            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [Inject(rate = 1, interval = seconds 1, during = seconds 5)]

    NBomberRunner.registerScenarios [ scenario ]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        let scnStats = stats.ScenarioStats[0]
        let okStep = scnStats.GetStepStats("ok step")
        let failStep = scnStats.GetStepStats("fail step")

        test <@ okStep.Ok.Request.Count = 5 @>
        test <@ okStep.Ok.Request.RPS = 1 @>

        test <@ okStep.Ok.DataTransfer.MinBytes = 100 @>
        test <@ okStep.Fail.Request.Count = 0 @>

        test <@ failStep.Fail.Request.Count = 5 @>
        test <@ failStep.Fail.Request.RPS = 1 @>

        test <@ failStep.Fail.DataTransfer.MinBytes = 10 @>
        test <@ failStep.Ok.Request.Count = 0 @>

[<Fact>]
let ``NodeStats ReportFiles should contain report content`` () =

    let scenario =
        Scenario.create("realtime stats scenario", fun ctx -> task {

            let! okStep = Step.run("ok step", ctx, fun () -> task {
                do! Task.Delay(milliseconds 500)
                return Response.ok(sizeBytes = 100)
            })

            let! failStep = Step.run("fail step", ctx, fun () -> task {
                do! Task.Delay(milliseconds 500)
                return Response.fail(message = "reason 1", statusCode = "10", sizeBytes = 10)
            })

            return Response.ok()
        })
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

    let scenario =
        Scenario.create("realtime stats scenario", fun ctx -> task {

            let! okStep = Step.run("ok step", ctx, fun () -> task {
                do! Task.Delay(milliseconds 100)
                return Response.ok(statusCode = "10")
            })

            let! okStepNoStatus = Step.run("ok step no status", ctx, fun () -> task {
                do! Task.Delay(milliseconds 100)
                return Response.ok()
            })

            let! failStep = Step.run("fail step", ctx, fun () -> task {
                do! Task.Delay(milliseconds 100)
                return Response.fail(statusCode = "-10")
            })

            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 10)]

    NBomberRunner.registerScenarios [ scenario ]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        let allCodes = stats.ScenarioStats[0].Ok.StatusCodes |> Array.append(stats.ScenarioStats[0].Fail.StatusCodes)
        let okStCodes = stats.ScenarioStats[0].GetStepStats("ok step").Ok.StatusCodes
        let okNoStatusStCodes = stats.ScenarioStats[0].GetStepStats("ok step no status").Ok.StatusCodes
        let failStCodes = stats.ScenarioStats[0].GetStepStats("fail step").Fail.StatusCodes

        test <@ allCodes
               |> Seq.find(fun x -> x.StatusCode = "10" || x.StatusCode = "-10")
               |> fun x -> x.Count > 10 @>

        test <@ okStCodes
               |> Seq.find(fun x -> x.StatusCode = "10")
               |> fun error -> error.Count > 10 @>

        test <@ failStCodes
               |> Seq.find(fun x -> x.StatusCode = "-10")
               |> fun error -> error.Count > 10 @>

        test <@ Array.isEmpty okNoStatusStCodes @>

[<Fact>]
let ``StatusCodeStats merge function returns sorted results`` () =

    let stats: StatusCodeStats[] = [|
       { StatusCode = "50"; IsError = false; Message = String.Empty; Count = 1 }
       { StatusCode = "80"; IsError = false; Message = String.Empty; Count = 1 }
       { StatusCode = "10"; IsError = false; Message = String.Empty; Count = 1 }
    |]

    let result =
        stats
        |> Statistics.StatusCodeStats.merge
        |> Array.map(fun x -> x.StatusCode)

    test <@ [| "10"; "50"; "80" |] = result @>

[<Fact>]
let ``Stats should be calculated without Pause simulation`` () =

    let scenario =
        Scenario.create("realtime stats scenario", fun ctx -> task {
            do! Task.Delay(milliseconds 100)
            return Response.ok()
        })
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            Pause(during = seconds 10)
            Inject(rate = 100, interval = seconds 1, during = seconds 1)
        ]

    NBomberRunner.registerScenarios [ scenario ]
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        let ok = stats.GetScenarioStats("realtime stats scenario").Ok
        let duration = stats.GetScenarioStats("realtime stats scenario").Duration

        test <@ ok.Request.RPS = 100 @>
        test <@ ok.Request.Count = 100 @>
        test <@ duration = seconds 11 @> // inject (1 sec) + pause (10 sec)
