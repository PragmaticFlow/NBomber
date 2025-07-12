using NBomber.CSharp;
using NBomber.Http.CSharp;

namespace Demo.Features.Thresholds;

public class ThresholdsConfigExample
{
    public void Run()
    {
        // more info about Runtime Thresholds can be found by the following link
        // https://nbomber.com/docs/nbomber/asserts_and_thresholds#runtime-thresholds

        using var httpClient = Http.CreateDefaultClient();

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
            .LoadConfig("Features/Thresholds/nbomber-config.json") // you can define Thresholds via JSON Config
            .Run();

        // Here, we attempt to find a failed threshold
        var failedThreshold = result.Thresholds.FirstOrDefault(x => x.IsFailed);
    }
}
