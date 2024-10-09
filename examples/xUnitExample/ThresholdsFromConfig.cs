using NBomber.CSharp;
using NBomber.Http.CSharp;
using Xunit;

namespace xUnitExample;

public class ThresholdsFromConfig
{
    [Fact]
    public void Runtime_Thresholds_Example()
    {
        using var httpClient = new HttpClient();

        var scenario = Scenario.Create("http_scenario", async context =>
        {
            var step1 = await Step.Run("step_1", context, async () =>
            {
                var request =
                    Http.CreateRequest("GET", "https://nbomber.com")
                        .WithHeader("Content-Type", "application/json");

                var response = await Http.Send(httpClient, request);

                return response;
            });

            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.Inject(rate: 1, interval: TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30))
        );

        var result = NBomberRunner
            .RegisterScenarios(scenario)
            .LoadConfig("Configs/nbomber-config.json")
            .Run();

        // Here, we attempt to find a failed threshold, and if one is found (i.e., it is not null), we throw an exception.
        var failedThreshold = result.Thresholds.FirstOrDefault(x => x.IsFailed);

        Assert.True(failedThreshold != null);
    }
}
