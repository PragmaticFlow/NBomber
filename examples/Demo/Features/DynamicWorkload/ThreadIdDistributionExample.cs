using NBomber.CSharp;

namespace Demo.Features.DynamicWorkload;

public class ThreadIdDistributionExample
{
    public void Run()
    {
        var scenario = Scenario.Create("home_page", async context =>
        {
            if (context.ScenarioInfo.ThreadNumber % 9 < 3)
            {
                // 0-2 range, run step 1
                await Step.Run("step_1", context, async () =>
                {
                    await Task.Delay(100);
                    return Response.Ok();
                });
            }
            else if (context.ScenarioInfo.ThreadNumber % 9 < 6)
            {
                // 3-5 range, run step 2
                await Step.Run("step_2", context, async () =>
                {
                    await Task.Delay(100);
                    return Response.Ok();
                });
            }
            else
            {
                // 6-8 range, run step 3
                await Step.Run("step_3", context, async () =>
                {
                    await Task.Delay(100);
                    return Response.Ok();
                });
            }

            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.KeepConstant(copies: 9, during: TimeSpan.FromSeconds(30))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
