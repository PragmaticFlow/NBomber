module internal NBomber.Domain.Statistics

open System
open System.Data

open HdrHistogram
open Nessos.Streams
open FSharp.UMX

open NBomber.Extensions.InternalExtensions
open NBomber.Contracts
open NBomber.Domain.DomainTypes

module Converter =

    let inline fromFloatBytesToKb (sizeBytes: float<bytes>) =
        if sizeBytes > 0.0<bytes> then (% sizeBytes) / 1024.0<kb>
        else 0.0<kb>

    let inline fromBytesToKb (sizeBytes: int64<bytes>) =
        if sizeBytes > 0L<bytes> then (% float sizeBytes) / 1024.0<kb>
        else 0.0<kb>

    let inline fromKbToMB (sizeKb: float<kb>) =
        if sizeKb > 0.0<kb> then (% sizeKb) / 1024.0<mb>
        else 0.0<mb>

    let fromBytesToMB = fromBytesToKb >> fromKbToMB

    let inline fromMsToTicks (ms: float<ms>) = (ms * float TimeSpan.TicksPerMillisecond) |> int64 |> UMX.tag<ticks>
    let inline fromTicksToMs (ticks: int64<ticks>) = (float ticks / float TimeSpan.TicksPerMillisecond) |> UMX.tag<ms>
    let inline fromFloatTicksToMs (ticks: float<ticks>) = (% ticks / float TimeSpan.TicksPerMillisecond) |> UMX.tag<ms>

    let inline round (digits: int) (value: float) = Math.Round(value, digits)

let inline min a b = if a < b then a else b
let inline max a b = if a > b then a else b

let calcRPS (requestCount: int) (executionTime: TimeSpan) =
    let totalSec = if executionTime.TotalSeconds < 1.0 then 1.0
                   else executionTime.TotalSeconds

    float requestCount / totalSec

module ErrorStats =

    let merge (stats: Stream<ErrorStats>) =
        stats
        |> Stream.groupBy(fun x -> x.ErrorCode)
        |> Stream.map(fun (code,errorStats) ->
            { ErrorCode = code
              Message = errorStats |> Seq.head |> fun x -> x.Message
              Count = errorStats |> Seq.sumBy(fun x -> x.Count) }
        )

module RequestStats =

    let create (stats: RawStepStats) (executionTime: TimeSpan) =
        { Count = stats.RequestCount
          RPS = calcRPS stats.LessOrEq1Sec executionTime }

    let merge (stats: Stream<RequestStats>) =
        { Count = stats |> Stream.sumBy(fun x -> x.Count)
          RPS = stats |> Stream.sumBy(fun x -> x.RPS) }

