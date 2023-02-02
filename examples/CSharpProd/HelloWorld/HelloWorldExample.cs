using NBomber.CSharp;

namespace CSharpProd.HelloWorld;

public class HelloWorldExample
{
    public void Run()
    {
        var scenario = Scenario.Create("hello_world_scenario", async context =>
        {
            // you can define and execute any logic here,
            // for example: send http request, SQL query etc
            // NBomber will measure how much time it takes to execute your logic
            await Task.Delay(1000);

            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.RampingInject(rate: 150, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1)), // rump-up to rate 150
            Simulation.Inject(rate: 150, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)),        // keep injecting with rate 150
            Simulation.RampingInject(rate: 0, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1))    // rump-down to rate 0
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
