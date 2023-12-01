using NBomber.CSharp;

namespace Demo.HelloWorld.LoadSimulation;

public class ScenarioTotalIterations
{
    public void Run()
    {
        var scenario = Scenario.Create("hello_world_scenario", async context =>
            {
                await Task.Delay(1_000);

                return Response.Ok();
            })
            .WithoutWarmUp()
            .WithLoadSimulations(
                Simulation.IterationsForConstant(copies: 50, iterations: 1000)

                // or you can use
                // Simulation.IterationsForInject(rate: 50, interval: TimeSpan.FromSeconds(1), iterations: 1000)
            );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
