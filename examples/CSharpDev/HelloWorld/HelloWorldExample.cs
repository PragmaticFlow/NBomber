using System;
using System.Threading.Tasks;
using NBomber.CSharp;

namespace CSharpDev.HelloWorld;

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
            Simulation.RampConstant(copies: 50, during: TimeSpan.FromMinutes(1)),
            Simulation.KeepConstant(copies: 50, during: TimeSpan.FromSeconds(30))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
