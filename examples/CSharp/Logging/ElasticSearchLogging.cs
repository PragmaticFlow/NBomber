using System;
using System.Threading.Tasks;

using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharp.Logging
{
    // in this example we use:
    // - Serilog.Sinks.Elasticsearch (https://github.com/serilog/serilog-sinks-elasticsearch)
    // to get more info about logging, please visit: (https://nbomber.com/docs/logging)

    public class ElasticSearchLogging
    {
        public static void Run()
        {
            var step = Step.Create("step", async context =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                // this message will be saved to elastic search
                context.Logger.Debug("hello from NBomber");

                return Response.Ok();
            });

            var scenario = ScenarioBuilder
                .CreateScenario("scenario", step)
                .WithoutWarmUp()
                .WithLoadSimulations(new[]
                {
                    Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(30))
                });

            NBomberRunner
                .RegisterScenarios(scenario)
                .WithTestSuite("logging")
                .WithTestName("elastic_search")
                .LoadInfraConfig("./Logging/infra-config.json")
                .Run();
        }
    }
}
