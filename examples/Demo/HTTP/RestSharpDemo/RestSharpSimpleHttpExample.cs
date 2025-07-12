using NBomber.CSharp;
using NBomber.Http;
using NBomber.Plugins.Network.Ping;
using NBomber.RestSharp;
using RestSharp;

namespace Demo.HTTP.RestSharpDemo;

public class RestSharpSimpleHttpExample
{
    public void Run()
    {
        // For this example, you'll need to start the HttpApiSimulator, which is located in the examples/simulators solution folder.
        // Make sure it’s running before executing the client tests to ensure proper communication.

        var host = "localhost:60529";
        var options = new RestClientOptions($"http://{host}");
        var client = RestClientBuilder.CreateDefaultClient(options);

        var scenario = Scenario.Create("http_scenario", async ctx =>
        {
            var userId = Random.Shared.Next(0, 1_000);

            var step1 = await Step.Run("add_user", ctx, async () =>
            {
                var request = new RestRequest("api/users/{id}");
                request.AddParameter("id", userId, ParameterType.UrlSegment);
                request.AddJsonBody(new UpdateUserReq("name", "last_name", 42));

                var response = await client.SendPut(request);
                return response;
            });

            var step2 = await Step.Run("get_user", ctx, async () =>
            {
                var request = new RestRequest("api/users/{id}");
                request.AddParameter("id", userId, ParameterType.UrlSegment);

                var response = await client.SendGet<User>(request);
                return response;
            });

            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.RampingInject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1)),
            Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1)),
            Simulation.RampingInject(rate: 0, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithWorkerPlugins(
                new PingPlugin(PingPluginConfig.CreateDefault(host)),
                new HttpMetricsPlugin([HttpVersion.Version1])
            )
            .Run();
    }
}
