module Tests.Statistics

open System
open System.Threading.Tasks

open FSharp.UMX
open FSharp.Control.Tasks.NonAffine
open FsCheck.Xunit
open Xunit
open Swensen.Unquote
open Nessos.Streams

open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Statistics
open NBomber.FSharp
open NBomber.Extensions.InternalExtensions
open Tests.TestHelper

module StepExecutionData =

    let fromMsToTicks (ms: float<ms>) = (ms * float TimeSpan.TicksPerMillisecond) |> int64 |> UMX.tag<ticks>
    let fromTicksToMs (ticks: int64<ticks>) = (float ticks / float TimeSpan.TicksPerMillisecond) |> UMX.tag<ms>

    let fromBytesToMB (bytes: int64<bytes>) =
        let mb = if bytes > 0L<bytes> then float bytes / 1024.0 / 1024.0 else 0.0
        mb |> UMX.untag |> UMX.tag<mb>

    [<Property>]
    let ``addResponse should take client response latency if it set`` (isClient: bool, latency: uint32) =

        let emptyData = Step.StepExecutionData.createEmpty()

        let clientResMs = if isClient then float latency else 0.0
        let stepResMs = int64 Int32.MaxValue

        let clientResponse = { StatusCode = Nullable(); IsError = false
                               ErrorMessage = ""; SizeBytes = 10
                               LatencyMs = clientResMs; Payload = null }
        let stepResponse = {
            ClientResponse = clientResponse
            EndTimeTicks = % Int64.MaxValue
            LatencyTicks = % stepResMs
        }

        let data = Step.StepExecutionData.addResponse emptyData stepResponse
        let realMinTicks =
            if clientResMs > 0.0 then clientResMs |> UMX.tag |> fromMsToTicks |> UMX.untag
            else stepResMs

        let min = data.OkStats.LatencyHistogramTicks |> Histogram.min

        let acceptableDifference = min - realMinTicks
        test <@ acceptableDifference < 1_000L @> // acceptableDifference is close to 0.1 ms

    [<Property>]
    let ``addResponse should properly calc latency count`` (latencies: uint32 list) =

        let mutable data = Step.StepExecutionData.createEmpty()

        let latencies = latencies |> List.filter(fun x -> x > 0u)
        latencies
        |> Seq.iter(fun latency ->
            let clientResponse = {
                StatusCode = Nullable(); IsError = false
                ErrorMessage = ""; SizeBytes = 10
                LatencyMs = float latency; Payload = null
            }
            let stepResponse = {
                ClientResponse = clientResponse
                EndTimeTicks = % Int64.MaxValue
                LatencyTicks = 0L<ticks>
            }
            data <- Step.StepExecutionData.addResponse data stepResponse
        )

        let lessOrEq800 = latencies |> Seq.filter(fun x -> x <= 800u) |> Seq.length
        let more800Less1200 = latencies |> Seq.filter(fun x -> x > 800u && x < 1200u) |> Seq.length
        let moreOrEq1200 = latencies |> Seq.filter(fun x -> x >= 1200u) |> Seq.length

        test <@ data.OkStats.LessOrEq800 = lessOrEq800 @>
        test <@ data.OkStats.More800Less1200 = more800Less1200 @>
        test <@ data.OkStats.MoreOrEq1200 = moreOrEq1200 @>

    [<Property>]
    let ``addResponse should properly handle OkStats and FailStats`` (latencies: (bool * uint32) list) =

        let mutable data = Step.StepExecutionData.createEmpty()

        let latencies =
            latencies
            |> List.filter(fun (_,latency) -> latency > 0u)
            |> List.map(fun (isOk,latency) -> isOk, latency |> int64)

        latencies
        |> Seq.iter(fun (isOk,latency) ->
            let clientResponse = {
                StatusCode = Nullable(); IsError = not isOk
                ErrorMessage = ""; SizeBytes = 10
                LatencyMs = 0.0; Payload = null
            }
            let stepResponse = {
                ClientResponse = clientResponse
                EndTimeTicks = % Int64.MaxValue
                LatencyTicks = % latency
            }
            data <- Step.StepExecutionData.addResponse data stepResponse
        )

        // calc OkStats
        let okLatencies = latencies |> Seq.filter(fun (isOk,_) -> isOk)
        let okCount = okLatencies |> Seq.length
        let okLessOrEq800 = okLatencies |> Seq.filter(fun (_,latency) -> latency <= 800L) |> Seq.length
        let okMinStats = if okCount > 0 then okLatencies |> Seq.map(snd) |> Seq.min else 0L
        let okMaxStats = if okCount > 0 then okLatencies |> Seq.map(snd) |> Seq.max else 0L

        // calc FailStats
        let failLatencies = latencies |> Seq.filter(fun (isOk,_) -> isOk = false)
        let failCount = failLatencies |> Seq.length
        let failLessOrEq800 = failLatencies |> Seq.filter(fun (_,latency) -> latency <= 800L) |> Seq.length
        let failMinStats = if failCount > 0 then failLatencies |> Seq.map(snd) |> Seq.min else 0L
        let failMaxStats = if failCount > 0 then failLatencies |> Seq.map(snd) |> Seq.max else 0L

        let okMin = if okCount > 0 then data.OkStats.LatencyHistogramTicks |> Histogram.min else 0L
        let okMax = if okCount > 0 then data.OkStats.LatencyHistogramTicks |> Histogram.max else 0L
        let failMin = if failCount > 0 then data.FailStats.LatencyHistogramTicks |> Histogram.min else 0L
        let failMax = if failCount > 0 then data.FailStats.LatencyHistogramTicks |> Histogram.max else 0L

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

        let mutable data = Step.StepExecutionData.createEmpty()

        let responseSizes =
            responseSizes
            |> List.filter(fun (_,resSize) -> resSize > 0u)
            |> List.map(fun (isOk,resSize) -> isOk, resSize |> int64)

        responseSizes
        |> Seq.iter(fun (isOk,resSize) ->
            let clientResponse = {
                StatusCode = Nullable(); IsError = not isOk
                ErrorMessage = ""; SizeBytes = int resSize
                LatencyMs = 1.0; Payload = null
            }
            let stepResponse = {
                ClientResponse = clientResponse
                EndTimeTicks = % Int64.MaxValue
                LatencyTicks = 0L<ticks>
            }
            data <- Step.StepExecutionData.addResponse data stepResponse
        )

        // calc OkStatsData
        let okResponses = responseSizes |> Seq.filter(fun (isOk,_) -> isOk)
        let okCount = okResponses |> Seq.length

        let okMinBytes = if okCount > 0 then okResponses |> Seq.map(snd) |> Seq.min else 0L
        let okMaxBytes = if okCount > 0 then okResponses |> Seq.map(snd) |> Seq.max else 0L
        let okAllMB    = if okCount > 0 then okResponses |> Seq.map(snd) |> Seq.sum |> UMX.tag |> fromBytesToMB else % 0.0

        // calc FailStatsData
        let failResponses = responseSizes |> Seq.filter(fun (isOk,_) -> isOk = false)
        let failCount = failResponses |> Seq.length

        let failMinBytes = if failCount > 0 then failResponses |> Seq.map(snd) |> Seq.min else 0L
        let failMaxBytes = if failCount > 0 then failResponses |> Seq.map(snd) |> Seq.max else 0L
        let failAllMB    = if failCount > 0 then failResponses |> Seq.map(snd) |> Seq.sum |> UMX.tag |> fromBytesToMB else % 0.0

        let okMin = if okCount > 0 then data.OkStats.DataTransferBytes |> Histogram.min else 0L
        let okMax = if okCount > 0 then data.OkStats.DataTransferBytes |> Histogram.max else 0L
        let failMin = if failCount > 0 then data.FailStats.DataTransferBytes |> Histogram.min else 0L
        let failMax = if failCount > 0 then data.FailStats.DataTransferBytes |> Histogram.max else 0L

        test <@ data.OkStats.RequestCount = okCount @>
        test <@ okMin = okMinBytes @>
        test <@ okMax = okMaxBytes @>
        test <@ data.OkStats.AllMB = okAllMB @>

        test <@ data.FailStats.RequestCount = failCount @>
        test <@ failMin = failMinBytes @>
        test <@ failMax = failMaxBytes @>
        test <@ data.FailStats.AllMB = failAllMB @>

