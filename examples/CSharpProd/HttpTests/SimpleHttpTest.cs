using System;
using System.Net.Http;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using NBomber.Plugins.Network.Ping;

namespace CSharpProd.HttpTests
{
    // in this example we use:
    // - NBomber.Http (https://nbomber.com/docs/plugins-http)

    public class SimpleHttpTest
    {
        public static void Run()
        {
            using var httpClient = new HttpClient();
            var step = Step.Create("fetch_html_page", async context =>
            {
                var response = await httpClient.GetAsync("https://nbomber.com", context.CancellationToken);

                return response.IsSuccessStatusCode
                    ? Response.Ok(statusCode: (int) response.StatusCode)
                    : Response.Fail(statusCode: (int) response.StatusCode);
            });

            var scenario = ScenarioBuilder
                .CreateScenario("simple_http", step)
                .WithWarmUpDuration(TimeSpan.FromSeconds(5))
                .WithLoadSimulations(new[]
                {
                    Simulation.InjectPerSec(rate: 100, during: TimeSpan.FromSeconds(30))
                });

            var pingPluginConfig = PingPluginConfig.CreateDefault(new[] {"nbomber.com"});
            var pingPlugin = new PingPlugin(pingPluginConfig);

            NBomberRunner
                .RegisterScenarios(scenario)
                .WithWorkerPlugins(pingPlugin)
                .Run();
        }
    }
}
