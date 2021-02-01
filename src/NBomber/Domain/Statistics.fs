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

    [<Literal>]
    let private TicksPerMillisecond = TimeSpan.TicksPerMillisecond

    let inline fromBytesToKb (sizeBytes: float<bytes>) =
        if sizeBytes > 0.0<bytes> then UMX.tag<kb>(sizeBytes / 1024.0<bytes>)
        else 0.0<kb>

    let inline fromKbToMB (sizeKb: float<kb>) =
        if sizeKb > 0.0<kb> then UMX.tag<mb>(sizeKb / 1024.0<kb>)
        else 0.0<mb>

    let fromBytesToMB = fromBytesToKb >> fromKbToMB

    let inline fromMsToTicks (ms: float<ms>) = (ms * float TicksPerMillisecond) |> UMX.untag |> UMX.tag<ticks>
    let inline fromTicksToMs (ticks: float<ticks>) = (ticks / float TicksPerMillisecond) |> UMX.untag |> UMX.tag<ms>

    let roundResult (value: float) =
        let result = Math.Round(value, 2)
        if result > 0.01 then result
        else Math.Round(value, 4)
        |> UMX.tag

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
          RPS = calcRPS stats.RequestLessSecCount executionTime }

    let merge (stats: Stream<RequestStats>) =
        { Count = stats |> Stream.sumBy(fun x -> x.Count)
          RPS = stats |> Stream.sumBy(fun x -> x.RPS) |> Converter.roundResult }

module LatencyStats =

    let create (stats: RawStepStats) =

        let inline toMs (value: float) = value |> UMX.tag<ticks> |> Converter.fromTicksToMs |> UMX.untag

        let latencies =
            if stats.LatencyHistogramTicks.TotalCount > 0L then ValueSome(stats.LatencyHistogramTicks.Copy())
            else ValueNone

        let minTicks = if % stats.MinTicks = Double.MaxValue then 0.0<ticks> else stats.MinTicks

        { MinMs = minTicks |> Converter.fromTicksToMs |> UMX.untag

          MeanMs = latencies
                   |> ValueOption.map(fun x -> x.GetMean() |> toMs)
                   |> ValueOption.defaultValue 0.0

          MaxMs = stats.MaxTicks |> Converter.fromTicksToMs |> UMX.untag

          Percent50 = latencies
                      |> ValueOption.map(fun x -> x.GetValueAtPercentile(50.0) |> float |> toMs)
                      |> ValueOption.defaultValue 0.0

          Percent75 = latencies
                      |> ValueOption.map(fun x -> x.GetValueAtPercentile(75.0) |> float |> toMs)
                      |> ValueOption.defaultValue 0.0

          Percent95 = latencies
                      |> ValueOption.map(fun x -> x.GetValueAtPercentile(95.0) |> float |> toMs)
                      |> ValueOption.defaultValue 0.0

          Percent99 = latencies
                      |> ValueOption.map(fun x -> x.GetValueAtPercentile(99.0) |> float |> toMs)
                      |> ValueOption.defaultValue 0.0

          StdDev = latencies
                   |> ValueOption.map(fun x -> x.GetStdDeviation() |> toMs |> Converter.roundResult)
                   |> ValueOption.defaultValue 0.0

          LatencyCount = { Less800 = stats.Less800; More800Less1200 = stats.More800Less1200; More1200 = stats.More1200 } }

    let merge (stats: Stream<LatencyStats>) =

        let latencyCount = {
            Less800 = stats |> Stream.sumBy(fun x -> x.LatencyCount.Less800)
            More800Less1200 = stats |> Stream.sumBy(fun x -> x.LatencyCount.More800Less1200)
            More1200 = stats |> Stream.sumBy(fun x -> x.LatencyCount.More1200)
        }

        { MinMs = stats |> Stream.map(fun x -> x.MinMs) |> Stream.minOrDefault 0.0 |> Converter.roundResult
          MeanMs = stats |> Stream.map(fun x -> x.MeanMs) |> Stream.averageOrDefault 0.0 |> Converter.roundResult
          MaxMs = stats |> Stream.map(fun x -> x.MaxMs) |> Stream.maxOrDefault 0.0 |> Converter.roundResult
          Percent50 = stats |> Stream.map(fun x -> x.Percent50) |> Stream.averageOrDefault 0.0 |> Converter.roundResult
          Percent75 = stats |> Stream.map(fun x -> x.Percent75) |> Stream.averageOrDefault 0.0 |> Converter.roundResult
          Percent95 = stats |> Stream.map(fun x -> x.Percent95) |> Stream.averageOrDefault 0.0 |> Converter.roundResult
          Percent99 = stats |> Stream.map(fun x -> x.Percent99) |> Stream.averageOrDefault 0.0 |> Converter.roundResult
          StdDev = stats |> Stream.map(fun x -> x.StdDev) |> Stream.averageOrDefault 0.0 |> Converter.roundResult
          LatencyCount = latencyCount }

