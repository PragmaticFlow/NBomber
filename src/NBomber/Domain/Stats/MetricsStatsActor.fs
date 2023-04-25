module internal NBomber.Domain.Stats.MetricsStatsActor

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Serilog
open HdrHistogram

open NBomber
open NBomber.Contracts
open NBomber.Contracts.Stats
open NBomber.Extensions.Internal

type Metric = {
    Name: string
    Value: float
}

type HistogramMetric = {
    Name: string
    MeasureUnit: string
    mutable Current: float
    ScalingFraction: float
    Histogram: LongHistogram
} with

    static member empty name measureUnit scFraction = {
        Name = name
        MeasureUnit = measureUnit
        Current = 0
        ScalingFraction = scFraction
        Histogram = LongHistogram(highestTrackableValue = Constants.MaxTrackableStepLatency,
                                  numberOfSignificantValueDigits = 3)
    }

type GaugeMetric = {
    Name: string
    MeasureUnit: string
    mutable Value: int64
    ScalingFraction: float
} with

    static member empty name measureUnit scFraction = {
        Name = name
        MeasureUnit = measureUnit
        Value = 0
        ScalingFraction = scFraction
    }

type RawMetricStats =
    | Histogram of HistogramMetric
    | Gauge     of GaugeMetric

type ActorMessage =
    | RegisterMetric      of name:string * measureUnit:string * scalingFraction:float * metricType:MetricType
    | AddMetric           of Metric
    | CleanAllMetrics
    | BuildReportingStats of reply:TaskCompletionSource<MetricStats[]> * currentTime:TimeSpan
    | GetFinalStats       of reply:TaskCompletionSource<MetricStats[]> * currentTime:TimeSpan

