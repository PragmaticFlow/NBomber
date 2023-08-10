using System.Net.Http.Json;
using NBomber.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;
using NBomber.Plugins.Network.Ping;

namespace Demo.HTTP;

public class JsonResponse
{
    public string Fact { get; set; }
    public int Length { get; set; }
}

public class HttpResponseValidation
{
    public void Run()
    {
        using var httpClient = new HttpClient();

        var scenario = Scenario.Create("http_scenario", async context =>
        {
            var request = Http.CreateRequest("GET", "https://catfact.ninja/fact");

            var response = await Http.Send(httpClient, request);

            // here, you can do validation of HTTP response

            if (!response.Payload.Value.Headers.Contains("server"))
                return Response.Fail(statusCode: "not found header", sizeBytes: response.SizeBytes);

            // var body = await response.Payload.Value.Content.ReadAsStringAsync();
            var body = await response.Payload.Value.Content.ReadFromJsonAsync<JsonResponse>();
            if (body?.Length > 11111)
                return Response.Fail(statusCode: "small length", sizeBytes: response.SizeBytes);

            return response;
        })
        .WithoutWarmUp()
        .WithLoadSimulations(Simulation.KeepConstant(copies: 1, TimeSpan.FromSeconds(30)));

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithWorkerPlugins(
                new PingPlugin(PingPluginConfig.CreateDefault("nbomber.com")),
                new HttpMetricsPlugin(new [] { HttpVersion.Version1 })
            )
            .Run();
    }
}
