using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using Xunit;

namespace xUnitExample;

public class Thresholds
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
        .WithLoadSimulations(Simulation.Inject(rate: 1, interval: TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30)))
        .WithThresholds(
            // Scenario's threshold that checks: error rate < 10%
            Threshold.Create(scenarioStats => scenarioStats.Fail.Request.Percent < 10),

            // Step's threshold that checks: error rate < 10%
            Threshold.Create("step_1", stepStats => stepStats.Fail.Request.Percent < 10),

            // Scenario's threshold that checks if any response contains status code 404
            Threshold.Create(
                scenarioStats => scenarioStats.Fail.StatusCodes.Exists("404"),

                abortWhenErrorCount: 5,  // Threshold checks are executed based on the ReportingInterval (by default each 5 sec),
                                         // so when the threshold ErrorCount = 5, the load test will be aborted.
                                         // By default, 'abortWhenErrorCount' is null meaning Thresholds errors will not abort the test execution.


                startCheckAfter: TimeSpan.FromSeconds(10) // Threshold check will be delayed on 10 sec
            ),

            Threshold.Create(scenarioStats => scenarioStats.Ok.StatusCodes.Find("200")?.Percent >= 80)
        );

        var result = NBomberRunner
            .RegisterScenarios(scenario)
            .Run();

        // Here, we attempt to find a failed threshold, and if one is found (i.e., it is not null), we throw an exception.
        var failedThreshold = result.Thresholds.FirstOrDefault(x => x.IsFailed);

        Assert.True(failedThreshold == null);
    }
}
