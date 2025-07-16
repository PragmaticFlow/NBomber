using NBomber.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;
using NBomber.Plugins.Network.Ping;
using System.Text;

namespace Demo.HTTP;

public class SimpleHttpExample
{
    public void Run()
    {
        var httpClient = Http.CreateDefaultClient();

        var scenario = Scenario.Create("http_scenario", async context =>
        {
            var request =
                Http.CreateRequest("GET", "https://nbomber.com")
                    .WithHeader("Content-Type", "application/json");
                 // .WithBody(new StringContent("{ some JSON }", Encoding.UTF8, "application/json"));

            var response = await Http.Send(httpClient, request);

            return response;
        })
        .WithWarmUpDuration(TimeSpan.FromSeconds(3))
        .WithLoadSimulations(
            Simulation.RampingInject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1)),
            Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1)),
            Simulation.RampingInject(rate: 0, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithWorkerPlugins(
                new PingPlugin(PingPluginConfig.CreateDefault("nbomber.com")),
                new HttpMetricsPlugin([HttpVersion.Version1])
            )
            .Run();
    }
}
