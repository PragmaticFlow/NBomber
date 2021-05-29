using System;
using System.Net.Http;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using NBomber.Plugins.Network.Ping;
using Serilog;

namespace CSharpProd.HttpTests
{
    // in this example we will log every raquest/response to local log file
    // in this example we use:
    // - NBomber.Http (https://nbomber.com/docs/plugins-http)

    public class TracingHttp
    {
        public static void Run()
        {
            var httpFactory = HttpClientFactory.Create();

            var step = Step.Create("fetch_html_page",
                                   clientFactory: httpFactory,
                                   execute: async context =>
            {
                var response = await context.Client.GetAsync("https://nbomber.com", context.CancellationToken);

                return response.IsSuccessStatusCode
                    ? Response.Ok(statusCode: (int) response.StatusCode)
                    : Response.Fail(statusCode: (int) response.StatusCode);
            });

            var scenario = ScenarioBuilder
                .CreateScenario("nbomber_web_site", step)
                .WithWarmUpDuration(TimeSpan.FromSeconds(5))
                .WithLoadSimulations(new[]
                {
                    Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(30))
                });

            var pingPluginConfig = PingPluginConfig.CreateDefault(new[] {"nbomber.com"});
            var pingPlugin = new PingPlugin(pingPluginConfig);

            NBomberRunner
                .RegisterScenarios(scenario)
                .WithWorkerPlugins(pingPlugin)
                .WithTestSuite("http")
                .WithTestName("tracing_test")
                .WithLoggerConfig(() => new LoggerConfiguration().MinimumLevel.Verbose()) // set log to verbose
                .Run();
        }
    }
}
