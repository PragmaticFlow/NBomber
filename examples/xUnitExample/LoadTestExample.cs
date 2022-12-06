using NBomber.CSharp;
using NBomber.Http.CSharp;
using Xunit;

namespace xUnitExample;

public class LoadTestExample
{
    [Fact]
    public void SimpleHttpExample()
    {
        using var httpClient = new HttpClient();

        var scenario = Scenario.Create("http_scenario", async context =>
        {
            var step1 = await Step.Run("step_1", context, async () =>
            {
                var request =
                    Http.CreateRequest("GET", "https://nbomber.com")
                        .WithHeader("Accept", "text/html")
                        .WithBody(new StringContent("{ some JSON }"));

                var response = await Http.Send(httpClient, request);

                return response;
            });

            var step2 = await Step.Run("step_2", context, async () =>
            {
                var request =
                    Http.CreateRequest("GET", "https://nbomber.com")
                        .WithHeader("Accept", "text/html")
                        .WithBody(new StringContent("{ some JSON }"));

                var response = await Http.Send(httpClient, request);

                return response;
            });

            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(Simulation.Inject(rate: 10, interval: TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10)));

        var result = NBomberRunner
            .RegisterScenarios(scenario)
            .Run();

        var scnStats = result.GetScenarioStats("http_scenario");
        var step1Stats = scnStats.GetStepStats("step_1");
        var step2Stats = scnStats.GetStepStats("step_2");

        Assert.True(result.AllBytes > 0);
        Assert.True(result.AllRequestCount > 0);
        Assert.True(result.AllOkCount > 0);
        Assert.True(result.AllFailCount == 0);

        Assert.True(scnStats.Ok.Request.RPS > 0);
        Assert.True(scnStats.Ok.Request.Count > 0);
        Assert.True(scnStats.Ok.Latency.MinMs > 0);
        Assert.True(scnStats.Ok.Latency.MaxMs > 0);
        Assert.True(scnStats.Fail.Request.Count == 0);
        Assert.True(scnStats.Fail.Request.Count == 0);
        Assert.True(scnStats.Fail.Latency.MinMs == 0);

        Assert.True(step1Stats.Ok.Latency.Percent50 > 0);
        Assert.True(step1Stats.Ok.Latency.Percent75 > 0);
        Assert.True(step1Stats.Ok.Latency.Percent99 > 0);

        Assert.True(step2Stats.Ok.DataTransfer.MinBytes > 0);
        Assert.True(step2Stats.Ok.DataTransfer.MaxBytes > 0);
        Assert.True(step2Stats.Ok.DataTransfer.Percent99 > 0);
        Assert.True(step2Stats.Ok.DataTransfer.AllBytes > 0);
    }
}
