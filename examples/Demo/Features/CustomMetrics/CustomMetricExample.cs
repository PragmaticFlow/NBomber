using NBomber.Contracts;
using NBomber.CSharp;

namespace Demo.Features.CustomMetrics;

public class CustomMetricExample
{
    public void Run()
    {
        // define custom metrics
        var counter = Metric.CreateCounter("my-counter", "MB");
        var gauge = Metric.CreateGauge("my-gauge", "KB");

        var scenario = Scenario.Create("hello_world_scenario", async context =>
        {
            await Task.Delay(500);

            counter.Add(1);
            gauge.Set(6.5);

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
        );

        var stats = NBomberRunner
            .RegisterScenarios(scenario)
            .Run();

        // We can retrieve the final values.
        var counterValue = stats.Metrics.Counters.Find("my-counter").Value;
        var gaugeValue = stats.Metrics.Gauges.Find("my-gauge").Value;
    }
}
