using NBomber.CSharp;

namespace Demo.HelloWorld;

public class ScenarioTimerTimeExample
{
    public void Run()
    {
        var scenario = Scenario.Create("hello_world_scenario", async context =>
        {
            var currentScenarioTime = context.GetScenarioTimerTime();

            await Task.Delay(1_000);

            // check log file to see this text
            context.Logger.Information($"Current scenario time: {currentScenarioTime}");

            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(30))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