type MetricsStatsActor(logger: ILogger) =

    let _queue = ConcurrentQueue<ActorMessage>()
    let _rawMetrics = Dictionary<string, RawMetricStats>()
    let _allRealtimeMetrics = ConcurrentDictionary<TimeSpan, MetricStats[]>()

    let mutable _currentMetrics = Array.empty<MetricStats>
    let mutable _stop = false

    let registerMetric (name) (measureUnit) (scFraction) (metricType) (rawMetrics: Dictionary<string, RawMetricStats>) =
        if not (rawMetrics.ContainsKey name) then
            match metricType with
            | MetricType.Histogram -> rawMetrics[name] <- Histogram(HistogramMetric.empty name measureUnit scFraction)
            | MetricType.Gauge   -> rawMetrics[name] <- Gauge(GaugeMetric.empty name measureUnit scFraction)
            | _ -> ()

    let addMetric (metric: Metric) (rawMetrics: Dictionary<string, RawMetricStats>) =
        match rawMetrics.TryGetValue metric.Name with
        | true, Histogram v ->
            let value = int64 (metric.Value * v.ScalingFraction)
            v.Histogram.RecordValue value
            v.Current <- float value

        | true, Gauge v ->
            v.Value <- int64 (metric.Value * v.ScalingFraction)

        | false, _ -> ()

    let cleanAllMetrics (rawMetrics: Dictionary<string, RawMetricStats>) =
        rawMetrics
        |> Seq.iter(fun (KeyValue(k, metric)) ->
            match metric with
            | Histogram m ->
                m.Current <- 0
                m.Histogram.Reset()

            | Gauge m ->
                m.Value <- 0
        )

    let buildStats (isFinal) (currentTime) =

        let buildPercentiles (m: HistogramMetric) =
            if m.Histogram.TotalCount > 0 then
                { Mean = (m.Histogram.GetMean() / m.ScalingFraction) |> Converter.round(Constants.StatsRounding)
                  Max = (float (m.Histogram.GetMaxValue()) / m.ScalingFraction) |> Converter.round(Constants.StatsRounding)
                  Percent50 = (float (m.Histogram.GetValueAtPercentile 50) / m.ScalingFraction) |> Converter.round(Constants.StatsRounding)
                  Percent75 = (float (m.Histogram.GetValueAtPercentile 75) / m.ScalingFraction) |> Converter.round(Constants.StatsRounding)
                  Percent95 = (float (m.Histogram.GetValueAtPercentile 95) / m.ScalingFraction) |> Converter.round(Constants.StatsRounding)
                  Percent99 = (float (m.Histogram.GetValueAtPercentile 99) / m.ScalingFraction) |> Converter.round(Constants.StatsRounding) }
                |> Some
            else
                None

        let calcMax (m: HistogramMetric) =
            if m.Histogram.TotalCount > 0 then
                (float (m.Histogram.GetMaxValue()) / m.ScalingFraction) |> Converter.round(Constants.StatsRounding)
            else
                0

        _rawMetrics
        |> Seq.map(fun (KeyValue(k, metric)) ->
            match metric with
            | Histogram m ->
                { Name = m.Name
                  MeasureUnit = m.MeasureUnit
                  MetricType = MetricType.Histogram
                  Current = (m.Current / m.ScalingFraction) |> Converter.round(Constants.StatsRounding)
                  Max = calcMax m
                  Duration = currentTime
                  Percentiles = if isFinal then m |> buildPercentiles
                                else None }

            | Gauge m ->
                { Name = m.Name
                  MeasureUnit = m.MeasureUnit
                  MetricType = MetricType.Gauge
                  Current = (float m.Value / m.ScalingFraction) |> Converter.round(Constants.StatsRounding)
                  Max = 0
                  Duration = currentTime
                  Percentiles = None }
        )
        |> Seq.toArray

    let loop () = backgroundTask {
        try
            while not _stop do
                match _queue.TryDequeue() with
                | true, msg ->

                    match msg with
                    | RegisterMetric (name, measureUnit, scFraction, metricType) ->
                        _rawMetrics |> registerMetric name measureUnit scFraction metricType

                    | AddMetric metric ->
                        _rawMetrics |> addMetric metric

                    | CleanAllMetrics ->
                        _rawMetrics |> cleanAllMetrics

                    | BuildReportingStats(reply, currentTime) ->
                        let metrics = buildStats false currentTime

                        _currentMetrics <- metrics
                        _allRealtimeMetrics[currentTime] <- metrics

                        reply.TrySetResult(metrics) |> ignore

                    | GetFinalStats(reply, currentTime) ->
                        let metrics = buildStats true currentTime
                        reply.TrySetResult(metrics) |> ignore

                | _ -> do! Task.Delay 100
        with
        | ex ->
            logger.Fatal $"Unhandled exception: {nameof MetricsStatsActor} failed: {ex.ToString()}"
    }

    do
        loop() |> ignore

    member this.CurrentMetrics = _currentMetrics
    member this.AllRealtimeMetrics = _allRealtimeMetrics :> IReadOnlyDictionary<_,_>

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member this.Publish(msg) = _queue.Enqueue msg

    member this.RegisterMetric(name, measureUnit, scalingFraction, metricType) =
        _queue.Enqueue(RegisterMetric(name, measureUnit, scalingFraction, metricType))

    member this.BuildReportingStats(currentTime) =
        let reply = TaskCompletionSource<_>()
        _queue.Enqueue(BuildReportingStats(reply, currentTime))
        reply.Task

    member this.GetFinalStats(executedDuration) =
        let reply = TaskCompletionSource<_>()
        _queue.Enqueue(GetFinalStats(reply, executedDuration))
        reply.Task

    interface IMetricsProvider with

        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member this.PublishMetric(name, value) =
            this.Publish(AddMetric { Name = name; Value = value })

        member this.RegisterMetric(name, measureUnit, scalingFraction, metricType) =
            this.RegisterMetric(name, measureUnit, scalingFraction, metricType)

    interface IDisposable with
        member this.Dispose() = _stop <- true
