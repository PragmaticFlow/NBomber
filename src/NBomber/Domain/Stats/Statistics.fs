module internal NBomber.Domain.Stats.Statistics

open System
open System.Collections.Generic

open HdrHistogram
open FSharp.UMX

open NBomber
open NBomber.Contracts.Metrics
open NBomber.Contracts.Stats
open NBomber.Domain
open NBomber.Domain.DomainTypes

let calcRPS (requestCount: int) (executionTime: TimeSpan) =
    let totalSec =
        if executionTime.TotalSeconds < 1.0 then 1.0
        else executionTime.TotalSeconds

    float requestCount / totalSec

module Converter =

    let fromMicroSecToMs (microSec: float) = (microSec / 1000.0)
    let fromMsToMicroSec (ms: float) = (ms * 1000.0) |> int

    let inline fromBytesToKb (bytes) = Math.Round(float bytes / 1024.0, 3)
    let inline fromBytesToMb (bytes) = Math.Round(decimal bytes / 1024.0M / 1024.0M, 4)
    let inline round (digits: int) (value: float) = Math.Round(value, digits)

module Histogram =

    let mean (histogram: HistogramBase) = histogram.GetMean()
    let getPercentile (percentile: float) (histogram: HistogramBase) = histogram.GetValueAtPercentile(percentile)
    let stdDev (histogram: HistogramBase) = histogram.GetStdDeviation()

module RequestStats =

    let create (stats: RawStepStats) (executionTime: TimeSpan) =
        { Count = stats.RequestCount
          RPS = calcRPS stats.RequestCount executionTime }

    let round (stats: RequestStats) =
        { stats with RPS = stats.RPS |> Converter.round 1 }

module LatencyStats =

    let create (stats: RawStepStats) =

        let latencies =
            if stats.LatencyHistogram.TotalCount > 0 then ValueSome stats.LatencyHistogram
            else ValueNone

        { MinMs  = if latencies.IsSome then stats.MinMicroSec |> float |> Converter.fromMicroSecToMs else 0.0
          MeanMs = if latencies.IsSome then latencies.Value |> Histogram.mean |> Converter.fromMicroSecToMs else 0.0
          MaxMs  = if latencies.IsSome then stats.MaxMicroSec |> float |> Converter.fromMicroSecToMs else 0.0
          Percent50 = if latencies.IsSome then latencies.Value |> Histogram.getPercentile(50.0) |> float |> Converter.fromMicroSecToMs else 0.0
          Percent75 = if latencies.IsSome then latencies.Value |> Histogram.getPercentile(75.0) |> float |> Converter.fromMicroSecToMs else 0.0
          Percent95 = if latencies.IsSome then latencies.Value |> Histogram.getPercentile(95.0) |> float |> Converter.fromMicroSecToMs else 0.0
          Percent99 = if latencies.IsSome then latencies.Value |> Histogram.getPercentile(99.0) |> float |> Converter.fromMicroSecToMs else 0.0
          StdDev    = if latencies.IsSome then latencies.Value |> Histogram.stdDev |> Converter.fromMicroSecToMs else 0.0
          LatencyCount = { LessOrEq800 = stats.LessOrEq800; More800Less1200 = stats.More800Less1200; MoreOrEq1200 = stats.MoreOrEq1200 } }

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
            if stats.DataTransferHistogram.TotalCount > 0L then ValueSome(stats.DataTransferHistogram)
            else ValueNone

        { MinBytes  = if dataTransfer.IsSome then stats.MinBytes else 0
          MeanBytes = if dataTransfer.IsSome then dataTransfer.Value |> Histogram.mean |> int else 0
          MaxBytes  = if dataTransfer.IsSome then stats.MaxBytes else 0
          Percent50 = if dataTransfer.IsSome then dataTransfer.Value |> Histogram.getPercentile(50.0) |> int else 0
          Percent75 = if dataTransfer.IsSome then dataTransfer.Value |> Histogram.getPercentile(75.0) |> int else 0
          Percent95 = if dataTransfer.IsSome then dataTransfer.Value |> Histogram.getPercentile(95.0) |> int else 0
          Percent99 = if dataTransfer.IsSome then dataTransfer.Value |> Histogram.getPercentile(99.0) |> int else 0
          StdDev    = if dataTransfer.IsSome then dataTransfer.Value |> Histogram.stdDev else 0.0
          AllBytes  = stats.AllBytes }

    let round (stats: DataTransferStats) =
        { stats with StdDev = stats.StdDev |> Converter.round(Constants.StatsRounding) }

