using NBomber.Contracts;
using NBomber.CSharp;

namespace Demo.Features.CustomMetrics;

public class CustomMetricsExample
{
    public void Run()
    {
        // define custom metrics
        var counter = Metric.CreateCounter("my-counter", unitOfMeasure: "MB");
        var gauge = Metric.CreateGauge("my-gauge", unitOfMeasure: "KB");

        var scenario = Scenario.Create("scenario", async context =>
        {
            await Task.Delay(500);

            counter.Add(1); // tracks a value that may increase or decrease over time
            gauge.Set(6.5); // set the current value of the metric

            return Response.Ok();
        })
        .WithInit(ctx =>
        {
            // register custom metrics
            ctx.RegisterMetric(counter);
            ctx.RegisterMetric(gauge);

            return Task.CompletedTask;
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.Inject(rate: 150, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)) // keep injecting with rate 150
        )
        .WithThresholds(
            Threshold.Create(metric => metric.Counters.Get("my-counter").Value < 1000),
            Threshold.Create(metric => metric.Gauges.Get("my-gauge").Value >= 6.5)
        );

        var stats = NBomberRunner
            .RegisterScenarios(scenario)
            //.LoadConfig("./Features/CustomMetrics/nbomber-config.json")
            .Run();

        // We can retrieve the final values.
        var counterValue = stats.Metrics.Counters.Find("my-counter").Value;
        var gaugeValue = stats.Metrics.Gauges.Find("my-gauge").Value;
    }
}