module LatencyStats =

    let create (stats: RawStepStats) =

        let inline ticksToMs (value: int64) = value |> UMX.tag<ticks> |> Converter.fromTicksToMs |> UMX.untag
        let inline floatTicksToMs (value: float) = value |> UMX.tag<ticks> |> Converter.fromFloatTicksToMs |> UMX.untag

        let latencies =
            if stats.LatencyHistogramTicks.TotalCount > 0L then ValueSome(stats.LatencyHistogramTicks.Copy())
            else ValueNone

        let minTicks = if % stats.MinTicks = Int64.MaxValue then 0L<ticks> else stats.MinTicks

        { MinMs = minTicks |> Converter.fromTicksToMs |> UMX.untag

          MeanMs = latencies
                   |> ValueOption.map(fun x -> x.GetMean() |> floatTicksToMs)
                   |> ValueOption.defaultValue 0.0

          MaxMs = stats.MaxTicks |> Converter.fromTicksToMs |> UMX.untag

          Percent50 = latencies
                      |> ValueOption.map(fun x -> x.GetValueAtPercentile(50.0) |> ticksToMs)
                      |> ValueOption.defaultValue 0.0

          Percent75 = latencies
                      |> ValueOption.map(fun x -> x.GetValueAtPercentile(75.0) |> ticksToMs)
                      |> ValueOption.defaultValue 0.0

          Percent95 = latencies
                      |> ValueOption.map(fun x -> x.GetValueAtPercentile(95.0) |> ticksToMs)
                      |> ValueOption.defaultValue 0.0

          Percent99 = latencies
                      |> ValueOption.map(fun x -> x.GetValueAtPercentile(99.0) |> ticksToMs)
                      |> ValueOption.defaultValue 0.0

          StdDev = latencies
                   |> ValueOption.map(fun x -> x.GetStdDeviation() |> floatTicksToMs)
                   |> ValueOption.defaultValue 0.0

          LatencyCount = { LessOrEq800 = stats.LessOrEq800; More800Less1200 = stats.More800Less1200; MoreOrEq1200 = stats.MoreOrEq1200 } }

    let merge (stats: Stream<LatencyStats>) =

        let latencyCount = {
            LessOrEq800 = stats |> Stream.sumBy(fun x -> x.LatencyCount.LessOrEq800)
            More800Less1200 = stats |> Stream.sumBy(fun x -> x.LatencyCount.More800Less1200)
            MoreOrEq1200 = stats |> Stream.sumBy(fun x -> x.LatencyCount.MoreOrEq1200)
        }

        { MinMs = stats |> Stream.map(fun x -> x.MinMs) |> Stream.minOrDefault 0.0
          MeanMs = stats |> Stream.map(fun x -> x.MeanMs) |> Stream.averageOrDefault 0.0
          MaxMs = stats |> Stream.map(fun x -> x.MaxMs) |> Stream.maxOrDefault 0.0
          Percent50 = stats |> Stream.map(fun x -> x.Percent50) |> Stream.averageOrDefault 0.0
          Percent75 = stats |> Stream.map(fun x -> x.Percent75) |> Stream.averageOrDefault 0.0
          Percent95 = stats |> Stream.map(fun x -> x.Percent95) |> Stream.averageOrDefault 0.0
          Percent99 = stats |> Stream.map(fun x -> x.Percent99) |> Stream.averageOrDefault 0.0
          StdDev = stats |> Stream.map(fun x -> x.StdDev) |> Stream.averageOrDefault 0.0
          LatencyCount = latencyCount }

module DataTransferStats =

    let create (stats: RawStepStats) =

        let inline bytesToKb (value: int64) = value |> UMX.tag<bytes> |> Converter.fromBytesToKb |> UMX.untag
        let inline floatBytesToKb (value: float) = value |> UMX.tag<bytes> |> Converter.fromFloatBytesToKb |> UMX.untag

        let dataTransfer =
            if stats.DataTransferBytes.TotalCount > 0L then ValueSome(stats.DataTransferBytes.Copy()) // we copy for safe enumeration
            else ValueNone

        let minBytes = if % stats.MinBytes = Int64.MaxValue then 0L<bytes> else stats.MinBytes

        { MinKb = minBytes |> Converter.fromBytesToKb |> UMX.untag

          MeanKb = dataTransfer
                   |> ValueOption.map(fun x -> x.GetMean() |> floatBytesToKb)
                   |> ValueOption.defaultValue 0.0

          MaxKb = stats.MaxBytes |> Converter.fromBytesToKb |> UMX.untag

          Percent50 = dataTransfer
                      |> ValueOption.map(fun x -> x.GetValueAtPercentile(50.0) |> bytesToKb)
                      |> ValueOption.defaultValue 0.0

          Percent75 = dataTransfer
                      |> ValueOption.map(fun x -> x.GetValueAtPercentile(75.0) |> bytesToKb)
                      |> ValueOption.defaultValue 0.0

          Percent95 = dataTransfer
                      |> ValueOption.map(fun x -> x.GetValueAtPercentile(95.0) |> bytesToKb)
                      |> ValueOption.defaultValue 0.0

          Percent99 = dataTransfer
                      |> ValueOption.map(fun x -> x.GetValueAtPercentile(99.0) |> bytesToKb)
                      |> ValueOption.defaultValue 0.0

          StdDev = dataTransfer
                   |> ValueOption.map(fun x -> x.GetStdDeviation() |> floatBytesToKb)
                   |> ValueOption.defaultValue 0.0

          AllMB = % stats.AllMB }

    let merge (stats: Stream<DataTransferStats>) =
        { MinKb = stats |> Stream.map(fun x -> x.MinKb) |> Stream.minOrDefault 0.0
          MeanKb = stats |> Stream.map(fun x -> x.MeanKb) |> Stream.averageOrDefault 0.0
          MaxKb = stats |> Stream.map(fun x -> x.MaxKb) |> Stream.maxOrDefault 0.0
          Percent50 = stats |> Stream.map(fun x -> x.Percent50) |> Stream.averageOrDefault 0.0
          Percent75 = stats |> Stream.map(fun x -> x.Percent75) |> Stream.averageOrDefault 0.0
          Percent95 = stats |> Stream.map(fun x -> x.Percent95) |> Stream.averageOrDefault 0.0
          Percent99 = stats |> Stream.map(fun x -> x.Percent99) |> Stream.averageOrDefault 0.0
          StdDev = stats |> Stream.map(fun x -> x.StdDev) |> Stream.averageOrDefault 0.0
          AllMB = stats |> Stream.sumBy(fun x -> x.AllMB) }

