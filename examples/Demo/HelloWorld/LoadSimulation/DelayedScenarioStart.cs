using NBomber.CSharp;

namespace Demo.HelloWorld.LoadSimulation;

public class DelayedScenarioStart
{
    public void Run()
    {
        // In this example we will delay the start of Scenario via Simulation.Pause
        // The Pause duration will not impact the final stats calculations.

        var scenario = Scenario.Create("scenario_1", async context =>
        {
            await Task.Delay(1_000);
            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.Pause(during: TimeSpan.FromSeconds(10)), // we delay startup of scenario

            Simulation.RampingConstant(copies: 50, during: TimeSpan.FromSeconds(30)),
            Simulation.KeepConstant(copies: 50, during: TimeSpan.FromSeconds(30))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
