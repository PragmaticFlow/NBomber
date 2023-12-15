using NBomber.CSharp;

namespace Demo.Features.DynamicWorkload;

public class UniformDistributionExample
{
    public void Run()
    {
        var scenario = Scenario.Create("uniform_distribution", async context =>
        {
            // context.Random: System.Random
            var stepNumber = context.Random.Next(minValue: 1, maxValue: 4);
            if (stepNumber == 1)
            {
                await Step.Run("step_1", context, async () =>
                {
                    await Task.Delay(100);
                    return Response.Ok();
                });
            }
            else if (stepNumber == 2)
            {
                await Step.Run("step_2", context, async () =>
                {
                    await Task.Delay(100);
                    return Response.Ok();
                });
            }
            else
            {
                await Step.Run("step_3", context, async () =>
                {
                    await Task.Delay(100);
                    return Response.Ok();
                });
            }

            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(30)));

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
