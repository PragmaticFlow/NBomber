module Tests.Statistics

open System
open System.Threading.Tasks

open FSharp.UMX
open Xunit
open FsCheck.Xunit
open Swensen.Unquote
open FSharp.Control.Tasks.NonAffine

open NBomber.Contracts
open NBomber.Domain
open NBomber.Domain.DomainTypes
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
        let stepResMs = 200L

        let clientResponse = { Payload = (); SizeBytes = 10L; Exception = None
                               ErrorCode = 0; LatencyMs = clientResMs }
        let stepResponse = {
            ClientResponse = clientResponse
            EndTimeTicks = % Int64.MaxValue
            LatencyTicks = % stepResMs
        }

        let data = Step.StepExecutionData.addResponse emptyData stepResponse
        let minTicks =
            if clientResMs > 0.0 then clientResMs |> UMX.tag |> fromMsToTicks
            else stepResMs |> UMX.tag

        test <@ data.OkStats.MinTicks = minTicks @>

    [<Property>]
    let ``addResponse should properly calc latency count`` (latencies: uint32 list) =

        let mutable data = Step.StepExecutionData.createEmpty()

        let latencies = latencies |> List.filter(fun x -> x > 0u)
        latencies
        |> Seq.iter(fun latency ->
            let clientResponse = {
                Payload = (); SizeBytes = 10L; Exception = None
                ErrorCode = 0; LatencyMs = float latency
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
        let lessOrEq1Sec = latencies |> Seq.filter(fun x -> x <= 1000u) |> Seq.length

        test <@ data.OkStats.LessOrEq800 = lessOrEq800 @>
        test <@ data.OkStats.More800Less1200 = more800Less1200 @>
        test <@ data.OkStats.MoreOrEq1200 = moreOrEq1200 @>
        test <@ data.OkStats.LessOrEq1Sec = lessOrEq1Sec @>

    [<Property>]
    let ``addResponse should properly handle OkStats and FailStats`` (latencies: (bool * uint32) list) =

        let mutable data = Step.StepExecutionData.createEmpty()

        let latencies =
            latencies
            |> List.filter(fun (_,latency) -> latency > 0u)
            |> List.map(fun (isOk,latency) -> isOk, latency |> float |> UMX.tag<ms>)

        latencies
        |> Seq.iter(fun (isOk,latency) ->
            let clientResponse = {
                Payload = (); SizeBytes = 10L
                Exception = if isOk then None else Some(Exception())
                ErrorCode = 0; LatencyMs = % latency
            }
            let stepResponse = {
                ClientResponse = clientResponse
                EndTimeTicks = % Int64.MaxValue
                LatencyTicks = 0L<ticks>
            }
            data <- Step.StepExecutionData.addResponse data stepResponse
        )

        // calc OkStats
        let okLatencies = latencies |> Seq.filter(fun (isOk,_) -> isOk)
        let okCount = okLatencies |> Seq.length
        let okLessOrEq800 = okLatencies |> Seq.filter(fun (_,latency) -> latency <= 800.0<ms>) |> Seq.length

        let okMin =
            if okCount > 0 then okLatencies |> Seq.map(snd >> fromMsToTicks) |> Seq.min
            else % Int64.MaxValue
        let okMax =
            if okCount > 0 then okLatencies |> Seq.map(snd >> fromMsToTicks) |> Seq.max
            else 0L<ticks>

        // calc FailStats
        let failLatencies = latencies |> Seq.filter(fun (isOk,_) -> isOk = false)
        let failCount = failLatencies |> Seq.length
        let failLessOrEq800 = failLatencies |> Seq.filter(fun (_,latency) -> latency <= 800.0<ms>) |> Seq.length

        let failMin =
            if failCount > 0 then failLatencies |> Seq.map(snd >> fromMsToTicks) |> Seq.min
            else % Int64.MaxValue
        let failMax =
            if failCount > 0 then failLatencies |> Seq.map(snd >> fromMsToTicks) |> Seq.max
            else 0L<ticks>

        test <@ data.OkStats.RequestCount = okCount @>
        test <@ data.OkStats.LessOrEq800 = okLessOrEq800 @>
        test <@ data.OkStats.MinTicks = okMin @>
        test <@ data.OkStats.MaxTicks = okMax @>

        test <@ data.FailStats.RequestCount = failCount @>
        test <@ data.FailStats.LessOrEq800 = failLessOrEq800 @>
        test <@ data.FailStats.MinTicks = failMin @>
        test <@ data.FailStats.MaxTicks = failMax @>

    [<Property>]
    let ``addResponse should properly calc response sizes`` (responseSizes: (bool * uint32) list) =

        let mutable data = Step.StepExecutionData.createEmpty()

        let responseSizes =
            responseSizes
            |> List.filter(fun (_,resSize) -> resSize > 0u)
            |> List.map(fun (isOk,resSize) -> isOk, resSize |> int64 |> UMX.tag<bytes>)

        responseSizes
        |> Seq.iter(fun (isOk,resSize) ->
            let clientResponse = {
                Payload = (); SizeBytes = int64 resSize
                Exception = if isOk then None else Some(Exception())
                ErrorCode = 0; LatencyMs = 1.0
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

        let okMinBytes =
            if okCount > 0 then okResponses |> Seq.map(snd) |> Seq.min
            else % Int64.MaxValue
        let okMaxBytes =
            if okCount > 0 then okResponses |> Seq.map(snd) |> Seq.max
            else 0L<bytes>
        let okAllMB =
            if okCount > 0 then okResponses |> Seq.map(snd) |> Seq.sum |> fromBytesToMB
            else 0.0<mb>

        // calc FailStatsData
        let failResponses = responseSizes |> Seq.filter(fun (isOk,_) -> isOk = false)
        let failCount = failResponses |> Seq.length

        let failMinBytes =
            if failCount > 0 then failResponses |> Seq.map(snd) |> Seq.min
            else % Int64.MaxValue
        let failMaxBytes =
            if failCount > 0 then failResponses |> Seq.map(snd) |> Seq.max
            else 0L<bytes>
        let failAllMB =
            if failCount > 0 then failResponses |> Seq.map(snd) |> Seq.sum |> fromBytesToMB
            else 0.0<mb>

        test <@ data.OkStats.RequestCount = okCount @>
        test <@ data.OkStats.MinBytes = okMinBytes @>
        test <@ data.OkStats.MaxBytes = okMaxBytes @>
        test <@ data.OkStats.AllMB = okAllMB @>

        test <@ data.FailStats.RequestCount = failCount @>
        test <@ data.FailStats.MinBytes = failMinBytes @>
        test <@ data.FailStats.MaxBytes = failMaxBytes @>
        test <@ data.FailStats.AllMB = failAllMB @>

[<Fact>]
let ``ErrorStats should be calculated properly`` () =

    let okStep = Step.createAsync("ok step", fun _ -> task {
        do! Task.Delay(milliseconds 100)
        return Response.ok()
    })

    let failStep1 = Step.createAsync("fail step 1", fun context -> task {
        do! Task.Delay(milliseconds 10)
        return if context.InvocationCount <= 10 then Response.fail(reason = "reason 1", errorCode = 10)
               else Response.ok()
    })

    let failStep2 = Step.createAsync("fail step 2", fun context -> task {
        do! Task.Delay(milliseconds 10)
        return if context.InvocationCount <= 30 then Response.fail(reason = "reason 2", errorCode = 20)
               else Response.ok()
    })

    let scenario =
        Scenario.create "realtime stats scenario" [okStep; failStep1; failStep2]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [
            KeepConstant(copies = 2, during = seconds 10)
        ]

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.withReportFolder "./stats-tests/1/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        let allErrorStats = stats.ScenarioStats.[0].ErrorStats
        let fail1Stats = stats.ScenarioStats.[0].StepStats.[1].Fail.ErrorStats
        let fail2Stats = stats.ScenarioStats.[0].StepStats.[2].Fail.ErrorStats

        test <@ allErrorStats.Length = 2 @>
        test <@ fail1Stats.Length = 1 @>
        test <@ fail2Stats.Length = 1 @>

        test <@ fail1Stats
                |> Seq.find(fun x -> x.ErrorCode = 10)
                |> fun error -> error.Count = 20 @>

        test <@ fail2Stats
                |> Seq.find(fun x -> x.ErrorCode = 20)
                |> fun error -> error.Count = 60 @>

[<Fact>]
let ``NodeStats should be calculated properly`` () =

    let okStep = Step.createAsync("ok step", fun context -> task {
        do! Task.Delay(milliseconds 500)
        return Response.ok(sizeBytes = 100L)
    })

    let failStep = Step.createAsync("fail step 1", fun context -> task {
        if context.InvocationCount <= 2 then
            do! Task.Delay(milliseconds 50)
            return Response.ok(sizeBytes = 50L)
        else
            do! Task.Delay(milliseconds 500)
            return Response.fail(reason = "reason 1", errorCode = 10, sizeBytes = 10L)
    })

    let scenario =
        Scenario.create "realtime stats scenario" [okStep; failStep]
        |> Scenario.withoutWarmUp
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 5)]

    NBomberRunner.registerScenarios [scenario]
    |> NBomberRunner.withReportFolder "./stats-tests/2/"
    |> NBomberRunner.run
    |> Result.getOk
    |> fun stats ->
        let scnStats = stats.ScenarioStats.[0]
        let st1 = scnStats.StepStats.[0]
        let st2 = scnStats.StepStats.[1]

        test <@ st1.Ok.Request.Count >= 4 && st1.Ok.Request.Count <= 6 @>
        test <@ st1.Ok.Request.RPS >= 1.0 && st1.Ok.Request.RPS <= 1.5 @>
        test <@ st1.Ok.Latency.MinMs <= 510.0 @>
        test <@ st1.Ok.Latency.MaxMs <= 525.0 @>
        test <@ st1.Ok.Latency.Percent50 <= 515.0 @>
        test <@ st1.Ok.Latency.LatencyCount.LessOrEq800 >= 4 && st1.Ok.Latency.LatencyCount.LessOrEq800 <= 6 @>
        test <@ st1.Ok.DataTransfer.MinKb = 0.098 @>
        test <@ st1.Ok.DataTransfer.Percent50 = 0.098 @>
        test <@ st1.Ok.DataTransfer.StdDev = 0.0 @>

        test <@ st1.Fail.Request.Count = 0 @>
        test <@ st1.Fail.Latency.MinMs = 0.0 @>
        test <@ st1.Fail.DataTransfer.MinKb = 0.0 @>

        test <@ st2.Ok.Request.Count = 2 @>
        test <@ st2.Ok.Request.RPS = 0.4 @>
        test <@ st2.Ok.Latency.MinMs <= 55.0 @>
        test <@ st2.Ok.DataTransfer.MinKb = 0.049 @>

        test <@ st2.Fail.Request.Count >= 3 && st2.Fail.Request.Count <= 4 @>
        test <@ st2.Fail.Request.RPS >= 0.6 && st1.Fail.Request.RPS <= 0.8 @>
        test <@ st2.Fail.DataTransfer.MinKb = 0.01 @>
        test <@ st2.Fail.DataTransfer.AllMB = 0.0 @>

        test <@ scnStats.Duration = seconds 5 @>
        test <@ scnStats.RequestCount = scnStats.OkCount + scnStats.FailCount @>
        test <@ scnStats.OkCount = st1.Ok.Request.Count + st2.Ok.Request.Count @>
        test <@ scnStats.FailCount = st2.Fail.Request.Count @>
        test <@ scnStats.LatencyCount.LessOrEq800 = scnStats.RequestCount @>
