module internal NBomber.DomainServices.TestHost.MetricsGrabber

open System.Collections.Generic
open System.Diagnostics.Tracing

open NBomber.Constants.Metrics
open NBomber.Domain.Stats.MetricsStatsActor

type RuntimeMetricsGrabber(metricsActor: MetricsStatsActor) =
    inherit EventListener()

    do
        metricsActor.RegisterMetric(CPU_USAGE, "%", DEFAULT_SCALING_FRACTION, MetricType.Histogram)
        metricsActor.RegisterMetric(WORKING_SET, "MB", DEFAULT_SCALING_FRACTION, MetricType.Histogram)
        metricsActor.RegisterMetric(GC_HEAP_SIZE, "MB", DEFAULT_SCALING_FRACTION, MetricType.Histogram)
        metricsActor.RegisterMetric(THREAD_POOL_QUEUE_LENGTH, "", NO_SCALING_FRACTION, MetricType.Counter)
        metricsActor.RegisterMetric(THREAD_POOL_QUEUE_COUNT, "", NO_SCALING_FRACTION, MetricType.Counter)
        metricsActor.RegisterMetric(INCOMING_CONNECTIONS_ESTABLISHED, "", NO_SCALING_FRACTION, MetricType.Counter)
        metricsActor.RegisterMetric(OUTGOING_CONNECTIONS_ESTABLISHED, "", NO_SCALING_FRACTION, MetricType.Counter)
        metricsActor.RegisterMetric(DATA_RECEIVED, "MB", BYTES_TO_MB_SCALING_FRACTION, MetricType.Counter)
        metricsActor.RegisterMetric(DATA_SENT, "MB", BYTES_TO_MB_SCALING_FRACTION, MetricType.Counter)

    override this.OnEventSourceCreated(eventSource) =

        if eventSource.Name = "System.Runtime" || eventSource.Name = "System.Net.Sockets" then

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
            | true, name when name = CPU_USAGE ->

                let value = data["Max"] :?> float
                let metric = { Name = CPU_USAGE; Value = value * DEFAULT_SCALING_FRACTION }

                metricsActor.Publish(ActorMessage.AddMetric metric)

            | true, name when name = WORKING_SET ->

                let value = data["Max"] :?> float
                let metric = { Name = WORKING_SET; Value = value * DEFAULT_SCALING_FRACTION }

                metricsActor.Publish(ActorMessage.AddMetric metric)

            | true, name when name = GC_HEAP_SIZE ->

                let value = data["Max"] :?> float
                let metric = { Name = GC_HEAP_SIZE; Value = value * DEFAULT_SCALING_FRACTION }

                metricsActor.Publish(ActorMessage.AddMetric metric)

            | true, name when name = THREAD_POOL_QUEUE_LENGTH ->

                let value = data["Max"] :?> float
                let metric = { Name = THREAD_POOL_QUEUE_LENGTH; Value = value }

                metricsActor.Publish(ActorMessage.AddMetric metric)

            | true, name when name = THREAD_POOL_QUEUE_COUNT ->

                let value = data["Max"] :?> float
                let metric = { Name = THREAD_POOL_QUEUE_COUNT; Value = value }

                metricsActor.Publish(ActorMessage.AddMetric metric)

            | true, name when name = INCOMING_CONNECTIONS_ESTABLISHED ->

                let value = data["Max"] :?> float
                let metric = { Name = INCOMING_CONNECTIONS_ESTABLISHED; Value = value }

                metricsActor.Publish(ActorMessage.AddMetric metric)

            | true, name when name = OUTGOING_CONNECTIONS_ESTABLISHED ->

                let value = data["Max"] :?> float
                let metric = { Name = OUTGOING_CONNECTIONS_ESTABLISHED; Value = value }

                metricsActor.Publish(ActorMessage.AddMetric metric)

            | true, name when name = BYTES_RECEIVED ->

                let value = data["Max"] :?> float
                let metric = { Name = DATA_RECEIVED; Value = value }

                metricsActor.Publish(ActorMessage.AddMetric metric)

            | true, name when name = BYTES_SENT ->

                let value = data["Max"] :?> float
                let metric = { Name = DATA_SENT; Value = value }

                metricsActor.Publish(ActorMessage.AddMetric metric)

            | _ -> ()
