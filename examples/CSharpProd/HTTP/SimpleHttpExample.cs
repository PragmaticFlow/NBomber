using NBomber.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;
using NBomber.Plugins.Network.Ping;
using System.Text;

namespace CSharpProd.HTTP;

public class SimpleHttpExample
{
    public void Run()
    {
        using var httpClient = new HttpClient();

        var scenario = Scenario.Create("http_scenario", async context =>
        {
            var request =
                Http.CreateRequest("GET", "https://nbomber.com")
                    .WithHeader("Accept", "text/html");
                    //.WithBody(new StringContent("{ some JSON }", Encoding.UTF8, "application/json"));

            var response = await Http.Send(httpClient, request);

            return response;
        })
        .WithoutWarmUp()
        .WithLoadSimulations(Simulation.Inject(rate: 100, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)));

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithWorkerPlugins(
                new PingPlugin(PingPluginConfig.CreateDefault("nbomber.com")),
                new HttpMetricsPlugin(new [] { HttpVersion.Version1 })
            )
            .Run();
    }
}