module StepStats =

    let create (stepName: string) (stepData: StepExecutionData) (duration: TimeSpan) =

        let okStats = {
            Request = RequestStats.create stepData.OkStats duration
            Latency = LatencyStats.create stepData.OkStats
            DataTransfer = DataTransferStats.create stepData.OkStats
        }

        let failStats = {
            Request = RequestStats.create stepData.FailStats duration
            Latency = LatencyStats.create stepData.FailStats
            DataTransfer = DataTransferStats.create stepData.FailStats
            ErrorStats = stepData.ErrorStats.Values |> Stream.ofSeq |> Stream.toArray  // we use Stream for safe enumeration
        }

        { StepName = stepName
          Ok = okStats
          Fail = failStats }

    let mergeErrorStats (stepsStats: Stream<FailStepStats>) =
        stepsStats
        |> Stream.collect(fun x -> x.ErrorStats |> Stream.ofArray)
        |> ErrorStats.merge
        |> Stream.toArray

    let merge (stepsStats: Stream<StepStats>) =

        let mergeOkStats (stats: Stream<OkStepStats>) =
            { Request = stats |> Stream.map(fun x -> x.Request) |> RequestStats.merge
              Latency = stats |> Stream.map(fun x -> x.Latency) |> LatencyStats.merge
              DataTransfer = stats |> Stream.map(fun x -> x.DataTransfer) |> DataTransferStats.merge }

        let mergeFailStats (stats: Stream<FailStepStats>) =
            { Request = stats |> Stream.map(fun x -> x.Request) |> RequestStats.merge
              Latency = stats |> Stream.map(fun x -> x.Latency) |> LatencyStats.merge
              DataTransfer = stats |> Stream.map(fun x -> x.DataTransfer) |> DataTransferStats.merge
              ErrorStats = stats |> mergeErrorStats }

        stepsStats
        |> Stream.groupBy(fun x -> x.StepName)
        |> Stream.map(fun (name, stats) ->
            let statsStream = stats |> Stream.ofSeq
            { StepName = name
              Ok = statsStream |> Stream.map(fun x -> x.Ok) |> mergeOkStats
              Fail = statsStream |> Stream.map(fun x -> x.Fail) |> mergeFailStats }
        )

