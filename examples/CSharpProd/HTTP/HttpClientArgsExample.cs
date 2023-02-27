using NBomber.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;

namespace CSharpProd.HTTP;

public class HttpClientArgsExample
{
    public void Run()
    {
        using var httpClient = new HttpClient();

        var scenario = Scenario.Create("http_scenario", async context =>
        {
            var request =
                Http.CreateRequest("GET", "https://nbomber.com")
                    .WithHeader("Accept", "text/html")
                    .WithBody(new StringContent("{ some JSON }"));

            // HttpCompletionOption: https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpcompletionoption?view=net-7.0
            
            var clientArgs = new HttpClientArgs(
                httpCompletion: HttpCompletionOption.ResponseContentRead, // or ResponseHeadersRead
                cancellationToken: CancellationToken.None
            );

            var response = await Http.Send(httpClient, clientArgs, request);

            return response;
        })
        .WithoutWarmUp()
        .WithLoadSimulations(Simulation.Inject(rate: 100, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)));

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}