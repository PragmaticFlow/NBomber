using NBomber.CSharp;

namespace Demo.HelloWorld.LoadSimulation;

public class ScenarioInjectRate
{
    public void Run()
    {
        // Open systems - where you keep arrival rate of new clients requests without waitng on responses.
        // The good example could be some popular website like Amazon.
        // Under the load new clients arrive even though applications have trouble serving them.
        // Usually, in real-world scenarios systems that use stateless protocols like HTTP are tested as open systems.
        //
        // NBomber provides 3 simulations to mode open system: [RampingInject, Inject, InjectRandom]
        //
        // [RampingInject]
        // Injects a given number of scenario copies (threads) from the current rate to the target rate during a given duration.
        // Every single scenario copy will run only once.
        //
        // [Inject]
        // Injects a given number of scenario copies (threads) during a given duration.
        // Every single scenario copy will run only once.
        // Use it when you want to maintain a constant rate of requests without being affected by the performance of the system under test.
        //
        // [InjectRandom]
        // Injects a random number of scenario copies (threads) during a given duration.
        // Every single scenario copy will run only once.
        // Use it when you want to maintain a random rate of requests without being affected by the performance of the system under test.

        var scenario = Scenario.Create("hello_world_scenario", async context =>
        {
            await Task.Delay(1_000);

            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.RampingInject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1)),
            Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1)),
            Simulation.InjectRandom(minRate: 10, maxRate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
