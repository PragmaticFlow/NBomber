module Tests.Statistics

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.NonAffine
open FsCheck.Xunit
open Xunit
open Swensen.Unquote

open NBomber
open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Stats
open NBomber.FSharp
open NBomber.Extensions.InternalExtensions
open Tests.TestHelper

module StepStatsRawData =

    [<Property>]
    let ``addResponse should take client response latency if it set`` (isClient: bool, latencyMs: uint32) =

        let emptyData = StepStatsRawData.createEmpty()

        let clientResMs = if isClient then float latencyMs else 0.0
        let stepResMs = 42.0

        let clientResponse = { StatusCode = Nullable(); IsError = false
                               ErrorMessage = ""; SizeBytes = 10
                               LatencyMs = clientResMs; Payload = null }
        let stepResponse = {
            ClientResponse = clientResponse
            EndTimeMs = Double.MaxValue
            LatencyMs = stepResMs
        }

        let data = StepStatsRawData.addResponse emptyData stepResponse

        let realMin =
            if clientResMs > 0.0 then clientResMs
            else stepResMs

        let min = data.OkStats.MinMicroSec |> float |> Statistics.Converter.fromMicroSecToMs

        test <@ min = realMin @>

    [<Property>]
    let ``addResponse should properly calc latency count`` (latencies: uint32 list) =

        let mutable data = StepStatsRawData.createEmpty()

        let latencies = latencies |> List.filter(fun x -> x > 0u)

        latencies
        |> List.iter(fun latency ->
            let clientResponse = {
                StatusCode = Nullable(); IsError = false
                ErrorMessage = ""; SizeBytes = 10
                LatencyMs = float latency; Payload = null
            }
            let stepResponse = {
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
            |> List.filter(fun (_,latency) -> latency > 0u)
            |> List.map(fun (isOk,latency) -> isOk, latency |> float)

        latencies
        |> Seq.iter(fun (isOk,latency) ->
            let clientResponse = {
                StatusCode = Nullable(); IsError = not isOk
                ErrorMessage = ""; SizeBytes = 10
                LatencyMs = 0.0; Payload = null
            }
            let stepResponse = { // only stepResponse latency will be included
                ClientResponse = clientResponse
                EndTimeMs = Double.MaxValue
                LatencyMs = latency |> float
            }
            data <- StepStatsRawData.addResponse data stepResponse
        )

        // calc OkStats
        let okLatencies = latencies |> Seq.filter(fun (isOk,_) -> isOk)
        let okCount = okLatencies |> Seq.length
        let okLessOrEq800 = okLatencies |> Seq.filter(fun (_,latency) -> latency <= 800.0) |> Seq.length
        let okMinStats = if okCount > 0 then okLatencies |> Seq.map(snd) |> Seq.min else 0.0
        let okMaxStats = if okCount > 0 then okLatencies |> Seq.map(snd) |> Seq.max else 0.0

        // calc FailStats
        let failLatencies = latencies |> Seq.filter(fun (isOk,_) -> isOk = false)
        let failCount = failLatencies |> Seq.length
        let failLessOrEq800 = failLatencies |> Seq.filter(fun (_,latency) -> latency <= 800.0) |> Seq.length
        let failMinStats = if failCount > 0 then failLatencies |> Seq.map(snd) |> Seq.min else 0.0
        let failMaxStats = if failCount > 0 then failLatencies |> Seq.map(snd) |> Seq.max else 0.0

        let okMin = if okCount > 0 then data.OkStats.MinMicroSec |> float |> Statistics.Converter.fromMicroSecToMs else 0.0
        let okMax = if okCount > 0 then data.OkStats.MaxMicroSec |> float |> Statistics.Converter.fromMicroSecToMs else 0.0
        let failMin = if failCount > 0 then data.FailStats.MinMicroSec |> float |> Statistics.Converter.fromMicroSecToMs else 0.0
        let failMax = if failCount > 0 then data.FailStats.MaxMicroSec |> float |> Statistics.Converter.fromMicroSecToMs else 0.0

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
            |> List.filter(fun (_,resSize) -> resSize > 0u)
            |> List.map(fun (isOk,resSize) -> isOk, resSize |> int)

        responseSizes
        |> Seq.iter(fun (isOk,resSize) ->
            let clientResponse = {
                StatusCode = Nullable(); IsError = not isOk
                ErrorMessage = ""; SizeBytes = resSize
                LatencyMs = 1.0; Payload = null
            }
            let stepResponse = {
                ClientResponse = clientResponse
                EndTimeMs = Double.MaxValue
                LatencyMs = 0.0
            }
            data <- StepStatsRawData.addResponse data stepResponse
        )

        // calc OkStatsData
        let okResponses = responseSizes |> Seq.filter(fun (isOk,_) -> isOk)
        let okCount = okResponses |> Seq.length

        let okMinBytes = if okCount > 0 then okResponses |> Seq.map(snd) |> Seq.min else 0
        let okMaxBytes = if okCount > 0 then okResponses |> Seq.map(snd) |> Seq.max else 0
        let okAllBytes = int64(if okCount > 0 then okResponses |> Seq.map(snd) |> Seq.sum else 0)

        // calc FailStatsData
        let failResponses = responseSizes |> Seq.filter(fun (isOk,_) -> isOk = false)
        let failCount = failResponses |> Seq.length

        let failMinBytes = if failCount > 0 then failResponses |> Seq.map(snd) |> Seq.min else 0
        let failMaxBytes = if failCount > 0 then failResponses |> Seq.map(snd) |> Seq.max else 0
        let failAllBytes = int64(if failCount > 0 then failResponses |> Seq.map(snd) |> Seq.sum else 0)

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
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 10)]

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.withReportFolder "./stats-tests/1/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        let scnStats = stats.ScenarioStats.[0]
        let st1 = scnStats.StepStats.[0]
        let st2 = scnStats.StepStats.[1]

        test <@ st1.Ok.Request.Count >= 5 && st1.Ok.Request.Count <= 10 @>
        test <@ st1.Ok.DataTransfer.MinBytes = 100 @>
        test <@ st1.Fail.Request.Count = 0 @>

        test <@ st2.Fail.Request.Count >= 5 && st2.Fail.Request.Count <= 10 @>
        test <@ st2.Fail.DataTransfer.MinBytes = 10 @>
        test <@ st2.Ok.Request.Count = 0 @>

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
        Scenario.create "realtime stats scenario" [okStep; okStepNoStatus; failStep]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConstant(copies = 2, during = seconds 10)
        ]

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.withReportFolder "./stats-tests/2/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        let allCodes = stats.ScenarioStats.[0].StatusCodes
        let okStCodes = stats.ScenarioStats.[0].StepStats.[0].Ok.StatusCodes
        let okNoStatusStCodes = stats.ScenarioStats.[0].StepStats.[1].Ok.StatusCodes
        let failStCodes = stats.ScenarioStats.[0].StepStats.[2].Fail.StatusCodes

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

    let stats = [|
        { StatusCode = 50; IsError = false; Message = String.Empty; Count = 1 }
        { StatusCode = 80; IsError = false; Message = String.Empty; Count = 1 }
        { StatusCode = 10; IsError = false; Message = String.Empty; Count = 1 }
    |]

    let result =
        stats
        |> Statistics.StatusCodeStats.merge
        |> Array.map(fun x -> x.StatusCode)

    test <@ [| 10;50;80 |] = result @>
