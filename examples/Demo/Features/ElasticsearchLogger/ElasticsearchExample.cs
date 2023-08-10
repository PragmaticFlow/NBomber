using NBomber.CSharp;
using Serilog;

namespace Demo.Features.ElasticsearchLogger;

public class ElasticsearchExample
{
    public void Run()
    {
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
            .Run();
    }
}