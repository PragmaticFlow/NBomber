module internal NBomber.DomainServices.TestHost.MetricsGrabber

open System.Collections.Generic
open System.Diagnostics.Tracing

open NBomber.Constants.Metrics
open NBomber.Contracts.Stats
open NBomber.Domain.Stats.MetricsStatsActor

type RuntimeMetricsGrabber(metricsActor: MetricsStatsActor) =
    inherit EventListener()

    let publishMetric (data: IDictionary<string,obj>) (metricName) (mActor: MetricsStatsActor) =
        let value = data["Max"] :?> float
        let metric = { Name = metricName; Value = value }
        mActor.Publish(ActorMessage.AddMetric metric)

    do
        metricsActor.RegisterMetric(CPU_USAGE, "%", DEFAULT_SCALING_FRACTION, MetricType.Histogram)
        metricsActor.RegisterMetric(RAM_WORKING_SET, "MB", DEFAULT_SCALING_FRACTION, MetricType.Histogram)
        metricsActor.RegisterMetric(RAM_GC_HEAP_SIZE, "MB", DEFAULT_SCALING_FRACTION, MetricType.Histogram)
        metricsActor.RegisterMetric(THREAD_POOL_QUEUE_LENGTH, "", NO_SCALING_FRACTION, MetricType.Gauge)
        metricsActor.RegisterMetric(THREAD_POOL_QUEUE_COUNT, "", NO_SCALING_FRACTION, MetricType.Gauge)
        metricsActor.RegisterMetric(BYTES_RECEIVED, "MB", BYTES_TO_MB_SCALING_FRACTION, MetricType.Gauge)
        metricsActor.RegisterMetric(BYTES_SENT, "MB", BYTES_TO_MB_SCALING_FRACTION, MetricType.Gauge)

    override this.OnEventSourceCreated(eventSource) =

        if eventSource.Name = "System.Runtime"
           || eventSource.Name = "System.Net.Sockets"
           || eventSource.Name = "System.Net.NameResolution" then

            let args =
                ["EventCounterIntervalSec", "5"]
                |> dict

            base.EnableEvents(eventSource, EventLevel.LogAlways, EventKeywords.All, args)

    override this.OnEventWritten(eventData) =

        if eventData.Payload <> null
           && eventData.Payload.Count <> 0
           && eventData.Payload[0] :? IDictionary<string,obj> then

            let data = eventData.Payload[0] :?> IDictionary<string,obj>
            match data.TryGetValue "Name" with
            | true, name when name = "cpu-usage"    -> metricsActor |> publishMetric data CPU_USAGE
            | true, name when name = "working-set"  -> metricsActor |> publishMetric data RAM_WORKING_SET
            | true, name when name = "gc-heap-size" -> metricsActor |> publishMetric data RAM_GC_HEAP_SIZE

            | true, name when name = "threadpool-queue-length" -> metricsActor |> publishMetric data THREAD_POOL_QUEUE_LENGTH
            | true, name when name = "threadpool-thread-count" -> metricsActor |> publishMetric data THREAD_POOL_QUEUE_COUNT
            | true, name when name = "bytes-received" ->

                let value = data["Max"] :?> float
                let metric = { Name = BYTES_RECEIVED; Value = value / BYTES_TO_MB_SCALING_FRACTION }

                metricsActor.Publish(ActorMessage.AddMetric metric)

            | true, name when name = "bytes-sent" ->

                let value = data["Max"] :?> float
                let metric = { Name = BYTES_SENT; Value = value / BYTES_TO_MB_SCALING_FRACTION }

                metricsActor.Publish(ActorMessage.AddMetric metric)

            | _ -> ()
