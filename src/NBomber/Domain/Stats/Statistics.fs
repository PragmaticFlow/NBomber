module internal NBomber.Domain.Stats.Statistics

open System
open System.Collections.Generic
open HdrHistogram
open NBomber
open NBomber.Contracts.Stats
open NBomber.Extensions.Data
open NBomber.Domain
open NBomber.Domain.DomainTypes
open NBomber.Domain.Stats.RawMeasurementStats

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

    let empty = { Count = 0; RPS = 0 }

    let create (stats: RawItemStats) (executionTime: TimeSpan) =
        { Count = stats.RequestCount
          RPS = stats.RequestCount |> calcRPS(executionTime) |> Converter.round 1 }

module LatencyStats =

    let inline microSecToMs (x: 'T) = x |> float |> Converter.fromMicroSecToMs |> Converter.round(Constants.StatsRounding)

    let empty = {
        MinMs = 0; MeanMs = 0; MaxMs = 0
        Percent50 = 0; Percent75 = 0; Percent95 = 0; Percent99 = 0; StdDev= 0
        LatencyCount = { LessOrEq800 = 0; More800Less1200 = 0; MoreOrEq1200 = 0 }
    }

    let create (stats: RawItemStats) =

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

    let empty = {
        MinBytes = 0; MeanBytes = 0; MaxBytes = 0
        Percent50 = 0; Percent75 = 0; Percent95 = 0; Percent99 = 0; StdDev = 0
        AllBytes = 0
    }

    let create (stats: RawItemStats) =

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

    let merge (stats: StatusCodeStats seq) =
        stats
        |> Seq.groupBy(fun x -> x.StatusCode)
        |> Seq.sortBy(fun (code,codeStats) -> code)
        |> Seq.map(fun (code,codeStats) ->
            { StatusCodeStats.StatusCode = code
              IsError = codeStats |> Seq.head |> fun x -> x.IsError
              Message = codeStats |> Seq.head |> fun x -> x.Message
              Count = codeStats |> Seq.sumBy(fun x -> x.Count) }
        )
        |> Seq.toArray

module MeasurementStats =

    let empty = {
        Request = RequestStats.empty
        Latency = LatencyStats.empty
        DataTransfer = DataTransferStats.empty
        StatusCodes = Array.empty
    }

    let create (raw: RawMeasurementStats) (duration: TimeSpan) =

        let okStats = {
            Request = RequestStats.create raw.OkStats duration
            Latency = LatencyStats.create raw.OkStats
            DataTransfer = DataTransferStats.create raw.OkStats
            StatusCodes = StatusCodeStats.create raw.OkStats.StatusCodes
        }

        let failStats = {
            Request = RequestStats.create raw.FailStats duration
            Latency = LatencyStats.create raw.FailStats
            DataTransfer = DataTransferStats.create raw.FailStats
            StatusCodes = StatusCodeStats.create raw.FailStats.StatusCodes
        }

        struct (okStats, failStats)

module StepStats =

    let create (duration: TimeSpan)
               (stepData: RawMeasurementStats) =

        let struct (okStats, failStats) = MeasurementStats.create stepData duration

        { StepName = stepData.Name
          Ok = okStats
          Fail = failStats }

    let getAllRequestCount (stats: StepStats) =
        stats.Ok.Request.Count + stats.Fail.Request.Count

    let extractGlobalInfoStep (scn: ScenarioStats) = {
        StepName = Constants.ScenarioGlobalInfo
        Ok = scn.Ok
        Fail = scn.Fail
    }

module ScenarioStats =

    module GlobalInfo =

        let create (globalInfo: StepStats) (allStepStats: StepStats[]) =

            let okStatus =
                allStepStats
                |> Seq.collect(fun x -> x.Ok.StatusCodes)
                |> StatusCodeStats.merge

            let failStatus =
                allStepStats
                |> Seq.collect(fun x -> x.Fail.StatusCodes)
                |> StatusCodeStats.merge

            let ok = { globalInfo.Ok with StatusCodes = okStatus }
            let fail = { globalInfo.Fail with StatusCodes = failStatus }

            struct (ok, fail)

    let empty (scenario: Scenario) =

        let simulation = scenario.LoadSimulations.Head.Value
        let simulationStats = LoadSimulation.createSimulationStats simulation 0 0

        { ScenarioName = scenario.ScenarioName
          Ok = MeasurementStats.empty
          Fail = MeasurementStats.empty
          StepStats = Array.empty
          LoadSimulationStats = simulationStats
          CurrentOperation = OperationType.None
          AllRequestCount = 0
          AllOkCount = 0
          AllFailCount = 0
          AllBytes = 0
          Duration = TimeSpan.Zero }

    let create (scenarioName: string)
               (rawStats: RawMeasurementStats[])
               (simulationStats: LoadSimulationStats)
               (currentOperation: OperationType)
               (executedDuration: TimeSpan)
               (reportingInterval: TimeSpan)
               (pause: TimeSpan) =

        let execOnlyDuration = reportingInterval - pause
        let allStepStats = rawStats |> Array.map(StepStats.create execOnlyDuration)
        let stepStats    = allStepStats |> Array.filter(fun x -> x.StepName <> Constants.ScenarioGlobalInfo)
        let globalInfo   = allStepStats |> Array.tryFind(fun x -> x.StepName = Constants.ScenarioGlobalInfo)

        let allOkCount = allStepStats |> Array.sumBy(fun x -> x.Ok.Request.Count)
        let allFailCount = allStepStats |> Array.sumBy(fun x -> x.Fail.Request.Count)
        let allReqCount = allOkCount + allFailCount

        let struct (ok, fail) =
            match globalInfo with
            | Some v -> GlobalInfo.create v allStepStats
            | None   -> MeasurementStats.empty, MeasurementStats.empty

        let allBytes =
            if globalInfo.IsSome then
                ok.DataTransfer.AllBytes + fail.DataTransfer.AllBytes
            else
                allStepStats |> Array.sumBy(fun x -> x.Ok.DataTransfer.AllBytes + x.Fail.DataTransfer.AllBytes)

        { ScenarioName = scenarioName
          Ok = ok
          Fail = fail
          StepStats = stepStats
          LoadSimulationStats = simulationStats
          CurrentOperation = currentOperation
          AllRequestCount = allReqCount
          AllOkCount = allOkCount
          AllFailCount = allFailCount
          AllBytes = allBytes
          Duration = roundDuration executedDuration }

    let failStatsExist (stats: ScenarioStats) =
        stats.StepStats |> Array.exists(fun stats -> stats.Fail.Request.Count > 0)

    let calcAllBytes (stats: ScenarioStats) =
        stats.Ok.DataTransfer.AllBytes + stats.Fail.DataTransfer.AllBytes

module NodeStats =

    let create (testInfo: TestInfo) (nodeInfo: NodeInfo) (scnStats: ScenarioStats[]) =
        if Array.isEmpty scnStats then
            { NodeStats.empty with NodeInfo = nodeInfo; TestInfo = testInfo }
        else
            let maxDuration = scnStats |> Seq.map(fun x -> x.Duration) |> Seq.max |> roundDuration
            let okCount = scnStats |> Array.sumBy(fun x -> x.AllOkCount)
            let failCount = scnStats |> Array.sumBy(fun x -> x.AllFailCount)
            let requestCount = okCount + failCount
            let allBytes = scnStats |> Array.sumBy(fun x -> x.AllBytes)

            { ScenarioStats = scnStats
              PluginStats = Array.empty
              NodeInfo = nodeInfo
              TestInfo = testInfo
              ReportFiles = Array.empty
              AllRequestCount = requestCount
              AllOkCount = okCount
              AllFailCount = failCount
              AllBytes = allBytes
              Duration = maxDuration }
