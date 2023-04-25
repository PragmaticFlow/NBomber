module Tests.MetricsStatsActor

open Xunit
open Swensen.Unquote

open NBomber
open NBomber.Contracts.Stats
open NBomber.Domain.Stats.MetricsStatsActor
open Tests.TestHelper

[<Fact>]
let ``BuildReportingStats should calculate correctly for Gauge`` () =

    let env = Dependency.createWithInMemoryLogger NodeType.SingleNode
    let metricsActor = new MetricsStatsActor(env.Dep.Logger)
    let metricName = "my-metric"

    metricsActor.RegisterMetric(metricName, "MB", 100, MetricType.Gauge)

    metricsActor.Publish(AddMetric { Name = metricName; Value = 3.5 })
    metricsActor.Publish(AddMetric { Name = metricName; Value = 2.5 })
    metricsActor.Publish(AddMetric { Name = metricName; Value = 1.5 })

    let allMetrics = metricsActor.BuildReportingStats(seconds 1).GetAwaiter().GetResult()
    let result = allMetrics[0]

    test <@ result.Percentiles.IsNone @>
    test <@ result.Name = metricName @>
    test <@ result.Current = 1.5 @>
    test <@ result.Max = 0 @>

[<Fact>]
let ``BuildReportingStats should calculate correctly for Histogram`` () =

    let env = Dependency.createWithInMemoryLogger NodeType.SingleNode
    let metricsActor = new MetricsStatsActor(env.Dep.Logger)
    let metricName = "my-metric"

    metricsActor.RegisterMetric(metricName, "MB", 100, MetricType.Histogram)

    metricsActor.Publish(AddMetric { Name = metricName; Value = 3.5 })
    metricsActor.Publish(AddMetric { Name = metricName; Value = 2.5 })
    metricsActor.Publish(AddMetric { Name = metricName; Value = 1.5 })

    let allMetrics = metricsActor.BuildReportingStats(seconds 1).GetAwaiter().GetResult()
    let result = allMetrics[0]

    test <@ result.Percentiles.IsNone @>
    test <@ result.Name = metricName @>
    test <@ result.Current = 1.5 @>
    test <@ result.Max = 3.5 @>

[<Fact>]
let ``BuildReportingStats should handle invalid args`` () =

    let env = Dependency.createWithInMemoryLogger NodeType.SingleNode
    let metricsActor = new MetricsStatsActor(env.Dep.Logger)
    let metricName = "my-metric"

    // case 1
    let result1 = metricsActor.BuildReportingStats(seconds 1).GetAwaiter().GetResult()

    // case 2
    metricsActor.RegisterMetric(metricName, "MB", 100, MetricType.Histogram)
    let result2 = metricsActor.BuildReportingStats(seconds 1).GetAwaiter().GetResult()

    // case 3
    metricsActor.Publish(AddMetric { Name = "a"; Value = 3.5 })
    let result3 = metricsActor.BuildReportingStats(seconds 1).GetAwaiter().GetResult()

    test <@ result1.Length = 0 @>
    test <@ result2[0].Current = 0 @>
    test <@ result3[0].Current = 0 @>

[<Fact>]
let ``GetFinalStats should calculate correctly for Gauge`` () =

    let env = Dependency.createWithInMemoryLogger NodeType.SingleNode
    let metricsActor = new MetricsStatsActor(env.Dep.Logger)
    let metricName = "my-metric"

    metricsActor.RegisterMetric(metricName, "MB", 100, MetricType.Gauge)

    metricsActor.Publish(AddMetric { Name = metricName; Value = 3.5 })
    metricsActor.Publish(AddMetric { Name = metricName; Value = 2.5 })
    metricsActor.Publish(AddMetric { Name = metricName; Value = 1.5 })

    let allMetrics = metricsActor.GetFinalStats(seconds 1).GetAwaiter().GetResult()
    let result = allMetrics[0]

    test <@ result.Percentiles.IsNone @>
    test <@ result.Name = metricName @>
    test <@ result.Current = 1.5 @>
    test <@ result.Max = 0 @>

[<Fact>]
let ``GetFinalStats should calculate correctly for Histogram`` () =

    let env = Dependency.createWithInMemoryLogger NodeType.SingleNode
    let metricsActor = new MetricsStatsActor(env.Dep.Logger)
    let metricName = "my-metric"

    metricsActor.RegisterMetric(metricName, "MB", 100, MetricType.Histogram)

    metricsActor.Publish(AddMetric { Name = metricName; Value = 3.5 })
    metricsActor.Publish(AddMetric { Name = metricName; Value = 2.5 })
    metricsActor.Publish(AddMetric { Name = metricName; Value = 1.5 })

    let allMetrics = metricsActor.GetFinalStats(seconds 1).GetAwaiter().GetResult()
    let result = allMetrics[0]

    test <@ result.Percentiles.IsSome @>
    test <@ result.Name = metricName @>
    test <@ result.Current = 1.5 @>
    test <@ result.Max = 3.5 @>
    test <@ result.Percentiles.Value.Mean = 2.5 @>
    test <@ result.Percentiles.Value.Percent50 = 2.5 @>
    test <@ result.Percentiles.Value.Percent99 = 3.5 @>

[<Fact>]
let ``GetFinalStats should handle invalid args`` () =

    let env = Dependency.createWithInMemoryLogger NodeType.SingleNode
    let metricsActor = new MetricsStatsActor(env.Dep.Logger)
    let metricName = "my-metric"

    // case 1
    let result1 = metricsActor.GetFinalStats(seconds 1).GetAwaiter().GetResult()

    // case 2
    metricsActor.RegisterMetric(metricName, "MB", 100, MetricType.Histogram)
    let result2 = metricsActor.GetFinalStats(seconds 1).GetAwaiter().GetResult()

    // case 3
    metricsActor.Publish(AddMetric { Name = "a"; Value = 3.5 })
    let result3 = metricsActor.GetFinalStats(seconds 1).GetAwaiter().GetResult()

    test <@ result1.Length = 0 @>
    test <@ result2[0].Current = 0 @>
    test <@ result3[0].Current = 0 @>

[<Fact>]
let ``CleanAllMetrics should clean stats`` () =

    let env = Dependency.createWithInMemoryLogger NodeType.SingleNode
    let metricsActor = new MetricsStatsActor(env.Dep.Logger)
    let metricName = "my-metric"

    metricsActor.RegisterMetric(metricName, "MB", 100, MetricType.Gauge)

    metricsActor.Publish(AddMetric { Name = metricName; Value = 3.5 })
    metricsActor.Publish(AddMetric { Name = metricName; Value = 2.5 })

    let result1 = metricsActor.GetFinalStats(seconds 1).GetAwaiter().GetResult()

    metricsActor.Publish CleanAllMetrics

    let result2 = metricsActor.GetFinalStats(seconds 1).GetAwaiter().GetResult()

    test <@ result1[0].Current = 2.5 @>
    test <@ result2[0].Current = 0 @>
