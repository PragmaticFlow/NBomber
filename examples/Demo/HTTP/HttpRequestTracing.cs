using NBomber.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;

namespace Demo.HTTP;

public class HttpRequestTracing
{
    public void Run()
    {
        var httpClient = Http.CreateDefaultClient();

        var scenario = Scenario.Create("http_scenario", async context =>
        {
            var request = Http.CreateRequest("GET", "https://catfact.ninja/fact");

            var response = await httpClient.SendRequest(request, logger: context.Logger);

            return response;
        })
        .WithWarmUpDuration(TimeSpan.FromSeconds(3))
        .WithLoadSimulations(Simulation.Inject(rate: 1, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)));

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithWorkerPlugins(
                new HttpMetricsPlugin(new [] { HttpVersion.Version1 })
            )
            .Run();
    }
}
