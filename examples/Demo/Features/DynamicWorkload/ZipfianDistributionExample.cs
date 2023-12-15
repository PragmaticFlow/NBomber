using MathNet.Numerics.Distributions;
using NBomber.CSharp;

namespace Demo.Features.DynamicWorkload;

public class ZipfianDistributionExample
{
    public void Run()
    {
        var scenario = Scenario.Create("zipfian_distribution", async context =>
        {
            var stepNumber = Zipf.Sample(s: 1.3, n: 5, rnd: context.Random);
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
            else if (stepNumber == 3)
            {
                await Step.Run("step_3", context, async () =>
                {
                    await Task.Delay(100);
                    return Response.Ok();
                });
            }
            else if (stepNumber == 4)
            {
                await Step.Run("step_4", context, async () =>
                {
                    await Task.Delay(100);
                    return Response.Ok();
                });
            }
            else
            {
                await Step.Run("step_5", context, async () =>
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
