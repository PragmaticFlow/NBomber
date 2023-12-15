using MathNet.Numerics.Distributions;
using NBomber.CSharp;

namespace Demo.Features.DynamicWorkload;

public class MultinomialDistributionExample
{
    public void Run()
    {
        var ratios = new[] {0.7, 0.2, 0.1}; // 70%, 20%, 10%

        var scenario = Scenario.Create("multinomial_distribution", async context =>
        {
            int[] result = Multinomial.Sample(context.Random, ratios, 1);
            if (result[0] == 1) // 70% for read
            {
                await Step.Run("read", context, async () =>
                {
                    await Task.Delay(100);
                    return Response.Ok();
                });
            }
            else if (result[1] == 1) // 20% for write
            {
                await Step.Run("write", context, async () =>
                {
                    await Task.Delay(100);
                    return Response.Ok();
                });
            }
            else // 10% for delete
            {
                await Step.Run("delete", context, async () =>
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