module StatusCodeStats =

    let create (stats: Dictionary<int,StatusCodeStats>) =
        stats.Values |> Seq.toArray

    let merge (stats: StatusCodeStats[]) =
        stats
        |> Array.groupBy(fun x -> x.StatusCode)
        |> Array.sortBy(fun (code,codeStats) -> code)
        |> Array.map(fun (code,codeStats) ->
            { StatusCode = code
              IsError = codeStats |> Seq.head |> fun x -> x.IsError
              Message = codeStats |> Seq.head |> fun x -> x.Message
              Count = codeStats |> Seq.sumBy(fun x -> x.Count) }
        )

module StepStats =

    let create (stepName: string)
               (stepData: StepStatsRawData)
               (stepTimeout: TimeSpan)
               (clientFactoryName: string)
               (clientFactoryClientCount: int)
               (feedName: string)
               (duration: TimeSpan) =

        let okStats = {
            Request = RequestStats.create stepData.OkStats duration
            Latency = LatencyStats.create stepData.OkStats
            DataTransfer = DataTransferStats.create stepData.OkStats
            StatusCodes = StatusCodeStats.create stepData.OkStats.StatusCodes
        }

        let failStats = {
            Request = RequestStats.create stepData.FailStats duration
            Latency = LatencyStats.create stepData.FailStats
            DataTransfer = DataTransferStats.create stepData.FailStats
            StatusCodes = StatusCodeStats.create stepData.FailStats.StatusCodes
        }

        { StepName = stepName
          Ok = okStats
          Fail = failStats
          StepInfo = { Timeout = stepTimeout
                       ClientFactoryName = clientFactoryName
                       ClientFactoryClientCount = clientFactoryClientCount
                       FeedName = feedName } }

    let round (stats: StepStats) =
        { stats with Ok = { stats.Ok with Request = stats.Ok.Request |> RequestStats.round
                                          Latency = stats.Ok.Latency |> LatencyStats.round
                                          DataTransfer = stats.Ok.DataTransfer |> DataTransferStats.round }

                     Fail = { stats.Fail with Request = stats.Fail.Request |> RequestStats.round
                                              Latency = stats.Fail.Latency |> LatencyStats.round
                                              DataTransfer = stats.Fail.DataTransfer |> DataTransferStats.round } }

    let getAllRequestCount (stats: StepStats) =
        stats.Ok.Request.Count + stats.Fail.Request.Count

module MetricStats =

    let private applySepsStats stats f =
        (true, stats)
        ||> Array.fold (fun status stats ->
            status
            && (f stats.Ok || stats.Ok.Request.Count = 0)
            && (f stats.Fail || stats.Fail.Request.Count = 0)
        )

    let private applyRequestCountThresholds stepStats thresholds =
        let sum by = stepStats |> Array.sumBy by
        let okCount = sum (fun x -> x.Ok.Request.Count)
        let failedCount = sum (fun x -> x.Fail.Request.Count)
        let allCount = okCount + failedCount
        thresholds
        |> List.map (fun threshold ->
            let status =
                match threshold with
                | AllCount f -> f allCount
                | OkCount f -> f okCount
                | FailedCount f -> f failedCount
                | FailedRate f -> f (float failedCount * 100. / float allCount)
                | RPS f -> applySepsStats stepStats (fun s -> f s.Request.RPS)
                |> ThresholdStatus.map
            threshold, status)
        |> RequestCountStats

    let private applyLatencyThresholds stepStats thresholds =
        thresholds
        |> List.map (fun threshold ->
            let status =
                match threshold with
                | Min f -> (fun s -> f s.Latency.MinMs)
                | Mean f -> (fun s -> f s.Latency.MeanMs)
                | Max f -> (fun s -> f s.Latency.MaxMs)
                | StdDev f -> (fun s -> f s.Latency.StdDev)
                |> applySepsStats stepStats
                |> ThresholdStatus.map
            threshold, status)
        |> LatencyStats

    let private applyLatencyPercentileThresholds stepStats thresholds =
        thresholds
        |> List.map (fun threshold ->
            let status =
                match threshold with
                | P50 f -> (fun s -> f s.Latency.Percent50)
                | P75 f -> (fun s -> f s.Latency.Percent75)
                | P95 f -> (fun s -> f s.Latency.Percent95)
                | P99 f -> (fun s -> f s.Latency.Percent99)
                |> applySepsStats stepStats
                |> ThresholdStatus.map
            threshold, status)
        |> LatencyPercentileStats

    let applyMetricThresholds stepStats metric =
        match metric with
        | RequestCount thresholds -> applyRequestCountThresholds stepStats thresholds
        | Latency thresholds -> applyLatencyThresholds stepStats thresholds
        | LatencyPercentile thresholds -> applyLatencyPercentileThresholds stepStats thresholds

    let failStatsExist stats =
        let failed stats =
            stats
            |> List.map snd
            |> List.exists (fun status ->
                match status with
                | Passed -> false
                | Failed -> true
            )
        match stats with
        | RequestCountStats stats -> stats |> failed
        | LatencyStats stats -> stats |> failed
        | LatencyPercentileStats stats -> stats |> failed

