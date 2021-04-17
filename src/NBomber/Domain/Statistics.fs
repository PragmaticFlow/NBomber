module internal NBomber.Domain.Statistics

open System
open System.Collections.Generic
open System.Data

open HdrHistogram
open Nessos.Streams
open FSharp.UMX

open NBomber
open NBomber.Extensions.InternalExtensions
open NBomber.Contracts
open NBomber.Domain.DomainTypes

module Converter =

    let inline fromMsToTicks (ms: float<ms>) = (ms * float TimeSpan.TicksPerMillisecond) |> int64 |> UMX.tag<ticks>
    let inline fromTicksToMs (ticks: int64<ticks>) = (float ticks / float TimeSpan.TicksPerMillisecond) |> UMX.tag<ms>
    let inline fromFloatTicksToMs (ticks: float<ticks>) = (% ticks / float TimeSpan.TicksPerMillisecond) |> UMX.tag<ms>
    let inline fromBytesToKb (bytes) = Math.Round(float bytes / 1024.0, 2)
    let inline fromBytesToMb (bytes) = Math.Round(decimal bytes / 1024.0M / 1024.0M, 2)
    let inline fromBytesToMb4 (bytes) = Math.Round(decimal bytes / 1024.0M / 1024.0M, 4)
    let inline round (digits: int) (value: float) = Math.Round(value, digits)

module Histogram =

    let min (histogram: HistogramBase) = histogram.RecordedValues() |> Seq.head |> fun x -> x.ValueIteratedTo
    let mean (histogram: HistogramBase) = histogram.GetMean()
    let max (histogram: HistogramBase) = histogram.GetMaxValue()
    let getPercentile (percentile: float) (histogram: HistogramBase) = histogram.GetValueAtPercentile(percentile)
    let stdDev (histogram: HistogramBase) = histogram.GetStdDeviation()

let calcRPS (requestCount: int) (executionTime: TimeSpan) =
    let totalSec = if executionTime.TotalSeconds < 1.0 then 1.0
                   else executionTime.TotalSeconds

    float requestCount / totalSec

module RequestStats =

    let create (stats: RawStepStats) (executionTime: TimeSpan) =
        { Count = stats.RequestCount
          RPS = calcRPS stats.RequestCount executionTime }

    let merge (stats: Stream<RequestStats>) =
        { Count = stats |> Stream.sumBy(fun x -> x.Count)
          RPS = stats |> Stream.sumBy(fun x -> x.RPS) }

    let round (stats: RequestStats) =
        { stats with RPS = stats.RPS |> Converter.round 1 }

