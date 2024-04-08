using NBomber.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;

namespace Demo.HTTP;

public class HttpRequestTracing
{
    public void Run()
    {
        using var httpClient = new HttpClient();

        var scenario = Scenario.Create("http_scenario", async context =>
        {
            var request = Http.CreateRequest("GET", "https://catfact.ninja/fact");

            var clientArgs = HttpClientArgs.Create(logger: context.Logger);

            var response = await Http.Send(httpClient, clientArgs, request);

            return response;
        })
        .WithoutWarmUp()
        .WithLoadSimulations(Simulation.Inject(rate: 1, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)));

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithWorkerPlugins(
                new HttpMetricsPlugin(new [] { HttpVersion.Version1 })
            )
            .Run();
    }
}
