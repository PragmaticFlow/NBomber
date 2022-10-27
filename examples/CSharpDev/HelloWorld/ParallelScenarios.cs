using System;
using System.Threading.Tasks;
using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharpDev.HelloWorld;

public class ParallelScenarios
{
    public void Run()
    {
        var scenario1 = Scenario.Create("scenario_1", async context =>
        {
            await Task.Delay(1000);
            return Response.Ok(statusCode: "300", sizeBytes: 1000);
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.KeepConstant(copies: 10, TimeSpan.FromMinutes(1))
        );

        var scenario2 = Scenario.Create("scenario_2", async context =>
        {
            var step1 = await Step.Run("step_1", context, async () =>
            {
                await Task.Delay(1000);
                return Response.Ok(payload: "step_1 response", sizeBytes: 1000);
            });

            return Response.Ok(statusCode: "200");
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.RampConstant(copies: 50, during: TimeSpan.FromMinutes(1)),
            Simulation.KeepConstant(copies: 50, during: TimeSpan.FromSeconds(30))
        );

        NBomberRunner
            .RegisterScenarios(scenario1, scenario2) // here, we register two scenarios that will run in parallel
            .Run();
    }
}
