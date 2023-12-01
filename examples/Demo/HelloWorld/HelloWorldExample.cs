using NBomber.CSharp;

namespace Demo.HelloWorld;

public class HelloWorldExample
{
    public void Run()
    {
        var scenario = Scenario.Create("hello_world_scenario", async context =>
        {
            // you can define and execute any logic here,
            // for example: send http request, SQL query etc
            // NBomber will measure how much time it takes to execute your logic
            await Task.Delay(500);

            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.Inject(rate: 150, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)) // keep injecting with rate 150
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
