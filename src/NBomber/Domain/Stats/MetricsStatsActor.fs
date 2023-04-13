module internal NBomber.Domain.Stats.MetricsStatsActor

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Serilog
open HdrHistogram

open NBomber
open NBomber.Extensions.Internal

type Metric = {
    Name: string
    Value: float
}

type Percentiles = {
    Mean: float
    Max: float
    Percent50: float
    Percent75: float
    Percent95: float
    Percent99: float
    StdDev: float
}

type MetricType =
    | Histogram = 0
    | Counter = 1

type MetricStats = {
    Name: string
    MeasureUnit: string
    MetricType: MetricType
    Current: float
    Max: float
    Duration: TimeSpan
    Percentiles: Percentiles option
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

type CounterMetric = {
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
    | Counter   of CounterMetric

type ActorMessage =
    | RegisterMetric      of name:string * measureUnit:string * scalingFraction:float * metricType:MetricType
    | AddMetric           of Metric
    | BuildReportingStats of reply:TaskCompletionSource<MetricStats list> * executedDuration:TimeSpan
    | GetFinalStats       of reply:TaskCompletionSource<MetricStats list> * executedDuration:TimeSpan

type MetricsStatsActor(logger: ILogger) =

    let _queue = ConcurrentQueue<ActorMessage>()
    let _rawMetrics = Dictionary<string, RawMetricStats>()
    let _allRealtimeMetrics = ConcurrentDictionary<TimeSpan, MetricStats list>()

    let mutable _currentMetrics = List.empty<MetricStats>
    let mutable _stop = false

    let registerMetric (name) (measureUnit) (scFraction) (metricType) (rawMetrics: Dictionary<string, RawMetricStats>) =
        if not (rawMetrics.ContainsKey name) then
            match metricType with
            | MetricType.Histogram -> rawMetrics[name] <- Histogram(HistogramMetric.empty name measureUnit scFraction)
            | MetricType.Counter   -> rawMetrics[name] <- Counter(CounterMetric.empty name measureUnit scFraction)
            | _ -> ()

    let addMetric (metric: Metric) (rawMetrics: Dictionary<string, RawMetricStats>) =
        match rawMetrics.TryGetValue metric.Name with
        | true, Histogram v ->
            v.Histogram.RecordValue(int64 metric.Value)
            v.Current <- metric.Value

        | true, Counter v ->
            v.Value <- int64 metric.Value

        | false, _ -> ()

    let buildReportingStats (executedDuration) =
        _rawMetrics
        |> Seq.map(fun (KeyValue(k, metric)) ->
            match metric with
            | Histogram v ->
                { Name = v.Name
                  MeasureUnit = v.MeasureUnit
                  MetricType = MetricType.Histogram
                  Current = (v.Current / v.ScalingFraction) |> Converter.round(Constants.StatsRounding)
                  Max = (float (v.Histogram.GetMaxValue()) / v.ScalingFraction) |> Converter.round(Constants.StatsRounding)
                  Duration = executedDuration
                  Percentiles = None }

            | Counter v ->
                { Name = v.Name
                  MeasureUnit = v.MeasureUnit
                  MetricType = MetricType.Counter
                  Current = (float v.Value / v.ScalingFraction) |> Converter.round(Constants.StatsRounding)
                  Max = 0
                  Duration = executedDuration
                  Percentiles = None }
        )
        |> Seq.toList

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

                    | BuildReportingStats(reply, executedDuration) ->
                        let metrics = buildReportingStats executedDuration

                        _currentMetrics <- metrics
                        _allRealtimeMetrics[executedDuration] <- metrics

                        reply.TrySetResult(metrics) |> ignore

                    | GetFinalStats(reply, executedDuration) ->
                        ()

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

    member this.BuildReportingStats(executedDuration) =
        let reply = TaskCompletionSource<_>()
        _queue.Enqueue(BuildReportingStats(reply, executedDuration))
        reply.Task
