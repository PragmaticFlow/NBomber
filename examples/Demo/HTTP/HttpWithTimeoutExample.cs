using NBomber.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;
using NBomber.Plugins.Network.Ping;

namespace Demo.HTTP;

public class HttpWithTimeoutExample
{
    public void Run()
    {
        // Docs: https://nbomber.com/docs/nbomber/timeouts

        using var httpClient = new HttpClient();

        var scenario = Scenario.Create("http_scenario", async context =>
        {
            using var timeout = new CancellationTokenSource();
            timeout.CancelAfter(50); // the operation will be canceled after 50 ms

            var request =
                Http.CreateRequest("GET", "https://nbomber.com")
                    .WithHeader("Content-Type", "application/json");
                  //.WithBody(new StringContent("{ some JSON }", Encoding.UTF8, "application/json"));

            // HttpCompletionOption: https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpcompletionoption?view=net-7.0

            var clientArgs = new HttpClientArgs(
                httpCompletion: HttpCompletionOption.ResponseContentRead, // or ResponseHeadersRead
                cancellationToken: timeout.Token
            );

            var response = await Http.Send(httpClient, clientArgs, request);

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