module ScenarioStats =

    let create (scenario: Scenario) (simulationStats: LoadSimulationStats)
               (duration: TimeSpan) (allStepsStats: Stream<StepStats>) =

        let createByStepStats (scnName: string) (duration: TimeSpan)
                              (simulationStats: LoadSimulationStats)
                              (mergedStats: Stream<StepStats>) =

            let less800 = mergedStats |> Stream.sumBy(fun x -> x.Ok.Latency.LatencyCount.LessOrEq800 + x.Fail.Latency.LatencyCount.LessOrEq800)
            let more800Less1200 = mergedStats |> Stream.sumBy(fun x -> x.Ok.Latency.LatencyCount.More800Less1200 + x.Fail.Latency.LatencyCount.More800Less1200)
            let more1200 = mergedStats |> Stream.sumBy(fun x -> x.Ok.Latency.LatencyCount.MoreOrEq1200 + x.Fail.Latency.LatencyCount.MoreOrEq1200)

            { ScenarioName = scnName
              RequestCount = mergedStats |> Stream.sumBy(fun x -> x.Ok.Request.Count + x.Fail.Request.Count)
              OkCount = mergedStats |> Stream.sumBy(fun x -> x.Ok.Request.Count)
              FailCount = mergedStats |> Stream.sumBy(fun x -> x.Fail.Request.Count)
              AllDataMB = mergedStats |> Stream.sumBy(fun x -> x.Ok.DataTransfer.AllMB + x.Fail.DataTransfer.AllMB)
              StepStats = mergedStats |> Stream.toArray
              LatencyCount = { LessOrEq800 = less800; More800Less1200 = more800Less1200; MoreOrEq1200 = more1200 }
              LoadSimulationStats = simulationStats
              Duration = duration
              ErrorStats = mergedStats |> Stream.map(fun x -> x.Fail) |> StepStats.mergeErrorStats }

        allStepsStats
        |> StepStats.merge
        |> createByStepStats scenario.ScenarioName duration simulationStats

module NodeStats =

    let create (testInfo: TestInfo) (nodeInfo: NodeInfo)
               (scnStats: Stream<ScenarioStats>)
               (pluginStats: Stream<DataSet>) =

        { RequestCount = scnStats |> Stream.sumBy(fun x -> x.RequestCount)
          OkCount = scnStats |> Stream.sumBy(fun x -> x.OkCount)
          FailCount = scnStats |> Stream.sumBy(fun x -> x.FailCount)
          AllDataMB = scnStats |> Stream.sumBy(fun x -> x.AllDataMB)
          ScenarioStats = scnStats |> Stream.toArray
          PluginStats = pluginStats |> Stream.toArray
          NodeInfo = nodeInfo
          TestInfo = testInfo
          ReportFiles = Array.empty }

    let roundStats (stats: NodeStats) =

        let roundRequest (stats: RequestStats) =
            { stats with RPS = stats.RPS |> Converter.round 1 }

        let roundLatency (stats: LatencyStats) =
            { stats with MinMs = stats.MinMs |> Converter.round 3
                         MeanMs = stats.MeanMs |> Converter.round 3
                         MaxMs = stats.MaxMs |> Converter.round 3
                         Percent50 = stats.Percent50 |> Converter.round 3
                         Percent75 = stats.Percent75 |> Converter.round 3
                         Percent95 = stats.Percent95 |> Converter.round 3
                         Percent99 = stats.Percent99 |> Converter.round 3
                         StdDev = stats.StdDev |> Converter.round 3 }

        let roundDataTransfer (stats: DataTransferStats) =
            { stats with MinKb = stats.MinKb |> Converter.round 3
                         MeanKb = stats.MeanKb |> Converter.round 3
                         MaxKb = stats.MaxKb |> Converter.round 3
                         Percent50 = stats.Percent50 |> Converter.round 3
                         Percent75 = stats.Percent75 |> Converter.round 3
                         Percent95 = stats.Percent95 |> Converter.round 3
                         Percent99 = stats.Percent99 |> Converter.round 3
                         StdDev = stats.StdDev |> Converter.round 3
                         AllMB = stats.AllMB |> Converter.round 3 }

        let roundStepStats (stats: StepStats) =
            { stats with Ok = { stats.Ok with Request = stats.Ok.Request |> roundRequest
                                              Latency = stats.Ok.Latency |> roundLatency
                                              DataTransfer = stats.Ok.DataTransfer |> roundDataTransfer }

                         Fail = { stats.Fail with Request = stats.Fail.Request |> roundRequest
                                                  Latency = stats.Fail.Latency |> roundLatency
                                                  DataTransfer = stats.Fail.DataTransfer |> roundDataTransfer } }

        let roundScenarioStats (stats: ScenarioStats) =
            { stats with AllDataMB = stats.AllDataMB |> Converter.round 3
                         StepStats = stats.StepStats |> Array.map(roundStepStats) }

        { stats with AllDataMB = stats.AllDataMB |> Converter.round 3
                     ScenarioStats = stats.ScenarioStats |> Array.map(roundScenarioStats)  }
