using NBomber.CSharp;

namespace Demo.Features.DynamicWorkload;

public class ScenarioWeightExample
{
    public void Run()
    {
        var readScenario = Scenario.Create("read_scenario", async context =>
        {
            await Task.Delay(100);
            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            // we use the same LoadSimulation settings
            Simulation.Inject(rate: 10, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30))
        )
        .WithWeight(80); // sets 80%

        var writeScenario = Scenario.Create("write_scenario", async context =>
        {
            await Task.Delay(100);
            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            // we use the same LoadSimulation settings
            Simulation.Inject(rate: 10, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30))
        )
        .WithWeight(20); // sets 20%

        NBomberRunner
            .RegisterScenarios(readScenario, writeScenario)
            .Run();
    }
}