module LatencyStats =

    let create (stats: RawStepStats) =

        let inline ticksToMs (value: int64) = value |> UMX.tag<ticks> |> Converter.fromTicksToMs |> UMX.untag
        let inline floatTicksToMs (value: float) = value |> UMX.tag<ticks> |> Converter.fromFloatTicksToMs |> UMX.untag

        let latencies =
            if stats.LatencyHistogramTicks.TotalCount > 0L then ValueSome(stats.LatencyHistogramTicks.Copy())
            else ValueNone

        { MinMs  = if latencies.IsSome then latencies.Value |> Histogram.min |> ticksToMs else 0.0
          MeanMs = if latencies.IsSome then latencies.Value |> Histogram.mean |> floatTicksToMs else 0.0
          MaxMs  = if latencies.IsSome then latencies.Value |> Histogram.max |> ticksToMs else 0.0
          Percent50 = if latencies.IsSome then latencies.Value |> Histogram.getPercentile(50.0) |> ticksToMs else 0.0
          Percent75 = if latencies.IsSome then latencies.Value |> Histogram.getPercentile(75.0) |> ticksToMs else 0.0
          Percent95 = if latencies.IsSome then latencies.Value |> Histogram.getPercentile(95.0) |> ticksToMs else 0.0
          Percent99 = if latencies.IsSome then latencies.Value |> Histogram.getPercentile(99.0) |> ticksToMs else 0.0
          StdDev    = if latencies.IsSome then latencies.Value |> Histogram.stdDev |> floatTicksToMs else 0.0
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

    let round (stats: LatencyStats) =
        { stats with MinMs = stats.MinMs |> Converter.round(Constants.StatsRounding)
                     MeanMs = stats.MeanMs |> Converter.round(Constants.StatsRounding)
                     MaxMs = stats.MaxMs |> Converter.round(Constants.StatsRounding)
                     Percent50 = stats.Percent50 |> Converter.round(Constants.StatsRounding)
                     Percent75 = stats.Percent75 |> Converter.round(Constants.StatsRounding)
                     Percent95 = stats.Percent95 |> Converter.round(Constants.StatsRounding)
                     Percent99 = stats.Percent99 |> Converter.round(Constants.StatsRounding)
                     StdDev = stats.StdDev |> Converter.round(Constants.StatsRounding) }

module DataTransferStats =

    let create (stats: RawStepStats) =

        let dataTransfer =
            if stats.DataTransferBytes.TotalCount > 0L then ValueSome(stats.DataTransferBytes.Copy()) // we copy for safe enumeration
            else ValueNone

        { MinBytes  = if dataTransfer.IsSome then dataTransfer.Value |> Histogram.min |> int else 0
          MeanBytes = if dataTransfer.IsSome then dataTransfer.Value |> Histogram.mean |> int else 0
          MaxBytes  = if dataTransfer.IsSome then dataTransfer.Value |> Histogram.max |> int else 0
          Percent50 = if dataTransfer.IsSome then dataTransfer.Value |> Histogram.getPercentile(50.0) |> int else 0
          Percent75 = if dataTransfer.IsSome then dataTransfer.Value |> Histogram.getPercentile(75.0) |> int else 0
          Percent95 = if dataTransfer.IsSome then dataTransfer.Value |> Histogram.getPercentile(95.0) |> int else 0
          Percent99 = if dataTransfer.IsSome then dataTransfer.Value |> Histogram.getPercentile(99.0) |> int else 0
          StdDev    = if dataTransfer.IsSome then dataTransfer.Value |> Histogram.stdDev else 0.0
          AllBytes     = % stats.AllBytes }

    let merge (stats: Stream<DataTransferStats>) =
        { MinBytes = stats |> Stream.map(fun x -> x.MinBytes) |> Stream.minOrDefault 0
          MeanBytes = stats |> Stream.map(fun x -> float x.MeanBytes) |> Stream.averageOrDefault 0.0 |> int
          MaxBytes = stats |> Stream.map(fun x -> x.MaxBytes) |> Stream.maxOrDefault 0
          Percent50 = stats |> Stream.map(fun x -> float x.Percent50) |> Stream.averageOrDefault 0.0 |> int
          Percent75 = stats |> Stream.map(fun x -> float x.Percent75) |> Stream.averageOrDefault 0.0 |> int
          Percent95 = stats |> Stream.map(fun x -> float x.Percent95) |> Stream.averageOrDefault 0.0 |> int
          Percent99 = stats |> Stream.map(fun x -> float x.Percent99) |> Stream.averageOrDefault 0.0 |> int
          StdDev = stats |> Stream.map(fun x -> x.StdDev) |> Stream.averageOrDefault 0.0
          AllBytes = stats |> Stream.sumBy(fun x -> x.AllBytes) }

    let round (stats: DataTransferStats) =
        { stats with StdDev = stats.StdDev |> Converter.round(Constants.StatsRounding) }

module StatusCodeStats =

    let create (stats: Dictionary<int,StatusCodeStats>) =
        stats.Values |> Stream.ofSeq |> Stream.toArray // we use Stream for safe enumeration

    let merge (stats: Stream<StatusCodeStats>) =
        stats
        |> Stream.groupBy(fun x -> x.StatusCode)
        |> Stream.sortBy(fun (code,codeStats) -> code)
        |> Stream.map(fun (code,codeStats) ->
            { StatusCode = code
              IsError = codeStats |> Seq.head |> fun x -> x.IsError
              Message = codeStats |> Seq.head |> fun x -> x.Message
              Count = codeStats |> Seq.sumBy(fun x -> x.Count) }
        )

module StepStats =

    let create (stepName: string) (stepData: StepExecutionData) (duration: TimeSpan) =

        let okStats: OkStepStats = {
            Request = RequestStats.create stepData.OkStats duration
            Latency = LatencyStats.create stepData.OkStats
            DataTransfer = DataTransferStats.create stepData.OkStats
            StatusCodes = StatusCodeStats.create stepData.OkStats.StatusCodes
        }

        let failStats: FailStepStats = {
            Request = RequestStats.create stepData.FailStats duration
            Latency = LatencyStats.create stepData.FailStats
            DataTransfer = DataTransferStats.create stepData.FailStats
            StatusCodes = StatusCodeStats.create stepData.FailStats.StatusCodes
        }

        { StepName = stepName
          Ok = okStats
          Fail = failStats }

    let merge (stepsStats: Stream<StepStats>) =

        let mergeOkStepStats (stats: Stream<OkStepStats>): OkStepStats =
            { Request = stats |> Stream.map(fun x -> x.Request) |> RequestStats.merge
              Latency = stats |> Stream.map(fun x -> x.Latency) |> LatencyStats.merge
              DataTransfer = stats |> Stream.map(fun x -> x.DataTransfer) |> DataTransferStats.merge
              StatusCodes = stats |> Stream.collect(fun x -> x.StatusCodes |> Stream.ofArray) |> StatusCodeStats.merge |> Stream.toArray }

        let mergeFailStepStats (stats: Stream<FailStepStats>): FailStepStats =
            { Request = stats |> Stream.map(fun x -> x.Request) |> RequestStats.merge
              Latency = stats |> Stream.map(fun x -> x.Latency) |> LatencyStats.merge
              DataTransfer = stats |> Stream.map(fun x -> x.DataTransfer) |> DataTransferStats.merge
              StatusCodes = stats |> Stream.collect(fun x -> x.StatusCodes |> Stream.ofArray) |> StatusCodeStats.merge |> Stream.toArray }

        stepsStats
        |> Stream.groupBy(fun x -> x.StepName)
        |> Stream.map(fun (name,stats) ->
            let statsStream = stats |> Stream.ofSeq
            { StepName = name
              Ok = statsStream |> Stream.map(fun x -> x.Ok) |> mergeOkStepStats
              Fail = statsStream |> Stream.map(fun x -> x.Fail) |> mergeFailStepStats }
        )

    let round (stats: StepStats) =
        { stats with Ok = { stats.Ok with Request = stats.Ok.Request |> RequestStats.round
                                          Latency = stats.Ok.Latency |> LatencyStats.round
                                          DataTransfer = stats.Ok.DataTransfer |> DataTransferStats.round }

                     Fail = { stats.Fail with Request = stats.Fail.Request |> RequestStats.round
                                              Latency = stats.Fail.Latency |> LatencyStats.round
                                              DataTransfer = stats.Fail.DataTransfer |> DataTransferStats.round } }

module ScenarioStats =

    let create (scenario: Scenario) (simulationStats: LoadSimulationStats)
               (duration: TimeSpan) (currentOperation: OperationType)
               (allStepsStats: Stream<StepStats>) =

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
              AllBytes = mergedStats |> Stream.sumBy(fun x -> x.Ok.DataTransfer.AllBytes + x.Fail.DataTransfer.AllBytes)
              StepStats = mergedStats |> Stream.toArray
              LatencyCount = { LessOrEq800 = less800; More800Less1200 = more800Less1200; MoreOrEq1200 = more1200 }
              LoadSimulationStats = simulationStats

              StatusCodes = mergedStats
                            |> Stream.collect(fun x -> x.Ok.StatusCodes
                                                       |> Array.append(x.Fail.StatusCodes)
                                                       |> Stream.ofArray)
                            |> StatusCodeStats.merge
                            |> Stream.toArray

              CurrentOperation = currentOperation
              Duration = duration }

        allStepsStats
        |> StepStats.merge
        |> createByStepStats scenario.ScenarioName duration simulationStats

    let round (stats: ScenarioStats) =
        { stats with StepStats = stats.StepStats |> Array.map(StepStats.round)
                     Duration = TimeSpan(stats.Duration.Days, stats.Duration.Hours, stats.Duration.Minutes, stats.Duration.Seconds) }

module NodeStats =

    let create (testInfo: TestInfo) (nodeInfo: NodeInfo)
               (scnStats: Stream<ScenarioStats>) (pluginStats: DataSet[]) =

        let maxDuration = scnStats |> Stream.maxBy(fun x -> x.Duration) |> fun scn -> scn.Duration

        { RequestCount = scnStats |> Stream.sumBy(fun x -> x.RequestCount)
          OkCount = scnStats |> Stream.sumBy(fun x -> x.OkCount)
          FailCount = scnStats |> Stream.sumBy(fun x -> x.FailCount)
          AllBytes = scnStats |> Stream.sumBy(fun x -> x.AllBytes)
          ScenarioStats = scnStats |> Stream.toArray
          PluginStats = pluginStats
          NodeInfo = nodeInfo
          TestInfo = testInfo
          ReportFiles = Array.empty
          Duration = maxDuration }

    let round (stats: NodeStats) =
        { stats with ScenarioStats = stats.ScenarioStats |> Array.map(ScenarioStats.round)
                     Duration = TimeSpan(stats.Duration.Days, stats.Duration.Hours, stats.Duration.Minutes, stats.Duration.Seconds) }
