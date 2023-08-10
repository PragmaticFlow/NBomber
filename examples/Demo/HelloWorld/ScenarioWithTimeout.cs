using NBomber.CSharp;

namespace Demo.HelloWorld;

public class ScenarioWithTimeout
{
    public void Run()
    {
        var scenario = Scenario.Create("scenario_with_timeout", async context =>
        {
            using var timeout = new CancellationTokenSource();
            timeout.CancelAfter(600);

            await Task.Delay(1000, timeout.Token);

            return Response.Ok(statusCode: "200", sizeBytes: 1000);
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.KeepConstant(copies: 50, during: TimeSpan.FromSeconds(30))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
