using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Sinks.OpenTelemetry;

namespace Demo.Features.RealtimeReporting.OpenTelemetry;

public class OpenTelemetryExample
{
    public void Run()
    {
        var counterStep1 = Metric.CreateCounter("my-counter-step-1", unitOfMeasure: "MB");
        var gaugeStep1 = Metric.CreateGauge("my-gauge-step-1", unitOfMeasure: "KB");
        var counterStep2 = Metric.CreateCounter("my-counter-step-2", unitOfMeasure: "MB");
        var gaugeStep2 = Metric.CreateGauge("my-gauge-step-2", unitOfMeasure: "KB");

        var scenario = Scenario.Create("scenario", async context =>
            {
                await Step.Run("step_1", context, async () =>
                {
                    await Task.Delay(500);

                    counterStep1.Add(1); // tracks a value that may increase or decrease over time
                    gaugeStep1.Set(6.5); // set the current value of the metric

                    return Response.Ok();
                });

                await Step.Run("step_2", context, async () =>
                {
                    await Task.Delay(500);

                    counterStep2.Add(3); // tracks a value that may increase or decrease over time
                    gaugeStep2.Set(15); // set the current value of the metric

                    return Response.Ok();
                });

                return Response.Ok();
            })
            .WithInit(ctx =>
            {
                // register custom metrics
                ctx.RegisterMetric(counterStep1);
                ctx.RegisterMetric(counterStep2);
                ctx.RegisterMetric(gaugeStep1);
                ctx.RegisterMetric(gaugeStep2);

                return Task.CompletedTask;
            })
            .WithoutWarmUp()
            .WithLoadSimulations(
                Simulation.Inject(rate: 5, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(60)) // keep injecting with rate 5
            );

        var stats = NBomberRunner
            .RegisterScenarios(scenario)
            .LoadInfraConfig("./Features/RealtimeReporting/OpenTelemetry/infra-config.json")
            .WithoutReports()
            .WithReportingSinks(new OpenTelemetrySink())
            .Run();
    }
}
