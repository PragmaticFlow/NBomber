using NBomber.CSharp;
using Serilog;

namespace Demo.Features.Logger.Elasticsearch;

public class ElasticsearchLogger
{
    public void Run()
    {
        // Docs:
        // - https://nbomber.com/docs/nbomber/logger
        // - https://nbomber.com/docs/nbomber/logger#storing-logs-in-databases

        var scenario = Scenario.Create("hello_world_scenario", async context =>
        {
            await Task.Delay(1000);

            context.Logger.Debug("my log message: {0}", context.InvocationNumber);

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
                    .WriteTo.Elasticsearch(
                        nodeUris: "http://localhost:9200",
                        indexFormat: "nbomber-{0:yyyy.MM.dd}")
            )
            // or you can use JSON config
            // .LoadInfraConfig("infra-config.json")
            .Run();
    }
}
