using System;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using NBomber.Plugins.Network.Ping;

namespace CSharp.Examples.Scenarios
{
    class HttpScenario
    {
        public static void Run(string[] args)
        {
            // in this example we use NBomber.Http package which simplifies writing HTTP requests
            // you can find more here: https://github.com/PragmaticFlow/NBomber.Http

            var step = HttpStep.Create("http pull", context =>
                Http.CreateRequest("GET", "https://nbomber.com")
                    .WithHeader("Accept", "text/html")
                    //.WithHeader("Cookie", "cookie1=value1; cookie2=value2")
                    //.WithBody(new StringContent("{ some JSON }", Encoding.UTF8, "application/json"))
                    //.WithCheck(response => Task.FromResult(response.IsSuccessStatusCode))
            );

            var scenario = ScenarioBuilder.CreateScenario("test_nbomber", new[] {step})
                .WithWarmUpDuration(TimeSpan.FromSeconds(10))
                .WithLoadSimulations(new[]
                {
                    Simulation.InjectScenariosPerSec(copiesCount: 100, during: TimeSpan.FromSeconds(30))
                });

            var pingPluginConfig = PingPluginConfig.Create(new[] {"nbomber.com"});
            var pingPlugin = new PingPlugin(pingPluginConfig);

            NBomberRunner
                .RegisterScenarios(new[] {scenario})
                .WithPlugins(new[] { pingPlugin })
                .RunInConsole();
        }
    }
}
