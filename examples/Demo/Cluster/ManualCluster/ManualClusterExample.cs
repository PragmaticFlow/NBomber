using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;
using NBomber.Plugins.Network.Ping;

namespace Demo.Cluster.ManualCluster;

public class ManualClusterExample
{
    readonly HttpClient _httpClient = new();

    public void Run(string[] args)
    {
        var scenario = BuildScenario();
        StartNode(scenario, args);
    }

    private void StartNode(ScenarioProps scenario, string[] args)
    {
        NBomberRunner
            .RegisterScenarios(scenario)
            .WithWorkerPlugins(
                new PingPlugin(PingPluginConfig.CreateDefault("nbomber.com")),
                new HttpMetricsPlugin(new [] { HttpVersion.Version1 })
            )
            .LoadConfig("Cluster/ManualCluster/manual-cluster-config.json") // you can use: --config=Cluster/ManualCluster/manual-cluster-config.json
            .EnableLocalDevCluster(true)                                    // you can use: --cluster-local-dev=true
            .Run(args);                                                     // more info about available CLI args: https://nbomber.com/docs/getting-started/cli/
    }

    private ScenarioProps BuildScenario()
    {
        return Scenario.Create("http_scenario", async context =>
            {
                var request =
                    Http.CreateRequest("GET", "https://nbomber.com")
                        .WithHeader("Content-Type", "application/json");
                        // .WithBody(new StringContent("{ some JSON }", Encoding.UTF8, "application/json"));

                var response = await Http.Send(_httpClient, request);

                return response;
            })
            .WithoutWarmUp()
            .WithLoadSimulations(Simulation.Inject(rate: 10, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)));
    }
}
