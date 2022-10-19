module internal NBomber.Domain.Stats.Statistics

open System
open System.Collections.Generic
open HdrHistogram
open FSharp.UMX
open NBomber
open NBomber.Contracts.Stats
open NBomber.Extensions.Data
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Stats.StepStatsRawData

let roundDuration (duration: TimeSpan) =
    TimeSpan(duration.Days, duration.Hours, duration.Minutes, duration.Seconds)

module Histogram =

    let mean (histogram: HistogramBase) = histogram.GetMean()
    let getPercentile (percentile: float) (histogram: HistogramBase) = histogram.GetValueAtPercentile(percentile)
    let stdDev (histogram: HistogramBase) = histogram.GetStdDeviation()

module RequestStats =

    let calcRPS (executionTime: TimeSpan) (requestCount: int) =
        let totalSec =
            if executionTime.TotalSeconds < 1.0 then 1.0
            else executionTime.TotalSeconds

        float requestCount / totalSec

    let create (stats: RawStepStats) (executionTime: TimeSpan) =
        { Count = stats.RequestCount
          RPS = stats.RequestCount |> calcRPS(executionTime) |> Converter.round 1 }

module LatencyStats =

    let inline microSecToMs (x: 'T) = x |> float |> Converter.fromMicroSecToMs |> Converter.round(Constants.StatsRounding)

    let create (stats: RawStepStats) =

        let latencies =
            if stats.LatencyHistogram.TotalCount > 0 then ValueSome stats.LatencyHistogram
            else ValueNone

        { MinMs  = if latencies.IsSome then stats.MinMicroSec |> microSecToMs else 0.0
          MeanMs = if latencies.IsSome then latencies.Value |> Histogram.mean |> microSecToMs else 0.0
          MaxMs  = if latencies.IsSome then stats.MaxMicroSec |> microSecToMs else 0.0
          Percent50 = if latencies.IsSome then latencies.Value |> Histogram.getPercentile(50.0) |> microSecToMs else 0.0
          Percent75 = if latencies.IsSome then latencies.Value |> Histogram.getPercentile(75.0) |> microSecToMs else 0.0
          Percent95 = if latencies.IsSome then latencies.Value |> Histogram.getPercentile(95.0) |> microSecToMs else 0.0
          Percent99 = if latencies.IsSome then latencies.Value |> Histogram.getPercentile(99.0) |> microSecToMs else 0.0
          StdDev    = if latencies.IsSome then latencies.Value |> Histogram.stdDev |> microSecToMs else 0.0
          LatencyCount = { LessOrEq800 = stats.LessOrEq800; More800Less1200 = stats.More800Less1200; MoreOrEq1200 = stats.MoreOrEq1200 } }

module DataTransferStats =

    let create (stats: RawStepStats) =

        let dataTransfer =
            if stats.DataTransferHistogram.TotalCount > 0L then ValueSome stats.DataTransferHistogram
            else ValueNone

        { MinBytes  = if dataTransfer.IsSome then stats.MinBytes else 0
          MeanBytes = if dataTransfer.IsSome then dataTransfer.Value |> Histogram.mean |> int else 0
          MaxBytes  = if dataTransfer.IsSome then stats.MaxBytes else 0
          Percent50 = if dataTransfer.IsSome then dataTransfer.Value |> Histogram.getPercentile(50.0) |> int else 0
          Percent75 = if dataTransfer.IsSome then dataTransfer.Value |> Histogram.getPercentile(75.0) |> int else 0
          Percent95 = if dataTransfer.IsSome then dataTransfer.Value |> Histogram.getPercentile(95.0) |> int else 0
          Percent99 = if dataTransfer.IsSome then dataTransfer.Value |> Histogram.getPercentile(99.0) |> int else 0
          StdDev    = if dataTransfer.IsSome then dataTransfer.Value |> Histogram.stdDev |> Converter.round(Constants.StatsRounding) else 0.0
          AllBytes  = stats.AllBytes }

module StatusCodeStats =

    let create (stats: Dictionary<string,RawStatusCodeStats>) =
        stats.Values
        |> Seq.map(fun x ->
            { StatusCodeStats.StatusCode = x.StatusCode
              IsError = x.IsError
              Message = x.Message
              Count = x.Count }
        )
        |> Seq.toArray

    let merge (stats: StatusCodeStats[]): StatusCodeStats[] =
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

    let create (stepTimeout: TimeSpan)
               (clientFactoryName: string)
               (clientFactoryClientCount: int)
               (feedName: string)
               (duration: TimeSpan)
               (stepData: StepStatsRawData) =

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

        { StepName = stepData.StepName
          Ok = okStats
          Fail = failStats
          StepInfo = { Timeout = stepTimeout
                       ClientFactoryName = clientFactoryName
                       ClientFactoryClientCount = clientFactoryClientCount
                       FeedName = feedName } }

    let getAllRequestCount (stats: StepStats) =
        stats.Ok.Request.Count + stats.Fail.Request.Count

module ScenarioStats =

    let empty (scenario: Scenario) =

        let simulation = scenario.LoadTimeLine.Head.LoadSimulation
        let simulationStats = LoadTimeLine.createSimulationStats(simulation, 0, 0)

        { ScenarioName = scenario.ScenarioName
          RequestCount = 0
          OkCount = 0
          FailCount = 0
          AllBytes = 0
          StepStats = Array.empty
          LatencyCount = { LessOrEq800 = 0; More800Less1200 = 0; MoreOrEq1200 = 0 }
          LoadSimulationStats = simulationStats
          StatusCodes = Array.empty
          CurrentOperation = OperationType.None
          Duration = TimeSpan.Zero }

    let create (scenarioName: string)
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

        let stepStats = allStepsData |> Array.map(StepStats.create (TimeSpan.MinValue) "" 0 "" reportingInterval)

        // reorder steps to have GlobalInfo on top
        stepStats |> Array.sortInPlaceBy(fun x -> if x.StepName = Constants.ScenarioGlobalInfo then 0 else 1)

        let okCodes = allStepsData |> Array.collect(fun x -> StatusCodeStats.create x.OkStats.StatusCodes)
        let failCodes = allStepsData |> Array.collect(fun x -> StatusCodeStats.create x.FailStats.StatusCodes)
        let statusCodes = StatusCodeStats.merge (okCodes |> Array.append failCodes)

        { ScenarioName = scenarioName
          RequestCount = okCount + failCount
          OkCount = okCount
          FailCount = failCount
          AllBytes = allBytes
          StepStats = stepStats
          LatencyCount = { LessOrEq800 = less800; More800Less1200 = more800Less1200; MoreOrEq1200 = more1200 }
          LoadSimulationStats = simulationStats
          StatusCodes = statusCodes
          CurrentOperation = currentOperation
          Duration = duration |> UMX.untag |> roundDuration }

    let failStepStatsExist (stats: ScenarioStats) =
        stats.StepStats |> Array.exists(fun stats -> stats.Fail.Request.Count > 0)

module NodeStats =

    let create (testInfo: TestInfo) (nodeInfo: NodeInfo) (scnStats: ScenarioStats[]) =
        if Array.isEmpty scnStats then
            { NodeStats.empty with NodeInfo = nodeInfo; TestInfo = testInfo }
        else
            let maxDuration = scnStats |> Array.maxBy(fun x -> x.Duration) |> fun scn -> scn.Duration |> roundDuration

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
