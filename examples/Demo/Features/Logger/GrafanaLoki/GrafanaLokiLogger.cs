using NBomber.CSharp;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

namespace Demo.Features.Logger.GrafanaLoki;

public class GrafanaLokiLogger
{
    public void Run()
    {
        var scenario = Scenario.Create("hello_world_scenario", async context =>
            {
                await Task.Delay(1000);

                context.Logger.Information("my log message: {0}", context.InvocationNumber);

                return Response.Ok();
            })
            .WithoutWarmUp()
            .WithLoadSimulations(
                Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(30))
            );

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithLoggerConfig(() =>
                new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.GrafanaLoki("http://localhost:3100",
                        new [] {new LokiLabel {Key = "application", Value = "NBomber"}})
            )
            .Run();
    }
}
