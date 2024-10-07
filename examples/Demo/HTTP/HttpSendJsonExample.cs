using System.Text.Json;
using NBomber.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;
using NBomber.Plugins.Network.Ping;

namespace Demo.HTTP;

public class UserData
{
    public int UserId { get; set; }
    public int Id { get; set; }
    public string Title { get; set; }
    public bool Completed { get; set; }
}

public class HttpSendJsonExample
{
    public void Run()
    {
        // sets global JsonSerializerOptions to use CamelCase naming
        Http.GlobalJsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        using var httpClient = new HttpClient();

        var scenario = Scenario.Create("http_scenario", async context =>
        {
            // example of WithJsonBody
            var user = new UserData { UserId = 1, Title = "anton" };

            var request1 =
                Http.CreateRequest("GET", "https://nbomber.com")
                    .WithJsonBody(user);

            var response1 = await Http.Send(httpClient, request1);

            // example of typed Send<T> that deserialize JSON response
            var request2 =
                Http.CreateRequest("GET", "https://jsonplaceholder.typicode.com/todos/1")
                    .WithHeader("Content-Type", "application/json");

            var response2 = await Http.Send<UserData>(httpClient, request2);

            var userData = response2.Payload.Value.Data;
            var title = userData.Title;
            var userId = userData.UserId;

            return response2;
        })
        .WithoutWarmUp()
        .WithLoadSimulations(Simulation.Inject(rate: 5, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)));

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithWorkerPlugins(
                new PingPlugin(PingPluginConfig.CreateDefault("nbomber.com")),
                new HttpMetricsPlugin(new [] { HttpVersion.Version1 })
            )
            .Run();
    }
}