module ScenarioStats =

    let create (scenario: Scenario)
               (allStepsData: StepStatsRawData[])
               (simulationStats: LoadSimulationStats)
               (currentOperation: OperationType)
               (duration: TimeSpan<scenarioDuration>)
               (reportingInterval: TimeSpan) =

        let okCount = allStepsData |> Array.sumBy(fun x -> x.OkStats.RequestCount)
        let failCount = allStepsData |> Array.sumBy(fun x -> x.FailStats.RequestCount)
        let allBytes = allStepsData |> Array.sumBy(fun x -> x.OkStats.AllBytes + x.FailStats.AllBytes)

        let less800 = allStepsData |> Array.sumBy(fun x -> x.OkStats.LessOrEq800 + x.FailStats.LessOrEq800)
        let more800Less1200 = allStepsData |> Array.sumBy(fun x -> x.OkStats.More800Less1200 + x.FailStats.More800Less1200)
        let more1200 = allStepsData |> Array.sumBy(fun x -> x.OkStats.MoreOrEq1200 + x.FailStats.MoreOrEq1200)

        let stepStats =
            scenario.Steps
            |> List.mapi(fun i st ->
                if st.DoNotTrack then None
                else
                    let clName = st.ClientFactory |> Option.map(fun x -> x.FactoryName |> ClientFactory.getOriginalName) |> Option.defaultValue "none"
                    let clCount = st.ClientFactory |> Option.map(fun x -> x.ClientCount) |> Option.defaultValue 0
                    let fdName = st.Feed |> Option.map(fun x -> x.FeedName) |> Option.defaultValue "none"
                    Some(StepStats.create st.StepName allStepsData[i] st.Timeout clName clCount fdName reportingInterval))
            |> List.choose id
            |> List.toArray

        let okCodes = allStepsData |> Array.collect(fun x -> StatusCodeStats.create x.OkStats.StatusCodes)
        let failCodes = allStepsData |> Array.collect(fun x -> StatusCodeStats.create x.FailStats.StatusCodes)
        let statusCodes = StatusCodeStats.merge(okCodes |> Array.append(failCodes))

        let metricStats =
            scenario.Thresholds
            |> Option.map (
                MetricStats.applyMetricThresholds stepStats
                |> List.map
                >> Array.ofList
            )

        { ScenarioName = scenario.ScenarioName
          RequestCount = okCount + failCount
          OkCount = okCount
          FailCount = failCount
          AllBytes = allBytes
          StepStats = stepStats
          LatencyCount = { LessOrEq800 = less800; More800Less1200 = more800Less1200; MoreOrEq1200 = more1200 }
          LoadSimulationStats = simulationStats
          StatusCodes = statusCodes
          CurrentOperation = currentOperation
          Duration = duration
          MetricStats = metricStats }

    let round (stats: ScenarioStats) =
        { stats with StepStats = stats.StepStats |> Array.map(StepStats.round)
                     Duration = TimeSpan(stats.Duration.Days, stats.Duration.Hours, stats.Duration.Minutes, stats.Duration.Seconds) }

    let failStepStatsExist (stats: ScenarioStats) =
        stats.MetricStats
        |> Option.map (Array.exists MetricStats.failStatsExist)
        |> Option.defaultValue (stats.StepStats |> Array.exists(fun stats -> stats.Fail.Request.Count > 0))

module NodeStats =

    let create (testInfo: TestInfo) (nodeInfo: NodeInfo) (scnStats: ScenarioStats[]) =
        if Array.isEmpty scnStats then
            NodeStats.empty
        else
            let maxDuration = scnStats |> Array.maxBy(fun x -> x.Duration) |> fun scn -> scn.Duration
            { RequestCount = scnStats |> Array.sumBy(fun x -> x.RequestCount)
              OkCount = scnStats |> Array.sumBy(fun x -> x.OkCount)
              FailCount = scnStats |> Array.sumBy(fun x -> x.FailCount)
              AllBytes = scnStats |> Array.sumBy(fun x -> x.AllBytes)
              ScenarioStats = scnStats
              PluginStats = Array.empty
              NodeInfo = nodeInfo
              TestInfo = testInfo
              ReportFiles = Array.empty
              Duration = maxDuration }

    let round (stats: NodeStats) =
        { stats with ScenarioStats = stats.ScenarioStats |> Array.map(ScenarioStats.round)
                     Duration = TimeSpan(stats.Duration.Days, stats.Duration.Hours, stats.Duration.Minutes, stats.Duration.Seconds) }