[<Fact>]
let ``NodeStats should be calculated properly`` () =

    let okStep = Step.create("ok step", fun context -> task {
        do! Task.Delay(milliseconds 500)
        return Response.ok(sizeBytes = 100)
    })

    let failStep = Step.create("fail step 1", fun context -> task {
        if context.InvocationCount <= 2 then
            do! Task.Delay(milliseconds 50)
            return Response.ok(sizeBytes = 50)
        else
            do! Task.Delay(milliseconds 500)
            return Response.fail(error = "reason 1", statusCode = 10, sizeBytes = 10)
    })

    let scenario =
        Scenario.create "realtime stats scenario" [okStep; failStep]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 5)]

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.withReportFolder "./stats-tests/1/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        let scnStats = stats.ScenarioStats.[0]
        let st1 = scnStats.StepStats.[0]
        let st2 = scnStats.StepStats.[1]

        test <@ st1.Ok.Request.Count >= 4 && st1.Ok.Request.Count <= 6 @>
        test <@ st1.Ok.Request.RPS >= 1.0 && st1.Ok.Request.RPS <= 1.5 @>
        test <@ st1.Ok.Latency.MinMs <= 503.0 @>
        test <@ st1.Ok.Latency.MaxMs <= 515.0 @>
        test <@ st1.Ok.Latency.Percent50 <= 505.0 @>
        test <@ st1.Ok.Latency.LatencyCount.LessOrEq800 >= 4 && st1.Ok.Latency.LatencyCount.LessOrEq800 <= 6 @>
        test <@ st1.Ok.DataTransfer.MinKb = 0.1 @>
        test <@ st1.Ok.DataTransfer.Percent50 = 0.1 @>
        test <@ st1.Ok.DataTransfer.StdDev = 0.0 @>

        test <@ st1.Fail.Request.Count = 0 @>
        test <@ st1.Fail.Latency.MinMs = 0.0 @>
        test <@ st1.Fail.DataTransfer.MinKb = 0.0 @>

        test <@ st2.Ok.Request.Count = 2 @>
        test <@ st2.Ok.Request.RPS = 0.4 @>
        test <@ st2.Ok.Latency.MinMs <= 63.0 && st2.Ok.Latency.MinMs >= 50.0 @>
        test <@ st2.Ok.DataTransfer.MinKb = 0.05 @>

        test <@ st2.Fail.Request.Count >= 3 && st2.Fail.Request.Count <= 4 @>
        test <@ st2.Fail.Request.RPS >= 0.6 && st1.Fail.Request.RPS <= 0.8 @>
        test <@ st2.Fail.DataTransfer.MinKb = 0.01 @>
        test <@ st2.Fail.DataTransfer.AllMB = 0.0 @>

        test <@ scnStats.Duration = seconds 5 @>
        test <@ scnStats.RequestCount = scnStats.OkCount + scnStats.FailCount @>
        test <@ scnStats.OkCount = st1.Ok.Request.Count + st2.Ok.Request.Count @>
        test <@ scnStats.FailCount = st2.Fail.Request.Count @>
        test <@ scnStats.LatencyCount.LessOrEq800 = scnStats.RequestCount @>

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

    let stats = [
        { StatusCode = 50; IsError = false; Message = String.Empty; Count = 1 }
        { StatusCode = 80; IsError = false; Message = String.Empty; Count = 1 }
        { StatusCode = 10; IsError = false; Message = String.Empty; Count = 1 }
    ]

    let result =
        stats
        |> Stream.ofList
        |> StatusCodeStats.merge
        |> Stream.map(fun x -> x.StatusCode)
        |> Stream.toArray

    test <@ [| 10;50;80 |] = result @>