module DataTransferStats =

    let create (stats: RawStepStats) =

        let inline toKb (value: float) = value |> UMX.tag<bytes> |> Converter.fromBytesToKb |> UMX.untag

        let dataTransfer =
            if stats.DataTransferBytes.TotalCount > 0L then ValueSome(stats.DataTransferBytes.Copy()) // we copy for safe enumeration
            else ValueNone

        let minBytes = if % stats.MinBytes = Double.MaxValue then 0.0<bytes> else stats.MinBytes

        { MinKb = minBytes |> Converter.fromBytesToKb |> UMX.untag

          MeanKb = dataTransfer
                   |> ValueOption.map(fun x -> x.GetMean() |> UMX.tag |> Converter.fromBytesToKb |> UMX.untag)
                   |> ValueOption.defaultValue 0.0

          MaxKb = stats.MaxBytes |> Converter.fromBytesToKb |> UMX.untag

          Percent50 = dataTransfer
                      |> ValueOption.map(fun x -> x.GetValueAtPercentile(50.0) |> float |> toKb)
                      |> ValueOption.defaultValue 0.0

          Percent75 = dataTransfer
                      |> ValueOption.map(fun x -> x.GetValueAtPercentile(75.0) |> float |> toKb)
                      |> ValueOption.defaultValue 0.0

          Percent95 = dataTransfer
                      |> ValueOption.map(fun x -> x.GetValueAtPercentile(95.0) |> float |> toKb)
                      |> ValueOption.defaultValue 0.0

          Percent99 = dataTransfer
                      |> ValueOption.map(fun x -> x.GetValueAtPercentile(99.0) |> float |> toKb)
                      |> ValueOption.defaultValue 0.0

          StdDev = dataTransfer
                   |> ValueOption.map(fun x -> x.GetStdDeviation() |> toKb |> Converter.roundResult)
                   |> ValueOption.defaultValue 0.0

          AllMB = % stats.AllMB }

    let merge (stats: Stream<DataTransferStats>) =
        { MinKb = stats |> Stream.map(fun x -> x.MinKb) |> Stream.minOrDefault 0.0 |> Converter.roundResult
          MeanKb = stats |> Stream.map(fun x -> x.MeanKb) |> Stream.averageOrDefault 0.0 |> Converter.roundResult
          MaxKb = stats |> Stream.map(fun x -> x.MaxKb) |> Stream.maxOrDefault 0.0 |> Converter.roundResult
          Percent50 = stats |> Stream.map(fun x -> x.Percent50) |> Stream.averageOrDefault 0.0 |> Converter.roundResult
          Percent75 = stats |> Stream.map(fun x -> x.Percent75) |> Stream.averageOrDefault 0.0 |> Converter.roundResult
          Percent95 = stats |> Stream.map(fun x -> x.Percent95) |> Stream.averageOrDefault 0.0 |> Converter.roundResult
          Percent99 = stats |> Stream.map(fun x -> x.Percent99) |> Stream.averageOrDefault 0.0 |> Converter.roundResult
          StdDev = stats |> Stream.map(fun x -> x.StdDev) |> Stream.averageOrDefault 0.0 |> Converter.roundResult
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

        let createByStepStats (scnName: ScenarioName) (duration: TimeSpan)
                              (simulationStats: LoadSimulationStats)
                              (mergedStats: Stream<StepStats>) =

            let less800 = mergedStats |> Stream.sumBy(fun x -> x.Ok.Latency.LatencyCount.Less800 + x.Fail.Latency.LatencyCount.Less800)
            let more800Less1200 = mergedStats |> Stream.sumBy(fun x -> x.Ok.Latency.LatencyCount.More800Less1200 + x.Fail.Latency.LatencyCount.More800Less1200)
            let more1200 = mergedStats |> Stream.sumBy(fun x -> x.Ok.Latency.LatencyCount.More1200 + x.Fail.Latency.LatencyCount.More1200)

            { ScenarioName = scnName
              RequestCount = mergedStats |> Stream.sumBy(fun x -> x.Ok.Request.Count + x.Fail.Request.Count)
              OkCount = mergedStats |> Stream.sumBy(fun x -> x.Ok.Request.Count)
              FailCount = mergedStats |> Stream.sumBy(fun x -> x.Fail.Request.Count)
              AllDataMB = mergedStats |> Stream.sumBy(fun x -> x.Ok.DataTransfer.AllMB + x.Fail.DataTransfer.AllMB) |> Converter.roundResult
              StepStats = mergedStats |> Stream.toArray
              LatencyCount = { Less800 = less800; More800Less1200 = more800Less1200; More1200 = more1200 }
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
