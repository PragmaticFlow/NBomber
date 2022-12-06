using NBomber.CSharp;

namespace CSharpProd.HelloWorld.LoadSimulation;

public class ScenarioKeepConstant
{
    public void Run()
    {
        // Closed systems, where you keep a constant number of concurrent clients and they waiting on a response before sending a new request.
        // A good example will be a database with 20 concurrent clients that constantly repeat sending query then wait for a response and do it again.
        // Under the big load, requests will be queued and this queue will not grow since we have a finite number of clients.
        // Usually, in real-world scenarios systems with persisted connections (RabbitMq, Kafka, WebSockets, Databases) are tested as closed systems.
        //
        // NBomber provides 2 simulations to mode open system: [RampingConstant, KeepConstant]
        //
        // [RampingConstant]
        // Injects a given number of scenario copies (threads) with a linear ramp over a given duration.
        // Every single scenario copy will iterate while the specified duration.
        // Use it for ramp up and rump down.
        //
        // [KeepConstant]
        // A fixed number of scenario copies (threads) executes as many iterations as possible for a specified amount of time.
        // Every single scenario copy will iterate while the specified duration.
        // Use it when you need to run a specific amount of scenario copies (threads) for a certain amount of time.

        var scenario = Scenario.Create("hello_world_scenario", async context =>
        {
            await Task.Delay(1_000);

            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.RampingConstant(copies: 50, during: TimeSpan.FromSeconds(30)),
            Simulation.KeepConstant(copies: 50, during: TimeSpan.FromSeconds(30))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
