using System;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using NBomber.Plugins.Network.Ping;

namespace CSharp.HttpTests
{
    // in this example we use:
    // - NBomber.Http (https://nbomber.com/docs/plugins-http)

    public class SimpleHttpTest
    {
        public static void Run()
        {
            var step = HttpStep.Create("fetch_html_page", context =>
                Http.CreateRequest("GET", "https://nbomber.com")
                    .WithHeader("Accept", "text/html")
            );

            var scenario = ScenarioBuilder
                .CreateScenario("nbomber_web_site", step)
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
                .WithTestSuite("http")
                .WithTestName("simple_test")
                .Run();
        }
    }
}
