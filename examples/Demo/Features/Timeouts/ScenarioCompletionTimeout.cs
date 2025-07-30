using NBomber.CSharp;

namespace Demo.Features.Timeouts;

public class ScenarioCompletionTimeout
{
    public void Run()
    {
        // Docs: https://nbomber.com/docs/nbomber/timeouts

        // When NBomber finishes load tests, it waits for all running scenarios to complete their tasks.

        var scenario = Scenario.Create("scenario_1", async context =>
        {
            await Task.Delay(8_000); // set 8 sec, but the duration of the scenario is only 1 sec.
            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(1))); // set 1 sec duration

        var result =
            NBomberRunner
                .RegisterScenarios(scenario)
                .WithScenarioCompletionTimeout(TimeSpan.FromSeconds(10)) // we set 10 sec to wait
                .Run();

        Console.WriteLine(result.AllOkCount == 1
            ? "Scenario completed"
            : "Scenario not completed"
        );
    }
}
